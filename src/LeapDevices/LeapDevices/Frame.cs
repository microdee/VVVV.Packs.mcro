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
    [PluginInfo(Name = "GetFrame", Category = "Leap", Tags = "")]
    public class LeapGetFrameNode : IPluginEvaluate
    {
        [Input("Controller")]
        public ISpread<Leap.Controller> FController;
        [Input("Frame ID in history")]
        public ISpread<int> FFID;
        [Output("Frame")]
        public ISpread<Frame> FFrame;

        public void Evaluate(int SpreadMax)
        {
            FFrame.SliceCount = FFID.SliceCount;
            for (int i = 0; i < FFID.SliceCount; i++)
            {
                FFrame[i] = FController[0].Frame(FFID[i]);
            }
        }
    }

    [PluginInfo(Name = "Frame", Category = "Leap", Tags = "")]
    public class LeapFrameNode : IPluginEvaluate
    {
        [Input("Frame")]
        public Pin<Frame> FFrame;

        [Output("FPS")]
        public ISpread<float> FFPS;
        [Output("Timestamp")]
        public ISpread<float> FTimestamp;
        [Output("Interaction Box")]
        public ISpread<InteractionBox> FInteractBox;

        [Output("Hands")]
        public ISpread<ISpread<Hand>> FHand;
        [Output("Gestures")]
        public ISpread<ISpread<Gesture>> FGesture;

        public void Evaluate(int SpreadMax)
        {
            if (!FFrame.IsConnected || FFrame.SliceCount == 0)
            {
                FFPS.SliceCount = 0;
                FTimestamp.SliceCount = 0;
                FInteractBox.SliceCount = 0;
                FHand.SliceCount = 0;
                FGesture.SliceCount = 0;
            }
            else
            {
                FFPS.SliceCount = FFrame.SliceCount;
                FTimestamp.SliceCount = FFrame.SliceCount;
                FInteractBox.SliceCount = FFrame.SliceCount;
                FHand.SliceCount = FFrame.SliceCount;
                FGesture.SliceCount = FFrame.SliceCount;

                for (int i = 0; i < FFrame.SliceCount; i++)
                {
                    FFPS[i] = FFrame[i].CurrentFramesPerSecond;
                    FTimestamp[i] = FFrame[i].Timestamp;
                    FInteractBox[i] = FFrame[i].InteractionBox;

                    FHand[i].SliceCount = 0;
                    FGesture[i].SliceCount = 0;
                    foreach (Hand h in FFrame[i].Hands) FHand[i].Add(h);

                    GestureList gests = FFrame[i].Gestures();
                    foreach (Gesture g in gests) FGesture[i].Add(g);
                }
            }
        }
    }

    [PluginInfo(Name = "Serialize", Category = "Leap", Version= "Frame", Tags = "")]
    public class FrameLeapSerializeNode : IPluginEvaluate
    {
        [Input("Frame")]
        public Pin<Frame> FFrame;
        [Input("Serialize")]
        public ISpread<bool> FSer;
        [Output("Output")]
        public ISpread<Stream> FStream;

        public void Evaluate(int SpreadMax)
        {
            FStream.SliceCount = FFrame.SliceCount;
            for (int i = 0; i < FFrame.SliceCount; i++)
            {
                if (FSer[i])
                {
                    byte[] SerializedFrame = FFrame[i].Serialize;
                    FStream[i] = new MemoryStream(SerializedFrame);
                }
            }
        }
    }

    [PluginInfo(Name = "DeSerialize", Category = "Leap", Version = "Frame", Tags = "")]
    public class FrameLeapDeSerializeNode : IPluginEvaluate
    {
        [Input("Input")]
        public Pin<Stream> FStream;
        [Input("DeSerialize")]
        public ISpread<bool> FDeSer;
        [Output("Frame")]
        public ISpread<Frame> FFrame;

        public void Evaluate(int SpreadMax)
        {
            FFrame.SliceCount = FStream.SliceCount;
            for (int i = 0; i < FStream.SliceCount; i++)
            {
                if (FDeSer[i])
                {
                    FFrame[i] = new Frame();
                    byte[] tmp = new byte[FStream[i].Length];
                    FStream[i].Position = 0;
                    FStream[i].Read(tmp, 0, (int)FStream[i].Length);
                    FFrame[i].Deserialize(tmp);
                }
            }
        }
    }
}
