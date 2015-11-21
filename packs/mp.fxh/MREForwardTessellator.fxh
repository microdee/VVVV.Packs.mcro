
/////////////////////////
//////// Structs ////////
/////////////////////////
float2 R:TARGETSIZE;

struct HSin
{
    float4 PosW: POSITION;
	float4 TexCd: TEXCOORD0;
    float3 NormW: NORMAL;
    nointerpolation float ii : INSTANCEID;
    #if defined(HAS_NORMALMAP)
        float3 Tangent : TANGENT;
        float3 Binormal : BINORMAL;
    #endif
    #if defined(HAS_GEOMVELOCITY)
        float4 Velocity : PREVPOS;
    #endif
	/*
	#if defined(HAS_SUBSETID)
		float SubsetID : SUBSETID;
	#endif
	*/
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
        float3 Velocity : PREVPOS;
    #endif
    #if defined(HAS_NORMALMAP)
        float3 Tangent : TANGENT;
        float3 Binormal : BINORMAL;
    #endif
};

/////////////////////
///// Functions /////
/////////////////////

float3 DisplacedNormal(float3 norm, float4 nsew, float me, float am)
{              
	
	//find perpendicular vector to norm:        
	float3 temp = norm; //a temporary vector that is not parallel to norm
	if(norm.x==1)
		temp.y+=0.5;
	else
		temp.x+=0.5;
	
	//form a basis with norm being one of the axes:
	float3 perp1 = normalize(cross(norm,temp));
	float3 perp2 = normalize(cross(norm,perp1));
	
	//use the basis to move the normal in its own space by the offset        
	float3 normalOffset = -am*(((nsew.x-me)-(nsew.y-me))*perp1 + ((nsew.z-me)-(nsew.w-me))*perp2);
	return normalize(norm + normalOffset);
}

float3 SampleNormal(float3 norm, Texture2D disp, sampler sS, float2 uv, float ww, float am, float LOD)
{
	float me = disp.SampleLevel(sS,uv,LOD).x-.5;
	float n = disp.SampleLevel(sS,float2(uv.x,uv.y+ww),LOD).x-.5;
	float s = disp.SampleLevel(sS,float2(uv.x,uv.y-ww),LOD).x-.5;
	float e = disp.SampleLevel(sS,float2(uv.x+ww,uv.y),LOD).x-.5;
	float w = disp.SampleLevel(sS,float2(uv.x-ww,uv.y),LOD).x-.5;
	
	return DisplacedNormal(norm, float4(n,s,e,w), me, am);
}
float3 SampleArrayNormal(float3 norm, Texture2DArray disp, sampler sS, float2 uv, float i, float ww, float am, float LOD)
{
	float me = disp.SampleLevel(sS,float3(uv, i),LOD).x-.5;
	float n = disp.SampleLevel(sS,float3(uv.x,uv.y+ww, i),LOD).x-.5;
	float s = disp.SampleLevel(sS,float3(uv.x,uv.y-ww, i),LOD).x-.5;
	float e = disp.SampleLevel(sS,float3(uv.x+ww,uv.y, i),LOD).x-.5;
	float w = disp.SampleLevel(sS,float3(uv.x-ww,uv.y, i),LOD).x-.5;
	
	return DisplacedNormal(norm, float4(n,s,e,w), me, am);
}
struct TangentSpace
{
	float3 n;
	float3 t;
	float3 b;
};

TangentSpace SampleNormalTangents(TangentSpace normt, Texture2D disp, sampler sS, float2 uv, float ww, float am, float LOD)
{
	float me = disp.SampleLevel(sS,uv,LOD).x-.5;
	float n = disp.SampleLevel(sS,float2(uv.x,uv.y+ww),LOD).x-.5;
	float s = disp.SampleLevel(sS,float2(uv.x,uv.y-ww),LOD).x-.5;
	float e = disp.SampleLevel(sS,float2(uv.x+ww,uv.y),LOD).x-.5;
	float w = disp.SampleLevel(sS,float2(uv.x-ww,uv.y),LOD).x-.5;
	
	TangentSpace ret = (TangentSpace)0;
	ret.n = DisplacedNormal(normt.n, float4(n,s,e,w), me, am);
	ret.t = DisplacedNormal(normt.t, float4(n,s,e,w), me, am);
	ret.b = DisplacedNormal(normt.b, float4(n,s,e,w), me, am);
	return ret;
}
TangentSpace SampleArrayNormalTangents(TangentSpace normt, Texture2DArray disp, sampler sS, float2 uv, float i, float ww, float am, float LOD)
{
	float me = disp.SampleLevel(sS,float3(uv, i),LOD).x-.5;
	float n = disp.SampleLevel(sS,float3(uv.x,uv.y+ww, i),LOD).x-.5;
	float s = disp.SampleLevel(sS,float3(uv.x,uv.y-ww, i),LOD).x-.5;
	float e = disp.SampleLevel(sS,float3(uv.x+ww,uv.y, i),LOD).x-.5;
	float w = disp.SampleLevel(sS,float3(uv.x-ww,uv.y, i),LOD).x-.5;
	
	TangentSpace ret = (TangentSpace)0;
	ret.n = DisplacedNormal(normt.n, float4(n,s,e,w), me, am);
	ret.t = DisplacedNormal(normt.t, float4(n,s,e,w), me, am);
	ret.b = DisplacedNormal(normt.b, float4(n,s,e,w), me, am);
	return ret;
}

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
	/* FAILED ATTEMPT OF EDGE-LENGTH BASED FACTOR
	float4 Aipos[3];
	float3 Appos[3];
	float ii = I[0].ii;
	
    #if defined(INSTANCING)
        float4x4 w = mul(InstancedParams[ii].tW,tW);
    #else
        float4x4 w = tW;
    #endif
	
	[unroll]
	for(uint i=0; i<3; i++)
	{
		Aipos[i] = mul(float4(I[i].PosW.xyz,1), mul(w, tVP));
		Appos[i] = Aipos[i].xyz/Aipos[i].w;
		Appos[i].z = 0;
		//Appos[i].xy = min(Appos[i].xy, 1);
		//Appos[i].xy = max(Appos[i].xy, -1);
		Appos[i].xy *= R/20;
	}
	
	float FeU = distance(Appos[0], Appos[1]);
	float FeV = distance(Appos[1], Appos[2]);
	float FeW = distance(Appos[2], Appos[0]);
	*/
	
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
[partitioning("fractional_even")]
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
    #if defined(HAS_NORMALMAP)
        O.Tangent = I[uCPID].Tangent;
        O.Binormal = I[uCPID].Binormal;
    #endif
	
    return O;
}