using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDPEchoServer
{
    public class Server
    {
        private const int PORT = 8888;

        private UdpClient udpClient = new UdpClient(PORT);

        public void Start()
        {
            Console.WriteLine("[Info] Server started.");

            Task.Run(ReceiveProcess);
        }

        private void ReceiveProcess()
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                try
                {
                    byte[] buffer = udpClient.Receive(ref ipEndPoint);

                    udpClient.Send(buffer, buffer.Length, ipEndPoint);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[Error] {e}");
                }
            }
        }
    }
}
