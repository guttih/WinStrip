#ifndef HOST_TEST_STREAM_H
#define HOST_TEST_STREAM_H

#include <cstddef>
#include <cstdint>

// Minimal Arduino Stream/Print surface used by the production SerialReader.
// The firmware build uses the real framework header instead of this test shim.
class Stream
{
public:
    virtual ~Stream() = default;
    virtual int available() = 0;
    virtual int read() = 0;
    virtual int peek() = 0;
    virtual size_t write( uint8_t value ) = 0;
    virtual size_t write( const uint8_t *buffer, size_t size ) = 0;
};

#endif
