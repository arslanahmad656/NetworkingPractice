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
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace TcpServerApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string logFileName = "server.log";

        private IPAddress ipAddress;
        private int port;
        private StreamReader reader;
        private StreamWriter writer;
        private TcpClient client;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Trace.Listeners.Add(new TextWriterTraceListener(logFileName));
            Trace.AutoFlush = true;
            //Txt_Address.Text = ipAddress.ToString();

            Dns.GetHostAddresses(Dns.GetHostName())
                .Where(address => address.AddressFamily == AddressFamily.InterNetwork)
                .ToList()
                .ForEach(address => Cmb_Address.Items.Add(address));
            Cmb_Address.SelectedIndex = 0;
            Cmb_Address.Focus();
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

        private void HandleError(Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private async Task WriteToSummary(string message, MessageSource source)
        {
            await Dispatcher.BeginInvoke((Action)(() => Txt_Summary.AppendText($"[{source} at {DateTime.Now.ToLongTimeString()}]: {message}{Environment.NewLine}")));
        }

        private void SetControlsStateForServerStarted()
        {
            Btn_Start.IsEnabled = false;
            Txt_Message.IsEnabled = true;
            Btn_Send.IsEnabled = true;
            Cmb_Address.IsEnabled = false;
        }

        private async void StartReceivingMessages()
        {
            try
            {
                while (client.Connected)
                {
                    try
                    {
                        var message = reader.ReadLine();
                        if (message?.Length > 0)
                        {
                            await WriteToSummary(message, MessageSource.Client);
                        }
                        await Task.Delay(10);
                    }
                    catch (Exception exToIgnore)
                    {

                    }
                }
                await WriteToSummary("Client disconnected", MessageSource.Server);
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        private async void Btn_Start_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            var listener = new TcpListener(ipAddress, 0);
            listener.Start();

            port = ((IPEndPoint)listener.LocalEndpoint).Port;
            Txt_Port.Text = port.ToString();

            await WriteToSummary($"Listening at {listener.LocalEndpoint}", MessageSource.Server);
            WriteLog("Server started listening at " + listener.LocalEndpoint);

            await Task.Run(() => client = listener.AcceptTcpClient());

            SetControlsStateForServerStarted();

            await WriteToSummary("Client received", MessageSource.Server);
            WriteLog("Client received");

            reader = new StreamReader(client.GetStream());
            writer = new StreamWriter(client.GetStream());

#pragma warning disable CS4014
            Task.Run(() => StartReceivingMessages());
#pragma warning restore

            Txt_Message.Focus();
        }

        private async void Btn_Send_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (client.Connected)
                {
                    var message = Txt_Message.Text;
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        writer.WriteLine(message);
                        writer.Flush();
                        await WriteToSummary(message, MessageSource.Server);
                        Txt_Message.Clear();
                    }
                }
                else
                {
                    throw new Exception("Client not connected");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S)
            {
                if (Btn_Start.IsEnabled)
                {
                    Btn_Start.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
            }
        }

        private void Txt_Message_KeyUp(object sender, KeyEventArgs e)
        {
            if (Btn_Send.IsEnabled)
            {
                if (e.Key == Key.Enter)
                {
                    if (!string.IsNullOrWhiteSpace((sender as TextBox).Text))
                    {
                        Btn_Send.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    }
                }
            }
        }

        private void Cmb_Address_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ipAddress = ((IEnumerable<object>)e.AddedItems).First() as IPAddress;
        }
    }

    enum MessageSource
    {
        Server,
        Client
    }
}
