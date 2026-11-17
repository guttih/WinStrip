#ifndef TRAIL_PATTERN_H
#define TRAIL_PATTERN_H

#include "EffectPatterns.h"

class TrailPattern
{
public:
    void reset( size_t count, uint32_t mode )
    {
        position = mode == 2 && count > 0 ? count - 1 : 0;
        direction = mode == 2 ? -1 : 1;
    }

    void advance( size_t count, uint32_t mode )
    {
        if( count <= 1 ) { position = 0; return; }
        if( mode == 1 ) { direction = 1; position = ( position + 1 ) % count; }
        else if( mode == 2 ) { direction = -1; position = ( position + count - 1 ) % count; }
        else
        {
            if( position == count - 1 ) direction = -1;
            else if( position == 0 ) direction = 1;
            position = direction > 0 ? position + 1 : position - 1;
        }
    }

    template <typename Pixel>
    void render( Pixel *pixels, size_t count, size_t length, uint32_t mode,
                 Pixel light, Pixel background ) const
    {
        if( count == 0 ) return;
        for( size_t i = 0; i < count; ++i ) pixels[ i ] = background;
        if( length >= count ) length = count - 1;
        size_t head = position % count;
        for( size_t distance = 1; distance <= length; ++distance )
        {
            // Bouncing trails stay within the strip; one-way trails wrap.
            if( mode == 0 && ( direction > 0 ? distance > head : distance > count - 1 - head ) )
                continue;
            size_t index = direction > 0 ? ( head + count - distance ) % count
                                         : ( head + distance ) % count;
            uint32_t amount = ( length - distance + 1 ) * 255 / ( length + 1 );
            pixels[ index ] = Pixel(
                EffectPatterns::blendChannel( background.r, light.r, amount ),
                EffectPatterns::blendChannel( background.g, light.g, amount ),
                EffectPatterns::blendChannel( background.b, light.b, amount ) );
        }
        pixels[ head ] = light;
    }

private:
    size_t position = 0;
    int direction = 1;
};

#endif
