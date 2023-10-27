namespace Filemaneger
{
    partial class Register
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
            Username = new TextBox();
            Label2 = new Label();
            Label3 = new Label();
            Psw = new TextBox();
            Psw2 = new TextBox();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(126, 197);
            button1.Margin = new Padding(2, 2, 2, 2);
            button1.Name = "button1";
            button1.Size = new Size(71, 24);
            button1.TabIndex = 0;
            button1.Text = "确认注册";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(68, 42);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(44, 17);
            label1.TabIndex = 1;
            label1.Text = "用户名";
            // 
            // Username
            // 
            Username.Location = new Point(126, 40);
            Username.Margin = new Padding(2, 2, 2, 2);
            Username.Name = "Username";
            Username.Size = new Size(97, 23);
            Username.TabIndex = 2;
            // 
            // Label2
            // 
            Label2.AutoSize = true;
            Label2.Location = new Point(78, 91);
            Label2.Margin = new Padding(2, 0, 2, 0);
            Label2.Name = "Label2";
            Label2.Size = new Size(32, 17);
            Label2.TabIndex = 3;
            Label2.Text = "密码";
            // 
            // Label3
            // 
            Label3.AutoSize = true;
            Label3.Location = new Point(57, 136);
            Label3.Margin = new Padding(2, 0, 2, 0);
            Label3.Name = "Label3";
            Label3.Size = new Size(56, 17);
            Label3.TabIndex = 4;
            Label3.Text = "确认密码";
            // 
            // Psw
            // 
            Psw.Location = new Point(126, 89);
            Psw.Margin = new Padding(2, 2, 2, 2);
            Psw.Name = "Psw";
            Psw.PasswordChar = '*';
            Psw.Size = new Size(97, 23);
            Psw.TabIndex = 5;
            // 
            // Psw2
            // 
            Psw2.Location = new Point(126, 134);
            Psw2.Margin = new Padding(2, 2, 2, 2);
            Psw2.Name = "Psw2";
            Psw2.PasswordChar = '*';
            Psw2.Size = new Size(97, 23);
            Psw2.TabIndex = 6;
            // 
            // Register
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(318, 256);
            Controls.Add(Psw2);
            Controls.Add(Psw);
            Controls.Add(Label3);
            Controls.Add(Label2);
            Controls.Add(Username);
            Controls.Add(label1);
            Controls.Add(button1);
            Margin = new Padding(2, 2, 2, 2);
            Name = "Register";
            Text = "注册";
            FormClosed += Register_FormClosed;
            Load += Register_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Label label1;
        private TextBox Username;
        private Label Label2;
        private Label Label3;
        private TextBox Psw;
        private TextBox Psw2;
    }
}