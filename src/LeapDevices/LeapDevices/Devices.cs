using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.IO;
using System.IO.MemoryMappedFiles;
using SlimDX;

using VVVV.Core;
using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;
using VVVV.Utils.VColor;
using VVVV.Utils.SharedMemory;

using Leap;

namespace VVVV.Nodes
{
    public static class TrackingHelper
    {
        public static Matrix4x4 ToMatrix4x4(this Leap.Matrix m)
        {
            Matrix4x4 tmp = new Matrix4x4();
            tmp.m11 = m.xBasis.x;
            tmp.m12 = m.yBasis.x;
            tmp.m13 = m.zBasis.x;
            tmp.m21 = m.xBasis.y;
            tmp.m22 = m.yBasis.y;
            tmp.m23 = m.zBasis.y;
            tmp.m31 = m.xBasis.z;
            tmp.m32 = m.yBasis.z;
            tmp.m33 = m.zBasis.z;
            tmp.m44 = 1;
            return tmp;
        }
        public static Vector3D ToVector3D(this Leap.Vector V)
        {
            Vector3D tmp = new Vector3D((double)V.x, (double)V.y, (double)V.z);
            return tmp;
        }
        public static Vector ToLeapVector(this Vector3D V)
        {
            Vector tmp = new Vector((float)V.x, (float)V.y, (float)V.z);
            return tmp;
        }
        public static Vector3D mul(this Vector3D V, Matrix4x4 m)
        {
            Vector4 tmpv = SlimDX.Vector3.Transform(V.ToSlimDXVector(), m.ToSlimDXMatrix());
            Vector3D tmp = new Vector3D(tmpv.X, tmpv.Y, tmpv.Z);
            return tmp;
        }
        public static Vector3D mulz(this Vector3D V, double m)
        {
            V.z *= m;
            return V;
        }
        public static Matrix4x4 mulz(this Matrix4x4 m, double zm)
        {
            m.col3 *= zm;
            return m;
        }
    }

    [PluginInfo(Name = "Device", Category = "Leap", Tags = "", AutoEvaluate=true)]
    public class LeapDeviceNode : IPluginEvaluate
    {
        [Input("Scale")]
        public Pin<float> FScale;
        [Input("Mirror Z", DefaultValue = 1.0)]
        public Pin<bool> FMirror;

        [Input("Reinitialize", IsBang=true)]
        public ISpread<bool> FReinit;
        [Input("Device ID", DefaultValue=-1)]
        public ISpread<int> FDID;

        [Input("Age Filtering Threshold", Visibility = PinVisibility.OnlyInspector, DefaultValue=1500)]
        public Pin<float> FAgeThreshold;
        
        [Output("Device")]
        public ISpread<Leap.Device> FDevice;

        [Output("Controller")]
        public ISpread<Leap.Controller> FController;

        public static Leap.Device leapdevice;
        public static Leap.Controller leapcontroller;

        public static float GlobalScale = (float)0.01;
        public static float AgeCorrectionThreshold = (float)1500;
        public static double GlobalZMul = 1;

        private void leapinit()
        {
            leapcontroller = new Controller();
            leapcontroller.SetPolicyFlags(Controller.PolicyFlag.POLICY_BACKGROUND_FRAMES);
            leapcontroller.EnableGesture(Gesture.GestureType.TYPE_CIRCLE);
            leapcontroller.EnableGesture(Gesture.GestureType.TYPE_KEY_TAP);
            leapcontroller.EnableGesture(Gesture.GestureType.TYPE_SCREEN_TAP);
            leapcontroller.EnableGesture(Gesture.GestureType.TYPE_SWIPE);

            leapdevice = leapcontroller.Devices[0];
        }

        public LeapDeviceNode()
        {
            leapinit();
        }

        public void Evaluate(int SpreadMax)
        {
            if(leapcontroller!=null)
            {
                FDevice[0] = leapdevice;
                FController[0] = leapcontroller;
                leapdevice = leapcontroller.Devices[FDID[0]];
            }
            else
            {
                FDevice.SliceCount = 0;
                FController.SliceCount = 0;
                if (FReinit[0])
                {
                    leapcontroller.Dispose();
                    leapinit();
                }
            }
            GlobalScale = FScale[0];
            GlobalZMul = (FMirror[0]) ? -1 : 1;
            AgeCorrectionThreshold = FAgeThreshold[0];
        }
    }
    
    [PluginInfo(Name = "Frame", Category = "Leap", Tags = "")]
    public class LeapFrameNode : IPluginEvaluate
    {
        [Input("Controller")]
        public Pin<Leap.Controller> FController;

        [Output("FPS")]
        public ISpread<float> FFPS;
        [Output("Timestamp")]
        public ISpread<float> FTimestamp;
        [Output("Interaction Box")]
        public ISpread<InteractionBox> FInteractBox;

        [Output("Hands")]
        public ISpread<Hand> FHand;
        [Output("Gestures")]
        public ISpread<Gesture> FGesture;

        public void Evaluate(int SpreadMax)
        {
            if(!FController.IsConnected || FController.SliceCount == 0)
            {
                FFPS.SliceCount = 0;
                FTimestamp.SliceCount = 0;
                FInteractBox.SliceCount = 0;
                FHand.SliceCount = 0;
                FGesture.SliceCount = 0;
            }
            else
            {
                Frame frame = FController[0].Frame(0);
                FFPS.SliceCount = 1;
                FTimestamp.SliceCount = 1;
                FInteractBox.SliceCount = 1;

                FFPS[0] = frame.CurrentFramesPerSecond;
                FTimestamp[0] = frame.Timestamp;
                FInteractBox[0] = frame.InteractionBox;

                FHand.SliceCount = 0;
                FGesture.SliceCount = 0;
                foreach (Hand h in frame.Hands) FHand.Add(h);

                GestureList gests = frame.Gestures();
                foreach (Gesture g in gests) FGesture.Add(g);
            }
        }
    }

    [PluginInfo(Name = "DeviceInfo", Category = "Leap", Tags = "")]
    public class LeapDeviceInfoNode : IPluginEvaluate
    {
        [Input("Device")]
        public Pin<Leap.Device> FDevice;
        [Input("Boundary Reference Position")]
        public ISpread<Vector3D> FBoundPos;

        [Output("View Angles")]
        public ISpread<Vector2D> FViewAngle;
        [Output("Range")]
        public ISpread<float> FRange;
        [Output("Distance To Boundary")]
        public ISpread<float> FDtB;
        [Output("Streaming")]
        public ISpread<bool> FStreaming;

        public void Evaluate(int SpreadMax)
        {

            if (!FDevice.IsConnected || FDevice.SliceCount == 0)
            {
                FViewAngle.SliceCount = 0;
                FRange.SliceCount = 0;
                FDtB.SliceCount = 0;
                FStreaming.SliceCount = 0;
            }
            else
            {
                float gs;
                double zm;
                try {
                    gs = VVVV.Nodes.LeapDeviceNode.GlobalScale;
                    zm = VVVV.Nodes.LeapDeviceNode.GlobalZMul;
                }
                catch {
                    gs = 1;
                    zm = 1;
                }

                FViewAngle.SliceCount = 1;
                FRange.SliceCount = 1;
                FStreaming.SliceCount = 1;
                FDtB.SliceCount = FBoundPos.SliceCount;

                FViewAngle[0] = new Vector2D(FDevice[0].HorizontalViewAngle, FDevice[0].VerticalViewAngle);
                FViewAngle[0] /= 2 * Math.PI;
                FRange[0] = FDevice[0].Range * gs;
                FStreaming[0] = FDevice[0].IsStreaming;

                for(int i=0; i<FBoundPos.SliceCount; i++)
                {
                    Vector3D tpos = FBoundPos[i] / gs;
                    tpos.z *= zm;
                    Leap.Vector V = tpos.ToLeapVector();
                    FDtB[i] = FDevice[0].DistanceToBoundary(V) * gs;
                }
            }
        }
    }

    [PluginInfo(Name = "InteractionBox", Category = "Leap", Tags = "")]
    public class LeapInteractionBoxNode : IPluginEvaluate
    {
        [Input("Controller")]
        public Pin<Leap.Controller> FController;
        [Input("World Position")]
        public ISpread<Vector3D> FWorldPos;

        [Output("Dimensions")]
        public ISpread<Vector3D> FDimensions;
        [Output("Center")]
        public ISpread<Vector3D> FCenter;
        [Output("Normalized Position")]
        public ISpread<Vector3D> FNormPos;

        public void Evaluate(int SpreadMax)
        {

            if (!FController.IsConnected || FController.SliceCount == 0)
            {
                FDimensions.SliceCount = 0;
                FNormPos.SliceCount = 0;
                FCenter.SliceCount = 0;
            }
            else
            {
                float gs;
                double zm;
                try
                {
                    gs = VVVV.Nodes.LeapDeviceNode.GlobalScale;
                    zm = VVVV.Nodes.LeapDeviceNode.GlobalZMul;
                }
                catch
                {
                    gs = 1;
                    zm = 1;
                }

                FDimensions.SliceCount = 1;
                FCenter.SliceCount = 1;
                FNormPos.SliceCount = FWorldPos.SliceCount;

                FDimensions[0] = new Vector3D(
                    FController[0].Frame(0).InteractionBox.Width,
                    FController[0].Frame(0).InteractionBox.Height,
                    FController[0].Frame(0).InteractionBox.Depth);
                FDimensions[0] = FDimensions[0] * gs;

                FCenter[0] = FController[0].Frame(0).InteractionBox.Center.ToVector3D().mulz(zm) * gs;

                for (int i = 0; i < FWorldPos.SliceCount; i++)
                {
                    Vector3D tpos = FWorldPos[i].mulz(zm) / gs;
                    Leap.Vector V = tpos.ToLeapVector();
                    FNormPos[i] = FController[0].Frame(0).InteractionBox.NormalizePoint(V).ToVector3D().mulz(zm);
                }
            }
        }
    }
}
