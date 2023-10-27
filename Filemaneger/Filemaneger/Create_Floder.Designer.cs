namespace Filemaneger
{
    partial class Create_Floder
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button1 = new Button();
            label1 = new Label();
            Dir_name = new TextBox();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(112, 113);
            button1.Name = "button1";
            button1.Size = new Size(112, 34);
            button1.TabIndex = 0;
            button1.Text = "确认创建";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(43, 48);
            label1.Name = "label1";
            label1.Size = new Size(82, 24);
            label1.TabIndex = 1;
            label1.Text = "文件夹名";
            // 
            // Dir_name
            // 
            Dir_name.Location = new Point(129, 45);
            Dir_name.Name = "Dir_name";
            Dir_name.Size = new Size(150, 30);
            Dir_name.TabIndex = 2;
            // 
            // Create_Floder
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(348, 196);
            Controls.Add(Dir_name);
            Controls.Add(label1);
            Controls.Add(button1);
            Name = "Create_Floder";
            Text = "新建文件夹";
            Load += Create_Floder_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Label label1;
        private TextBox Dir_name;
    }
}