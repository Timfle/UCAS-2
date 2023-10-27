namespace Filemaneger
{
    partial class login
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
            login_1 = new Button();
            label1 = new Label();
            label2 = new Label();
            Id_input = new TextBox();
            Psw_input = new TextBox();
            Register = new Button();
            SuspendLayout();
            // 
            // login_1
            // 
            login_1.Location = new Point(186, 214);
            login_1.Margin = new Padding(2, 2, 2, 2);
            login_1.Name = "login_1";
            login_1.Size = new Size(71, 24);
            login_1.TabIndex = 0;
            login_1.Text = "登录";
            login_1.UseVisualStyleBackColor = true;
            login_1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(109, 91);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(44, 17);
            label1.TabIndex = 1;
            label1.Text = "用户名";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(109, 137);
            label2.Margin = new Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new Size(44, 17);
            label2.TabIndex = 2;
            label2.Text = "密   码";
            label2.Click += label2_Click;
            // 
            // Id_input
            // 
            Id_input.Location = new Point(186, 89);
            Id_input.Margin = new Padding(2, 2, 2, 2);
            Id_input.Name = "Id_input";
            Id_input.Size = new Size(165, 23);
            Id_input.TabIndex = 3;
            Id_input.TextChanged += Id_input_TextChanged;
            // 
            // Psw_input
            // 
            Psw_input.Location = new Point(186, 135);
            Psw_input.Margin = new Padding(2, 2, 2, 2);
            Psw_input.Name = "Psw_input";
            Psw_input.PasswordChar = '*';
            Psw_input.Size = new Size(165, 23);
            Psw_input.TabIndex = 4;
            // 
            // Register
            // 
            Register.Location = new Point(278, 214);
            Register.Margin = new Padding(2, 2, 2, 2);
            Register.Name = "Register";
            Register.Size = new Size(71, 24);
            Register.TabIndex = 5;
            Register.Text = "注册";
            Register.UseVisualStyleBackColor = true;
            Register.Click += Register_Click;
            // 
            // login
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(509, 319);
            Controls.Add(Register);
            Controls.Add(Psw_input);
            Controls.Add(Id_input);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(login_1);
            Margin = new Padding(2, 2, 2, 2);
            Name = "login";
            Text = "文件传输系统登陆界面";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button login_1;
        private Label label1;
        private Label label2;
        private TextBox Id_input;
        private TextBox Psw_input;
        private Button Register;
    }
}