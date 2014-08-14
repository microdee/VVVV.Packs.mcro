using System;
using System.IO;
using System.ComponentModel.Composition;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;
using VVVV.Utils.Streams;
using VVVV.Utils.Linq;

using VVVV.Core.Logging;
using VVVV.ROD;

namespace VVVV.Nodes
{
    [PluginInfo(Name = "Dictionary", Category = "Raw", Version = "Object")]
	public class ObjectRawServerNode : IPluginEvaluate
	{
		[Input("Clear", IsBang = true)]
		public ISpread<bool> FClear;

		[Output("Output")]
		public ISpread<RodWrap> FOut;

        RodWrap everything = new RodWrap();

		public void Evaluate(int spreadMax)
		{
			FOut.SliceCount = 1;
			FOut[0] = everything;
			if(FClear[0])
			{
				everything.Clear();
			}

            foreach(KeyValuePair<string, RawObject> kvp in everything.Objects)
            {
                if (kvp.Value.Remove) everything.RemoveList.Add(kvp.Key);
            }
            everything.RemoveTagged();
		}
	}

    [PluginInfo(Name = "ToSpread", Category = "Raw", Version = "Object")]
    public class ObjectRawToSpreadNode : IPluginEvaluate
    {
        [Input("Dictionary")]
        public ISpread<RodWrap> FDict;

        [Output("Spread")]
        public ISpread<RawObject> FSpread;

        public void Evaluate(int spreadMax)
        {
            FSpread.SliceCount = 0;
            foreach (KeyValuePair<string, RawObject> kvp in FDict[0].Objects) FSpread.Add(kvp.Value);
        }
    }

    [PluginInfo(Name = "GetObject", Category = "Raw", Version = "Object")]
    public class ObjectRawGetObjectNode : IPluginEvaluate
    {
        [Input("Dictionary")]
        public ISpread<RodWrap> FDict;
        [Input("Name")]
        public ISpread<string> FName;
        [Input("Match", DefaultValue=1.0)]
        public ISpread<bool> FMatch;

        [Output("Output")]
        public ISpread<RawObject> FSpread;

        public void Evaluate(int spreadMax)
        {
            FSpread.SliceCount = 0;
            if(FMatch[0])
            {
                for (int i = 0; i < FName.SliceCount; i++)
                {
                    if (FDict[0].Objects.ContainsKey(FName[i])) FSpread.Add(FDict[0].Objects[FName[i]]);
                }
            }
            else
            {
                for (int i = 0; i < FName.SliceCount; i++)
                {
                    foreach (KeyValuePair<string, RawObject> kvp in FDict[0].Objects)
                    {
                        if (kvp.Key.Contains(FName[i])) FSpread.Add(kvp.Value);
                    }
                }
            }
        }
    }
}
