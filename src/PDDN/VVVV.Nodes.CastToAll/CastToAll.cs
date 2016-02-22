using System;
using System.Collections.Generic;
using VVVV.Nodes.PDDN;
using VVVV.PluginInterfaces.V2;
using System.ComponentModel.Composition;

namespace VVVV.Nodes
{

    public static class TypeUtils
    {
        public static IEnumerable<Type> GetTypes(this Type type)
        {
            // is there any base type?
            if ((type == null) || (type.BaseType == null))
            {
                yield break;
            }
            yield return type;
            // return all implemented or inherited interfaces
            foreach (var i in type.GetInterfaces())
            {
                yield return i;
            }

            // return all inherited types
            var currentBaseType = type.BaseType;
            while (currentBaseType != null)
            {
                yield return currentBaseType;
                currentBaseType = currentBaseType.BaseType;
            }
        }

        public static string GetName(this Type T, bool full)
        {
            if (full) return T.FullName;
            else return T.Name;
        }
    }

    [PluginInfo(Name = "Cast", Category = "Node", AutoEvaluate = true)]
    public class CastToAllInheritedNode : PinDictionaryDynamicNode, IPluginEvaluate, IPartImportsSatisfiedNotification
    {
        [Import]
        public IPluginHost2 FPluginHost;

        [Input("FullName")]
        public IDiffSpread<bool> FFull;
        
        public GenericInput FInput;
        
        private Type OType;
        private IEnumerable<Type> Types;
        private readonly List<string> PreservePins = new List<string>();

        protected void Init()
        {
            if (FInput[0] == null)
            {
                OType = null;
                RemoveAllOutput();
            }
            else
            {
                OType = FInput[0].GetType();
                Types = OType.GetTypes();
                PreservePins.Clear();
                foreach (var T in Types)
                {
                    if (OutputPins.ContainsKey(T.GetName(FFull[0])))
                    {
                        if(OutputPins[T.GetName(FFull[0])].Type == T)
                            PreservePins.Add(T.GetName(FFull[0]));
                    }
                }

                foreach (var p in OutputPins.Keys)
                {
                    if(!PreservePins.Contains(p))
                        OutputTaggedForRemove.Add(p);
                }
                RemoveTaggedOutput();

                foreach (var T in Types)
                {
                    if (!OutputPins.ContainsKey(T.GetName(FFull[0])))
                    {
                        AddOutput(T, new OutputAttribute(T.GetName(FFull[0])));
                    }
                }
            }
        }

        protected void Write()
        {
            int sc = FInput.Pin.SliceCount;
            foreach (var p in OutputPins.Values)
            {
                p.Spread.SliceCount = sc;
            }
            for (int i = 0; i < sc; i++)
            {
                foreach (var p in OutputPins.Values)
                {
                    p.Spread[i] = FInput[i];
                }
            }
        }
        public void OnImportsSatisfied()
        {
            FInput = new GenericInput(FPluginHost, new InputAttribute("Input"));
            Init();
            Write();
        }

        public void Evaluate(int SpreadMax)
        {
            if (FInput.Pin.SliceCount != 0)
            {
                if (FInput[0] != null)
                {
                    if (FInput[0].GetType() != OType)
                    {
                        Init();
                    }
                    Write();
                }
            }
            else
            {
                foreach (var p in OutputPins.Values)
                {
                    p.Spread.SliceCount = 0;
                }
            }
        }
    }
}
