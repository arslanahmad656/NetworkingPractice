using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace TcpApplicationFromTutorial
{
    public partial class FormMain : Form
    {
        private readonly IPAddress ipAddress;

        private StreamReader reader;
        private StreamWriter writer;
        private string messageToSend;
        private string receivedMessage;
        private TcpClient client;
        private int port;

        public FormMain()
        {
            InitializeComponent();

            ipAddress = Dns.GetHostAddresses(Dns.GetHostName())
                .First(a => a.AddressFamily == AddressFamily.InterNetwork);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            var address = ipAddress.ToString();
            TxtServerIP.Text = address;
            TxtClientIP.Text = address;
            Trace.Listeners.Add(new TextWriterTraceListener(File.Create("trace.log")));
            Trace.AutoFlush = true;
        }

        private void BtnServerStart_Click(object sender, EventArgs e)
        {
            try
            {
                var listener = new TcpListener(IPAddress.Any, 0);
                listener.Start();

                port = ((IPEndPoint)listener.LocalEndpoint).Port;
                WriteToSummary($"Server listening at {listener.LocalEndpoint}", MessageSource.Server);
                WriteLog($"Server listening at {listener.LocalEndpoint}");

                TxtServerPort.Text = port.ToString();
                TxtClientPort.Text = port.ToString();

                WriteLog($"Sever listening at {listener.LocalEndpoint}");

                client = listener.AcceptTcpClient();

                WriteToSummary("Client received", MessageSource.Server);

                reader = new StreamReader(client.GetStream());
                writer = new StreamWriter(client.GetStream());

                Task.Run(() => StartReceivingMessages());

                //backgroundWorker1.RunWorkerAsync();
                //backgroundWorker2.WorkerSupportsCancellation = true;
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        private void HandleError(Exception ex)
        {
            MessageBox.Show(ex.Message, "Error starting server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            var error = new StringBuilder();
            error.Append($"##Error {{{ex.GetType().FullName}}}## Message: {ex.Message}");
            error.Append($"{Environment.NewLine}Trace{Environment.NewLine}");
            error.Append(ex.StackTrace);
            
            while (ex.InnerException != null)
            {
                error.Append(Environment.NewLine);
                error.Append($"Inner exception: {{{ex.InnerException.GetType().FullName}}}: {ex.InnerException.Message}");
                error.Append(Environment.NewLine);
                error.Append("Stack trace:");
                error.Append(Environment.NewLine);
                error.Append(ex.InnerException.StackTrace);

                ex = ex.InnerException;
            }

            WriteLog(error.ToString());
            Environment.Exit(0);
        }

        private void WriteToSummary(string message, MessageSource source)
        {
            TxtSummary.Invoke((Action)(() => TxtSummary.AppendText($"[{source} at {DateTime.Now.ToLongTimeString()}]: {message}{Environment.NewLine}")));
        }

        private void WriteLog(string text, bool debugAlso = true)
        {
            var date = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            Trace.WriteLine($"[{date}] {text}");
            if (debugAlso)
            {
                Debug.WriteLine($"[{date}] {text}");
            }
        }

        private void BtnClientConnect_Click(object sender, EventArgs e)
        {
            try
            {
                var endpoint = new IPEndPoint(IPAddress.Parse(TxtClientIP.Text), int.Parse(TxtClientPort.Text));
                var client = new TcpClient();
                client.Connect(endpoint);
                if (client.Connected)
                {
                    WriteLog($"Client connected at {endpoint}");
                    WriteToSummary($"Connected at {endpoint}", MessageSource.Client);
                }
                else
                {
                    throw new Exception($"Could not connect to client at {endpoint}");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        private void StartReceivingMessages()
        {
            while (client.Connected)
            {
                try
                {
                    receivedMessage = reader.ReadLine();
                    if (receivedMessage != null)
                    {
                        WriteToSummary(receivedMessage, MessageSource.Client);
                    }
                }
                catch (Exception ex)
                {
                    HandleError(ex);
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            
        }
    }

    enum MessageSource
    {
        Server,
        Client
    }
}
