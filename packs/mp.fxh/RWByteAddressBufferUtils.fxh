#define RWBYTEADDRESSBUFFERUTILS_FXH

float RWBABLoad(RWByteAddressBuffer bab, uint i) { return asfloat(bab.Load( i*4 )); }
float2 RWBABLoad2(RWByteAddressBuffer bab, uint i) { return asfloat(bab.Load2( i*4 )); }
float3 RWBABLoad3(RWByteAddressBuffer bab, uint i) { return asfloat(bab.Load3( i*4 )); }
float4 RWBABLoad4(RWByteAddressBuffer bab, uint i) { return asfloat(bab.Load4( i*4 )); }
float4x4 RWBABLoad4x4(RWByteAddressBuffer bab, uint ii)
{
    float4x4 tmp = 0;
    [unroll]
    for(uint i=0; i<4; i++)
    {
        [unroll]
        for(uint j=0; j<4; j++)
        {
            tmp[i][j] = asfloat(bab.Load( ii*4 + i*16 + j*4 ));
        }
    }
    return tmp;
}

void RWBABStore(RWByteAddressBuffer bab, uint i, float src) { bab.Store( i*4, asuint(src) ); }
void RWBABStore2(RWByteAddressBuffer bab, uint i, float2 src) { bab.Store2( i*4, asuint(src) ); }
void RWBABStore3(RWByteAddressBuffer bab, uint i, float3 src) { bab.Store3( i*4, asuint(src) ); }
void RWBABStore4(RWByteAddressBuffer bab, uint i, float4 src) { bab.Store4( i*4, asuint(src) ); }
void RWBABStore4x4(RWByteAddressBuffer bab, uint ii, float4x4 src)
{
    [unroll]
    for(uint i=0; i<4; i++)
    {
        [unroll]
        for(uint j=0; j<4; j++)
        {
            bab.Store( ii*4 + i*16 + j*4, asuint(src[i][j]));
        }
    }
}
