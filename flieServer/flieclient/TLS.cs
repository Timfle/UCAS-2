using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net;
using System.Net.Sockets;

namespace flieclient
{
    internal class TLS
    {
        private static readonly string CertificatePath = "E:\\Filemaneger\\Filemaneger\\Filemaneger\\src\\localhost.pfx";
        private static readonly string CertificatePassword = "lcs09190924";
        public class CertificateValidator
        {
            public static bool ValidateCertificate(X509Certificate2 certificate, X509Certificate2 rootCertificate)
            {
                try
                {
                    // 创建证书链对象
                    using (X509Chain chain = new X509Chain())
                    {
                        // 添加根证书到证书链
                        chain.ChainPolicy.ExtraStore.Add(rootCertificate);

                        // 建立证书链，并进行验证
                        bool isValid = chain.Build(certificate);

                        if (isValid)
                        {
                            // 验证成功
                            Console.WriteLine("证书验证成功");
                            return true;
                        }
                        else
                        {
                            // 验证失败，输出错误信息
                            Console.WriteLine("证书验证失败");

                            foreach (X509ChainStatus status in chain.ChainStatus)
                            {
                                Console.WriteLine($"链状态：{status.Status}，信息：{status.StatusInformation}");
                            }

                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"发生错误：{ex.Message}");
                    return false;
                }
            }
        }

        public static bool CustomCertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // 跳过吊销检查
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

            // 返回其他验证结果
            return sslPolicyErrors == SslPolicyErrors.None;
        }

        public static bool check_chain(X509Certificate2 certificate, X509Certificate2 rootCertificate)
        {
            ServicePointManager.ServerCertificateValidationCallback += CustomCertificateValidation;
            X509Chain chain = new X509Chain();
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            chain.Build(certificate);

            bool isChainValid = false;

            foreach (X509ChainElement element in chain.ChainElements)
            {
                if (element.Certificate.Subject == rootCertificate.Subject && element.Certificate.Issuer == rootCertificate.Issuer)
                {
                    isChainValid = true;
                    break;
                }
            }

            if (isChainValid)
            {
                Console.WriteLine("其他证书是根证书的子证书。");
                return true;
            }
            else
            {
                Console.WriteLine("其他证书不是根证书的子证书。");
                return false;
            }
        }

        public static SslStream Build_SSL(TcpClient client, X509Certificate2 serverCertificate)
        {
            try
            {
                SslStream sslStream = new SslStream(client.GetStream(), false);
                sslStream.AuthenticateAsServer(serverCertificate, false, SslProtocols.Tls, true);
                Console.WriteLine("SSL/TLS handshake completed successfully.");

                return sslStream;



            }
            catch (Exception ex)
            {
                Console.WriteLine("SSL/TLS handshake failed: " + ex.Message);
                return null;
            }
        }


    }
}
