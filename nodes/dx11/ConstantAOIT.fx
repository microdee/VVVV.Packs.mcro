//@author: microdee
//@help: standard constant shader
//@tags: color
//@credits: 

Texture2D texture2d <string uiname="Texture";>;
RWTexture2D<float4> target : BACKBUFFER;
float2 R : TARGETSIZE;

SamplerState g_samLinear <string uiname="Sampler State";>
{
    Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Clamp;
    AddressV = Clamp;
};

 
cbuffer cbPerDraw : register( b0 )
{
	float4x4 tVP : VIEWPROJECTION;
};


cbuffer cbPerObj : register( b1 )
{
	float4x4 tW : WORLD;
	float2 NearFar = float2(0.1,100);
	float Alpha <float uimin=0.0; float uimax=1.0;> = 1; 
	float4 cAmb <bool color=true;String uiname="Color";> = { 1.0f,1.0f,1.0f,1.0f };
	float4x4 tTex <string uiname="Texture Transform"; bool uvspace=true; >;
	float4x4 tColor <string uiname="Color Transform";>;
};

struct VS_IN
{
	float4 PosO : POSITION;
	float4 TexCd : TEXCOORD0;

};

struct vs2ps
{
    float4 PosWVP: SV_POSITION;
    float4 TexCd: TEXCOORD0;
	float depth: DEPTH;
};

vs2ps VS(VS_IN input)
{
    vs2ps Out = (vs2ps)0;
    Out.PosWVP  = mul(input.PosO,mul(tW,tVP));
    Out.TexCd = mul(input.TexCd, tTex);
	Out.depth = Out.PosWVP.z / Out.PosWVP.w;
    return Out;
}
float Join(float a, float b)
{
	uint ab = f32tof16(a);
	uint bb = f32tof16(b);
	bb = bb << 16;
	uint ret = ab | bb;
	return asfloat(ret);
}

float2 Split(float a)
{
	uint2 ret = 0;
	ret.x = f16tof32(asuint(a));
	ret.y = f16tof32(asuint(a) >> 16);
	return asfloat(ret);
}
struct psout
{
	float4 col : SV_Target;
	float depth : SV_Depth;
};

psout PS(vs2ps In)
{
	psout output = (psout)0;
	uint2 uv = In.TexCd.xy*R;
	float4 prin = target[uv];
	float2 prad = Split(prin.w);
	float prAlpha = prad.x;
	float prDepth = (prad.y == 0) ? NearFar.y : prad.y;
	float3 prcol = prin.rgb;
	float4 ccol = texture2d.Sample(g_samLinear, In.TexCd) * cAmb;
	float4 outCol = 0;
	float outDepth = 0;
	if((prAlpha >= 1) && (prDepth < In.depth)) discard;
	if((ccol.a >= 1) && (prDepth > In.depth))
	{
		output.col.rgb = ccol.rgb;
		output.col.a = Join(ccol.a, In.depth);
		output.depth = In.depth;
		return output;
	}
	
	if(prDepth > In.depth)
	{
		output.col.rgb = ccol.rgb * ccol.a + prcol * prAlpha * (1-ccol.a);
		output.col.a = ccol.a + prAlpha * (1-ccol.a);
		output.depth = In.depth;
		return output;
	}
	else
	{
		output.col.rgb = ccol.rgb * ccol.a * (1-prAlpha) + prcol * prAlpha;
		output.col.a = ccol.a * (1-prAlpha) + prAlpha;
		output.depth = In.depth;
		return output;
	}
}





technique10 Constant
{
	pass P0
	{
		SetVertexShader( CompileShader( vs_5_0, VS() ) );
		SetPixelShader( CompileShader( ps_5_0, PS() ) );
	}
}




