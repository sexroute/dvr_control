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
    using ZXing;
    using EricZhao.UiThread;
    using System.Threading;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using Newtonsoft.Json.Linq;
    using System.Net;

    public class Main : Form
    {
        private bool _bDrawMouseSelection;
        private bool _bZoomed;
        private CameraChoice _CameraChoice = new CameraChoice();
        private double _fZoomValue = 1.0;
        private NormalizedRect _MouseSelectionRect = new NormalizedRect(0f, 0f, 0f, 0f);
        private Button buttonCameraSettings;
        private Button buttonClearSnapshotFrame;
        private Button buttonPinOutputSettings;
        private Button buttonSaveSnapshotFrame;
        private Button buttonSnapshotNextSourceFrame;
        private Button buttonSnapshotOutputFrame;
        private Button buttonUnZoom;
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
        private ToolStripMenuItem 保存截图PToolStripMenuItem;
        private ToolStripMenuItem 菜单MToolStripMenuItem;
        private ToolStripMenuItem 输出截图ToolStripMenuItem;
        private ToolStripMenuItem 输出设置ToolStripMenuItem;
        private ToolStripMenuItem 输入截图IToolStripMenuItem;
        private CameraControl cameraControl;
        private System.Windows.Forms.Timer timer1;
        private ToolStripMenuItem testToolStripMenuItem;
        private ToolStripMenuItem autoSearchToolStripMenuItem;
        private System.Windows.Forms.Timer timer2;
        private ToolStripMenuItem 属性AToolStripMenuItem;

        public Main()
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
                   
                    this.UpdateCameraBitmap();
                }
                else
                {
                  
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
                SaveFileDialog dialog = new SaveFileDialog
                {
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
                PointF tf = this.cameraControl.ConvertWinToNorm(new PointF((float)e.X, (float)e.Y));
                this._MouseSelectionRect = new NormalizedRect(tf.X, tf.Y, tf.X, tf.Y);
                this._bDrawMouseSelection = true;
                this.UpdateCameraBitmap();
            }
        }

        private void cameraControl_MouseMove(object sender, MouseEventArgs e)
        {
            if ((((e.Button == MouseButtons.Left) && this.cameraControl.CameraCreated) && !this._bZoomed) && this._bDrawMouseSelection)
            {
                PointF tf = this.cameraControl.ConvertWinToNorm(new PointF((float)e.X, (float)e.Y));
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
                    double num5 = ((double)width) / ((double)height);
                    double num6 = num3 / num4;
                    if (num5 >= num6)
                    {
                        double num7 = num4 * num5;
                        this._MouseSelectionRect.left -= (float)(((num7 - num3) / 2.0) / ((double)width));
                        this._MouseSelectionRect.right += (float)(((num7 - num3) / 2.0) / ((double)width));
                        this._fZoomValue = ((double)height) / num4;
                    }
                    else
                    {
                        double num8 = num3 / num5;
                        this._MouseSelectionRect.top -= (float)(((num8 - num4) / 2.0) / ((double)height));
                        this._MouseSelectionRect.bottom += (float)(((num8 - num4) / 2.0) / ((double)height));
                        this._fZoomValue = ((double)width) / num3;
                    }
                    Rectangle zoomRect = new Rectangle((int)(this._MouseSelectionRect.left * width), (int)(this._MouseSelectionRect.top * height), (int)((this._MouseSelectionRect.right - this._MouseSelectionRect.left) * width), (int)((this._MouseSelectionRect.bottom - this._MouseSelectionRect.top) * height));
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
                try
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
                catch (Exception e)
                {
                    ThreadUiController.log(e.Message, ThreadUiController.LOG_LEVEL.FATAL);
                }

            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.cameraControl.CloseCamera();          
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.SetupQRDecoder();
            this.loadSetting();
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
            this.toolStripStatusLabel1.Text = "";
            this.StartSenderThread();
            this.StartAutoSearchThread();
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
                Rectangle rectangle = new Rectangle((int)(this._MouseSelectionRect.left * width), (int)(this._MouseSelectionRect.top * height), (int)((this._MouseSelectionRect.right - this._MouseSelectionRect.left) * width), (int)((this._MouseSelectionRect.bottom - this._MouseSelectionRect.top) * height));
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
                graphics.DrawString("Zoom: " + Math.Round(this._fZoomValue, 1).ToString("0.0") + "x", font, brush, (float)4f, (float)4f);
                font.Dispose();
                brush.Dispose();
            }
            Font font2 = new Font("Tahoma", 16f);
            Brush brush2 = new SolidBrush(Color.BlueViolet);
            string text = this.textBoxZiMu.Text;
            graphics.DrawString(text, font2, brush2, 4f, (float)(height - 30));
            graphics.Dispose();
            return image;
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.菜单MToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.属性AToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.输出设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.输出截图ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.输入截图IToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.保存截图PToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.textBoxZiMu = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonCameraSettings = new System.Windows.Forms.Button();
            this.buttonPinOutputSettings = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonSaveSnapshotFrame = new System.Windows.Forms.Button();
            this.buttonSnapshotOutputFrame = new System.Windows.Forms.Button();
            this.buttonClearSnapshotFrame = new System.Windows.Forms.Button();
            this.buttonSnapshotNextSourceFrame = new System.Windows.Forms.Button();
            this.pictureBoxScreenshot = new System.Windows.Forms.PictureBox();
            this.comboBoxResolutionList = new System.Windows.Forms.ComboBox();
            this.labelResolutionTitle = new System.Windows.Forms.Label();
            this.comboBoxCameraList = new System.Windows.Forms.ComboBox();
            this.labelCameraTitle = new System.Windows.Forms.Label();
            this.cameraControl = new Camera_NET.CameraControl();
            this.buttonUnZoom = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScreenshot)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.菜单MToolStripMenuItem,
            this.testToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(935, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // 菜单MToolStripMenuItem
            // 
            this.菜单MToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.属性AToolStripMenuItem,
            this.输出设置ToolStripMenuItem,
            this.输出截图ToolStripMenuItem,
            this.输入截图IToolStripMenuItem,
            this.保存截图PToolStripMenuItem});
            this.菜单MToolStripMenuItem.Name = "菜单MToolStripMenuItem";
            this.菜单MToolStripMenuItem.Size = new System.Drawing.Size(64, 21);
            this.菜单MToolStripMenuItem.Text = "菜单(&M)";
            // 
            // 属性AToolStripMenuItem
            // 
            this.属性AToolStripMenuItem.Name = "属性AToolStripMenuItem";
            this.属性AToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.属性AToolStripMenuItem.Text = "设置属性(&A)";
            this.属性AToolStripMenuItem.Click += new System.EventHandler(this.buttonCameraSettings_Click);
            // 
            // 输出设置ToolStripMenuItem
            // 
            this.输出设置ToolStripMenuItem.Name = "输出设置ToolStripMenuItem";
            this.输出设置ToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.输出设置ToolStripMenuItem.Text = "输出设置(&S)";
            this.输出设置ToolStripMenuItem.Click += new System.EventHandler(this.buttonPinOutputSettings_Click);
            // 
            // 输出截图ToolStripMenuItem
            // 
            this.输出截图ToolStripMenuItem.Name = "输出截图ToolStripMenuItem";
            this.输出截图ToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.输出截图ToolStripMenuItem.Text = "输出截图(&O)";
            this.输出截图ToolStripMenuItem.Click += new System.EventHandler(this.buttonSnapshotOutputFrame_Click);
            // 
            // 输入截图IToolStripMenuItem
            // 
            this.输入截图IToolStripMenuItem.Name = "输入截图IToolStripMenuItem";
            this.输入截图IToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.输入截图IToolStripMenuItem.Text = "输入截图(&I)";
            this.输入截图IToolStripMenuItem.Click += new System.EventHandler(this.buttonSnapshotNextSourceFrame_Click);
            // 
            // 保存截图PToolStripMenuItem
            // 
            this.保存截图PToolStripMenuItem.Name = "保存截图PToolStripMenuItem";
            this.保存截图PToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.保存截图PToolStripMenuItem.Text = "保存截图(&P)";
            this.保存截图PToolStripMenuItem.Click += new System.EventHandler(this.buttonSaveSnapshotFrame_Click);
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoSearchToolStripMenuItem});
            this.testToolStripMenuItem.Enabled = false;
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.testToolStripMenuItem.Text = "Test";
            // 
            // autoSearchToolStripMenuItem
            // 
            this.autoSearchToolStripMenuItem.Name = "autoSearchToolStripMenuItem";
            this.autoSearchToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.autoSearchToolStripMenuItem.Text = "Auto Search";
            this.autoSearchToolStripMenuItem.Click += new System.EventHandler(this.autoSearchToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 556);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(935, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(131, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.splitContainer1.Panel1.Controls.Add(this.textBoxZiMu);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1.Controls.Add(this.comboBoxResolutionList);
            this.splitContainer1.Panel1.Controls.Add(this.labelResolutionTitle);
            this.splitContainer1.Panel1.Controls.Add(this.comboBoxCameraList);
            this.splitContainer1.Panel1.Controls.Add(this.labelCameraTitle);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Info;
            this.splitContainer1.Panel2.Controls.Add(this.cameraControl);
            this.splitContainer1.Size = new System.Drawing.Size(935, 531);
            this.splitContainer1.SplitterDistance = 230;
            this.splitContainer1.TabIndex = 2;
            // 
            // textBoxZiMu
            // 
            this.textBoxZiMu.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxZiMu.Location = new System.Drawing.Point(54, 236);
            this.textBoxZiMu.Name = "textBoxZiMu";
            this.textBoxZiMu.Size = new System.Drawing.Size(146, 21);
            this.textBoxZiMu.TabIndex = 24;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 240);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 23;
            this.label1.Text = "识别：";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonCameraSettings);
            this.groupBox2.Controls.Add(this.buttonPinOutputSettings);
            this.groupBox2.Location = new System.Drawing.Point(14, 114);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(198, 99);
            this.groupBox2.TabIndex = 22;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "设置";
            // 
            // buttonCameraSettings
            // 
            this.buttonCameraSettings.Location = new System.Drawing.Point(11, 26);
            this.buttonCameraSettings.Name = "buttonCameraSettings";
            this.buttonCameraSettings.Size = new System.Drawing.Size(175, 30);
            this.buttonCameraSettings.TabIndex = 11;
            this.buttonCameraSettings.Text = "调整摄像头";
            this.buttonCameraSettings.UseVisualStyleBackColor = true;
            this.buttonCameraSettings.Click += new System.EventHandler(this.buttonCameraSettings_Click);
            // 
            // buttonPinOutputSettings
            // 
            this.buttonPinOutputSettings.Location = new System.Drawing.Point(11, 62);
            this.buttonPinOutputSettings.Name = "buttonPinOutputSettings";
            this.buttonPinOutputSettings.Size = new System.Drawing.Size(175, 30);
            this.buttonPinOutputSettings.TabIndex = 12;
            this.buttonPinOutputSettings.Text = "输出设置";
            this.buttonPinOutputSettings.UseVisualStyleBackColor = true;
            this.buttonPinOutputSettings.Click += new System.EventHandler(this.buttonPinOutputSettings_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonSaveSnapshotFrame);
            this.groupBox1.Controls.Add(this.buttonSnapshotOutputFrame);
            this.groupBox1.Controls.Add(this.buttonClearSnapshotFrame);
            this.groupBox1.Controls.Add(this.buttonSnapshotNextSourceFrame);
            this.groupBox1.Controls.Add(this.pictureBoxScreenshot);
            this.groupBox1.Location = new System.Drawing.Point(14, 264);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(198, 256);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "截图";
            // 
            // buttonSaveSnapshotFrame
            // 
            this.buttonSaveSnapshotFrame.Location = new System.Drawing.Point(114, 215);
            this.buttonSaveSnapshotFrame.Name = "buttonSaveSnapshotFrame";
            this.buttonSaveSnapshotFrame.Size = new System.Drawing.Size(72, 30);
            this.buttonSaveSnapshotFrame.TabIndex = 21;
            this.buttonSaveSnapshotFrame.Text = "保存截图";
            this.buttonSaveSnapshotFrame.UseVisualStyleBackColor = true;
            this.buttonSaveSnapshotFrame.Click += new System.EventHandler(this.buttonSaveSnapshotFrame_Click);
            // 
            // buttonSnapshotOutputFrame
            // 
            this.buttonSnapshotOutputFrame.Location = new System.Drawing.Point(11, 16);
            this.buttonSnapshotOutputFrame.Name = "buttonSnapshotOutputFrame";
            this.buttonSnapshotOutputFrame.Size = new System.Drawing.Size(180, 30);
            this.buttonSnapshotOutputFrame.TabIndex = 17;
            this.buttonSnapshotOutputFrame.Text = "从输出截图";
            this.buttonSnapshotOutputFrame.UseVisualStyleBackColor = true;
            this.buttonSnapshotOutputFrame.Click += new System.EventHandler(this.buttonSnapshotOutputFrame_Click);
            // 
            // buttonClearSnapshotFrame
            // 
            this.buttonClearSnapshotFrame.Location = new System.Drawing.Point(11, 215);
            this.buttonClearSnapshotFrame.Name = "buttonClearSnapshotFrame";
            this.buttonClearSnapshotFrame.Size = new System.Drawing.Size(68, 30);
            this.buttonClearSnapshotFrame.TabIndex = 20;
            this.buttonClearSnapshotFrame.Text = "清空截图";
            this.buttonClearSnapshotFrame.UseVisualStyleBackColor = true;
            this.buttonClearSnapshotFrame.Click += new System.EventHandler(this.buttonClearSnapshotFrame_Click);
            // 
            // buttonSnapshotNextSourceFrame
            // 
            this.buttonSnapshotNextSourceFrame.Location = new System.Drawing.Point(11, 48);
            this.buttonSnapshotNextSourceFrame.Name = "buttonSnapshotNextSourceFrame";
            this.buttonSnapshotNextSourceFrame.Size = new System.Drawing.Size(180, 30);
            this.buttonSnapshotNextSourceFrame.TabIndex = 18;
            this.buttonSnapshotNextSourceFrame.Text = "从摄像头输入截图";
            this.buttonSnapshotNextSourceFrame.UseVisualStyleBackColor = true;
            this.buttonSnapshotNextSourceFrame.Click += new System.EventHandler(this.buttonSnapshotNextSourceFrame_Click);
            // 
            // pictureBoxScreenshot
            // 
            this.pictureBoxScreenshot.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxScreenshot.Location = new System.Drawing.Point(7, 87);
            this.pictureBoxScreenshot.Name = "pictureBoxScreenshot";
            this.pictureBoxScreenshot.Size = new System.Drawing.Size(184, 122);
            this.pictureBoxScreenshot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxScreenshot.TabIndex = 19;
            this.pictureBoxScreenshot.TabStop = false;
            // 
            // comboBoxResolutionList
            // 
            this.comboBoxResolutionList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxResolutionList.FormattingEnabled = true;
            this.comboBoxResolutionList.Location = new System.Drawing.Point(18, 78);
            this.comboBoxResolutionList.Name = "comboBoxResolutionList";
            this.comboBoxResolutionList.Size = new System.Drawing.Size(194, 20);
            this.comboBoxResolutionList.TabIndex = 9;
            this.comboBoxResolutionList.TabStop = false;
            this.comboBoxResolutionList.SelectedIndexChanged += new System.EventHandler(this.comboBoxResolutionList_SelectedIndexChanged);
            // 
            // labelResolutionTitle
            // 
            this.labelResolutionTitle.AutoSize = true;
            this.labelResolutionTitle.Location = new System.Drawing.Point(18, 63);
            this.labelResolutionTitle.Margin = new System.Windows.Forms.Padding(3, 7, 3, 0);
            this.labelResolutionTitle.Name = "labelResolutionTitle";
            this.labelResolutionTitle.Size = new System.Drawing.Size(77, 12);
            this.labelResolutionTitle.TabIndex = 8;
            this.labelResolutionTitle.Text = "分辨率选择：";
            // 
            // comboBoxCameraList
            // 
            this.comboBoxCameraList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCameraList.FormattingEnabled = true;
            this.comboBoxCameraList.Location = new System.Drawing.Point(18, 31);
            this.comboBoxCameraList.Name = "comboBoxCameraList";
            this.comboBoxCameraList.Size = new System.Drawing.Size(194, 20);
            this.comboBoxCameraList.TabIndex = 6;
            this.comboBoxCameraList.TabStop = false;
            this.comboBoxCameraList.SelectedIndexChanged += new System.EventHandler(this.comboBoxCameraList_SelectedIndexChanged);
            // 
            // labelCameraTitle
            // 
            this.labelCameraTitle.AutoSize = true;
            this.labelCameraTitle.Location = new System.Drawing.Point(18, 12);
            this.labelCameraTitle.Margin = new System.Windows.Forms.Padding(3, 7, 3, 0);
            this.labelCameraTitle.Name = "labelCameraTitle";
            this.labelCameraTitle.Size = new System.Drawing.Size(77, 12);
            this.labelCameraTitle.TabIndex = 5;
            this.labelCameraTitle.Text = "摄像头选择：";
            // 
            // cameraControl
            // 
            this.cameraControl.DirectShowLogFilepath = "";
            this.cameraControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cameraControl.Location = new System.Drawing.Point(0, 0);
            this.cameraControl.Name = "cameraControl";
            this.cameraControl.Size = new System.Drawing.Size(701, 531);
            this.cameraControl.TabIndex = 0;
            // 
            // buttonUnZoom
            // 
            this.buttonUnZoom.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonUnZoom.Font = new System.Drawing.Font("SimSun", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonUnZoom.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonUnZoom.Location = new System.Drawing.Point(13, 18);
            this.buttonUnZoom.Margin = new System.Windows.Forms.Padding(0);
            this.buttonUnZoom.Name = "buttonUnZoom";
            this.buttonUnZoom.Size = new System.Drawing.Size(115, 35);
            this.buttonUnZoom.TabIndex = 2;
            this.buttonUnZoom.Text = "放大还原";
            this.buttonUnZoom.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonUnZoom.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.buttonUnZoom.UseVisualStyleBackColor = true;
            this.buttonUnZoom.Visible = false;
            this.buttonUnZoom.Click += new System.EventHandler(this.buttonUnZoom_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Interval = 1000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(935, 578);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "摄像头控制器";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScreenshot)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private bool IsMouseSelectionRectCorrect()
        {
            if ((Math.Abs((float)(this._MouseSelectionRect.right - this._MouseSelectionRect.left)) < 1.401298E-44f) || (Math.Abs((float)(this._MouseSelectionRect.bottom - this._MouseSelectionRect.top)) < 1.401298E-44f))
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
            return ((Math.Abs((float)(this._MouseSelectionRect.right - this._MouseSelectionRect.left)) >= 0.1f) && (Math.Abs((float)(this._MouseSelectionRect.bottom - this._MouseSelectionRect.top)) >= 0.1f));
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

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
        BarcodeReader reader = new BarcodeReader();

        private void SetupQRDecoder()
        {
            reader.Options.CharacterSet = "UTF-8";
            reader.TryInverted = false;
            reader.Options.PossibleFormats = new List<ZXing.BarcodeFormat>();
            reader.Options.PossibleFormats.Add(BarcodeFormat.QR_CODE);
            reader.AutoRotate = false;
        }


        DateTime m_nTimeLastSucceed = DateTime.Now;
        int m_nTimeSecondsThreshold = 100; //seconds
        public int TimeSecondsThreshold
        {
            get { lock (this) return m_nTimeSecondsThreshold; }
            set { lock (this) m_nTimeSecondsThreshold = value; }
        }
        public System.DateTime TimeLastSucceed
        {
            get { lock(this)return m_nTimeLastSucceed; }
            set { lock (this) m_nTimeLastSucceed = value; }
        }

        private void MarkScanSucceed()
        {
            this.TimeLastSucceed = DateTime.Now;
            ThreadUiController.Feed();
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            Boolean lbEncodeSuccessfully = false;
          
           
            if (this.cameraControl.CameraCreated)
            {

                try
                {
                    //  return;
                    using (Bitmap bitmap = this.cameraControl.SnapshotSourceImage())
                    {
                        if (bitmap != null)
                        {
                            //using (Bitmap bitMapGray = ToGray(bitmap))
                            {
                                
                                Result result = reader.Decode(bitmap);
                                if (null != result)
                                {
                                    this.textBoxZiMu.Text = result.Text;
                                    if (!String.IsNullOrEmpty(result.Text))
                                    {
                                        if(result.Text.ToUpper().CompareTo("OK".ToUpper())==0)
                                        {
                                            this.MarkScanSucceed();
                                        }
                                        this.PushData(result.Text);
                                    }
                                    this.UpdateCameraBitmap();
                                    lbEncodeSuccessfully = true;
                                    this.AutoSearch = 2;
                                }
                            }
                           

                        }
                    }

                }
                catch (Exception exception1)
                {
                    ThreadUiController.log(exception1.Message, ThreadUiController.LOG_LEVEL.FATAL);
                    // MessageBox.Show(exception1.Message, "Error while getting a snapshot");
                }

                if(!lbEncodeSuccessfully)
                {
                    try
                    {
                        this.textBoxZiMu.Text = "";
                        this.UpdateCameraBitmap();
                    }
                    catch(Exception ex)
                    {

                    }

                }

            }
        }
        String m_strRemoteServerUrl = "http://192.168.122.97:8180/?";
        public System.String RemoteServerUrl
        {
            get { lock (this) return m_strRemoteServerUrl; }
            set { lock (this) m_strRemoteServerUrl = value; }
        }
        List<String> m_Datas = new List<string>();
        int m_nDataQueueSize = 1024;
        public int DataQueueSize
        {
            get { lock (this) return m_nDataQueueSize; }
            set { if (value <= 0) { return; } lock (this) m_nDataQueueSize = value; }
        }
        public void PushData(String astrData)
        {
            lock (this)
            {
                if (m_Datas.Count > this.DataQueueSize)
                {
                    this.m_Datas.RemoveAt(0);
                }
                
                {
                    this.m_Datas.Add(astrData);
                }
            }
        }

        public String GetData()
        {
            lock (this)
            {
                if (this.m_Datas.Count > 0)
                {
                    String lstrData = this.m_Datas[0];
                    this.m_Datas.RemoveAt(0);
                    return lstrData;
                }
            }
            return null;
        }
        IniFile m_oSettingsFile = new IniFile("./settings.ini");
        public EricZhao.UiThread.IniFile SettingsFile
        {
            get { lock (this) return m_oSettingsFile; }
            set { lock (this) m_oSettingsFile = value; }
        }
        String m_strAlarmIDLast = "0";
        public System.String AlarmIDLast
        {
            get { lock(this) return m_strAlarmIDLast; }
            set { lock (this) m_strAlarmIDLast = value; }
        }

        String m_strMaintainacePhone = "18612240190";
        public System.String MaintainacePhone
        {
            get { lock(this)return m_strMaintainacePhone; }
            set { lock(this)m_strMaintainacePhone = value; }
        }

        String m_strSMSUrl = "";
        public System.String SMSUrl
        {
            get { lock (this) return m_strSMSUrl; }
            set { lock (this) m_strSMSUrl = value; }
        }
        public void loadSetting()
        {
            lock(this)
            {
                this.RemoteServerUrl = this.SettingsFile.IniReadStringValue("setting", "url", "http://127.0.0.1", true);
                this.DataQueueSize = this.SettingsFile.IniReadIntValue("setting", "queue_size", 1024, true);
                this.AlarmIDLast = this.SettingsFile.IniReadStringValue("setting", "alarm_id_last", "0", true);
                this.TimeSecondsThreshold = this.SettingsFile.IniReadIntValue("setting", "succeed_time_sencond_threshold", 100, true);
                this.MaintainacePhone = this.SettingsFile.IniReadStringValue("setting", "maintanace_phone", this.MaintainacePhone, true);
                this.SMSUrl = this.SettingsFile.IniReadStringValue("setting", "maintanace_sms_url", this.SMSUrl, true);
            }

        }

        public void saveSetting()
        {
            lock(this)
            {
                this.SettingsFile.IniWriteStringValue("setting", "url", this.RemoteServerUrl);
                this.SettingsFile.IniWriteStringValue("setting", "alarm_id_last", this.AlarmIDLast);
            }

        }
        public Thread m_pSenderThread = null;
        public void StopSenderThread()
        {
            lock (this)
            {
                try
                {
                    if (this.m_pSenderThread != null)
                    {
                        this.m_pSenderThread.Abort();
                    }
                }
                catch (Exception e)
                {

                }

                try
                {
                    this.m_pSenderThread = null;
                }
                catch (Exception e)
                {

                }
            }
        }

        Thread m_pThreadAutoSearch = null;

        public void StopAutoSearchThread()
        {
            lock (this)
            {
                try
                {
                    if (this.m_pThreadAutoSearch != null)
                    {
                        this.m_pThreadAutoSearch.Abort();
                    }
                }
                catch (Exception e)
                {

                }

                try
                {
                    this.m_pThreadAutoSearch = null;
                }
                catch (Exception e)
                {

                }
            }
        }

        public void StartAutoSearchThread()
        {
            this.StopAutoSearchThread();

            lock (this)
            {
                try
                {
                    this.m_pThreadAutoSearch = new Thread(this.ThreadAutoSearch);
                    this.m_pThreadAutoSearch.IsBackground = true;
                    this.m_pThreadAutoSearch.Start();
                }
                catch (Exception e)
                {

                }
            }
        }

        String m_strDeviceName = "";
        public System.String DeviceName
        {
            get { lock (this) return m_strDeviceName; }
            set { lock (this) m_strDeviceName = value; }
        }
        int m_bAutoSearch = 0;
        public int AutoSearch
        {
            get { lock(this)return m_bAutoSearch; }
            set { lock (this) m_bAutoSearch = value; }
        }

        int m_nXMin = -131;
        int m_nXMax = 182;
        int m_nYMin = -17;
        int m_nYMax = 90;
        int m_nSearchInterval = 20;
        int m_nSearchInterval2 = 100;
        public void ThreadAutoSearch()
        {
            int lnStartXIndex = m_nXMin;
            int lnStartYIndex = m_nYMin;
            while (true)
            {
                try
                {
                    if(AutoSearch>0 && !String.IsNullOrEmpty(this.DeviceName))
                    {
                        PTZDevice lpDevice = new PTZDevice(this.DeviceName, PTZType.Relative);
                        lpDevice.Move(m_nXMin, m_nYMin);
                        for (int x= lnStartXIndex; x<=m_nXMax;x=x+m_nSearchInterval)
                        {
                            if(x>m_nXMax)
                            {
                                x = m_nXMax;
                            }

                            for(int y= lnStartYIndex; y<=m_nYMax;y=y+m_nSearchInterval)
                            {
                                if(y>m_nYMax)
                                {
                                    y = m_nYMax;
                                }

                                if (AutoSearch == 2 || AutoSearch == 0)
                                {
                                    AutoSearch = 0;
                                    break;
                                }
                                //                                 lpDevice.MoveXRelative(m_nSearchInterval2);
                                //                                 Thread.Sleep(1000);
                                //                                 lpDevice.MoveYRealtive(m_nSearchInterval2);
                                lpDevice.Move(x, y);
                                Thread.Sleep(2000);
                                if(AutoSearch==2 || AutoSearch ==0)
                                {
                                    AutoSearch = 0;
                                    break;
                                }
                            }

                            if (AutoSearch == 2 || AutoSearch == 0)
                            {
                                AutoSearch = 0;
                                break;
                            }
                        }

                    }
                }catch(Exception e)
                {
                    ThreadUiController.log(e.Message, ThreadUiController.LOG_LEVEL.FATAL);
                }
            }
        }

        /// <summary>
        /// 图像灰度化
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Bitmap ToGray(Bitmap bmp)
        {
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    //获取该点的像素的RGB的颜色
                    Color color = bmp.GetPixel(i, j);
                    //利用公式计算灰度值
                    int gray = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
                    Color newColor = Color.FromArgb(gray, gray, gray);
                    bmp.SetPixel(i, j, newColor);
                }
            }
            return bmp;
        }

        public void StartSenderThread()
        {
            this.StopSenderThread();
            this.TimeLastSucceed = DateTime.Now;
            lock (this)
            {
                try
                {
                    this.m_pSenderThread = new Thread(this.ThreadSendToRemoteServer);
                    this.m_pSenderThread.IsBackground = true;
                    this.m_pSenderThread.Start();
                }
                catch (Exception e)
                {

                }
            }
        }

        public Boolean PostDataToRemoteServer2(String astrata,String astrUrl)
        {
            Boolean lbSucceed = false;
            try
            {
                using (var client = new HttpClient())                {

                    var data = new System.Net.Http.StringContent(astrata, Encoding.UTF8, "application/json");
                    var response =   client.PostAsync(astrUrl, data).Result;
                    if (response.IsSuccessStatusCode)
                    {                       
                        lbSucceed = true;
                    }
                }
            }
            catch(Exception e)
            {

            }

            return lbSucceed;
        }

        public Boolean PostDataToRemoteServer(String astrData,String astrUrl)
        {
            Boolean lbRet = false;
            try
            {
                var http = (HttpWebRequest)WebRequest.Create(new Uri(this.RemoteServerUrl));
                http.Accept = "application/json";
                http.ContentType = "application/json";
                http.Method = "POST";
                UTF8Encoding encoding = new UTF8Encoding();
                Byte[] bytes = encoding.GetBytes(astrData);

                try
                {
                    using (Stream newStream = http.GetRequestStream())
                    {
                        newStream.Write(bytes, 0, bytes.Length);
                        newStream.Close();
                    }

                }
                catch (Exception e)
                {

                }

                try
                {
                    HttpWebResponse response = (HttpWebResponse)http.GetResponse();

                    if(response.StatusCode== HttpStatusCode.OK)
                    {
                        var stream = response.GetResponseStream();
                        using (var sr = new StreamReader(stream))
                        {
                            var content = sr.ReadToEnd();
                            Debug.WriteLine(content);
                            if(String.IsNullOrEmpty(content))
                            {
                                lbRet = true;
                            }
                        }
                    }

                }
                catch (Exception e)
                {

                }
            }
            catch (Exception e2)
            {
                Debug.WriteLine(e2.Message);
            }

            return lbRet;
        }

        public void ThreadSendToRemoteServer()
        {
            while (true)
            {
                Boolean lbSucceed = false;
                try
                {
                    String lstrData = this.GetData();
                    try
                    {
                        if(String.IsNullOrEmpty(lstrData))
                        {
                            continue;
                        }
                        Newtonsoft.Json.Linq.JArray array = null;
                        try
                        {
                            array = Newtonsoft.Json.Linq.JArray.Parse(lstrData);
                            lbSucceed = true;
                        }
                        catch(Exception e)
                        {
                            ThreadUiController.log("Parsing data Error:"+lstrData, ThreadUiController.LOG_LEVEL.FATAL);
                            ThreadUiController.log(e.Message, ThreadUiController.LOG_LEVEL.FATAL);
                            continue;
                        }


                        if (lstrData != null)
                        {
                           
                            String lstrAlarmIDParsed = "";

                            foreach (JObject obj in array.Children<JObject>())
                            {
                                foreach (JProperty property in obj.Properties())
                                {
                                    string name = property.Name;
                                    string value = property.Value.ToString();

                                    if (property.Name == "alarmid")
                                    {
                                        lstrAlarmIDParsed = property.Value.ToString();
                                        break;
                                    }
                                    //Do something with name and value
                                    //System.Windows.MessageBox.Show("name is "+name+" and value is "+value);
                                }
                            }

                            if (String.IsNullOrEmpty(lstrAlarmIDParsed))
                            {
                                continue;
                            }


                            if ( String.Compare(lstrAlarmIDParsed, this.AlarmIDLast) == 0)
                            {
                                lbSucceed = true;
                                continue;
                            }


                            lbSucceed = PostDataToRemoteServer2(lstrData, this.RemoteServerUrl);
                            if(lbSucceed)
                            {
                                this.AlarmIDLast = lstrAlarmIDParsed;
                                this.saveSetting();
                            }

                        }
                    }
                    catch (Exception e)
                    {
                        this.PushData(lstrData);
                    }finally
                    {
                        if(lbSucceed)
                        {
                            this.MarkScanSucceed();
                        }
                    }
                }
                catch (Exception e)
                {
                    Thread.Sleep(100);
                }
            }
        }

       


        private void autoSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String lstrDeviceName = comboBoxCameraList.Items[0].ToString();
            this.DeviceName = lstrDeviceName;
            this.autoSearchToolStripMenuItem.Checked = !this.autoSearchToolStripMenuItem.Checked;
            Thread.Sleep(100);
            if(this.autoSearchToolStripMenuItem.Checked)
            {
                this.AutoSearch = 1;
            }
            else
            {
                this.AutoSearch = 0;
            }
        }


        private void NotifyMaintanaceStaff()
        {
            if (!String.IsNullOrWhiteSpace(this.SMSUrl) 
                && !(String.IsNullOrWhiteSpace(this.MaintainacePhone)))
            {

            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
           TimeSpan lpOffset = DateTime.Now - this.TimeLastSucceed;
            if(lpOffset.TotalSeconds>this.TimeSecondsThreshold)
            {
                try
                {
                    this.NotifyMaintanaceStaff();
                }catch(Exception ex)
                {
                    ThreadUiController.log(ex.Message, ThreadUiController.LOG_LEVEL.FATAL);
                }
            }
        }
    }
}

