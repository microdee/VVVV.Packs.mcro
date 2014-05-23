#region usings
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
#endregion usings

namespace VVVV.Nodes
{
	public class RawObject : IDisposable
	{
		public Dictionary<string, Stream> Fields = new Dictionary<string, Stream>();
		public string Name = "";
		public Stopwatch Age = new Stopwatch();
		public string Debug = "";
		
		bool disposed = false;
		public void Dispose()
		{ 
			Dispose(true);
			GC.SuppressFinalize(this);
		
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposed)
				return; 
			
			if (disposing)
			{
				List<Stream> streamlist = this.Fields.Values.ToList();
				for(int i=0; i<streamlist.Count; i++)
				{
					streamlist[i].Dispose();
				}
			}
			
			// Free any unmanaged objects here. 
			//
			disposed = true;
		}

		
		public RawObject()
		{
			this.Age.Start();
		}
	}
	#region PluginInfo
	[PluginInfo(Name = "Server", Category = "Raw")]
	#endregion PluginInfo
	public class RawServerNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Clear", IsBang = true)]
		public ISpread<bool> FClear;

		[Output("Output")]
		public ISpread<List<RawObject>> FOut;

		List<RawObject> everything = new List<RawObject>();
		#endregion fields & pins

		public void Evaluate(int spreadMax)
		{
			FOut.SliceCount = 1;
			FOut[0] = everything;
			if(FClear[0])
			{
				for(int i=0; i<everything.Count; i++)
				{
					everything[i].Dispose();
				}
				everything.Clear();
			}
		}
	}
	#region PluginInfo
	[PluginInfo(Name = "ToSpread", Category = "Raw", Version = "Object")]
	#endregion PluginInfo
	public class ObjectRawToSpreadNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("List")]
		public ISpread<List<RawObject>> FList;

		[Output("Spread")]
		public ISpread<RawObject> FSpread;

		List<RawObject> everything = new List<RawObject>();
		#endregion fields & pins

		public void Evaluate(int spreadMax)
		{
			FSpread.SliceCount = FList[0].Count;
			for(int i=0; i<FList[0].Count; i++)
			{
				FSpread[i] = FList[0][i];
			}
		}
	}
	
	public enum ManageExisting
	{
		Ignore,
		OverWrite
	}
	#region PluginInfo
	[PluginInfo(Name = "CreateObject", Category = "Raw", AutoEvaluate = true)]
	#endregion PluginInfo
	public class RawCreateObjectNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("List")]
		public ISpread<List<RawObject>> FList;
		
		[Input("Streams")]
		public ISpread<ISpread<Stream>> FStreamIn;
		[Input("Key")]
		public ISpread<ISpread<string>> FKey;
		[Input("Name")]
		public ISpread<string> FName;
		[Input("Debug")]
		public ISpread<string> FDebug;
		[Input("Create", IsBang = true)]
		public ISpread<bool> FCreate;
		[Input("Manage Existing", DefaultEnumEntry = "Ignore")]
        public IDiffSpread<ManageExisting> FExistMan;
		#endregion fields & pins

		public void Evaluate(int spreadMax)
		{
			int sprmx =
				Math.Max(FStreamIn.SliceCount,
				Math.Max(FKey.SliceCount,
				Math.Max(FName.SliceCount,
				Math.Max(FDebug.SliceCount,
				Math.Max(FCreate.SliceCount,FExistMan.SliceCount)
			))));
			
			for (int i = 0; i < sprmx; i++)
			{
				if(FCreate[i])
				{
					RawObject temp = new RawObject();
					temp.Name = FName[i];
					temp.Debug = FDebug[i];
					int currSpreadMax = Math.Max(FStreamIn[i].SliceCount, FKey[i].SliceCount);
					for(int j=0; j<currSpreadMax; j++)
					{
						FStreamIn[i][j].Position = 0;
						bool contains = temp.Fields.ContainsKey(FKey[i][j]);
						if(contains)
						{
							if(FExistMan[i] == ManageExisting.OverWrite)
							{
								temp.Fields[FKey[i][j]].Flush();
								temp.Fields[FKey[i][j]].SetLength(FStreamIn[i][j].Length);
								FStreamIn[i][j].CopyTo(temp.Fields[FKey[i][j]]);
							}
						}
						else
						{
							Stream newstream = new MemoryStream();
							newstream.SetLength(FStreamIn[i][j].Length);
							FStreamIn[i][j].CopyTo(newstream);
							temp.Fields.Add(FKey[i][j], newstream);
						}
					}
					FList[0].Add(temp);
				}
			}
		}
	}
	
	public enum ManageNotExisting
	{
		Ignore,
		AutoCreate
	}
	#region PluginInfo
	[PluginInfo(Name = "SetObject", Category = "Raw", AutoEvaluate = true)]
	#endregion PluginInfo
	public class RawSetObjectNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Object")]
		public ISpread<RawObject> FObject;
		
		[Input("Streams")]
		public ISpread<ISpread<Stream>> FStreamIn;
		[Input("Key")]
		public ISpread<ISpread<string>> FKey;
		[Input("Write From")]
		public ISpread<ISpread<int>> FFrom;
		[Input("Set", IsBang = true)]
		public ISpread<bool> FSet;
		[Input("Manage Not-Existing", DefaultEnumEntry = "Ignore")]
        public IDiffSpread<ManageNotExisting> FNotExistMan;
		#endregion fields & pins
		
		Stream FBuffer = new MemoryStream();
		byte[] FBBuffer = new byte[1];

		public void Evaluate(int spreadMax)
		{
			int sprmx =
				Math.Max(FStreamIn.SliceCount,
				Math.Max(FFrom.SliceCount,FKey.SliceCount)
			);
			
			for (int i = 0; i < sprmx; i++)
			{
				if(FSet[i])
				{
					RawObject temp = FObject[i];
					int csprmx =
						Math.Max(FStreamIn[i].SliceCount,
						Math.Max(FFrom[i].SliceCount,FKey[i].SliceCount)
					);
					for(int j=0; j<csprmx; j++)
					{
						FStreamIn[i][j].Position = 0;
						bool contains = temp.Fields.ContainsKey(FKey[i][j]);
						if(contains)
						{
							/*
							temp.Fields[FKey[i][j]].SetLength(
								Math.Max(
									temp.Fields[FKey[i][j]].Length,
									FFrom[i][j] + FStreamIn[i][j].Length
								)
							);
							*/
							FBuffer.SetLength(FStreamIn[i][j].Length);
							FBuffer.Position = 0;
							FStreamIn[i][j].CopyTo(FBuffer);
							FBuffer.Position = 0;
							
							temp.Fields[FKey[i][j]].Position = FFrom[i][j];
							for(int k=0; k<FStreamIn[i][j].Length; k++)
							{
								FBuffer.Read(FBBuffer,0,1);
								temp.Fields[FKey[i][j]].WriteByte(FBBuffer[0]);
							}
						}
						else
						{
							if(FNotExistMan[i] == ManageNotExisting.AutoCreate)
							{
								Stream newstream = new MemoryStream();
								newstream.SetLength(FStreamIn[i][j].Length);
								newstream.Position = 0;
								FStreamIn[i][j].CopyTo(newstream);
								newstream.Position = 0;
								temp.Fields.Add(FKey[i][j], newstream);
							}
						}
					}
				}
			}
		}
	}
	#region PluginInfo
	[PluginInfo(Name = "GetObject", Category = "Raw")]
	#endregion PluginInfo
	public class RawGetObjectNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Object")]
		public ISpread<RawObject> FObject;
		[Input("Key")]
		public ISpread<ISpread<string>> FKey;
		
		[Output("Streams")]
		public ISpread<ISpread<Stream>> FStream;
		#endregion fields & pins
		
		Stream FBuffer = new MemoryStream();

		public void Evaluate(int spreadMax)
		{
			FStream.SliceCount = FObject.SliceCount;
			for (int i = 0; i < FObject.SliceCount; i++)
			{
				RawObject temp = FObject[i];
				FStream[i].SliceCount = 0;
				for(int j=0; j<FKey[i].SliceCount; j++)
				{
					bool contains = temp.Fields.ContainsKey(FKey[i][j]);
					if(contains)
					{
						FStream[i].Add(temp.Fields[FKey[i][j]]);
					}
				}
			}
		}
	}
}
