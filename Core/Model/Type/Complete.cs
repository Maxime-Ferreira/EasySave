using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;

namespace Core.Model.Type
{
    public class Complete : SaveWorkType, ISaveWorkType
    {
        private readonly string _type = "complete";
        private readonly string _name;
        private readonly List<string> _priorityFilesList = new List<string>();
        private readonly List<string> _notPriorityFilesList = new List<string>();
        private readonly StateFileContent _stateFileContent = new StateFileContent();
        private readonly BusinessSoft _business = BusinessSoft.GetInstance();
        public static bool isPaused = false;
        public static bool isStop = false;


        /// <summary>
        /// Get the type of the savework
        /// </summary>
        /// <returns> The type of the savework</returns>
        public new string GetType()
        {
            return _type;
        }

        /// <summary>
        /// Set _name attribute.
        /// </summary>
        /// <param name="name"></param>
        public Complete(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Copy all the folders between source folder and destination folder.
        /// Extensions list set to null is a non encrypt differential
        /// extensions list not empty is a encrypt differential.
        /// </summary>
        /// <param name="srcFolder">source folder</param>
        /// <param name="dstFolder">destination folder</param>
        /// <param name="extensions">The list of cryptosoft extensions</param>
        /// <param name="priorityExtensions">The list of priority extensions</param>
        /// <param name="barrier"></param>
        public override void SaveData(string srcFolder, string dstFolder, List<string> extensions, List<string> priorityExtensions, Barrier barrier)
        {
            _priorityFilesList.Clear();
            _notPriorityFilesList.Clear();
            _stateFileContent.ResetCount();

            _stateFileContent.InitStateFile(_name, srcFolder, dstFolder);

            string dstFolderPath = CreateCompleteFolderPath(dstFolder);

            Uri srcPath = new Uri(srcFolder, UriKind.Absolute);
            if (extensions == null)
            {
                if (Directory.GetDirectories(srcFolder, "*", SearchOption.AllDirectories) == null || Directory.GetDirectories(srcFolder, "*", SearchOption.AllDirectories).Length == 0)
                {
                    string srcFolderPath = new DirectoryInfo(srcFolder).Name;
                    string dstPath = GetCombinedPath(dstFolderPath, srcFolderPath);
                    Directory.CreateDirectory(dstPath);
                }
                else
                {
                    foreach (string dir in Directory.GetDirectories(srcFolder, "*", SearchOption.AllDirectories))
                    {
                        Uri srcSubPath = new Uri(dir, UriKind.Absolute);
                        string relPath = GetRelativePath(srcPath, srcSubPath);
                        string dstPath = GetCombinedPath(dstFolderPath, relPath);
                        Directory.CreateDirectory(Uri.UnescapeDataString(dstPath));
                    }
                }

                foreach (string filePath in Directory.GetFiles(srcFolder, "*", SearchOption.AllDirectories))
                {
                    SortFiles(priorityExtensions, filePath);
                }
            }
            else
            {
                if (Directory.GetDirectories(srcFolder, "*", SearchOption.AllDirectories) == null || Directory.GetDirectories(srcFolder, "*", SearchOption.AllDirectories).Length == 0)
                {
                    string srcFolderPath = new DirectoryInfo(srcFolder).Name;
                    string dstPath = GetCombinedPath(dstFolderPath, srcFolderPath);
                    Directory.CreateDirectory(dstPath);
                }
                else
                {
                    foreach (string dir in Directory.GetDirectories(srcFolder, "*", SearchOption.AllDirectories))
                    {
                        Uri srcSubPath = new Uri(dir, UriKind.Absolute);
                        string relPath = GetRelativePath(srcPath, srcSubPath);
                        string dstPath = GetCombinedPath(dstFolderPath, relPath);
                        Directory.CreateDirectory(Uri.UnescapeDataString(dstPath));
                    }
                }
                foreach (string filePath in Directory.GetFiles(srcFolder, "*", SearchOption.AllDirectories))
                {
                    SortFiles(priorityExtensions, filePath);
                }
            }

            SavePriorityFiles(srcPath, dstFolderPath, _stateFileContent, extensions);
            barrier.SignalAndWait();
            SaveNonPriorityFiles(srcPath, dstFolderPath, _stateFileContent, extensions);
            _stateFileContent.SetEndState(_name, srcFolder, dstFolder);
            isStop = false;
        }

        /// <summary>
        /// Sorts a file in the priority file list or not priority file list.
        /// </summary>
        /// <param name="priorityExtensions">List of priority extensions</param>
        /// <param name="filePath">The file we want to sort</param>
        public void SortFiles(List<string> priorityExtensions, string filePath)
        {
            bool isSorted = false;
            if (priorityExtensions.Count != 0)
            {
                foreach (string extension in priorityExtensions)
                {
                    if (isSorted)
                    {
                        break;
                    }
                    else if (Path.GetExtension(filePath).Equals(extension))
                    {
                        _priorityFilesList.Add(filePath);
                        isSorted = true;
                    }
                    else
                    {
                        if (!_notPriorityFilesList.Contains(filePath) && (priorityExtensions.IndexOf(extension) == priorityExtensions.Count - 1))
                        {
                            _notPriorityFilesList.Add(filePath);
                        }

                    }
                }
            }
            else
            {
                _notPriorityFilesList.Add(filePath);
            }
        }

        /// <summary>
        /// Save files contained in the priority files list, and crypt them if needed.
        /// </summary>
        /// <param name="dstFolder"></param>
        /// <param name="srcPath">The path of the source folder</param>
        /// <param name="dstFolderPath">The path of the destination folder</param>
        /// <param name="stateFileContent">The instance of StateFileContent class used</param>
        /// <param name="extensions">List of cryptosoft extensions</param>
        public void SavePriorityFiles(Uri srcPath, string dstFolderPath, StateFileContent stateFileContent, List<string> extensions)
        {
            foreach (string filePath in _priorityFilesList)
            {
                if (!isStop)
                {
                    using FileStream fs = File.Open(filePath, FileMode.Open);
                    if (fs.Length >= LimitSizeTranfer.maxSize)
                    {
                        fs.Close();
                        fs.Dispose();
                        Monitor.Enter(LimitSizeTranfer._lock);
                        try
                        {
                            CopyData(filePath, srcPath, dstFolderPath, stateFileContent, extensions);
                        }
                        finally
                        {
                            Monitor.Exit(LimitSizeTranfer._lock);
                        }
                    }
                    else
                    {
                        fs.Close();
                        fs.Dispose();
                        CopyData(filePath, srcPath, dstFolderPath, stateFileContent, extensions);
                    }
                }
            }
        }

        /// <summary>
        /// Save files contained in the not priority files list, and crypt them if needed.
        /// </summary>
        /// <param name="dstFolder"></param>
        /// <param name="srcPath">The path of the source folder</param>
        /// <param name="dstFolderPath">The path of the destination folder</param>
        /// <param name="stateFileContent">The instance of StateFileContent class used</param>
        /// <param name="extensions">List of cryptosoft extensions</param>
        public void SaveNonPriorityFiles(Uri srcPath, string dstFolderPath, StateFileContent stateFileContent, List<string> extensions)
        {
            foreach (string filePath in _notPriorityFilesList)
            {
                if (!isStop)
                {
                    using FileStream fs = File.Open(filePath, FileMode.Open);
                    if (fs.Length >= LimitSizeTranfer.maxSize)
                    {
                        fs.Close();
                        Monitor.Enter(LimitSizeTranfer._lock);
                        try
                        {
                            CopyData(filePath, srcPath, dstFolderPath, stateFileContent, extensions);
                        }
                        finally
                        {
                            Monitor.Exit(LimitSizeTranfer._lock);
                        }
                    }
                    else
                    {
                        fs.Close();
                        CopyData(filePath, srcPath, dstFolderPath, stateFileContent, extensions);
                    }
                }
            }
        }

        /// <summary>
        /// Save data when we want to encrypt files.
        /// </summary>
        /// <param name="filePath">The file we copy</param>
        /// <param name="srcPath">The path of the source folder</param>
        /// <param name="dstFolderPath">The path of the destination folder</param>
        /// <param name="stateFileContent">The instance of StateFileContent class used</param>
        /// <param name="extensions">List of cryptosoft extensions</param>
        public void CopyData(string filePath, Uri srcPath, string dstFolderPath, StateFileContent stateFileContent, List<string> extensions)
        {
            Pause();
            Stop();
            _business.CheckBusinessSoft();
            int transferTime = 0;
            int encryptTime = 0;
            Stopwatch copyTime = new Stopwatch();


            Uri srcSubPath = new Uri(filePath, UriKind.Absolute);
            string relPath = GetRelativePath(srcPath, srcSubPath);
            string dstPath = Uri.UnescapeDataString(GetCombinedPath(dstFolderPath, relPath));
            if (extensions != null && extensions.Contains(Path.GetExtension(filePath)))
            {
                encryptTime = EncryptFile(filePath, Path.GetDirectoryName(Uri.UnescapeDataString(dstPath)));
            }
            else
            {
                copyTime.Start();
                File.Copy(Uri.UnescapeDataString(filePath), dstPath);
                copyTime.Stop();
                transferTime = copyTime.Elapsed.Milliseconds;
            }

            stateFileContent.Update(_name, filePath, dstPath, transferTime, encryptTime);

        }

        /// <summary>
        /// Return the actual date and hour in a string format DD.MM.YYYY_HH.HH .
        /// </summary>
        /// <returns>Date of the save windows folder's format</returns>
        private string GetFolderName()
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("fr-FR");
            return String.Concat(DateTime.Now.ToString("g", culture).Replace("/", ".").Replace(" ", "_").Replace(":", "H"), "s", DateTime.Now.Second);
        }

        /// <summary>
        /// Create and return the path of the folder to save.
        /// </summary>
        /// <param name="dstFolder">destination folder</param>
        /// <returns></returns>
        private string CreateCompleteFolderPath(string dstFolder)
        {
            string saveDate = GetFolderName();
            string completePath = GetCombinedPath(dstFolder, saveDate);
            Directory.CreateDirectory(completePath); //create a new folder with the date as name
            return completePath;
        }

        /// <summary>
        /// Returns the stateFileContent linked with the save.
        /// </summary>
        /// <returns>The stateFileContent</returns>
        public override StateFileContent GetStateFileContent()
        {
            return _stateFileContent;
        }

        /// <summary>
        /// Pause the saveWork.
        /// </summary>
        public void Pause()
        {
            while (isPaused)
            {
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Stop the savework.
        /// </summary>
        public void Stop()
        {
            /*            if (isStopped)
                        {
                            cts.Dispose();
                            //Task.Dispose();
                        }*/
        }
    }
}
 