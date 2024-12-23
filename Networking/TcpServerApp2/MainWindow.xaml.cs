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
using Microsoft.Win32;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;

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
        private SslStream sslStream;
        private X509Certificate2 certificate;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Trace.Listeners.Add(new TextWriterTraceListener(logFileName));
            Trace.AutoFlush = true;

            Dns.GetHostAddresses(Dns.GetHostName())
                .Where(address => address.AddressFamily == AddressFamily.InterNetwork)
                .ToList()
                .ForEach(address => Cmb_Address.Items.Add(address));
            Cmb_Address.SelectedIndex = 0;
            Chk_Tls.Focus();
        }

        private void Validate()
        {
            if (ipAddress == null)
            {
                throw new InvalidOperationException($"IP Address is not set.");
            }

            //if (port == 0) // port is dynamically allocated
            //{
            //    throw new InvalidOperationException($"Invalid Port {0}.");
            //}

            if (Chk_Tls.IsChecked == true)
            {
                if (string.IsNullOrWhiteSpace(Txt_CertificatePath.Text))
                {
                    throw new InvalidOperationException("Certificate path is required for TLS.");
                }

                if (string.IsNullOrWhiteSpace(Pwd_Certificate.Password))
                {
                    throw new InvalidOperationException("Certificate password is required for TLS.");
                }
            }
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
            Txt_CertificatePath.IsEnabled = false;
            Pwd_Certificate.IsEnabled = false;
            Btn_SelectCertificate.IsEnabled = false;
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
                    catch
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
            try
            {
                Validate();

                (sender as Button).IsEnabled = false;
                int.TryParse(Txt_Port.Text, out var portToAttempt);
                var listener = new TcpListener(ipAddress, portToAttempt);
                listener.Start();

                port = ((IPEndPoint)listener.LocalEndpoint).Port;
                Txt_Port.Text = port.ToString();

                await WriteToSummary($"Listening at {listener.LocalEndpoint}", MessageSource.Server);
                WriteLog("Server started listening at " + listener.LocalEndpoint);

                await Task.Run(() => client = listener.AcceptTcpClient());

                SetControlsStateForServerStarted();

                await WriteToSummary("Client received", MessageSource.Server);
                WriteLog("Client received");

                Stream streamToUse = client.GetStream();
                if (Chk_Tls.IsChecked == true)
                {
                    await WriteToSummary("Establishing TLS connection.", MessageSource.Server);
                    WriteLog("Establishing TLS connection.");

                    sslStream = new SslStream(streamToUse, false);

                    certificate = new X509Certificate2(Txt_CertificatePath.Text, Pwd_Certificate.Password);
                    await sslStream.AuthenticateAsServerAsync(certificate, false, SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12, true);

                    await WriteToSummary("TLS connection established.", MessageSource.Server);
                    WriteLog("TLS connection established.");

                    streamToUse = sslStream;
                }

                reader = new StreamReader(streamToUse);
                writer = new StreamWriter(streamToUse);

#pragma warning disable CS4014
                Task.Run(() => StartReceivingMessages());
#pragma warning restore

                Txt_Message.Focus();
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
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
            if (e.Key == Key.Enter)
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

        private void Chk_Tls_Checked(object sender, RoutedEventArgs e)
        {
            Txt_CertificatePath.IsEnabled = true;
            Pwd_Certificate.IsEnabled = true;
            Btn_SelectCertificate.IsEnabled = true;
        }

        private void Chk_Tls_Unchecked(object sender, RoutedEventArgs e)
        {
            Txt_CertificatePath.IsEnabled = false;
            Pwd_Certificate.IsEnabled = false;
            Btn_SelectCertificate.IsEnabled = false;
        }

        private void Btn_SelectCertificate_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Certificate Files|*.pfx"
            };

            if (dialog.ShowDialog() == true)
            {
                Txt_CertificatePath.Text = dialog.FileName;
            }
        }
    }

    enum MessageSource
    {
        Server,
        Client
    }
}
