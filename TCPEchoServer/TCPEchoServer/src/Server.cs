using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPEchoServer
{
    public class Server
    {
        public const int PORT = 8888;

        private List<ClientInstance> clients = new List<ClientInstance>();
        private object lockObj = new object();

        public async Task Start()
        {
            TcpListener tcpListener = null;

            try
            {
                tcpListener = new TcpListener(IPAddress.Any, PORT);
                tcpListener.Start();
            }
            catch (Exception e)
            {
                tcpListener?.Stop();

                Console.WriteLine($"[Error] {e}");
            }

            Console.WriteLine("[Info] Server started.");

            while (true)
            {
                try
                {
                    TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();

                    ClientInstance clientInstance = new ClientInstance(tcpClient, this);
                    lock (lockObj)
                    {
                        clients.Add(clientInstance);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[Error] {e}");
                }
            }
        }

        public void Stop()
        {
            lock (lockObj)
            {
                foreach (ClientInstance c in clients)
                {
                    c.Disconnect();
                }
            }
        }
    }
}
