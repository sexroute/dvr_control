namespace Camera_NET
{
    using DirectShowLib;
    using System;
    using System.Runtime.InteropServices;

    internal sealed class DirectXInterfaces
    {
        public IBaseFilter CaptureFilter;
        public IAMCrossbar Crossbar;
        public IFilterGraph2 FilterGraph;
        public IMediaControl MediaControl;
        public IVMRMixerBitmap9 MixerBitmap;
        public ISampleGrabber SampleGrabber;
        public IBaseFilter SampleGrabberFilter;
        public IBaseFilter SmartTee;
        public IBaseFilter VMRenderer;
        public IVMRWindowlessControl9 WindowlessCtrl;

        public void CloseInterfaces()
        {
            if (this.VMRenderer != null)
            {
                Marshal.ReleaseComObject(this.VMRenderer);
                this.VMRenderer = null;
                this.WindowlessCtrl = null;
                this.MixerBitmap = null;
            }
            if (this.FilterGraph != null)
            {
                Marshal.ReleaseComObject(this.FilterGraph);
                this.FilterGraph = null;
                this.MediaControl = null;
            }
            if (this.SmartTee != null)
            {
                Marshal.ReleaseComObject(this.SmartTee);
                this.SmartTee = null;
            }
            if (this.SampleGrabber != null)
            {
                Marshal.ReleaseComObject(this.SampleGrabber);
                this.SampleGrabber = null;
                this.SampleGrabberFilter = null;
            }
            if (this.CaptureFilter != null)
            {
                Marshal.ReleaseComObject(this.CaptureFilter);
                this.CaptureFilter = null;
            }
            if (this.Crossbar != null)
            {
                Marshal.ReleaseComObject(this.Crossbar);
                this.Crossbar = null;
            }
        }
    }
}

