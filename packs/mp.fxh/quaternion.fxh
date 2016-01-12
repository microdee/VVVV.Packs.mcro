#define PI 3.1415926535897932
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
    float4x4 res = {
        1,0,0,0,
        0,1,0,0,
        0,0,1,0,
        0,0,0,1
    };
	res[0][0] = 1 - 2*q.y*q.y - 2*q.z*q.z;
	res[0][1] = 2*q.x*q.y - 2*q.w*q.z;
	res[0][2] = 2*q.x*q.z + 2*q.w*q.y;
	res[1][0] = 2*q.x*q.y + 2*q.w*q.z;
	res[1][1] = 1 - 2*q.x*q.x - 2*q.z*q.z;
	res[1][2] = 2*q.y*q.z + 2*q.w*q.x;
	res[2][0] = 2*q.x*q.z - 2*q.w*q.y;
	res[2][1] = 2*q.y*q.z - 2*q.w*q.x;
	res[2][2] = 1 - 2*q.x*q.x - 2*q.y*q.y;
	return res;
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
