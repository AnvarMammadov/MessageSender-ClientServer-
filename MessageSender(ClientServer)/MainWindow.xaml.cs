using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
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

namespace MessageSender_ClientServer_
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private TcpClient client;
        private StreamWriter writer;
        private string clientName;
        public MainWindow()
        {
            InitializeComponent();
        }


        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                client = new TcpClient();
                client.Connect("000.000.00.0", 27001); //ipaddress
                writer = new StreamWriter(client.GetStream()) { AutoFlush = true };

                clientName = clientNameTxt.Text;
                writer.WriteLine(clientName);

                clientNameTxt.Text = "";

                ReceiveMessages();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to server: {ex.Message}");
            }
        }

        private async void ReceiveMessages()
        {
            try
            {
                using (StreamReader reader = new StreamReader(client.GetStream()))
                {
                    while (true)
                    {
                        string message = await reader.ReadLineAsync();

                        int index = message.IndexOf(':');
                        if (index != -1 && index < message.Length - 1)
                        {
                            string senderName = message.Substring(0, index);
                            string actualMessage = message.Substring(index + 1);
                            Application.Current.Dispatcher.Invoke(() => messageListBox.Items.Add($"{senderName} - [{actualMessage.TrimStart()}]"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => MessageBox.Show($"Error receiving message: {ex.Message}"));
            }
        }
    }
}
