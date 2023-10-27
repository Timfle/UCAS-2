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
    public partial class Create_Floder : Form
    {
        private Form1 _Form1;

        public Create_Floder(Form1 form1)
        {
            InitializeComponent();
            _Form1 = form1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String name = Dir_name.Text.Trim();


            if (name.Length > 0)
            {
                Regex regex = new Regex("[^\u4e00-\u9fa5a-zA-Z0-9_]");
                bool isMatch = regex.IsMatch(name);

                // 如果输入字符不是字母、数字或下划线，则阻止输入
                if (!isMatch)
                {
                    _Form1.dirName0 = name;
                    this.Hide();
                    this.Owner.Show();
                }
                else
                {
                    MessageBox.Show("输入非法，请输入汉字，字母、数字与下划线_的组合");
                }
            }
            else
            {
                MessageBox.Show("请输入名称");
            }
        }

        private void Create_Floder_Load(object sender, EventArgs e)
        {

        }
    }
}
