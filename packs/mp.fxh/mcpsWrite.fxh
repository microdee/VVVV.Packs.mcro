#if !defined(RWBYTEADDRESSBUFFERUTILS_FXH)
#include "../../../mp.fxh/RWByteAddressBufferUtils.fxh"
#endif
#if !defined(MCPSWORLD_FXH)
#include "../../../mp.fxh/mcpsWorld.fxh"
#endif

StructuredBuffer<uint> EmitOffset : PS_EMITTEROFFSET;

float3 mcpsPositionLoad(RWByteAddressBuffer mcpsb, uint i) { return RWBABLoad3(mcpsb, i*PELSIZE); }
float4 mcpsVelocityLoad(RWByteAddressBuffer mcpsb, uint i) { return RWBABLoad4(mcpsb, i*PELSIZE + OFFS_VELOCITY); }
float3 mcpsForceLoad(RWByteAddressBuffer mcpsb, uint i)    { return RWBABLoad3(mcpsb, i*PELSIZE + OFFS_FORCE); }
float4 mcpsColorLoad(RWByteAddressBuffer mcpsb, uint i)    { return RWBABLoad4(mcpsb, i*PELSIZE + OFFS_COLOR); }
float  mcpsSizeLoad(RWByteAddressBuffer mcpsb, uint i)     { return RWBABLoad(mcpsb, i*PELSIZE + OFFS_SIZE); }
float2 mcpsAgeLoad(RWByteAddressBuffer mcpsb, uint i)      { return RWBABLoad2(mcpsb, i*PELSIZE + OFFS_AGE); }
float3 mcpsScaleLoad(RWByteAddressBuffer mcpsb, uint i)    { return RWBABLoad3(mcpsb, i*PELSIZE + OFFS_SCALE); }
float4 mcpsRotationLoad(RWByteAddressBuffer mcpsb, uint i) { return RWBABLoad4(mcpsb, i*PELSIZE + OFFS_ROTATION); }

void mcpsPositionStore(RWByteAddressBuffer mcpsb, uint i, float3 src) { RWBABStore3(mcpsb, i*PELSIZE, src); }
void mcpsVelocityStore(RWByteAddressBuffer mcpsb, uint i, float4 src) { RWBABStore4(mcpsb, i*PELSIZE + OFFS_VELOCITY, src); }
void mcpsForceStore(RWByteAddressBuffer mcpsb, uint i, float3 src)    { RWBABStore3(mcpsb, i*PELSIZE + OFFS_FORCE, src); }
void mcpsColorStore(RWByteAddressBuffer mcpsb, uint i, float4 src)    { RWBABStore4(mcpsb, i*PELSIZE + OFFS_COLOR, src); }
void mcpsSizeStore(RWByteAddressBuffer mcpsb, uint i, float src)      { RWBABStore(mcpsb, i*PELSIZE + OFFS_SIZE, src); }
void mcpsAgeStore(RWByteAddressBuffer mcpsb, uint i, float2 src)      { RWBABStore2(mcpsb, i*PELSIZE + OFFS_AGE, src); }
void mcpsScaleStore(RWByteAddressBuffer mcpsb, uint i, float3 src)    { RWBABStore3(mcpsb, i*PELSIZE + OFFS_SCALE, src); }
void mcpsRotationStore(RWByteAddressBuffer mcpsb, uint i, float4 src) { RWBABStore4(mcpsb, i*PELSIZE + OFFS_ROTATION, src); }

float mcpsLoad(RWByteAddressBuffer mcpsb, uint i, uint o) { return RWBABLoad(mcpsb, i*PELSIZE + o );}
float2 mcpsLoad2(RWByteAddressBuffer mcpsb, uint i, uint o) { return RWBABLoad2(mcpsb, i*PELSIZE + o );}
float3 mcpsLoad3(RWByteAddressBuffer mcpsb, uint i, uint o) { return RWBABLoad3(mcpsb, i*PELSIZE + o );}
float4 mcpsLoad4(RWByteAddressBuffer mcpsb, uint i, uint o) { return RWBABLoad4(mcpsb, i*PELSIZE + o );}
float4x4 mcpsLoad4x4(RWByteAddressBuffer mcpsb, uint i, uint o) { return RWBABLoad4x4(mcpsb, i*PELSIZE + o );}

void mcpsStore(RWByteAddressBuffer mcpsb, uint i, uint o, float src) { RWBABStore(mcpsb, i*PELSIZE + o, src);}
void mcpsStore2(RWByteAddressBuffer mcpsb, uint i, uint o, float2 src) { RWBABStore2(mcpsb, i*PELSIZE + o, src);}
void mcpsStore3(RWByteAddressBuffer mcpsb, uint i, uint o, float3 src) { RWBABStore3(mcpsb, i*PELSIZE + o, src);}
void mcpsStore4(RWByteAddressBuffer mcpsb, uint i, uint o, float4 src) { RWBABStore4(mcpsb, i*PELSIZE + o, src);}
void mcpsStore4x4(RWByteAddressBuffer mcpsb, uint i, uint o, float4x4 src) { RWBABStore4x4(mcpsb, i*PELSIZE + o, src);}
