#ifndef _SERIAL_READER_H
#define _SERIAL_READER_H

#include <Stream.h>

// One instance per connection. Neither input nor replies use the global Serial.
class SerialReader
{
public:
    static constexpr size_t MAX_MESSAGE_LENGTH = 3300;
    static constexpr size_t READ_BYTE_BUDGET = 256;
    static constexpr size_t REPLY_CHUNK_LENGTH = 255;

    enum class ReadResult
    {
        Incomplete,
        MessageReady,
        MessageDiscarded
    };

    explicit SerialReader( Stream &stream );
    ReadResult readMessage( size_t byteBudget = READ_BYTE_BUDGET );
    const char *getMessage() const;
    size_t getMaxLength() const;
    char getSeparator() const;
    // Call after handling a message, or when this connection is lost.
    void resetBuffer();
    void printLine( const char *message );

private:
    void append( char value );

    Stream &stream;
    char inputBuffer[ MAX_MESSAGE_LENGTH + 1 ] = {};
    size_t inputLength = 0;
    bool messageReady = false;
    bool discarding = false;
    // Defer these bytes so framing markers do not consume payload capacity.
    bool pendingSeparator = false;
    bool pendingCarriageReturn = false;
};

#endif // _SERIAL_READER_H
