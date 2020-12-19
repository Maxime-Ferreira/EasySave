using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

namespace ClientEasySave
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> list = new List<string>();
        private List<TextBlock> nameList = new List<TextBlock>();
        private List<TextBlock> progressionList = new List<TextBlock>();
        private SocketClient _socketClient;
        private bool clear = true;

        /// <summary>
        /// Constructor of the client
        /// </summary>
        public MainWindow()
        {
            _socketClient = new SocketClient(this);
            InitializeComponent();
        }

        /// <summary>
        /// Button play click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Play(object sender, RoutedEventArgs e)
        {
            _socketClient.Send("Play");
        }
        /// <summary>
        /// Button pause click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pause(object sender, RoutedEventArgs e)
        {
            _socketClient.Send("Pause");
        }
        /// <summary>
        /// Button stop click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Stop(object sender, RoutedEventArgs e)
        {
            _socketClient.Send("Stop");
        }

        /// <summary>
        /// Button to connect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectButton(object sender, RoutedEventArgs e)
        {
            _socketClient.Connect();
        }

        /// <summary>
        /// Button to disconnect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisconnectButton(object sender, RoutedEventArgs e)
        {
            _socketClient.Disconnect();
        }

        /// <summary>
        /// Send the name of the savework to play
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayClick(object sender, RoutedEventArgs e)
        {
            FrameworkElement parent = (FrameworkElement)((Button)sender).Parent;
            string parent_name = parent.Name;
            string message = parent_name + " Play";
            _socketClient.Send(message);
        }

        /// <summary>
        /// Send the name of the savework to pause
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PauseClick(object sender, RoutedEventArgs e)
        {
            FrameworkElement parent = (FrameworkElement)((Button)sender).Parent;
            string parent_name = parent.Name;
            string message = parent_name + " Pause";
            _socketClient.Send(message);
        }

        /// <summary>
        /// Send the name of the savework to stop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopClick(object sender, RoutedEventArgs e)
        {
            FrameworkElement parent = (FrameworkElement)((Button)sender).Parent;
            string parent_name = parent.Name;
            string message = parent_name + " Stop";
            _socketClient.Send(message);
        }

        /// <summary>
        /// edit the stack panel if it already exist
        /// </summary>
        /// <param name="name">savework's name</param>
        /// <param name="progression">progression of the save</param>
        /// <param name="number">number in the list</param>
        private void ChangeStackPanel(string name, string progression, int number)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                //Debug.WriteLine(nameList[number].Name);
                nameList[number].Text = name;
                progressionList[number].Text = progression;
            }));
        }

        /// <summary>
        /// Display the content of the save
        /// </summary>
        /// <param name="message">The message receive</param>
        public void Display(string message)
        {
            string[] table = message.Split(' ');
            //Debug.WriteLine("Recu : " + table[0] + " " + table[1]);
            if (table[1] == "begin" && clear)
            {
                list.Clear();
                nameList.Clear();
                progressionList.Clear();
                ClearView();
                clear = false;
            }

            int number = IsInList(table[0]);
            if (number == -1)
            {
                AddStackPanel(table[0], table[1], _socketClient);
            }
            else
            {
                ChangeStackPanel(table[0], table[1], number);
            }

            if (table[1] == "End")
            {
                clear = true;
            
            }
        }

        /// <summary>
        /// Create list with name of the saveworks
        /// </summary>
        /// <param name="name">Savework name</param>
        /// <returns></returns>
        private int IsInList(string name)
        {
            int i = 0;
            foreach (string item in list)
            {
                if (item == name)
                {
                    return i;
                }
                i++;
            }
            list.Add(name);
            return -1;
        }

        /// <summary>
        /// Create stack panel to display all contents of the save
        /// </summary>
        /// <param name="name">name of the savework</param>
        /// <param name="progression">current progression</param>
        /// <param name="scl">instance of socketclient</param>
        private void AddStackPanel(string name, string progression, SocketClient scl)
        {
            System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
            {
                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;
                stackPanel.Margin = new Thickness(0, 0, 2, 2);
                stackPanel.Name = name;
                stackPanelParent.Children.Add(stackPanel);
                AddTextBlock(name, "name", stackPanel, scl, "Name");
                AddTextBlock(name, "progression", stackPanel, scl, "Progression");
                //AddButton("play", stackPanel);
                //AddButton("pause", stackPanel);
                //AddButton("stop", stackPanel);
            });
        }

        /// <summary>
        /// Create textblock where there will be teh name of the savework and the progression
        /// </summary>
        /// <param name="name">Save work name</param>
        /// <param name="type">Save work type</param>
        /// <param name="stackPanel">The stack pannel parent</param>
        /// <param name="scl">The socket client for binding</param>
        /// <param name="valueToBind">The value to bind</param>
        private void AddTextBlock(string name, string type, StackPanel stackPanel, SocketClient scl, string valueToBind)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.Margin = new Thickness(30, 30, 0, 0);
            textBlock.Name = name + type;
            System.Windows.Data.Binding binding = new System.Windows.Data.Binding(valueToBind);
            binding.Source = scl;
            //textBlock.DataContext = this;
            //textBlock.SetBinding(System.Windows.Controls.TextBlock.TextProperty, binding);
            stackPanel.Children.Add(textBlock);
            if (type == "name")
            {
                nameList.Add(textBlock);
            }
            else
            {
                progressionList.Add(textBlock);
            }
        }

        /// <summary>
        /// Add buttons in the interface
        /// </summary>
        /// <param name="type">Type of the button</param>
        /// <param name="stackPanel">Stackpanel element</param>
        private void AddButton(string type, StackPanel stackPanel)
        {
            Button button = new Button();
            button.Content = type;
            if (type == "pause")
            {
                button.Click += PauseClick;
            }
            else if (type == "play")
            {
                button.Click += PlayClick;
            }
            else
            {
                button.Click += StopClick;
            }
            
            button.Width = 108;
            button.Height = 24;
            button.Margin = new Thickness(30, 30, 0, 0);
            stackPanel.Children.Add(button);
        }
        
        /// <summary>
        /// Clear the stack panel
        /// </summary>
        private void ClearView()
        {
            Dispatcher.Invoke(new Action(() =>
            {
                stackPanelParent.Children.Clear();
            }));
        }
    }
}