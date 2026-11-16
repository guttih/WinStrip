#include "AnimationClock.h"

#include <cstdlib>
#include <iostream>

#define CHECK( condition ) do { if( !( condition ) ) { \
    std::cerr << __func__ << ':' << __LINE__ << ": " << #condition << '\n'; \
    std::exit( 1 ); } } while( false )

int main()
{
    AnimationClock clock;
    clock.reset( 10 );
    CHECK( !clock.ready( 109, 100 ) );
    CHECK( clock.ready( 110, 100 ) );
    CHECK( !clock.ready( 110, 100 ) );
    clock.pause( 110, 600 ); // Stars: 500 ms pause + 100 ms frame interval.
    CHECK( !clock.ready( 709, 100 ) );
    CHECK( clock.ready( 710, 100 ) );
    CHECK( !clock.ready( 809, 100 ) );
    CHECK( clock.ready( 810, 100 ) );
    std::cout << "PASS: nonblocking animation intervals and pauses\n";

    clock.pause( 810, 5000 );
    clock.reset( 900 ); // Switching effects cancels the Stars pause.
    CHECK( clock.ready( 930, 30 ) );
    CHECK( clock.ready( 930, 0 ) );
    std::cout << "PASS: animation switch cancels pause\n";

    clock.reset( UINT32_MAX - 49 );
    CHECK( !clock.ready( 49, 100 ) );
    CHECK( clock.ready( 50, 100 ) );
    clock.reset( UINT32_MAX - 49 );
    clock.pause( UINT32_MAX - 49, 200 );
    CHECK( !clock.ready( 149, 100 ) );
    CHECK( clock.ready( 150, 100 ) );
    std::cout << "PASS: animation timing across millis rollover\n";
}
