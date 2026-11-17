#include "EffectPatterns.h"
#include <array>
#include <cassert>
#include <iostream>
#include <limits>

struct Pixel {
    uint8_t r = 0, g = 0, b = 0;
    Pixel() = default;
    Pixel( uint8_t r, uint8_t g, uint8_t b ) : r(r), g(g), b(b) {}
    bool operator==( const Pixel &other ) const { return r==other.r && g==other.g && b==other.b; }
    bool operator!=( const Pixel &other ) const { return !( *this == other ); }
};

int main()
{
    std::array<Pixel, 62> pixels;
    Pixel background(12,12,12), highlight(255,255,255);
    auto brightCount = [&]() { size_t n=0; for( auto p:pixels ) if(p==highlight) ++n; return n; };
    for( size_t count=0; count<=70; ++count )
    {
        EffectPatterns::frost(pixels.data(),62,0,123,count,40,80,background,highlight);
        assert(brightCount()==(count>62 ? 62 : count));
    }
    EffectPatterns::frost(pixels.data(),62,39,123,5,40,80,background,highlight);
    assert(brightCount()==5);
    auto first=pixels;
    EffectPatterns::frost(pixels.data(),62,40,123,5,40,80,background,highlight);
    for(auto p:pixels) assert(p==background);
    EffectPatterns::frost(pixels.data(),62,119,123,5,40,80,background,highlight);
    assert(brightCount()==0);
    EffectPatterns::frost(pixels.data(),62,120,123,5,40,80,background,highlight);
    assert(brightCount()==5 && pixels!=first);
    EffectPatterns::frost(pixels.data(),62,999,123,5,1000,2000,background,highlight);
    assert(brightCount()==5);
    EffectPatterns::frost(pixels.data(),62,1000,123,5,1000,2000,background,highlight);
    assert(brightCount()==0);
    std::cout<<"PASS: Frost exact counts, fast/slow flash boundaries, pause and reselection\n";

    // Count one and composite strip sizes exercise permutation termination.
    for(size_t size=1;size<=62;++size) {
        EffectPatterns::frost(pixels.data(),size,0,0,size,1,1,background,highlight);
        for(size_t i=0;i<size;++i) assert(pixels[i]==highlight);
    }
    uint32_t start=UINT32_MAX-10, now=9;
    EffectPatterns::frost(pixels.data(),62,uint32_t(now-start),123,5,40,80,background,highlight);
    assert(brightCount()==5);
    EffectPatterns::frost<Pixel>(nullptr,0,0,0,5,0,0,background,highlight);
    std::cout<<"PASS: Frost short strips, zero length and rollover elapsed time\n";

    Pixel palette[]={Pixel(0,0,8),Pixel(0,255,80),Pixel(0,160,180),Pixel(120,0,200)};
    EffectPatterns::aurora(pixels.data(),62,0,22,0,3,palette);
    first=pixels;
    EffectPatterns::aurora(pixels.data(),62,100,22,0,3,palette);
    assert(pixels!=first);
    for(auto p:pixels) { assert(p.r<=120); assert(p.g<=255); assert(p.b<=200); }
    EffectPatterns::aurora(pixels.data(),62,UINT32_MAX,UINT32_MAX,UINT32_MAX,UINT32_MAX,palette);
    EffectPatterns::aurora(pixels.data(),1,0,0,0,0,palette);
    EffectPatterns::aurora<Pixel>(nullptr,0,0,0,0,0,palette);
    Pixel dark[]={Pixel(0,0,0),Pixel(0,0,0),Pixel(0,0,0),Pixel(0,0,0)};
    EffectPatterns::aurora(pixels.data(),62,50,0,255,0,dark);
    for(auto p:pixels) assert(p==Pixel(0,0,0));
    std::cout<<"PASS: Aurora drift, bounded colors, extreme parameters and empty strips\n";
}
