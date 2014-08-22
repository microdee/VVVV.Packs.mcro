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
        [Input("Frame")]
        public Pin<Frame> FFrame;
        [Input("Enabled")]
        public ISpread<bool> FEnabled;

        [Output("Left")]
        protected Pin<DX11Resource<DX11DynamicTexture2D>> FLeft;
        [Output("Right")]
        protected Pin<DX11Resource<DX11DynamicTexture2D>> FRight;

        [Output("Is Valid")]
        protected ISpread<bool> FValid;

        private bool FInvalidate;
        private List<ImageList> images = new List<ImageList>();
        private int fcr = 0;

        public void Evaluate(int SpreadMax)
        {
            if (FFrame.IsConnected && this.FEnabled[0])
            {
                if(fcr == 0)
                {
                    this.FLeft.SliceCount = FFrame.SliceCount;
                    this.FRight.SliceCount = FFrame.SliceCount;
                }
                this.FInvalidate = true;
                images.Clear();
                for (int i = 0; i < FFrame.SliceCount; i++)
                {
                    images.Add(FFrame[i].Images);
                }
                
                for (int i = 0; i < FLeft.SliceCount; i++)
                {
                    if (this.FLeft[i] == null) { this.FLeft[i] = new DX11Resource<DX11DynamicTexture2D>(); }
                    if (this.FRight[i] == null) { this.FRight[i] = new DX11Resource<DX11DynamicTexture2D>(); }
                }
                if(FLeft.SliceCount > FFrame.SliceCount)
                {
                    for(int i=FFrame.SliceCount; i<(FLeft.SliceCount-FFrame.SliceCount); i++)
                    {
                        if (this.FLeft[i] != null) { this.FLeft[i].Dispose(); }
                        if (this.FRight[i] != null) { this.FRight[i].Dispose(); }
                    }
                }
                this.FLeft.SliceCount = FFrame.SliceCount;
                this.FRight.SliceCount = FFrame.SliceCount;
                fcr++;
            }
            else
            {
                if ((this.FLeft.SliceCount > 0) || (this.FRight.SliceCount > 0))
                {
                    for (int i = 0; i < FLeft.SliceCount; i++)
                    {
                        if (this.FLeft[i] != null) { this.FLeft[i].Dispose(); }
                        if (this.FRight[i] != null) { this.FRight[i].Dispose(); }
                    }
                    this.FLeft.SliceCount = 0;
                    this.FRight.SliceCount = 0;
                }
                fcr = 0;
            }

        }

        public void Update(IPluginIO pin, DX11RenderContext context)
        {
            if ((this.FLeft.SliceCount == 0) || (this.FRight.SliceCount == 0)) { return; }
            for (int i = 0; i < FLeft.SliceCount; i++)
            {
                if (this.FInvalidate || !this.FLeft[i].Contains(context))
                {

                    SlimDX.DXGI.Format fmt = SlimDX.DXGI.Format.R8_UNorm;

                    Texture2DDescription LeftDesc;

                    if (this.FLeft[i].Contains(context))
                    {
                        LeftDesc = this.FLeft[i][context].Resource.Description;

                        if (LeftDesc.Width != images[i][0].Width || LeftDesc.Height != images[i][0].Height || LeftDesc.Format != fmt)
                        {
                            this.FLeft[i].Dispose(context);
                            this.FLeft[i][context] = new DX11DynamicTexture2D(context, images[i][0].Width, images[i][0].Height, fmt);
                        }
                    }
                    else
                    {
                        this.FLeft[i][context] = new DX11DynamicTexture2D(context, images[i][0].Width, images[i][0].Height, fmt);

#if DEBUG
                        this.FLeft[i][context].Resource.DebugName = "DynamicTexture";
#endif
                    }

                    LeftDesc = this.FLeft[i][context].Resource.Description;

                    Texture2DDescription RightDesc;

                    if (this.FRight[i].Contains(context))
                    {
                        RightDesc = this.FRight[i][context].Resource.Description;

                        if (RightDesc.Width != images[i][1].Width || RightDesc.Height != images[i][1].Height || RightDesc.Format != fmt)
                        {
                            this.FRight[i].Dispose(context);
                            this.FRight[i][context] = new DX11DynamicTexture2D(context, images[i][1].Width, images[i][1].Height, fmt);
                        }
                    }
                    else
                    {
                        this.FRight[i][context] = new DX11DynamicTexture2D(context, images[i][1].Width, images[i][1].Height, fmt);

#if DEBUG
                        this.FRight[i][context].Resource.DebugName = "DynamicTexture";
#endif
                    }

                    RightDesc = this.FRight[i][context].Resource.Description;

                    this.FLeft[i][context].WriteData(images[i][0].Data);
                    this.FRight[i][context].WriteData(images[i][1].Data);
                    this.FInvalidate = false;
                }
            }
        }

        public void Destroy(IPluginIO pin, DX11RenderContext context, bool force)
        {

            for (int i = 0; i < FLeft.SliceCount; i++)
            {
                this.FLeft[i].Dispose(context);
                this.FRight[i].Dispose(context);
            }
        }


        #region IDisposable Members
        public void Dispose()
        {
            if (this.FLeft.SliceCount > 0)
            {
                for (int i = 0; i < FLeft.SliceCount; i++)
                {
                    if (this.FLeft[i] != null)
                    {
                        this.FLeft[i].Dispose();
                    }

                    if (this.FRight[i] != null)
                    {
                        this.FRight[i].Dispose();
                    }
                }
            }
        }
        #endregion
    }
}
