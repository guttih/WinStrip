#include "SerialReader.h"

#include <cstring>

SerialReader::SerialReader( Stream &stream ) : stream( stream )
{
}

void SerialReader::resetBuffer()
{
    inputLength = 0;
    inputBuffer[ 0 ] = '\0';
    messageReady = false;
    discarding = false;
    pendingSeparator = false;
    pendingCarriageReturn = false;
}

void SerialReader::append( char value )
{
    if( discarding )
        return;

    // Reject embedded NULs as well, so a truncated command cannot execute.
    if( value == '\0' || inputLength == MAX_MESSAGE_LENGTH )
    {
        inputLength = 0;
        inputBuffer[ 0 ] = '\0';
        discarding = true;
        return;
    }

    inputBuffer[ inputLength++ ] = value;
    inputBuffer[ inputLength ] = '\0';
}

SerialReader::ReadResult SerialReader::readMessage( size_t byteBudget )
{
    if( messageReady )
        return ReadResult::MessageReady;

    // Return at a logical message boundary or at the budget, whichever is first.
    // This leaves other connections and the animation a chance to run.
    for( size_t count = 0; count < byteBudget && stream.available() > 0; ++count )
    {
        int value = stream.read();
        if( value < 0 )
            break;

        char inChar = static_cast<char>( value );
        if( inChar == '\n' )
        {
            pendingCarriageReturn = false;
            if( pendingSeparator )
            {
                pendingSeparator = false;
                continue;
            }

            if( discarding )
            {
                resetBuffer();
                return ReadResult::MessageDiscarded;
            }

            messageReady = true;
            return ReadResult::MessageReady;
        }

        // Only a CR immediately before LF is framing. Preserve embedded CRs.
        if( pendingCarriageReturn )
        {
            if( pendingSeparator )
            {
                append( getSeparator() );
                pendingSeparator = false;
            }
            append( '\r' );
            pendingCarriageReturn = false;
        }
        if( inChar == '\r' )
        {
            pendingCarriageReturn = true;
            continue;
        }

        if( pendingSeparator )
        {
            append( getSeparator() );
            pendingSeparator = false;
        }

        if( inChar == getSeparator() )
            pendingSeparator = true;
        else
            append( inChar );
    }

    return ReadResult::Incomplete;
}

const char *SerialReader::getMessage() const
{
    return inputBuffer;
}

size_t SerialReader::getMaxLength() const
{
    return MAX_MESSAGE_LENGTH;
}

char SerialReader::getSeparator() const
{
    return '@';
}

void SerialReader::printLine( const char *message )
{
    size_t remaining = std::strlen( message );
    while( remaining > REPLY_CHUNK_LENGTH )
    {
        stream.write( reinterpret_cast<const uint8_t *>( message ), REPLY_CHUNK_LENGTH );
        stream.write( static_cast<uint8_t>( getSeparator() ) );
        stream.write( reinterpret_cast<const uint8_t *>( "\r\n" ), 2 );
        message += REPLY_CHUNK_LENGTH;
        remaining -= REPLY_CHUNK_LENGTH;
    }
    stream.write( reinterpret_cast<const uint8_t *>( message ), remaining );
    stream.write( reinterpret_cast<const uint8_t *>( "\r\n" ), 2 );
}
