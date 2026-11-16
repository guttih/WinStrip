#ifndef ANIMATION_CLOCK_H
#define ANIMATION_CLOCK_H

#include <cstdint>

// Unsigned elapsed times remain valid across millis() rollover.
class AnimationClock
{
public:
    void reset( uint32_t now )
    {
        lastFrame = now;
        pauseDuration = 0;
    }

    void pause( uint32_t now, uint32_t duration )
    {
        pauseStarted = now;
        pauseDuration = duration;
    }

    bool ready( uint32_t now, uint32_t interval )
    {
        if( static_cast<uint32_t>( now - pauseStarted ) < pauseDuration )
            return false;
        pauseDuration = 0;
        if( static_cast<uint32_t>( now - lastFrame ) < interval )
            return false;
        lastFrame = now;
        return true;
    }

private:
    uint32_t lastFrame = 0;
    uint32_t pauseStarted = 0;
    uint32_t pauseDuration = 0;
};

#endif
