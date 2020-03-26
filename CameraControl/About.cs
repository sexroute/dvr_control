namespace CameraControl
{
    using CameraControl.Properties;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;

    public class About : Form
    {
        private Button btnCaiFuTong;
        private Button btnZhiFuBao;
        private IContainer components;
        private ContextMenuStrip contextMenuStrip1;
        private ContextMenuStrip contextMenuStrip2;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private RichTextBox richTextBox1;
        private RichTextBox richTextBox2;
        private RichTextBox richTextBox3;
        private RichTextBox richTextBox4;
        private ToolStripMenuItem 复制ToolStripMenuItem;
        private ToolStripMenuItem 复制ToolStripMenuItem1;

        public About()
        {
            this.InitializeComponent();
        }

        private void About_Load(object sender, EventArgs e)
        {
            DateTime time2 = DateTime.Now.AddMonths(-3);
            string str = string.Format("{0:Y}", time2) + " 写于杭州";
            this.pictureBox1.Image = Resources.haiou;
            this.pictureBox2.Image = Resources.WeiXinPay;
            this.pictureBox3.Image = Resources.AliPay;
            this.richTextBox1.Text = "本软件的最终解释权归软件作者所有。\r\n为了能够继续开发更多免费易用的软件，同时也为了生存和发展，\r\n该软件在使用过程中可能会出现广告页面，敬请谅解，可不是病毒哦。\r\n若软件对您有帮助，您可以通过捐助方式支持我，5元、10元都是爱！\r\n支付宝账号：bandariz@163.com\r\n财付通账号：252505845\r\n捐助的朋友可以获得该软件的个性化定制的多功能版本！\r\n(注：个性化定制包括定制软件名称、标题、界面皮肤、增加新功能等。)";
            this.richTextBox2.Text = "如果您有任何意见（建议）或者您需要开发定制软件，欢迎与我联系！\r\n  QQ：252505845\r\n邮箱：252505845@qq.com";
            this.richTextBox3.Text = "您也可以直接用手机上的微信或者支付宝扫描下面的二维码进行捐助:-)";
            this.richTextBox4.Text = str;
        }

        private void btnCaiFuTong_Click(object sender, EventArgs e)
        {
            Clipboard.SetText("252505845");
            MessageBox.Show("252505845复制成功，请打开财付通页面！", "感谢您的支持", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            Process.Start("www.tenpay.com");
        }

        private void btnZhiFuBao_Click(object sender, EventArgs e)
        {
            Clipboard.SetText("bandariz@163.com");
            MessageBox.Show("bandariz@163.com复制成功，请打开支付宝页面！", "感谢您的支持", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            Process.Start("www.alipay.com");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            ComponentResourceManager manager = new ComponentResourceManager(typeof(About));
            this.richTextBox1 = new RichTextBox();
            this.contextMenuStrip1 = new ContextMenuStrip(this.components);
            this.复制ToolStripMenuItem = new ToolStripMenuItem();
            this.label1 = new Label();
            this.richTextBox2 = new RichTextBox();
            this.contextMenuStrip2 = new ContextMenuStrip(this.components);
            this.复制ToolStripMenuItem1 = new ToolStripMenuItem();
            this.label2 = new Label();
            this.btnZhiFuBao = new Button();
            this.btnCaiFuTong = new Button();
            this.richTextBox4 = new RichTextBox();
            this.label3 = new Label();
            this.richTextBox3 = new RichTextBox();
            this.pictureBox3 = new PictureBox();
            this.pictureBox2 = new PictureBox();
            this.pictureBox1 = new PictureBox();
            this.label4 = new Label();
            this.label5 = new Label();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            ((ISupportInitialize) this.pictureBox3).BeginInit();
            ((ISupportInitialize) this.pictureBox2).BeginInit();
            ((ISupportInitialize) this.pictureBox1).BeginInit();
            base.SuspendLayout();
            this.richTextBox1.BackColor = SystemColors.ButtonHighlight;
            this.richTextBox1.BorderStyle = BorderStyle.None;
            this.richTextBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.richTextBox1.Font = new Font("宋体", 9f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.richTextBox1.Location = new Point(12, 80);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new Size(0x18d, 0x85);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            ToolStripItem[] toolStripItems = new ToolStripItem[] { this.复制ToolStripMenuItem };
            this.contextMenuStrip1.Items.AddRange(toolStripItems);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new Size(0x65, 0x1a);
            this.复制ToolStripMenuItem.Name = "复制ToolStripMenuItem";
            this.复制ToolStripMenuItem.Size = new Size(100, 0x16);
            this.复制ToolStripMenuItem.Text = "复制";
            this.复制ToolStripMenuItem.Click += new EventHandler(this.复制ToolStripMenuItem_Click);
            this.label1.AutoSize = true;
            this.label1.Font = new Font("宋体", 9f, FontStyle.Underline, GraphicsUnit.Point, 0x86);
            this.label1.ForeColor = Color.Teal;
            this.label1.Location = new Point(10, 0x16e);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x191, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "                                                                  ";
            this.richTextBox2.BackColor = SystemColors.ButtonHighlight;
            this.richTextBox2.BorderStyle = BorderStyle.None;
            this.richTextBox2.ContextMenuStrip = this.contextMenuStrip2;
            this.richTextBox2.Font = new Font("宋体", 9f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.richTextBox2.Location = new Point(12, 0x17d);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.ReadOnly = true;
            this.richTextBox2.Size = new Size(0x18d, 0x36);
            this.richTextBox2.TabIndex = 3;
            this.richTextBox2.Text = "";
            ToolStripItem[] itemArray2 = new ToolStripItem[] { this.复制ToolStripMenuItem1 };
            this.contextMenuStrip2.Items.AddRange(itemArray2);
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new Size(0x65, 0x1a);
            this.复制ToolStripMenuItem1.Name = "复制ToolStripMenuItem1";
            this.复制ToolStripMenuItem1.Size = new Size(100, 0x16);
            this.复制ToolStripMenuItem1.Text = "复制";
            this.复制ToolStripMenuItem1.Click += new EventHandler(this.复制ToolStripMenuItem1_Click);
            this.label2.AutoSize = true;
            this.label2.Font = new Font("宋体", 9f, FontStyle.Underline, GraphicsUnit.Point, 0x86);
            this.label2.ForeColor = Color.Teal;
            this.label2.Location = new Point(10, 0x1ab);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0x191, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "                                                                  ";
            this.btnZhiFuBao.Location = new Point(0xcc, 0x8e);
            this.btnZhiFuBao.Name = "btnZhiFuBao";
            this.btnZhiFuBao.Size = new Size(0x33, 0x13);
            this.btnZhiFuBao.TabIndex = 0;
            this.btnZhiFuBao.Text = "复制";
            this.btnZhiFuBao.UseVisualStyleBackColor = true;
            this.btnZhiFuBao.Click += new EventHandler(this.btnZhiFuBao_Click);
            this.btnCaiFuTong.Location = new Point(0xcc, 0x9f);
            this.btnCaiFuTong.Name = "btnCaiFuTong";
            this.btnCaiFuTong.Size = new Size(0x33, 0x13);
            this.btnCaiFuTong.TabIndex = 1;
            this.btnCaiFuTong.Text = "复制";
            this.btnCaiFuTong.UseVisualStyleBackColor = true;
            this.btnCaiFuTong.Click += new EventHandler(this.btnCaiFuTong_Click);
            this.richTextBox4.BackColor = SystemColors.ButtonHighlight;
            this.richTextBox4.BorderStyle = BorderStyle.None;
            this.richTextBox4.Font = new Font("宋体", 9f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.richTextBox4.Location = new Point(0x11c, 0x1ba);
            this.richTextBox4.Name = "richTextBox4";
            this.richTextBox4.ReadOnly = true;
            this.richTextBox4.Size = new Size(120, 20);
            this.richTextBox4.TabIndex = 8;
            this.richTextBox4.Text = "";
            this.label3.AutoSize = true;
            this.label3.Font = new Font("宋体", 9f, FontStyle.Underline, GraphicsUnit.Point, 0x86);
            this.label3.ForeColor = Color.Teal;
            this.label3.Location = new Point(9, 0xce);
            this.label3.Name = "label3";
            this.label3.Size = new Size(0x191, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "                                                                  ";
            this.richTextBox3.BackColor = SystemColors.ButtonHighlight;
            this.richTextBox3.BorderStyle = BorderStyle.None;
            this.richTextBox3.Font = new Font("宋体", 9f, FontStyle.Regular, GraphicsUnit.Point, 0x86);
            this.richTextBox3.Location = new Point(12, 0xdd);
            this.richTextBox3.Name = "richTextBox3";
            this.richTextBox3.ReadOnly = true;
            this.richTextBox3.Size = new Size(0x188, 20);
            this.richTextBox3.TabIndex = 10;
            this.richTextBox3.Text = "";
            this.pictureBox3.BackColor = SystemColors.ButtonHighlight;
            this.pictureBox3.InitialImage = (Image) manager.GetObject("pictureBox3.InitialImage");
            this.pictureBox3.Location = new Point(0x111, 0xf2);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new Size(0x86, 0x86);
            this.pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 12;
            this.pictureBox3.TabStop = false;
            this.pictureBox2.BackColor = SystemColors.ButtonHighlight;
            this.pictureBox2.InitialImage = (Image) manager.GetObject("pictureBox2.InitialImage");
            this.pictureBox2.Location = new Point(12, 0xf2);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new Size(0x86, 0x86);
            this.pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 11;
            this.pictureBox2.TabStop = false;
            this.pictureBox1.BackColor = SystemColors.ButtonHighlight;
            this.pictureBox1.InitialImage = (Image) manager.GetObject("pictureBox1.InitialImage");
            this.pictureBox1.Location = new Point(12, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Size(0x18d, 0x49);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.label4.AutoSize = true;
            this.label4.Font = new Font("宋体", 12f, FontStyle.Bold, GraphicsUnit.Point, 0x86);
            this.label4.ForeColor = Color.Red;
            this.label4.Location = new Point(0x93, 0x108);
            this.label4.Name = "label4";
            this.label4.Size = new Size(0x4c, 0x10);
            this.label4.TabIndex = 13;
            this.label4.Text = "微信支付";
            this.label5.AutoSize = true;
            this.label5.Font = new Font("宋体", 12f, FontStyle.Bold, GraphicsUnit.Point, 0x86);
            this.label5.ForeColor = Color.Red;
            this.label5.Location = new Point(0xb1, 0x159);
            this.label5.Name = "label5";
            this.label5.Size = new Size(0x5d, 0x10);
            this.label5.TabIndex = 14;
            this.label5.Text = "支付宝支付";
            base.AutoScaleDimensions = new SizeF(6f, 12f);
            base.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = SystemColors.ButtonHighlight;
            base.ClientSize = new Size(0x1a3, 0x1d0);
            base.Controls.Add(this.label5);
            base.Controls.Add(this.label4);
            base.Controls.Add(this.pictureBox3);
            base.Controls.Add(this.pictureBox2);
            base.Controls.Add(this.richTextBox3);
            base.Controls.Add(this.label3);
            base.Controls.Add(this.richTextBox4);
            base.Controls.Add(this.btnCaiFuTong);
            base.Controls.Add(this.btnZhiFuBao);
            base.Controls.Add(this.label2);
            base.Controls.Add(this.richTextBox2);
            base.Controls.Add(this.label1);
            base.Controls.Add(this.richTextBox1);
            base.Controls.Add(this.pictureBox1);
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.MaximizeBox = false;
            base.Name = "About";
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "关于";
            base.Load += new EventHandler(this.About_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            ((ISupportInitialize) this.pictureBox3).EndInit();
            ((ISupportInitialize) this.pictureBox2).EndInit();
            ((ISupportInitialize) this.pictureBox1).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.richTextBox1.SelectedText);
            MessageBox.Show("复制成功！", "感谢您的支持", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void 复制ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.richTextBox2.SelectedText);
            MessageBox.Show("复制成功！", "感谢您的支持", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }
    }
}

