#region usings
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.ComponentModel.Composition;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

using VVVV.Utils.VMath;
using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;

#endregion usings

namespace VVVV.Nodes
{
	public class TouchProperty
	{
		public Point Position;
		public Rect Bounds;
		public Size Size;
		public int Id;
		public TouchProperty() { }
	}
	#region PluginInfo
	[PluginInfo(Name = "Touch", Category = "Devices", Version = "Desktop", AutoEvaluate = true)]
	#endregion PluginInfo
	public class DesktopDevicesTouchNode : IPluginEvaluate
	{
		[Output("Position")]
		public ISpread<Vector2D> FPos;
		[Output("Size")]
		public ISpread<Vector2D> FSize;
		[Output("Bounds")]
		public ISpread<Vector4D> FBounds;
		[Output("ID")]
		public ISpread<int> FID;
		
		public bool init;
		
		public Dictionary<int, TouchProperty> Touches = new Dictionary<int, TouchProperty>();
		
		void Touch_FrameReported(object sender, TouchFrameEventArgs e)
		{
			foreach(TouchPoint tp in e.GetTouchPoints(null))
			{
				TouchProperty otp = new TouchProperty();
				otp.Position = tp.Position;
				otp.Bounds = tp.Bounds;
				otp.Size = tp.Size;
				otp.Id = tp.TouchDevice.Id;
				
				if(Touches.ContainsKey(otp.Id)) Touches[otp.Id] = otp;
				else Touches.Add(otp.Id, otp);
			}
		}

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			if(init)
			{
				Touch.FrameReported += Touch_FrameReported;
				init = false;
			}
			FPos.SliceCount = Touches.Count;
			FSize.SliceCount = Touches.Count;
			FBounds.SliceCount = Touches.Count;
			FID.SliceCount = Touches.Count;
			
			List<TouchProperty> tlist = Touches.Values.ToList();
			for(int i=0; i<Touches.Count; i++)
			{
				FPos[i] = new Vector2D(tlist[i].Position.X, tlist[i].Position.Y);
				FSize[i] = new Vector2D(tlist[i].Size.Width, tlist[i].Size.Height);
				FBounds[i] = new Vector4D(
					tlist[i].Bounds.TopLeft.X,
					tlist[i].Bounds.TopLeft.Y,
					tlist[i].Bounds.BottomRight.X,
					tlist[i].Bounds.BottomRight.Y);
				FID[i] = tlist[i].Id;
			}
			Touches.Clear();
		}
	}
}
