#include "BluetoothSession.h"

#include <cstdlib>
#include <iostream>
#include <vector>

#define CHECK( condition ) do { if( !( condition ) ) { \
    std::cerr << __func__ << ':' << __LINE__ << ": " << #condition << '\n'; \
    std::exit( 1 ); } } while( false )

void reconnectBetweenLoopIterations()
{
    BluetoothSession session;
    CHECK( !session.isCurrent( session.getGeneration() ) );
    CHECK( session.open( 11 ) );
    uint32_t previous = session.getGeneration();
    const uint8_t partial[] = { 'S', 'T', 'A' };
    session.receive( 11, partial, sizeof( partial ) );
    CHECK( session.read( previous ) == 'S' );
    CHECK( session.write( previous, partial, sizeof( partial ) ) == sizeof( partial ) );
    session.close( 11 );
    CHECK( session.open( 22 ) );
    uint32_t current = session.getGeneration();
    CHECK( current != previous );
    CHECK( session.isCurrent( current ) );
    CHECK( !session.isCurrent( previous ) );
    CHECK( session.available( previous ) == 0 );
    CHECK( session.read( previous ) == -1 );
    CHECK( session.peek( previous ) == -1 );
    CHECK( session.write( previous, partial, sizeof( partial ) ) == 0 );
    uint8_t packet[ BluetoothSession::PACKET_SIZE ];
    uint32_t handle = 0, generation = 0;
    CHECK( session.nextPacket( 100, packet, handle, generation ) == 0 );
    session.receive( 22, partial, sizeof( partial ) );
    CHECK( session.available( current ) == 3 ); // New client's first bytes survive.
    CHECK( session.read( current ) == 'S' );
    CHECK( session.read( current ) == 'T' );
    CHECK( session.read( current ) == 'A' );
    CHECK( session.read( current ) == -1 );
}

void rejectedAndStaleHandles()
{
    BluetoothSession session;
    CHECK( !session.open( 0 ) );
    CHECK( session.open( 11 ) );
    uint32_t generation = session.getGeneration();
    CHECK( !session.open( 22 ) );
    const uint8_t data[] = { 'X' };
    session.receive( 22, data, 1 );
    session.close( 22 );
    session.setCongested( 22, true, 0 );
    session.writeComplete( 22, false, true, 0 );
    session.fail( 22, generation );
    CHECK( session.isCurrent( generation ) );
    CHECK( session.available( generation ) == 0 );
    session.close( 11 );
    CHECK( session.open( 33 ) );
    uint32_t current = session.getGeneration();
    session.close( 11 );
    session.writeComplete( 11, false, true, 0 );
    session.fail( 11, generation );
    CHECK( session.isCurrent( current ) );
}

void receiveBufferWraparound()
{
    BluetoothSession session;
    session.open( 11 );
    uint32_t generation = session.getGeneration();
    std::vector<uint8_t> bytes( BluetoothSession::BUFFER_SIZE );
    for( size_t i = 0; i < bytes.size(); ++i )
        bytes[ i ] = static_cast<uint8_t>( i );
    session.receive( 11, bytes.data(), bytes.size() );
    CHECK( session.available( generation ) == bytes.size() );
    for( size_t i = 0; i < 3000; ++i )
        CHECK( session.read( generation ) == bytes[ i ] );
    session.receive( 11, bytes.data(), 3000 );
    for( size_t i = 3000; i < bytes.size(); ++i )
        CHECK( session.read( generation ) == bytes[ i ] );
    for( size_t i = 0; i < 3000; ++i )
        CHECK( session.read( generation ) == bytes[ i ] );
    CHECK( session.read( generation ) == -1 );
    CHECK( session.isCurrent( generation ) );
}

void receiveOverflowDisconnects()
{
    BluetoothSession session;
    session.open( 11 );
    uint32_t generation = session.getGeneration();
    std::vector<uint8_t> bytes( BluetoothSession::BUFFER_SIZE + 1, 'X' );
    session.receive( 11, bytes.data(), bytes.size() );
    CHECK( !session.isCurrent( generation ) );
    CHECK( session.available( session.getGeneration() ) == 0 );
    CHECK( session.takeDisconnectRequest() == 11 );
    CHECK( session.takeDisconnectRequest() == 0 );
    CHECK( !session.open( 22 ) ); // Wait for the close event before accepting.
    session.close( 11 );
    CHECK( session.open( 22 ) );
    generation = session.getGeneration();
    session.receive( 22, bytes.data(), 1 );
    CHECK( session.read( generation ) == 'X' );
}

void asynchronousPacketsAndCongestion()
{
    BluetoothSession session;
    session.open( 11 );
    uint32_t generation = session.getGeneration();
    std::vector<uint8_t> bytes( 3000 );
    for( size_t i = 0; i < bytes.size(); ++i )
        bytes[ i ] = static_cast<uint8_t>( i );
    CHECK( session.write( generation, bytes.data(), bytes.size() ) == bytes.size() );
    uint8_t packet[ BluetoothSession::PACKET_SIZE ];
    uint32_t handle = 0, targetGeneration = 0;
    CHECK( session.nextPacket( 10, packet, handle, targetGeneration ) == 256 );
    CHECK( handle == 11 && targetGeneration == generation );
    std::vector<uint8_t> actual( packet, packet + 256 );
    CHECK( session.nextPacket( 11, packet, handle, targetGeneration ) == 0 );
    session.writeComplete( 11, true, true, 12 );
    CHECK( session.nextPacket( 13, packet, handle, targetGeneration ) == 0 );
    session.setCongested( 11, false, 14 );
    for( uint32_t now = 15; actual.size() < bytes.size(); ++now )
    {
        size_t count = session.nextPacket( now, packet, handle, targetGeneration );
        CHECK( count > 0 && count <= 256 );
        actual.insert( actual.end(), packet, packet + count );
        session.writeComplete( handle, true, false, now );
    }
    CHECK( actual == bytes );
    CHECK( session.nextPacket( 100, packet, handle, targetGeneration ) == 0 );
}

void transmitOverflowAndErrors()
{
    for( int failure = 0; failure < 3; ++failure )
    {
        BluetoothSession session;
        session.open( 11 );
        uint32_t generation = session.getGeneration();
        std::vector<uint8_t> bytes( BluetoothSession::BUFFER_SIZE, 'X' );
        CHECK( session.write( generation, bytes.data(), bytes.size() ) == bytes.size() );
        if( failure == 0 )
            CHECK( session.write( generation, bytes.data(), 1 ) == 0 );
        else if( failure == 1 )
            session.writeComplete( 11, false, false, 100 );
        else
            session.fail( 11, generation ); // Immediate esp_spp_write failure.
        CHECK( !session.isCurrent( generation ) );
        CHECK( session.takeDisconnectRequest() == 11 );
    }
}

void timeoutsAcrossRollover()
{
    for( bool congestion : { false, true } )
    {
        BluetoothSession session;
        session.open( 11 );
        uint32_t generation = session.getGeneration();
        const uint8_t bytes[] = { 'O', 'K' };
        CHECK( session.write( generation, bytes, sizeof( bytes ) ) == sizeof( bytes ) );
        uint8_t packet[ BluetoothSession::PACKET_SIZE ];
        uint32_t handle = 0, targetGeneration = 0;
        uint32_t start = UINT32_MAX - 100;
        if( congestion )
            session.setCongested( 11, true, start );
        else
            CHECK( session.nextPacket( start, packet, handle, targetGeneration ) == 2 );
        CHECK( session.nextPacket( start + 4999, packet, handle, targetGeneration ) == 0 );
        CHECK( session.isCurrent( generation ) );
        CHECK( session.nextPacket( start + 5000, packet, handle, targetGeneration ) == 0 );
        CHECK( !session.isCurrent( generation ) );
        CHECK( session.takeDisconnectRequest() == 11 );
    }
}

int main()
{
    const struct { const char *name; void ( *run )(); } tests[] = {
        { "Bluetooth reconnect between loop iterations", reconnectBetweenLoopIterations },
        { "Bluetooth rejected/stale handles", rejectedAndStaleHandles },
        { "Bluetooth receive queue wraparound", receiveBufferWraparound },
        { "Bluetooth receive overflow disconnect", receiveOverflowDisconnects },
        { "Bluetooth asynchronous replies and congestion", asynchronousPacketsAndCongestion },
        { "Bluetooth transmit overflow and errors", transmitOverflowAndErrors },
        { "Bluetooth write timeouts and millis rollover", timeoutsAcrossRollover }
    };
    for( const auto &test : tests )
    {
        test.run();
        std::cout << "PASS: " << test.name << '\n';
    }
}
