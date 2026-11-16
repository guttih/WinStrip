#include "SerialReader.h"

#include <cstdlib>
#include <deque>
#include <iostream>
#include <string>
#include <vector>

#define CHECK( condition ) do { if( !( condition ) ) { \
    std::cerr << __func__ << ':' << __LINE__ << ": " << #condition << '\n'; \
    std::exit( 1 ); } } while( false )

using Result = SerialReader::ReadResult;

class FakeStream : public Stream
{
public:
    std::deque<uint8_t> input;
    std::string output;
    bool failNextRead = false;

    void receive( const std::string &bytes )
    {
        input.insert( input.end(), bytes.begin(), bytes.end() );
    }

    int available() override { return static_cast<int>( input.size() ); }
    int peek() override { return input.empty() ? -1 : input.front(); }
    int read() override
    {
        if( failNextRead )
        {
            failNextRead = false;
            return -1;
        }
        int value = peek();
        if( value >= 0 )
            input.pop_front();
        return value;
    }

    size_t write( uint8_t value ) override
    {
        output += static_cast<char>( value );
        return 1;
    }

    size_t write( const uint8_t *buffer, size_t size ) override
    {
        output.append( reinterpret_cast<const char *>( buffer ), size );
        return size;
    }
};

void collectAvailable( FakeStream &stream, SerialReader &reader,
                       std::vector<std::string> &messages, size_t byteBudget = 256 )
{
    while( stream.available() )
    {
        if( reader.readMessage( byteBudget ) == Result::MessageReady )
        {
            messages.emplace_back( reader.getMessage() );
            reader.resetBuffer();
        }
    }
}

void splitMessages()
{
    FakeStream stream;
    SerialReader reader( stream );
    stream.receive( "STA" );
    CHECK( reader.readMessage() == Result::Incomplete );
    CHECK( reader.readMessage() == Result::Incomplete );
    stream.receive( "TUS" );
    CHECK( reader.readMessage() == Result::Incomplete );
    stream.receive( "\n" );
    CHECK( reader.readMessage() == Result::MessageReady );
    CHECK( std::string( reader.getMessage() ) == "STATUS" );
}

void multipleMessages()
{
    FakeStream stream;
    SerialReader reader( stream );
    stream.receive( "STATUS\nPIXELCOUNT\nALLSTATUS\n" );
    CHECK( reader.readMessage() == Result::MessageReady );
    CHECK( std::string( reader.getMessage() ) == "STATUS" );
    int unread = stream.available();
    CHECK( unread > 0 );
    CHECK( reader.readMessage() == Result::MessageReady );
    CHECK( stream.available() == unread );
    reader.resetBuffer();
    CHECK( reader.readMessage() == Result::MessageReady );
    CHECK( std::string( reader.getMessage() ) == "PIXELCOUNT" );
    reader.resetBuffer();
    CHECK( reader.readMessage() == Result::MessageReady );
    CHECK( std::string( reader.getMessage() ) == "ALLSTATUS" );
    CHECK( stream.available() == 0 );
}

void allFragmentBoundaries()
{
    const std::string bytes = "STA@\r\nTUS\n{\"colors\":[1,@\n2]}\r\nALLSTATUS\n";
    const std::vector<std::string> expected = { "STATUS", "{\"colors\":[1,2]}", "ALLSTATUS" };
    for( size_t split = 0; split <= bytes.size(); ++split )
    {
        FakeStream stream;
        SerialReader reader( stream );
        std::vector<std::string> actual;
        stream.receive( bytes.substr( 0, split ) );
        collectAvailable( stream, reader, actual, 3 );
        stream.receive( bytes.substr( split ) );
        collectAvailable( stream, reader, actual, 3 );
        CHECK( actual == expected );
    }
}

void oneByteAtATime()
{
    FakeStream stream;
    SerialReader reader( stream );
    std::vector<std::string> messages;
    const std::string bytes = "@\nSTA@\r\nTUS\r\n\nX@@\nY\n";
    for( char value : bytes )
    {
        stream.receive( std::string( 1, value ) );
        collectAvailable( stream, reader, messages, 1 );
    }
    CHECK( messages == std::vector<std::string>( { "STATUS", "", "X@Y" } ) );
}

void carriageReturns()
{
    FakeStream stream;
    SerialReader reader( stream );
    std::vector<std::string> messages;
    stream.receive( "STA\rTUS\nX@\rY\nZ\r\r\n" );
    collectAvailable( stream, reader, messages );
    CHECK( messages == std::vector<std::string>( { "STA\rTUS", "X@\rY", "Z\r" } ) );
}

void independentConnections()
{
    FakeStream usb, bluetooth;
    SerialReader usbReader( usb ), bluetoothReader( bluetooth );
    usb.receive( "STA@\n" );
    CHECK( usbReader.readMessage() == Result::Incomplete );
    bluetooth.receive( "PIXELCOUNT\n" );
    CHECK( bluetoothReader.readMessage() == Result::MessageReady );
    CHECK( std::string( bluetoothReader.getMessage() ) == "PIXELCOUNT" );
    bluetoothReader.printLine( "62" );
    CHECK( bluetooth.output == "62\r\n" );
    CHECK( usb.output.empty() );
    usb.receive( "TUS\n" );
    CHECK( usbReader.readMessage() == Result::MessageReady );
    CHECK( std::string( usbReader.getMessage() ) == "STATUS" );
    usbReader.printLine( "OK" );
    CHECK( usb.output == "OK\r\n" );
    CHECK( bluetooth.output == "62\r\n" );
}

void boundedReads()
{
    FakeStream stream;
    SerialReader reader( stream );
    stream.receive( std::string( 1000, 'x' ) );
    CHECK( reader.readMessage( 0 ) == Result::Incomplete );
    CHECK( stream.available() == 1000 );
    CHECK( reader.readMessage() == Result::Incomplete );
    CHECK( stream.available() == 1000 - 256 );
    CHECK( reader.readMessage( 7 ) == Result::Incomplete );
    CHECK( stream.available() == 1000 - 256 - 7 );
}

void fullLengthMessages()
{
    for( const std::string &suffix : { std::string( "\n" ), std::string( "\r\n" ),
                                      std::string( "@\r\n\n" ) } )
    {
        FakeStream stream;
        SerialReader reader( stream );
        std::vector<std::string> messages;
        std::string payload( reader.getMaxLength(), 'x' );
        CHECK( reader.getMaxLength() == 3300 );
        CHECK( reader.getSeparator() == '@' );
        stream.receive( payload + suffix );
        collectAvailable( stream, reader, messages );
        CHECK( messages == std::vector<std::string>( { payload } ) );
    }
}

void oversizedMessages()
{
    FakeStream stream;
    SerialReader reader( stream );
    stream.receive( std::string( reader.getMaxLength() + 1, 'x' ) );
    while( stream.available() )
        CHECK( reader.readMessage() == Result::Incomplete );
    CHECK( reader.readMessage() == Result::Incomplete );
    stream.receive( "@\r\nSTATUS@\n\r\nPIXELCOUNT\n" );
    CHECK( reader.readMessage() == Result::MessageDiscarded );
    CHECK( std::string( reader.getMessage() ).empty() );
    CHECK( reader.readMessage() == Result::MessageReady );
    CHECK( std::string( reader.getMessage() ) == "PIXELCOUNT" );
}

void limitAcrossContinuations()
{
    FakeStream stream;
    SerialReader reader( stream );
    stream.receive( std::string( 2000, 'x' ) + "@\n" + std::string( 1301, 'y' ) + "\nSTATUS\n" );
    Result result = Result::Incomplete;
    while( result == Result::Incomplete )
        result = reader.readMessage();
    CHECK( result == Result::MessageDiscarded );
    CHECK( reader.readMessage() == Result::MessageReady );
    CHECK( std::string( reader.getMessage() ) == "STATUS" );
}

void rejectEmbeddedNul()
{
    FakeStream stream;
    SerialReader reader( stream );
    stream.receive( std::string( "STATUS" ) + '\0' + "junk@\nPIXELCOUNT\nSTATUS\n" );
    CHECK( reader.readMessage() == Result::MessageDiscarded );
    CHECK( reader.readMessage() == Result::MessageReady );
    CHECK( std::string( reader.getMessage() ) == "STATUS" );
}

void resetOnDisconnect()
{
    FakeStream stream;
    SerialReader reader( stream );
    for( const std::string &partial : { std::string( "STA@\r" ), std::string( 4000, 'x' ) } )
    {
        stream.receive( partial );
        while( stream.available() )
            CHECK( reader.readMessage() == Result::Incomplete );
        reader.resetBuffer();
        stream.receive( "STATUS\n" );
        CHECK( reader.readMessage() == Result::MessageReady );
        CHECK( std::string( reader.getMessage() ) == "STATUS" );
        reader.resetBuffer();
    }
}

void noByteAvailableAfterCheck()
{
    FakeStream stream;
    SerialReader reader( stream );
    stream.receive( "STATUS\n" );
    stream.failNextRead = true;
    CHECK( reader.readMessage() == Result::Incomplete );
    CHECK( reader.readMessage() == Result::MessageReady );
    CHECK( std::string( reader.getMessage() ) == "STATUS" );
}

void replyFraming()
{
    FakeStream stream;
    SerialReader reader( stream );
    reader.printLine( "" );
    CHECK( stream.output == "\r\n" );
    stream.output.clear();
    reader.printLine( "3300" );
    CHECK( stream.output == "3300\r\n" );
    stream.output.clear();
    reader.printLine( "@" );
    CHECK( stream.output == "@\r\n" );
    stream.output.clear();
    reader.printLine( std::string( 255, 'x' ).c_str() );
    CHECK( stream.output == std::string( 255, 'x' ) + "\r\n" );
    stream.output.clear();
    reader.printLine( std::string( 256, 'x' ).c_str() );
    CHECK( stream.output == std::string( 255, 'x' ) + "@\r\nx\r\n" );
    stream.output.clear();
    reader.printLine( std::string( 510, 'x' ).c_str() );
    CHECK( stream.output == std::string( 255, 'x' ) + "@\r\n" + std::string( 255, 'x' ) + "\r\n" );
}

void longReplyRoundTrip()
{
    FakeStream sender, receiver;
    SerialReader writer( sender ), reader( receiver );
    const std::string payload = "{\"programs\":[\"" + std::string( 3000, 'x' ) + "\"]}";
    writer.printLine( payload.c_str() );
    CHECK( sender.output.find( "@\r\n" ) != std::string::npos );
    std::vector<std::string> messages;
    for( char value : sender.output )
    {
        receiver.receive( std::string( 1, value ) );
        collectAvailable( receiver, reader, messages );
    }
    CHECK( messages == std::vector<std::string>( { payload } ) );
}

int main()
{
    const struct { const char *name; void ( *run )(); } tests[] = {
        { "split messages", splitMessages },
        { "multiple messages in one read", multipleMessages },
        { "all fragment boundaries", allFragmentBoundaries },
        { "one byte at a time", oneByteAtATime },
        { "CRLF and embedded CR", carriageReturns },
        { "independent connections and reply routing", independentConnections },
        { "bounded reads", boundedReads },
        { "exact buffer limit", fullLengthMessages },
        { "oversized message recovery", oversizedMessages },
        { "limit across continuation chunks", limitAcrossContinuations },
        { "embedded NUL rejection", rejectEmbeddedNul },
        { "disconnect reset", resetOnDisconnect },
        { "read unavailable after availability check", noByteAvailableAfterCheck },
        { "reply framing", replyFraming },
        { "long reply round trip", longReplyRoundTrip }
    };
    for( const auto &test : tests )
    {
        test.run();
        std::cout << "PASS: " << test.name << '\n';
    }
}
