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

// algorithm inspired from http://www.blackpawn.com/texts/lightmaps/default.html

namespace VVVV.Nodes
{
	public class Rectangle
	{
		public double Left = -1;
		public double Top = -1;
		public double Width = 0;
		public double Height = 0;
		public int RectID = -1;
		
		public Rectangle() { }
		public Rectangle(double l, double t, double w, double h, int i)
		{
			this.Left = l;
			this.Top = t;
			this.Width = w;
			this.Height = h;
			this.RectID = i;
		}
		public Rectangle(Vector4D v, int i)
		{
			this.Left = v.x;
			this.Top = v.y;
			this.Width = v.z;
			this.Height = v.w;
			this.RectID = i;
		}
	}
	public class RectPartition
	{
		public Rectangle Metrics;
		public bool Filled = false;
		
		public RectPartition Parent;
		public RectPartition TopLeft;
		public RectPartition TopRight;
		public RectPartition BottomLeft;
		public RectPartition BottomRight;
		public RectPartition[] Partitions = new RectPartition[4];
		
		public RectPartition(double l, double t, double w, double h)
		{
			this.Metrics = new Rectangle(l, t, w, h, -1);
		}
		
		public bool Insert(double w, double h, int i)
		{
			if((this.Metrics.Width <= w || this.Metrics.Height <= h))
			{
				return false;
			}
			else
			{
				this.TopLeft = new RectPartition(0, 0, w, h);
				this.TopLeft.Filled = true;
				this.TopLeft.Metrics.RectID = i;
				this.Partitions[0] = this.TopLeft;
				
				this.TopRight = new RectPartition(w, 0, this.Metrics.Width-w, h);
				this.Partitions[1] = this.TopRight;
				
				this.BottomLeft = new RectPartition(0, h, w, this.Metrics.Height-h);
				this.Partitions[2] = this.BottomLeft;
				
				this.BottomRight = new RectPartition(w, h, this.Metrics.Width-w, this.Metrics.Height-h);
				this.Partitions[3] = this.BottomRight;
				
				for(int j = 0; j<4; j++)
					this.Partitions[j].Parent = this;
				
				return true;
			}
		}
		public Vector2D GetGlobalPosition(Vector2D Pos)
		{
			if((this.Parent == null))
			{
				return new Vector2D(Pos.x, Pos.y);
			}
			else
			{
				return this.Parent.GetGlobalPosition(new Vector2D(
					Pos.x + this.Metrics.Left,
					Pos.y + this.Metrics.Top
				));
			}
		}
	}
	
	public static class Partitioner
	{
		public static bool InsertRecursive(this RectPartition rp, double w, double h, int i)
		{
			if(rp.TopLeft == null)
			{
				if(rp.Insert(w, h, i)) return true;
				else return false;
			}
			else
			{
				for(int j = 0; j<4; j++)
				{
					if(rp.Partitions[j].InsertRecursive(w, h, i)) return true;
				}
				return false;
			}
		}
		
		public static void Read(this RectPartition rp, List<Rectangle> Result)
		{
			if(rp.Filled)
			{
				Vector2D gPos = rp.GetGlobalPosition(new Vector2D(0, 0));
				Result.Add(new Rectangle(
					gPos.x,
					gPos.y,
					rp.Metrics.Width,
					rp.Metrics.Height,
					rp.Metrics.RectID
				));
				return;
			}
			else
			{
				if(rp.TopLeft == null) return;
				else
				{
					for(int j = 0; j<4; j++)
						rp.Partitions[j].Read(Result);
				}
			}
		}
	}
	#region PluginInfo
	[PluginInfo(Name = "PackRectangles", Category = "2d", Help = "Good for packing textures", Tags = "")]
	#endregion PluginInfo
	public class C2dPackRectanglesNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Width Height")]
		public IDiffSpread<Vector2D> FInput;
		[Input("Max Width", DefaultValue=512.0, IsSingle=true)]
		public IDiffSpread<double> FWidth;
		[Input("Max Height", DefaultValue=512.0, IsSingle=true)]
		public IDiffSpread<double> FHeight;

		[Output("Top Left Out")]
		public ISpread<Vector2D> FPos;
		[Output("Width Height Out")]
		public ISpread<Vector2D> FSize;
		[Output("Success")]
		public ISpread<bool> FSuccess;

		[Import()]
		public ILogger FLogger;
		#endregion fields & pins
		
		private RectPartition TopLevel;

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			if(FInput.IsChanged || FWidth.IsChanged || FHeight.IsChanged)
			{
				FPos.SliceCount = SpreadMax;
				FSize.SliceCount = SpreadMax;
				FSuccess.SliceCount = SpreadMax;
				for(int i=0; i<SpreadMax; i++)
				{
					FPos[i] = new Vector2D(0,0);
					FSize[i] = new Vector2D(0,0);
					FSuccess[i] = false;
				}
				this.TopLevel = new RectPartition(0, 0, FWidth[0], FHeight[0]);
				for(int i=0; i<SpreadMax; i++)
				{
					Vector2D v = FInput[i];
					this.TopLevel.InsertRecursive(v.x, v.y, i);
				}
				List<Rectangle> rlist = new List<Rectangle>();
				this.TopLevel.Read(rlist);
				foreach(Rectangle r in rlist)
				{
					FPos[r.RectID] = new Vector2D(r.Left, r.Top);
					FSize[r.RectID] = new Vector2D(r.Width, r.Height);
					FSuccess[r.RectID] = true;
				}
			}

			//FLogger.Log(LogType.Debug, "hi tty!");
		}
	}
}
