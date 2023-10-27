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
    public partial class Form1 : Form   //������ȡJson���ж�Э�����͵�code
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
        //�������˵�����
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
            // �ڶ�ʱ���¼���������ִ���޸�ListView�Ĳ���
            if (flag[index] == 0)
            {
                Trans_list.Invoke(new Action(() => { Trans_list.Items[index].SubItems[2].Text = offsets[index] + " / " + len_s[index]; }));

            }
        }


        static string get_response(SslStream stream, string json_message)    //��stream�������ж�Ӧ��server port�������ݲ�������Ӧ
        {

            byte[] data = Encoding.UTF8.GetBytes(json_message);
            stream.Write(data, 0, data.Length);
            Debug.WriteLine("�ѷ�������" + json_message);

            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Debug.WriteLine("���յ���Ӧ��" + response);
            return response;
        }
        static void get_response2(SslStream stream, string json_message)    //��������Ӧ
        {

            byte[] data = Encoding.UTF8.GetBytes(json_message);
            stream.Write(data, 0, data.Length);
            Debug.WriteLine("�ѷ�������" + json_message);


        }
        static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // �Զ���֤����֤�߼������Ը�����Ҫ����ʵ��
            // ��ʾ�������Ǽ򵥵ط��� true���������κη�����֤��
            return true;
        }

        void Req_filedown(string json_mes, string local_path, long offset, int index)  //�ӷ�����������
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

                // ���ӵ�Զ������
                client2.Connect(serverIP, serverPort);
                NetworkStream networkStream2 = client2.GetStream();
                SslStream sslStream2 = new SslStream(client2.GetStream(), false, ValidateServerCertificate);
                sslStream2.AuthenticateAsClient(serverIP, new X509CertificateCollection() { clientCertificate }, SslProtocols.Tls, true);

                // ���������ļ���·��
                string filePath = local_path;
                string resumeFilePath = filePath + ".dat";
                if (!File.Exists(filePath))
                    using (File.Create(filePath)) ;


                using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.Write))
                {
                    // �����ļ���ƫ����
                    fileStream.Seek(offset, SeekOrigin.Begin);
                    byte[] buffer = new byte[buffersize];
                    int bytesRead;
                    // ѭ�������ļ�����
                    while ((bytesRead = sslStream2.Read(buffer, 0, buffer.Length)) > 0)
                    {

                        //�����յ�������д���ļ���
                        fileStream.Write(buffer, 0, bytesRead);

                        // ���¶ϵ��¼
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
                    Debug.WriteLine("�ļ�������ɣ�����Ϊ��" + filePath);
                    File.Delete(resumeFilePath);
                }
                else
                    Debug.WriteLine("�ļ�����ȡ��");
                // �ر�����
                client2.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("��������: " + ex.ToString());
            }
        }

        static string Choose_dirpath(string file_name)
        {
            string defaultFolder = srcdic_pth;
            defaultFolder = "C:\\Users\\HUAWEI\\Desktop\\localdir";
            string selectedFilePath = "error";
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = file_name; //���ó�ʼ�ļ���
            saveFileDialog.InitialDirectory = defaultFolder; //����Ĭ���ļ���·��
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
            //openFileDialog.Filter = "�ı��ļ� (*.txt)|*.txt"; // �������ù��������޶���ѡ�ļ�����
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
                Debug.WriteLine("ѡ����ļ�·���ǣ�" + filePath);
            }
            return filePath;
        }


        public void Req_fileup(string json_mes, string local_path, int index) //�ϴ���������
        {
            try
            {
                string res = get_response(sslStream, json_mes);
                string code = Read_json.Code(res);
                int new_port = Read_json.Port(res);
                TcpClient client2 = new TcpClient();
                int serverPort = new_port;

                // ���ӵ�Զ������
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
                    // �����ļ���ƫ����
                    fileStream.Seek(offset, SeekOrigin.Begin);

                    byte[] buffer = new byte[buffersize];
                    int bytesRead;
                    Debug.WriteLine("���ڷ���");
                    // ѭ�������ļ�����
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        // �����ݷ��͵�������
                        sslStream2.Write(buffer, 0, bytesRead);

                        // ���¶ϵ��¼
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
                // �ж϶ϵ��ļ��ļ�¼�Ƿ�����ļ�����
                bool isResumeComplete = fileInfo.Exists && offset == fileInfo.Length;
                if (isResumeComplete)
                {
                    Debug.WriteLine("�������");
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
                //Debug.WriteLine("�ѷ��ͶϿ���������" + json);


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

                if (fileName.Split("\\").Length - 1 == count0 + 1)  //�²��ļ���
                {
                    if (fileName.Substring(0, (fileName.Substring(0, fileName.LastIndexOf("\\"))).LastIndexOf("\\") + 1) == server_pth)
                    {
                        ListViewItem item = new ListViewItem();
                        item.Text = Path.GetFileName(fileName.Substring(0, fileName.LastIndexOf("\\")));
                        item.ImageIndex = 0;
                        item.SubItems.Add("�ļ���");
                        Filelist.Items.Add(item);
                    }
                }
            }

            foreach (string fileName in nameList)
            {
                if (fileName.Split("\\").Length - 1 == count0)  //�㼶��ͬ
                {
                    if (fileName.Substring(0, fileName.LastIndexOf("\\") + 1) == server_pth)    //���ڵ�ǰ�ļ���

                    {
                        ListViewItem item = new ListViewItem();
                        item.Text = Path.GetFileName(fileName);
                        string f_extension = Path.GetExtension(fileName);
                        f_extension = f_extension.Substring(1) + "�ļ�";
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

                if (a == "...")  //�����ϲ����
                {
                    if (server_pth == home)
                    {
                        MessageBox.Show("�ѵ����Ŀ¼");
                    }
                    else
                    {
                        server_pth = server_pth.Substring(0, server_pth.LastIndexOf("\\"));
                        server_pth = server_pth.Substring(0, server_pth.LastIndexOf("\\") + 1);
                        Form1_Load(sender, e);
                    }
                }
                else if (Filelist.SelectedItems[0].SubItems[1].Text == "�ļ���")
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
                item1.SubItems.Add("�ϴ��ļ�");
                item1.SubItems.Add("�ȴ�");
                Trans_list.Items.Add(item1);




                Task.Run(() =>
                {
                    flag.Add(0);
                    offsets.Add(0);
                    len_s.Add(length);
                    Stopwatch stopwatch = new Stopwatch();
                    // ��ʼ��ʱ
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
                            Trans_list.Invoke(new Action(() => { Trans_list.Items[index].SubItems[2].Text = "��ͣ"; }));
                            flag[index] = 11;
                        }
                    }

                    timer.Stop();
                    if (offsets[index] == len_s[index])
                        Trans_list.Invoke(new Action(() => { Trans_list.Items[index].SubItems[2].Text = "�����"; }));
                    Filelist.Invoke(new Action(() => Form1_Load(sender, e)));
                    stopwatch.Stop();
                    // �������ʱ��
                    Debug.WriteLine("\n\n\n\n\n����ʱ��: " + stopwatch.Elapsed);
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

                if (a == "...")  //�����ϲ����
                {
                    MessageBox.Show("��Ϊ�ϲ��ļ��У��޷�����");
                }

                else if (Directory.Exists(server_pth + "\\" + a))
                {
                    MessageBox.Show("��Ϊ�²��ļ��У��޷�����");
                }
                else
                {
                    full_serverpath = server_pth + "\\" + a;
                    full_localpath = Choose_dirpath(a); //���ļ���׺��
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
                    item1.SubItems.Add("�����ļ�");
                    item1.SubItems.Add("�ȴ�");
                    Trans_list.Items.Add(item1);


                    Task.Run(() =>
                   {
                       // ���� Stopwatch ����
                       Stopwatch stopwatch = new Stopwatch();

                       // ��ʼ��ʱ
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
                               Trans_list.Invoke(new Action(() => { Trans_list.Items[index].SubItems[2].Text = "��ͣ"; }));
                               flag[index] = 11;
                           }
                       }

                       timer.Stop();
                       if (offsets[index] == len_s[index])
                           Trans_list.Invoke(new Action(() => { Trans_list.Items[index].SubItems[2].Text = "�����"; }));

                       // ֹͣ��ʱ
                       stopwatch.Stop();

                       // �������ʱ��
                       Debug.WriteLine("����ʱ��: " + stopwatch.Elapsed);
                   }
                    );
                }
            }
            else
            {
                MessageBox.Show("δѡ���ļ�");
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
            Debug.WriteLine("�ѷ�������" + json);

            byte[] buffer = new byte[1024];
            int bytesRead = sslStream.Read(buffer, 0, buffer.Length);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Debug.WriteLine("���յ���Ӧ��" + response);

            string mes = Read_json.Json_data<string>(response, "mes");
            MessageBox.Show(mes);
            if (mes == "�ļ��д����ɹ���")
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
            Debug.WriteLine("�ѷ�������" + json);



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
                if (a.SubItems[1].Text == "�����ļ�")
                {
                    if (flag[index] == 11)
                        flag[index] = 0;
                    else if (flag[index] == 0)
                        flag[index] = 1;
                }
                else if (a.SubItems[1].Text == "�ϴ��ļ�")
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

                if (a == "...")  //�����ϲ����
                {
                    MessageBox.Show("��Ϊ�ϲ��ļ��У��޷�������");
                }

                else if (Filelist.SelectedItems[0].SubItems[1].Text == "�ļ���")
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
                    if (Filelist.SelectedItems[0].SubItems[1].Text == "�ļ���")
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

                if (a == "...")  //�����ϲ����
                {
                    MessageBox.Show("��Ϊ�ϲ��ļ��У��޷�������");
                }

                else if (Filelist.SelectedItems[0].SubItems[1].Text == "�ļ���")
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
                    if (Filelist.SelectedItems[0].SubItems[1].Text == "�ļ���")
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
                MessageBox.Show("�˳��ɹ�");
                client.Close();
                this.Hide();
                this.Owner.Show();

            }
            else
            {
                MessageBox.Show("�˳�ʧ��");
            }
        }
    }
}