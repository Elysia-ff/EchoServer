using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPEchoServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            Task.Run(server.Start);

            Console.ReadLine();

            server.Stop();
        }
    }
}
