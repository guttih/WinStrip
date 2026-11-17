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

inline uint8_t addChannel( uint8_t background, uint8_t highlight )
{
    unsigned int sum = static_cast<unsigned int>( background ) + highlight;
    return static_cast<uint8_t>( sum > 255 ? 255 : sum );
}

// Independent, staggered sparkle slots. Disjoint pixel regions prevent
// collisions without allocations. Every unlit pixel stays at background.
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
    if( sparkles > count ) sparkles = count;
    if( sparkles == 0 ) return;
    Pixel light( addChannel( background.r, highlight.r ),
                 addChannel( background.g, highlight.g ),
                 addChannel( background.b, highlight.b ) );
    for( size_t slot = 0; slot < sparkles; ++slot )
    {
        uint32_t bucketStart = static_cast<uint64_t>( slot ) * period / sparkles;
        uint32_t bucketEnd = static_cast<uint64_t>( slot + 1 ) * period / sparkles;
        uint32_t bucketWidth = bucketEnd - bucketStart;
        uint32_t random = mix( seed + static_cast<uint32_t>( slot ) * 0x9e3779b9U );
        uint32_t offset = bucketStart + ( bucketWidth ? random % bucketWidth : 0 );
        uint64_t localTime = static_cast<uint64_t>( elapsed ) + offset;
        if( localTime % period >= flashMs ) continue;

        size_t first = slot * count / sparkles;
        size_t end = ( slot + 1 ) * count / sparkles;
        uint32_t cycle = static_cast<uint32_t>( localTime / period );
        size_t position = first + mix( random ^ mix( cycle ) ) % ( end - first );
        pixels[ position ] = light;
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
