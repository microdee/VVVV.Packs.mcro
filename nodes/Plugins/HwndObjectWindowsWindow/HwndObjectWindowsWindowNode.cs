#region usings
using System;
using System.Runtime.InteropServices;
using System.ComponentModel.Composition;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

using WindowScrape;
using WindowScrape.Types;
//using WindowScrape.Constants;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;

#endregion usings

namespace VVVV.Nodes
{
	#region PluginInfo
	[PluginInfo(Name = "WindowList", Category = "Windows", Version = "HwndObject", Help = "Get windows to HwndObject class", Tags = "")]
	#endregion PluginInfo
	public class HwndObjectWindowsWindowListNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Update", DefaultValue = 0, IsSingle=true, IsBang=true)]
		ISpread<bool> FUpdate;

		[Output("Output")]
		ISpread<HwndObject> FOutput;
		[Output("Debug")]
		ISpread<string> FDebug;
		#endregion fields & pins
		public List<HwndObject> wndl;

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			if(FUpdate[0]) {
				wndl = HwndObject.GetWindows();
				FOutput.SliceCount = wndl.Count;
				FDebug.SliceCount = wndl.Count;
				for(int i=0; i<wndl.Count; i++)
				{
					FOutput[i] = wndl[i];
					FDebug[i] = wndl[i].ToString();
				}
			}
		}
	}
	#region PluginInfo
	[PluginInfo(Name = "Info", Category = "Windows", Version = "HwndObject", Help = "Expand HwndObject", Tags = "")]
	#endregion PluginInfo
	public class HwndObjectWindowsInfoNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Input")]
		ISpread<HwndObject> FHwnd;
		[Input("Update", DefaultValue = 0, IsSingle=true, IsBang=true)]
		ISpread<bool> FEnabled;

		[Output("Handle")]
		ISpread<int> FHandle;
		[Output("Title")]
		ISpread<string> FTitle;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			bool valid = true;
			if(FEnabled[0])
			{
				for(int i=0; i<FHwnd.SliceCount; i++)
				{
					var VHwnd = FHwnd[i];
					if(String.Equals(VHwnd.ToString(), String.Empty))
						valid=false;
				}
			}
			
			if(FEnabled[0] && valid) {
				FHandle.SliceCount=FHwnd.SliceCount;
				FTitle.SliceCount=FHwnd.SliceCount;
				for(int i=0; i<FHwnd.SliceCount; i++)
				{
					var THwnd = FHwnd[i];
					FHandle[i] = THwnd.Hwnd.ToInt32();
					FTitle[i] = THwnd.Title;
				}
			}
		}
	}
	#region PluginInfo
	[PluginInfo(Name = "GetWindow", Category = "Windows", Version = "HwndObject", Help = "Get location and size", Tags = "")]
	#endregion PluginInfo
	public class HwndObjectWindowsGetWindowNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Input")]
		ISpread<HwndObject> FHwnd;
		[Input("Update", DefaultValue = 1, IsSingle=true)]
		ISpread<bool> FEnabled;
		
		[Output("Location")]
		ISpread<int> FPos;
		[Output("Size")]
		ISpread<int> FSize;
		
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			bool valid = true;
			if(FEnabled[0] && !(FHwnd.SliceCount==0))
			{
				for(int i=0; i<FHwnd.SliceCount; i++)
				{
					var VHwnd = FHwnd[i];
					if(String.Equals(VHwnd.ToString(), String.Empty))
						valid=false;
				}
			}
			
			if(FEnabled[0] && valid) {
				FPos.SliceCount=FHwnd.SliceCount*2;
				FSize.SliceCount=FHwnd.SliceCount*2;
				for(int i=0; i<FHwnd.SliceCount; i++)
				{
					var THwnd = FHwnd[i];
					FPos[i*2] = THwnd.Location.X;
					FPos[i*2+1] = THwnd.Location.Y;
					FSize[i*2] = THwnd.Size.Width;
					FSize[i*2+1] = THwnd.Size.Height;
				}
			}
		}
	}
	#region PluginInfo
	[PluginInfo(Name = "SetWindowTitle", Category = "Windows", Version = "HwndObject", Help = "Set window title. Thank you captain obvious!", Tags = "", AutoEvaluate=true)]
	#endregion PluginInfo
	public class HwndObjectWindowsSetWindowTitleNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("HwndObject")]
		ISpread<HwndObject> FHwnd;
		[Input("Title")]
		ISpread<string> FTitle;
		[Input("Update", DefaultValue = 0)]
		ISpread<bool> FEnabled;
		
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			if(!(FHwnd.SliceCount==0)) {
				for(int i=0; i<FHwnd.SliceCount; i++)
				{
					if(FEnabled[i])
					{
						var THwnd = FHwnd[i];
						THwnd.Title = FTitle[i];
					}
				}
			}
		}
	}
	#region PluginInfo
	[PluginInfo(Name = "SetWindow", Category = "Windows", Version = "HwndObject", Help = "Set location and size", Tags = "", AutoEvaluate=true)]
	#endregion PluginInfo
	public class HwndObjectWindowsSetWindowNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Input")]
		ISpread<HwndObject> FHwnd;
		[Input("Location XY", DefaultValue = 0)]
		ISpread<int> FPos;
		[Input("Size XY", DefaultValue = 0)]
		ISpread<int> FSize;
		
		[Input("HWND_BOTTOM", DefaultValue = 0, IsBang=true)]
		ISpread<bool> FHWND_BOTTOM;
		[Input("HWND_NOTOPMOST", DefaultValue = 0, IsBang=true)]
		ISpread<bool> FHWND_NOTOPMOST;
		[Input("HWND_TOP", DefaultValue = 0, IsBang=true)]
		ISpread<bool> FHWND_TOP;
		[Input("HWND_TOPMOST", DefaultValue = 0, IsBang=true)]
		ISpread<bool> FHWND_TOPMOST;
		
		[Input("SWP_NOACTIVATE", DefaultValue = 1)]
		ISpread<bool> FSWP_NOACTIVATE;
		[Input("SWP_NOCOPYBITS", DefaultValue = 0)]
		ISpread<bool> FSWP_NOCOPYBITS;
		[Input("SWP_NOOWNERZORDER", DefaultValue = 0)]
		ISpread<bool> FSWP_NOOWNERZORDER;
		[Input("SWP_NOSENDCHANGING", DefaultValue = 0)]
		ISpread<bool> FSWP_NOSENDCHANGING;
		
		[Input("Update", DefaultValue = 0)]
		ISpread<bool> FEnabled;
		
		#endregion fields & pins
		
		[DllImport("C:\\Windows\\System32\\user32.dll")]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
		[DllImport("C:\\Windows\\System32\\user32.dll")]
		public static extern bool SetWindowPos(IntPtr hWnd, int X, int Y, int cx, int cy, uint uFlags);
		
		//[Flags]
		public enum WindowFlags
		{
			SWP_ASYNCWINDOWPOS = 0x4000,
			SWP_DEFERERASE = 0x2000,
			SWP_DRAWFRAME = 0x0020,
			SWP_HIDEWINDOW = 0x0080,
			SWP_NOACTIVATE = 0x0010,
			SWP_NOCOPYBITS = 0x0100,
			SWP_NOMOVE = 0x0002,
			SWP_NOOWNERZORDER = 0x0200,
			SWP_NOREDRAW = 0x0008,
			SWP_NOSENDCHANGING = 0x0400,
			SWP_NOSIZE = 0x0001,
			SWP_NOZORDER = 0x0004,
			SWP_SHOWWINDOW = 0x0040
		}

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			if(FHwnd.IsChanged || FPos.IsChanged || FEnabled.IsChanged || FHWND_BOTTOM.IsChanged || FHWND_NOTOPMOST.IsChanged || FHWND_TOP.IsChanged || FHWND_TOPMOST.IsChanged || FSize.IsChanged || FSWP_NOACTIVATE.IsChanged || FSWP_NOCOPYBITS.IsChanged || FSWP_NOOWNERZORDER.IsChanged || FSWP_NOSENDCHANGING.IsChanged)
			{
				bool valid = true;
				if(!(FHwnd.SliceCount==0))
				{
					for(int i=0; i<FHwnd.SliceCount; i++)
					{
						var VHwnd = FHwnd[i];
						if(String.Equals(VHwnd.ToString(), String.Empty))
							valid=false;
					}
				}
				try
				{
					if(valid)	
					{
						for(int i=0; i<FHwnd.SliceCount; i++)
						{
							if(FEnabled[i])
							{
								var THwnd = FHwnd[i];
								IntPtr handle = THwnd.Hwnd;
								uint aSyncWindowPos = (uint)WindowFlags.SWP_ASYNCWINDOWPOS;
								uint noActivate = (uint)WindowFlags.SWP_NOACTIVATE * (uint)((FSWP_NOACTIVATE[i]) ? 1 : 0);
								uint noCopyBits = (uint)WindowFlags.SWP_NOCOPYBITS * (uint)((FSWP_NOCOPYBITS[i]) ? 1 : 0);
								uint noOwnerZOrder = (uint)WindowFlags.SWP_NOOWNERZORDER * (uint)((FSWP_NOOWNERZORDER[i]) ? 1 : 0);
								uint noSendChanging = (uint)WindowFlags.SWP_NOSENDCHANGING * (uint)((FSWP_NOSENDCHANGING[i]) ? 1 : 0);
								
								uint noMove = (uint)WindowFlags.SWP_NOMOVE * (uint)((FHWND_BOTTOM[i] || FHWND_NOTOPMOST[i] || FHWND_TOP[i] || FHWND_TOPMOST[i]) ? 1 : 0);
								uint noSize = (uint)WindowFlags.SWP_NOSIZE * (uint)((FHWND_BOTTOM[i] || FHWND_NOTOPMOST[i] || FHWND_TOP[i] || FHWND_TOPMOST[i]) ? 1 : 0);
								SetWindowPos(handle, (IntPtr)(-2), FPos[i*2], FPos[i*2+1], FSize[i*2], FSize[i*2+1],
									aSyncWindowPos |
									noActivate |
									noCopyBits |
									noOwnerZOrder |
									noSendChanging
								);
								if(FHWND_BOTTOM[i] || FHWND_NOTOPMOST[i] || FHWND_TOP[i] || FHWND_TOPMOST[i])
								{
									int hwndflag = ((FHWND_BOTTOM[i]) ? 1 : 0) - ((FHWND_NOTOPMOST[i]) ? 2 : 0) - ((FHWND_TOPMOST[i]) ? 1 : 0);
									SetWindowPos(handle, (IntPtr)hwndflag, 0, 0, 0, 0,
										aSyncWindowPos |
										noActivate |
										noCopyBits |
										noOwnerZOrder |
										noSendChanging |
										noMove |
										noSize
									);
								}
								
							}
						}
					}
				}
				catch {}
			}
		}
	}
}
