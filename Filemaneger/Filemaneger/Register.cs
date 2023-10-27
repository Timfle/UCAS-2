using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Filemaneger
{
    public partial class Register : Form
    {
        private static readonly string CertificatePath = "E:\\Filemaneger\\Filemaneger\\Filemaneger\\src\\localhost.pfx";
        private static readonly string CertificatePassword = "lcs09190924";
        private static SslStream sslStream = null;
        public Register()
        {
            InitializeComponent();


        }
        static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string userID = Username.Text.Trim();
            string psw = Psw.Text.Trim();
            string psw2 = Psw2.Text.Trim();
            if (userID == null)
            {
                MessageBox.Show("请输入用户名");
            }
            else if (psw == null)
            {
                MessageBox.Show("请输入密码");
            }
            else if (psw2 == null)
            {
                MessageBox.Show("请确认密码");
            }
            else if (psw != psw2)
            {
                MessageBox.Show("两次输入密码不一致" + psw + "   " + psw2);
            }
            else
            {
                TcpClient client = Client.Start_Client("127.0.0.1", 8080);
                X509Certificate2 clientCertificate = new X509Certificate2(CertificatePath, CertificatePassword);
                sslStream = new SslStream(client.GetStream(), false, ValidateServerCertificate);
                sslStream.AuthenticateAsClient("127.0.0.1", new X509CertificateCollection() { clientCertificate }, SslProtocols.Tls, true);

                var obj = new { sourse = "127.0.0.1", destination = "127.0.0.1", code = "11", User = userID, password = Crypt.GetMd5Hash(psw) };
                string json = JsonConvert.SerializeObject(obj);
                string res = Client.get_response(sslStream, json);
                string show = null;
                string code = Read_json.Json_data<string>(res, "code");
                if (code == "12")
                {
                    string mes = Read_json.Json_data<string>(res, "mes");
                    show = mes;
                    MessageBox.Show(show);
                    if (mes == "成功注册")
                    {
                        this.Hide();
                        this.Owner.Show();
                    }
                }


            }
        }

        private void Register_Load(object sender, EventArgs e)
        {

        }

        private void Register_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Owner.Show();
        }
    }
}
