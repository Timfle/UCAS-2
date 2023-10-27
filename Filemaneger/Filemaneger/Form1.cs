using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Web;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms.Design;
using System.Diagnostics;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Reflection;
using System.Data.SqlClient;
using System.IO.Pipes;
using System.Security.Cryptography.X509Certificates;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Net.Security;
using System.Security.Authentication;

namespace Filemaneger
{
    public partial class Form1 : Form   //用作获取Json中判断协议类型的code
    {
        List<int> flag = new List<int>();
        public string dirName0 { get; set; }
        public string fileName0 { get; set; }

        static List<long> offsets = new List<long>();
        static List<long> len_s = new List<long>();
        List<string> nameList = new List<string>();
        List<string> dirList = new List<string>();
        static string connectionString = System.Configuration.ConfigurationManager.AppSettings["connectionString"];
        SqlConnection con = new SqlConnection(connectionString);
        static List<string> fileTree = new List<string>();
        static string serverIP = "127.0.0.1";
        SslStream sslStream = null;
        int serverPort = 8080;
        static string local_pth = null;
        static string srcdic_pth = null;
        private static readonly string CertificatePath = "E:\\Filemaneger\\Filemaneger\\Filemaneger\\src\\localhost.pfx";
        private static readonly string CertificatePassword = "lcs09190924";
        X509Certificate2 clientCertificate = new X509Certificate2(CertificatePath, CertificatePassword);
        bool vip;
        int buffersize;
        int count0 = 0;
        //服务器端的配置
        string home = null;
        string server_pth = null;
        bool first = true;

        private static NetworkStream stream = null;
        static TcpClient client = new TcpClient();
        public Form1(TcpClient client0, SslStream sslStream0, bool vip)
        {
            InitializeComponent();
            client = client0;
            stream = client.GetStream();
            sslStream = sslStream0;
            if (vip) buffersize = 40960;
            else buffersize = 4096;
            string exeFullPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            local_pth = exeFullPath;
            local_pth = local_pth.Substring(0, local_pth.LastIndexOf("\\"));
            local_pth = local_pth.Substring(0, local_pth.LastIndexOf("\\"));
            local_pth = local_pth.Substring(0, local_pth.LastIndexOf("\\"));
            srcdic_pth = local_pth + "\\src";
            if (!Directory.Exists(srcdic_pth))
            {
                Directory.CreateDirectory(srcdic_pth);
            }


            Ip_port Serveripandport = Ip_port.GetServerIPAndPort(client);

            serverIP = Serveripandport.ip;
            serverPort = Serveripandport.port;
        }

        private void flash_filetrans(object sender, EventArgs e, int index)
        {
            // 在定时器事件处理函数中执行修改ListView的操作
            if (flag[index] == 0)
            {
                Trans_list.Invoke(new Action(() => { Trans_list.Items[index].SubItems[2].Text = offsets[index] + " / " + len_s[index]; }));

            }
        }


        static string get_response(SslStream stream, string json_message)    //往stream数据流中对应的server port发送数据并监听回应
        {

            byte[] data = Encoding.UTF8.GetBytes(json_message);
            stream.Write(data, 0, data.Length);
            Debug.WriteLine("已发送请求：" + json_message);

            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Debug.WriteLine("接收到回应：" + response);
            return response;
        }
        static void get_response2(SslStream stream, string json_message)    //不监听回应
        {

            byte[] data = Encoding.UTF8.GetBytes(json_message);
            stream.Write(data, 0, data.Length);
            Debug.WriteLine("已发送请求：" + json_message);


        }
        static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // 自定义证书验证逻辑，可以根据需要进行实现
            // 在示例中我们简单地返回 true，即接受任何服务器证书
            return true;
        }

        void Req_filedown(string json_mes, string local_path, long offset, int index)  //从服务器端下载
        {
            try
            {

                string res = get_response(sslStream, json_mes);
                string code = Read_json.Json_data<string>(res, "code");
                int new_port = Read_json.Json_data<int>(res, "port");
                len_s[index] = Read_json.Json_data<int>(res, "len");
                Debug.WriteLine(index.ToString() + " reqF" + len_s[index]);
                TcpClient client2 = new TcpClient();
                int serverPort = new_port;

                // 连接到远程主机
                client2.Connect(serverIP, serverPort);
                NetworkStream networkStream2 = client2.GetStream();
                SslStream sslStream2 = new SslStream(client2.GetStream(), false, ValidateServerCertificate);
                sslStream2.AuthenticateAsClient(serverIP, new X509CertificateCollection() { clientCertificate }, SslProtocols.Tls, true);

                // 创建接收文件的路径
                string filePath = local_path;
                string resumeFilePath = filePath + ".dat";
                if (!File.Exists(filePath))
                    using (File.Create(filePath)) ;


                using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.Write))
                {
                    // 设置文件流偏移量
                    fileStream.Seek(offset, SeekOrigin.Begin);
                    byte[] buffer = new byte[buffersize];
                    int bytesRead;
                    // 循环接收文件数据
                    while ((bytesRead = sslStream2.Read(buffer, 0, buffer.Length)) > 0)
                    {

                        //将接收到的数据写入文件流
                        fileStream.Write(buffer, 0, bytesRead);

                        // 更新断点记录
                        offset += bytesRead;
                        offsets[index] = offset;
                        using (FileStream resumeFile = File.Create(resumeFilePath))
                        {
                            byte[] offsetBytes = BitConverter.GetBytes(offset);
                            resumeFile.Write(offsetBytes, 0, offsetBytes.Length);
                        }

                        while (flag[index] == 11)
                        {

                        }
                    }
                }
                if (offset == len_s[index])
                {
                    Debug.WriteLine("文件接收完成，保存为：" + filePath);
                    File.Delete(resumeFilePath);
                }
                else
                    Debug.WriteLine("文件传输取消");
                // 关闭连接
                client2.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("发生错误: " + ex.ToString());
            }
        }

        static string Choose_dirpath(string file_name)
        {
            string defaultFolder = srcdic_pth;
            defaultFolder = "C:\\Users\\HUAWEI\\Desktop\\localdir";
            string selectedFilePath = "error";
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = file_name; //设置初始文件名
            saveFileDialog.InitialDirectory = defaultFolder; //设置默认文件夹路径
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                selectedFilePath = saveFileDialog.FileName;
            }
            return selectedFilePath;
        }

        static string Choose_filepath()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string filePath = null;
            openFileDialog.InitialDirectory = @"C:\Users\HUAWEI\Desktop\localdir";
            //openFileDialog.Filter = "文本文件 (*.txt)|*.txt"; // 可以设置过滤器，限定可选文件类型
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
                Debug.WriteLine("选择的文件路径是：" + filePath);
            }
            return filePath;
        }


        public void Req_fileup(string json_mes, string local_path, int index) //上传至服务器
        {
            try
            {
                string res = get_response(sslStream, json_mes);
                string code = Read_json.Code(res);
                int new_port = Read_json.Port(res);
                TcpClient client2 = new TcpClient();
                int serverPort = new_port;

                // 连接到远程主机
                client2.Connect(serverIP, serverPort);
                NetworkStream networkStream2 = client2.GetStream();
                SslStream sslStream2 = new SslStream(client2.GetStream(), false, ValidateServerCertificate);
                sslStream2.AuthenticateAsClient(serverIP, new X509CertificateCollection() { clientCertificate }, SslProtocols.Tls, true);


                long offset = 0;

                string resumeFilePath = local_path + ".dat";

                if (File.Exists(resumeFilePath))
                {
                    using (FileStream resumeFile = File.OpenRead(resumeFilePath))
                    {
                        byte[] offsetBytes = new byte[sizeof(long)];
                        resumeFile.Read(offsetBytes, 0, offsetBytes.Length);
                        offset = BitConverter.ToInt64(offsetBytes, 0);
                    }
                }

                FileInfo fileInfo = new FileInfo(local_path);
                long length = fileInfo.Length - offset;



                using (FileStream fileStream = File.Open(local_path, FileMode.Open, FileAccess.Read))
                {
                    // 设置文件流偏移量
                    fileStream.Seek(offset, SeekOrigin.Begin);

                    byte[] buffer = new byte[buffersize];
                    int bytesRead;
                    Debug.WriteLine("正在发送");
                    // 循环发送文件数据
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        // 将数据发送到服务器
                        sslStream2.Write(buffer, 0, bytesRead);

                        // 更新断点记录
                        offset += bytesRead;
                        offsets[index] = offset;
                        using (FileStream resumeFile = File.Create(resumeFilePath))
                        {
                            byte[] offsetBytes = BitConverter.GetBytes(offset);
                            resumeFile.Write(offsetBytes, 0, offsetBytes.Length);
                        }
                        while (flag[index] == 11)
                        {
                            ;
                        }
                    }
                }
                // 判断断点文件的记录是否等于文件长度
                bool isResumeComplete = fileInfo.Exists && offset == fileInfo.Length;
                if (isResumeComplete)
                {
                    Debug.WriteLine("发送完成");
                    File.Delete(resumeFilePath);
                }
                byte[] buffer1 = new byte[1024];
                int bytesRead1 = sslStream2.Read(buffer1, 0, buffer1.Length);
                string response1 = Encoding.UTF8.GetString(buffer1, 0, bytesRead1);
                string code1 = Read_json.Json_data<string>(response1, "code");
                //Debug.WriteLine(code1);
                if (code1 == "34")
                {
                    string pth = Read_json.Json_data<string>(response1, "pth");
                    nameList.Add(pth);

                }




                client2.Close();

                //var obj = new { sourse = "127.0.0.1", destination = "127.0.0.1", code = "34" };
                //string json = JsonConvert.SerializeObject(obj);
                //byte[] data = Encoding.UTF8.GetBytes(json);
                //stream.Write(data, 0, data.Length);
                //Debug.WriteLine("已发送断开连接请求：" + json);


            }
            catch (Exception ex)
            {
            }



        }


        public void Director(string dir, List<string> list, List<string> diclist, List<string> fileTree)
        {

            var obj = new { sourse = "127.0.0.1", destination = "127.0.0.1", code = "51" };
            string json = JsonConvert.SerializeObject(obj);
            get_response2(sslStream, json);

            byte[] buffer0 = new byte[1024];
            int bytesRead0;

            while ((bytesRead0 = sslStream.Read(buffer0, 0, buffer0.Length)) > 0)
            {

                string request = Encoding.UTF8.GetString(buffer0, 0, bytesRead0);
                Debug.WriteLine(request);

                string pth = Read_json.Json_data<String>(request, "pth");
                if (home == null)
                {
                    home = pth;
                    server_pth = home;
                }
                else fileTree.Add(pth);
                string code = Read_json.Json_data<string>(request, "code");
                if (code == "53") break;
                //Debug.WriteLine(request);
            }

            foreach (string item in fileTree)
            {
                if (item != null)
                {
                    Debug.WriteLine(item);
                    if (item.EndsWith("\\"))
                    {
                        diclist.Add(item);
                    }
                    else
                    {
                        list.Add(item);
                    }
                }

            }

        }


        private void Form1_Load(object sender, EventArgs e)
        {

            if (first)
            {

                Director(server_pth, nameList, dirList, fileTree);
                first = false;
            }

            count0 = server_pth.Split("\\").Length - 1;
            Filelist.Items.Clear();

            ListViewItem item0 = new ListViewItem();
            item0.Text = "...";
            item0.ImageIndex = 0;
            Filelist.Items.Add(item0);



            foreach (string fileName in dirList)
            {

                if (fileName.Split("\\").Length - 1 == count0 + 1)  //下层文件夹
                {
                    if (fileName.Substring(0, (fileName.Substring(0, fileName.LastIndexOf("\\"))).LastIndexOf("\\") + 1) == server_pth)
                    {
                        ListViewItem item = new ListViewItem();
                        item.Text = Path.GetFileName(fileName.Substring(0, fileName.LastIndexOf("\\")));
                        item.ImageIndex = 0;
                        item.SubItems.Add("文件夹");
                        Filelist.Items.Add(item);
                    }
                }
            }

            foreach (string fileName in nameList)
            {
                if (fileName.Split("\\").Length - 1 == count0)  //层级相同
                {
                    if (fileName.Substring(0, fileName.LastIndexOf("\\") + 1) == server_pth)    //属于当前文件夹

                    {
                        ListViewItem item = new ListViewItem();
                        item.Text = Path.GetFileName(fileName);
                        string f_extension = Path.GetExtension(fileName);
                        f_extension = f_extension.Substring(1) + "文件";
                        item.SubItems.Add(f_extension);
                        Filelist.Items.Add(item);
                    }

                }
            }


            //string a = Filelist.SelectedItems[0].SubItems[0].Text;
            //Console.WriteLine(a);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        private void Filelist_DoubleClick(object sender, EventArgs e)
        {
            if (Filelist.SelectedItems.Count > 0)
            {

                string a = Filelist.SelectedItems[0].SubItems[0].Text;

                if (a == "...")  //返回上层操作
                {
                    if (server_pth == home)
                    {
                        MessageBox.Show("已到达根目录");
                    }
                    else
                    {
                        server_pth = server_pth.Substring(0, server_pth.LastIndexOf("\\"));
                        server_pth = server_pth.Substring(0, server_pth.LastIndexOf("\\") + 1);
                        Form1_Load(sender, e);
                    }
                }
                else if (Filelist.SelectedItems[0].SubItems[1].Text == "文件夹")
                {

                    server_pth = server_pth + a + "\\";
                    Debug.WriteLine(server_pth);
                    Form1_Load(sender, e);
                }
                else
                    MessageBox.Show(a);
            }
        }

        private void button_Upload_Click(object sender, EventArgs e)
        {
            string local_filepth = Choose_filepath();
            if (local_filepth != null)
            {
                FileInfo fileInfo = new FileInfo(local_filepth);
                long length = fileInfo.Length;
                var obj = new { sourse = "127.0.0.1", destination = "127.0.0.1", code = "32", path = server_pth + Path.GetFileName(local_filepth), len = length };
                Debug.WriteLine(obj);
                string json = JsonConvert.SerializeObject(obj);

                int index = Trans_list.Items.Count;
                ListViewItem item1 = new ListViewItem();
                item1.Text = Path.GetFileName(local_filepth);
                item1.SubItems.Add("上传文件");
                item1.SubItems.Add("等待");
                Trans_list.Items.Add(item1);




                Task.Run(() =>
                {
                    flag.Add(0);
                    offsets.Add(0);
                    len_s.Add(length);
                    Stopwatch stopwatch = new Stopwatch();
                    // 开始计时
                    stopwatch.Start();
                    Task.Run(() => { Req_fileup(json, local_filepth, index); });

                    System.Timers.Timer timer = new System.Timers.Timer();
                    timer.Interval = 1000;
                    timer.Elapsed += (sender, e) => flash_filetrans(sender, e, index);

                    timer.Start();
                    while (offsets[index] != len_s[index])
                    {
                        if (flag[index] == 1)
                        {
                            Trans_list.Invoke(new Action(() => { Trans_list.Items[index].SubItems[2].Text = "暂停"; }));
                            flag[index] = 11;
                        }
                    }

                    timer.Stop();
                    if (offsets[index] == len_s[index])
                        Trans_list.Invoke(new Action(() => { Trans_list.Items[index].SubItems[2].Text = "已完成"; }));
                    Filelist.Invoke(new Action(() => Form1_Load(sender, e)));
                    stopwatch.Stop();
                    // 输出运行时间
                    Debug.WriteLine("\n\n\n\n\n运行时间: " + stopwatch.Elapsed);
                }

                );

            }



        }

        private void button_Download_Click(object sender, EventArgs e)
        {
            string full_localpath = "error";
            string full_serverpath = null;
            if (Filelist.SelectedItems.Count > 0)
            {
                string a = Filelist.SelectedItems[0].SubItems[0].Text;

                if (a == "...")  //返回上层操作
                {
                    MessageBox.Show("此为上层文件夹，无法传输");
                }

                else if (Directory.Exists(server_pth + "\\" + a))
                {
                    MessageBox.Show("此为下层文件夹，无法传输");
                }
                else
                {
                    full_serverpath = server_pth + "\\" + a;
                    full_localpath = Choose_dirpath(a); //含文件后缀名
                }

                if (full_localpath != "error")
                {


                    string resumeFilePath = full_localpath + ".dat";
                    long offset = 0;

                    if (File.Exists(resumeFilePath))
                    {
                        using (FileStream resumeFile = File.OpenRead(resumeFilePath))
                        {
                            byte[] offsetBytes = new byte[sizeof(long)];
                            resumeFile.Read(offsetBytes, 0, offsetBytes.Length);
                            offset = BitConverter.ToInt64(offsetBytes, 0);
                        }
                    }

                    var obj = new { sourse = "127.0.0.1", destination = "127.0.0.1", code = "31", path = full_serverpath, offset = offset };
                    string json = JsonConvert.SerializeObject(obj);

                    int index = Trans_list.Items.Count;
                    ListViewItem item1 = new ListViewItem();
                    item1.Text = Path.GetFileName(full_localpath);
                    item1.SubItems.Add("下载文件");
                    item1.SubItems.Add("等待");
                    Trans_list.Items.Add(item1);


                    Task.Run(() =>
                   {
                       // 创建 Stopwatch 对象
                       Stopwatch stopwatch = new Stopwatch();

                       // 开始计时
                       stopwatch.Start();


                       flag.Add(0);
                       offsets.Add(0);
                       len_s.Add(-1);
                       Task.Run(() => { Req_filedown(json, full_localpath, offset, index); });

                       System.Timers.Timer timer = new System.Timers.Timer();
                       timer.Interval = 1000;
                       timer.Elapsed += (sender, e) => flash_filetrans(sender, e, index);

                       timer.Start();
                       while (offsets[index] != len_s[index])
                       {
                           if (flag[index] == 1)
                           {
                               Trans_list.Invoke(new Action(() => { Trans_list.Items[index].SubItems[2].Text = "暂停"; }));
                               flag[index] = 11;
                           }
                       }

                       timer.Stop();
                       if (offsets[index] == len_s[index])
                           Trans_list.Invoke(new Action(() => { Trans_list.Items[index].SubItems[2].Text = "已完成"; }));

                       // 停止计时
                       stopwatch.Stop();

                       // 输出运行时间
                       Debug.WriteLine("运行时间: " + stopwatch.Elapsed);
                   }
                    );
                }
            }
            else
            {
                MessageBox.Show("未选择文件");
            }


        }

        private void button3_Click(object sender, EventArgs e)
        {


            Create_Floder childrenForm = new Create_Floder(this);
            childrenForm.Owner = this;
            childrenForm.ShowDialog();
            string dir_name = server_pth + dirName0 + "\\";
            var obj = new { sourse = "127.0.0.1", destination = "127.0.0.1", code = "54", dir_name = dir_name };
            string json = JsonConvert.SerializeObject(obj);
            byte[] data = Encoding.UTF8.GetBytes(json);
            sslStream.Write(data, 0, data.Length);
            Debug.WriteLine("已发送请求：" + json);

            byte[] buffer = new byte[1024];
            int bytesRead = sslStream.Read(buffer, 0, buffer.Length);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Debug.WriteLine("接收到回应：" + response);

            string mes = Read_json.Json_data<string>(response, "mes");
            MessageBox.Show(mes);
            if (mes == "文件夹创建成功！")
            {
                Debug.WriteLine(dir_name);
                dirList.Add(dir_name);
                Form1_Load(sender, e);
            }
        }

        private void button_Ping_Click(object sender, EventArgs e)
        {
            var obj = new { sourse = "127.0.0.1", destination = "127.0.0.1", code = "99", };
            string json = JsonConvert.SerializeObject(obj);
            byte[] data = Encoding.UTF8.GetBytes(json);
            sslStream.Write(data, 0, data.Length);
            Debug.WriteLine("已发送请求：" + json);



        }

        private void Fresh_Click(object sender, EventArgs e)
        {
            Form1_Load(sender, e);
        }

        private void Trans_view_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button_pause_down_Click(object sender, EventArgs e)
        {
            if (Trans_list.SelectedItems.Count > 0)
            {
                ListViewItem a = Trans_list.SelectedItems[0];
                int index = Trans_list.Items.IndexOf(a);
                if (a.SubItems[1].Text == "下载文件")
                {
                    if (flag[index] == 11)
                        flag[index] = 0;
                    else if (flag[index] == 0)
                        flag[index] = 1;
                }
                else if (a.SubItems[1].Text == "上传文件")
                {
                    if (flag[index] == 11)
                        flag[index] = 0;
                    else if (flag[index] == 0)
                        flag[index] = 1;
                }

            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button_rename_Click(object sender, EventArgs e)
        {

            string full_serverpath = null;
            if (Filelist.SelectedItems.Count > 0)
            {
                string a = Filelist.SelectedItems[0].SubItems[0].Text;

                if (a == "...")  //返回上层操作
                {
                    MessageBox.Show("此为上层文件夹，无法重命名");
                }

                else if (Filelist.SelectedItems[0].SubItems[1].Text == "文件夹")
                {
                    full_serverpath = server_pth + a + "\\";
                }
                else
                {
                    full_serverpath = server_pth + a;

                }

                New_filename childrenForm = new New_filename(this);
                childrenForm.Owner = this;
                childrenForm.ShowDialog();

                string name = "test";
                string n = full_serverpath;


                var obj = new { sourse = "127.0.0.1", destination = "127.0.0.1", code = "41", path = full_serverpath, name = fileName0 };
                string json = JsonConvert.SerializeObject(obj);
                string res = get_response(sslStream, json);
                if (Read_json.Json_data<string>(res, "code") == "42")
                {
                    if (Filelist.SelectedItems[0].SubItems[1].Text == "文件夹")
                        dirList[dirList.IndexOf(n)] = server_pth + fileName0 + "\\";
                    else
                        nameList[nameList.IndexOf(n)] = server_pth + fileName0 + Path.GetExtension(nameList[nameList.IndexOf(n)]);
                    Form1_Load(sender, e);
                }
            }
        }

        private void button2_delete_Click(object sender, EventArgs e)
        {
            string full_localpath = "error";
            string full_serverpath = null;
            if (Filelist.SelectedItems.Count > 0)
            {
                string a = Filelist.SelectedItems[0].SubItems[0].Text;

                if (a == "...")  //返回上层操作
                {
                    MessageBox.Show("此为上层文件夹，无法重命名");
                }

                else if (Filelist.SelectedItems[0].SubItems[1].Text == "文件夹")
                {
                    full_serverpath = server_pth + a + "\\";
                }

                else
                {
                    full_serverpath = server_pth + a;

                }

                var obj = new { sourse = "127.0.0.1", destination = "127.0.0.1", code = "43", path = full_serverpath };
                string json = JsonConvert.SerializeObject(obj);
                string res = get_response(sslStream, json);
                string n = full_serverpath;
                if (Read_json.Json_data<string>(res, "code") == "44")
                {
                    if (Filelist.SelectedItems[0].SubItems[1].Text == "文件夹")
                        dirList.Remove(full_serverpath);
                    else
                        nameList.Remove(full_serverpath);
                    Form1_Load(sender, e);
                }

            }
        }

        private void button_logout_Click(object sender, EventArgs e)
        {
            var obj = new { sourse = "127.0.0.1", destination = "127.0.0.1", code = "24" };
            string json = JsonConvert.SerializeObject(obj);
            string res = get_response(sslStream, json);
            if (Read_json.Json_data<string>(res, "code") == "25")
            {
                MessageBox.Show("退出成功");
                client.Close();
                this.Hide();
                this.Owner.Show();

            }
            else
            {
                MessageBox.Show("退出失败");
            }
        }
    }
}