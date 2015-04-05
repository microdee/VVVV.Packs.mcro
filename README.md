mcropack
========
is a general purpose random collection of useful modules and plugins for vvvv.

Simple Plugins
-------
 - **Sift (String Advanced)**
   BinSize-able Sift (String)
 - **Find (String Bin)**
   BinSize-able Find (String)
 - **OR (Value Bitwise)**
   OR bitwise operator, values interpreted as uint
 - **SplitUint32 (Value Bitwise)**
   Represent individual bits as toggles
 - **DragAndDrop (Control)**
   Listen to drag-and-drop events on forms (exposed to vvvv)
 - **DirectInput (Devices)**
   Raw mouse and keyboard data
 - **FakeMenu (GUI Dynamic)**
   Helper plugin for displaying fake dropdown menus with *FakeMenu (GUI)* module
 - **Clipboard (String Fix)**
   Better and more stable clipboard implementation
 - **DropBox (GUI)**
   A window filled with a box listening to drag-and-drop events
 - **TextBox (GUI)**
   A window filled with a textbox. Used with *TextBox (GUI controller)* or in *TextBox (GUI simple)* modules
 - **PID (Windows Handle)**
   Get the parent process ID of a handle
 - **Handle (Windows PID)**
   Get the child handles of a process ID
 - **Regex (String)**
   An alternative regular expressions implementation with more options and more output information
 - **RemoveZero (String)**
   Remove unnecessary zeros from the end of fractional numbers converted to string with FormatValue or AsString
 - **Router (String)**
   Group spread of strings coupled with an other spread of strings describing the type of the current slice into a single spread which can be unzipped later. Order of the type in a single "zip" is determined by the order fed into the Unique Type pin
 - **SHA-256 (String)**
 - **SeparateFix (String)**
   Separate with regex
 - **SpreadToFrames (String)**
   Individual slices of incoming spread will be fired one-by-one on each frame. Useful for MidiShort out for example.
 - **SequentialAdd (Value)**
   It's like integral but keeping the slicecount and BinSize-able.
 - **TouchProcessor (Value)**
   Touch processing helper plugin working with ID's. Can keep touch data after disappearance for specified amount of frames (this can be used as a bang when touch ends (default behavior))
 - **GetForegroundWindow (Windows)**
 - **GetParent (Windows)**
   Get parent handle of a handle (if the handle has a parent)
 - **GetWindowText (Windows)**
 - **HandleFromPoint (Windows)**
 - **WindowPos (Windows)**
   Advanced version of Window (Windows)

Simple Modules
-------
 - **AsRaw (Transform Bin)**
 - **AutoRun (VVVV MultiThreading)**
   runs another instance of vvvv with user defined arguments if an argument is present or not present
 - **AvoidNIL (Transform)**
 - **Between (Boolean)**
   True if value is between a minimum and a maximum
 - **Border (Transform)**
   Gives you 4 transforms around a quad transform (forming a frame)
 - **Camera (Transform Leap)**
   Camera controlled with Leap Motion
 - **Camera (Transform microdee)**
   Alternative camera controls without axis restrictions and with cursor lock (mouse movement is read by DirectInput (Devices)). Also keyboard and mouse keys can be rebinded in Inspektor
  - **Remote**
    Variant without DirectInput so patches on remote computers without mouse won't go crazy
  - **Sequencable**
    Camera movement can be recorded and played back
 - **ComponentMode (VVVV bounds)**
 - **Connect (Transform [Vector])**
   Connects 3D points. Not aligned
 - **Counter (Animation Simple)**
   Low level counter module if native counter is naughty for some reason
 - **DoubleClick (Animation)**
 - **DoubleTouch (Animation)**
 - **Dragging (Animation)**
 - **FakeFullscreen (VVVV)**
   Makes a vvvv window "fullscreen" by removing borders and make the window fill the entire screen. Sometimes it has much more control than simple fullscreen modes of renderers
 - **FakeMenu (GUI)**
   Dropdown context menu displayed with a window without borders. Real context menus on control forms in vvvv's thread block the mainloop.
 - **GetSpread (Node [Advanced])**
 - **GridSplit (Transform)**
 - **InputMorph (Transform)**
 - **KeepLast (String / Transform / Value)**
   In case of NIL keep the last known values
 - **PackCamera (Transform)**
   Simple zip for camera transforms (View, ViewInv, Proj, ProjInv). Used in Emeshe
 - **SmoothRandom (Value)**
  RandomSpread with discreet seed. The fractional part is the linear interpolation between the current and the next seed.
 - **TextBox (GUI controller)**
   A helper module for the TextBox (GUI) plugin to make it appear under the cursor, make it only the size of the text and make it borderless
  - **Simple**
   Encapsulates the TextBox plugin as well
 - **VideoPlayer (DX9.Texture)**
   Advanced video player based around Player (EX9.Texture) also converting your video to image sequence on-th-fly with ffmpeg
 - **Z-sort (EX9.Mesh)**

DX11 Modules
------------

 - **FakeCubemap (DX11)**
   Cubemap renderer to Texture arrays from an "era" when DX11 didn't have a proper cubemap renderer yet
  - **MRT**
   allowing multiple rendertargets
 - **FromEX9Mesh (DX11.Geometry)**
   Convert EX9 meshes to DX11 geometries
 - **GetFactorMultiplier (Value)**
   Calculates how many vertices will be rendered with regular triangle tessellation
 - **GetMaxElement (DX11.Geometry GeomFX)**
   Ugly way of getting max element count of GeomFX's
 - **Invert (DX11.RenderState)**
   Invert anything below it
 - **LineStrip (DX11 Manual)**
   Advanced linestrip module if you need more data per vertex.
 - **MitterLine (DX11)**
   Line with Mitter corners.
 - **PerfMeter (DX11 Advanced)**
   More information on rendering statistics (displaying DX11 context info and connected Pipeline Statistics)
 - **Picture (DX11)**
   Fast way of displaying images at their actual size and aspect ratio
 - **RoundRect (DX11)**
   Aspect ratio and size maintaining RoundRect
 - **STLFile (DX11.Geometry)**
   Read STL's in vvvv
 - **SkinningSimple (DX11 Assimp)**
   Out of the box skinning with assimp
 - **UsualInpuElements (DX11)**
   float3 position, float3 normals and float2 texcoords
 - **VelocityInpuElements (DX11 MRE)**
   Geometry input layout used in Emeshe a lot (same as in UsualInputElements plus float4 COLOR0 containing previous frame positions and instance ID)
 - **VisualizeColorSpace (DX11)**
   Interpret colors as world coordinates
 - **WaveForm (DX11)**
   The waveform of an image
 - **Writer (DX11.Geometry STL)**
   Save geometry as STL
  - **Rawbuffer**
    Read data from rawbuffer instead of geometry input

Shaders
-------

 - **SuppressDiagonals.fx**
   The same thing we have in girlpower of DX11 making quads displaying without the diagonal line in wireframe
 - **Skinning.gsfx**
   Simple skinning with transforms fed from structured buffers
 - **Thru.gsfx**
   This does nothing the use of this is only to have a geometry represented in a rawbuffer.
 - **BlendControl.tfx**
   Blend with texture controlled fade
 - **BlurFlow.tfx**
 - **BlurMask.tfx**
 - **DisplaceHeight.tfx**
   Alternative displace effect
 - **FilterNoise.tfx**
   Dilate some noise coming from kinect
 - **GenerateMips.tfx**
 - **LavaTransform.tfx**
 - **Lookup.tfx**
   Lookup incoming texture with an UV texture
 - **Plasma.tfx**
   Port of Plasma from DX9
 - **PreviewSlice.tfx**
   View slices of TextureArrays and volumes
 - **RGBA.tfx**
   Join to RGBA channels from separate textures
 - **ScalarOperation.tfx**
   Simple scalar operation
 - **ScalarOperationBundle.tfx**
   Combined scalar operations with optional clamp
 - **StencilView.tfx**
   From DX11 girlpower
 - **TemporalFilter.tfx**
   helper TFX for temporal effects (like unc's SSAO or CSSGI (Color Bleeding))
 - **TransformValues.tfx**
 - **UnPreMultiply.tfx**
   Remove black halo from premultiplied alpha (blended alpha coming from a renderer with black background is premultiplied)
