
/////////////////////////
//////// Structs ////////
/////////////////////////

struct HSin
{
    float4 PosW: POSITION;
	float4 TexCd: TEXCOORD0;
    float3 NormW: NORMAL;
    nointerpolation float ii : INSTANCEID;
    #if defined(HAS_GEOMVELOCITY)
        float4 Velocity : COLOR0;
    #endif
};
struct hsconst
{
    float fTessFactor[3]    : SV_TessFactor ;
    float fInsideTessFactor : SV_InsideTessFactor ;
    float3 f3B210    : POSITION3 ;
    float3 f3B120    : POSITION4 ;
    float3 f3B021    : POSITION5 ;
    float3 f3B012    : POSITION6 ;
    float3 f3B102    : POSITION7 ;
    float3 f3B201    : POSITION8 ;
    float3 f3B111    : CENTER ;
    float3 f3N110    : NORMAL3 ;
    float3 f3N011    : NORMAL4 ;
    float3 f3N101    : NORMAL5 ;
};
struct DSin
{
    float4 PosW: POSITION;
	float4 TexCd: TEXCOORD0;
    float3 NormW: NORMAL;
    nointerpolation float ii : INSTANCEID;
    #if defined(HAS_GEOMVELOCITY)
        float4 Velocity : COLOR0;
    #endif
};

/////////////////////
///// Functions /////
/////////////////////


float3 InterpolateDir(
    hsconst HSConstantData,
    float3 in0, float3 in1, float3 in2,
    float3 fUVW2, float3 fUVW, float curve)
{

    float3 f3 = in0 * fUVW.z +
        in1 * fUVW.x +
        in2 * fUVW.y +
        HSConstantData.f3N110 * fUVW2.z * fUVW2.x +
        HSConstantData.f3N011 * fUVW2.x * fUVW2.y +
        HSConstantData.f3N101 * fUVW2.z * fUVW2.y;
    float3 o = in0 * fUVW.z +
        in1 * fUVW.x +
        in2 * fUVW.y;
    f3 = lerp(o,f3,curve);
    return normalize(f3);
}

float3 InterpolatePos(
    hsconst HSConstantData,
    float3 in0, float3 in1, float3 in2,
    float3 fUVW2, float3 fUVW, float curve,
    out float3 f3, out float3 o)
{
    float fUU3 = fUVW2.x * 3.0f;
    float fVV3 = fUVW2.y * 3.0f;
    float fWW3 = fUVW2.z * 3.0f;

    f3 = in0 * fUVW2.z * fUVW.z +
        in1 * fUVW2.x * fUVW.x +
        in2 * fUVW2.y * fUVW.y +
        HSConstantData.f3B210 * fWW3 * fUVW.x +
        HSConstantData.f3B120 * fUVW.z * fUU3 +
        HSConstantData.f3B201 * fWW3 * fUVW.y +
        HSConstantData.f3B021 * fUU3 * fUVW.y +
        HSConstantData.f3B102 * fUVW.z * fVV3 +
        HSConstantData.f3B012 * fUVW.x * fVV3 +
        HSConstantData.f3B111 * 6.0f * fUVW.z * fUVW.x * fUVW.y ;
    o = in0 * fUVW.z +
        in1 * fUVW.x +
        in2 * fUVW.y ;
    return lerp(o,f3,curve);
}

/////////////////////
//////// HSC ////////
/////////////////////

hsconst HSC( InputPatch<HSin, 3> I )
{
    hsconst O = (hsconst)0;
	O.fTessFactor[0] = Factor;    
    O.fTessFactor[1] = Factor;    
    O.fTessFactor[2] = Factor;   
    O.fInsideTessFactor = ( O.fTessFactor[0] + O.fTessFactor[1] + O.fTessFactor[2] ) / 3.0f;  
		
    float3 f3B003 = I[0].PosW;
    float3 f3B030 = I[1].PosW;
    float3 f3B300 = I[2].PosW;
    // And Normals
    float3 f3N002 = I[0].NormW;
    float3 f3N020 = I[1].NormW;
    float3 f3N200 = I[2].NormW;

	O.f3B210 = ( ( 2.0f * f3B003 ) + f3B030 - ( dot( ( f3B030 - f3B003 ), f3N002 ) * f3N002 ) ) / 3.0f;
	O.f3B120 = ( ( 2.0f * f3B030 ) + f3B003 - ( dot( ( f3B003 - f3B030 ), f3N020 ) * f3N020 ) ) / 3.0f;
    O.f3B021 = ( ( 2.0f * f3B030 ) + f3B300 - ( dot( ( f3B300 - f3B030 ), f3N020 ) * f3N020 ) ) / 3.0f;
    O.f3B012 = ( ( 2.0f * f3B300 ) + f3B030 - ( dot( ( f3B030 - f3B300 ), f3N200 ) * f3N200 ) ) / 3.0f;
    O.f3B102 = ( ( 2.0f * f3B300 ) + f3B003 - ( dot( ( f3B003 - f3B300 ), f3N200 ) * f3N200 ) ) / 3.0f;
    O.f3B201 = ( ( 2.0f * f3B003 ) + f3B300 - ( dot( ( f3B300 - f3B003 ), f3N002 ) * f3N002 ) ) / 3.0f;

    float3 f3E = ( O.f3B210 + O.f3B120 + O.f3B021 + O.f3B012 + O.f3B102 + O.f3B201 ) / 6.0f;
    float3 f3V = ( f3B003 + f3B030 + f3B300 ) / 3.0f;
    O.f3B111 = f3E + ( ( f3E - f3V ) / 2.0f );
    
    float fV12 = 2.0f * dot( f3B030 - f3B003, f3N002 + f3N020 ) / dot( f3B030 - f3B003, f3B030 - f3B003 );
    O.f3N110 = normalize( f3N002 + f3N020 - fV12 * ( f3B030 - f3B003 ) );
    float fV23 = 2.0f * dot( f3B300 - f3B030, f3N020 + f3N200 ) / dot( f3B300 - f3B030, f3B300 - f3B030 );
    O.f3N011 = normalize( f3N020 + f3N200 - fV23 * ( f3B300 - f3B030 ) );
    float fV31 = 2.0f * dot( f3B003 - f3B300, f3N200 + f3N002 ) / dot( f3B003 - f3B300, f3B003 - f3B300 );
    O.f3N101 = normalize( f3N200 + f3N002 - fV31 * ( f3B003 - f3B300 ) );
    return O;
}

////////////////////
//////// HS ////////
////////////////////

[domain("tri")]
[partitioning("fractional_odd")]
[outputtopology("triangle_cw")]
[patchconstantfunc("HSC")]
[outputcontrolpoints(3)]
DSin HS( InputPatch<HSin, 3> I, uint uCPID : SV_OutputControlPointID )
{
    DSin O = (DSin)0;
    O.PosW = I[uCPID].PosW;
    O.NormW = I[uCPID].NormW;
	O.TexCd = I[uCPID].TexCd;
	O.ii = I[uCPID].ii;

    #if defined(HAS_GEOMVELOCITY)
        O.Velocity = I[uCPID].Velocity;
    #endif
	
    return O;
}