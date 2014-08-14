using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.IO.MemoryMappedFiles;

using VVVV.Core;
using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;
using VVVV.Utils.VColor;
using VVVV.Utils.SharedMemory;

using Leap;

namespace VVVV.Nodes
{

    [PluginInfo(Name = "Hand", Category = "Leap", Tags = "")]
    public class LeapHandNode : IPluginEvaluate
    {
        [Input("Hands")]
        public Pin<Hand> FHand;

        [Output("Basis")]
        public ISpread<Matrix4x4> FBasis;
        [Output("Palm Position")]
        public ISpread<Vector3D> FPos;
        [Output("Stabilized Palm Position")]
        public ISpread<Vector3D> FStabilPos;
        [Output("Direction")]
        public ISpread<Vector3D> FDirection;
        [Output("Palm Normal")]
        public ISpread<Vector3D> FNormal;
        [Output("Palm Velocity")]
        public ISpread<Vector3D> FVel;
        [Output("Palm Width")]
        public ISpread<float> FWidth;
        [Output("Wrist Position")]
        public ISpread<Vector3D> FWristPos;

        [Output("Sphere Center")]
        public ISpread<Vector3D> FSphereC;
        [Output("Sphere Radius")]
        public ISpread<float> FSphereR;

        [Output("Confidence")]
        public ISpread<float> FConfidence;
        [Output("Grab")]
        public ISpread<float> FGrab;
        [Output("Pinch")]
        public ISpread<float> FPinch;

        [Output("Arm")]
        public ISpread<Arm> FArm;
        [Output("Pointables")]
        public ISpread<ISpread<Pointable>> FPointable;

        [Output("ID")]
        public ISpread<int> FID;
        [Output("Side")]
        public ISpread<bool> FSide;
        [Output("Age")]
        public ISpread<double> FAge;

        public void Evaluate(int SpreadMax)
        {
            if (FHand.IsConnected)
            {
                float gs;
                double zm;
                float AgeCorrection;
                try
                {
                    gs = VVVV.Nodes.LeapDeviceNode.GlobalScale;
                    zm = VVVV.Nodes.LeapDeviceNode.GlobalZMul;
                    AgeCorrection = VVVV.Nodes.LeapDeviceNode.AgeCorrectionThreshold;
                }
                catch
                {
                    gs = 1;
                    zm = 1;
                    AgeCorrection = 1500;
                }

                FBasis.SliceCount = FHand.SliceCount;
                FPos.SliceCount = FHand.SliceCount;
                FStabilPos.SliceCount = FHand.SliceCount;
                FDirection.SliceCount = FHand.SliceCount;
                FNormal.SliceCount = FHand.SliceCount;
                FVel.SliceCount = FHand.SliceCount;
                FWidth.SliceCount = FHand.SliceCount;
                FWristPos.SliceCount = FHand.SliceCount;
                FSphereC.SliceCount = FHand.SliceCount;
                FSphereR.SliceCount = FHand.SliceCount;
                FConfidence.SliceCount = FHand.SliceCount;
                FGrab.SliceCount = FHand.SliceCount;
                FPinch.SliceCount = FHand.SliceCount;
                FArm.SliceCount = FHand.SliceCount;
                FPointable.SliceCount = FHand.SliceCount;
                FID.SliceCount = FHand.SliceCount;
                FSide.SliceCount = FHand.SliceCount;
                FAge.SliceCount = FHand.SliceCount;

                for(int i=0; i<FHand.SliceCount; i++)
                {
                    FBasis[i] = FHand[i].Basis.ToMatrix4x4().Transpose().mulz(zm);
                    FPos[i] = FHand[i].PalmPosition.ToVector3D().mulz(zm) * gs;
                    FStabilPos[i] = FHand[i].StabilizedPalmPosition.ToVector3D().mulz(zm) * gs;
                    FDirection[i] = FHand[i].Direction.ToVector3D().mulz(zm);
                    FNormal[i] = FHand[i].PalmNormal.ToVector3D().mulz(zm);
                    FVel[i] = FHand[i].PalmVelocity.ToVector3D().mulz(zm) * gs;
                    FWristPos[i] = FHand[i].WristPosition.ToVector3D().mulz(zm) * gs;
                    FSphereC[i] = FHand[i].SphereCenter.ToVector3D().mulz(zm) * gs;

                    FSphereR[i] = FHand[i].SphereRadius * gs;
                    FConfidence[i] = FHand[i].Confidence;
                    FGrab[i] = FHand[i].GrabStrength;
                    FPinch[i] = FHand[i].PinchStrength;
                    FWidth[i] = FHand[i].PalmWidth * gs;

                    if (FHand[i].TimeVisible < AgeCorrection) FAge[i] = FHand[i].TimeVisible;
                    FID[i] = FHand[i].Id;
                    FSide[i] = FHand[i].IsRight;

                    FArm[i] = FHand[i].Arm;
                    FPointable[i].SliceCount = 0;
                    foreach (Pointable f in FHand[i].Pointables) FPointable[i].Add(f);
                }
            }
            else
            {
                FBasis.SliceCount = 0;
                FPos.SliceCount = 0;
                FStabilPos.SliceCount = 0;
                FDirection.SliceCount = 0;
                FNormal.SliceCount = 0;
                FVel.SliceCount = 0;
                FWidth.SliceCount = 0;
                FWristPos.SliceCount = 0;
                FSphereC.SliceCount = 0;
                FSphereR.SliceCount = 0;
                FConfidence.SliceCount = 0;
                FGrab.SliceCount = 0;
                FPinch.SliceCount = 0;
                FArm.SliceCount = 0;
                FPointable.SliceCount = 0;
                FID.SliceCount = 0;
                FSide.SliceCount = 0;
                FAge.SliceCount = 0;
            }
        }
    }

    [PluginInfo(Name = "Bone", Category = "Leap", Tags = "")]
    public class LeapBoneNode : IPluginEvaluate
    {
        [Input("Bones")]
        public Pin<Bone> FBone;

        [Output("Start")]
        public ISpread<Vector3D> FProx;
        [Output("Center")]
        public ISpread<Vector3D> FCenter;
        [Output("End")]
        public ISpread<Vector3D> FDistal;
        [Output("Direction")]
        public ISpread<Vector3D> FDirection;
        [Output("Basis")]
        public ISpread<Matrix4x4> FBasis;

        [Output("Width")]
        public ISpread<float> FWidth;
        [Output("Length")]
        public ISpread<float> FLength;

        [Output("Type")]
        public ISpread<string> FType;

        public void Evaluate(int SpreadMax)
        {
            if (FBone.IsConnected)
            {
                float ScaleVal;
                double zm;
                try
                {
                    ScaleVal = VVVV.Nodes.LeapDeviceNode.GlobalScale;
                    zm = VVVV.Nodes.LeapDeviceNode.GlobalZMul;
                }
                catch
                {
                    ScaleVal = 1;
                    zm = 1;
                }

                FProx.SliceCount = FBone.SliceCount;
                FDistal.SliceCount = FBone.SliceCount;
                FDirection.SliceCount = FBone.SliceCount;
                FCenter.SliceCount = FBone.SliceCount;
                FWidth.SliceCount = FBone.SliceCount;
                FLength.SliceCount = FBone.SliceCount;
                FBasis.SliceCount = FBone.SliceCount;
                FType.SliceCount = FBone.SliceCount;

                for (int i = 0; i < FBone.SliceCount; i++)
                {
                    FProx[i] = FBone[i].PrevJoint.ToVector3D().mulz(zm) * ScaleVal;
                    FDistal[i] = FBone[i].NextJoint.ToVector3D().mulz(zm) * ScaleVal;
                    FDirection[i] = FBone[i].Direction.ToVector3D().mulz(zm);
                    FCenter[i] = FBone[i].Center.ToVector3D().mulz(zm) * ScaleVal;
                    FWidth[i] = FBone[i].Width * ScaleVal;
                    FLength[i] = FBone[i].Length * ScaleVal;

                    FBasis[i] = FBone[i].Basis.ToMatrix4x4().Transpose().mulz(zm);
                    FType[i] = FBone[i].Type.ToString();
                }
            }
            else
            {
                FProx.SliceCount = 0;
                FDistal.SliceCount = 0;
                FDirection.SliceCount = 0;
                FCenter.SliceCount = 0;
                FWidth.SliceCount = 0;
                FLength.SliceCount = 0;
                FBasis.SliceCount = 0;
                FType.SliceCount = 0;
            }
        }
    }

    [PluginInfo(Name = "Arm", Category = "Leap", Tags = "")]
    public class LeapArmNode : IPluginEvaluate
    {
        [Input("Arms")]
        public Pin<Arm> FArm;

        [Output("Elbow")]
        public ISpread<Vector3D> FElbow;
        [Output("Wrist")]
        public ISpread<Vector3D> FWrist;
        [Output("Direction")]
        public ISpread<Vector3D> FDirection;
        [Output("Basis")]
        public ISpread<Matrix4x4> FBasis;

        [Output("Width")]
        public ISpread<float> FWidth;

        public void Evaluate(int SpreadMax)
        {
            if (FArm.IsConnected)
            {
                float ScaleVal;
                double zm;
                try
                {
                    ScaleVal = VVVV.Nodes.LeapDeviceNode.GlobalScale;
                    zm = VVVV.Nodes.LeapDeviceNode.GlobalZMul;
                }
                catch
                {
                    ScaleVal = 1;
                    zm = 1;
                }

                FElbow.SliceCount = FArm.SliceCount;
                FDirection.SliceCount = FArm.SliceCount;
                FWrist.SliceCount = FArm.SliceCount;
                FWidth.SliceCount = FArm.SliceCount;
                FBasis.SliceCount = FArm.SliceCount;

                for (int i = 0; i < FArm.SliceCount; i++)
                {
                    FElbow[i] = FArm[i].ElbowPosition.ToVector3D().mulz(zm) * ScaleVal;
                    FDirection[i] = FArm[i].Direction.ToVector3D().mulz(zm);
                    FWrist[i] = FArm[i].WristPosition.ToVector3D().mulz(zm) * ScaleVal;
                    FWidth[i] = FArm[i].Width * ScaleVal;

                    FBasis[i] = FArm[i].Basis.ToMatrix4x4().Transpose().mulz(zm);
                }
            }
            else
            {
                FElbow.SliceCount = 0;
                FDirection.SliceCount = 0;
                FWrist.SliceCount = 0;
                FWidth.SliceCount = 0;
                FBasis.SliceCount = 0;
            }
        }
    }

    [PluginInfo(Name = "Pointable", Category = "Leap", Tags = "")]
    public class LeapPointableNode : LeapPointable<Pointable>
    {
        public override void SpecificEvaluate()
        { }
        public override void SpecificOff()
        { }
    }

    [PluginInfo(Name = "Finger", Category = "Leap", Tags = "")]
    public class LeapFingerNode : LeapPointable<Finger>
    {
        [Output("Bones")]
        public ISpread<ISpread<Bone>> FBone;
        [Output("Type")]
        public ISpread<string> FType;

        public override void SpecificEvaluate()
        {
            FBone.SliceCount = FPointable.SliceCount;
            FType.SliceCount = FPointable.SliceCount;
            for (int i = 0; i < FPointable.SliceCount; i++)
            {
                FType[i] = FPointable[i].Type().ToString();

                FBone[i].SliceCount = 0;
                foreach (Bone.BoneType boneType in (Bone.BoneType[])Enum.GetValues(typeof(Bone.BoneType)))
                {
                    FBone[i].Add(FPointable[i].Bone(boneType));
                }
            }
        }
        public override void SpecificOff()
        {
            FBone.SliceCount = 0;
            FType.SliceCount = 0;
        }
    }
}
