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
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;

namespace EmailClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string _SmtpServer = "SMTPSERVER";
        private const string _SmtpPort = "SMTPPORT";
        private const string _Ssl = "SSL";

        private const string _SmtpServerGmail = "smtp.gmail.com";
        private const int _SmtpPortGmail = 587;
        private const bool _SslEnabledGmail = true;

        private const string _SmtpServerOutlook = "smtp.office365.com";
        private const int _SmtpPortOutlook = 587;
        private const bool _SslEnabledOutlook = true;


        private readonly List<FrameworkElement> _serverSpecificControls;

        private string portValue;
        private string attachedFile;

        public MainWindow()
        {
            InitializeComponent();

            _serverSpecificControls = new List<FrameworkElement>
            {
                Txt_SmtpServer,
                Txt_SmtpPort,
                Chk_EnableSsl,
                Tb_Info
            };
        }

        private void PopulateServicesComboBox()
        {
            Cmb_EmailService.ItemsSource = new[]
            {
                EmailServices.GMail,
                EmailServices.Outlook,
                EmailServices.Other
            };

            Cmb_EmailService.SelectedItem = EmailServices.GMail;
        }

        private void SetDefaultsControlsStatus(bool enabled)
        {
            Txt_SmtpServer.IsReadOnly = enabled;
            Txt_SmtpPort.IsReadOnly = enabled;
            Chk_EnableSsl.IsEnabled = !enabled;
        }

        private async void Btn_SendEmail_Click(object sender, RoutedEventArgs e)
        {
            var (validated, reason) = ValidateParameters();

            if (!validated)
            {
                MessageBox.Show(reason, "Paramters incorrect", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var smtpServer = Txt_SmtpServer.Text;
            var port = int.Parse(Txt_SmtpPort.Text);
            var username = Txt_Username.Text;
            var password = Pwd_Password.Password;
            var from = Txt_From.Text;
            var to = Txt_To.Text.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var subject = Txt_Subject.Text;
            var body = Txt_Body.Text;
            var sslEnabled = Chk_EnableSsl.IsChecked ?? false;


            var client = new SmtpClient
            {
                Host = smtpServer,
                Port = port
            };

            if (sslEnabled)
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(username, password);
            }

            var message = new MailMessage
            {
                From = new MailAddress(from),
                Subject = subject,
                Body = body
            };

            to.ToList().ForEach(a => message.To.Add(new MailAddress(a)));

            await client.SendMailAsync(message);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PopulateServicesComboBox();
        }

        private void Cmb_EmailService_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearServerSpecificValues();

            var selectedItem = (EmailServices)((sender as ComboBox).SelectedItem);
            if (selectedItem != EmailServices.Other)
            {
                if (selectedItem == EmailServices.GMail)
                {
                    PopulateServerSpecificControls(_SmtpServerGmail, _SmtpPortGmail, _SslEnabledGmail);
                }
                else
                {
                    PopulateServerSpecificControls(_SmtpServerOutlook, _SmtpPortOutlook, _SslEnabledOutlook);
                }

                SetServerSpecificControlsStatus(@readonly: true);
            }
            else
            {
                SetServerSpecificControlsStatus(@readonly: false);
            }
        }

        private void SetServerSpecificControlsStatus(bool @readonly)
        {
            Txt_SmtpServer.IsReadOnly = @readonly;
            Txt_SmtpPort.IsReadOnly = @readonly;
            Chk_EnableSsl.IsEnabled = !@readonly;
        }

        private void PopulateServerSpecificControls(string server, int port, bool sslEnabled)
        {
            Txt_SmtpServer.Text = server;
            Txt_SmtpPort.Text = port.ToString();
            Chk_EnableSsl.IsChecked = sslEnabled;
        }

        private void ClearServerSpecificValues()
        {
            _serverSpecificControls.ForEach(c =>
            {
                if (c is TextBox)
                {
                    (c as TextBox).Clear();
                }
                else if (c is CheckBox)
                {
                    (c as CheckBox).IsChecked = false;
                }
            });
        }

        private void Txt_SmtpPort_GotFocus(object sender, RoutedEventArgs e)
        {
            var control = sender as TextBox;
            if (!control.IsReadOnly)
            {
                portValue = control.Text;
            }
        }

        private void Txt_SmtpPort_LostFocus(object sender, RoutedEventArgs e)
        {
            var control = sender as TextBox;
            if (!control.IsReadOnly)
            {
                if (!int.TryParse(control.Text, out int result))
                {
                    control.Text = portValue;
                }
            }
        }

        private void Txt_Username_LostFocus(object sender, RoutedEventArgs e)
        {
            var control = sender as TextBox;
            if (!ValidateEmail(control.Text))
            {
                control.Clear();
            }
        }

        private bool ValidateEmail(string email)
        {
            return Regex.IsMatch(email, @"(?i)[a-z].*@[a-z].*");
        }

        private void Txt_From_LostFocus(object sender, RoutedEventArgs e)
        {
            var control = sender as TextBox;
            if (!ValidateEmail(control.Text))
            {
                control.Clear();
            }
        }

        private void Txt_To_LostFocus(object sender, RoutedEventArgs e)
        {
            var control = sender as TextBox;
            if (!string.IsNullOrWhiteSpace(control.Text))
            {
                var emails = control.Text.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                var invalidEmails = new List<string>();

                foreach (var email in emails)
                {
                    if (!ValidateEmail(email))
                    {
                        invalidEmails.Add(email);
                    }
                }

                var validEmails = emails.Except(invalidEmails);
                var validEmailStr = string.Join(";", validEmails);
                control.Text = validEmailStr;
            }
        }

        private void Txt_CC_LostFocus(object sender, RoutedEventArgs e)
        {
            var control = sender as TextBox;
            if (!string.IsNullOrWhiteSpace(control.Text))
            {
                var emails = control.Text.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                var invalidEmails = new List<string>();

                foreach (var email in emails)
                {
                    if (!ValidateEmail(email))
                    {
                        invalidEmails.Add(email);
                    }
                }

                var validEmails = emails.Except(invalidEmails);
                var validEmailStr = string.Join(";", validEmails);
                control.Text = validEmailStr;
            }
        }

        private void Txt_BCC_LostFocus(object sender, RoutedEventArgs e)
        {
            var control = sender as TextBox;
            if (!string.IsNullOrWhiteSpace(control.Text))
            {
                var emails = control.Text.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                var invalidEmails = new List<string>();

                foreach (var email in emails)
                {
                    if (!ValidateEmail(email))
                    {
                        invalidEmails.Add(email);
                    }
                }

                var validEmails = emails.Except(invalidEmails);
                var validEmailStr = string.Join(";", validEmails);
                control.Text = validEmailStr;
            }
        }

        private void Chk_EnableSslCheckedChange(object sender, RoutedEventArgs e)
        {
            var control = sender as CheckBox;

            Txt_Username.Clear();
            Pwd_Password.Clear();
            Txt_Username.IsEnabled = control.IsChecked ?? false;
            Pwd_Password.IsEnabled = control.IsChecked ?? false;
        }

        private (bool validated, string reason) ValidateParameters()
        {
            if (string.IsNullOrWhiteSpace(Txt_SmtpServer.Text))
            {
                return (false, "SMTP server cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(Txt_SmtpServer.Text))
            {
                return (false, "SMTP port cannot be empty");
            }

            if (Chk_EnableSsl.IsChecked ?? false)
            {
                if (string.IsNullOrWhiteSpace(Txt_Username.Text))
                {
                    return (false, "Username cannot be empty with SSL enabled");
                }

                if (string.IsNullOrWhiteSpace(Pwd_Password.Password))
                {
                    return (false, "Password cannot be empty with SSL");
                }
            }

            if (string.IsNullOrWhiteSpace(Txt_From.Text))
            {
                return (false, "From address cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(Txt_To.Text))
            {
                return (false, "To address cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(Txt_Subject.Text))
            {
                return (false, "Subject cannot be empty");
            }

            if (!string.IsNullOrWhiteSpace(attachedFile))
            {
                if (!File.Exists(attachedFile))
                {
                    return (false, $"File {attachedFile} does not exist.");
                }
            }

            return (true, "");
        }
    }

    enum EmailServices
    {
        GMail,
        Outlook,
        Other
    }
}
