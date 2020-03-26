namespace Camera_NET
{
    using DirectShowLib;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.InteropServices.ComTypes;
    using System.Threading;
    using System.Windows.Forms;

    public class CameraControl : UserControl
    {
        private Camera_NET.Camera _Camera;
        private string _DirectShowLogFilepath = string.Empty;
        private const string CameraWasNotCreatedMessage = "Camera is not created.";
        private IContainer components;

        public event EventHandler OutputVideoSizeChanged;

        public CameraControl()
        {
            this.InitializeComponent();
        }

        private void _ThrowIfCameraWasNotCreated()
        {
            if (!this.CameraCreated)
            {
                throw new Exception("Camera is not created.");
            }
        }

        private void Camera_OutputVideoSizeChanged(object sender, EventArgs e)
        {
            if (this.OutputVideoSizeChanged != null)
            {
                this.OutputVideoSizeChanged(sender, e);
            }
        }

        public void CloseCamera()
        {
            if (this._Camera != null)
            {
                this._Camera.StopGraph();
                this._Camera.CloseAll();
                this._Camera.Dispose();
                this._Camera = null;
            }
        }

        public PointF ConvertWinToNorm(PointF p)
        {
            this._ThrowIfCameraWasNotCreated();
            return this._Camera.ConvertWinToNorm(p);
        }

        public void DisplayPropertyPage_CaptureFilter(IntPtr hwndOwner)
        {
            this._ThrowIfCameraWasNotCreated();
            this._Camera.DisplayPropertyPage_CaptureFilter(hwndOwner);
        }

        public void DisplayPropertyPage_Crossbar(IntPtr hwndOwner)
        {
            this._ThrowIfCameraWasNotCreated();
            this._Camera.DisplayPropertyPage_Crossbar(hwndOwner);
        }

        public static void DisplayPropertyPage_Device(IMoniker moniker, IntPtr hwndOwner)
        {
            Camera_NET.Camera.DisplayPropertyPage_Device(moniker, hwndOwner);
        }

        public void DisplayPropertyPage_SourcePinOutput(IntPtr hwndOwner)
        {
            this._ThrowIfCameraWasNotCreated();
            this._Camera.DisplayPropertyPage_SourcePinOutput(hwndOwner);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        public static IMoniker GetDeviceMoniker(int iDeviceNum)
        {
            return Camera_NET.Camera.GetDeviceMoniker(iDeviceNum);
        }

        public static ResolutionList GetResolutionList(IMoniker moniker)
        {
            return Camera_NET.Camera.GetResolutionList(moniker);
        }

        public AnalogVideoStandard GetTVMode()
        {
            this._ThrowIfCameraWasNotCreated();
            return this._Camera.GetTVMode();
        }

        private void InitializeComponent()
        {
            base.SuspendLayout();
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.Name = "CameraControl";
            base.Size = new Size(0x129, 0xf3);
            base.ResumeLayout(false);
        }

        public void SetCamera(IMoniker moniker, Camera_NET.Resolution resolution)
        {
            this.CloseCamera();
            if (moniker != null)
            {
                this._Camera = new Camera_NET.Camera();
                if (!string.IsNullOrEmpty(this._DirectShowLogFilepath))
                {
                    this._Camera.DirectShowLogFilepath = this._DirectShowLogFilepath;
                }
                if (resolution != null)
                {
                    this._Camera.Resolution = resolution;
                }
                this._Camera.Initialize(this, moniker);
                this._Camera.BuildGraph();
                this._Camera.RunGraph();
                this._Camera.OutputVideoSizeChanged += new EventHandler(this.Camera_OutputVideoSizeChanged);
            }
        }

        public void SetTVMode(AnalogVideoStandard mode)
        {
            this._ThrowIfCameraWasNotCreated();
            this._Camera.SetTVMode(mode);
        }

        public Bitmap SnapshotOutputImage()
        {
            this._ThrowIfCameraWasNotCreated();
            return this._Camera.SnapshotOutputImage();
        }

        public Bitmap SnapshotSourceImage()
        {
            this._ThrowIfCameraWasNotCreated();
            return this._Camera.SnapshotSourceImage();
        }

        public void ZoomToRect(Rectangle ZoomRect)
        {
            this._ThrowIfCameraWasNotCreated();
            this._Camera.ZoomToRect(ZoomRect);
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Camera_NET.Camera Camera
        {
            get
            {
                return this._Camera;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public bool CameraCreated
        {
            get
            {
                return (this._Camera != null);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public bool CrossbarAvailable
        {
            get
            {
                this._ThrowIfCameraWasNotCreated();
                return this._Camera.CrossbarAvailable;
            }
        }

        [Description("Log file path for DirectShow (used in BuildGraph)")]
        public string DirectShowLogFilepath
        {
            get
            {
                if (!this.CameraCreated)
                {
                    return this._DirectShowLogFilepath;
                }
                return this._Camera.DirectShowLogFilepath;
            }
            set
            {
                this._DirectShowLogFilepath = value;
                if (this.CameraCreated)
                {
                    this._Camera.DirectShowLogFilepath = this._DirectShowLogFilepath;
                }
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float GDIAlphaValue
        {
            get
            {
                this._ThrowIfCameraWasNotCreated();
                return this._Camera.GDIAlphaValue;
            }
            set
            {
                this._ThrowIfCameraWasNotCreated();
                this._Camera.GDIAlphaValue = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color GDIColorKey
        {
            get
            {
                this._ThrowIfCameraWasNotCreated();
                return this._Camera.GDIColorKey;
            }
            set
            {
                this._ThrowIfCameraWasNotCreated();
                this._Camera.GDIColorKey = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public bool MixerEnabled
        {
            get
            {
                this._ThrowIfCameraWasNotCreated();
                return this._Camera.MixerEnabled;
            }
            set
            {
                this._ThrowIfCameraWasNotCreated();
                this._Camera.MixerEnabled = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IMoniker Moniker
        {
            get
            {
                this._ThrowIfCameraWasNotCreated();
                return this._Camera.Moniker;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public Size OutputVideoSize
        {
            get
            {
                this._ThrowIfCameraWasNotCreated();
                return this._Camera.OutputVideoSize;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public Bitmap OverlayBitmap
        {
            get
            {
                this._ThrowIfCameraWasNotCreated();
                return this._Camera.OverlayBitmap;
            }
            set
            {
                this._ThrowIfCameraWasNotCreated();
                this._Camera.OverlayBitmap = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public Camera_NET.Resolution Resolution
        {
            get
            {
                this._ThrowIfCameraWasNotCreated();
                return this._Camera.Resolution;
            }
            set
            {
                this._ThrowIfCameraWasNotCreated();
                this._Camera.Resolution = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ResolutionList ResolutionListRGB
        {
            get
            {
                this._ThrowIfCameraWasNotCreated();
                return this._Camera.ResolutionListRGB;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Camera_NET.VideoInput VideoInput
        {
            get
            {
                this._ThrowIfCameraWasNotCreated();
                return this._Camera.VideoInput;
            }
            set
            {
                this._ThrowIfCameraWasNotCreated();
                this._Camera.VideoInput = value;
            }
        }
    }
}

