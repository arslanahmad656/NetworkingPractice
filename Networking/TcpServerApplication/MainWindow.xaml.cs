using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TcpServerApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private event EventHandler<ServerStatusEventsArgs> OnServerStatusChanged;

        private TcpListener server;
        private CancellationTokenSource tokenSource;
        private bool pendingMessageToSend;
        private string pendingMessage;

        private int oldPort;
        private const int minPortAllowed = 32000;
        private const int maxPortAllowed = 45000;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OnServerStatusChanged += (senderObj, args) => SetControlsStatus(args.CurrentStatus);
            oldPort = int.Parse(Txt_Port.Text);
        }

        private void SetControlsStatus(bool serverStarted)
        {
            Txt_Port.IsEnabled = !serverStarted;
            Btn_Start.IsEnabled = !serverStarted;
            Btn_Stop.IsEnabled = serverStarted;
            Txt_Message.IsEnabled = serverStarted;
            Btn_Send.IsEnabled = serverStarted;
        }

        private void Btn_Start_Click(object sender, RoutedEventArgs e)
        {
            OnServerStatusChanged?.Invoke(new object(), new ServerStatusEventsArgs(true));
            tokenSource = new CancellationTokenSource();
            Task.Run(() => StartServer(tokenSource.Token));
        }

        private async void Btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            OnServerStatusChanged?.Invoke(new object(), new ServerStatusEventsArgs(false));
            tokenSource.Cancel();
            server.Stop();
            await WriteToSummary($"Server stopped.", MessageSource.Server);
        }

        private void Txt_Port_LostFocus(object sender, RoutedEventArgs e)
        {
            var txtBox = sender as TextBox;
            var text = txtBox.Text;
            if (int.TryParse(text, out int result) && result >= minPortAllowed && result <= maxPortAllowed)
            {
                oldPort = result;
            }
            else
            {
                MessageBox.Show($"Invalid Port.{Environment.NewLine}Allowed ports: {minPortAllowed} - {maxPortAllowed}", "Invalid Port", MessageBoxButton.OK, MessageBoxImage.Error);
                txtBox.Text = oldPort.ToString();
            }
        }

        private async void StartServer(CancellationToken token)
        {
            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, oldPort);
                server = new TcpListener(endPoint);
                server.Start();
                await WriteToSummary($"Server started. Listening at {server.LocalEndpoint}", MessageSource.Server);
                try
                {
                    while (!token.IsCancellationRequested)
                    {
                        TcpClient client = server.AcceptTcpClient();
                        await WriteToSummary($"Client connected.", MessageSource.Server);
                        NetworkStream clientStream = client.GetStream();
                        if (server.Pending())
                        {
                            using (StreamReader reader = new StreamReader(clientStream))
                            {
                                string message = reader.ReadToEnd();
                                await WriteToSummary(message, MessageSource.Client);
                            }
                        }

                        if (pendingMessageToSend)
                        {
                            using (StreamWriter writer = new StreamWriter(clientStream))
                            {
                                writer.Write(pendingMessage ?? "");
                                writer.Flush();
                            }
                        }

                        await WriteToSummary(pendingMessage, MessageSource.Server);

                        client.Close();

                        Thread.Sleep(10);
                    }
                }
                catch (Exception ex)
                {
                    await WriteToSummary($"Error! Connections are no longer being accepted. Details: {ex.Message}.", MessageSource.Server);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "TCP server error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                server?.Stop();
                server = null;
            }
        }

        private async Task WriteToSummary(string message, MessageSource source)
        {
            await Dispatcher.BeginInvoke((Action)(() => Txt_Summary.AppendText($"[{source} ({DateTime.Now.ToLongTimeString()})]: {message}{Environment.NewLine}")));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            server?.Stop();
        }

        private void Btn_Send_Click(object sender, RoutedEventArgs e)
        {
            pendingMessageToSend = true;
            pendingMessage = Txt_Message.Text;
        }

        private void Txt_Summary_KeyUp(object sender, KeyEventArgs e)
        {
            if (Btn_Send.IsEnabled)
            {
                if (!string.IsNullOrWhiteSpace((sender as TextBox).Text))
                {
                    Btn_Send.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
            }
        }
    }

    class ServerStatusEventsArgs : EventArgs
    {
        public ServerStatusEventsArgs(bool enabled) => CurrentStatus = enabled;
        public bool CurrentStatus { get; set; }
    }

    enum MessageSource
    {
        Server,
        Client
    }
}
