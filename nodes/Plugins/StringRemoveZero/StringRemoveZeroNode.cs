#region usings
using System;
using System.ComponentModel.Composition;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using VVVV.Core.Logging;
#endregion usings

namespace VVVV.Nodes
{
	#region PluginInfo
	[PluginInfo(Name = "RemoveZero", Category = "String", Help = "Basic template with one string in/out", Tags = "")]
	#endregion PluginInfo
	public class StringRemoveZeroNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Input", DefaultString = "1.0000000")]
		ISpread<string> FInput;
		//[Input("MaxZero")]
		//ISpread<int> FMaxZ;

		[Output("Output")]
		ISpread<string> FOutput;

		[Import()]
		ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			FOutput.SliceCount = SpreadMax;

			for(int i = 0; i < SpreadMax; i++)
			{
				if(FInput[i]!="0") FOutput[i] = FInput[i].TrimEnd('0').TrimEnd('.');
				else FOutput[i] = "0";
			}
			//FLogger.Log(LogType.Debug, "Logging to Renderer (TTY)");
		}
	}
}
