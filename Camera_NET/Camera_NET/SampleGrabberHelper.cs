namespace Camera_NET
{
    using DirectShowLib;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal sealed class SampleGrabberHelper : ISampleGrabberCB, IDisposable
    {
        private bool m_bBufferSamplesOfCurrentFrame;
        private volatile bool m_bWantOneFrame;
        private int m_ImageSize;
        private IntPtr m_ipBuffer = IntPtr.Zero;
        private volatile ManualResetEvent m_PictureReady;
        private ISampleGrabber m_SampleGrabber;
        private int m_videoBitCount;
        private int m_videoHeight;
        private int m_videoWidth;

        public SampleGrabberHelper(ISampleGrabber sampleGrabber, bool buffer_samples_of_current_frame)
        {
            this.m_SampleGrabber = sampleGrabber;
            this.m_bBufferSamplesOfCurrentFrame = buffer_samples_of_current_frame;
            this.m_PictureReady = new ManualResetEvent(false);
        }

        public void ConfigureMode()
        {
            AMMediaType pmt = new AMMediaType {
                majorType = MediaType.Video,
                subType = MediaSubType.RGB24,
                formatType = FormatType.VideoInfo
            };
            DsError.ThrowExceptionForHR(this.m_SampleGrabber.SetMediaType(pmt));
            DsUtils.FreeAMMediaType(pmt);
            pmt = null;
            DsError.ThrowExceptionForHR(this.m_SampleGrabber.SetCallback(this, 1));
            if (this.m_bBufferSamplesOfCurrentFrame)
            {
                DsError.ThrowExceptionForHR(this.m_SampleGrabber.SetBufferSamples(true));
            }
        }

        int ISampleGrabberCB.BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
        {
            if (this.m_bWantOneFrame)
            {
                this.m_bWantOneFrame = false;
                NativeMethodes.CopyMemory(this.m_ipBuffer, pBuffer, BufferLen);
                this.m_PictureReady.Set();
            }
            return 0;
        }

        int ISampleGrabberCB.SampleCB(double SampleTime, IMediaSample pSample)
        {
            Marshal.ReleaseComObject(pSample);
            return 0;
        }

        public void Dispose()
        {
            if (this.m_PictureReady != null)
            {
                this.m_PictureReady.Close();
            }
            this.m_SampleGrabber = null;
        }

        private IntPtr GetCurrentFrame()
        {
            if (!this.m_bBufferSamplesOfCurrentFrame)
            {
                throw new Exception("SampleGrabberHelper was created without buffering-mode (buffer of current frame)");
            }
            IntPtr zero = IntPtr.Zero;
            int pBufferSize = 0;
            DsError.ThrowExceptionForHR(this.m_SampleGrabber.GetCurrentBuffer(ref pBufferSize, zero));
            zero = Marshal.AllocCoTaskMem(pBufferSize);
            DsError.ThrowExceptionForHR(this.m_SampleGrabber.GetCurrentBuffer(ref pBufferSize, zero));
            return zero;
        }

        private IntPtr GetNextFrame()
        {
            this.m_PictureReady.Reset();
            this.m_ipBuffer = Marshal.AllocCoTaskMem(Math.Abs((int) ((this.m_videoBitCount / 8) * this.m_videoWidth)) * this.m_videoHeight);
            try
            {
                this.m_bWantOneFrame = true;
                if (!this.m_PictureReady.WaitOne(0x1388, false))
                {
                    throw new Exception("Timeout while waiting to get a snapshot");
                }
            }
            catch
            {
                Marshal.FreeCoTaskMem(this.m_ipBuffer);
                this.m_ipBuffer = IntPtr.Zero;
                throw;
            }
            return this.m_ipBuffer;
        }

        public void SaveMode()
        {
            AMMediaType pmt = new AMMediaType();
            DsError.ThrowExceptionForHR(this.m_SampleGrabber.GetConnectedMediaType(pmt));
            if ((pmt.formatType != FormatType.VideoInfo) || (pmt.formatPtr == IntPtr.Zero))
            {
                throw new NotSupportedException("Unknown Grabber Media Format");
            }
            VideoInfoHeader header = (VideoInfoHeader) Marshal.PtrToStructure(pmt.formatPtr, typeof(VideoInfoHeader));
            this.m_videoWidth = header.BmiHeader.Width;
            this.m_videoHeight = header.BmiHeader.Height;
            this.m_videoBitCount = header.BmiHeader.BitCount;
            this.m_ImageSize = header.BmiHeader.ImageSize;
            DsUtils.FreeAMMediaType(pmt);
            pmt = null;
        }

        public Bitmap SnapshotCurrentFrame()
        {
            if (this.m_SampleGrabber == null)
            {
                throw new Exception("SampleGrabber was not initialized");
            }
            if (!this.m_bBufferSamplesOfCurrentFrame)
            {
                throw new Exception("SampleGrabberHelper was created without buffering-mode (buffer of current frame)");
            }
            IntPtr currentFrame = this.GetCurrentFrame();
            Bitmap bitmap = null;
            PixelFormat format = PixelFormat.Format24bppRgb;
            switch (this.m_videoBitCount)
            {
                case 0x18:
                    format = PixelFormat.Format24bppRgb;
                    break;

                case 0x20:
                    format = PixelFormat.Format32bppRgb;
                    break;

                case 0x30:
                    format = PixelFormat.Format48bppRgb;
                    break;

                default:
                    throw new Exception("Unsupported BitCount");
            }
            Bitmap bitmap2 = new Bitmap(this.m_videoWidth, this.m_videoHeight, (this.m_videoBitCount / 8) * this.m_videoWidth, format, currentFrame);
            bitmap = bitmap2.Clone(new Rectangle(0, 0, this.m_videoWidth, this.m_videoHeight), PixelFormat.Format24bppRgb);
            bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
            if (currentFrame != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(currentFrame);
                currentFrame = IntPtr.Zero;
            }
            bitmap2.Dispose();
            bitmap2 = null;
            return bitmap;
        }

        public Bitmap SnapshotNextFrame()
        {
            if (this.m_SampleGrabber == null)
            {
                throw new Exception("SampleGrabber was not initialized");
            }
            IntPtr nextFrame = this.GetNextFrame();
            if (nextFrame == IntPtr.Zero)
            {
                throw new Exception("Can not snap next frame");
            }
            Bitmap bitmap = null;
            PixelFormat format = PixelFormat.Format24bppRgb;
            switch (this.m_videoBitCount)
            {
                case 0x18:
                    format = PixelFormat.Format24bppRgb;
                    break;

                case 0x20:
                    format = PixelFormat.Format32bppRgb;
                    break;

                case 0x30:
                    format = PixelFormat.Format48bppRgb;
                    break;

                default:
                    throw new Exception("Unsupported BitCount");
            }
            Bitmap bitmap2 = new Bitmap(this.m_videoWidth, this.m_videoHeight, (this.m_videoBitCount / 8) * this.m_videoWidth, format, nextFrame);
            bitmap = bitmap2.Clone(new Rectangle(0, 0, this.m_videoWidth, this.m_videoHeight), PixelFormat.Format24bppRgb);
            bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
            if (nextFrame != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(nextFrame);
                nextFrame = IntPtr.Zero;
            }
            bitmap2.Dispose();
            bitmap2 = null;
            return bitmap;
        }
    }
}

