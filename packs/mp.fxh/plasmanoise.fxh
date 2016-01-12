#define PLASMANOISE_FXH

#define MINTWOPI -6.283185307179586476925286766559
#define TWOPI 6.283185307179586476925286766559

float Freqs[22] : IMMUTABLE = {17,13,9,23,33,43,5,6,7,93,73,53,43,33,23,53,93,73,13,1,17,8};
float Amps[5] : IMMUTABLE = {5,6,9,7,2};

float3 lungth(float3 x,float3 c){
       return float3(length(x+c.r),length(x+c.g),length(x+c.b));
}
float3 distort(float3 inpos, float4x4 Transform, float saturation, float freq[22], float amp[5])
{
    float3 c=0;
    float3 x=mul(float4(inpos,1),Transform).xyz;
    x+=sin(x.zyx*sqrt(float3(freq[0],freq[1],freq[2])))/amp[0];
    c=lungth(sin(x*sqrt(float3(freq[3],freq[4],freq[5]))),float3(freq[6],freq[7],freq[8])*saturation);
    x+=sin(x.zyx*sqrt(float3(freq[9],freq[10],freq[11])))/amp[1];
    c=2*lungth(sin(x*sqrt(float3(freq[12],freq[13],freq[14]))),c/amp[2]);
    x+=sin(x.zyx*sqrt(float3(freq[15],freq[16],freq[17])))/amp[3];
    c=lungth(sin(x*sqrt(float3(freq[18],freq[19],freq[20]))),c/amp[4]);
    c=sin(c*freq[21]);
    return normalize(c);
}
