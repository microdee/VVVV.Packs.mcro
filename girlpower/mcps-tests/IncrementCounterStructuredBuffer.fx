RWStructuredBuffer<uint> BufOut : BACKBUFFER;

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
		
		if (BufIn[input.DTID.x] > 0.0f){
			BufOut.IncrementCounter();
		}	
	}
	
}

[numthreads(1, 1, 1)]
void CS_Count(csin input)
{
	if (input.DTID.x > 1) return;
	
	if(apply){
		BufOut[0] = BufOut.IncrementCounter();
	}
	
}

technique11 main { pass P0{SetComputeShader( CompileShader( cs_5_0, CS() ) );} }
technique11 final { pass P0{SetComputeShader( CompileShader( cs_5_0, CS_Count() ) );} }