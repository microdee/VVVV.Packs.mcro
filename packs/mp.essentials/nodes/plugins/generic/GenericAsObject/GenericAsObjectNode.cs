#region usings
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using VVVV.Core.Logging;
#endregion usings

namespace VVVV.Nodes
{
	public abstract class AsObjectNode<T> : IPluginEvaluate
	{
		#region fields & pins
		[Input("Input")]
		public ISpread<T> FInput;

		[Output("Output")]
		public ISpread<object> FOutput;

		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			FOutput.SliceCount = FInput.SliceCount;
			for (int i = 0; i < FInput.SliceCount; i++)
			{
				FOutput[i] = (object)FInput[i];
			}
		}
	}

	// Miss a type? Can you see the pattern here? ;)

	[PluginInfo(Name = "AsObject", Category = "Value", Tags = "")]
	public class ValueAsObjectNode : AsObjectNode<double>
	{
	}

	[PluginInfo(Name = "AsObject", Category = "String", Tags = "")]
	public class StringAsObjectNode : AsObjectNode<string>
	{
	}
}
