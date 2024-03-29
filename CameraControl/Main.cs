﻿namespace CameraControl
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
    using Aliyun.Acs.Core.Profile;
    using Aliyun.Acs.Core;
    using Aliyun.Acs.Core.Http;
    using Aliyun.Acs.Core.Exceptions;
    using Aliyun.Acs.Dysmsapi.Model.V20170525;
    using Newtonsoft.Json;
    using TencentCloud.Common;
    using TencentCloud.Common.Profile;
    using TencentCloud.Cvm.V20170312;
    using TencentCloud.Cvm.V20170312.Models;
    using TencentCloud.Ocr.V20181119;
    using TencentCloud.Ocr.V20181119.Models;

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
        private System.Windows.Forms.Timer timerDetect;
        private ToolStripMenuItem testToolStripMenuItem;
        private ToolStripMenuItem autoSearchToolStripMenuItem;
        private System.Windows.Forms.Timer timerMaintance;
        private ToolStripMenuItem 测试二维码ToolStripMenuItem;
        private ToolStripMenuItem 测试摄像头二维码ToolStripMenuItem;
        private ToolStripMenuItem 自动识别ToolStripMenuItem;
        private ToolStripMenuItem 短信测试ToolStripMenuItem;
        private PictureBox pictureBox1;
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


        public void UpdateCameraBitmap()
        {
            this.UpdateCameraBitmap(false);
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
            this.initSMS();
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

        private Bitmap GenerateColorKeyBitmap(bool useAntiAlias,bool abForceBitmap)
        {
            int width = this.cameraControl.OutputVideoSize.Width;
            int height = this.cameraControl.OutputVideoSize.Height;
            int lnTextHeight = 70;
            if ((width <= 0) || (height <= 0))
            {
                if(!abForceBitmap)
                {
                    return null;
                }
                 width = this.cameraControl.Width;
                 height = this.cameraControl.Height;
                if ((width <= 0) || (height <= 0))
                {
                    return null;
                }
                lnTextHeight = 120;
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
            Font font2 = new Font("Tahoma", 9f);
            Brush brush2 = new SolidBrush(Color.BlueViolet);
            string text = this.textBoxZiMu.Text;
            RectangleF rectF1 = new RectangleF(4f, (float)(height - lnTextHeight), width-4f, (float)(lnTextHeight-10));
            graphics.DrawString(text, font2, brush2, rectF1);
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
            this.测试二维码ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.测试摄像头二维码ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.短信测试ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.自动识别ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.cameraControl = new Camera_NET.CameraControl();
            this.buttonUnZoom = new System.Windows.Forms.Button();
            this.timerDetect = new System.Windows.Forms.Timer(this.components);
            this.timerMaintance = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScreenshot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
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
            this.autoSearchToolStripMenuItem,
            this.测试二维码ToolStripMenuItem,
            this.测试摄像头二维码ToolStripMenuItem,
            this.短信测试ToolStripMenuItem,
            this.自动识别ToolStripMenuItem});
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.testToolStripMenuItem.Text = "Test";
            // 
            // autoSearchToolStripMenuItem
            // 
            this.autoSearchToolStripMenuItem.Enabled = false;
            this.autoSearchToolStripMenuItem.Name = "autoSearchToolStripMenuItem";
            this.autoSearchToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.autoSearchToolStripMenuItem.Text = "Auto Search";
            this.autoSearchToolStripMenuItem.Click += new System.EventHandler(this.autoSearchToolStripMenuItem_Click);
            // 
            // 测试二维码ToolStripMenuItem
            // 
            this.测试二维码ToolStripMenuItem.Name = "测试二维码ToolStripMenuItem";
            this.测试二维码ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.测试二维码ToolStripMenuItem.Text = "测试文件二维码";
            this.测试二维码ToolStripMenuItem.Click += new System.EventHandler(this.测试二维码ToolStripMenuItem_Click);
            // 
            // 测试摄像头二维码ToolStripMenuItem
            // 
            this.测试摄像头二维码ToolStripMenuItem.Name = "测试摄像头二维码ToolStripMenuItem";
            this.测试摄像头二维码ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.测试摄像头二维码ToolStripMenuItem.Text = "测试摄像头二维码";
            this.测试摄像头二维码ToolStripMenuItem.Click += new System.EventHandler(this.测试摄像头二维码ToolStripMenuItem_Click);
            // 
            // 短信测试ToolStripMenuItem
            // 
            this.短信测试ToolStripMenuItem.Name = "短信测试ToolStripMenuItem";
            this.短信测试ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.短信测试ToolStripMenuItem.Text = "短信测试";
            this.短信测试ToolStripMenuItem.Click += new System.EventHandler(this.短信测试ToolStripMenuItem_Click);
            // 
            // 自动识别ToolStripMenuItem
            // 
            this.自动识别ToolStripMenuItem.Checked = true;
            this.自动识别ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.自动识别ToolStripMenuItem.Name = "自动识别ToolStripMenuItem";
            this.自动识别ToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.自动识别ToolStripMenuItem.Text = "自动识别";
            this.自动识别ToolStripMenuItem.Click += new System.EventHandler(this.自动识别ToolStripMenuItem_Click);
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
            this.splitContainer1.Panel2.Controls.Add(this.pictureBox1);
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
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(336, 328);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(308, 181);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
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
            this.buttonUnZoom.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
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
            // timerDetect
            // 
            this.timerDetect.Enabled = true;
            this.timerDetect.Interval = 500;
            this.timerDetect.Tick += new System.EventHandler(this.timerDetect_Tick);
            // 
            // timerMaintance
            // 
            this.timerMaintance.Enabled = true;
            this.timerMaintance.Interval = 1000;
            this.timerMaintance.Tick += new System.EventHandler(this.timerMaintanace_Tick);
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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
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
            this.UpdateCameraBitmap(false);
            this.UpdateUnzoomButton();
            this._bDrawMouseSelection = false;
        }

        private void UpdateCameraBitmap(bool updateifCameraError)
        {
            Bitmap lpBitMap = this.GenerateColorKeyBitmap(false, false);
            if(updateifCameraError && lpBitMap == null)
            {
                lpBitMap = this.GenerateColorKeyBitmap(false, true);
                this.pictureBox1.Visible = true;
                this.pictureBox1.Dock = DockStyle.Fill;
                this.pictureBox1.Image = lpBitMap;

            }
            else if (this.cameraControl.MixerEnabled)
            {
                this.cameraControl.OverlayBitmap = lpBitMap;
                this.pictureBox1.Visible = false;
            }else
            {
                this.pictureBox1.Visible = false;
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



        public Boolean DetectByTencent(Bitmap apBitMap, ref String astrRet)
        {
            Boolean lbRet = false ;
            try
            {
                using(Bitmap lpBitMap = (Bitmap)apBitMap.Clone())
                {

                    Credential cred = new Credential
                    {
                        SecretId = "AKID6ynLdft8U0FF0IIFMCmAG6h1m13fBAZh",
                        SecretKey = "0jSvkIakeZEfroUvtb3EGk00jDRO5d2j"
                    };

                    ClientProfile clientProfile = new ClientProfile();
                    HttpProfile httpProfile = new HttpProfile();
                    httpProfile.Endpoint = ("ocr.tencentcloudapi.com");
                    clientProfile.HttpProfile = httpProfile;

                    System.IO.MemoryStream ms = new MemoryStream();
                    lpBitMap.Save(ms, ImageFormat.Jpeg);
                    byte[] byteImage = ms.ToArray();
                    var SigBase64 = Convert.ToBase64String(byteImage);

                    OcrClient client = new OcrClient(cred, "ap-beijing", clientProfile);
                    QrcodeOCRRequest req = new QrcodeOCRRequest();
                    string strParams = "{\"ImageBase64\":\""+ SigBase64 + "\"}";
                    req = QrcodeOCRRequest.FromJsonString<QrcodeOCRRequest>(strParams);
                    QrcodeOCRResponse resp = client.QrcodeOCRSync(req);
                    String lstrJson = AbstractModel.ToJsonString(resp);
                    JObject json = JObject.Parse(lstrJson);
                    astrRet = (string)json["CodeResults"][0]["Url"];
                    Console.WriteLine(astrRet);
                    if(!String.IsNullOrWhiteSpace(astrRet))
                    {
                        lbRet = true;
                    }
                   
                }
                
            }
            catch (Exception e)
            {
                ThreadUiController.Fatal(e);
            }

            return lbRet;
        }

        private Boolean DetectByRemoteServer(Bitmap apBitMap,ref String astrRet)
        {
            Boolean lbRet = false;
            try
            {
                lbRet =  DetectByTencent(apBitMap,ref astrRet);
            }catch(Exception e)
            {
                ThreadUiController.Fatal(e);
            }

            return lbRet;
        }


        private Boolean DetectPatch(Boolean abDetectRemote,out String astrResult)
        {
            Boolean lbEncodeSuccessfully = false;
            astrResult = "";
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
                                String lstrRet = "";
                                lbEncodeSuccessfully = DetectByRemoteServer(bitmap,ref lstrRet);
                                if(lbEncodeSuccessfully)
                                {
                                    if (lstrRet.ToUpper().CompareTo("OK".ToUpper()) == 0)
                                    {
                                        this.MarkScanSucceed();
                                    }
                                    this.PushData(lstrRet);
                                    this.AutoSearch = 2;
                                    astrResult = lstrRet;
                                }

                            }


                        }

                        if(!lbEncodeSuccessfully)
                        {
                            try
                            {
                                bitmap.Save("finally_error_temp.jpg");
                            }
                            catch (Exception e)
                            {
                                ThreadUiController.Fatal(e);
                            }
                        }
                    }

                }
                catch (Exception exception1)
                {
                    ThreadUiController.log(exception1.Message, ThreadUiController.LOG_LEVEL.FATAL);
                    // MessageBox.Show(exception1.Message, "Error while getting a snapshot");
                }

                if (!lbEncodeSuccessfully)
                {
                    try
                    {
                        this.textBoxZiMu.Text = "";
                        this.UpdateCameraBitmap(false);
                    }
                    catch (Exception ex)
                    {

                    }

                   

                }

            }

            return lbEncodeSuccessfully;
        }

        long m_lErrorCount = 0;
        
        private void timerDetect_Tick(object sender, EventArgs e)
        {
            Boolean lbEncodeSuccessfully = false;

            Boolean lbShouldUpdateWindows = false;
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
                                        if (result.Text.ToUpper().CompareTo("OK".ToUpper()) == 0)
                                        {
                                            this.MarkScanSucceed();
                                        }
                                        this.PushData(result.Text);
                                    }
                                    this.UpdateCameraBitmap(false);
                                    lbEncodeSuccessfully = true;
                                    this.AutoSearch = 2;
                                }
                            }


                        }

                        if(!lbEncodeSuccessfully)
                        {
                            if((DateTimeOffset.Now.ToUnixTimeMilliseconds()- this.m_lErrorCount)>1000)
                            {
                                bitmap.Save("error_temp.jpg");
                                this.m_lErrorCount = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                                lbShouldUpdateWindows = true;
                            }                          
                           
                        }
                    }

                }
                catch (Exception exception1)
                {
                    //ThreadUiController.log(exception1.Message, ThreadUiController.LOG_LEVEL.FATAL);
                    // MessageBox.Show(exception1.Message, "Error while getting a snapshot");
                }

                if (!lbEncodeSuccessfully)
                {
                    try
                    {
                        this.textBoxZiMu.Text = "";
                       if(lbShouldUpdateWindows)
                        {
                            this.UpdateCameraBitmap(false);
                        }
                       
                    }
                    catch (Exception ex)
                    {

                    }

                }

            }
        }
        String m_strRemoteServerUrl = "http://192.168.122.97:8180/?";

        int m_nIsEnableMessageFilter = 0;
        public int IsEnableMessageFilter
        {
            get { lock (this) return m_nIsEnableMessageFilter; }
            set { lock (this) m_nIsEnableMessageFilter = value; }
        }
        public System.String RemoteServerUrl
        {
            get { lock (this) return m_strRemoteServerUrl; }
            set { lock (this) m_strRemoteServerUrl = value; }
        }
        List<String> m_Datas = new List<string>();
        public System.Collections.Generic.List<System.String> Datas
        {
            get { lock(this) return m_Datas; }
            set { lock (this) m_Datas = value; }
        }
        int m_nDataQueueSize = 1024*100;
        public int DataQueueSize
        {
            get { lock (this) return m_nDataQueueSize; }
            set { if (value <= 0) { return; } lock (this) m_nDataQueueSize = value; }
        }
        public void PushData(String astrData)
        {
            lock (this)
            {
                if (Datas.Count > this.DataQueueSize)
                {
                    this.Datas.RemoveAt(0);
                }
                
                {
                    this.Datas.Add(astrData);
                }

                this.saveDataToLocalStorage();
            }
        }

        public void loadDataFromLocalStorage()
        {
            lock (this)
            {
                try
                {
                    string json = File.ReadAllText(AlarmDataFileName);
                    List<String> lpData =  (List<string>)JsonConvert.DeserializeObject(json);
                    if(lpData!=null)
                    {
                        this.Datas = lpData;
                    }

                }
                catch (Exception ex)
                {
                    ThreadUiController.Fatal(ex.Message);
                }
            }
        }

        String m_strAlarmDataFileName = "./alarm_data.json";
        public System.String AlarmDataFileName
        {
            get { return m_strAlarmDataFileName; }
            set { m_strAlarmDataFileName = value; }
        }
        public void saveDataToLocalStorage()
        {
            lock(this)
            {
                try
                {
                    string json = JsonConvert.SerializeObject(this.Datas.ToArray());
                    File.WriteAllText(AlarmDataFileName, JsonConvert.SerializeObject(json));
                }
                catch (Exception ex)
                {
                    ThreadUiController.log(ex.Message, ThreadUiController.LOG_LEVEL.FATAL);
                }
            }

        }

        public String GetData()
        {
            lock (this)
            {
                if (this.Datas.Count > 0)
                {
                    String lstrData = this.Datas[0];
                    this.Datas.RemoveAt(0);
                    return lstrData;
                }
            }

            this.saveDataToLocalStorage();
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

        String m_strMaintainacePhone = "18600677886";
        public System.String MaintainacePhone
        {
            get { lock(this)return m_strMaintainacePhone; }
            set { lock(this)m_strMaintainacePhone = value; }
        }

        String m_strSMSTag = "";
        public System.String SMSTag
        {
            get { lock (this) return m_strSMSTag; }
            set { lock (this) m_strSMSTag = value; }
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
                this.SMSTag = this.SettingsFile.IniReadStringValue("setting", "maintanace_sms_tag", this.SMSTag, true);
                this.IsEnableMessageFilter = this.SettingsFile.IniReadIntValue("setting", "enable_message_filter", 0, true);
            }

            this.loadDataFromLocalStorage();

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

        public Boolean ShouldPostData(String astrata)
        {
            if(IsEnableMessageFilter<=0)
            {
                return true;
            }
            Boolean lbShouldPost = true;
            try
            {
                //1.check if vib point
                astrata = astrata.ToLower();
                int lnIndex = astrata.IndexOf("um");
                Boolean lbIsVibPoint = false;
                if(lnIndex>=0)
                {
                    lbIsVibPoint = true;
                }
                else
                {
                    lnIndex = astrata.IndexOf("mil");
                    if (lnIndex >= 0)
                    {
                        lbIsVibPoint = true;
                    }else
                    {
                        lnIndex = astrata.IndexOf("mm");
                        if (lnIndex >= 0)
                        {
                            lbIsVibPoint = true;
                        }
                    }    
                }


                //2.check if sud message
                Newtonsoft.Json.Linq.JArray array = null;
                Boolean lbIsSudMessage = false;
                try
                {
                    array = Newtonsoft.Json.Linq.JArray.Parse(astrata);
                    
                }
                catch (Exception e)
                {
                    ThreadUiController.log("Parsing data Error:" + astrata, ThreadUiController.LOG_LEVEL.FATAL);
                    ThreadUiController.log(e.Message, ThreadUiController.LOG_LEVEL.FATAL);                   
                }

                foreach (JObject obj in array.Children<JObject>())
                {
                    foreach (JProperty property in obj.Properties())
                    {
                        string name = property.Name;
                        string value = property.Value.ToString();

                        if (property.Name == "alarmstatus")
                        {
                            try
                            {
                                int lnAlarmStatus = int.Parse(value);
                                if(lnAlarmStatus ==9 || lnAlarmStatus == 10)
                                {
                                    lbIsSudMessage = true;
                                }
                            }catch(Exception e)
                            {
                                ThreadUiController.Error(e.Message);
                            }
                            break;
                        }
                      
                    }
                }

                //3.check if Regular alarm message
                Boolean lbIsRegularAlarmMessage = false;
                double ldblHH = -1000.0;
                double ldblHL = -1000.0;
                double ldblCurrentValue = -1100.0;
                try
                {
                    array = Newtonsoft.Json.Linq.JArray.Parse(astrata);

                }
                catch (Exception e)
                {
                    ThreadUiController.log("Parsing data Error:" + astrata, ThreadUiController.LOG_LEVEL.FATAL);
                    ThreadUiController.log(e.Message, ThreadUiController.LOG_LEVEL.FATAL);
                }

                foreach (JObject obj in array.Children<JObject>())
                {
                    foreach (JProperty property in obj.Properties())
                    {
                        string name = property.Name;
                        string value = property.Value.ToString();

                        if (property.Name == "hh")
                        {
                            ldblHH = double.Parse(value);
                        }else if (property.Name == "hl")
                        {
                            ldblHL = double.Parse(value);
                        }else if (property.Name == "val")
                        {
                            ldblCurrentValue = double.Parse(value);
                        }
                    }
                }

                if((ldblCurrentValue>ldblHH && ldblHH>0) || (ldblCurrentValue>ldblHL&&ldblHL>0))
                {
                    lbIsRegularAlarmMessage = true;
                }

                //4. is vib and regular alarm message  Or is sud message should be sent
                if((lbIsRegularAlarmMessage && lbIsVibPoint) || (lbIsSudMessage))
                {
                    lbShouldPost = true;
                }else
                {
                    lbShouldPost = false;
                }
            }
            catch(Exception e)
            {
                lbShouldPost = true;
                ThreadUiController.Error(e.Message);
            }

            return lbShouldPost;
        }

        public Boolean PostDataToRemoteServer2(String astrata,String astrUrl)
        {
            Boolean lbSucceed = false;
            try
            {
                if(!ShouldPostData(astrata))
                {
                    return true;
                }
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
        void initSMS()
        {
            try
            {
                //产品名称:云通信短信API产品,开发者无需替换
                const String product = "Dysmsapi";
                //产品域名,开发者无需替换
                const String domain = "dysmsapi.aliyuncs.com";

                DefaultProfile.AddEndpoint("cn-hangzhou",
                    "cn-hangzhou",
                    product,
                    domain);
            }
            catch(Exception ex)
            {
                ThreadUiController.log(ex.Message, ThreadUiController.LOG_LEVEL.FATAL);
            }
        }
        public void ThreadSendToRemoteServer()
        {
            while (true)
            {
                Boolean lbSucceed = false;
                Boolean lbShouldPushBack = false;
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


                            lbShouldPushBack = true;

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
                        if(lbShouldPushBack)
                        {
                            this.PushData(lstrData);
                        }
                        
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


        const String accessKeyId = "LTAIZofTLpflOxi3";
        const String accessKeySecret = "Mrrr2lCy2HUXFcR9nBr6fTBhlWBLlb";

        private Boolean  NotifyMaintanaceStaff(String astrBody)
        {
            try
            {
               
                if ( !(String.IsNullOrWhiteSpace(this.MaintainacePhone)))
                {

                    IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", 
                        accessKeyId, 
                        accessKeySecret);


                    IAcsClient acsClient = new DefaultAcsClient(profile);
                    SendSmsRequest request = new SendSmsRequest();
                    SendSmsResponse response = null;

                    //必填:待发送手机号。支持以逗号分隔的形式进行批量调用，批量上限为1000个手机号码,批量调用相对于单条调用及时性稍有延迟,验证码类型的短信推荐使用单条调用的方式
                    request.PhoneNumbers = this.MaintainacePhone;
                    //必填:短信签名-可在短信控制台中找到
                    request.SignName = "博华科技";
                    //必填:短信模板-可在短信控制台中找到
                    request.TemplateCode = "SMS_150570828";
                    //可选:模板中的变量替换JSON串,如模板内容为"亲爱的${name},您的验证码为${code}"时,此处的值为

                    String lstrTemp = String.Format("\"company\":\"{0}\"," +
                        "\"factory\":\"{1}\",\"set\":\"{2}\"," +
                        "\"plant\":\"{3}\",\"time\":\"{4}\"," +
                        "\"detail\":\"{5}\",\"channel\":\"{6}\"," +
                        "\"value\":\"{7}\"",
                                                     "博华科技", 
                                                     this.SMSTag, 
                                                     "", 
                                                     "二维码识别", 
                                                     DateTime.Now.ToString(), 
                                                     1, 
                                                    // "二维码超过时间没有结果", 
                                                     astrBody,
                                                     this.TimeSecondsThreshold+"秒");
                    String lstrTemp1 = "{" + lstrTemp + "}";
                    request.TemplateParam = lstrTemp1;//"{\"plant\":\" 大连石化-三催化-P1001A\",\"time\":\"2018年2月9日14:35:25\",\"detail\":\"数据中断\",\"channel\":\"1H\"}";
                                                      //可选:outId为提供给业务方扩展字段,最终在短信回执消息中将此值带回给调用者
                                                      //request.OutId = "yourOutId";
                                                      //请求失败这里会抛ClientException异常
                    response = acsClient.GetAcsResponse(request);

                    if (response.Code.Contains("OK"))
                    {
                        String lstrLog = String.Format("发送短信成功:{0}", lstrTemp);
                        ThreadUiController.
                            log(lstrLog,ThreadUiController.LOG_LEVEL.INFO);
                        return true;
                    }
                    else
                    {
                        String lstrLog = String.Format("发送短信失败:{0},{1}", response.Code, lstrTemp);
                        ThreadUiController.log(lstrLog, ThreadUiController.LOG_LEVEL.INFO);
                    }                  
                }
            }
            catch(Exception ex)
            {
                ThreadUiController.log(ex.Message, ThreadUiController.LOG_LEVEL.FATAL);
            }

            return false;

        }

        private void timerMaintanace_Tick(object sender, EventArgs e)
        {
           TimeSpan lpOffset = DateTime.Now - this.TimeLastSucceed;
            if(lpOffset.TotalSeconds>this.TimeSecondsThreshold)
            {
                try
                {
                    String lstrResult = "";
                    Boolean lbRest = this.DetectPatch(true,out lstrResult);
                    ThreadUiController.log("远程二维码识别结果:" + lstrResult, ThreadUiController.LOG_LEVEL.FATAL);
                    if (lbRest)
                    {
                        this.NotifyMaintanaceStaff("远程二维码识别成功");
                        ThreadUiController.log("远程二维码识别成功:"+lstrResult, ThreadUiController.LOG_LEVEL.FATAL);
                        this.TimeLastSucceed = DateTime.Now;
                    }
                    else if(this.NotifyMaintanaceStaff("二维码识别失败"))
                    {
                        ThreadUiController.log("二维码识别失败", ThreadUiController.LOG_LEVEL.FATAL);
                        this.TimeLastSucceed = DateTime.Now;
                    }
                }catch(Exception ex)
                {
                    ThreadUiController.log(ex.Message, ThreadUiController.LOG_LEVEL.FATAL);
                }
            }
        }

        private void 测试二维码ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var bitmap = new Bitmap(@"test.jpg");
            String lstrRest = "";           
            Boolean lbRet = DetectByRemoteServer(bitmap,ref lstrRest);
            Debug.WriteLine(lstrRest);
            if (String.IsNullOrEmpty(lstrRest))
            {
                this.textBoxZiMu.Text = "文件二维码远程测试失败";
            }
            else
            {
                this.textBoxZiMu.Text = lstrRest;
            }
            this.UpdateCameraBitmap(true);
        }

        private void 测试摄像头二维码ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String lstrResult = "";
            Boolean lbRest = this.DetectPatch(true, out lstrResult);
            if (String.IsNullOrEmpty(lstrResult))
            {
                this.textBoxZiMu.Text = "摄像头二维码远程测试失败";
            }
            else
            {
                this.textBoxZiMu.Text = lstrResult;
            }
            this.UpdateCameraBitmap(false);
        }

        private void 自动识别ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.自动识别ToolStripMenuItem.Checked = !this.自动识别ToolStripMenuItem.Checked;
            this.timerDetect.Enabled = this.自动识别ToolStripMenuItem.Checked;
        }

        private void 短信测试ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.NotifyMaintanaceStaff("短信测试");
        }
    }
}

