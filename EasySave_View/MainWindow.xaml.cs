using Core.Model;
using EasySave_View.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace EasySave_View
{
    public partial class MainWindow : Window
    {
        private Grid actualGrid;
        private Grid previousGrid;
        public new string Language { get; set; }
        private readonly ViewModel _viewmodel;
        public string msgError = "Program is already running";
        public string msgExecuted = "Your savework has been executed";
        public string msgCreated = "Your savework has been created";
        public string msgDeleted = "Your savework has been deleted";
        public string msgPath = "The path of your job application is the following one?";
        public string msgExtension = "Your extension has been deleted";
        public string appPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..");
        private static readonly System.Object _lock = new System.Object();
        private int _displayGridRow = 0;

        /// <summary>
        /// Intialize the graphical view.
        /// </summary>
        public MainWindow()
        {
            if (StartCheck())
            {
                _viewmodel = new ViewModel(this);
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                InitializeComponent();
                _viewmodel.StartSocketServer();
                actualGrid = (Grid)FindName("run");
                ChooseSaveWork(listSaveWorks);
            }
            else
            {
                System.Windows.MessageBox.Show(msgError);
                System.Windows.Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Checks if the application is already running.
        /// </summary>
        /// <returns>False the application is already running, true otherwise</returns>
        private bool StartCheck()
        {
            return (Process.GetProcessesByName("EasySave_View").Length == 1);
        }

        /// <summary>
        /// Show the run grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Run(object sender, RoutedEventArgs e)
        {
            SwitchVisibility("run");
            ChooseSaveWork(listSaveWorks);
        }

        /// <summary>
        /// Show the create grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Create(object sender, RoutedEventArgs e)
        {
            SwitchVisibility("create");
            cmbType.Items.Clear();
            cmbType.Items.Add("Complete");
            cmbType.Items.Add("Differential");
        }

        /// <summary>
        /// Show the delete grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete(object sender, RoutedEventArgs e)
        {
            listSaveWorksDelete.Items.Clear();
            SwitchVisibility("delete");
            ChooseSaveWork(listSaveWorksDelete);
        }

        /// <summary>
        /// Show the edit grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Edit(object sender, RoutedEventArgs e)
        {
            SwitchVisibility("edit");
        }

        /// <summary>
        /// Open and see the state file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void State(object sender, RoutedEventArgs e)
        {
            _viewmodel.SeeStateFile();
        }

        /// <summary>
        /// Open and see the log file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Log(object sender, RoutedEventArgs e)
        {
            _viewmodel.SeeLogFile();
        }

        /// <summary>
        /// Quit the software.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Quit(object sender, RoutedEventArgs e)
        {
            _viewmodel.Quit();
        }

        /// <summary>
        /// Choose a business soft.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Job(object sender, RoutedEventArgs e)
        {
            string pathJob;
            using (var fbd = new OpenFileDialog())
            {
                fbd.ShowDialog();
                pathJob = fbd.SafeFileName;
            }
            MessageBoxResult result = System.Windows.MessageBox.Show(msgPath + pathJob, "", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    _viewmodel.AddBusinessSoft(pathJob);
                    break;
                case MessageBoxResult.No:
                    Job(sender, e);
                    break;
                case MessageBoxResult.Cancel:
                    SwitchVisibility("run");
                    ChooseSaveWork(listSaveWorks);
                    break;
            }
        }

        /// <summary>
        /// Can switch between differents grids in out view.
        /// </summary>
        /// <param name="gridVisible">actual grid</param>
        private void SwitchVisibility(string gridVisible)
        {
            previousGrid = actualGrid;
            actualGrid = (Grid)FindName(gridVisible);
            previousGrid.Visibility = Visibility.Collapsed;
            actualGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Open the explorer to choose the folder source.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChoosePathSource(object sender, RoutedEventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            txtSource.Text = fbd.SelectedPath;
        }

        /// <summary>
        /// Open the explorer to choose the destination path.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChoosePathDestination(object sender, RoutedEventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            txtDest.Text = fbd.SelectedPath;
        }

        /// <summary>
        /// Create a savework.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateValidate(object sender, RoutedEventArgs e)
        {
            _viewmodel.CreateSaveWork(txtName.Text, txtSource.Text, txtDest.Text, cmbType.SelectedItem.ToString(), (bool)chkEncrypt.IsChecked);
            System.Windows.MessageBox.Show(msgCreated);
            SwitchVisibility("run");
            ChooseSaveWork(listSaveWorks);
        }

        /// <summary>
        /// About us grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AboutUs(object sender, RoutedEventArgs e)
        {
            SwitchVisibility("about");
        }

        /// <summary>
        /// Display all the actual saveworks we have.
        /// </summary>
        /// <param name="listBox">list to create or delete a savework</param>
        private void ChooseSaveWork(System.Windows.Controls.ListBox listBox)
        {
            listSaveWorks.Items.Clear();
            List<SaveWorkModel> SaveWorks = _viewmodel.RecoverSaveWorks();
            for (int i = 0; i < SaveWorks.Count; i++)
            {
                listBox.Items.Add(SaveWorks[i].name);
            }
            listBox.Items.Add("All the saveworks");
        }

        /// <summary>
        /// Validate to run or delete a savework.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChooseValidate(object sender, RoutedEventArgs e)
        {
            string action = actualGrid.Name;

            if (action == "run")
            {
                if (listSaveWorks.SelectedItem != null)
                {
                    SwitchVisibility("stateView");
                    state.Children.Clear();
                    _viewmodel.RunSaveWork(listSaveWorks.SelectedItem.ToString());
                }
            }
            else
            {
                _viewmodel.DeleteSaveWork(listSaveWorksDelete.SelectedItem.ToString());
                System.Windows.MessageBox.Show(msgDeleted);
            }
            ChooseSaveWork(listSaveWorks);
        }

        /// <summary>
        /// Grid cryptosoft.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Crypto(object sender, RoutedEventArgs e)
        {
            int counter = 0;
            string line;
            SwitchVisibility("crypto");
            System.IO.StreamReader file = new System.IO.StreamReader(Path.Combine(appPath, "Extensions", "extensions.txt"));
            while ((line = file.ReadLine()) != null)
            {
                listCrypto.Items.Add(line);
                counter++;
            }
            file.Close();

            UpdateExtension();
        }

        /// <summary>
        /// Grid to define priority files.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PriorityFiles(object sender, RoutedEventArgs e)
        {
            int counter = 0;
            string line;
            SwitchVisibility("priorityFiles");
            System.IO.StreamReader file = new System.IO.StreamReader(appPath + "\\Extensions\\extensions.txt");
            while ((line = file.ReadLine()) != null)
            {
                listPriority.Items.Add(line);
                counter++;
            }
            file.Close();

            UpdatePriorityExtension();
        }

        /// <summary>
        /// Grid to define maximum file size.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaximumFileSize(object sender, RoutedEventArgs e)
        {
            fileSize_lblSize.Content = $"{_viewmodel.RecoverMaximumFileSize()} bytes";
            SwitchVisibility("fileSize");
        }

        /// <summary>
        /// Can update listbox after adding or deleting an cryptosoft extension.
        /// </summary>
        public void UpdateExtension()
        {
            listExtension.Items.Clear();
            List<string> extensions = _viewmodel.RecoverExtensions("cryptosoft");
            for (int i = 0; i < extensions.Count; i++)
            {
                listExtension.Items.Add(extensions[i]);
            }
        }

        /// <summary>
        /// Can update listbox after adding or deleting an priority file extension.
        /// </summary>
        private void UpdatePriorityExtension()
        {
            listPriorityExtension.Items.Clear();
            List<string> extensions = _viewmodel.RecoverExtensions("priority");
            for (int i = 0; i < extensions.Count; i++)
            {
                listPriorityExtension.Items.Add(extensions[i]);
            }
        }

        /// <summary>
        /// Button to delete an extension.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteExtension(object sender, RoutedEventArgs e)
        {
            _viewmodel.DeleteCryptosoftExtension(listExtension.SelectedItem.ToString());
            System.Windows.MessageBox.Show(msgExtension);
            UpdateExtension();
        }

        /// <summary>
        /// Button to delete an extension.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeletePriorityExtension(object sender, RoutedEventArgs e)
        {
            _viewmodel.DeleteEntensionPriority(listPriorityExtension.SelectedItem.ToString());
            System.Windows.MessageBox.Show(msgExtension);
            UpdatePriorityExtension();
        }

        /// <summary>
        /// Add an extension to listExtensions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExtensionValidate(object sender, RoutedEventArgs e)
        {
            List<string> listExtensions = new List<string>();
            foreach (var item in listCrypto.SelectedItems)
            {
                listExtensions.Add(item.ToString());
            }
            if (!(txtExtension.Text == ""))
            {
                listExtensions.Add(txtExtension.Text);
            }
            _viewmodel.SetCryptosoftExtension(listExtensions);
            UpdateExtension();
        }

        /// <summary>
        /// Add an extension to listExtensions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExtensionPriorityValidate(object sender, RoutedEventArgs e)
        {
            List<string> listExtensions = new List<string>();
            foreach (var item in listPriority.SelectedItems)
            {
                listExtensions.Add(item.ToString());
            }
            if (!(txtExtension.Text == ""))
            {
                listExtensions.Add(txtExtension.Text);
            }
            _viewmodel.SetExtensionPriority(listExtensions);
            UpdatePriorityExtension();
        }

        /// <summary>
        /// Defines the maximum size file for simultaneous transfer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMaxSizeFile(object sender, RoutedEventArgs e)
        {
            string size = fileSize_txt.Text;
            _viewmodel.SetMaximumFileSize(size);
            fileSize_lblSize.Content = $"{size} bytes";
        }

        /// <summary>
        /// Display the advancement of save works.
        /// </summary>
        /// <param name="sender"></param>
        public void DisplayState(StateFileContent stfc)
        {
            lock (_lock)
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    System.Windows.Controls.Border border = CreateBorder(60, 800);
                    System.Windows.Controls.Grid grid = CreateGrid(2);
                    System.Windows.Controls.StackPanel leftStackPanel = CreateStackPanel(0, System.Windows.HorizontalAlignment.Left);
                    leftStackPanel.Children.Add(CreateTextBlock(20, stfc, "OutSaveworkName"));
                    leftStackPanel.Children.Add(CreateTextBlock(20, stfc, "OutType"));
                    leftStackPanel.Children.Add(CreateTextBlock(20, stfc, "OutProgression"));
                    /*              System.Windows.Controls.StackPanel rightStackPanel = CreateStackPanel(1, System.Windows.HorizontalAlignment.Right);
                                  leftStackPanel.Children.Add(CreateButton("play",20,20,10));
                                  leftStackPanel.Children.Add(CreateButton("pause", 50, 20, 10));
                                  leftStackPanel.Children.Add(CreateButton("stop", 50, 20, 10));*/
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    grid.Children.Add(leftStackPanel);
                    Grid.SetColumn(leftStackPanel, 0);
                    /*                    grid.Children.Add(rightStackPanel);
                                        Grid.SetColumn(rightStackPanel, 1);*/
                    border.Child = grid;
                    state.RowDefinitions.Add(new RowDefinition());
                    state.Children.Add(border);
                    Grid.SetRow(border, _displayGridRow);

                });
                _displayGridRow += 1;
            }
        }

        /// <summary>
        /// Call the .resx file for the english language.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void English(object sender, RoutedEventArgs e)
        {
            Grid createGrid = (Grid)FindName("create");
            Grid runGrid = (Grid)FindName("run");
            System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");
            Savework_to_run.Content = ResourceEN.Savework_to_run;
            header_run.Header = ResourceEN.header_run;
            header_config.Header = ResourceEN.header_config;
            header_create.Header = ResourceEN.header_create;
            header_delete.Header = ResourceEN.header_delete;
            header_edit.Header = ResourceEN.header_edit;
            job.Header = ResourceEN.header_job;
            header_logs.Header = ResourceEN.header_logs;
            header_logFile.Header = ResourceEN.header_logfile;
            header_stateFile.Header = ResourceEN.header_statefile;
            header_about.Header = ResourceEN.header_about;
            header_quit.Header = ResourceEN.header_quit;
            header_language.Header = ResourceEN.header_language;
            header_english.Header = ResourceEN.header_english;
            header_french.Header = ResourceEN.header_french;
            btnChooseValidate.Content = ResourceEN.btnChooseValidate;
            create_lbl.Content = ResourceEN.create_lbl;
            name_lbl.Content = ResourceEN.name_lbl;
            source_lbl.Content = ResourceEN.source_lbl;
            dest_lbl.Content = ResourceEN.dest_lbl;
            type_lbl.Content = ResourceEN.type_lbl;
            btnValidate.Content = ResourceEN.btnValidate;
            lblAbout.Text = ResourceEN.lblAbout;
            lblchoosedelete.Content = ResourceEN.lblchoosedelete;
            lblComing.Content = ResourceEN.lblComing;
            btnChooseValidateDelete.Content = ResourceEN.btnChooseValidateDelete;
            btnPriorityExtensionDelete.Content = ResourceEN.btnPriorityExtensionDelete;
            btnPriorityValidate.Content = ResourceEN.btnPriorityValidate;
            priority_lbl.Content = ResourceEN.priority_lbl;
            lblPriorityExtension.Content = ResourceEN.lblPriorityExtension;
            txtBlckSoftName.Text = ResourceEN.txtBlckSoftName;
            priority_header.Header = ResourceEN.priority_header;
            sizeFile_header.Header = ResourceEN.sizeFile_header;

            if (runGrid.Visibility == Visibility.Visible)
            {
                listSaveWorks.Items[^1] = ResourceEN.all_saveworks;
            }
            txtDest.Text = ResourceEN.txtDest;
            txtName.Text = ResourceEN.txtName;
            txtSource.Text = ResourceEN.txtSource;
            msgExecuted = ResourceEN.msgExecuted;
            msgCreated = ResourceEN.msgCreated;
            msgDeleted = ResourceEN.msgDeleted;
            msgPath = ResourceEN.msgPath;
            if (createGrid.Visibility == Visibility.Visible)
            {
                cmbType.Items[1] = "Differential";
            }
            lblExtension.Content = ResourceEN.lblExtension;
            btnCryptoValidate.Content = ResourceEN.btnCryptoValidate;
            crypto_lbl.Content = ResourceEN.crypto_lbl;
            msgExtension = ResourceEN.msgExtension;
            btnExtensionDelete.Content = ResourceEN.msgExtension;
        }
        /// <summary>
        /// Call the .resx file for the french language.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void French(object sender, RoutedEventArgs e)
        {
            Grid createGrid = (Grid)FindName("create");
            Grid runGrid = (Grid)FindName("run");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("fr-FR");
            Savework_to_run.Content = ResourceFR.Savework_to_run;
            header_run.Header = ResourceFR.header_run;
            header_config.Header = ResourceFR.header_config;
            header_create.Header = ResourceFR.header_create;
            header_delete.Header = ResourceFR.header_delete;
            header_edit.Header = ResourceFR.header_edit;
            job.Header = ResourceFR.header_job;
            header_logs.Header = ResourceFR.header_logs;
            header_logFile.Header = ResourceFR.header_logfile;
            header_stateFile.Header = ResourceFR.header_statefile;
            header_about.Header = ResourceFR.header_about;
            header_quit.Header = ResourceFR.header_quit;
            header_language.Header = ResourceFR.header_language;
            header_english.Header = ResourceFR.header_english;
            header_french.Header = ResourceFR.header_french;
            btnChooseValidate.Content = ResourceFR.btnChooseValidate;
            create_lbl.Content = ResourceFR.create_lbl;
            name_lbl.Content = ResourceFR.name_lbl;
            source_lbl.Content = ResourceFR.source_lbl;
            dest_lbl.Content = ResourceFR.dest_lbl;
            type_lbl.Content = ResourceFR.type_lbl;
            btnValidate.Content = ResourceFR.btnValidate;
            lblAbout.Text = ResourceFR.lblAbout;
            lblchoosedelete.Content = ResourceFR.lblchoosedelete;
            lblComing.Content = ResourceFR.lblComing;
            btnChooseValidateDelete.Content = ResourceFR.btnChooseValidateDelete;
            btnPriorityExtensionDelete.Content = ResourceFR.btnPriorityExtensionDelete;
            btnPriorityValidate.Content = ResourceFR.btnPriorityValidate;
            priority_lbl.Content = ResourceFR.priority_lbl;
            lblPriorityExtension.Content = ResourceFR.lblPriorityExtension;
            txtBlckSoftName.Text = ResourceFR.txtBlckSoftName;
            priority_header.Header = ResourceFR.priority_header;
            sizeFile_header.Header = ResourceFR.sizeFile_header;
            if (runGrid.Visibility == Visibility.Visible)
            {
                listSaveWorks.Items[^1] = ResourceFR.all_saveworks;
            }
            txtDest.Text = ResourceFR.txtDest;
            txtName.Text = ResourceFR.txtName;
            txtSource.Text = ResourceFR.txtSource;
            msgExecuted = ResourceFR.msgExecuted;
            msgCreated = ResourceFR.msgCreated;
            msgDeleted = ResourceFR.msgDeleted;
            msgPath = ResourceFR.msgPath;
            if (createGrid.Visibility == Visibility.Visible)
            {
                cmbType.Items[1] = "Differentielle";
            }
            lblExtension.Content = ResourceFR.lblExtension;
            btnCryptoValidate.Content = ResourceFR.btnCryptoValidate;
            crypto_lbl.Content = ResourceFR.crypto_lbl;
        }


        /// <summary>
        /// Create the grid to show the advancement of the save.
        /// </summary>
        /// <param name="column">The number of grid paramref name="column"</param>
        /// <returns>The grid with all properties</returns>
        public System.Windows.Controls.Grid CreateGrid(int column)
        {
            System.Windows.Controls.Grid grid = new System.Windows.Controls.Grid();
            Grid.SetColumn(grid, column);
            return grid;
        }

        /// <summary>
        /// Create the the stack panel to show the advancement of the save.
        /// </summary>
        /// <param name="nbColumn">The number of grid column</param>
        /// <param name="orientation">The orientation of the stack panel</param>
        /// <returns>The stack panel with all properties</returns>
        public System.Windows.Controls.StackPanel CreateStackPanel(int nbColumn, System.Windows.HorizontalAlignment orientation)
        {
            System.Windows.Controls.StackPanel stackPanel = new System.Windows.Controls.StackPanel
            {
                Orientation = System.Windows.Controls.Orientation.Horizontal,
                HorizontalAlignment = orientation
            };
            Grid.SetColumn(stackPanel, nbColumn);
            return stackPanel;
        }

        /// <summary>
        /// Create the the Text Block to show the advancement of the save.
        /// </summary>
        /// <param name="margin">The nargin of the text block</param>
        /// <param name="stfc">the state file content for binding</param>
        /// <param name="valueToBind">The value to bind</param>
        /// <returns>The text block with all properties</returns>
        public System.Windows.Controls.TextBlock CreateTextBlock(double margin, StateFileContent stfc, string valueToBind)
        {
            System.Windows.Data.Binding binding = new System.Windows.Data.Binding(valueToBind)
            {
                Source = stfc
            };
            System.Windows.Controls.TextBlock textBlock = new System.Windows.Controls.TextBlock
            {
                DataContext = this
            };
            textBlock.SetBinding(System.Windows.Controls.TextBlock.TextProperty, binding);
            textBlock.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            textBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            textBlock.Margin = new System.Windows.Thickness(margin, 0, margin, 0);
            return textBlock;
        }

        /// <summary>
        /// Create the the button play pause and stop.
        /// </summary>
        /// <param name="content">The content of the buton (play, pause, stop</param>
        /// <param name="height">The height of the button</param>
        /// <param name="width">The width of the button</param>
        /// <param name="margin">The margin of the button</param>
        /// <returns>The button with all properties</returns>
        public System.Windows.Controls.Button CreateButton(string content, int height, int width, int margin)
        {
            System.Windows.Controls.Button button = new System.Windows.Controls.Button
            {
                Content = content,
                Height = height,
                Width = width,
                Margin = new System.Windows.Thickness(margin, 0, margin, 0)
            };
            return button;
        }

        /// <summary>
        /// Create the the Border  to show the advancement of the save.
        /// </summary>
        /// <param name="height">The height of the border</param>
        /// <param name="width">The width of the border</param>
        /// <returns>The border with all properties</returns>
        public System.Windows.Controls.Border CreateBorder(int height, int width)
        {
            System.Windows.Controls.Border border = new System.Windows.Controls.Border
            {
                BorderThickness = new System.Windows.Thickness(1),
                BorderBrush = System.Windows.Media.Brushes.Gray,
                Height = height,
                Width = width
            };
            return border;
        }

        /// <summary>
        /// Pause all save works.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Pause(object sender, RoutedEventArgs e)
        {
            bool isPaused = false;
            _viewmodel.ThreadState(isPaused);
        }

        /// <summary>
        /// Play all save works.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Play(object sender, RoutedEventArgs e)
        {
            bool isPaused = true;
            _viewmodel.ThreadState(isPaused);
        }

        /// <summary>
        /// Stop all save works.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Stop(object sender, RoutedEventArgs e)
        {
            _viewmodel.ThreadStop();
        }
    }
}