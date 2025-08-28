using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EchoClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (TcpClient tcpClient = new TcpClient())
            {
                try
                {
                    tcpClient.Connect("127.0.0.1", 7777);
                }
                catch (SocketException ex)
                { 
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    tcpClient.Close();
                }
            }
        }
    }
}
