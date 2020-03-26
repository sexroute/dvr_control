namespace CameraControl
{
    using Camera_NET;
    using DirectShowLib;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Drawing.Text;
    using System.IO;
    using System.Runtime.InteropServices.ComTypes;
    using System.Windows.Forms;

    public class Form1 : Form
    {
        private bool _bDrawMouseSelection;
        private bool _bZoomed;
        private CameraChoice _CameraChoice = new CameraChoice();
        private double _fZoomValue = 1.0;
        private NormalizedRect _MouseSelectionRect = new NormalizedRect(0f, 0f, 0f, 0f);
        private Button buttonCameraSettings;
        private Button buttonClearSnapshotFrame;
        private Button buttonCrossbarSettings;
        private Button buttonMixerOnOff;
        private Button buttonPinOutputSettings;
        private Button buttonSaveSnapshotFrame;
        private Button buttonSnapshotNextSourceFrame;
        private Button buttonSnapshotOutputFrame;
        private Button buttonTVMode;
        private Button buttonUnZoom;
        private CameraControl cameraControl;
        private ComboBox comboBoxCameraList;
        private ComboBox comboBoxResolutionList;
        private IContainer components;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Label label1;
        private Label labelCameraTitle;
        private Label labelResolutionTitle;
        private MenuStrip menuStrip1;
        private PictureBox pictureBoxScreenshot;
        private SplitContainer splitContainer1;
        private StatusStrip statusStrip1;
        private TextBox textBoxZiMu;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripMenuItem tVToolStripMenuItem;
        private ToolStripMenuItem 帮助HToolStripMenuItem;
        private ToolStripMenuItem 保存截图PToolStripMenuItem;
        private ToolStripMenuItem 菜单MToolStripMenuItem;
        private ToolStripMenuItem 模式ToolStripMenuItem;
        private ToolStripMenuItem 输出截图ToolStripMenuItem;
        private ToolStripMenuItem 输出设置ToolStripMenuItem;
        private ToolStripMenuItem 输入截图IToolStripMenuItem;
        private ToolStripMenuItem 属性AToolStripMenuItem;

        public Form1()
        {
            this.InitializeComponent();
        }

        private void buttonCameraSettings_Click(object sender, EventArgs e)
        {
            if (this.cameraControl.CameraCreated)
            {
                Camera.DisplayPropertyPage_Device(this.cameraControl.Moniker, base.Handle);
            }
        }

        private void buttonClearSnapshotFrame_Click(object sender, EventArgs e)
        {
            this.pictureBoxScreenshot.Image = null;
            this.pictureBoxScreenshot.Update();
        }

        private void buttonCrossbarSettings_Click(object sender, EventArgs e)
        {
            if (this.cameraControl.CameraCreated)
            {
                this.cameraControl.DisplayPropertyPage_Crossbar(base.Handle);
            }
        }

        private void buttonMixerOnOff_Click(object sender, EventArgs e)
        {
            if (this.cameraControl.CameraCreated)
            {
                if (this.cameraControl.MixerEnabled)
                {
                    this.buttonMixerOnOff.Text = "显示字幕";
                    this.UpdateCameraBitmap();
                }
                else
                {
                    this.buttonMixerOnOff.Text = "隐藏字幕";
                    this.UpdateCameraBitmap();
                }
                this.cameraControl.MixerEnabled = !this.cameraControl.MixerEnabled;
            }
        }

        private void buttonPinOutputSettings_Click(object sender, EventArgs e)
        {
            if (this.cameraControl.CameraCreated)
            {
                this.cameraControl.DisplayPropertyPage_SourcePinOutput(base.Handle);
            }
        }

        private void buttonSaveSnapshotFrame_Click(object sender, EventArgs e)
        {
            if (this.pictureBoxScreenshot.Image != null)
            {
                SaveFileDialog dialog = new SaveFileDialog {
                    Title = "图片保存",
                    Filter = "JPEG(*.JPG;*JPEG;*JPE;*JFIF)|*.jpg|位图BMP(*bmp)|*.bmp|PNG(*.png)|*.png|GIF(*.gif)|*.gif|Tiff files (*.tif;*.tiff)|*.tif;*.tiff|Windows Metafile Format(*.wmf)|*.wmf",
                    FileName = "摄像头控制精灵.jpg"
                };
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string path = dialog.FileName.ToString();
                    switch (path)
                    {
                        case "":
                        case null:
                            return;
                    }
                    string str2 = Path.GetExtension(path).ToLower();
                    if (str2 != "")
                    {
                        try
                        {
                            if (((str2 == ".jpg") || (str2 == ".jpeg")) || ((str2 == ".jpe") || (str2 == ".jfif")))
                            {
                                this.pictureBoxScreenshot.Image.Save(path, ImageFormat.Jpeg);
                            }
                            else if (str2 == ".bmp")
                            {
                                this.pictureBoxScreenshot.Image.Save(path, ImageFormat.Bmp);
                            }
                            else if (str2 == ".png")
                            {
                                this.pictureBoxScreenshot.Image.Save(path, ImageFormat.Png);
                            }
                            else if (str2 == ".gif")
                            {
                                this.pictureBoxScreenshot.Image.Save(path, ImageFormat.Gif);
                            }
                            else if ((str2 == ".tif") || (str2 == ".tiff"))
                            {
                                this.pictureBoxScreenshot.Image.Save(path, ImageFormat.Tiff);
                            }
                            else if (str2 == ".wmf")
                            {
                                this.pictureBoxScreenshot.Image.Save(path, ImageFormat.Wmf);
                            }
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("保存图片失败！请检查是否有同名文件存在！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        }
                    }
                }
            }
        }

        private void buttonSnapshotNextSourceFrame_Click(object sender, EventArgs e)
        {
            if (this.cameraControl.CameraCreated)
            {
                Bitmap bitmap = null;
                try
                {
                    bitmap = this.cameraControl.SnapshotSourceImage();
                }
                catch (Exception exception1)
                {
                    MessageBox.Show(exception1.Message, "Error while getting a snapshot");
                }
                if (bitmap != null)
                {
                    this.pictureBoxScreenshot.Image = bitmap;
                    this.pictureBoxScreenshot.Update();
                }
            }
        }

        private void buttonSnapshotOutputFrame_Click(object sender, EventArgs e)
        {
            if (this.cameraControl.CameraCreated)
            {
                Bitmap bitmap = this.cameraControl.SnapshotOutputImage();
                if (bitmap != null)
                {
                    this.pictureBoxScreenshot.Image = bitmap;
                    this.pictureBoxScreenshot.Update();
                }
            }
        }

        private void buttonTVMode_Click(object sender, EventArgs e)
        {
            if (this.cameraControl.CameraCreated)
            {
                MessageBox.Show(this.cameraControl.GetTVMode().ToString());
            }
        }

        private void buttonUnZoom_Click(object sender, EventArgs e)
        {
            this.UnzoomCamera();
        }

        private void Camera_OutputVideoSizeChanged(object sender, EventArgs e)
        {
            this.UpdateCameraBitmap();
            this.UpdateUnzoomButton();
        }

        private void cameraControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.UnzoomCamera();
        }

        private void cameraControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (((e.Button == MouseButtons.Left) && this.cameraControl.CameraCreated) && !this._bZoomed)
            {
                PointF tf = this.cameraControl.ConvertWinToNorm(new PointF((float) e.X, (float) e.Y));
                this._MouseSelectionRect = new NormalizedRect(tf.X, tf.Y, tf.X, tf.Y);
                this._bDrawMouseSelection = true;
                this.UpdateCameraBitmap();
            }
        }

        private void cameraControl_MouseMove(object sender, MouseEventArgs e)
        {
            if ((((e.Button == MouseButtons.Left) && this.cameraControl.CameraCreated) && !this._bZoomed) && this._bDrawMouseSelection)
            {
                PointF tf = this.cameraControl.ConvertWinToNorm(new PointF((float) e.X, (float) e.Y));
                this._MouseSelectionRect.right = tf.X;
                this._MouseSelectionRect.bottom = tf.Y;
                this.UpdateCameraBitmap();
            }
        }

        private void cameraControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (!this._bZoomed && this._bDrawMouseSelection)
            {
                if (!this.IsMouseSelectionRectCorrectAndGood())
                {
                    this._bDrawMouseSelection = false;
                    this.UpdateCameraBitmap();
                }
                else
                {
                    int width = this.cameraControl.Resolution.Width;
                    int height = this.cameraControl.Resolution.Height;
                    double num3 = width * (this._MouseSelectionRect.right - this._MouseSelectionRect.left);
                    double num4 = height * (this._MouseSelectionRect.bottom - this._MouseSelectionRect.top);
                    double num5 = ((double) width) / ((double) height);
                    double num6 = num3 / num4;
                    if (num5 >= num6)
                    {
                        double num7 = num4 * num5;
                        this._MouseSelectionRect.left -= (float) (((num7 - num3) / 2.0) / ((double) width));
                        this._MouseSelectionRect.right += (float) (((num7 - num3) / 2.0) / ((double) width));
                        this._fZoomValue = ((double) height) / num4;
                    }
                    else
                    {
                        double num8 = num3 / num5;
                        this._MouseSelectionRect.top -= (float) (((num8 - num4) / 2.0) / ((double) height));
                        this._MouseSelectionRect.bottom += (float) (((num8 - num4) / 2.0) / ((double) height));
                        this._fZoomValue = ((double) width) / num3;
                    }
                    Rectangle zoomRect = new Rectangle((int) (this._MouseSelectionRect.left * width), (int) (this._MouseSelectionRect.top * height), (int) ((this._MouseSelectionRect.right - this._MouseSelectionRect.left) * width), (int) ((this._MouseSelectionRect.bottom - this._MouseSelectionRect.top) * height));
                    this.cameraControl.ZoomToRect(zoomRect);
                    this._bZoomed = true;
                    this._bDrawMouseSelection = false;
                    this.UpdateCameraBitmap();
                    this.UpdateUnzoomButton();
                }
            }
        }

        private void comboBoxCameraList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBoxCameraList.SelectedIndex < 0)
            {
                this.cameraControl.CloseCamera();
            }
            else
            {
                this.SetCamera(this._CameraChoice.Devices[this.comboBoxCameraList.SelectedIndex].Mon, null);
            }
            this.FillResolutionList();
        }

        private void comboBoxResolutionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cameraControl.CameraCreated)
            {
                int selectedIndex = this.comboBoxResolutionList.SelectedIndex;
                if (selectedIndex >= 0)
                {
                    ResolutionList resolutionList = Camera.GetResolutionList(this.cameraControl.Moniker);
                    if (((resolutionList != null) && (selectedIndex < resolutionList.Count)) && (resolutionList[selectedIndex].CompareTo(this.cameraControl.Resolution) != 0))
                    {
                        this.SetCamera(this.cameraControl.Moniker, resolutionList[selectedIndex]);
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void FillCameraList()
        {
            this.comboBoxCameraList.Items.Clear();
            this._CameraChoice.UpdateDeviceList();
            foreach (DsDevice device in this._CameraChoice.Devices)
            {
                this.comboBoxCameraList.Items.Add(device.Name);
            }
        }

        private void FillResolutionList()
        {
            this.comboBoxResolutionList.Items.Clear();
            if (this.cameraControl.CameraCreated)
            {
                ResolutionList resolutionList = Camera.GetResolutionList(this.cameraControl.Moniker);
                if (resolutionList != null)
                {
                    int num = -1;
                    for (int i = 0; i < resolutionList.Count; i++)
                    {
                        this.comboBoxResolutionList.Items.Add(resolutionList[i].ToString());
                        if (resolutionList[i].CompareTo(this.cameraControl.Resolution) == 0)
                        {
                            num = i;
                        }
                    }
                    if (num >= 0)
                    {
                        this.comboBoxResolutionList.SelectedIndex = num;
                    }
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.cameraControl.CloseCamera();
            this.StartUpdate();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.FillCameraList();
            if (this.comboBoxCameraList.Items.Count > 0)
            {
                this.comboBoxCameraList.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("未检测到摄像头，请通过USB端口接入摄像头！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            this.FillResolutionList();
            this.toolStripStatusLabel1.Text = "欢迎使用海鸥摄像头控制精灵！";
        }

        private Bitmap GenerateColorKeyBitmap(bool useAntiAlias)
        {
            int width = this.cameraControl.OutputVideoSize.Width;
            int height = this.cameraControl.OutputVideoSize.Height;
            if ((width <= 0) || (height <= 0))
            {
                return null;
            }
            Bitmap image = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            Graphics graphics = Graphics.FromImage(image);
            if (useAntiAlias)
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            }
            else
            {
                graphics.SmoothingMode = SmoothingMode.None;
                graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
            }
            graphics.Clear(this.cameraControl.GDIColorKey);
            if (this._bDrawMouseSelection && this.IsMouseSelectionRectCorrect())
            {
                Color gray = Color.Gray;
                if (this.IsMouseSelectionRectCorrectAndGood())
                {
                    gray = Color.Green;
                }
                Pen pen = new Pen(gray, 2f);
                Rectangle rectangle = new Rectangle((int) (this._MouseSelectionRect.left * width), (int) (this._MouseSelectionRect.top * height), (int) ((this._MouseSelectionRect.right - this._MouseSelectionRect.left) * width), (int) ((this._MouseSelectionRect.bottom - this._MouseSelectionRect.top) * height));
                graphics.DrawLine(pen, rectangle.Left - 5, rectangle.Top, rectangle.Right + 5, rectangle.Top);
                graphics.DrawLine(pen, rectangle.Left - 5, rectangle.Bottom, rectangle.Right + 5, rectangle.Bottom);
                graphics.DrawLine(pen, rectangle.Left, rectangle.Top - 5, rectangle.Left, rectangle.Bottom + 5);
                graphics.DrawLine(pen, rectangle.Right, rectangle.Top - 5, rectangle.Right, rectangle.Bottom + 5);
                pen.Dispose();
            }
            if (this._bZoomed)
            {
                Font font = new Font("Tahoma", 16f);
                Brush brush = new SolidBrush(Color.DarkBlue);
                graphics.DrawString("Zoom: " + Math.Round(this._fZoomValue, 1).ToString("0.0") + "x", font, brush, (float) 4f, (float) 4f);
                font.Dispose();
                brush.Dispose();
            }
            Font font2 = new Font("Tahoma", 16f);
            Brush brush2 = new SolidBrush(Color.Yellow);
            string text = this.textBoxZiMu.Text;
            graphics.DrawString(text, font2, brush2, 4f, (float) (height - 30));
            graphics.Dispose();
            return image;
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new MenuStrip();
            this.菜单MToolStripMenuItem = new ToolStripMenuItem();
            this.属性AToolStripMenuItem = new ToolStripMenuItem();
            this.输出设置ToolStripMenuItem = new ToolStripMenuItem();
            this.tVToolStripMenuItem = new ToolStripMenuItem();
            this.模式ToolStripMenuItem = new ToolStripMenuItem();
            this.输出截图ToolStripMenuItem = new ToolStripMenuItem();
            this.输入截图IToolStripMenuItem = new ToolStripMenuItem();
            this.保存截图PToolStripMenuItem = new ToolStripMenuItem();
            this.帮助HToolStripMenuItem = new ToolStripMenuItem();
            this.statusStrip1 = new StatusStrip();
            this.toolStripStatusLabel1 = new ToolStripStatusLabel();
            this.splitContainer1 = new SplitContainer();
            this.textBoxZiMu = new TextBox();
            this.label1 = new Label();
            this.groupBox2 = new GroupBox();
            this.buttonCameraSettings = new Button();
            this.buttonTVMode = new Button();
            this.buttonPinOutputSettings = new Button();
            this.buttonMixerOnOff = new Button();
            this.buttonCrossbarSettings = new Button();
            this.groupBox1 = new GroupBox();
            this.buttonSaveSnapshotFrame = new Button();
            this.buttonSnapshotOutputFrame = new Button();
            this.buttonClearSnapshotFrame = new Button();
            this.buttonSnapshotNextSourceFrame = new Button();
            this.pictureBoxScreenshot = new PictureBox();
            this.comboBoxResolutionList = new ComboBox();
            this.labelResolutionTitle = new Label();
            this.comboBoxCameraList = new ComboBox();
            this.labelCameraTitle = new Label();
            this.buttonUnZoom = new Button();
            this.cameraControl = new CameraControl();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.splitContainer1.BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((ISupportInitialize) this.pictureBoxScreenshot).BeginInit();
            base.SuspendLayout();
            this.menuStrip1.BackColor = SystemColors.GradientActiveCaption;
            ToolStripItem[] toolStripItems = new ToolStripItem[] { this.菜单MToolStripMenuItem, this.帮助HToolStripMenuItem };
            this.menuStrip1.Items.AddRange(toolStripItems);
            this.menuStrip1.Location = new Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new Size(0x3a7, 0x19);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            ToolStripItem[] itemArray2 = new ToolStripItem[] { this.属性AToolStripMenuItem, this.输出设置ToolStripMenuItem, this.tVToolStripMenuItem, this.模式ToolStripMenuItem, this.输出截图ToolStripMenuItem, this.输入截图IToolStripMenuItem, this.保存截图PToolStripMenuItem };
            this.菜单MToolStripMenuItem.DropDownItems.AddRange(itemArray2);
            this.菜单MToolStripMenuItem.Name = "菜单MToolStripMenuItem";
            this.菜单MToolStripMenuItem.Size = new Size(0x40, 0x15);
            this.菜单MToolStripMenuItem.Text = "菜单(&M)";
            this.属性AToolStripMenuItem.Name = "属性AToolStripMenuItem";
            this.属性AToolStripMenuItem.Size = new Size(0x8e, 0x16);
            this.属性AToolStripMenuItem.Text = "设置属性(&A)";
            this.属性AToolStripMenuItem.Click += new EventHandler(this.buttonCameraSettings_Click);
            this.输出设置ToolStripMenuItem.Name = "输出设置ToolStripMenuItem";
            this.输出设置ToolStripMenuItem.Size = new Size(0x8e, 0x16);
            this.输出设置ToolStripMenuItem.Text = "输出设置(&S)";
            this.输出设置ToolStripMenuItem.Click += new EventHandler(this.buttonPinOutputSettings_Click);
            this.tVToolStripMenuItem.Name = "tVToolStripMenuItem";
            this.tVToolStripMenuItem.Size = new Size(0x8e, 0x16);
            this.tVToolStripMenuItem.Text = "TV模式(&T)";
            this.tVToolStripMenuItem.Click += new EventHandler(this.buttonTVMode_Click);
            this.模式ToolStripMenuItem.Name = "模式ToolStripMenuItem";
            this.模式ToolStripMenuItem.Size = new Size(0x8e, 0x16);
            this.模式ToolStripMenuItem.Text = "显示字幕(&D)";
            this.模式ToolStripMenuItem.Click += new EventHandler(this.buttonMixerOnOff_Click);
            this.输出截图ToolStripMenuItem.Name = "输出截图ToolStripMenuItem";
            this.输出截图ToolStripMenuItem.Size = new Size(0x8e, 0x16);
            this.输出截图ToolStripMenuItem.Text = "输出截图(&O)";
            this.输出截图ToolStripMenuItem.Click += new EventHandler(this.buttonSnapshotOutputFrame_Click);
            this.输入截图IToolStripMenuItem.Name = "输入截图IToolStripMenuItem";
            this.输入截图IToolStripMenuItem.Size = new Size(0x8e, 0x16);
            this.输入截图IToolStripMenuItem.Text = "输入截图(&I)";
            this.输入截图IToolStripMenuItem.Click += new EventHandler(this.buttonSnapshotNextSourceFrame_Click);
            this.保存截图PToolStripMenuItem.Name = "保存截图PToolStripMenuItem";
            this.保存截图PToolStripMenuItem.Size = new Size(0x8e, 0x16);
            this.保存截图PToolStripMenuItem.Text = "保存截图(&P)";
            this.保存截图PToolStripMenuItem.Click += new EventHandler(this.buttonSaveSnapshotFrame_Click);
            this.帮助HToolStripMenuItem.Name = "帮助HToolStripMenuItem";
            this.帮助HToolStripMenuItem.Size = new Size(0x3d, 0x15);
            this.帮助HToolStripMenuItem.Text = "帮助(&H)";
            this.帮助HToolStripMenuItem.Click += new EventHandler(this.帮助HToolStripMenuItem_Click);
            ToolStripItem[] itemArray3 = new ToolStripItem[] { this.toolStripStatusLabel1 };
            this.statusStrip1.Items.AddRange(itemArray3);
            this.statusStrip1.Location = new Point(0, 0x22c);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new Size(0x3a7, 0x16);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new Size(0x83, 0x11);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            this.splitContainer1.Dock = DockStyle.Fill;
            this.splitContainer1.FixedPanel = FixedPanel.Panel1;
            this.splitContainer1.Location = new Point(0, 0x19);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Panel1.BackColor = SystemColors.GradientInactiveCaption;
            this.splitContainer1.Panel1.Controls.Add(this.textBoxZiMu);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1.Controls.Add(this.comboBoxResolutionList);
            this.splitContainer1.Panel1.Controls.Add(this.labelResolutionTitle);
            this.splitContainer1.Panel1.Controls.Add(this.comboBoxCameraList);
            this.splitContainer1.Panel1.Controls.Add(this.labelCameraTitle);
            this.splitContainer1.Panel2.BackColor = SystemColors.Info;
            this.splitContainer1.Panel2.Controls.Add(this.buttonUnZoom);
            this.splitContainer1.Panel2.Controls.Add(this.cameraControl);
            this.splitContainer1.Size = new Size(0x3a7, 0x213);
            this.splitContainer1.SplitterDistance = 230;
            this.splitContainer1.TabIndex = 2;
            this.textBoxZiMu.BackColor = SystemColors.Window;
            this.textBoxZiMu.Location = new Point(0x36, 0xec);
            this.textBoxZiMu.Name = "textBoxZiMu";
            this.textBoxZiMu.Size = new Size(0x92, 0x15);
            this.textBoxZiMu.TabIndex = 0x18;
            this.textBoxZiMu.Text = "欢迎使用海鸥摄像头控制精灵";
            this.label1.AutoSize = true;
            this.label1.Location = new Point(0x10, 240);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x29, 12);
            this.label1.TabIndex = 0x17;
            this.label1.Text = "字幕：";
            this.groupBox2.Controls.Add(this.buttonCameraSettings);
            this.groupBox2.Controls.Add(this.buttonTVMode);
            this.groupBox2.Controls.Add(this.buttonPinOutputSettings);
            this.groupBox2.Controls.Add(this.buttonMixerOnOff);
            this.groupBox2.Controls.Add(this.buttonCrossbarSettings);
            this.groupBox2.Location = new Point(14, 0x6c);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new Size(0xc6, 120);
            this.groupBox2.TabIndex = 0x16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "设置";
            this.buttonCameraSettings.Location = new Point(11, 20);
            this.buttonCameraSettings.Name = "buttonCameraSettings";
            this.buttonCameraSettings.Size = new Size(0x65, 30);
            this.buttonCameraSettings.TabIndex = 11;
            this.buttonCameraSettings.Text = "属性";
            this.buttonCameraSettings.UseVisualStyleBackColor = true;
            this.buttonCameraSettings.Click += new EventHandler(this.buttonCameraSettings_Click);
            this.buttonTVMode.Location = new Point(11, 0x34);
            this.buttonTVMode.Name = "buttonTVMode";
            this.buttonTVMode.Size = new Size(0x67, 30);
            this.buttonTVMode.TabIndex = 13;
            this.buttonTVMode.Text = "获取当前TV模式";
            this.buttonTVMode.UseVisualStyleBackColor = true;
            this.buttonTVMode.Click += new EventHandler(this.buttonTVMode_Click);
            this.buttonPinOutputSettings.Location = new Point(0x79, 20);
            this.buttonPinOutputSettings.Name = "buttonPinOutputSettings";
            this.buttonPinOutputSettings.Size = new Size(70, 30);
            this.buttonPinOutputSettings.TabIndex = 12;
            this.buttonPinOutputSettings.Text = "输出设置";
            this.buttonPinOutputSettings.UseVisualStyleBackColor = true;
            this.buttonPinOutputSettings.Click += new EventHandler(this.buttonPinOutputSettings_Click);
            this.buttonMixerOnOff.Location = new Point(0x7a, 0x34);
            this.buttonMixerOnOff.Name = "buttonMixerOnOff";
            this.buttonMixerOnOff.Size = new Size(0x45, 30);
            this.buttonMixerOnOff.TabIndex = 15;
            this.buttonMixerOnOff.Text = "隐藏字幕";
            this.buttonMixerOnOff.UseVisualStyleBackColor = true;
            this.buttonMixerOnOff.Click += new EventHandler(this.buttonMixerOnOff_Click);
            this.buttonCrossbarSettings.Location = new Point(11, 0x54);
            this.buttonCrossbarSettings.Name = "buttonCrossbarSettings";
            this.buttonCrossbarSettings.Size = new Size(180, 0x1a);
            this.buttonCrossbarSettings.TabIndex = 14;
            this.buttonCrossbarSettings.Text = "Crossbar settings";
            this.buttonCrossbarSettings.UseVisualStyleBackColor = true;
            this.buttonCrossbarSettings.Visible = false;
            this.buttonCrossbarSettings.Click += new EventHandler(this.buttonCrossbarSettings_Click);
            this.groupBox1.Controls.Add(this.buttonSaveSnapshotFrame);
            this.groupBox1.Controls.Add(this.buttonSnapshotOutputFrame);
            this.groupBox1.Controls.Add(this.buttonClearSnapshotFrame);
            this.groupBox1.Controls.Add(this.buttonSnapshotNextSourceFrame);
            this.groupBox1.Controls.Add(this.pictureBoxScreenshot);
            this.groupBox1.Location = new Point(14, 0x108);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(0xc6, 0x100);
            this.groupBox1.TabIndex = 0x15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "截图";
            this.buttonSaveSnapshotFrame.Location = new Point(0x72, 0xd7);
            this.buttonSaveSnapshotFrame.Name = "buttonSaveSnapshotFrame";
            this.buttonSaveSnapshotFrame.Size = new Size(0x48, 30);
            this.buttonSaveSnapshotFrame.TabIndex = 0x15;
            this.buttonSaveSnapshotFrame.Text = "保存截图";
            this.buttonSaveSnapshotFrame.UseVisualStyleBackColor = true;
            this.buttonSaveSnapshotFrame.Click += new EventHandler(this.buttonSaveSnapshotFrame_Click);
            this.buttonSnapshotOutputFrame.Location = new Point(11, 0x10);
            this.buttonSnapshotOutputFrame.Name = "buttonSnapshotOutputFrame";
            this.buttonSnapshotOutputFrame.Size = new Size(180, 30);
            this.buttonSnapshotOutputFrame.TabIndex = 0x11;
            this.buttonSnapshotOutputFrame.Text = "从输出截图";
            this.buttonSnapshotOutputFrame.UseVisualStyleBackColor = true;
            this.buttonSnapshotOutputFrame.Click += new EventHandler(this.buttonSnapshotOutputFrame_Click);
            this.buttonClearSnapshotFrame.Location = new Point(11, 0xd7);
            this.buttonClearSnapshotFrame.Name = "buttonClearSnapshotFrame";
            this.buttonClearSnapshotFrame.Size = new Size(0x44, 30);
            this.buttonClearSnapshotFrame.TabIndex = 20;
            this.buttonClearSnapshotFrame.Text = "清空截图";
            this.buttonClearSnapshotFrame.UseVisualStyleBackColor = true;
            this.buttonClearSnapshotFrame.Click += new EventHandler(this.buttonClearSnapshotFrame_Click);
            this.buttonSnapshotNextSourceFrame.Location = new Point(11, 0x30);
            this.buttonSnapshotNextSourceFrame.Name = "buttonSnapshotNextSourceFrame";
            this.buttonSnapshotNextSourceFrame.Size = new Size(180, 30);
            this.buttonSnapshotNextSourceFrame.TabIndex = 0x12;
            this.buttonSnapshotNextSourceFrame.Text = "从摄像头输入截图";
            this.buttonSnapshotNextSourceFrame.UseVisualStyleBackColor = true;
            this.buttonSnapshotNextSourceFrame.Click += new EventHandler(this.buttonSnapshotNextSourceFrame_Click);
            this.pictureBoxScreenshot.BorderStyle = BorderStyle.FixedSingle;
            this.pictureBoxScreenshot.Location = new Point(7, 0x57);
            this.pictureBoxScreenshot.Name = "pictureBoxScreenshot";
            this.pictureBoxScreenshot.Size = new Size(0xb8, 0x7a);
            this.pictureBoxScreenshot.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBoxScreenshot.TabIndex = 0x13;
            this.pictureBoxScreenshot.TabStop = false;
            this.comboBoxResolutionList.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxResolutionList.FormattingEnabled = true;
            this.comboBoxResolutionList.Location = new Point(0x12, 0x4e);
            this.comboBoxResolutionList.Name = "comboBoxResolutionList";
            this.comboBoxResolutionList.Size = new Size(0xc2, 20);
            this.comboBoxResolutionList.TabIndex = 9;
            this.comboBoxResolutionList.SelectedIndexChanged += new EventHandler(this.comboBoxResolutionList_SelectedIndexChanged);
            this.labelResolutionTitle.AutoSize = true;
            this.labelResolutionTitle.Location = new Point(0x12, 0x3f);
            this.labelResolutionTitle.Margin = new Padding(3, 7, 3, 0);
            this.labelResolutionTitle.Name = "labelResolutionTitle";
            this.labelResolutionTitle.Size = new Size(0x4d, 12);
            this.labelResolutionTitle.TabIndex = 8;
            this.labelResolutionTitle.Text = "分辨率选择：";
            this.comboBoxCameraList.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBoxCameraList.FormattingEnabled = true;
            this.comboBoxCameraList.Location = new Point(0x12, 0x1f);
            this.comboBoxCameraList.Name = "comboBoxCameraList";
            this.comboBoxCameraList.Size = new Size(0xc2, 20);
            this.comboBoxCameraList.TabIndex = 6;
            this.comboBoxCameraList.SelectedIndexChanged += new EventHandler(this.comboBoxCameraList_SelectedIndexChanged);
            this.labelCameraTitle.AutoSize = true;
            this.labelCameraTitle.Location = new Point(0x12, 12);
            this.labelCameraTitle.Margin = new Padding(3, 7, 3, 0);
            this.labelCameraTitle.Name = "labelCameraTitle";
            this.labelCameraTitle.Size = new Size(0x4d, 12);
            this.labelCameraTitle.TabIndex = 5;
            this.labelCameraTitle.Text = "摄像头选择：";
            this.buttonUnZoom.FlatStyle = FlatStyle.Flat;
            this.buttonUnZoom.Font = new Font("宋体", 12f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
        //    this.buttonUnZoom.Image = (Image) manager.GetObject("buttonUnZoom.Image");
            this.buttonUnZoom.ImageAlign = ContentAlignment.MiddleRight;
            this.buttonUnZoom.Location = new Point(13, 0x12);
            this.buttonUnZoom.Margin = new Padding(0);
            this.buttonUnZoom.Name = "buttonUnZoom";
            this.buttonUnZoom.Size = new Size(0x73, 0x23);
            this.buttonUnZoom.TabIndex = 2;
            this.buttonUnZoom.Text = "放大还原";
            this.buttonUnZoom.TextAlign = ContentAlignment.MiddleLeft;
            this.buttonUnZoom.TextImageRelation = TextImageRelation.ImageBeforeText;
            this.buttonUnZoom.UseVisualStyleBackColor = true;
            this.buttonUnZoom.Visible = false;
            this.buttonUnZoom.Click += new EventHandler(this.buttonUnZoom_Click);
            this.cameraControl.BackColor = SystemColors.Control;
            this.cameraControl.DirectShowLogFilepath = "";
            this.cameraControl.Dock = DockStyle.Fill;
            this.cameraControl.Location = new Point(0, 0);
            this.cameraControl.Name = "cameraControl";
            this.cameraControl.Size = new Size(0x2bd, 0x213);
            this.cameraControl.TabIndex = 0;
            this.cameraControl.MouseDoubleClick += new MouseEventHandler(this.cameraControl_MouseDoubleClick);
            this.cameraControl.MouseDown += new MouseEventHandler(this.cameraControl_MouseDown);
            this.cameraControl.MouseMove += new MouseEventHandler(this.cameraControl_MouseMove);
            this.cameraControl.MouseUp += new MouseEventHandler(this.cameraControl_MouseUp);
            base.AutoScaleDimensions = new SizeF(6f, 12f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x3a7, 0x242);
            base.Controls.Add(this.splitContainer1);
            base.Controls.Add(this.statusStrip1);
            base.Controls.Add(this.menuStrip1);
          //  base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.MainMenuStrip = this.menuStrip1;
            base.Name = "Form1";
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "摄像头控制精灵";
            base.FormClosing += new FormClosingEventHandler(this.Form1_FormClosing);
            base.Load += new EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((ISupportInitialize) this.pictureBoxScreenshot).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private bool IsMouseSelectionRectCorrect()
        {
            if ((Math.Abs((float) (this._MouseSelectionRect.right - this._MouseSelectionRect.left)) < 1.401298E-44f) || (Math.Abs((float) (this._MouseSelectionRect.bottom - this._MouseSelectionRect.top)) < 1.401298E-44f))
            {
                return false;
            }
            if ((this._MouseSelectionRect.left >= this._MouseSelectionRect.right) || (this._MouseSelectionRect.top >= this._MouseSelectionRect.bottom))
            {
                return false;
            }
            return (((this._MouseSelectionRect.left >= 0f) && (this._MouseSelectionRect.top >= 0f)) && ((this._MouseSelectionRect.right <= 1.0) && (this._MouseSelectionRect.bottom <= 1.0)));
        }

        private bool IsMouseSelectionRectCorrectAndGood()
        {
            if (!this.IsMouseSelectionRectCorrect())
            {
                return false;
            }
            return ((Math.Abs((float) (this._MouseSelectionRect.right - this._MouseSelectionRect.left)) >= 0.1f) && (Math.Abs((float) (this._MouseSelectionRect.bottom - this._MouseSelectionRect.top)) >= 0.1f));
        }

        protected void RenameNewFile(string sFileName)
        {
            string path = sFileName + ".new";
            bool flag = false;
            if (File.Exists(path))
            {
                flag = File.Exists(sFileName);
                string startupPath = Application.StartupPath;
                string destFileName = startupPath + @"\" + sFileName;
                string fileName = startupPath + @"\" + path;
                try
                {
                    if (!flag)
                    {
                        new FileInfo(fileName).MoveTo(destFileName);
                    }
                    else
                    {
                        File.Delete(destFileName);
                        new FileInfo(fileName).MoveTo(destFileName);
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private void SetCamera(IMoniker camera_moniker, Resolution resolution)
        {
            try
            {
                this.cameraControl.SetCamera(camera_moniker, resolution);
            }
            catch (Exception exception1)
            {
                MessageBox.Show(exception1.Message, "Error while running camera");
            }
            if (this.cameraControl.CameraCreated)
            {
                this.cameraControl.MixerEnabled = true;
                this.cameraControl.OutputVideoSizeChanged += new EventHandler(this.Camera_OutputVideoSizeChanged);
                this.UpdateCameraBitmap();
                this.UpdateGUIButtons();
            }
        }

        protected void StartUpdate()
        {
            string startupPath = Application.StartupPath;
            string path = startupPath + @"\\update.exe";
            this.RenameNewFile("update.exe");
            if (File.Exists(path))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo {
                    FileName = path,
                    Arguments = "电脑自动关机软件:AutoShutdown",
                    WindowStyle = ProcessWindowStyle.Minimized,
                    WorkingDirectory = startupPath
                };
                try
                {
                    Process.Start(startInfo);
                }
                catch (Win32Exception)
                {
                }
            }
        }

        private void UnzoomCamera()
        {
            this.cameraControl.ZoomToRect(new Rectangle(0, 0, this.cameraControl.Resolution.Width, this.cameraControl.Resolution.Height));
            this._bZoomed = false;
            this._fZoomValue = 1.0;
            this.UpdateCameraBitmap();
            this.UpdateUnzoomButton();
            this._bDrawMouseSelection = false;
        }

        private void UpdateCameraBitmap()
        {
            if (this.cameraControl.MixerEnabled)
            {
                this.cameraControl.OverlayBitmap = this.GenerateColorKeyBitmap(false);
            }
        }

        private void UpdateGUIButtons()
        {
            this.buttonCrossbarSettings.Enabled = this.cameraControl.CrossbarAvailable;
        }

        private void UpdateUnzoomButton()
        {
            if (this._bZoomed)
            {
                this.buttonUnZoom.Left = (this.cameraControl.Left + ((this.cameraControl.ClientRectangle.Width - this.cameraControl.OutputVideoSize.Width) / 2)) + 4;
                this.buttonUnZoom.Top = (this.cameraControl.Top + ((this.cameraControl.ClientRectangle.Height - this.cameraControl.OutputVideoSize.Height) / 2)) + 40;
                this.buttonUnZoom.Visible = true;
            }
            else
            {
                this.buttonUnZoom.Visible = false;
            }
        }

        private void 帮助HToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new About { Owner = this }.Show();
        }
    }
}

