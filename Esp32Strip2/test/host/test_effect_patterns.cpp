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
    Pixel background(50,30,10), highlight(20,80,3), lit(70,110,13);
    auto brightCount = [&]() { size_t n=0; for( auto p:pixels ) if(p==lit) ++n; return n; };
    // Inspect every pixel, including between flash boundaries: the background
    // must remain exact, and sparkles must never darken any color channel.
    for(size_t slots : {size_t(0),size_t(1),size_t(5),size_t(62),size_t(70)}) {
        size_t maximum=0;
        for(uint32_t time=0;time<240;++time) {
            EffectPatterns::frost(pixels.data(),62,time,123,slots,40,80,background,highlight);
            size_t active=brightCount();
            assert(active <= (slots>62 ? 62 : slots));
            if(active>maximum) maximum=active;
            for(auto p:pixels) {
                assert(p==background || p==lit);
                assert(p.r>=background.r && p.g>=background.g && p.b>=background.b);
            }
        }
        if(slots) assert(maximum>0);
        if(slots==5) assert(maximum<5); // Reject the old synchronized group strobe.
    }
    // A single sparkle has exactly the requested on-time per cycle, with a
    // stable position throughout its flash and exact background restoration.
    for(uint32_t flash : {40U,1000U}) {
        uint32_t pause = flash==40 ? 80 : 2000;
        uint32_t onFrames=0, transition=0;
        bool previous=false;
        for(uint32_t time=0;time<flash+pause+1;++time) {
            EffectPatterns::frost(pixels.data(),62,time,123,1,flash,pause,background,highlight);
            bool on=brightCount()==1;
            if(time<flash+pause && on) ++onFrames;
            if(time && on && !previous) transition=time;
            previous=on;
        }
        assert(onFrames==flash && transition>0);
        EffectPatterns::frost(pixels.data(),62,transition,123,1,flash,pause,background,highlight);
        auto first=pixels;
        EffectPatterns::frost(pixels.data(),62,transition+flash-1,123,1,flash,pause,background,highlight);
        assert(pixels==first);
        EffectPatterns::frost(pixels.data(),62,transition+flash,123,1,flash,pause,background,highlight);
        for(auto p:pixels) assert(p==background);
    }
    std::cout<<"PASS: Frost steady background, independent sparkles and fast/slow timing\n";

    // Full count must still be staggered, rather than flashing the whole strip.
    EffectPatterns::frost(pixels.data(),62,0,123,62,40,80,background,highlight);
    assert(brightCount()>0 && brightCount()<62);
    for(size_t size=1;size<=62;++size) {
        EffectPatterns::frost(pixels.data(),size,UINT32_MAX,0,size,40,80,background,highlight);
        for(size_t i=0;i<size;++i) assert(pixels[i]==background || pixels[i]==lit);
    }
    uint32_t start=UINT32_MAX-10, now=9;
    EffectPatterns::frost(pixels.data(),62,uint32_t(now-start),123,5,40,80,background,highlight);
    auto first=pixels;
    EffectPatterns::frost(pixels.data(),62,20,123,5,40,80,background,highlight);
    assert(pixels==first);
    Pixel high(250,100,200), add(100,200,100);
    EffectPatterns::frost(pixels.data(),62,0,123,62,40,80,high,add);
    for(auto p:pixels) assert(p==high || p==Pixel(255,255,255));
    EffectPatterns::frost<Pixel>(nullptr,0,0,0,5,0,0,background,highlight);
    std::cout<<"PASS: Frost staggered full count, short strips, saturation and rollover\n";

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
