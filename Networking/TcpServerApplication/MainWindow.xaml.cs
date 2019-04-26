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

namespace TcpServerApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private event EventHandler<ServerStatusEventsArgs> OnServerStatusChanged;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OnServerStatusChanged += (senderObj, args) => SetControlsStatus(args.CurrentStatus);
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
        }

        private void Btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            OnServerStatusChanged?.Invoke(new object(), new ServerStatusEventsArgs(false));
        }
    }

    class ServerStatusEventsArgs : EventArgs
    {
        public ServerStatusEventsArgs(bool enabled) => CurrentStatus = enabled;
        public bool CurrentStatus { get; set; }
    }
}
