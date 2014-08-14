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
    public abstract class LeapPointable<T> : IPluginEvaluate where T : Pointable
    {
        [Input("Pointables")]
        public Pin<T> FPointable;

        [Output("Tip Position")]
        public ISpread<Vector3D> FPos;
        [Output("Stabilized Tip Position")]
        public ISpread<Vector3D> FStabilPos;
        [Output("Direction")]
        public ISpread<Vector3D> FDirection;
        [Output("Tip Velocity")]
        public ISpread<Vector3D> FVel;

        [Output("Width")]
        public ISpread<float> FWidth;
        [Output("Length")]
        public ISpread<float> FLength;

        [Output("Touch Distance")]
        public ISpread<float> FTouchDist;

        [Output("Extended")]
        public ISpread<bool> FExtended;
        [Output("Tool")]
        public ISpread<bool> FIsTool;

        [Output("ID")]
        public ISpread<int> FID;
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
            FPos.SliceCount = FPointable.SliceCount;
            FStabilPos.SliceCount = FPointable.SliceCount;
            FDirection.SliceCount = FPointable.SliceCount;
            FVel.SliceCount = FPointable.SliceCount;
            FWidth.SliceCount = FPointable.SliceCount;
            FLength.SliceCount = FPointable.SliceCount;
            FTouchDist.SliceCount = FPointable.SliceCount;
            FExtended.SliceCount = FPointable.SliceCount;
            FIsTool.SliceCount = FPointable.SliceCount;
            FID.SliceCount = FPointable.SliceCount;
            FAge.SliceCount = FPointable.SliceCount;

            for (int i = 0; i < FPointable.SliceCount; i++)
            {
                FPos[i] = FPointable[i].TipPosition.ToVector3D().mulz(zm) * ScaleVal;
                FStabilPos[i] = FPointable[i].StabilizedTipPosition.ToVector3D().mulz(zm) * ScaleVal;
                FDirection[i] = FPointable[i].Direction.ToVector3D().mulz(zm);
                FVel[i] = FPointable[i].TipVelocity.ToVector3D().mulz(zm) * ScaleVal;
                FWidth[i] = FPointable[i].Width * ScaleVal;
                FLength[i] = FPointable[i].Length * ScaleVal;

                FTouchDist[i] = FPointable[i].TouchDistance;
                FExtended[i] = FPointable[i].IsExtended;
                FIsTool[i] = FPointable[i].IsTool;

                if (FPointable[i].TimeVisible < AgeCorrection) FAge[i] = FPointable[i].TimeVisible;
                FID[i] = FPointable[i].Id;
            }
        }
        public void GeneralOff()
        {
            FPos.SliceCount = 0;
            FStabilPos.SliceCount = 0;
            FDirection.SliceCount = 0;
            FVel.SliceCount = 0;
            FWidth.SliceCount = 0;
            FLength.SliceCount = 0;
            FTouchDist.SliceCount = 0;
            FExtended.SliceCount = 0;
            FIsTool.SliceCount = 0;
            FPointable.SliceCount = 0;
            FID.SliceCount = 0;
            FAge.SliceCount = 0;
        }

        public abstract void SpecificEvaluate();
        public abstract void SpecificOff();

        public void Evaluate(int SpreadMax)
        {
            if (FPointable.IsConnected)
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
