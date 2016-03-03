RWStructuredBuffer<uint> Buf0 : BUF0;
RWStructuredBuffer<uint> Buf1 : BUF1;
#if BUFN > 2
RWStructuredBuffer<uint> Buf2 : BUF2;
#endif
#if BUFN > 3
RWStructuredBuffer<uint> Buf3 : BUF3;
#endif

cbuffer cbuf
{
	bool incr0 = false;
	bool incr1 = false;
	bool incr2 = false;
	bool incr3 = false;
};

struct csin
{
	uint3 DTID : SV_DispatchThreadID;
	uint3 GTID : SV_GroupThreadID;
	uint3 GID : SV_GroupID;
};

[numthreads(1, 1, 1)]
void CS_Count(csin input)
{
	if(incr0)
	{
		Buf0[0] = Buf0.IncrementCounter();
	}
	if(incr1)
	{
		Buf1[0] = Buf1.IncrementCounter();
	}
#if BUFN > 2
	if(incr2)
	{
		Buf2[0] = Buf2.IncrementCounter();
	}
#endif
#if BUFN > 3
	if(incr3)
	{
		Buf3[0] = Buf3.IncrementCounter();
	}
#endif
}
technique11 count { pass P0{SetComputeShader( CompileShader( cs_5_0, CS_Count() ) );} }