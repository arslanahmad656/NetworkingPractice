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
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace TcpClientApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string logFileName = "client.log";

        private IPAddress ipAddress;
        private int port;
        private StreamReader reader;
        private StreamWriter writer;
        private TcpClient client;
        private SslStream sslStream;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Txt_Address_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.SelectAll();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Trace.Listeners.Add(new TextWriterTraceListener(logFileName));
            Trace.AutoFlush = true;
            ipAddress = Dns.GetHostAddresses(Dns.GetHostName()).First(address => address.AddressFamily == AddressFamily.InterNetwork);
            Txt_Address.Text = ipAddress.ToString();

            Chk_Tls.Focus();
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

        private async void Btn_Connect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                client = new TcpClient();
                var ep = new IPEndPoint(ipAddress, port);
                client.Connect(ep);

                if (client.Connected)
                {
                    SetControlsForState(connected: true);
                    WriteLog("Client connected");
                    Stream streamToUse = client.GetStream();
                    if (Chk_Tls.IsChecked == true)
                    {
                        WriteLog("Performing TLS handshake.");
                        await WriteToSummary("Performing TLS handshake.", MessageSource.Client);
                        sslStream = new SslStream(streamToUse, false, IsServerCertificateValid);
                        await sslStream.AuthenticateAsClientAsync(Txt_Address.Text, null, SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12, true);
                        WriteLog("TLS handshake completed.");
                        await WriteToSummary("TLS handshake completed.", MessageSource.Client);
                        streamToUse = sslStream;
                    }

                    reader = new StreamReader(streamToUse);
                    writer = new StreamWriter(streamToUse);

                    Txt_Message.Focus();
                    await WriteToSummary($"Connected to {ep}", MessageSource.Client);
#pragma warning disable CS4014
                    Task.Run(() => StartReceivingMessages());
#pragma warning restore
                }
                else
                {
                    throw new Exception($"Could not connect to {ep}");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        private static bool IsServerCertificateValid(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private void SetControlsForState(bool connected)
        {
            Btn_Connect.IsEnabled = !connected;
            Btn_Disconnect.IsEnabled = connected;
            Txt_Address.IsEnabled = !connected;
            Txt_Port.IsEnabled = !connected;
            Txt_Message.IsEnabled = connected;
            Btn_Send.IsEnabled = connected;
        }

        private void Btn_Disconnect_Click(object sender, RoutedEventArgs e)
        {
            //SetControlsForState(false);
        }

        private void Txt_Address_LostFocus(object sender, RoutedEventArgs e)
        {
            var textbox = sender as TextBox;
            try
            {
                ipAddress = IPAddress.Parse(textbox.Text);
            }
            catch
            {
                textbox.Text = ipAddress.ToString();
            }
        }

        private void Txt_Port_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void Txt_Port_LostFocus(object sender, RoutedEventArgs e)
        {
            var textbox = sender as TextBox;
            try
            {
                var value = int.Parse(textbox.Text);
                if (value < 0 || value > 65535)
                {
                    throw new Exception("Port out of range");
                }

                port = value;
            }
            catch
            {
                textbox.Text = port.ToString();
            }
        }

        private void ConnectOnKeyEvent(object sender, KeyEventArgs e)
        {
            if (Btn_Connect.IsEnabled)
            {
                if (e.Key == Key.Enter)
                {
                    var textbox = sender as TextBox;
                    textbox.RaiseEvent(new RoutedEventArgs(TextBox.LostFocusEvent));
                    Btn_Connect.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
            }
        }

        private void Btn_Send_KeyUp(object sender, KeyEventArgs e)
        {
            if (Btn_Send.IsEnabled)
            {
                if (e.Key == Key.Enter)
                {
                    Btn_Send.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
            }
        }

        private async void Btn_Send_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Txt_Message.Text))
            {
                writer.WriteLine(Txt_Message.Text);
                writer.Flush();
                await WriteToSummary(Txt_Message.Text, MessageSource.Client);
                Txt_Message.Clear();
            }
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
                            await WriteToSummary(message, MessageSource.Server);
                        }
                        await Task.Delay(10);
                    }
                    catch (Exception exToIgnore)
                    {

                    }
                }
                await WriteToSummary("Disconnected", MessageSource.Client);
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }
    }

    enum MessageSource
    {
        Server,
        Client
    }
}
