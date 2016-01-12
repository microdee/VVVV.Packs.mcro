#define BYTEADDRESSBUFFERUTILS_FXH

float BABLoad(ByteAddressBuffer bab, uint i) {return asfloat(bab.Load( i*4 ));}
float2 BABLoad2(ByteAddressBuffer bab, uint i) {return asfloat(bab.Load2( i*4 ));}
float3 BABLoad3(ByteAddressBuffer bab, uint i) {return asfloat(bab.Load3( i*4 ));}
float4 BABLoad4(ByteAddressBuffer bab, uint i) {return asfloat(bab.Load4( i*4 ));}
float4x4 BABLoad4x4(ByteAddressBuffer bab, uint ii)
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
