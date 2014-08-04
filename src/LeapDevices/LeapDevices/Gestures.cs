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
    [PluginInfo(Name = "Gestures", Category = "Leap", Version ="Split", Tags = "")]
    public class SplitLeapGesturesNode : IPluginEvaluate
    {
        public enum LeapConfig
        {
            None,
            Circle_MinRadius,
            Circle_MinArc,
            Swipe_MinLength,
            Swipe_MinVelocity,
            KeyTap_MinDownVelocity,
            KeyTap_HistorySeconds,
            KeyTap_MinDistance,
            ScreenTap_MinForwardVelocity,
            ScreenTap_HistorySeconds,
            ScreenTap_MinDistance
        };
        [Input("Gestures")]
        public Pin<Gesture> FGesture;

        [Input("Configuration Key", DefaultEnumEntry="None")]
        public ISpread<LeapConfig> FConfig;
        [Input("Configuration Value")]
        public ISpread<float> FConfigVal;
        [Input("Set Configuration")]
        public ISpread<bool> FConfigSet;

        [Output("Circle")]
        public ISpread<CircleGesture> FCircle;
        [Output("Key Tap")]
        public ISpread<KeyTapGesture> FKeyTap;
        [Output("Screen Tap")]
        public ISpread<ScreenTapGesture> FScreenTap;
        [Output("Swipe")]
        public ISpread<SwipeGesture> FSwipe;

        public void Evaluate(int SpreadMax)
        {

            FCircle.SliceCount = 0;
            FKeyTap.SliceCount = 0;
            FScreenTap.SliceCount = 0;
            FSwipe.SliceCount = 0;

            if (FGesture.IsConnected)
            {
                for(int i=0; i<FGesture.SliceCount; i++)
                {
                    if (FGesture[i].Type == Gesture.GestureType.TYPE_CIRCLE)
                        FCircle.Add(new CircleGesture(FGesture[i]));
                    
                    if (FGesture[i].Type == Gesture.GestureType.TYPE_KEY_TAP)
                        FKeyTap.Add(new KeyTapGesture(FGesture[i]));

                    if (FGesture[i].Type == Gesture.GestureType.TYPE_SCREEN_TAP)
                        FScreenTap.Add(new ScreenTapGesture(FGesture[i]));

                    if (FGesture[i].Type == Gesture.GestureType.TYPE_SWIPE)
                        FSwipe.Add(new SwipeGesture(FGesture[i]));
                }
            }
            if (VVVV.Nodes.LeapDeviceNode.leapcontroller != null)
            {
                Controller leapctrl = VVVV.Nodes.LeapDeviceNode.leapcontroller;
                if (FConfigSet[0])
                {
                    for (int i = 0; i < FConfig.SliceCount; i++)
                    {
                        switch (FConfig[i])
                        {
                            case LeapConfig.Circle_MinRadius:
                                leapctrl.Config.SetFloat("Gesture.Circle.MinRadius", FConfigVal[i]);
                                break;

                            case LeapConfig.Circle_MinArc:
                                leapctrl.Config.SetFloat("Gesture.Circle.MinArc", FConfigVal[i]);
                                break;

                            case LeapConfig.Swipe_MinLength:
                                leapctrl.Config.SetFloat("Gesture.Swipe.MinLength", FConfigVal[i]);
                                break;

                            case LeapConfig.Swipe_MinVelocity:
                                leapctrl.Config.SetFloat("Gesture.Swipe.MinVelocity", FConfigVal[i]);
                                break;

                            case LeapConfig.KeyTap_MinDownVelocity:
                                leapctrl.Config.SetFloat("Gesture.KeyTap.MinDownVelocity", FConfigVal[i]);
                                break;

                            case LeapConfig.KeyTap_HistorySeconds:
                                leapctrl.Config.SetFloat("Gesture.KeyTap.HistorySeconds", FConfigVal[i]);
                                break;

                            case LeapConfig.KeyTap_MinDistance:
                                leapctrl.Config.SetFloat("Gesture.KeyTap.MinDistance", FConfigVal[i]);
                                break;

                            case LeapConfig.ScreenTap_MinForwardVelocity:
                                leapctrl.Config.SetFloat("Gesture.ScreenTap.MinForwardVelocity", FConfigVal[i]);
                                break;

                            case LeapConfig.ScreenTap_HistorySeconds:
                                leapctrl.Config.SetFloat("Gesture.ScreenTap.HistorySeconds", FConfigVal[i]);
                                break;

                            case LeapConfig.ScreenTap_MinDistance:
                                leapctrl.Config.SetFloat("Gesture.ScreenTap.MinDistance", FConfigVal[i]);
                                break;
                        }
                    }
                    leapctrl.Config.Save();
                }
            }
        }
    }

    public abstract class LeapGestureNodes<T> : IPluginEvaluate where T : Gesture
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
        public double zm;
        public void ScaleEval()
        {
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
        }
        public void GeneralEvaluate()
        {
            FHand.SliceCount = FGesture.SliceCount;
            FID.SliceCount = FGesture.SliceCount;
            FType.SliceCount = FGesture.SliceCount;
            FState.SliceCount = FGesture.SliceCount;
            FAge.SliceCount = FGesture.SliceCount;

            for(int i=0; i<FGesture.SliceCount; i++)
            {

                FHand[i].SliceCount = FGesture[i].Hands.Count;
                for(int j=0; j<FHand[i].SliceCount; j++)
                {
                    FHand[i][j] = FGesture[i].Hands[j];
                }

                FID[i] = FGesture[i].Id;
                FType[i] = FGesture[i].Type.ToString();
                FState[i] = FGesture[i].State.ToString();
                if (FGesture[i].DurationSeconds < 5000) FAge[i] = FGesture[i].DurationSeconds;
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
    [PluginInfo(Name = "Gesture", Category = "Leap", Tags = "")]
    public class LeapGestureNode : LeapGestureNodes<Gesture>
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
    public class LeapCircleNode : LeapGestureNodes<CircleGesture>
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
                FProgress[i] = FGesture[i].Progress;
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
    public class LeapKeyTapNode : LeapGestureNodes<KeyTapGesture>
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
    public class LeapScreenTapNode : LeapGestureNodes<ScreenTapGesture>
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
    public class LeapSwipeNode : LeapGestureNodes<SwipeGesture>
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
