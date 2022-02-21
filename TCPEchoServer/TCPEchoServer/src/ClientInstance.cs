using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPEchoServer
{
    public class ClientInstance
    {
        private TcpClient tcpClient;
        private Server server;

        private const int BUFFER_SIZE = 1024;

        public ClientInstance(TcpClient _tcpClient, Server _server)
        {
            tcpClient = _tcpClient;
            server = _server;

            Task.Run(ReceiveProcess);
        }

        public void Disconnect()
        {
            tcpClient?.Close();
        }

        private void ReceiveProcess()
        {
            byte[] packetSizeBuffer = new byte[sizeof(ushort)];
            byte[] buffer = new byte[BUFFER_SIZE];
            MemoryStream memoryStream = new MemoryStream();

            while (true)
            {
                try
                {
                    NetworkStream stream = tcpClient.GetStream();

                    // receive packet size 
                    for (int receivedBytesCount = 0; receivedBytesCount < packetSizeBuffer.Length;)
                    {
                        int expectedBytesCount = packetSizeBuffer.Length - receivedBytesCount;
                        int c = stream.Read(packetSizeBuffer, receivedBytesCount, expectedBytesCount);
                        if (c == 0) // disconnected 
                        {
                            goto DISCONNECTED;
                        }

                        receivedBytesCount += c;
                    }

                    // receive packet 
                    int packetSize = BitConverter.ToUInt16(packetSizeBuffer, 0) - packetSizeBuffer.Length;
                    for (int receivedBytesCount = 0; receivedBytesCount < packetSize;)
                    {
                        int expectedBytesCount = packetSize - receivedBytesCount < buffer.Length ? packetSize - receivedBytesCount : buffer.Length;
                        int c = stream.Read(buffer, 0, expectedBytesCount);
                        if (c == 0) // disconnected 
                        {
                            goto DISCONNECTED;
                        }

                        memoryStream.Write(buffer, 0, c);
                        receivedBytesCount += c;
                    }

                    // echo
                    byte[] sendBuffer = memoryStream.ToArray();
                    stream.Write(packetSizeBuffer, 0, packetSizeBuffer.Length);
                    stream.Write(sendBuffer, 0, sendBuffer.Length);

                    memoryStream.SetLength(0);
                }
                catch
                {
                    break;
                }
            }

DISCONNECTED:
            memoryStream.Dispose();
            Disconnect();
        }
    }
}
