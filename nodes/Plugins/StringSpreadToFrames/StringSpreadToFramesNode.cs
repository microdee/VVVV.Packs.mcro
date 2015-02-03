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
	[PluginInfo(Name = "SpreadToFrames", Category = "String", Help = "Basic template with one value in/out", Tags = "")]
	#endregion PluginInfo
	public class StringSpreadToFramesNode : IPluginEvaluate, IPartImportsSatisfiedNotification
	{
		#region fields & pins
		[Input("Input")]
		public ISpread<string> FInput;
		[Input("Roll", IsSingle=true, IsBang=true)]
		public ISpread<bool> FRoll;

		[Output("Output")]
		public ISpread<string> FOutput;

		[Import()]
		public ILogger FLogger;
		#endregion fields & pins
		
		public void OnImportsSatisfied()
		{
			FOutput.SliceCount = 0;
		}

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			if(FOutput.SliceCount > 0)
				FOutput.RemoveAt(0);
			if(FRoll[0])
			{
				for (int i = 0; i < FInput.SliceCount; i++)
				{
					FOutput.Add(FInput[i]);
				}
			}

			//FLogger.Log(LogType.Debug, "hi tty!");
		}
	}
}
