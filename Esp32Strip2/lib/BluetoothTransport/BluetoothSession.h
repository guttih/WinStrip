#ifndef BLUETOOTH_SESSION_H
#define BLUETOOTH_SESSION_H

#include <cstddef>
#include <cstdint>

// Hardware-independent session state. The transport serializes access from
// the Bluetooth task and the Arduino loop; no command runs in a BT callback.
class BluetoothSession
{
public:
    static constexpr size_t BUFFER_SIZE = 4096;
    static constexpr size_t PACKET_SIZE = 256;
    static constexpr uint32_t WRITE_TIMEOUT_MS = 5000;

    bool open( uint32_t newHandle )
    {
        if( handle != 0 || newHandle == 0 )
            return false;
        handle = newHandle;
        ++generation;
        clearBuffers();
        failed = false;
        disconnectRequested = false;
        return true;
    }

    void close( uint32_t oldHandle )
    {
        if( handle != oldHandle || handle == 0 )
            return;
        handle = 0;
        ++generation;
        clearBuffers();
    }

    uint32_t getGeneration() const { return generation; }
    bool isCurrent( uint32_t selectedGeneration ) const
    {
        return handle != 0 && !failed && generation == selectedGeneration;
    }

    void receive( uint32_t sourceHandle, const uint8_t *data, size_t length )
    {
        if( sourceHandle != handle || !isCurrent( generation ) )
            return;
        if( !input.push( data, length ) )
            fail( handle, generation );
    }

    size_t available( uint32_t selectedGeneration ) const
    {
        return isCurrent( selectedGeneration ) ? input.size() : 0;
    }
    int peek( uint32_t selectedGeneration ) const
    {
        return isCurrent( selectedGeneration ) ? input.peek() : -1;
    }
    int read( uint32_t selectedGeneration )
    {
        return isCurrent( selectedGeneration ) ? input.pop() : -1;
    }

    size_t write( uint32_t selectedGeneration, const uint8_t *data, size_t length )
    {
        if( !isCurrent( selectedGeneration ) )
            return 0;
        if( !output.push( data, length ) )
        {
            fail( handle, selectedGeneration );
            return 0;
        }
        return length;
    }

    size_t nextPacket( uint32_t now, uint8_t *data, uint32_t &targetHandle,
                       uint32_t &targetGeneration )
    {
        if( !isCurrent( generation ) )
            return 0;
        if( ( writing || congested ) &&
            static_cast<uint32_t>( now - waitStarted ) >= WRITE_TIMEOUT_MS )
        {
            fail( handle, generation );
            return 0;
        }
        if( writing || congested )
            return 0;

        size_t length = 0;
        while( length < PACKET_SIZE && output.size() > 0 )
            data[ length++ ] = static_cast<uint8_t>( output.pop() );
        if( length > 0 )
        {
            writing = true;
            waitStarted = now;
            targetHandle = handle;
            targetGeneration = generation;
        }
        return length;
    }

    void writeComplete( uint32_t targetHandle, bool success, bool isCongested, uint32_t now )
    {
        if( targetHandle != handle || !isCurrent( generation ) )
            return;
        if( !success )
        {
            fail( handle, generation );
            return;
        }
        writing = false;
        setCongested( targetHandle, isCongested, now );
    }

    void setCongested( uint32_t targetHandle, bool value, uint32_t now )
    {
        if( targetHandle != handle || !isCurrent( generation ) )
            return;
        if( value && !congested && !writing )
            waitStarted = now;
        congested = value;
    }

    void fail( uint32_t targetHandle, uint32_t targetGeneration )
    {
        if( targetHandle != handle || !isCurrent( targetGeneration ) )
            return;
        failed = true;
        ++generation;
        clearBuffers();
    }

    uint32_t takeDisconnectRequest()
    {
        if( !failed || handle == 0 || disconnectRequested )
            return 0;
        disconnectRequested = true;
        return handle;
    }

private:
    class ByteQueue
    {
    public:
        size_t size() const { return length; }
        int peek() const { return length > 0 ? bytes[ first ] : -1; }
        int pop()
        {
            int value = peek();
            if( length > 0 )
            {
                first = ( first + 1 ) % BUFFER_SIZE;
                --length;
            }
            return value;
        }
        bool push( const uint8_t *data, size_t count )
        {
            if( count > BUFFER_SIZE - length )
                return false;
            for( size_t i = 0; i < count; ++i )
                bytes[ ( first + length + i ) % BUFFER_SIZE ] = data[ i ];
            length += count;
            return true;
        }
        void clear() { first = length = 0; }

    private:
        uint8_t bytes[ BUFFER_SIZE ] = {};
        size_t first = 0;
        size_t length = 0;
    };

    void clearBuffers()
    {
        input.clear();
        output.clear();
        writing = congested = false;
    }

    ByteQueue input, output;
    uint32_t handle = 0;
    uint32_t generation = 0;
    uint32_t waitStarted = 0;
    bool writing = false;
    bool congested = false;
    bool failed = false;
    bool disconnectRequested = false;
};

#endif
