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
    public abstract class LeapGesture<T> : IPluginEvaluate where T : Gesture
    {
        [Input("Gestures")]
        public Pin<T> FGesture;

        [Output("Hands")]
        public ISpread<ISpread<Hand>> FHand;
        [Output("ID")]
        public ISpread<int> FID;
        [Output("Type")]
        public ISpread<string> FType;
        [Output("State")]
        public ISpread<string> FState;
        [Output("Age")]
        public ISpread<double> FAge;

        public float ScaleVal;
        public float AgeCorrection;
        public double zm;
        public void ScaleEval()
        {
            try
            {
                ScaleVal = VVVV.Nodes.LeapDeviceNode.GlobalScale;
                zm = VVVV.Nodes.LeapDeviceNode.GlobalZMul;
                AgeCorrection = VVVV.Nodes.LeapDeviceNode.AgeCorrectionThreshold;
            }
            catch
            {
                ScaleVal = 1;
                zm = 1;
                AgeCorrection = 1500;
            }
        }
        public void GeneralEvaluate()
        {
            FHand.SliceCount = FGesture.SliceCount;
            FID.SliceCount = FGesture.SliceCount;
            FType.SliceCount = FGesture.SliceCount;
            FState.SliceCount = FGesture.SliceCount;
            FAge.SliceCount = FGesture.SliceCount;

            for (int i = 0; i < FGesture.SliceCount; i++)
            {

                FHand[i].SliceCount = FGesture[i].Hands.Count;
                for (int j = 0; j < FHand[i].SliceCount; j++)
                {
                    FHand[i][j] = FGesture[i].Hands[j];
                }

                FID[i] = FGesture[i].Id;
                FType[i] = FGesture[i].Type.ToString();
                FState[i] = FGesture[i].State.ToString();
                if (FGesture[i].DurationSeconds < AgeCorrection) FAge[i] = FGesture[i].DurationSeconds;
            }
        }
        public void GeneralOff()
        {
            FHand.SliceCount = 0;
            FID.SliceCount = 0;
            FType.SliceCount = 0;
            FState.SliceCount = 0;
            FAge.SliceCount = 0;
        }

        public abstract void SpecificEvaluate();
        public abstract void SpecificOff();

        public void Evaluate(int SpreadMax)
        {
            if (FGesture.IsConnected)
            {
                ScaleEval();
                GeneralEvaluate();
                SpecificEvaluate();
            }
            else
            {
                GeneralOff();
                SpecificOff();
            }
        }
    }
}
