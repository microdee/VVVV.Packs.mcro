#include "pows.fxh"
// utilities
float lengthn(float2 v, float n) {return pow((pow(abs(v.x),n)+pow(abs(v.y),n)),1/n);}
float lengthn(float3 v, float n) {return pow((pow(abs(v.x),n)+pow(abs(v.y),n)+pow(abs(v.z),n)),1/n);}
float lengthn(float4 v, float n) {return pow((pow(abs(v.x),n)+pow(abs(v.y),n)+pow(abs(v.z),n)+pow(abs(v.w),n)),1/n);}

// simple operations
float U(float a, float b) {return min(a,b);}
float S(float a, float b) {return max(a,-b);}
float I(float a, float b) {return max(a,b);}

// smooth operations
float sU( float a, float b, float k )
{
    float h = clamp( 0.5+0.5*(b-a)/k, 0.0, 1.0 );
    return lerp( b, a, h ) - k*h*(1.0-h);
}
float sS( float a, float b, float k )
{
    return log( exp(a/k) + exp(-b/k) ) * k;
}
float sI( float a, float b, float k )
{
    return log( exp(a/k) + exp(b/k) ) * k;
}

float blend(float a, float b, float s, float mass)
{
	float res = b;
	if(floor(s)==1) res = U(a,b);
	if(floor(s)==2) res = S(a,b);
	if(floor(s)==3) res = I(a,b);
	if(floor(s)==4) res = sU(a,b,mass);
	if(floor(s)==5) res = sS(a,b,mass/4);
	if(floor(s)==6) res = sI(a,b,mass/4);
	if(floor(s)==7) res = a;
	return res;
}

// primitives
float sphere(float3 p, float r) {return length(p)-r;}
float sphere(float3 p, float r, float n) {return lengthn(p,n)-r;}
float cylinder(float3 p, float3 c) {return length(p.xz-c.xy)-c.z;}
float cylinder(float3 p, float3 c, float n) {return lengthn(p.xz-c.xy,n)-c.z;}
float plane(float3 p, float4 n)
{
	float4 nn = normalize(n);
	return dot(p,n.xyz) + n.w;
}
float box(float3 p, float3 b)
{
	float3 d = abs(p) - b;
	return min(max(d.x,max(d.y,d.z)),0.0) + length(max(d,0.0));
}
float torus( float3 p, float2 t )
{
	float2 q = float2(length(p.xz)-t.x,p.y);
	return length(q)-t.y;
}
float torus( float3 p, float2 t, float2 n)
{
	float2 q = float2(lengthn(p.xz,n.x)-t.x,p.y);
	return lengthn(q,n.y)-t.y;
}
float cone( float3 p, float2 c )
{
    float2 cc = normalize(c);
    float q = length(p.xy);
    return dot(cc,float2(q,p.z));
}
float cone( float3 p, float2 c, float n )
{
    float2 cc = normalize(c);
    float q = lengthn(p.xy,n);
    return dot(cc,float2(q,p.z));
}
float capsule( float3 p, float3 a, float3 b, float r )
{
    float3 pa = p - a;
    float3 ba = b - a;
    float h = clamp( dot(pa,ba)/dot(ba,ba), 0.0, 1.0 );
    return length( pa - ba*h ) - r;
}
float capsule( float3 p, float3 a, float3 b, float r, float n)
{
    float3 pa = p - a;
    float3 ba = b - a;
    float h = clamp( dot(pa,ba)/dot(ba,ba), 0.0, 1.0 );
    return lengthn(pa - ba*h, n) - r;
}

// interface for distance function
/*
	"uint i" attribute recommended structure:
	.x: Particle ID
	.y: Iteration count
	.z: Primitive Property buffer size
	.w: Operation Property buffer size
*/
interface DFInterface
{
	float df(float initd, float3 p, uint4 i);
};

// calculate normals
float3 CalcNorm(float initd, float3 p, uint4 i, float width, DFInterface idf)
{
	float3 grad;
	grad.x = idf.df(initd, p + float3( width, 0, 0), i) -
	         idf.df(initd, p + float3(-width, 0, 0), i);
	grad.y = idf.df(initd, p + float3( 0, width, 0), i) -
	         idf.df(initd, p + float3( 0,-width, 0), i);
	grad.z = idf.df(initd, p + float3( 0, 0, width), i) -
	         idf.df(initd, p + float3( 0, 0,-width), i);
	return normalize(grad);
};