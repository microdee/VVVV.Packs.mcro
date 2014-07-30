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
    [PluginInfo(Name = "Device", Category = "Leap", Tags = "")]
    public class LeapDeviceNode : IPluginEvaluate
    {
        [Output("Device")]
        public ISpread<Leap.Device> FDevice;

        [Output("Controller")]
        public ISpread<Leap.Controller> FController;

        Leap.Device leapdevice;
        Leap.Controller leapcontroller = new Controller();

        [ImportingConstructor]
        LeapDeviceNode()
        {
            leapcontroller.SetPolicyFlags(Controller.PolicyFlag.POLICY_BACKGROUND_FRAMES);
            for(int i=0; i<leapcontroller.Devices.Count; i++)
            {
                if(leapcontroller.Devices[i].IsValid)
                {
                    leapdevice = leapcontroller.Devices[i];
                    break;
                }
            }
            leapcontroller.EnableGesture(Gesture.GestureType.TYPE_CIRCLE);
            leapcontroller.EnableGesture(Gesture.GestureType.TYPE_KEY_TAP);
            leapcontroller.EnableGesture(Gesture.GestureType.TYPE_SCREEN_TAP);
            leapcontroller.EnableGesture(Gesture.GestureType.TYPE_SWIPE);
        }

        public void Evaluate(int SpreadMax)
        {
            if(leapdevice!=null)
            {
                FDevice[0] = leapdevice;
                FController[0] = leapcontroller;
            }
            else
            {
                FDevice.SliceCount = 0;
                FController.SliceCount = 0;
            }
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
                FViewAngle.SliceCount = 1;
                FRange.SliceCount = 1;
                FStreaming.SliceCount = 1;
                FDtB.SliceCount = FBoundPos.SliceCount;

                FViewAngle[0] = new Vector2D(FDevice[0].HorizontalViewAngle, FDevice[0].VerticalViewAngle);
                FRange[0] = FDevice[0].Range;
                FStreaming[0] = FDevice[0].IsStreaming;

                for(int i=0; i<FBoundPos.SliceCount; i++)
                {
                    Leap.Vector V = new Vector((float)FBoundPos[i].x, (float)FBoundPos[i].y, (float)FBoundPos[i].z);
                    FDtB[i] = FDevice[0].DistanceToBoundary(V);
                }
            }
        }
    }
}
