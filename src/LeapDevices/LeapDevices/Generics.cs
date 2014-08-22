using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.IO.MemoryMappedFiles;

using VVVV.Core;
using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;
using VVVV.Utils.VColor;
using VVVV.Utils.SharedMemory;
using VVVV.Nodes.Generic;

using Leap;

namespace VVVV.Nodes
{
    // Pointable
    [PluginInfo(Name = "Cons", Category = "Leap", Version = "Pointable")]
    public class PointableLeapConsNode : Cons<Pointable> { }

    [PluginInfo(Name = "Zip", Category = "Leap", Version = "Pointable")]
    public class PointableLeapZipNode : Zip<Pointable> { }

    [PluginInfo(Name = "Unzip", Category = "Leap", Version = "Pointable")]
    public class PointableLeapUnzipNode : Unzip<Pointable> { }

    [PluginInfo(Name = "CAR", Category = "Leap", Version = "Pointable")]
    public class PointableLeapCARNode : CARBin<Pointable> { }

    [PluginInfo(Name = "CDR", Category = "Leap", Version = "Pointable")]
    public class PointableLeapCDRNode : CDRBin<Pointable> { }

    [PluginInfo(Name = "Select", Category = "Leap", Version = "Pointable")]
    public class PointableLeapSelectNode : SelectBin<Pointable> { }

    [PluginInfo(Name = "Queue", Category = "Leap", Version = "Pointable")]
    public class PointableLeapQueueNode : Queue<Pointable> { }

    // Finger
    [PluginInfo(Name = "Cons", Category = "Leap", Version = "Finger")]
    public class FingerLeapConsNode : Cons<Finger> { }

    [PluginInfo(Name = "Zip", Category = "Leap", Version = "Finger")]
    public class FingerLeapZipNode : Zip<Finger> { }

    [PluginInfo(Name = "Unzip", Category = "Leap", Version = "Finger")]
    public class FingerLeapUnzipNode : Unzip<Finger> { }

    [PluginInfo(Name = "CAR", Category = "Leap", Version = "Finger")]
    public class FingerLeapCARNode : CARBin<Finger> { }

    [PluginInfo(Name = "CDR", Category = "Leap", Version = "Finger")]
    public class FingerLeapCDRNode : CDRBin<Finger> { }

    [PluginInfo(Name = "Select", Category = "Leap", Version = "Finger")]
    public class FingerLeapSelectNode : SelectBin<Finger> { }

    [PluginInfo(Name = "Queue", Category = "Leap", Version = "Finger")]
    public class FingerLeapQueueNode : Queue<Finger> { }

    // Bone
    [PluginInfo(Name = "Cons", Category = "Leap", Version = "Bone")]
    public class BoneLeapConsNode : Cons<Bone> { }

    [PluginInfo(Name = "Zip", Category = "Leap", Version = "Bone")]
    public class BoneLeapZipNode : Zip<Bone> { }

    [PluginInfo(Name = "Unzip", Category = "Leap", Version = "Bone")]
    public class BoneLeapUnzipNode : Unzip<Bone> { }

    [PluginInfo(Name = "CAR", Category = "Leap", Version = "Bone")]
    public class BoneLeapCARNode : CARBin<Bone> { }

    [PluginInfo(Name = "CDR", Category = "Leap", Version = "Bone")]
    public class BoneLeapCDRNode : CDRBin<Bone> { }

    [PluginInfo(Name = "Select", Category = "Leap", Version = "Bone")]
    public class BoneLeapSelectNode : SelectBin<Bone> { }

    [PluginInfo(Name = "Queue", Category = "Leap", Version = "Bone")]
    public class BoneLeapQueueNode : Queue<Bone> { }

    // Gesture
    [PluginInfo(Name = "Cons", Category = "Leap", Version = "Gesture")]
    public class GestureLeapConsNode : Cons<Gesture> { }

    [PluginInfo(Name = "Zip", Category = "Leap", Version = "Gesture")]
    public class GestureLeapZipNode : Zip<Gesture> { }

    [PluginInfo(Name = "Unzip", Category = "Leap", Version = "Gesture")]
    public class GestureLeapUnzipNode : Unzip<Gesture> { }

    [PluginInfo(Name = "CAR", Category = "Leap", Version = "Gesture")]
    public class GestureLeapCARNode : CARBin<Gesture> { }

    [PluginInfo(Name = "CDR", Category = "Leap", Version = "Gesture")]
    public class GestureLeapCDRNode : CDRBin<Gesture> { }

    [PluginInfo(Name = "Select", Category = "Leap", Version = "Gesture")]
    public class GestureLeapSelectNode : SelectBin<Gesture> { }

    [PluginInfo(Name = "Queue", Category = "Leap", Version = "Gesture")]
    public class GestureLeapQueueNode : Queue<Gesture> { }

    // Frame
    [PluginInfo(Name = "Cons", Category = "Leap", Version = "Frame")]
    public class FrameLeapConsNode : Cons<Frame> { }

    [PluginInfo(Name = "Zip", Category = "Leap", Version = "Frame")]
    public class FrameLeapZipNode : Zip<Frame> { }

    [PluginInfo(Name = "Unzip", Category = "Leap", Version = "Frame")]
    public class FrameLeapUnzipNode : Unzip<Frame> { }

    [PluginInfo(Name = "CAR", Category = "Leap", Version = "Frame")]
    public class FrameLeapCARNode : CARBin<Frame> { }

    [PluginInfo(Name = "CDR", Category = "Leap", Version = "Frame")]
    public class FrameLeapCDRNode : CDRBin<Frame> { }

    [PluginInfo(Name = "Select", Category = "Leap", Version = "Frame")]
    public class FrameLeapSelectNode : SelectBin<Frame> { }

    [PluginInfo(Name = "Queue", Category = "Leap", Version = "Frame")]
    public class FrameLeapQueueNode : Queue<Frame> { }
}