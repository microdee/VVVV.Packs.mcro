#define MREFORWARDPS_FXH

//#include "../fxh/ColorSpace.fxh"

// include MREForward.fxh before including this

// declare outside:
// StructuredBuffer<InstanceParams> InstancedParams;

StructuredBuffer<float4> SubsetTexID;
Texture2DArray DiffTex;
#if defined(HAS_NORMALMAP)
	#if defined(WRITEDEPTH)
		Texture2DArray BumpTex : FR_BUMPTEX;
	#endif
	Texture2DArray NormalTex : FR_NORMALTEX;
#endif

cbuffer cbPerObjectPS : register( b2 )
{
	float alphatest = 0.5;
	float bumpOffset = 0;
	float gVelocityGain = 1;
};


interface IAddress
{
	float Address(float res);
};
class CWrap : IAddress
{
	float Address(float res)
	{
		return frac(res);
	}
};
class CMirror : IAddress
{
	float Address(float res)
	{
		if(floor(res)%2==0)
			return frac(res);
		else
			return 1-frac(res);
	}
};
class CClamp : IAddress
{
	float Address(float res)
	{
		return saturate(res);
	}
};
class CUnlimited : IAddress
{
	float Address(float res)
	{
		return res;
	}
};

CWrap AWrap;
CMirror AMirror;
CClamp AClamp;
CUnlimited Unlimited;

IAddress addressU <string uiname="AddressU"; string linkclass="AWrap,AMirror,AClamp,Unlimited";> = AWrap;
IAddress addressV <string uiname="AddressV"; string linkclass="AWrap,AMirror,AClamp,Unlimited";> = AWrap;

float2 AddressUV(float2 uv)
{
	float2 res = uv;
	res.x = addressU.Address(res.x);
	res.y = addressV.Address(res.y);
	return res;
}

PSOut PS(PSin In)
{
	float ii = In.ii;
	float3 PosV = In.PosV.xyz;

	PSOut Out = (PSOut)0;
	float3 NormV = In.NormV;
	
	float2 uvb = In.TexCd.xy;

	float depth = InstancedParams[ii].BumpAmount;
	float4 TexID = SubsetTexID[ii];

    float4 diffcol = DiffTex.Sample(Sampler, float3(uvb, TexID.x));

    #if defined(HAS_NORMALMAP)
    	float3 normmap = NormalTex.Sample(Sampler, float3(uvb, TexID.y)).xyz*2-1;
		float3 outnorm = normalize(normmap.x * In.Tangent + normmap.y * In.Binormal + normmap.z * In.NormV);
		Out.normalV = float4(lerp(NormV, outnorm, depth),1);
	#else
		Out.normalV = float4(NormV,1);
	#endif

	float alphat = diffcol.a;

	#if defined(ALPHATEST)
		if(alphatest!=0)
		{
			alphat = lerp(alphat, (alphat>=alphatest), min(alphatest*10,1));
			clip(alphat - (1-alphatest));
		}
	#endif
	
    diffcol.rgb *= InstancedParams[ii].DiffAmount * InstancedParams[ii].DiffCol.rgb;
	Out.color.rgb = diffcol.rgb;
	//Out.color.rgb = HUEtoRGB(ii/10);
	//Out.color.a = alphat;
	
	#if defined(WRITEDEPTH)
		float mdepth = BumpTex.Sample(Sampler, float3(uvb, TexID.z)).r + bumpOffset;
		if(depth!=0) PosV += In.NormV * mdepth * depth * 0.1;
		if(DepthMode == 1)
		{
			float d = length(PosV.xyz);
			d -= NearFarPow.x;
			d /= abs(NearFarPow.y - NearFarPow.x);
			d = pows(d, NearFarPow.z);
			Out.depth = saturate(d);
		}
		else
		{
			float4 posout = mul(float4(PosV,1),tP);
			Out.depth = posout.z/posout.w;
		}
	#endif
	
	Out.veluv.xy = In.PosP.xy/In.PosP.w - In.velocity.xy/In.velocity.w;
    Out.veluv.xy *= 0.5 * gVelocityGain * InstancedParams[ii].VelocityGain;
	Out.veluv.xy += 0.5;
	
    Out.veluv.zw = AddressUV(uvb);
	
	Out.color.a = InstancedParams[ii].MatID;
	Out.normalV.a = InstancedParams[ii].ObjID0;
	
    return Out;
}
PSOut PSw(PSin In)
{
    return (PSOut)1;
}