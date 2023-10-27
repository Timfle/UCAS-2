using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Filemaneger
{
    internal class Ip_port
    {
        public string ip;
        public int port;

        public static Ip_port GetServerIPAndPort(TcpClient client)
        {
            IPEndPoint remoteEndPoint = (IPEndPoint)client.Client.RemoteEndPoint;
            string clientIp = remoteEndPoint.Address.ToString();
            int clientPort = remoteEndPoint.Port;

            Ip_port newc = new Ip_port();
            newc.ip = clientIp;
            newc.port = clientPort;
            return newc; 
        }
    }
}
