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
            StreamWriter writer = new StreamWriter(client.GetStream(), Encoding.UTF8, 32, true);

            writer.WriteLine("First message");
            writer.WriteLine("Second message");
            writer.WriteLine("Third message");
            writer.Flush();

            Thread.Sleep(2000);

            writer.WriteLine("1 message");
            writer.WriteLine("2 message");
            writer.WriteLine("3 message");

            writer.Flush();
            writer.Close();

            Thread.Sleep(2000);

            writer = new StreamWriter(client.GetStream());
            writer.WriteLine("AGaing");

            writer.Flush();
            writer.Close();
            writer.Dispose();

            client.Close();
        }
    }
}
