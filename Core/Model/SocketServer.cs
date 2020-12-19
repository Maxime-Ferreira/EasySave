using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Core.Model
{
    public class SocketServer
    {
        private Socket _socket;
        private static readonly System.Object _lock = new System.Object();
        private readonly MultithreadingActions m = new MultithreadingActions();

        /// <summary>
        /// Constructor of SocketServer class.
        /// </summary>
        public SocketServer()
        {

        }

        /// <summary>
        /// Create the socket.
        /// </summary>
        public void CreateSocket()
        {
            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);
            _socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(remoteEP);
            _socket.Listen(10);
        }

        /// <summary>
        /// Accept the client connexion.
        /// </summary>
        public void AcceptConnexion()
        {
            _socket = _socket.Accept();
            Debug.WriteLine("client connecté");
        }

        /// <summary>
        /// Listen the socket to receive messages.
        /// </summary>
        public void ListenNetwork()
        {
            byte[] bytes = new byte[1024];
            while (true)
            {
                string message = "";
                if (_socket.Connected)
                {
                    int bytesRec = _socket.Receive(bytes);
                    message = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                }
                switch (message)
                {
                    case ("Play"):
                        m.Play();
                        break;
                    case ("Pause"):
                        m.Pause();
                        break;
                    case ("Stop"):
                        m.Stop();
                        break;
                    default:
                        string[] table = message.Split(' ');
                        switch (table[1])
                        {
                            case ("Play"):
                                m.Play(table[0]);
                                break;
                            case ("Pause"):
                                m.Pause(table[0]);
                                break;
                            case ("Stop"):
                                m.Stop(table[0]);
                                break;
                        }
                        break;
                }

            }
        }

        /// <summary>
        /// Send message to the client.
        /// </summary>
        /// <param name="saveWorkName">The save work name</param>
        /// <param name="progression">The save work progression</param>
        public void Send(string saveWorkName, string progression)
        {
            lock (_lock)
            {
                if (_socket.Connected)
                {
                    string message = saveWorkName + " " + progression;
                    _socket.Send(Encoding.ASCII.GetBytes(message));
                }
            }
        }

        /// <summary>
        /// Disconnect the socket.
        /// </summary>
        public void Disconnect()
        {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
    }
}
