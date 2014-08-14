using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.IO.MemoryMappedFiles;
using System.Diagnostics;

using VVVV.Core;
using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;
using VVVV.Utils.VColor;
using VVVV.Utils.SharedMemory;

using Leap;

namespace VVVV.Nodes
{
    [PluginInfo(Name = "Pointables", Category = "Leap", Version = "Split", Tags = "")]
    public class SplitLeapPointablesNode : IPluginEvaluate
    {
        [Input("Pointables")]
        public ISpread<ISpread<Pointable>> FPointable;

        [Output("Finger")]
        public ISpread<ISpread<Finger>> FFinger;
        [Output("Tool")]
        public ISpread<ISpread<Pointable>> FTool;

        public void Evaluate(int SpreadMax)
        {
            try
            {
                FFinger.SliceCount = FPointable.SliceCount;
                FTool.SliceCount = FPointable.SliceCount;

                for (int i = 0; i < FPointable.SliceCount; i++)
                {
                    FFinger[i].SliceCount = 0;
                    FTool[i].SliceCount = 0;
                    for (int j = 0; j < FPointable[i].SliceCount; j++)
                    {
                        if (FPointable[i][j].IsFinger) FFinger[i].Add(new Finger(FPointable[i][j]));
                        else FTool[i].Add(FPointable[i][j]);
                    }
                }
            }
            catch
            {
                FFinger.SliceCount = 0;
                FTool.SliceCount = 0;
            }
        }
    }

    [PluginInfo(Name = "FingerList", Category = "Leap", Tags = "")]
    public class LeapFingerListNode : IPluginEvaluate
    {
        public enum FilterFinger
        {
            All,
            Thumb,
            Index,
            Middle,
            Ring,
            Pinky
        };

        [Input("Hands")]
        public Pin<Hand> FHand;
        [Input("Filter")]
        public Pin<FilterFinger> FFilter;

        [Output("Frontmost")]
        public ISpread<Finger> FFM;
        [Output("Leftmost")]
        public ISpread<Finger> FLM;
        [Output("Rightmost")]
        public ISpread<Finger> FRM;
        [Output("Fingers")]
        public ISpread<ISpread<Finger>> FAll;

        public void Evaluate(int SpreadMax)
        {
            if (FHand.IsConnected)
            {
                FFM.SliceCount = FHand.SliceCount;
                FLM.SliceCount = FHand.SliceCount;
                FRM.SliceCount = FHand.SliceCount;
                FAll.SliceCount = FHand.SliceCount;

                for (int i = 0; i < FHand.SliceCount; i++)
                {
                    FFM[i] = FHand[i].Fingers.Frontmost;
                    FLM[i] = FHand[i].Fingers.Leftmost;
                    FRM[i] = FHand[i].Fingers.Rightmost;

                    FAll[i].SliceCount = 0;

                    for (int j = 0; j < FFilter.SliceCount; j++)
                    {
                        if(FFilter[j] == FilterFinger.All)
                            foreach (Finger f in FHand[i].Fingers) FAll[i].Add(f);
                        else
                        {
                            int address = (int)FFilter[j] - 1;
                            Finger tmp = FHand[i].Fingers.FingerType((Finger.FingerType)address)[0];
                            FAll[i].Add(tmp);
                        }
                    }
                }
            }
            else
            {
                FFM.SliceCount = 0;
                FLM.SliceCount = 0;
                FRM.SliceCount = 0;
                FAll.SliceCount = 0;
            }
        }
    }

    [PluginInfo(Name = "Gestures", Category = "Leap", Version = "Split", Tags = "")]
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

        [Input("Configuration Key", DefaultEnumEntry = "None")]
        public ISpread<LeapConfig> FConfig;
        [Input("Configuration Value")]
        public ISpread<float> FConfigVal;
        [Input("Set Configuration")]
        public ISpread<bool> FConfigSet;

        [Output("Circle")]
        public ISpread<CircleGesture> FCircle;
        [Output("Circle Original Slice")]
        public ISpread<int> FCircleOS;
        [Output("Key Tap")]
        public ISpread<KeyTapGesture> FKeyTap;
        [Output("Key Tap Original Slice")]
        public ISpread<int> FKeyTapOS;
        [Output("Screen Tap")]
        public ISpread<ScreenTapGesture> FScreenTap;
        [Output("Screen Tap Original Slice")]
        public ISpread<int> FScreenTapOS;
        [Output("Swipe")]
        public ISpread<SwipeGesture> FSwipe;
        [Output("Swipe Original Slice")]
        public ISpread<int> FSwipeOS;

        public void Evaluate(int SpreadMax)
        {

            FCircle.SliceCount = 0;
            FKeyTap.SliceCount = 0;
            FScreenTap.SliceCount = 0;
            FSwipe.SliceCount = 0;

            FCircleOS.SliceCount = 0;
            FKeyTapOS.SliceCount = 0;
            FScreenTapOS.SliceCount = 0;
            FSwipeOS.SliceCount = 0;

            if (FGesture.IsConnected)
            {
                for (int i = 0; i < FGesture.SliceCount; i++)
                {
                    if (FGesture[i].Type == Gesture.GestureType.TYPE_CIRCLE)
                    {
                        FCircle.Add(new CircleGesture(FGesture[i]));
                        FCircleOS.Add(i);
                    }

                    if (FGesture[i].Type == Gesture.GestureType.TYPE_KEY_TAP)
                    {
                        FKeyTap.Add(new KeyTapGesture(FGesture[i]));
                        FKeyTapOS.Add(i);
                    }

                    if (FGesture[i].Type == Gesture.GestureType.TYPE_SCREEN_TAP)
                    {
                        FScreenTap.Add(new ScreenTapGesture(FGesture[i]));
                        FScreenTapOS.Add(i);
                    }

                    if (FGesture[i].Type == Gesture.GestureType.TYPE_SWIPE)
                    {
                        FSwipe.Add(new SwipeGesture(FGesture[i]));
                        FSwipeOS.Add(i);
                    }
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

    [PluginInfo(Name = "Keep", Category = "Leap", Version = "Gesture", Tags = "")]
    public class GestureLeapKeepNode : IPluginEvaluate
    {
        [Input("Gestures")]
        public Pin<Gesture> FGesture;
        [Input("Keep Time")]
        public ISpread<float> FKeep;

        [Output("Gestures Out")]
        public ISpread<Gesture> FOut;
        [Output("Kept Seconds")]
        public ISpread<double> FAge;

        Dictionary<int, Stopwatch> GstrTime = new Dictionary<int, Stopwatch>();
        Dictionary<int, Gesture> GstrDict = new Dictionary<int, Gesture>();
        List<int> ToDelete = new List<int>();

        public void Evaluate(int SpreadMax)
        {
            ToDelete.Clear();
            foreach (KeyValuePair<int, Stopwatch> kvp in GstrTime)
            {
                if (kvp.Value.Elapsed.TotalSeconds > FKeep[0])
                {
                    ToDelete.Add(kvp.Key);
                }
            }
            foreach (int k in ToDelete)
            {
                GstrTime.Remove(k);
                GstrDict.Remove(k);
            }
            for(int i=0; i<FGesture.SliceCount; i++)
            {
                if(GstrDict.ContainsKey(FGesture[i].Id))
                {
                    GstrDict[FGesture[i].Id] = FGesture[i];
                    GstrTime[FGesture[i].Id].Restart();
                }
                else
                {
                    GstrDict.Add(FGesture[i].Id, FGesture[i]);
                    GstrTime.Add(FGesture[i].Id, new Stopwatch());
                    GstrTime[FGesture[i].Id].Reset();
                    GstrTime[FGesture[i].Id].Start();
                }
            }
            FOut.SliceCount = 0;
            FAge.SliceCount = 0;
            foreach (KeyValuePair<int, Gesture> kvp in GstrDict) FOut.Add(kvp.Value);
            foreach (KeyValuePair<int, Stopwatch> kvp in GstrTime) FAge.Add(kvp.Value.Elapsed.TotalSeconds);
        }
    }
}
