using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualBasic;
using Azure.Core;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using flieclient;

class TCPClient
{ 
    static string connectionString = System.Configuration.ConfigurationManager.AppSettings["connectionString"];
    static SqlConnection con = new SqlConnection(connectionString);
    static readonly string serverroot = "E:\\Filemaneger\\test\\serverroot\\";
    static string user_root;
    private static readonly string CertificatePath = "E:\\Filemaneger\\Filemaneger\\Filemaneger\\src\\localhost.pfx";
    private static readonly string CertificatePassword = "lcs09190924";
    static X509Certificate2 serverCertificate = new X509Certificate2(CertificatePath, CertificatePassword);
    static bool vip;
    static int buffersize;

    public static DataSet Query(String sql)
    {
        //创建数据库连接
        SqlConnection con = new SqlConnection(connectionString);
        //创建DataSet和SQL Server之间的桥接器，用于对数据库进行操作
        SqlDataAdapter sda = new SqlDataAdapter(sql, con);

        //创建数据集
        DataSet ds = new DataSet();
        try
        {
            con.Open();
            sda.Fill(ds, "Users"); //往窗体里的students表填充数据集ds
            return ds;
        }
        catch (SqlException e)
        {
            throw new Exception(e.Message);//抛出异常
        }
        finally
        {
            sda.Dispose();//sda处理，以便空间回收
            con.Close();
        }
    }

    public static DataSet Lg_Reg_Query(String sql,string username,string psw,string flag)
    {
        //创建数据库连接
        SqlConnection con = new SqlConnection(connectionString);
        //创建DataSet和SQL Server之间的桥接器，用于对数据库进行操作

        //创建数据集
        DataSet ds = new DataSet();

        try
        {
            using (SqlCommand command = new SqlCommand(sql, con))
        {
                command.Parameters.Add("@username", SqlDbType.NChar, 10).Value=username;
                if (flag =="login")
                    command.Parameters.Add("@password",SqlDbType.NChar, 32).Value=psw;
                // 预编译查询
                con.Open();
                command.Prepare();
                SqlDataAdapter sda = new SqlDataAdapter(command);
                sda.Fill(ds); //往窗体里的students表填充数据集ds
                sda.Dispose();//sda处理，以便空间回收
                return ds;
            }
        }
        catch (SqlException e)
        {
            Console.WriteLine(e.Message);
            throw new Exception(e.Message);//抛出异常
        }
        finally
        {

            con.Close();
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
    public static bool Lg_Reg(String Username,string psw,string flag)
    {
        string sql = null;
        if (flag == "login")
            sql = "SELECT * FROM Myuser WHERE UserID = @username AND Psw = @password";
        else if (flag =="reg")
            sql = "SELECT * FROM Myuser WHERE UserID = @username ";

        DataSet ds =Lg_Reg_Query(sql,Username,psw,flag);
       // DataSet ds = Query(sql);
        DataTable dt = ds.Tables[0];
        if (dt.Rows.Count == 0)
        {
            
        }

        //string sqlpsw = dt.Rows[0][1].ToString();
        //bool isEqual = string.Equals(psw, sqlpsw, StringComparison.OrdinalIgnoreCase);
        bool isEqual = dt.Rows.Count == 0? false: true;
        //Console.WriteLine(isEqual);
        return isEqual;
    }

    public static bool online(String Username)
    {
        string sql = "select Online from Myuser where UserID = '" + Username + "'";
        DataSet ds = Query(sql);
        DataTable dt = ds.Tables[0];
        //Console.WriteLine(dt.Rows[0][1]);
        //Console.WriteLine(psw);
        string online = dt.Rows[0][0].ToString();
        return online == "0" ?  true :  false;
    }
    public static bool Vip(String Username)
    {
        string sql = "select vip from Myuser where UserID = '" + Username + "'";
        DataSet ds = Query(sql);
        DataTable dt = ds.Tables[0];
        //Console.WriteLine(dt.Rows[0][1]);
        //Console.WriteLine(psw);
        string vip = dt.Rows[0][0].ToString();
        return vip == "1" ? true : false;
    }

    static string serverIP = "127.0.0.1";
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
    public static (int,TcpListener) FindFreePort(int startPort)
    {
        int port = startPort;

        while (port <= 65535) // 端口范围从0到65535
        {
            try
            {
                TcpListener listener = new TcpListener(IPAddress.Loopback, port);
                listener.Start();
                //listener.Stop(); 
               
                return (port,listener);
                
            }
            catch
            {
                // 端口已被占用，尝试下一个端口
                port++;
            }
        }

        return (-1,null); // 未找到空闲端口
    }

    static string json_path(string jsonData)
    {
        string path = "00";

        JObject jObject = JObject.Parse(jsonData);
        try
        {
            path = jObject.GetValue("path").ToString();
        }
        catch
        {

        }
        return path;
    }
    static long json_len(string jsonData)
    {
        long length = 0;

        JObject jObject = JObject.Parse(jsonData);
        try
        {
            length = jObject.GetValue("len").Value<int>();
        }
        catch
        {

        }
        return length;
    }

    static T Json_data<T>(string jsonData,string name)
    {
        JToken value =null;
        JObject jObject = JObject.Parse(jsonData);
        try
        {
            value = jObject.GetValue(name);
            return value != null ? value.ToObject<T>() : default(T);
        }
        catch
        {

            return typeof(T) == typeof(int) ? (T)(object)0 : (T)(object)"0";
        }

    }

    static string reg(string user,string psw)
    {
        string result = null;
        string sql = "insert into Myuser(UserId,Psw,Vip,Online) values(@userid,@psw,'0','0')";
        
            //创建数据库连接
            SqlConnection con = new SqlConnection(connectionString);
            //创建DataSet和SQL Server之间的桥接器，用于对数据库进行操作

            //创建数据集
            DataSet ds = new DataSet();

            try
            {
                using (SqlCommand command = new SqlCommand(sql, con))
                {
                    command.Parameters.Add("@userid", SqlDbType.NChar, 10).Value = user;
                    command.Parameters.Add("@psw", SqlDbType.NChar, 32).Value = psw;
                    // 预编译查询
                    con.Open();
                    command.Prepare();
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    result = "成功注册";
                    Console.WriteLine("成功插入 " + rowsAffected + " 行数据。");
                }
                else
                {
                    result = "注册不成功";
                    Console.WriteLine("未插入任何数据。");
                }
            }
            }
            catch (SqlException e)
            {
                result = e.Message;
                Console.WriteLine(e.Message);
                throw new Exception(e.Message);//抛出异常
            }
            finally
            {

                con.Close();
            }
        return result;
    }

    public static List<string> GetFileTree(string path)
    {
        List<string> fileTree = new List<string>();

        // 获取当前路径下的所有文件
        foreach (string file in Directory.GetFiles(path))
        {
            fileTree.Add(file);
        }

        // 获取当前路径下的所有文件夹
        foreach (string directory in Directory.GetDirectories(path))
        {
            fileTree.Add(directory + Path.DirectorySeparatorChar); // 添加文件夹路径

            // 递归遍历子文件夹
            List<string> subFileTree = GetFileTree(directory);
            foreach (string subFile in subFileTree)
            {
                fileTree.Add(subFile);
            }
        }
        return fileTree;
    }
    static void HandleFile(TcpClient client,string request, TcpListener listener2 )
    {
        
        NetworkStream networkStream2 = client.GetStream();
        SslStream sslStream = TLS.Build_SSL(client, serverCertificate);

        string filePath = json_path(request);
        {
            long rec_len = json_len(request);
            // 创建接收文件的路径
            long offset = 0;
            string resumeFilePath = filePath + ".dat";
            if (File.Exists(resumeFilePath))
            {
                using (FileStream resumeFile = File.OpenRead(resumeFilePath))
                {
                    byte[] offsetBytes = new byte[sizeof(long)];
                    resumeFile.Read(offsetBytes, 0, offsetBytes.Length);
                    offset = BitConverter.ToInt64(offsetBytes, 0);
                }
            }
            if (!File.Exists(filePath))
                using (File.Create(filePath)) ;

            using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.Write))
            {
                // 设置文件流偏移量
                fileStream.Seek(offset, SeekOrigin.Begin);

                byte[] buffer1 = new byte[buffersize];
                int bytesRead1;

                // 循环接收文件数据

                while ((bytesRead1 = sslStream.Read(buffer1, 0, buffer1.Length)) > 0)
                {
                    //将接收到的数据写入文件流
                    fileStream.Write(buffer1, 0, bytesRead1);

                    // 更新断点记录
                    offset += bytesRead1;
                    using (FileStream resumeFile = File.Create(resumeFilePath))
                    {
                        byte[] offsetBytes = BitConverter.GetBytes(offset);
                        resumeFile.Write(offsetBytes, 0, offsetBytes.Length);
                    }


                    if (offset == rec_len)
                    {
                        Console.WriteLine("文件接收完成，保存为：" + filePath);
                        File.Delete(resumeFilePath);
                        var obj = new { sourse = "127.0.0.1", destination = "127.0.0.1", code = "34", pth = filePath };
                        string json = JsonConvert.SerializeObject(obj);
                        byte[] data = Encoding.UTF8.GetBytes(json);
                        sslStream.Write(data, 0, data.Length);
                        //Console.WriteLine(json);
                        client.Close();
                        listener2.Stop();
                        break;
                    }
                }

            }
            //Debug.WriteLine("文件接收完成，保存为：" + filePath);
            //// 关闭连接
            //client2.Close();
        }
    }
    static void HandleFile_down(TcpClient client2, string request, TcpListener listener2,string filePath,long offset)
    {

        NetworkStream networkStream2 = client2.GetStream();
        SslStream sslStream = TLS.Build_SSL(client2, serverCertificate);

        using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            // 设置文件流偏移量
            fileStream.Seek(offset, SeekOrigin.Begin);
            byte[] buffer0 = new byte[buffersize];
            int bytesRead0;

            // 循环发送文件数据
            while ((bytesRead0 = fileStream.Read(buffer0, 0, buffer0.Length)) > 0)
            {
                // 将数据发送到服务器
                sslStream.Write(buffer0, 0, bytesRead0);
                // 更新断点记录
                offset += bytesRead0;
            }
        }
        Console.WriteLine("文件发送完成");

        // 关闭连接
        client2.Close();
        listener2.Stop();
    }
    static void HandleClient(TcpClient client)
    {
        string user = null;
        try
        {
            // 获取网络流对象
            NetworkStream stream = client.GetStream();
            SslStream sslStream = TLS.Build_SSL(client, serverCertificate);

            // 循环接收请求并发送回应
            while (true)
            {
                // 监听客户端请求
                byte[] buffer = new byte[1024];
                int bytesRead = sslStream.Read(buffer, 0, buffer.Length);
                string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                Console.WriteLine("接收到请求：" + request);
                string response;
                string code;

                try
                {
                    code = json_code(request);
                    if (code == "11")
                    {
                        string res;
                        user = Json_data<string>(request, "User");
                        string psw = Json_data<string>(request, "password");
                        bool lg = Lg_Reg(user, psw, "reg");
                        string json;
                        if (lg)  //找到说明用户名重复
                        {
                            res = "用户名重复，注册失败";
                        }
                        else
                        {
                            res = reg(user, psw);
                            string folderPath = serverroot + user; // 定义文件夹路径
                            if (!Directory.Exists(folderPath)) // 判断文件夹是否已经存在
                            {
                                Directory.CreateDirectory(folderPath); // 创建文件夹
                            }
                        }
                        var obj = new { sourse = "127.0.0.1", destination = "127.0.0.1", code = "12", mes = res };
                        json = JsonConvert.SerializeObject(obj);
                        byte[] data = Encoding.UTF8.GetBytes(json);
                        sslStream.Write(data, 0, data.Length);
                        Console.WriteLine("已发送请求：" + json);
                    }

                    else if (code == "21")
                    {
                        user = Json_data<string>(request, "User");
                        string psw = Json_data<string>(request, "password");
                        bool lg = Lg_Reg(user, psw, "login");
                        string json;
                        vip = Vip(user);
                        if (vip) buffersize = 40960;
                        else buffersize = 4096;

                        if (lg)
                        {
                            var obj = new { sourse = "127.0.0.1", destination = "127.0.0.1", code = "22" , vip = vip};//登录成功
                            json = JsonConvert.SerializeObject(obj);

                            string sql = "update Myuser set Online = 1 where UserID = '" + user + "'";
                            DataSet ds = Query(sql);

                            user_root = serverroot + user + "\\";


                        }
                        else
                        {
                            var obj = new { sourse = "127.0.0.1", destination = "127.0.0.1", code = "23" };//登陆失败
                            json = JsonConvert.SerializeObject(obj);
                        }

                        byte[] data = Encoding.UTF8.GetBytes(json);
                        sslStream.Write(data, 0, data.Length);
                        Console.WriteLine("已发送回应：" + json);
                        //stream.Close();
                    }

                    else if (code == "24")
                    {
                        var obj = new { sourse = "127.0.0.1", destination = "127.0.0.1", code = "25" };//退出成功
                        string json = JsonConvert.SerializeObject(obj);
                        byte[] data = Encoding.UTF8.GetBytes(json);
                        sslStream.Write(data, 0, data.Length);
                        Console.WriteLine("已发送回应：" + json);
                        string sql = "update Myuser set Online = 0 where UserID = '" + user + "'";
                        DataSet ds = Query(sql);
                        stream.Close();
                        break;
                    }
                    else if (code == "31")
                    {
                        var tal = FindFreePort(10000);
                        int port0 = tal.Item1;
                        TcpListener listener2 = tal.Item2;
                        long offset = Json_data<long>(request, "offest");
                        string filePath = json_path(request);
                        FileInfo fileInfo = new FileInfo(filePath);
                        long len = fileInfo.Length;

                        var obj = new { sourse = "127.0.0.1", destination = "127.0.0.1", code = "33", port = port0,len= len };
                        string json = JsonConvert.SerializeObject(obj);
                        byte[] data = Encoding.UTF8.GetBytes(json);
                        sslStream.Write(data, 0, data.Length);
                        Console.WriteLine("已发送请求：" + json);
                        //TcpListener listener2 = new TcpListener(IPAddress.Parse(serverIP), port0);
                        listener2.Start();
                        Console.WriteLine("文件传输端口开放，等待客户端连接...");
                        TcpClient client2 = listener2.AcceptTcpClient();
                        Console.WriteLine("客户端已连接");

                        try
                        {
                            Task.Run(() => HandleFile_down(client2, request, listener2, filePath,offset));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }

                       

                    }
                    else if (code == "32")
                    {
                        var tal = FindFreePort(10000);
                        int port0 = tal.Item1;
                        TcpListener listener2 = tal.Item2;
                        var obj = new { sourse = "127.0.0.1", destination = "127.0.0.1", code = "33", port = port0 };
                        string json = JsonConvert.SerializeObject(obj);
                        byte[] data = Encoding.UTF8.GetBytes(json);
                        sslStream.Write(data, 0, data.Length);
                        Console.WriteLine("已发送回应：" + json);

                        //TcpListener listener2 = new TcpListener(IPAddress.Parse(serverIP), port0);
                        listener2.Start();
                        Console.WriteLine("文件传输端口开放，等待客户端连接...");
                        TcpClient client2 = listener2.AcceptTcpClient();
                        Console.WriteLine("客户端已连接");
                        try
                        {
                            Task.Run(() => HandleFile(client2, request, listener2));
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                        
                    }
                    
                    else if (code == "41")
                    {
                        string pth = Json_data<string>(request, "path");
                        string name = Json_data<string>(request, "name");
                        string new_filename = name + Path.GetExtension(pth);
                        // 使用File.Move方法重命名文件
                        if (pth.EndsWith("\\"))
                        {
                            Directory.Move(pth, Path.Combine(Path.GetDirectoryName( pth.Substring( 0,pth.LastIndexOf("\\") )  ), name+"\\"));
                        }
                        else
                        {
                            File.Move(pth, Path.Combine(Path.GetDirectoryName(pth), new_filename));
                        }

                        
                        var obj = new { sourse = "127.0.0.1", destination = "127.0.0.1", code = "42" };
                        string json = JsonConvert.SerializeObject(obj);
                        byte[] data = Encoding.UTF8.GetBytes(json);
                        sslStream.Write(data, 0, data.Length);
                    }
                    else if (code == "43")
                    {
                       
                        string pth = Json_data<string>(request, "path");
                        if (pth.EndsWith("\\"))
                        {
                            Directory.Delete(pth);
                        }
                        else
                        {
                            File.Delete(pth);
                        }

                        var obj = new { sourse = "127.0.0.1", destination = "127.0.0.1", code = "44" };
                        string json = JsonConvert.SerializeObject(obj);
                        byte[] data = Encoding.UTF8.GetBytes(json);
                        sslStream.Write(data, 0, data.Length);
                    }
                    else if (code == "51")      //文件列表传输
                    {

                        List<string> fileTree = GetFileTree(user_root);
                        var obj = new { sourse = "127.0.0.1", destination = "127.0.0.1", code = "52", pth = user_root };
                        string json = JsonConvert.SerializeObject(obj);
                        byte[] data = Encoding.UTF8.GetBytes(json);
                        //Console.WriteLine(json);
                        //byte[] item_msg = Encoding.UTF8.GetBytes(item);
                        Array.Resize(ref data, 1024);
                        sslStream.Write(data, 0, data.Length);
                        //StreamWriter writer = new StreamWriter(stream);
                        foreach (string item in fileTree)
                        {
                            obj = new { sourse = "127.0.0.1", destination = "127.0.0.1", code = "52", pth = item };
                            json = JsonConvert.SerializeObject(obj);
                            data = Encoding.UTF8.GetBytes(json);
                            //byte[] item_msg = Encoding.UTF8.GetBytes(item);
                            Array.Resize(ref data, 1024);
                            sslStream.Write(data, 0, data.Length);
                        }
                        var obj2 = new { sourse = "127.0.0.1", destination = "127.0.0.1", code = "53",pth="" };//扫描完毕
                        string json2 = JsonConvert.SerializeObject(obj2);
                        byte[] data2 = Encoding.UTF8.GetBytes(json2);
                        sslStream.Write(data2, 0, data2.Length);
                    }
                    else if (code == "54")
                    {
                        string dir_name = Json_data<string>(request, "dir_name");
                        string mes = null;
                        if (Directory.Exists(dir_name))
                        {
                            mes = "文件夹已存在";
                        }
                        else try
                        {
                            // 创建文件夹
                            Directory.CreateDirectory(dir_name);
                            mes = "文件夹创建成功！";
                            Console.WriteLine("文件夹创建成功！");
                        }
                        catch (Exception ex)
                        {
                            mes = "文件夹创建失败"+ex.Message;
                            Console.WriteLine("文件夹创建失败： " + ex.Message);
                        }

                        var obj = new { sourse = "127.0.0.1", destination = "127.0.0.1", code = "33", mes = mes };
                        string json = JsonConvert.SerializeObject(obj);
                        byte[] data = Encoding.UTF8.GetBytes(json);
                        sslStream.Write(data, 0, data.Length);
                    }
                }

                catch (Exception e)
                {
                    Console.WriteLine("处理客户端连接异常1：" + e.Message);
                    if (user != null)
                    {
                        Console.WriteLine(user+"已退出");
                        string sql = "update Myuser set Online = 0 where UserID = '" + user + "'";
                        Query(sql);
                    }
                    client.Close();
                    break;
                }

                // 处理请求并生成回应
                // 关闭客户端连接
            }


        }
        catch (Exception e)
        {
            Console.WriteLine("处理客户端连接异常2：" + e.Message);
        }
    }
    static void Main()
    {
        try
        {
            int serverPort = 8080;
            TcpListener listener = new TcpListener(IPAddress.Parse(serverIP), serverPort);
            listener.Start();
            Console.WriteLine("服务器已启动，等待客户端连接...");
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("客户端已连接");
                Task.Run(() => HandleClient(client));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("服务器异常：" + e.Message);
        }

    }




}