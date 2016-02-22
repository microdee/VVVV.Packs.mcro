#define MCPSWORLD_FXH

/*
indicate optional features with:
KNOW_COLOR
KNOW_SIZE
KNOW_SCALE
KNOW_ROTATION
*/

// Offsets
#if !defined(OFFS_VELOCITY)
#define OFFS_VELOCITY 3
#endif
#if !defined(OFFS_FORCE)
#define OFFS_FORCE 7
#endif
#if !defined(OFFS_AGE)
#define OFFS_AGE 10
#endif
#if !defined(OFFS_COLOR)
#define OFFS_COLOR 12
#endif
#if !defined(OFFS_SIZE)
#define OFFS_SIZE 16
#endif
#if !defined(OFFS_SCALE)
#define OFFS_SCALE 17
#endif
#if !defined(OFFS_ROTATION)
#define OFFS_ROTATION 20
#endif

// Immutables
// Num of particles
#if !defined(PCOUNT)
#define PCOUNT 512
#endif
// Num of floats in a particle
#if !defined(PELSIZE)
#define PELSIZE 24
#endif
// total size of mcps RawBuffer in bytes (PCOUNT * PELSIZE * 4)
#if !defined(BUFSIZE)
#define BUFSIZE 49152
#endif

cbuffer mcpsUniforms
{
    float2 mcpsTime : PS_TIME;
    float WorldEmitOffset : PS_WORLDEMITTEROFFSET;
};
