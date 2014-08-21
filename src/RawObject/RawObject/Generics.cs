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
using VVVV.Nodes.Generic;
using VVVV.ROD;

namespace VVVV.Nodes
{
    [PluginInfo(Name = "Cons", Category = "Raw", Version = "Object")]
    public class ObjectRawConsNode : Cons<RawObject> { }

    [PluginInfo(Name = "Zip", Category = "Raw", Version = "Object")]
    public class ObjectRawZipNode : Zip<RawObject> { }

    [PluginInfo(Name = "Unzip", Category = "Raw", Version = "Object")]
    public class ObjectRawUnzipNode : Unzip<RawObject> { }

    [PluginInfo(Name = "CAR", Category = "Raw", Version = "Object")]
    public class ObjectRawCARNode : CARBin<RawObject> { }

    [PluginInfo(Name = "CDR", Category = "Raw", Version = "Object")]
    public class ObjectRawCDRNode : CDRBin<RawObject> { }

    [PluginInfo(Name = "Select", Category = "Raw", Version = "Object")]
    public class ObjectRawSelectNode : SelectBin<RawObject> { }
}
