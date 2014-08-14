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

    [PluginInfo(Name = "Gesture", Category = "Leap", Tags = "")]
    public class LeapGestureNode : LeapGesture<Gesture>
    {
        [Output("Pointables")]
        public ISpread<ISpread<Pointable>> FPointable;

        public override void SpecificEvaluate()
        {
            FPointable.SliceCount = FGesture.SliceCount;
            for (int i = 0; i < FGesture.SliceCount; i++)
            {
                FPointable[i].SliceCount = FGesture[i].Pointables.Count;
                for (int j = 0; j < FPointable[i].SliceCount; j++)
                {
                    FPointable[i][j] = FGesture[i].Pointables[j];
                }
            }
        }
        public override void SpecificOff()
        {
            FPointable.SliceCount = 0;
        }
    }

    [PluginInfo(Name = "Circle", Category = "Leap", Version="Gesture", Tags = "")]
    public class LeapCircleNode : LeapGesture<CircleGesture>
    {
        [Output("Center")]
        public ISpread<Vector3D> FCenter;
        [Output("Normal")]
        public ISpread<Vector3D> FNormal;
        [Output("Progress")]
        public ISpread<float> FProgress;
        [Output("Radius")]
        public ISpread<float> FRadius;
        [Output("Direction")]
        public ISpread<double> FCW;
        [Output("Pointable")]
        public ISpread<Pointable> FPointable;

        public override void SpecificEvaluate()
        {
            FCenter.SliceCount = FGesture.SliceCount;
            FNormal.SliceCount = FGesture.SliceCount;
            FProgress.SliceCount = FGesture.SliceCount;
            FRadius.SliceCount = FGesture.SliceCount;
            FPointable.SliceCount = FGesture.SliceCount;
            FCW.SliceCount = FGesture.SliceCount;
            
            for(int i=0; i<FGesture.SliceCount; i++)
            {
                FCenter[i] = FGesture[i].Center.ToVector3D().mulz(zm) * ScaleVal;
                FNormal[i] = FGesture[i].Normal.ToVector3D().mulz(zm);
                FProgress[i] = FGesture[i].Progress * (float)zm;
                FRadius[i] = FGesture[i].Radius * ScaleVal;
                FCW[i] = FGesture[i].Normal.AngleTo(FGesture[i].Pointable.Direction) / Math.PI - 0.5;
                FCW[i] *= -1;
                FPointable[i] = FGesture[i].Pointable;
            }
        }

        public override void SpecificOff()
        {
            FCenter.SliceCount = 0;
            FNormal.SliceCount = 0;
            FProgress.SliceCount = 0;
            FRadius.SliceCount = 0;
            FPointable.SliceCount = 0;
            FCW.SliceCount = 0;
        }
    }

    [PluginInfo(Name = "KeyTap", Category = "Leap", Version = "Gesture", Tags = "")]
    public class LeapKeyTapNode : LeapGesture<KeyTapGesture>
    {
        [Output("Position")]
        public ISpread<Vector3D> FPosition;
        [Output("Direction")]
        public ISpread<Vector3D> FDirection;
        [Output("Pointable")]
        public ISpread<Pointable> FPointable;

        public override void SpecificEvaluate()
        {
            FPosition.SliceCount = FGesture.SliceCount;
            FDirection.SliceCount = FGesture.SliceCount;
            FPointable.SliceCount = FGesture.SliceCount;

            for (int i = 0; i < FGesture.SliceCount; i++)
            {
                FPosition[i] = FGesture[i].Position.ToVector3D().mulz(zm) * ScaleVal;
                FDirection[i] = FGesture[i].Direction.ToVector3D().mulz(zm);
                FPointable[i] = FGesture[i].Pointable;
            }
        }

        public override void SpecificOff()
        {
            FPosition.SliceCount = 0;
            FDirection.SliceCount = 0;
            FPointable.SliceCount = 0;
        }
    }

    [PluginInfo(Name = "ScreenTap", Category = "Leap", Version = "Gesture", Tags = "")]
    public class LeapScreenTapNode : LeapGesture<ScreenTapGesture>
    {
        [Output("Position")]
        public ISpread<Vector3D> FPosition;
        [Output("Direction")]
        public ISpread<Vector3D> FDirection;
        [Output("Pointable")]
        public ISpread<Pointable> FPointable;

        public override void SpecificEvaluate()
        {
            FPosition.SliceCount = FGesture.SliceCount;
            FDirection.SliceCount = FGesture.SliceCount;
            FPointable.SliceCount = FGesture.SliceCount;

            for (int i = 0; i < FGesture.SliceCount; i++)
            {
                FPosition[i] = FGesture[i].Position.ToVector3D().mulz(zm) * ScaleVal;
                FDirection[i] = FGesture[i].Direction.ToVector3D().mulz(zm);
                FPointable[i] = FGesture[i].Pointable;
            }
        }

        public override void SpecificOff()
        {
            FPosition.SliceCount = 0;
            FDirection.SliceCount = 0;
            FPointable.SliceCount = 0;
        }
    }

    [PluginInfo(Name = "Swipe", Category = "Leap", Version = "Gesture", Tags = "")]
    public class LeapSwipeNode : LeapGesture<SwipeGesture>
    {
        [Output("Start Position")]
        public ISpread<Vector3D> FStartPosition;
        [Output("Position")]
        public ISpread<Vector3D> FPosition;
        [Output("Direction")]
        public ISpread<Vector3D> FDirection;
        [Output("Speed")]
        public ISpread<float> FSpeed;
        [Output("Pointable")]
        public ISpread<Pointable> FPointable;

        public override void SpecificEvaluate()
        {
            FStartPosition.SliceCount = FGesture.SliceCount;
            FPosition.SliceCount = FGesture.SliceCount;
            FDirection.SliceCount = FGesture.SliceCount;
            FSpeed.SliceCount = FGesture.SliceCount;
            FPointable.SliceCount = FGesture.SliceCount;

            for (int i = 0; i < FGesture.SliceCount; i++)
            {
                FStartPosition[i] = FGesture[i].StartPosition.ToVector3D().mulz(zm) * ScaleVal;
                FPosition[i] = FGesture[i].Position.ToVector3D().mulz(zm) * ScaleVal;
                FDirection[i] = FGesture[i].Direction.ToVector3D().mulz(zm);
                FSpeed[i] = FGesture[i].Speed * ScaleVal;
                FPointable[i] = FGesture[i].Pointable;
            }
        }

        public override void SpecificOff()
        {
            FStartPosition.SliceCount = 0;
            FPosition.SliceCount = 0;
            FDirection.SliceCount = 0;
            FSpeed.SliceCount = 0;
            FPointable.SliceCount = 0;
        }
    }
}
