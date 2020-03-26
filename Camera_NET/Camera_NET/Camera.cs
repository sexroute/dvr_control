namespace Camera_NET
{
    using DirectShowLib;
    using Microsoft.Win32;
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Threading;
    using System.Windows.Forms;

    public class Camera : IDisposable
    {
        internal bool _bGraphIsBuilt;
        internal bool _bHandlersAdded;
        private bool _bMixerEnabled;
        internal bool _bMixerImageWasUsed;
        private string _DirectShowLogFilepath = string.Empty;
        private IntPtr _DirectShowLogHandle = IntPtr.Zero;
        private float _GDIAlphaValue = 1f;
        private System.Drawing.Color _GDIColorKey = System.Drawing.Color.Violet;
        private Control _HostingControl;
        private IMoniker _Moniker;
        private Size _OutputVideoSize;
        private Bitmap _OverlayBitmap;
        internal SampleGrabberHelper _pSampleGrabberHelper;
        private Camera_NET.Resolution _Resolution;
        private ResolutionList _ResolutionList = new ResolutionList();
        private Camera_NET.VideoInput _VideoInput = Camera_NET.VideoInput.Default;
        private static readonly int DIB_Image_HeaderSize = Marshal.SizeOf(typeof(BitmapInfoHeader));
        internal DirectXInterfaces DX = new DirectXInterfaces();

        public event EventHandler OutputVideoSizeChanged;

        internal static void _DisplayPropertyPage(object filter_or_pin, IntPtr hwndOwner)
        {
            if (filter_or_pin != null)
            {
                DirectShowLib.ISpecifyPropertyPages o = filter_or_pin as DirectShowLib.ISpecifyPropertyPages;
                if (o == null)
                {
                    IAMVfwCompressDialogs dialogs = filter_or_pin as IAMVfwCompressDialogs;
                    if (dialogs != null)
                    {
                    /*    DsError.ThrowExceptionForHR(dialogs.ShowDialog(VfwCompressDialogs.Config, IntPtr.Zero));*/
                    }
                }
                else
                {
                    DsCAUUID scauuid;
                    string lpszCaption = string.Empty;
                    if (filter_or_pin is IBaseFilter)
                    {
                        FilterInfo info;
                        IBaseFilter filter = filter_or_pin as IBaseFilter;
//                         DsError.ThrowExceptionForHR(filter.QueryFilterInfo(out info));
//                         lpszCaption = info.achName;
//                         if (info.pGraph != null)
//                         {
//                             Marshal.ReleaseComObject(info.pGraph);
//                         }
                    }
                    else if (filter_or_pin is IPin)
                    {
                        PinInfo info2;
                        IPin pin = filter_or_pin as IPin;
                       /* DsError.ThrowExceptionForHR(pin.QueryPinInfo(out info2));*/
                       /* lpszCaption = info2.name;*/
                    }
                    DsError.ThrowExceptionForHR(o.GetPages(out scauuid));
                    object ppUnk = filter_or_pin;
                    DsError.ThrowExceptionForHR(NativeMethodes.OleCreatePropertyFrame(hwndOwner, 0, 0, lpszCaption, 1, ref ppUnk, scauuid.cElems, scauuid.pElems, 0, 0, IntPtr.Zero));
                    Marshal.FreeCoTaskMem(scauuid.pElems);
                    Marshal.ReleaseComObject(o);
                }
            }
        }

        private void AddFilter_Crossbar()
        {
            this.DX.Crossbar = null;
            ICaptureGraphBuilder2 builder = null;
            try
            {
                builder = (ICaptureGraphBuilder2) new CaptureGraphBuilder2();
                DsError.ThrowExceptionForHR(builder.SetFiltergraph(this.DX.FilterGraph));
                object ppint = null;
                builder.FindInterface(FindDirection.UpstreamOnly, Guid.Empty, this.DX.CaptureFilter, typeof(IAMCrossbar).GUID, out ppint);
                if (ppint != null)
                {
                    this.DX.Crossbar = (IAMCrossbar) ppint;
                }
            }
            finally
            {
                SafeReleaseComObject(builder);
                builder = null;
            }
            if (this.DX.Crossbar != null)
            {
                DsError.ThrowExceptionForHR(this.DX.FilterGraph.AddFilter((IBaseFilter) this.DX.Crossbar, "Crossbar"));
                SetCrossbarInput(this.DX.Crossbar, this._VideoInput);
                this._VideoInput = GetCrossbarInput(this.DX.Crossbar);
            }
        }

        private void AddFilter_Renderer()
        {
            this.DX.VMRenderer = (IBaseFilter) new VideoMixingRenderer9();
            this.ConfigureVMRInWindowlessMode();
            DsError.ThrowExceptionForHR(this.DX.FilterGraph.AddFilter(this.DX.VMRenderer, "Video Mixing Renderer 9"));
        }

        private void AddFilter_SampleGrabber()
        {
            this.DX.SampleGrabber = new SampleGrabber() as ISampleGrabber;
            this.DX.SampleGrabberFilter = this.DX.SampleGrabber as IBaseFilter;
            this._pSampleGrabberHelper = new SampleGrabberHelper(this.DX.SampleGrabber, false);
            this._pSampleGrabberHelper.ConfigureMode();
            DsError.ThrowExceptionForHR(this.DX.FilterGraph.AddFilter(this.DX.SampleGrabberFilter, "Sample Grabber"));
        }

        private void AddFilter_Source()
        {
            this.DX.CaptureFilter = null;
            DsError.ThrowExceptionForHR(this.DX.FilterGraph.AddSourceFilterForMoniker(this._Moniker, null, "Source Filter", out this.DX.CaptureFilter));
            this._ResolutionList = GetResolutionsAvailable(this.DX.CaptureFilter);
        }

        private void AddFilter_TeeSplitter()
        {
            this.DX.SmartTee = (IBaseFilter) new SmartTee();
            DsError.ThrowExceptionForHR(this.DX.FilterGraph.AddFilter(this.DX.SmartTee, "SmartTee"));
        }

        private void AddHandlers()
        {
            if (this._HostingControl == null)
            {
                throw new Exception("Can't add handlers. Hosting control is not set.");
            }
            this._HostingControl.Paint += new PaintEventHandler(this.HostingControl_Paint);
            this._HostingControl.Resize += new EventHandler(this.HostingControl_ResizeMove);
            this._HostingControl.Move += new EventHandler(this.HostingControl_ResizeMove);
            SystemEvents.DisplaySettingsChanged += new EventHandler(this.SystemEvents_DisplaySettingsChanged);
            this._bHandlersAdded = true;
        }

        private static void AnalyzeMediaType(AMMediaType media_type, Camera_NET.Resolution resolution_desired, out bool bit_count_ok, out bool sub_type_ok, out bool resolution_ok)
        {
            short bitCountForMediaType = GetBitCountForMediaType(media_type);
            bit_count_ok = IsBitCountAppropriate(bitCountForMediaType);
            sub_type_ok = ((((media_type.subType == MediaSubType.RGB32) || (media_type.subType == MediaSubType.ARGB32)) || ((media_type.subType == MediaSubType.RGB24) || (media_type.subType == MediaSubType.RGB16_D3D_DX9_RT))) || (media_type.subType == MediaSubType.RGB16_D3D_DX7_RT)) || (media_type.subType == MediaSubType.YUY2);
            resolution_ok = IsResolutionAppropiate(media_type, resolution_desired);
        }

        private void ApplyDirectShowLogFile()
        {
            if (this.DX.FilterGraph != null)
            {
                this.CloseDirectShowLogFile();
                if (!string.IsNullOrEmpty(this._DirectShowLogFilepath))
                {
                    this._DirectShowLogHandle = NativeMethodes.CreateFile(this._DirectShowLogFilepath, FileAccess.Write, FileShare.Read, IntPtr.Zero, FileMode.OpenOrCreate, FileAttributes.Normal, IntPtr.Zero);
                    if ((this._DirectShowLogHandle.ToInt32() == -1) || (this._DirectShowLogHandle == IntPtr.Zero))
                    {
                        this._DirectShowLogHandle = IntPtr.Zero;
                        throw new Exception("Can't open log file for writing: " + this._DirectShowLogFilepath);
                    }
                    NativeMethodes.SetFilePointerEx(this._DirectShowLogHandle, 0L, IntPtr.Zero, 2);
                    this.DX.FilterGraph.SetLogFile(this._DirectShowLogHandle);
                }
            }
        }

        public void BuildGraph()
        {
            this._bGraphIsBuilt = false;
            try
            {
                this.DX.FilterGraph = (IFilterGraph2) new FilterGraph();
                this.DX.MediaControl = (IMediaControl) this.DX.FilterGraph;
                this.ApplyDirectShowLogFile();
                this.GraphBuilding_AddFilters();
                this.GraphBuilding_ConnectPins();
                this.PostActions_SampleGrabber();
                this.PostActions_Renderer();
                this.UpdateOutputVideoSize();
                this.SetMixerSettings();
                this._bGraphIsBuilt = true;
            }
            catch
            {
                this.CloseAll();
                throw;
            }
        }

        public void CloseAll()
        {
            this._bGraphIsBuilt = false;
            try
            {
                this.CloseDirectShowLogFile();
            }
            catch (Exception)
            {
            }
            if (this.DX.MediaControl != null)
            {
                try
                {
                    this.DX.MediaControl.StopWhenReady();
                    this.DX.MediaControl.Stop();
                }
                catch (Exception)
                {
                }
            }
            if (this._bHandlersAdded)
            {
                this.RemoveHandlers();
            }
            if (this._pSampleGrabberHelper != null)
            {
                this._pSampleGrabberHelper.Dispose();
                this._pSampleGrabberHelper = null;
            }
            this.DX.CloseInterfaces();
            this._bMixerImageWasUsed = false;
            this._bMixerEnabled = false;
        }

        private void CloseDirectShowLogFile()
        {
            try
            {
                if (this.DX.FilterGraph != null)
                {
                    this.DX.FilterGraph.SetLogFile(IntPtr.Zero);
                }
                NativeMethodes.CloseHandle(this._DirectShowLogHandle);
            }
            catch
            {
                throw;
            }
            finally
            {
                this._DirectShowLogHandle = IntPtr.Zero;
            }
        }

        private void ConfigureVMRInWindowlessMode()
        {
            IVMRFilterConfig9 vMRenderer = (IVMRFilterConfig9) this.DX.VMRenderer;
            DsError.ThrowExceptionForHR(vMRenderer.SetNumberOfStreams(1));
            DsError.ThrowExceptionForHR(vMRenderer.SetRenderingMode(VMR9Mode.Windowless));
            this.DX.WindowlessCtrl = (IVMRWindowlessControl9) this.DX.VMRenderer;
            if (this._HostingControl != null)
            {
                DsError.ThrowExceptionForHR(this.DX.WindowlessCtrl.SetVideoClippingWindow(this._HostingControl.Handle));
            }
            DsError.ThrowExceptionForHR(this.DX.WindowlessCtrl.SetAspectRatioMode(VMR9AspectRatioMode.LetterBox));
            this.AddHandlers();
            this.HostingControl_ResizeMove(null, null);
        }

        public PointF ConvertWinToNorm(PointF point)
        {
            NormalizedRect videoRect = this.GetVideoRect();
            return new PointF((point.X - videoRect.left) / (videoRect.right - videoRect.left), (point.Y - videoRect.top) / (videoRect.bottom - videoRect.top));
        }

        public void DisplayPropertyPage_CaptureFilter(IntPtr hwndOwner)
        {
            DisplayPropertyPageFilter(this.DX.CaptureFilter, hwndOwner);
        }

        public void DisplayPropertyPage_Crossbar(IntPtr hwndOwner)
        {
            if (this.DX.Crossbar != null)
            {
                DisplayPropertyPageFilter((IBaseFilter) this.DX.Crossbar, hwndOwner);
                this._VideoInput = GetCrossbarInput(this.DX.Crossbar);
            }
        }

        public static void DisplayPropertyPage_Device(IMoniker moniker, IntPtr hwndOwner)
        {
            if (moniker != null)
            {
                object ppvResult = null;
                Guid gUID = typeof(IBaseFilter).GUID;
                moniker.BindToObject(null, null, ref gUID, out ppvResult);
                IBaseFilter filter = (IBaseFilter) ppvResult;
                DisplayPropertyPageFilter(filter, hwndOwner);
                SafeReleaseComObject(filter);
                filter = null;
            }
        }

        public void DisplayPropertyPage_SourcePinOutput(IntPtr hwndOwner)
        {
            IPin pin = null;
            try
            {
                pin = DsFindPin.ByDirection(this.DX.CaptureFilter, PinDirection.Output, 0);
                DisplayPropertyPagePin(pin, hwndOwner);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                SafeReleaseComObject(pin);
                pin = null;
            }
        }

        internal static void DisplayPropertyPageFilter(IBaseFilter filter, IntPtr hwndOwner)
        {
            _DisplayPropertyPage(filter, hwndOwner);
        }

        internal static void DisplayPropertyPagePin(IPin pin, IntPtr hwndOwner)
        {
            _DisplayPropertyPage(pin, hwndOwner);
        }

        public void Dispose()
        {
            this.CloseAll();
        }

        ~Camera()
        {
            this.Dispose();
        }

        private static void FreeMediaType(ref AMMediaType media_type)
        {
            if (media_type != null)
            {
                DsUtils.FreeAMMediaType(media_type);
                media_type = null;
            }
        }

        private static void FreeSCCMemory(ref IntPtr pSCC)
        {
            if (pSCC != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(pSCC);
                pSCC = IntPtr.Zero;
            }
        }

        private static short GetBitCountForMediaType(AMMediaType media_type)
        {
            VideoInfoHeader structure = new VideoInfoHeader();
            Marshal.PtrToStructure(media_type.formatPtr, structure);
            return structure.BmiHeader.BitCount;
        }

        private static Camera_NET.VideoInput GetCrossbarInput(IAMCrossbar crossbar)
        {
            int num;
            int num2;
            Camera_NET.VideoInput input = Camera_NET.VideoInput.Default;
            if (crossbar.get_PinCounts(out num2, out num) == 0)
            {
                int num4;
                int num6;
                int outputPinIndex = -1;
                for (int i = 0; i < num2; i++)
                {
                    PhysicalConnectorType type;
                    if ((crossbar.get_CrossbarPinInfo(false, i, out num4, out type) == 0) && (type == PhysicalConnectorType.Video_VideoDecoder))
                    {
                        outputPinIndex = i;
                        break;
                    }
                }
                if ((outputPinIndex != -1) && (crossbar.get_IsRoutedTo(outputPinIndex, out num6) == 0))
                {
                    PhysicalConnectorType type2;
                    crossbar.get_CrossbarPinInfo(true, num6, out num4, out type2);
                    input = new Camera_NET.VideoInput(num6, type2);
                }
            }
            return input;
        }

        public static IMoniker GetDeviceMoniker(int iDeviceIndex)
        {
            DsDevice[] devicesOfCat = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            if (iDeviceIndex >= devicesOfCat.Length)
            {
                throw new Exception("No video capture devices found at that index.");
            }
            return devicesOfCat[iDeviceIndex].Mon;
        }

        private static Camera_NET.Resolution GetResolutionForMediaType(AMMediaType media_type)
        {
            VideoInfoHeader structure = new VideoInfoHeader();
            Marshal.PtrToStructure(media_type.formatPtr, structure);
            return new Camera_NET.Resolution(structure.BmiHeader.Width, structure.BmiHeader.Height);
        }

        public static ResolutionList GetResolutionList(IMoniker moniker)
        {
            ResolutionList resolutionsAvailable = null;
            IFilterGraph2 graph = new FilterGraph() as IFilterGraph2;
            IBaseFilter ppFilter = null;
            try
            {
                DsError.ThrowExceptionForHR(graph.AddSourceFilterForMoniker(moniker, null, "Source Filter", out ppFilter));
                resolutionsAvailable = GetResolutionsAvailable(ppFilter);
            }
            finally
            {
                SafeReleaseComObject(graph);
                graph = null;
                SafeReleaseComObject(ppFilter);
                ppFilter = null;
            }
            return resolutionsAvailable;
        }

        private static ResolutionList GetResolutionsAvailable(IBaseFilter captureFilter)
        {
            ResolutionList resolutionsAvailable = null;
            IPin pinOutput = null;
            try
            {
                pinOutput = DsFindPin.ByDirection(captureFilter, PinDirection.Output, 0);
                resolutionsAvailable = GetResolutionsAvailable(pinOutput);
            }
            catch
            {
                throw;
            }
            finally
            {
                SafeReleaseComObject(pinOutput);
                pinOutput = null;
            }
            return resolutionsAvailable;
        }

        private static ResolutionList GetResolutionsAvailable(IPin pinOutput)
        {
            ResolutionList list = new ResolutionList();
            AMMediaType ppmt = null;
            IntPtr zero = IntPtr.Zero;
            try
            {
                IAMStreamConfig config = pinOutput as IAMStreamConfig;
                DsError.ThrowExceptionForHR(config.SetFormat(null));
                int piCount = 0;
                int piSize = 0;
                DsError.ThrowExceptionForHR(config.GetNumberOfCapabilities(out piCount, out piSize));
                for (int i = 0; i < piCount; i++)
                {
                    zero = Marshal.AllocCoTaskMem(piSize);
                    config.GetStreamCaps(i, out ppmt, zero);
                    if (IsBitCountAppropriate(GetBitCountForMediaType(ppmt)))
                    {
                        list.AddIfNew(GetResolutionForMediaType(ppmt));
                    }
                    FreeSCCMemory(ref zero);
                    FreeMediaType(ref ppmt);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                FreeSCCMemory(ref zero);
                FreeMediaType(ref ppmt);
            }
            return list;
        }

        public AnalogVideoStandard GetTVMode()
        {
            if (this.DX.CaptureFilter == null)
            {
                return AnalogVideoStandard.None;
            }
            IAMAnalogVideoDecoder captureFilter = this.DX.CaptureFilter as IAMAnalogVideoDecoder;
            if (captureFilter == null)
            {
                return AnalogVideoStandard.None;
            }
            AnalogVideoStandard none = AnalogVideoStandard.None;
            DsError.ThrowExceptionForHR(captureFilter.get_TVFormat(out none));
            return none;
        }

        private NormalizedRect GetVideoRect()
        {
            int width = this._HostingControl.ClientRectangle.Width;
            int height = this._HostingControl.ClientRectangle.Height;
            return new NormalizedRect(((float) (width - this._OutputVideoSize.Width)) / 2f, ((float) (height - this._OutputVideoSize.Height)) / 2f, width - (((float) (width - this._OutputVideoSize.Width)) / 2f), height - (((float) (height - this._OutputVideoSize.Height)) / 2f));
        }

        private void GraphBuilding_AddFilters()
        {
            this.AddFilter_Source();
            this.SetSourceParams();
            this.AddFilter_Renderer();
            this.AddFilter_Crossbar();
            this.AddFilter_TeeSplitter();
            this.AddFilter_SampleGrabber();
        }

        private void GraphBuilding_ConnectPins()
        {
            IPin ppinOut = null;
            IPin ppinIn = null;
            IPin pin3 = null;
            IPin pin4 = null;
            IPin pin5 = null;
            IPin pin6 = null;
            try
            {
                ppinOut = DsFindPin.ByDirection(this.DX.CaptureFilter, PinDirection.Output, 0);
                ppinIn = DsFindPin.ByDirection(this.DX.SmartTee, PinDirection.Input, 0);
                pin3 = DsFindPin.ByName(this.DX.SmartTee, "Preview");
                pin4 = DsFindPin.ByName(this.DX.SmartTee, "Capture");
                pin5 = DsFindPin.ByDirection(this.DX.SampleGrabberFilter, PinDirection.Input, 0);
                pin6 = DsFindPin.ByDirection(this.DX.VMRenderer, PinDirection.Input, 0);
                DsError.ThrowExceptionForHR(this.DX.FilterGraph.Connect(ppinOut, ppinIn));
                DsError.ThrowExceptionForHR(this.DX.FilterGraph.Connect(pin3, pin5));
                DsError.ThrowExceptionForHR(this.DX.FilterGraph.Connect(pin4, pin6));
            }
            catch
            {
                throw;
            }
            finally
            {
                SafeReleaseComObject(ppinOut);
                ppinOut = null;
                SafeReleaseComObject(ppinIn);
                ppinIn = null;
                SafeReleaseComObject(pin3);
                pin3 = null;
                SafeReleaseComObject(pin4);
                pin4 = null;
                SafeReleaseComObject(pin5);
                pin5 = null;
                SafeReleaseComObject(pin6);
                pin6 = null;
            }
        }

        private void HostingControl_Paint(object sender, PaintEventArgs e)
        {
            if (this._bGraphIsBuilt && ((this.DX.WindowlessCtrl != null) && (this._HostingControl != null)))
            {
                IntPtr hdc = e.Graphics.GetHdc();
                try
                {
                    this.DX.WindowlessCtrl.RepaintVideo(this._HostingControl.Handle, hdc);
                }
                catch (COMException exception)
                {
                    if (exception.ErrorCode != -2147220980)
                    {
                        throw;
                    }
                }
                finally
                {
                    e.Graphics.ReleaseHdc(hdc);
                }
            }
        }

        private void HostingControl_ResizeMove(object sender, EventArgs e)
        {
            if ((this.DX.WindowlessCtrl != null) && (this._HostingControl != null))
            {
                this.DX.WindowlessCtrl.SetVideoPosition(null, DsRect.FromRectangle(this._HostingControl.ClientRectangle));
                if (this._bGraphIsBuilt)
                {
                    this.UpdateOutputVideoSize();
                    if (this.OutputVideoSizeChanged != null)
                    {
                        this.OutputVideoSizeChanged(sender, e);
                    }
                }
            }
        }

        public void Initialize(Control hControl, IMoniker moniker)
        {
            if (hControl == null)
            {
                throw new Exception("Hosting control should be set.");
            }
            if (moniker == null)
            {
                throw new Exception("Camera's moniker should be set.");
            }
            this._Moniker = moniker;
            this._HostingControl = hControl;
        }

        private static bool IsBitCountAppropriate(short bit_count)
        {
            if (((bit_count != 0x10) && (bit_count != 0x18)) && (bit_count != 0x20))
            {
                return false;
            }
            return true;
        }

        private static bool IsResolutionAppropiate(AMMediaType media_type, Camera_NET.Resolution resolution_desired)
        {
            if (resolution_desired != null)
            {
                VideoInfoHeader structure = new VideoInfoHeader();
                Marshal.PtrToStructure(media_type.formatPtr, structure);
                if ((resolution_desired.Width > 0) && (structure.BmiHeader.Width != resolution_desired.Width))
                {
                    return false;
                }
                if ((resolution_desired.Height > 0) && (structure.BmiHeader.Height != resolution_desired.Height))
                {
                    return false;
                }
            }
            return true;
        }

        private void PostActions_Renderer()
        {
            int num2;
            int num3;
            int num4;
            int num5;
            DsError.ThrowExceptionForHR(this.DX.WindowlessCtrl.GetNativeVideoSize(out num2, out num3, out num4, out num5));
            this._Resolution = new Camera_NET.Resolution(num2, num3);
            this.DX.MixerBitmap = (IVMRMixerBitmap9) this.DX.VMRenderer;
        }

        private void PostActions_SampleGrabber()
        {
            this._pSampleGrabberHelper.SaveMode();
        }

        private void RemoveHandlers()
        {
            if (this._HostingControl == null)
            {
                throw new Exception("Can't remove handlers. Hosting control is not set.");
            }
            this._bHandlersAdded = false;
            this._HostingControl.Paint -= new PaintEventHandler(this.HostingControl_Paint);
            this._HostingControl.Resize -= new EventHandler(this.HostingControl_ResizeMove);
            this._HostingControl.Move -= new EventHandler(this.HostingControl_ResizeMove);
            SystemEvents.DisplaySettingsChanged -= new EventHandler(this.SystemEvents_DisplaySettingsChanged);
        }

        public void RunGraph()
        {
            if (this.DX.MediaControl != null)
            {
                DsError.ThrowExceptionForHR(this.DX.MediaControl.Run());
            }
        }

        private static void SafeReleaseComObject(object obj)
        {
            if (obj != null)
            {
                Marshal.ReleaseComObject(obj);
            }
        }

        private static void SetCrossbarInput(IAMCrossbar crossbar, Camera_NET.VideoInput videoInput)
        {
            int num;
            int num2;
            if (((videoInput.Type != Camera_NET.VideoInput.PhysicalConnectorType_Default) && (videoInput.Index != -1)) && (crossbar.get_PinCounts(out num2, out num) == 0))
            {
                int num5;
                PhysicalConnectorType type;
                int outputPinIndex = -1;
                int inputPinIndex = -1;
                for (int i = 0; i < num2; i++)
                {
                    if ((crossbar.get_CrossbarPinInfo(false, i, out num5, out type) == 0) && (type == PhysicalConnectorType.Video_VideoDecoder))
                    {
                        outputPinIndex = i;
                        break;
                    }
                }
                for (int j = 0; j < num; j++)
                {
                    if (((crossbar.get_CrossbarPinInfo(true, j, out num5, out type) == 0) && (type == videoInput.Type)) && (j == videoInput.Index))
                    {
                        inputPinIndex = j;
                        break;
                    }
                }
                if ((inputPinIndex == -1) || (outputPinIndex == -1))
                {
                    throw new Exception("Can't find routing pins.");
                }
                if (crossbar.CanRoute(outputPinIndex, inputPinIndex) != 0)
                {
                    throw new Exception("Can't route from selected VideoInput to VideoDecoder.");
                }
                DsError.ThrowExceptionForHR(crossbar.Route(outputPinIndex, inputPinIndex));
            }
        }

        private void SetMixerSettings()
        {
            VMR9AlphaBitmap bitmap;
            if (!this._bMixerEnabled)
            {
                if (this._bMixerImageWasUsed)
                {
                    DsError.ThrowExceptionForHR(this.DX.MixerBitmap.GetAlphaBitmapParameters(out bitmap));
                    bitmap.dwFlags = VMR9AlphaBitmapFlags.Disable;
                    DsError.ThrowExceptionForHR(this.DX.MixerBitmap.UpdateAlphaBitmapParameters(ref bitmap));
                }
            }
            else if (this._OverlayBitmap != null)
            {
                Graphics graphics = Graphics.FromImage(this._OverlayBitmap);
                IntPtr hdc = graphics.GetHdc();
                IntPtr ptr2 = NativeMethodes.CreateCompatibleDC(hdc);
                IntPtr hbitmap = this._OverlayBitmap.GetHbitmap();
                NativeMethodes.SelectObject(ptr2, hbitmap);
                bitmap = new VMR9AlphaBitmap {
                    dwFlags = VMR9AlphaBitmapFlags.FilterMode | VMR9AlphaBitmapFlags.SrcColorKey | VMR9AlphaBitmapFlags.hDC,
                    hdc = ptr2,
                    rSrc = new DsRect(0, 0, this._OverlayBitmap.Size.Width, this._OverlayBitmap.Size.Height),
                    rDest = new NormalizedRect(0f, 0f, 1f, 1f),
                    clrSrcKey = ColorTranslator.ToWin32(this._GDIColorKey),
                    dwFilterMode = VMRMixerPrefs.PointFiltering,
                    fAlpha = this._GDIAlphaValue
                };
                DsError.ThrowExceptionForHR(this.DX.MixerBitmap.SetAlphaBitmap(ref bitmap));
                NativeMethodes.DeleteObject(hbitmap);
                NativeMethodes.DeleteDC(ptr2);
                graphics.ReleaseHdc(hdc);
                graphics.Dispose();
                this._bMixerImageWasUsed = true;
            }
        }

        private void SetSourceParams()
        {
            IPin pinSourceCapture = null;
            try
            {
                pinSourceCapture = DsFindPin.ByDirection(this.DX.CaptureFilter, PinDirection.Output, 0);
                SetSourceParams(pinSourceCapture, this._Resolution);
            }
            catch
            {
                throw;
            }
            finally
            {
                SafeReleaseComObject(pinSourceCapture);
                pinSourceCapture = null;
            }
        }

        private static void SetSourceParams(IPin pinSourceCapture, Camera_NET.Resolution resolution_desired)
        {
            AMMediaType pmt = null;
            AMMediaType ppmt = null;
            IntPtr zero = IntPtr.Zero;
            bool flag = false;
            try
            {
                IAMStreamConfig config = pinSourceCapture as IAMStreamConfig;
                DsError.ThrowExceptionForHR(config.SetFormat(null));
                int piCount = 0;
                int piSize = 0;
                DsError.ThrowExceptionForHR(config.GetNumberOfCapabilities(out piCount, out piSize));
                for (int i = 0; i < piCount; i++)
                {
                    zero = Marshal.AllocCoTaskMem(piSize);
                    config.GetStreamCaps(i, out ppmt, zero);
                    FreeSCCMemory(ref zero);
                    bool flag2 = false;
                    bool flag3 = false;
                    bool flag4 = false;
                    AnalyzeMediaType(ppmt, resolution_desired, out flag2, out flag3, out flag4);
                    if (flag2 && flag4)
                    {
                        if (flag3)
                        {
                            DsError.ThrowExceptionForHR(config.SetFormat(ppmt));
                            flag = true;
                            break;
                        }
                        if (pmt == null)
                        {
                            pmt = ppmt;
                            ppmt = null;
                        }
                    }
                    FreeMediaType(ref ppmt);
                }
                if (!flag)
                {
                    if (pmt == null)
                    {
                        throw new Exception("Camera doesn't support media type with requested resolution and bits per pixel.");
                    }
                    DsError.ThrowExceptionForHR(config.SetFormat(pmt));
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                FreeMediaType(ref ppmt);
                FreeMediaType(ref pmt);
                FreeSCCMemory(ref zero);
            }
        }

        public void SetTVMode(AnalogVideoStandard mode)
        {
            if (this.DX.CaptureFilter != null)
            {
                IAMAnalogVideoDecoder captureFilter = this.DX.CaptureFilter as IAMAnalogVideoDecoder;
                if (captureFilter != null)
                {
                    DsError.ThrowExceptionForHR(captureFilter.put_TVFormat(mode));
                }
            }
        }

        public Bitmap SnapshotOutputImage()
        {
            if (this.DX.WindowlessCtrl == null)
            {
                throw new Exception("WindowlessCtrl is not initialized.");
            }
            IntPtr zero = IntPtr.Zero;
            Bitmap bitmap = null;
            Bitmap bitmap2 = null;
            try
            {
                DsError.ThrowExceptionForHR(this.DX.WindowlessCtrl.GetCurrentImage(out zero));
                if (zero != IntPtr.Zero)
                {
                    BitmapInfoHeader structure = new BitmapInfoHeader();
                    Marshal.PtrToStructure(zero, structure);
                    PixelFormat format = PixelFormat.Format24bppRgb;
                    switch (structure.BitCount)
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
                            throw new Exception("Unsupported BitCount.");
                    }
                    bitmap = new Bitmap(structure.Width, structure.Height, (structure.BitCount / 8) * structure.Width, format, new IntPtr(zero.ToInt64() + DIB_Image_HeaderSize));
                    bitmap2 = bitmap.Clone(new Rectangle(0, 0, structure.Width, structure.Height), PixelFormat.Format24bppRgb);
                    bitmap2.RotateFlip(RotateFlipType.Rotate180FlipX);
                }
                return bitmap2;
            }
            catch
            {
                if (bitmap != null)
                {
                    bitmap.Dispose();
                    bitmap = null;
                }
                throw;
            }
            finally
            {
                Marshal.FreeCoTaskMem(zero);
            }
           
        }

        public Bitmap SnapshotSourceImage()
        {
            if (this._pSampleGrabberHelper == null)
            {
                throw new Exception("SampleGrabberHelper is not initialized.");
            }
            return this._pSampleGrabberHelper.SnapshotNextFrame();
        }

        public void StopGraph()
        {
            if (this.DX.MediaControl != null)
            {
                DsError.ThrowExceptionForHR(this.DX.MediaControl.Stop());
            }
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            if (this._bGraphIsBuilt && (this.DX.WindowlessCtrl != null))
            {
                this.DX.WindowlessCtrl.DisplayModeChanged();
            }
        }

        private void UpdateMixerStuff()
        {
            this.SetMixerSettings();
        }

        private void UpdateOutputVideoSize()
        {
            Size size;
            int width = this._HostingControl.ClientRectangle.Width;
            int height = this._HostingControl.ClientRectangle.Height;
            int num = this._Resolution.Width;
            int num2 = this._Resolution.Height;
            int num3 = this._HostingControl.ClientRectangle.Width;
            int num4 = this._HostingControl.ClientRectangle.Height;
            if ((num3 == 0) || (num4 == 0))
            {
                throw new Exception("Incorrect window size (zero).");
            }
            if ((num == 0) || (num2 == 0))
            {
                throw new Exception("Incorrect video size (zero).");
            }
            double num5 = ((double) num) / ((double) num2);
            double num6 = ((double) num3) / ((double) num4);
            if (num5 <= num6)
            {
                int num7 = Convert.ToInt32(Math.Round((double) (num4 * num5)));
                num7 = Math.Min(num3, num7);
                size = new Size(num7, num4);
            }
            else
            {
                int num8 = Convert.ToInt32(Math.Round((double) (((double) num3) / num5)));
                num8 = Math.Min(num4, num8);
                size = new Size(num3, num8);
            }
            this._OutputVideoSize = size;
        }

        public void ZoomToRect(Rectangle zoomRect)
        {
            NormalizedRect rect = new NormalizedRect();
            if ((zoomRect.Height == 0) || (zoomRect.Width == 0))
            {
                throw new Exception("ZoomRect has zero size.");
            }
            IVMRMixerControl9 vMRenderer = (IVMRMixerControl9) this.DX.VMRenderer;
            if (vMRenderer == null)
            {
                throw new Exception("The Mixer control is not created.");
            }
            float num = ((float) this._Resolution.Width) / ((float) zoomRect.Width);
            float num2 = ((float) this._Resolution.Height) / ((float) zoomRect.Height);
            rect = new NormalizedRect(-((float) zoomRect.Left) * num, -((float) zoomRect.Top) * num2, (-((float) zoomRect.Right) * num) + (this._Resolution.Width * (num + 1f)), (-((float) zoomRect.Bottom) * num2) + (this._Resolution.Height * (num2 + 1f))) {
                left = rect.left / ((float) this._Resolution.Width),
                right = rect.right / ((float) this._Resolution.Width),
                top = rect.top / ((float) this._Resolution.Height),
                bottom = rect.bottom / ((float) this._Resolution.Height)
            };
            vMRenderer.SetOutputRect(0, ref rect);
        }

        public bool CrossbarAvailable
        {
            get
            {
                return (this.DX.Crossbar != null);
            }
        }

        public string DirectShowLogFilepath
        {
            get
            {
                return this._DirectShowLogFilepath;
            }
            set
            {
                this._DirectShowLogFilepath = value;
                this.ApplyDirectShowLogFile();
            }
        }

        public float GDIAlphaValue
        {
            get
            {
                return this._GDIAlphaValue;
            }
            set
            {
                this._GDIAlphaValue = value;
            }
        }

        public System.Drawing.Color GDIColorKey
        {
            get
            {
                return this._GDIColorKey;
            }
            set
            {
                this._GDIColorKey = value;
            }
        }

        public Control HostingControl
        {
            get
            {
                return this._HostingControl;
            }
        }

        public bool MixerEnabled
        {
            get
            {
                return this._bMixerEnabled;
            }
            set
            {
                this._bMixerEnabled = value;
                this.UpdateMixerStuff();
            }
        }

        public IMoniker Moniker
        {
            get
            {
                return this._Moniker;
            }
        }

        public Size OutputVideoSize
        {
            get
            {
                return this._OutputVideoSize;
            }
        }

        public Bitmap OverlayBitmap
        {
            get
            {
                return this._OverlayBitmap;
            }
            set
            {
                this._OverlayBitmap = value;
                if (this._bMixerEnabled)
                {
                    this.UpdateMixerStuff();
                }
            }
        }

        public Camera_NET.Resolution Resolution
        {
            get
            {
                return this._Resolution;
            }
            set
            {
                if (this._bGraphIsBuilt)
                {
                    throw new Exception("Change of resolution is not allowed after graph's built.");
                }
                this._Resolution = value;
            }
        }

        public ResolutionList ResolutionListRGB
        {
            get
            {
                return this._ResolutionList;
            }
        }

        public Camera_NET.VideoInput VideoInput
        {
            get
            {
                return this._VideoInput;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("VideoInput", "VideoInput should not be set to null, use Default instead.");
                }
                this._VideoInput = value;
                if (this._bGraphIsBuilt)
                {
                    SetCrossbarInput(this.DX.Crossbar, this._VideoInput);
                    this._VideoInput = GetCrossbarInput(this.DX.Crossbar);
                }
            }
        }
    }
}

