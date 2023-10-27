using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Filemaneger
{
    public partial class New_filename : Form
    {
        private Form1 _Form1;
        public New_filename(Form1 form)
        {
            InitializeComponent();
            _Form1 = form;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String name = New_name.Text.Trim();


            if (name.Length > 0)
            {
                Regex regex = new Regex("[^\u4e00-\u9fa5a-zA-Z0-9_]");
                bool isMatch = regex.IsMatch(name);

                
                if (!isMatch)
                {
                    _Form1.fileName0 = name;
                    this.Hide();
                    this.Owner.Show();
                }
                else
                {
                    MessageBox.Show("输入非法，请输入字母、数字与下划线_的组合");
                }
            }
            else
            {
                MessageBox.Show("请输入名称");
            }
        }
    }
}
