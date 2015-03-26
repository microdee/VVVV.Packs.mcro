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
	#region PluginInfo
	[PluginInfo(Name = "MitterAdjacency", Category = "Value", Help = "Basic template with one value in/out", Tags = "")]
	#endregion PluginInfo
	public class ValueMitterAdjacencyNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Input", DefaultValue = 1.0)]
		public ISpread<ISpread<Vector4D>> FInput;

		[Output("Output")]
		public ISpread<Vector4D> FOutput;
		[Output("VertexCount")]
		public ISpread<int> FCount;

		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			FOutput.SliceCount = 0;
			int vcount = 0;
			for (int i = 0; i < FInput.SliceCount; i++)
			{
				int ccount = FInput[i].SliceCount-3;
				
				for(int j=0; j<ccount; j++)
				{
					for(int jj=0; jj<4; jj++)
					{
						FOutput.Add(new Vector4D(FInput[i][j+jj]));
						vcount++;
					}
				}
			}
			FCount[0] = vcount;

			//FLogger.Log(LogType.Debug, "hi tty!");
		}
	}
	
	#region PluginInfo
	[PluginInfo(Name = "Injector", Category = "Value", Help = "Basic template with one value in/out", Tags = "")]
	#endregion PluginInfo
	public class ValueInjectorNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Input", DefaultValue = 1.0)]
		public ISpread<ISpread<Vector4D>> FInput;

		[Output("Output")]
		public ISpread<ISpread<Vector4D>> FOutput;

		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			FOutput.SliceCount = FInput.SliceCount;
			for (int i = 0; i < FInput.SliceCount; i++)
			{
				FOutput[i].SliceCount = FInput[i].SliceCount;
				
				for(int j=0; j<FInput[i].SliceCount; j++)
				{
					FOutput[i][j] = new Vector4D(FInput[i][j]);
				}
				
				double lx = FInput[i][0].x * 2 - FInput[i][1].x;
				double ly = FInput[i][0].y * 2 - FInput[i][1].y;
				double lz = FInput[i][0].z * 2 - FInput[i][1].z;
				double lw = FInput[i][0].w;
				FOutput[i].Insert(0, new Vector4D(lx,ly,lz,lw));
				
				double tx = FInput[i][FInput[i].SliceCount-1].x * 2 - FInput[i][FInput[i].SliceCount-2].x;
				double ty = FInput[i][FInput[i].SliceCount-1].y * 2 - FInput[i][FInput[i].SliceCount-2].y;
				double tz = FInput[i][FInput[i].SliceCount-1].z * 2 - FInput[i][FInput[i].SliceCount-2].z;
				double tw = FInput[i][FInput[i].SliceCount-1].w;
				FOutput[i].Add(new Vector4D(tx,ty,tz,tw));
			}
		}
	}
	
	#region PluginInfo
	[PluginInfo(Name = "VectorFilter", Category = "Value", Help = "Basic template with one value in/out", Tags = "")]
	#endregion PluginInfo
	public class ValueVectorFilterNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Input", DefaultValue = 1.0)]
		public ISpread<ISpread<Vector4D>> FInput;

		[Output("Output")]
		public ISpread<ISpread<Vector4D>> FOutput;

		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			FOutput.SliceCount = FInput.SliceCount;
			for (int i = 0; i < FInput.SliceCount; i++)
			{
				FOutput[i].SliceCount = FInput[i].SliceCount;
				
				List<int> removables = new List<int>();
				for(int j=0; j<FInput[i].SliceCount; j++)
				{
					FOutput[i][j] = new Vector4D(FInput[i][j]);
					if(j>0)
					{
						double dx = FInput[i][j].x - FInput[i][j-1].x;
						double dy = FInput[i][j].y - FInput[i][j-1].y;
						double dz = FInput[i][j].z - FInput[i][j-1].z;
						double dist = Math.Sqrt(dx*dx + dy*dy + dz*dz);
						if(dist < 0.00001) removables.Add(j);
					}
				}
				foreach(int ii in removables)
					FOutput[i].RemoveAt(ii);
			}
		}
	}
}
