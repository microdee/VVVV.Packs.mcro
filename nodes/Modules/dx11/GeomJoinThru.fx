struct VSin
{
	float4 cpoint : POSITION;
	float3 norm : NORMAL;
	#if defined(TEXCOORD_IN)
		float4 TexCd: TEXCOORD0;
	#endif
	#if defined(TANGENTS_IN)
		float3 Tangent : TANGENT;
		float3 Binormal : BINORMAL;
	#endif
	#if defined(BLENDWEIGHTS_IN)
		float4 BlendId : BLENDINDICES;
		float4 BlendWeight : BLENDWEIGHT;
	#endif
	uint vid : SV_VertexID;
	uint iid : SV_InstanceID;
};

struct GSin
{
	float4 cpoint : SV_Position;
	float3 norm : NORMAL;
	#if defined(TEXCOORD_OUT)
		float4 TexCd: TEXCOORD0;
	#endif
	float4 PrevPos : COLOR0;
	#if defined(TANGENTS_OUT)
		float3 Tangent : TANGENT;
		float3 Binormal : BINORMAL;
	#endif
	#if defined(BLENDWEIGHTS_OUT)
		float4 BlendId : BLENDINDICES;
		float4 BlendWeight : BLENDWEIGHT;
	#endif
};

float ID = 0;

GSin VS(VSin input)
{
    GSin output;
    output.cpoint = input.cpoint;
	output.norm = input.norm;
	#if defined(TEXCOORD_IN) && defined(TEXCOORD_OUT)
		output.TexCd = input.TexCd;
	#endif
	#if defined(TANGENTS_IN) && defined(TANGENTS_OUT)
		output.Tangent = input.Tangent;
		output.Binormal = input.Binormal;
	#endif
	#if defined(BLENDWEIGHTS_IN) && defined(BLENDWEIGHTS_OUT)
		output.BlendId = input.BlendId;
		output.BlendWeight = input.BlendWeight;
	#endif
	output.PrevPos = float4(input.cpoint.xyz, ID);
	
    return output;
}
[maxvertexcount(3)]
void GS(triangle GSin input[3], inout TriangleStream<GSin>GSOut)
{
	GSin v = (GSin)0;

	for(uint i=0;i<3;i++)
	{
		v=input[i];
		GSOut.Append(v);
	}
}

GeometryShader StreamOutGS = ConstructGSWithSO( CompileShader( gs_5_0, GS() ),
	"SV_Position.xyz;"
	"NORMAL.xyz"
	#if defined(TEXCOORD_OUT)
		";TEXCOORD0.xy"
	#endif
	#if defined(TANGENTS_OUT)
		";TANGENT.xyz"
		";BINORMAL.xyz"
	#endif
	#if defined(BLENDWEIGHTS_OUT)
		";BLENDINDICES"
		";BLENDWEIGHT"
	#endif
	";COLOR0"
);

technique11 Layout
{
	pass P0
	{
		
		SetVertexShader( CompileShader( vs_5_0, VS() ) );
		SetGeometryShader( CompileShader( gs_5_0, GS() ) );
	    SetGeometryShader( StreamOutGS );

	}
}