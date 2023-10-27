namespace Filemaneger
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            Filelist = new ListView();
            columnHeader1 = new ColumnHeader("(无)");
            columnHeader3 = new ColumnHeader();
            imageList1 = new ImageList(components);
            button_Upload = new Button();
            button_Download = new Button();
            button3 = new Button();
            button_Ping = new Button();
            Fresh = new Button();
            Trans_list = new ListView();
            文件名称 = new ColumnHeader();
            任务类型 = new ColumnHeader();
            任务进度 = new ColumnHeader();
            button_pause_down = new Button();
            label1 = new Label();
            button_rename = new Button();
            button2_delete = new Button();
            button_logout = new Button();
            SuspendLayout();
            // 
            // Filelist
            // 
            Filelist.Activation = ItemActivation.OneClick;
            Filelist.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader3 });
            Filelist.Location = new Point(133, 28);
            Filelist.Margin = new Padding(2);
            Filelist.MultiSelect = false;
            Filelist.Name = "Filelist";
            Filelist.Size = new Size(495, 193);
            Filelist.SmallImageList = imageList1;
            Filelist.TabIndex = 0;
            Filelist.UseCompatibleStateImageBehavior = false;
            Filelist.View = View.Details;
            Filelist.SelectedIndexChanged += listView1_SelectedIndexChanged;
            Filelist.DoubleClick += Filelist_DoubleClick;
            // 
            // columnHeader1
            // 
            columnHeader1.Tag = "";
            columnHeader1.Text = "名称";
            columnHeader1.Width = 400;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "类型";
            columnHeader3.Width = 200;
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth8Bit;
            imageList1.ImageStream = (ImageListStreamer)resources.GetObject("imageList1.ImageStream");
            imageList1.TransparentColor = Color.Transparent;
            imageList1.Images.SetKeyName(0, "floder.png");
            // 
            // button_Upload
            // 
            button_Upload.Location = new Point(29, 246);
            button_Upload.Margin = new Padding(2);
            button_Upload.Name = "button_Upload";
            button_Upload.Size = new Size(71, 24);
            button_Upload.TabIndex = 1;
            button_Upload.Text = "上传文件";
            button_Upload.UseVisualStyleBackColor = true;
            button_Upload.Click += button_Upload_Click;
            // 
            // button_Download
            // 
            button_Download.Location = new Point(29, 299);
            button_Download.Margin = new Padding(2);
            button_Download.Name = "button_Download";
            button_Download.Size = new Size(71, 24);
            button_Download.TabIndex = 2;
            button_Download.Text = "下载文件";
            button_Download.UseVisualStyleBackColor = true;
            button_Download.Click += button_Download_Click;
            // 
            // button3
            // 
            button3.Location = new Point(25, 40);
            button3.Margin = new Padding(2);
            button3.Name = "button3";
            button3.Size = new Size(80, 24);
            button3.TabIndex = 3;
            button3.Text = "新建文件夹";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button_Ping
            // 
            button_Ping.Location = new Point(453, 0);
            button_Ping.Margin = new Padding(2);
            button_Ping.Name = "button_Ping";
            button_Ping.Size = new Size(71, 24);
            button_Ping.TabIndex = 4;
            button_Ping.Text = "Ping";
            button_Ping.UseVisualStyleBackColor = true;
            button_Ping.Visible = false;
            button_Ping.Click += button_Ping_Click;
            // 
            // Fresh
            // 
            Fresh.Location = new Point(369, 0);
            Fresh.Margin = new Padding(2);
            Fresh.Name = "Fresh";
            Fresh.Size = new Size(71, 24);
            Fresh.TabIndex = 5;
            Fresh.Text = "刷新";
            Fresh.UseVisualStyleBackColor = true;
            Fresh.Visible = false;
            Fresh.Click += Fresh_Click;
            // 
            // Trans_list
            // 
            Trans_list.Columns.AddRange(new ColumnHeader[] { 文件名称, 任务类型, 任务进度 });
            Trans_list.Location = new Point(133, 254);
            Trans_list.Margin = new Padding(2);
            Trans_list.Name = "Trans_list";
            Trans_list.Size = new Size(495, 137);
            Trans_list.TabIndex = 6;
            Trans_list.UseCompatibleStateImageBehavior = false;
            Trans_list.View = View.Details;
            Trans_list.SelectedIndexChanged += Trans_view_SelectedIndexChanged;
            // 
            // 文件名称
            // 
            文件名称.Text = "文件名称";
            文件名称.Width = 200;
            // 
            // 任务类型
            // 
            任务类型.Text = "任务类型";
            任务类型.Width = 150;
            // 
            // 任务进度
            // 
            任务进度.Text = "任务进度";
            任务进度.Width = 300;
            // 
            // button_pause_down
            // 
            button_pause_down.Location = new Point(29, 353);
            button_pause_down.Margin = new Padding(2);
            button_pause_down.Name = "button_pause_down";
            button_pause_down.Size = new Size(71, 24);
            button_pause_down.TabIndex = 7;
            button_pause_down.Text = "暂停传输";
            button_pause_down.UseVisualStyleBackColor = true;
            button_pause_down.Click += button_pause_down_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(133, 228);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(56, 17);
            label1.TabIndex = 8;
            label1.Text = "传输列表";
            label1.Click += label1_Click;
            // 
            // button_rename
            // 
            button_rename.Location = new Point(29, 98);
            button_rename.Margin = new Padding(2);
            button_rename.Name = "button_rename";
            button_rename.Size = new Size(71, 24);
            button_rename.TabIndex = 9;
            button_rename.Text = "重命名";
            button_rename.UseVisualStyleBackColor = true;
            button_rename.Click += button_rename_Click;
            // 
            // button2_delete
            // 
            button2_delete.Location = new Point(29, 152);
            button2_delete.Margin = new Padding(2);
            button2_delete.Name = "button2_delete";
            button2_delete.Size = new Size(71, 24);
            button2_delete.TabIndex = 10;
            button2_delete.Text = "删除文件";
            button2_delete.UseVisualStyleBackColor = true;
            button2_delete.Click += button2_delete_Click;
            // 
            // button_logout
            // 
            button_logout.Location = new Point(547, 1);
            button_logout.Name = "button_logout";
            button_logout.Size = new Size(75, 23);
            button_logout.TabIndex = 11;
            button_logout.Text = "退出";
            button_logout.UseVisualStyleBackColor = true;
            button_logout.Click += button_logout_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(634, 399);
            Controls.Add(button_logout);
            Controls.Add(button2_delete);
            Controls.Add(button_rename);
            Controls.Add(label1);
            Controls.Add(button_pause_down);
            Controls.Add(Trans_list);
            Controls.Add(Fresh);
            Controls.Add(button_Ping);
            Controls.Add(button3);
            Controls.Add(button_Download);
            Controls.Add(button_Upload);
            Controls.Add(Filelist);
            Margin = new Padding(2);
            Name = "Form1";
            Text = "文件传输系统";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListView Filelist;
        public ColumnHeader columnHeader1;
        private ColumnHeader columnHeader3;
        private ImageList imageList1;
        private Button button_Upload;
        private Button button_Download;
        private Button button3;
        private Button button_Ping;
        private Button Fresh;
        public ColumnHeader 文件名称;
        private ColumnHeader 任务类型;
        private ColumnHeader 任务进度;
        protected ListView Trans_list;
        private Button button_pause_down;
        private Label label1;
        private Button button_rename;
        private Button button2_delete;
        private Button button_logout;
    }
}