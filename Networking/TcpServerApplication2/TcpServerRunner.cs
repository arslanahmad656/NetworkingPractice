using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TcpApp
{
    class TcpServerRunner : IDisposable
    {
        private int port;
        private TcpListener server;

        public void Run()
        {
            GetPortFromUser();
            if (!InitializeServer())
            {
                return;
            }
            WaitForClient();
        }

        private void Close()
        {
            server?.Stop();
        }

        private void WaitForClient()
        {
            var client = server.AcceptTcpClient();
            Console.WriteLine($"Client received");
        }

        private bool InitializeServer()
        {
            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, port);
                server = new TcpListener(endPoint);
                server.Start();
                Console.WriteLine($"Server listening at {server.LocalEndpoint}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        private void GetPortFromUser()
        {
            Console.Write("Enter port (32000 - 45000): ");
            string text = Console.ReadLine();
            int result;
            while (!(int.TryParse(text, out result) && result >= 32000 && result <= 45000))
            {
                Console.Write($"Invalid Value. Enter between 32000 and 45000: ");
                text = Console.ReadLine();
            }

            port = result;
        }

        public void Dispose()
        {
            Close();
        }
    }
}
