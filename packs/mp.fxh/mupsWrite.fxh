#if !defined(RWBYTEADDRESSBUFFERUTILS_FXH)
#include "../../../mp.fxh/RWByteAddressBufferUtils.fxh"
#endif
#if !defined(MUPSWORLD_FXH)
#include "../../../mp.fxh/mupsWorld.fxh"
#endif

StructuredBuffer<uint> EmitOffset : PS_EMITTEROFFSET;

float3 mupsPositionLoad(RWByteAddressBuffer mupsb, uint i) { return RWBABLoad3(mupsb, i*PELSIZE); }
float4 mupsVelocityLoad(RWByteAddressBuffer mupsb, uint i) { return RWBABLoad4(mupsb, i*PELSIZE + OFFS_VELOCITY); }
float3 mupsForceLoad(RWByteAddressBuffer mupsb, uint i)    { return RWBABLoad3(mupsb, i*PELSIZE + OFFS_FORCE); }
float4 mupsColorLoad(RWByteAddressBuffer mupsb, uint i)    { return RWBABLoad4(mupsb, i*PELSIZE + OFFS_COLOR); }
float  mupsSizeLoad(RWByteAddressBuffer mupsb, uint i)     { return RWBABLoad(mupsb, i*PELSIZE + OFFS_SIZE); }
float2 mupsAgeLoad(RWByteAddressBuffer mupsb, uint i)      { return RWBABLoad2(mupsb, i*PELSIZE + OFFS_AGE); }
float3 mupsScaleLoad(RWByteAddressBuffer mupsb, uint i)    { return RWBABLoad3(mupsb, i*PELSIZE + OFFS_SCALE); }
float4 mupsRotationLoad(RWByteAddressBuffer mupsb, uint i) { return RWBABLoad4(mupsb, i*PELSIZE + OFFS_ROTATION); }

void mupsPositionStore(RWByteAddressBuffer mupsb, uint i, float3 src) { RWBABStore3(mupsb, i*PELSIZE, src); }
void mupsVelocityStore(RWByteAddressBuffer mupsb, uint i, float4 src) { RWBABStore4(mupsb, i*PELSIZE + OFFS_VELOCITY, src); }
void mupsForceStore(RWByteAddressBuffer mupsb, uint i, float3 src)    { RWBABStore3(mupsb, i*PELSIZE + OFFS_FORCE, src); }
void mupsColorStore(RWByteAddressBuffer mupsb, uint i, float4 src)    { RWBABStore4(mupsb, i*PELSIZE + OFFS_COLOR, src); }
void mupsSizeStore(RWByteAddressBuffer mupsb, uint i, float src)      { RWBABStore(mupsb, i*PELSIZE + OFFS_SIZE, src); }
void mupsAgeStore(RWByteAddressBuffer mupsb, uint i, float2 src)      { RWBABStore2(mupsb, i*PELSIZE + OFFS_AGE, src); }
void mupsScaleStore(RWByteAddressBuffer mupsb, uint i, float3 src)    { RWBABStore3(mupsb, i*PELSIZE + OFFS_SCALE, src); }
void mupsRotationStore(RWByteAddressBuffer mupsb, uint i, float4 src) { RWBABStore4(mupsb, i*PELSIZE + OFFS_ROTATION, src); }

float mupsLoad(RWByteAddressBuffer mupsb, uint i, uint o) { return RWBABLoad(mupsb, i*PELSIZE + o );}
float2 mupsLoad2(RWByteAddressBuffer mupsb, uint i, uint o) { return RWBABLoad2(mupsb, i*PELSIZE + o );}
float3 mupsLoad3(RWByteAddressBuffer mupsb, uint i, uint o) { return RWBABLoad3(mupsb, i*PELSIZE + o );}
float4 mupsLoad4(RWByteAddressBuffer mupsb, uint i, uint o) { return RWBABLoad4(mupsb, i*PELSIZE + o );}
float4x4 mupsLoad4x4(RWByteAddressBuffer mupsb, uint i, uint o) { return RWBABLoad4x4(mupsb, i*PELSIZE + o );}

void mupsStore(RWByteAddressBuffer mupsb, uint i, uint o, float src) { RWBABStore(mupsb, i*PELSIZE + o, src);}
void mupsStore2(RWByteAddressBuffer mupsb, uint i, uint o, float2 src) { RWBABStore2(mupsb, i*PELSIZE + o, src);}
void mupsStore3(RWByteAddressBuffer mupsb, uint i, uint o, float3 src) { RWBABStore3(mupsb, i*PELSIZE + o, src);}
void mupsStore4(RWByteAddressBuffer mupsb, uint i, uint o, float4 src) { RWBABStore4(mupsb, i*PELSIZE + o, src);}
void mupsStore4x4(RWByteAddressBuffer mupsb, uint i, uint o, float4x4 src) { RWBABStore4x4(mupsb, i*PELSIZE + o, src);}
