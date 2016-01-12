#if !defined(BYTEADDRESSBUFFERUTILS_FXH)
#include "../../../mp.fxh/ByteAddressBufferUtils.fxh"
#endif
#if !defined(MUPSWORLD_FXH)
#include "../../../mp.fxh/mupsWorld.fxh"
#endif

float3 mupsPositionLoad(ByteAddressBuffer mupsb, uint i) { return BABLoad3(mupsb, i*PELSIZE); }
float4 mupsVelocityLoad(ByteAddressBuffer mupsb, uint i) { return BABLoad4(mupsb, i*PELSIZE + OFFS_VELOCITY); }
float3 mupsForceLoad(ByteAddressBuffer mupsb, uint i)    { return BABLoad3(mupsb, i*PELSIZE + OFFS_FORCE); }
float4 mupsColorLoad(ByteAddressBuffer mupsb, uint i)    { return BABLoad4(mupsb, i*PELSIZE + OFFS_COLOR); }
float  mupsSizeLoad(ByteAddressBuffer mupsb, uint i)     { return BABLoad(mupsb, i*PELSIZE + OFFS_SIZE); }
float2 mupsAgeLoad(ByteAddressBuffer mupsb, uint i)      { return BABLoad2(mupsb, i*PELSIZE + OFFS_AGE); }
float3 mupsScaleLoad(ByteAddressBuffer mupsb, uint i)    { return BABLoad3(mupsb, i*PELSIZE + OFFS_SCALE); }
float4 mupsRotationLoad(ByteAddressBuffer mupsb, uint i) { return BABLoad4(mupsb, i*PELSIZE + OFFS_ROTATION); }

float mupsLoad(ByteAddressBuffer mupsb, uint i, uint o) { return BABLoad(mupsb, i*PELSIZE + o );}
float2 mupsLoad2(ByteAddressBuffer mupsb, uint i, uint o) { return BABLoad2(mupsb, i*PELSIZE + o );}
float3 mupsLoad3(ByteAddressBuffer mupsb, uint i, uint o) { return BABLoad3(mupsb, i*PELSIZE + o );}
float4 mupsLoad4(ByteAddressBuffer mupsb, uint i, uint o) { return BABLoad4(mupsb, i*PELSIZE + o );}
float4x4 mupsLoad4x4(ByteAddressBuffer mupsb, uint i, uint o) { return BABLoad4x4(mupsb, i*PELSIZE + o );}
