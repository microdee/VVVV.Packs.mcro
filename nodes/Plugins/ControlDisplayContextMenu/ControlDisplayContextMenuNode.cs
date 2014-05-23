#region usings
using System;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using VVVV.Core.Logging;
#endregion usings

namespace VVVV.Nodes
{
	#region PluginInfo
	[PluginInfo(Name = "DisplayContextMenu", Category = "Control", Help = "Template with some gui elements", Tags = "", AutoEvaluate = true)]
	#endregion PluginInfo
	// UserControl,
	public class ControlDisplayContextMenuNode : IPluginEvaluate
	{
		#region fields & pins
		//[Input("PosSize", DefaultValue = 0.0)]
		//ISpread<int> FPos;
		[Input("Input", IsSingle = true)]
		ISpread<System.Windows.Forms.Control> FInControl;
		[Input("Context Menu", IsSingle = true)]
		ISpread<ContextMenu> FContextMenu;
		
		[Output("Valid")]
		ISpread<bool> FValid;

		[Import()]
		ILogger FLogger;
		#endregion fields & pins

		public void Evaluate(int SpreadMax)
		{
			FValid[0] = false;
			if((FContextMenu.SliceCount != 0) &&
			   (FInControl[0] is System.Windows.Forms.Control) &&
			   (FContextMenu[0] is ContextMenu)
			  )
			{
				FInControl[0].ContextMenu = FContextMenu[0];
				FValid[0] = true;
			}
			else
			{
				FInControl[0].ContextMenu = null;
				FValid[0] = false;
			}
		}
	}
}