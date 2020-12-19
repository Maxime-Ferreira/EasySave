using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace ClientEasySave
{
    public class SocketClient : INotifyPropertyChanged
    {
        private Socket _socket;
        private MainWindow _mainWindow;

        public event PropertyChangedEventHandler PropertyChanged;

        private string _progression;
        private string _name;
        public string Progression
        {
            get
            {
                return _progression;
            }
            set
            {
                _progression = value;
                OnPropertyChanged("Progression");
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// Constructor of SocketClient.
        /// Connect to the server and create the thread to listen the soccket.
        /// </summary>
        /// <param name="mainWindow"></param>
        public SocketClient(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            if (Connect())
            {
                Thread receive = new Thread(() => { ListenNetwork(); });
                receive.Start();
            }
            else
            {
                Environment.Exit(0);
            }
        }

        protected void OnPropertyChanged(string stateFileContent)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(stateFileContent));
            }
        }

        /// <summary>
        /// Connect to the server
        /// </summary>
        public bool Connect()
        {
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);
            _socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                _socket.Connect(remoteEP);
            }
            catch (Exception e)
            {
                MessageBox.Show("Please run EasySave soft before opening client");
                
                return false;
            }
            return true;
            Debug.WriteLine("Connection success");
        }

        /// <summary>
        /// Listen the socket
        /// </summary>
        private void ListenNetwork()
        {
            byte[] bytes = null;
            bool test = true;
            while (test)
            {
                try
                {
                    bytes = new byte[1024];
                    int bytesRec = _socket.Receive(bytes);
                    string message = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    string[] table = message.Split(' ');
                    Name = table[0];
                    Progression = table[1];
                    _mainWindow.Display(message);
                }
                catch
                {
                    test = false;
                }

            }
        }

        /// <summary>
        /// Send a message to the server
        /// </summary>
        /// <param name="message">The message to send</param>
        public void Send(string message)
        {
            _socket.Send(Encoding.ASCII.GetBytes(message));
        }

        /// <summary>
        /// Disconnect the socket between the client and the server
        /// </summary>
        public void Disconnect()
        {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
    }
}
