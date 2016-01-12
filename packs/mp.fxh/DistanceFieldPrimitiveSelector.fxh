
StructuredBuffer<float> Prop;
/*
	(uint Sel)	(Name)		(Size in floats)	
	 0			sphere		1
	 1			sphereN		2
	 2			cylinder	3
	 3			cylinderN	4
	 4			plane		4
	 5			box			3
	 6			torus		2
	 7			torusN		4
	 8			cone		2
	 9			coneN		3
	10			capsule		7
	11			capsuleN	8
*/

float PSphere(float3 p, uint PropAddress)
{
	uint ii = PropAddress;
	return sphere(p,Prop[ii+0]);
}
float PSphereN(float3 p, uint PropAddress)
{
	uint ii = PropAddress;
	return sphere(p,Prop[ii+0],Prop[ii+1]);
}
float PCylinder(float3 p, uint PropAddress)
{
	uint ii = PropAddress;
	float3 cc;
	cc.x = Prop[ii+0];
	cc.y = Prop[ii+1];
	cc.z = Prop[ii+2];
	return cylinder(p,cc);
}
float PCylinderN(float3 p, uint PropAddress)
{
	uint ii = PropAddress;
	float3 cc;
	cc.x = Prop[ii+0];
	cc.y = Prop[ii+1];
	cc.z = Prop[ii+2];
	return cylinder(p,cc,Prop[ii+3]);
}
float PPlane(float3 p, uint PropAddress)
{
	uint ii = PropAddress;
	float4 cc;
	cc.x = Prop[ii+0];
	cc.y = Prop[ii+1];
	cc.z = Prop[ii+2];
	cc.w = Prop[ii+3];
	return plane(p,cc);
}
float PBox(float3 p, uint PropAddress)
{
	uint ii = PropAddress;
	float3 cc;
	cc.x = Prop[ii+0];
	cc.y = Prop[ii+1];
	cc.z = Prop[ii+2];
	return box(p,cc);
}
float PTorus(float3 p, uint PropAddress)
{
	uint ii = PropAddress;
	float2 cc;
	cc.x = Prop[ii+0];
	cc.y = Prop[ii+1];
	return torus(p,cc);
}
float PTorusN(float3 p, uint PropAddress)
{
	uint ii = PropAddress;
	float2 cc;
	cc.x = Prop[ii+0];
	cc.y = Prop[ii+1];
	float2 nn;
	nn.x = Prop[ii+2];
	nn.y = Prop[ii+3];
	return torus(p,cc,nn);
}
float PCone(float3 p, uint PropAddress)
{
	uint ii = PropAddress;
	float2 cc;
	cc.x = Prop[ii+0];
	cc.y = Prop[ii+1];
	return cone(p,cc);
}
float PConeN(float3 p, uint PropAddress)
{
	uint ii = PropAddress;
	float2 cc;
	cc.x = Prop[ii+0];
	cc.y = Prop[ii+1];
	return cone(p,cc,Prop[ii+2]);
}
float PCapsule(float3 p, uint PropAddress)
{
	uint ii = PropAddress;
	float3 aa;
	aa.x = Prop[ii+0];
	aa.y = Prop[ii+1];
	aa.z = Prop[ii+2];
	float3 bb;
	bb.x = Prop[ii+3];
	bb.y = Prop[ii+4];
	bb.z = Prop[ii+5];
	return capsule(p,aa,bb,Prop[ii+6]);
}
float PCapsuleN(float3 p, uint PropAddress)
{
	uint ii = PropAddress;
	float3 aa;
	aa.x = Prop[ii+0];
	aa.y = Prop[ii+1];
	aa.z = Prop[ii+2];
	float3 bb;
	bb.x = Prop[ii+3];
	bb.y = Prop[ii+4];
	bb.z = Prop[ii+5];
	return capsule(p,aa,bb,Prop[ii+6],Prop[ii+7]);
}

// selector:
float Primitive(float3 p, float Select, uint PropAddress)
{
	float res = 0;
	if(floor(Select)==0) res = PSphere(p, PropAddress);
	if(floor(Select)==1) res = PSphereN(p, PropAddress);
	if(floor(Select)==2) res = PCylinder(p, PropAddress);
	if(floor(Select)==3) res = PCylinderN(p, PropAddress);
	if(floor(Select)==4) res = PPlane(p, PropAddress);
	if(floor(Select)==5) res = PBox(p, PropAddress);
	if(floor(Select)==6) res = PTorus(p, PropAddress);
	if(floor(Select)==7) res = PTorusN(p, PropAddress);
	if(floor(Select)==8) res = PCone(p, PropAddress);
	if(floor(Select)==9) res = PConeN(p, PropAddress);
	if(floor(Select)==10) res = PCapsule(p, PropAddress);
	if(floor(Select)==11) res = PCapsuleN(p, PropAddress);
	return res;
}

interface iPrimitive{
	float Primitive(float3 p, uint i);
	uint PropSize();
};
class iSphere : iPrimitive{
	uint PropSize(){return 1;}
	float Primitive(float3 p, uint i)
	{
		uint ii = i*this.PropSize();
		return PSphere(p,ii);
	}
};
class iSphereN : iPrimitive{
	uint PropSize(){return 2;}
	float Primitive(float3 p, uint i)
	{
		uint ii = i*this.PropSize();
		return PSphereN(p,ii);
	}
};
class iCylinder : iPrimitive{
	uint PropSize(){return 3;}
	float Primitive(float3 p, uint i)
	{
		uint ii = i*this.PropSize();
		return PCylinder(p,ii);
	}
};
class iCylinderN : iPrimitive{
	uint PropSize(){return 4;}
	float Primitive(float3 p, uint i)
	{
		uint ii = i*this.PropSize();
		return PCylinderN(p,ii);
	}
};
class iPlane : iPrimitive{
	uint PropSize(){return 4;}
	float Primitive(float3 p, uint i)
	{
		uint ii = i*this.PropSize();
		return PPlane(p,ii);
	}
};
class iBox : iPrimitive{
	uint PropSize(){return 3;}
	float Primitive(float3 p, uint i)
	{
		uint ii = i*this.PropSize();
		return PBox(p,ii);
	}
};
class iTorus : iPrimitive{
	uint PropSize(){return 2;}
	float Primitive(float3 p, uint i)
	{
		uint ii = i*this.PropSize();
		return PTorus(p,ii);
	}
};
class iTorusN : iPrimitive{
	uint PropSize(){return 4;}
	float Primitive(float3 p, uint i)
	{
		uint ii = i*this.PropSize();
		return PTorusN(p,ii);
	}
};
class iCone : iPrimitive{
	uint PropSize(){return 2;}
	float Primitive(float3 p, uint i)
	{
		uint ii = i*this.PropSize();
		return PCone(p,ii);
	}
};
class iConeN : iPrimitive{
	uint PropSize(){return 3;}
	float Primitive(float3 p, uint i)
	{
		uint ii = i*this.PropSize();
		return PConeN(p,ii);
	}
};
class iCapsule : iPrimitive{
	uint PropSize(){return 7;}
	float Primitive(float3 p, uint i)
	{
		uint ii = i*this.PropSize();
		return PCapsule(p,ii);
	}
};
class iCapsuleN : iPrimitive{
	uint PropSize(){return 8;}
	float Primitive(float3 p, uint i)
	{
		uint ii = i*this.PropSize();
		return PCapsuleN(p,ii);
	}
};
iSphere Sphere;
iSphereN SphereN;
iCylinder Cylinder;
iCylinderN CylinderN;
iPlane Plane;
iBox Box;
iTorus Torus;
iTorusN TorusN;
iCone Cone;
iConeN ConeN;
iCapsule Capsule;
iCapsuleN CapsuleN;

iPrimitive iprim <string uiname="Primitive";string linkclass="Sphere,SphereN,Cylinder,CylinderN,Plane,Box,Torus,TorusN,Cone,ConeN,Capsule,CapsuleN";> = Sphere;
