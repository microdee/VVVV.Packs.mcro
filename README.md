mcropack
========
is a general purpose random collection of useful modules and plugins for vvvv.

Simple Plugins
-------
 - **Sift (String Advanced)**<br />
   BinSize-able Sift (String)
 - **Find (String Bin)**<br />
   BinSize-able Find (String)
 - **OR (Value Bitwise)**<br />
   OR bitwise operator, values interpreted as uint
 - **SplitUint32 (Value Bitwise)**<br />
   Represent individual bits as toggles
 - **DragAndDrop (Control)**<br />
   Listen to drag-and-drop events on forms (exposed to vvvv)
 - **DirectInput (Devices)**<br />
   Raw mouse and keyboard data
 - **FakeMenu (GUI Dynamic)**<br />
   Helper plugin for displaying fake dropdown menus with *FakeMenu (GUI)* module
 - **Clipboard (String Fix)**<br />
   Better and more stable clipboard implementation
 - **DropBox (GUI)**<br />
   A window filled with a box listening to drag-and-drop events
 - **TextBox (GUI)**<br />
   A window filled with a textbox. Used with *TextBox (GUI controller)* or in *TextBox (GUI simple)* modules
 - **PID (Windows Handle)**<br />
   Get the parent process ID of a handle
 - **Handle (Windows PID)**<br />
   Get the child handles of a process ID
 - **Regex (String)**<br />
   An alternative regular expressions implementation with more options and more output information
 - **RemoveZero (String)**<br />
   Remove unnecessary zeros from the end of fractional numbers converted to string with FormatValue or AsString
 - **Router (String)**<br />
   Group spread of strings coupled with an other spread of strings describing the type of the current slice into a single spread which can be unzipped later. Order of the type in a single "zip" is determined by the order fed into the Unique Type pin
 - **SHA-256 (String)**
 - **SeparateFix (String)**<br />
   Separate with regex
 - **SpreadToFrames (String)**<br />
   Individual slices of incoming spread will be fired one-by-one on each frame. Useful for MidiShort out for example.
 - **SequentialAdd (Value)**<br />
   It's like integral but keeping the slicecount and BinSize-able.
 - **TouchProcessor (Value)**<br />
   Touch processing helper plugin working with ID's. Can keep touch data after disappearance for specified amount of frames (this can be used as a bang when touch ends (default behavior))
 - **GetForegroundWindow (Windows)**
 - **GetParent (Windows)**<br />
   Get parent handle of a handle (if the handle has a parent)
 - **GetWindowText (Windows)**
 - **HandleFromPoint (Windows)**
 - **WindowPos (Windows)**<br />
   Advanced version of Window (Windows)

Simple Modules
-------
 - **AsRaw (Transform Bin)**
 - **AutoRun (VVVV MultiThreading)**<br />
   runs another instance of vvvv with user defined arguments if an argument is present or not present
 - **AvoidNIL (Transform)**
 - **Between (Boolean)**<br />
   True if value is between a minimum and a maximum
 - **Border (Transform)**<br />
   Gives you 4 transforms around a quad transform (forming a frame)
 - **Camera (Transform Leap)**<br />
   Camera controlled with Leap Motion
 - **Camera (Transform microdee)**<br />
   Alternative camera controls without axis restrictions and with cursor lock (mouse movement is read by DirectInput (Devices)). Also keyboard and mouse keys can be rebinded in Inspektor
  - **Remote**<br />
    Variant without DirectInput so patches on remote computers without mouse won't go crazy
  - **Sequencable**<br />
    Camera movement can be recorded and played back
 - **ComponentMode (VVVV bounds)**
 - **Connect (Transform [Vector])**<br />
   Connects 3D points. Not aligned
 - **Counter (Animation Simple)**<br />
   Low level counter module if native counter is naughty for some reason
 - **DoubleClick (Animation)**
 - **DoubleTouch (Animation)**
 - **Dragging (Animation)**
 - **FakeFullscreen (VVVV)**<br />
   Makes a vvvv window "fullscreen" by removing borders and make the window fill the entire screen. Sometimes it has much more control than simple fullscreen modes of renderers
 - **FakeMenu (GUI)**<br />
   Dropdown context menu displayed with a window without borders. Real context menus on control forms in vvvv's thread block the mainloop.
 - **GetSpread (Node [Advanced])**
 - **GridSplit (Transform)**
 - **InputMorph (Transform)**
 - **KeepLast (String / Transform / Value)**<br />
   In case of NIL keep the last known values
 - **PackCamera (Transform)**<br />
   Simple zip for camera transforms (View, ViewInv, Proj, ProjInv). Used in Emeshe
 - **SmoothRandom (Value)**<br />
  RandomSpread with discreet seed. The fractional part is the linear interpolation between the current and the next seed.
 - **TextBox (GUI controller)**<br />
   A helper module for the TextBox (GUI) plugin to make it appear under the cursor, make it only the size of the text and make it borderless
  - **Simple**<br />
   Encapsulates the TextBox plugin as well
 - **VideoPlayer (DX9.Texture)**<br />
   Advanced video player based around Player (EX9.Texture) also converting your video to image sequence on-th-fly with ffmpeg
 - **Z-sort (EX9.Mesh)**

DX11 Modules
------------

 - **FakeCubemap (DX11)**<br />
   Cubemap renderer to Texture arrays from an "era" when DX11 didn't have a proper cubemap renderer yet
  - **MRT**<br />
   allowing multiple rendertargets
 - **FromEX9Mesh (DX11.Geometry)**<br />
   Convert EX9 meshes to DX11 geometries
 - **GetFactorMultiplier (Value)**<br />
   Calculates how many vertices will be rendered with regular triangle tessellation
 - **GetMaxElement (DX11.Geometry GeomFX)**<br />
   Ugly way of getting max element count of GeomFX's
 - **Invert (DX11.RenderState)**<br />
   Invert anything below it
 - **LineStrip (DX11 Manual)**<br />
   Advanced linestrip module if you need more data per vertex.
 - **MitterLine (DX11)**<br />
   Line with Mitter corners.
 - **PerfMeter (DX11 Advanced)**<br />
   More information on rendering statistics (displaying DX11 context info and connected Pipeline Statistics)
 - **Picture (DX11)**<br />
   Fast way of displaying images at their actual size and aspect ratio
 - **RoundRect (DX11)**<br />
   Aspect ratio and size maintaining RoundRect
 - **SetSlice (DX11.Texture2DArray)**<br />
   Better "SetSlice" for Texture2DArrays because current one is a little bit limiting
 - **STLFile (DX11.Geometry)**<br />
   Read STL's in vvvv
 - **SkinningSimple (DX11 Assimp)**<br />
   Out of the box skinning with assimp
 - **UsualInpuElements (DX11)**<br />
   float3 position, float3 normals and float2 texcoords
 - **VelocityInpuElements (DX11 MRE)**<br />
   Geometry input layout used in Emeshe a lot (same as in UsualInputElements plus float4 COLOR0 containing previous frame positions and instance ID)
 - **VisualizeColorSpace (DX11)**<br />
   Interpret colors as world coordinates
 - **WaveForm (DX11)**<br />
   The waveform of an image
 - **Writer (DX11.Geometry STL)**<br />
   Save geometry as STL
  - **Rawbuffer**<br />
    Read data from rawbuffer instead of geometry input

Shaders
-------

 - **SuppressDiagonals.fx**<br />
   The same thing we have in girlpower of DX11 making quads displaying without the diagonal line in wireframe
 - **Skinning.gsfx**<br />
   Simple skinning with transforms fed from structured buffers
 - **Thru.gsfx**<br />
   This does nothing the use of this is only to have a geometry represented in a rawbuffer.
 - **BlendControl.tfx**<br />
   Blend with texture controlled fade
 - **BlurFlow.tfx**
 - **BlurMask.tfx**
 - **DisplaceHeight.tfx**<br />
   Alternative displace effect
 - **FilterNoise.tfx**<br />
   Dilate some noise coming from kinect
 - **GenerateMips.tfx**
 - **LavaTransform.tfx**
 - **Lookup.tfx**<br />
   Lookup incoming texture with an UV texture
 - **Plasma.tfx**<br />
   Port of Plasma from DX9
 - **PreviewSlice.tfx**<br />
   View slices of TextureArrays and volumes
 - **RGBA.tfx**<br />
   Join to RGBA channels from separate textures
 - **ScalarOperation.tfx**<br />
   Simple scalar operation
 - **ScalarOperationBundle.tfx**<br />
   Combined scalar operations with optional clamp
 - **StencilView.tfx**<br />
   From DX11 girlpower
 - **TemporalFilter.tfx**<br />
   helper TFX for temporal effects (like unc's SSAO or CSSGI (Color Bleeding))
 - **TransformValues.tfx**
 - **UnPreMultiply.tfx**<br />
   Remove black halo from premultiplied alpha (blended alpha coming from a renderer with black background is premultiplied)
