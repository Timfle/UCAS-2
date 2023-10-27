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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Filemaneger
{

    public partial class login : Form
    {
        private static readonly string CertificatePath = "E:\\Filemaneger\\Filemaneger\\Filemaneger\\src\\localhost.pfx";
        private static readonly string CertificatePassword = "lcs09190924";

        private static NetworkStream stream = null;
        private static SslStream sslStream = null;
        public login()
        {
            InitializeComponent();
        }

        static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        static TcpClient Client(string serverip, int serverport)
        {

            TcpClient client = new TcpClient();
            X509Certificate2 clientCertificate = new X509Certificate2(CertificatePath, CertificatePassword);


            try
            {
                // 创建一个TcpClient对象

                // 设置远程主机的IP地址和端口号
                string serverIP = serverip;
                int serverPort = serverport;
                // 连接到远程主机
                client.Connect(serverIP, serverPort);
                // 连接成功
                Console.WriteLine("连接成功");
                // 获取网络流对象
                stream = client.GetStream();
                sslStream = new SslStream(client.GetStream(), false, ValidateServerCertificate);
                sslStream.AuthenticateAsClient("127.0.0.1", new X509CertificateCollection() { clientCertificate }, SslProtocols.Tls, true);
            }
            catch (Exception e)
            {
                Console.WriteLine("连接失败：" + e.Message);
                MessageBox.Show("连接失败：" + e.Message);
            }
            return client;
        }
        static string get_response(string json_message)    //往stream数据流中对应的server port发送数据并监听回应
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(json_message);
                sslStream.Write(data, 0, data.Length);
                Debug.WriteLine("已发送请求：" + json_message);
                sslStream.ReadTimeout = 5000;
                byte[] buffer = new byte[1024];
                int bytesRead = sslStream.Read(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Debug.WriteLine("接收到回应：" + response);
                return response;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        static string GetMd5Hash(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    builder.Append(data[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        static string json_code(string jsonData)
        {
            string code = "00";
            JObject jObject = JObject.Parse(jsonData);
            try
            {
                code = jObject.GetValue("code").ToString();
            }
            catch
            {

            }
            return code;

        }


        private void button1_Click(object sender, EventArgs e)
        {
            string userID = Id_input.Text.Trim();
            string psw = Psw_input.Text.Trim();
            Regex regex = new Regex("[^\u4e00-\u9fa5a-zA-Z0-9_]");
            if (regex.IsMatch(userID)) 
            {
                MessageBox.Show("输入非法，请输入汉字，字母、数字与下划线_的组合");
            }
            else if (regex.IsMatch(psw)) 
            {
                MessageBox.Show("输入非法");
            }
            else if (userID == null)
            {
                MessageBox.Show("请输入用户名");
            }
            else if (psw == null)
            {
                MessageBox.Show("请输入密码");
            }
            else
            {
                TcpClient client = Client("127.0.0.1", 8080);
                var obj = new { sourse = "127.0.0.1", destination = "127.0.0.1", code = "21", User = userID, password = GetMd5Hash(psw) };
                string json = JsonConvert.SerializeObject(obj);
                //get_response2(client.GetStream(), json);
                try
                {
                    string res = get_response(json);

                    if (json_code(res) == "22")
                    {
                        bool vip = Read_json.Json_data<bool>(res, "vip");
                        Form1 childrenForm = new Form1(client, sslStream, vip);
                        childrenForm.Owner = this;
                        childrenForm.Show();
                        this.Hide();
                    }
                    else
                    {

                        MessageBox.Show("用户名或密码不正确");
                    }
                }
                catch
                {

                    MessageBox.Show("服务器繁忙，请稍后重试");
                }
            }
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Register_Click(object sender, EventArgs e)
        {
            Register childrenForm = new Register();
            childrenForm.Owner = this;
            childrenForm.Show();
            this.Hide();
        }

        private void Id_input_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
