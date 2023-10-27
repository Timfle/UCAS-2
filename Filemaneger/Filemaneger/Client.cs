using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Filemaneger
{
    internal class Client
    {
        public static  TcpClient Start_Client(string serverip,int serverport)
        {
                 NetworkStream stream = null;
                TcpClient client = new TcpClient();
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
                }
                catch (Exception e)
                {
                    Console.WriteLine("连接失败：" + e.Message);
                    MessageBox.Show("连接失败：" + e.Message);
                }
                return client;
            

        }

        public static string get_response(SslStream stream, string json_message)    //往stream数据流中对应的server port发送数据并监听回应
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(json_message);
                stream.Write(data, 0, data.Length);
                Debug.WriteLine("已发送请求：" + json_message);
                stream.ReadTimeout = 5000;
                byte[] buffer = new byte[1024];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Debug.WriteLine("接收到回应：" + response);
                return response;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}
