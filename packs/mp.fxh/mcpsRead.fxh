#if !defined(BYTEADDRESSBUFFERUTILS_FXH)
#include "../../../mp.fxh/ByteAddressBufferUtils.fxh"
#endif
#if !defined(MCPSWORLD_FXH)
#include "../../../mp.fxh/mcpsWorld.fxh"
#endif

float3 mcpsPositionLoad(ByteAddressBuffer mcpsb, uint i) { return BABLoad3(mcpsb, i*PELSIZE); }
float4 mcpsVelocityLoad(ByteAddressBuffer mcpsb, uint i) { return BABLoad4(mcpsb, i*PELSIZE + OFFS_VELOCITY); }
float3 mcpsForceLoad(ByteAddressBuffer mcpsb, uint i)    { return BABLoad3(mcpsb, i*PELSIZE + OFFS_FORCE); }
float4 mcpsColorLoad(ByteAddressBuffer mcpsb, uint i)    { return BABLoad4(mcpsb, i*PELSIZE + OFFS_COLOR); }
float  mcpsSizeLoad(ByteAddressBuffer mcpsb, uint i)     { return BABLoad(mcpsb, i*PELSIZE + OFFS_SIZE); }
float2 mcpsAgeLoad(ByteAddressBuffer mcpsb, uint i)      { return BABLoad2(mcpsb, i*PELSIZE + OFFS_AGE); }
float3 mcpsScaleLoad(ByteAddressBuffer mcpsb, uint i)    { return BABLoad3(mcpsb, i*PELSIZE + OFFS_SCALE); }
float4 mcpsRotationLoad(ByteAddressBuffer mcpsb, uint i) { return BABLoad4(mcpsb, i*PELSIZE + OFFS_ROTATION); }

float mcpsLoad(ByteAddressBuffer mcpsb, uint i, uint o) { return BABLoad(mcpsb, i*PELSIZE + o );}
float2 mcpsLoad2(ByteAddressBuffer mcpsb, uint i, uint o) { return BABLoad2(mcpsb, i*PELSIZE + o );}
float3 mcpsLoad3(ByteAddressBuffer mcpsb, uint i, uint o) { return BABLoad3(mcpsb, i*PELSIZE + o );}
float4 mcpsLoad4(ByteAddressBuffer mcpsb, uint i, uint o) { return BABLoad4(mcpsb, i*PELSIZE + o );}
float4x4 mcpsLoad4x4(ByteAddressBuffer mcpsb, uint i, uint o) { return BABLoad4x4(mcpsb, i*PELSIZE + o );}
