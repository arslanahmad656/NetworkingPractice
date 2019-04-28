using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace TcpServerApplication2
{
    class AnotherClient
    {
        public void Run()
        {
            Console.Write("Enter IP: ");
            IPAddress ip = IPAddress.Parse(Console.ReadLine());
            Console.Write("Enter port: ");
            int port = int.Parse(Console.ReadLine());

            IPEndPoint ep = new IPEndPoint(ip, port);
            TcpClient client = new TcpClient();
            client.Connect(ep);
            using (StreamWriter writer = new StreamWriter(client.GetStream()))
            {
                writer.Write("Hello from client");
            }
        }
    }
}
