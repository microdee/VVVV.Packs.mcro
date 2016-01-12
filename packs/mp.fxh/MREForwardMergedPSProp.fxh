#define MREFORWARDPS_FXH
// include MREForward.fxh before including this

// declare outside:
// StructuredBuffer<InstanceParams> InstancedParams;

StructuredBuffer<float4> SubsetTexID;
#if defined(ALPHATEST)
	Texture2DArray DiffTex : FR_DIFFTEX;
#endif
#if defined(WRITEDEPTH)
	Texture2DArray BumpTex : FR_BUMPTEX;
#endif

cbuffer cbPerDrawPS : register( b2 )
{
	float3 CamPos : CAM_POSITION;
};

cbuffer cbPerObjectPS : register( b3 )
{
	float alphatest = 0.5;
	float bumpOffset = 0;
};

float PS(PSinProp In) : SV_Target
{
	float ii = In.ii;
	
	float2 uvb = In.TexCd.xy;
	float4 TexID = SubsetTexID[ii];
	
	#if defined(WRITEDEPTH)
		float depth = InstancedParams[ii].BumpAmount;
		float mdepth = BumpTex.Sample(Sampler, float3(uvb, TexID.z)).r + bumpOffset;
		float3 PosW = In.PosW.xyz + In.NormW * mdepth * depth * 0.1;
	#else
		float3 PosW = In.PosW.xyz;
	#endif

	#if defined(ALPHATEST)
		if(alphatest!=0)
		{
		    float4 diffcol = DiffTex.Sample( Sampler, float3(uvb, TexID.x));
			float alphat = diffcol.a;
			alphat = lerp(alphat, (alphat>=alphatest), min(alphatest*10,1));
			clip(alphat - (1-alphatest));
		}
	#endif
	
	float d = distance(PosW, CamPos);
	
    return d;
}