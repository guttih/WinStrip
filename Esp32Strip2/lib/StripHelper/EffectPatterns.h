#ifndef EFFECT_PATTERNS_H
#define EFFECT_PATTERNS_H

#include <cstddef>
#include <cstdint>

namespace EffectPatterns {
inline uint32_t mix( uint32_t value )
{
    value ^= value >> 16;
    value *= 0x7feb352dU;
    value ^= value >> 15;
    value *= 0x846ca68bU;
    return value ^ ( value >> 16 );
}

inline size_t gcd( size_t a, size_t b )
{
    while( b ) { size_t next = a % b; a = b; b = next; }
    return a;
}

// An affine permutation gives exactly N distinct sparkle positions without
// allocating memory or retrying random positions until they are unique.
template <typename Pixel>
void frost( Pixel *pixels, size_t count, uint32_t elapsed, uint32_t seed,
            size_t sparkles, uint32_t flashMs, uint32_t pauseMs,
            Pixel background, Pixel highlight )
{
    if( count == 0 ) return;
    for( size_t i = 0; i < count; ++i ) pixels[ i ] = background;
    flashMs = flashMs == 0 ? 40 : ( flashMs > 60000 ? 60000 : flashMs );
    pauseMs = pauseMs > 60000 ? 60000 : pauseMs;
    uint32_t period = flashMs + pauseMs;
    if( elapsed % period >= flashMs ) return;
    if( sparkles > count ) sparkles = count;
    uint32_t random = mix( seed + elapsed / period );
    size_t position = random % count;
    size_t stride = mix( random ) % count;
    while( gcd( stride, count ) != 1 ) stride = ( stride + 1 ) % count;
    for( size_t i = 0; i < sparkles; ++i )
    {
        pixels[ position ] = highlight;
        position = ( position + stride ) % count;
    }
}

inline uint8_t triangle( uint32_t phase )
{
    phase &= 511;
    return static_cast<uint8_t>( phase < 256 ? phase : 511 - phase );
}

inline uint8_t blendChannel( uint8_t background, uint8_t color, uint32_t amount )
{
    return static_cast<uint8_t>( ( background * ( 255 - amount ) + color * amount ) / 255 );
}

template <typename Pixel>
void aurora( Pixel *pixels, size_t count, uint32_t tick, size_t width,
             uint32_t shimmer, size_t ribbons, const Pixel *palette )
{
    if( count == 0 ) return;
    width = width == 0 ? ( count / 3 + 1 ) : width;
    if( width > count ) width = count;
    shimmer = shimmer > 255 ? 255 : shimmer;
    ribbons = ribbons == 0 ? 3 : ( ribbons > 8 ? 8 : ribbons );
    for( size_t i = 0; i < count; ++i )
    {
        Pixel color = palette[ 0 ];
        for( size_t ribbon = 0; ribbon < ribbons; ++ribbon )
        {
            // Ribbons drift in opposite directions and breathe at different rates.
            uint32_t phase = tick * ( ribbon % 3 + 1 ) + ribbon * 173;
            size_t center = static_cast<size_t>( triangle( phase ) ) * ( count - 1 ) / 255;
            size_t distance = i > center ? i - center : center - i;
            if( distance >= width ) continue;
            uint32_t amount = ( width - distance ) * 255 / width;
            amount = amount * amount / 255; // Soft edges, no moving hard boundary.
            uint32_t breath = 128 + triangle( tick + ribbon * 91 ) / 2;
            uint32_t flicker = mix( static_cast<uint32_t>( i ) + tick / 3 + ribbon * 37 ) & 255;
            uint32_t modulation = 255 - shimmer + flicker * shimmer / 255;
            amount = amount * breath / 255 * modulation / 255;
            Pixel tint = palette[ 1 + ribbon % 3 ];
            color = Pixel( blendChannel( color.r, tint.r, amount ),
                           blendChannel( color.g, tint.g, amount ),
                           blendChannel( color.b, tint.b, amount ) );
        }
        pixels[ i ] = color;
    }
}
}

#endif
