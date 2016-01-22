#if !defined(STDMATH_FXH)
#include "../../../mp.fxh/stdmath.fxh"
#endif
#if !defined(POWS_FXH)
#include "../../../mp.fxh/pows.fxh"
#endif

#if !defined(PI)
#define PI 3.1415926535897932
#endif
float4 qmul(float4 q1, float4 q2)
{
	return float4(
	q1.w * q2.x + q1.x * q2.w + q1.z * q2.y - q1.y * q2.z,
	q1.w * q2.y + q1.y * q2.w + q1.x * q2.z - q1.z * q2.x,
	q1.w * q2.z + q1.z * q2.w + q1.y * q2.x - q1.x * q2.y,
	q1.w * q2.w - q1.x * q2.x - q1.y * q2.y - q1.z * q2.z
	);
}
float4 qinvert(float4 q)
{
	return float4(-q.xyz,q.w);
}

float4x4 qrot(float4 q)
{
    float4x4 m1 = {
         q.w, q.z,-q.y, q.x,
        -q.z, q.w, q.x, q.y,
         q.y,-q.x, q.w, q.z,
        -q.x,-q.y,-q.z, q.w
    };
    float4x4 m2 = {
         q.w, q.z,-q.y,-q.x,
        -q.z, q.w, q.x,-q.y,
         q.y,-q.x, q.w,-q.z,
         q.x, q.y, q.z, q.w
    };
	return mul(m1, m2);
}

float4 m2q(float4x4 m)
{
    float4 q = float4(0,0,0,1);
    q.w = sqrt( max( 0, 1 + m[0][0] + m[1][1] + m[2][2] ) ) / 2;
    q.x = sqrt( max( 0, 1 + m[0][0] - m[1][1] - m[2][2] ) ) / 2;
    q.y = sqrt( max( 0, 1 - m[0][0] + m[1][1] - m[2][2] ) ) / 2;
    q.z = sqrt( max( 0, 1 - m[0][0] - m[1][1] + m[2][2] ) ) / 2;
    q.x = copysign( q.x, m[1][2] - m[2][1] );
    q.y = copysign( q.y, m[2][0] - m[0][2] );
    q.z = copysign( q.z, m[0][1] - m[1][0] );
    return q;
}

float4 aa2q(float3 a, float r)
{
	float4 res = 0;
	float sinr = sin( r*PI );
	float cosr = cos( r*PI );
	res.x = a.x * sinr;
	res.y = a.y * sinr;
	res.z = a.z * sinr;
	res.w = cosr;
	return res;
}
float4 euler(float3 pyr)
{
	float4 res = aa2q(float3(0,1,0), pyr.y);
	res = qmul(res, aa2q(float3(1,0,0), pyr.x));
	res = qmul(res, aa2q(float3(0,0,1), pyr.z));
	return res;
}
float4 euler(float pp, float yy, float rr)
{
	float4 res = aa2q(float3(0,1,0), yy);
	res = qmul(res, aa2q(float3(1,0,0), pp));
	res = qmul(res, aa2q(float3(0,0,1), rr));
	return res;
}

float4 slerp (float4 a, float4 b, float t )
{
    if ( t <= 0.0f )
    {
        return a;
    }

    if ( t >= 1.0f )
    {
        return b;
    }

    float coshalftheta = dot(a, b);
    //coshalftheta = std::min (1.0f, std::max (-1.0f, coshalftheta));
    float4 c = b;

    // Angle is greater than 180. We can negate the angle/quat to get the
    // shorter rotation to reach the same destination.
    if ( coshalftheta < 0.0f )
    {
        coshalftheta = -coshalftheta;
        c = -c;
    }

        if ( coshalftheta > 0.99f )
        {
        // Angle is tiny - save some computation by lerping instead.
                float4 r = lerp(a, c, t);
                return r;
        }

    float halftheta = acos(coshalftheta);
    float sintheta = sin(halftheta);

    return (sin((1.0f - t) * halftheta) * a + sin(t * halftheta) * c) / sin(halftheta);
}
float4 qLookAt(float3 d, float3 fwvec)
{
	float3 dn = normalize(d);
	float3 fn = normalize(fwvec);
    float dfd = pows(dot(dn, fn), 1.5);
    if(dfd >= 0.9999)
		return float4(0,0,0,1);
    if(dfd <= -0.9999)
		return float4(0,1,0,0);

    float3 dfc = normalize(cross(fn, dn));

    float mangle = dfd * -0.25 + 0.25;
    float4 rot = aa2q(dfc, mangle);
    float eangle = clamp(dfd * 0.5, -0.5, 0);
    return qmul(aa2q(fn, eangle), rot);
}
