using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

using VVVV.Core;
using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;
using VVVV.Utils.VColor;

using Leap;

namespace VVVV.Nodes
{
    [PluginInfo(Name = "Hand", Category = "Leap", Tags = "")]
    public class LeapHandNode : IPluginEvaluate
    {
        [Output("Hands")]
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
        public ISpread<Vector3D> FPos;
        [Output("Wrist Position")]
        public ISpread<Vector3D> FPos;

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
        [Output("Fingers")]
        public ISpread<ISpread<Finger>> FFinger;
        [Output("Tools")]
        public ISpread<ISpread<Tool>> FTool;
        [Output("Pointables")]
        public ISpread<ISpread<Pointable>> FPointable;

        [Output("ID")]
        public ISpread<int> FID;
        [Output("Side")]
        public ISpread<bool> FSide;
        [Output("Age")]
        public ISpread<float> FAge;


        public void Evaluate(int SpreadMax)
        {
            if (FHand.IsConnected)
            {

            }
            else
            {

            }
        }
    }
}
