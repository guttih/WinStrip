#include "TrailPattern.h"
#include <array>
#include <cassert>
#include <iostream>
#include <vector>

struct Pixel {
    uint8_t r=0, g=0, b=0;
    Pixel() = default;
    Pixel(uint8_t r,uint8_t g,uint8_t b):r(r),g(g),b(b){}
    bool operator==(const Pixel &p) const {return r==p.r && g==p.g && b==p.b;}
};

int main()
{
    Pixel light(255,100,50), background(0,10,20);
    const std::vector<std::vector<size_t>> paths={
        {0,1,2,3,2,1,0,1,2}, {0,1,2,3,0,1,2,3,0}, {3,2,1,0,3,2,1,0,3}};
    for(uint32_t mode=0;mode<3;++mode) {
        TrailPattern trail;
        trail.reset(4,mode);
        for(size_t expected:paths[mode]) {
            std::array<Pixel,4> pixels;
            trail.render(pixels.data(),4,0,mode,light,background);
            for(size_t i=0;i<4;++i) assert(pixels[i]==(i==expected ? light : background));
            trail.advance(4,mode);
        }
    }
    std::cout<<"PASS: Trail up, down, bounce and zero-length single pixel\n";

    TrailPattern trail;
    trail.reset(4,1);trail.advance(4,1);trail.advance(4,1);
    std::array<Pixel,4> pixels;
    trail.render(pixels.data(),4,2,1,light,background);
    assert(pixels[2]==light && pixels[3]==background);
    assert(pixels[1].r>pixels[0].r && pixels[0].r>background.r);
    assert(pixels[1].g<light.g && pixels[0].b>=background.b);
    trail.reset(4,2);
    trail.render(pixels.data(),4,2,2,light,background);
    assert(pixels[3]==light && pixels[2]==background);
    assert(pixels[0].r>pixels[1].r && pixels[1].r>background.r);
    std::cout<<"PASS: Trail fading tail follows direction and wraps\n";

    trail.reset(4,0);
    trail.render(pixels.data(),4,UINT32_MAX,0,light,background);
    assert(pixels[0]==light);
    for(size_t i=1;i<4;++i) assert(pixels[i]==background);
    for(int i=0;i<4;++i) trail.advance(4,0);
    trail.render(pixels.data(),4,3,0,light,background);
    assert(pixels[2]==light && pixels[0]==background && pixels[1]==background);
    assert(pixels[3].r>background.r);
    std::cout<<"PASS: Trail bounce endpoints and bounded oversized tails\n";

    for(uint32_t mode=0;mode<3;++mode) {
        trail.reset(1,mode);
        for(int i=0;i<10;++i) {
            trail.render(pixels.data(),1,UINT32_MAX,mode,light,background);
            assert(pixels[0]==light);
            trail.advance(1,mode);
        }
        trail.reset(0,mode);
        trail.render<Pixel>(nullptr,0,0,mode,light,background);
        trail.advance(0,mode);
    }
    trail.reset(4,1);trail.advance(4,1);trail.reset(4,2);
    trail.render(pixels.data(),4,0,2,light,background);
    assert(pixels[3]==light);
    std::cout<<"PASS: Trail empty/single-pixel strips and direction reset\n";
}
