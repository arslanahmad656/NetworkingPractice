using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TcpClientApplication
{
    class TcpClientRunner : IDisposable
    {
        private int port;
        private TcpClient client;
        NetworkStream stream;

        public void Run()
        {
            GetPortFromUser();
            if (!InitializeClient())
            {
                return;
            }
            var messageReciever = Task.Run(() => StartReceivingIncomingMessages());
            messageReciever.Wait();
        }

        private void Close()
        {
            stream?.Close();
            stream?.Dispose();
            client?.Close();
            client?.Dispose();
        }

        private void StartReceivingIncomingMessages()
        {
            while (true)
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string message = reader.ReadToEnd();
                    if (message.Length > 0)
                    {
                        Console.WriteLine($"{Environment.NewLine}[Server at {DateTime.Now}] {message}");
                    }
                }

                Thread.Sleep(100);
            }
        }

        private bool InitializeClient()
        {
            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, port);
                client = new TcpClient(endPoint);
                stream = client.GetStream();
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
