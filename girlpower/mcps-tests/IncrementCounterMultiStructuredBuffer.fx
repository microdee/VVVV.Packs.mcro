RWStructuredBuffer<uint> BufOut : BACKBUFFER1;
StructuredBuffer<float> BufIn;

bool apply;

struct csin
{
	uint3 DTID : SV_DispatchThreadID;
	uint3 GTID : SV_GroupThreadID;
	uint3 GID : SV_GroupID;
};

[numthreads(1, 1, 1)]
void CS(csin input)
{
	if(apply){
		
		if (BufIn[input.DTID.x] > 0){
			BufOut.IncrementCounter();
		}	
	}
}

[numthreads(1, 1, 1)]
void CS_Count(csin input)
{
	if(apply){
		BufOut[0] =	BufOut.IncrementCounter();
	}
}

technique11 main { pass P0{SetComputeShader( CompileShader( cs_5_0, CS() ) );} }
technique11 count { pass P0{SetComputeShader( CompileShader( cs_5_0, CS_Count() ) );} }