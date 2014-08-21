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

using SlimDX.Direct3D11;
using SlimDX;

using FeralTic.DX11.Resources;
using FeralTic.DX11;
using VVVV.DX11;

using Leap;

namespace VVVV.Nodes
{
    [PluginInfo(Name = "Images", Category = "Leap")]
    public unsafe class LeapImagesNode : IPluginEvaluate, IDX11ResourceProvider, IDisposable
    {
        [Input("Controller")]
        public Pin<Leap.Controller> FController;
        [Input("Enabled")]
        public ISpread<bool> FEnabled;

        [Output("Left")]
        protected Pin<DX11Resource<DX11DynamicTexture2D>> FLeft;
        [Output("Right")]
        protected Pin<DX11Resource<DX11DynamicTexture2D>> FRight;

        [Output("Is Valid")]
        protected ISpread<bool> FValid;

        private bool FInvalidate;
        private Frame frame;
        private ImageList images;

        public void Evaluate(int SpreadMax)
        {
            frame = FController[0].Frame(0);
            images = frame.Images;

            if (FController.IsConnected && this.FEnabled[0])
            {
                this.FInvalidate = true;
            }

            if ((!FController.IsConnected) || FEnabled.SliceCount == 0)
            {
                if (this.FLeft.SliceCount == 1)
                {
                    if (this.FLeft[0] != null) { this.FLeft[0].Dispose(); }
                    this.FLeft.SliceCount = 0;
                }
                if (this.FRight.SliceCount == 1)
                {
                    if (this.FRight[0] != null) { this.FRight[0].Dispose(); }
                    this.FRight.SliceCount = 0;
                }
            }
            else
            {
                this.FLeft.SliceCount = 1;
                this.FRight.SliceCount = 1;
                if (this.FLeft[0] == null) { this.FLeft[0] = new DX11Resource<DX11DynamicTexture2D>(); }
                if (this.FRight[0] == null) { this.FRight[0] = new DX11Resource<DX11DynamicTexture2D>(); }
            }
        }

        public void Update(IPluginIO pin, DX11RenderContext context)
        {
            if ((this.FLeft.SliceCount == 0) || (this.FRight.SliceCount == 0)) { return; }

            if (this.FInvalidate || !this.FLeft[0].Contains(context))
            {

                SlimDX.DXGI.Format fmt = SlimDX.DXGI.Format.R8_UNorm;

                Texture2DDescription LeftDesc;

                if (this.FLeft[0].Contains(context))
                {
                    LeftDesc = this.FLeft[0][context].Resource.Description;

                    if (LeftDesc.Width != images[0].Width || LeftDesc.Height != images[0].Height || LeftDesc.Format != fmt)
                    {
                        this.FLeft[0].Dispose(context);
                        this.FLeft[0][context] = new DX11DynamicTexture2D(context, images[0].Width, images[0].Height, fmt);
                    }
                }
                else
                {
                    this.FLeft[0][context] = new DX11DynamicTexture2D(context, images[0].Width, images[0].Height, fmt);

#if DEBUG
                    this.FLeft[0][context].Resource.DebugName = "DynamicTexture";
#endif
                }

                LeftDesc = this.FLeft[0][context].Resource.Description;

                Texture2DDescription RightDesc;

                if (this.FRight[0].Contains(context))
                {
                    RightDesc = this.FRight[0][context].Resource.Description;

                    if (RightDesc.Width != images[1].Width || RightDesc.Height != images[1].Height || RightDesc.Format != fmt)
                    {
                        this.FRight[0].Dispose(context);
                        this.FRight[0][context] = new DX11DynamicTexture2D(context, images[1].Width, images[1].Height, fmt);
                    }
                }
                else
                {
                    this.FRight[0][context] = new DX11DynamicTexture2D(context, images[1].Width, images[1].Height, fmt);

#if DEBUG
                    this.FRight[0][context].Resource.DebugName = "DynamicTexture";
#endif
                }

                RightDesc = this.FRight[0][context].Resource.Description;

                this.FLeft[0][context].WriteData(images[0].Data);
                this.FRight[0][context].WriteData(images[1].Data);
                this.FInvalidate = false;
            }

        }

        public void Destroy(IPluginIO pin, DX11RenderContext context, bool force)
        {

            this.FLeft[0].Dispose(context);
            this.FRight[0].Dispose(context);
        }


        #region IDisposable Members
        public void Dispose()
        {
            if (this.FLeft.SliceCount > 0)
            {
                if (this.FLeft[0] != null)
                {
                    this.FLeft[0].Dispose();
                }
            }

            if (this.FRight.SliceCount > 0)
            {
                if (this.FRight[0] != null)
                {
                    this.FRight[0].Dispose();
                }
            }

        }
        #endregion
    }
}
