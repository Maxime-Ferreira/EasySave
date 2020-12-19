using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Threading;

namespace Core.Model.Type
{
    public class Differential : SaveWorkType, ISaveWorkType
    {
        readonly string _type = "diff";
        private readonly string _name;
        private readonly List<string> _priorityFilesList = new List<string>();
        private readonly List<string> _notPriorityFilesList = new List<string>();
        private readonly StateFileContent _stateFileContent = new StateFileContent();
        private readonly BusinessSoft _business = BusinessSoft.GetInstance();
        public static bool isPaused = false;
        public static bool isStop = false;

        /// <summary>
        /// The constructor initialize the name the save work
        /// </summary>
        public Differential(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Get the type of the savework
        /// </summary>
        /// <returns> The type of the savework</returns>
        public new string GetType()
        {
            return _type;
        }

        /// <summary>
        /// Sorts files in priority files lists and run a differential save, 
        /// extensions list set to null is a non encrypt differential
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

            Uri sourcePath = new Uri(srcFolder, UriKind.Absolute); // instanciate as absolute path the sourcePath given by the controller

            if (Directory.GetDirectories(srcFolder, "*", SearchOption.AllDirectories) == null || Directory.GetDirectories(srcFolder, "*", SearchOption.AllDirectories).Length == 0)
            {
                string srcFolderPath = new DirectoryInfo(srcFolder).Name;
                string dstPath = GetCombinedPath(dstFolder, srcFolderPath);
                Directory.CreateDirectory(dstPath);
            }

            else
            {
                foreach (string dir in Directory.GetDirectories(srcFolder, "*", SearchOption.AllDirectories)) //Check if folder in source file exist in destination file, if not, create the folder
                {
                    Uri dirPath = new Uri(dir, UriKind.Absolute);
                    string relativeDirPath = GetRelativePath(sourcePath, dirPath);
                    string dstDir = GetCombinedPath(dstFolder, relativeDirPath);
                    if (Directory.Exists(dstDir))
                    {
                        Console.WriteLine(@"/ ! \  Folder " + dstDir + " already exist !");
                    }
                    else
                    {
                        Directory.CreateDirectory(Uri.UnescapeDataString(Path.Combine(dstFolder, dstDir)));
                    }
                }
            }

            foreach (string filePath in Directory.GetFiles(srcFolder, "*", SearchOption.AllDirectories))    // Decrypt all files in the srcfolder to compare them
            {
                if (Path.GetExtension(filePath).Contains(".cryptosoft"))
                {
                    EncryptFile(filePath, Path.GetDirectoryName(Uri.UnescapeDataString(filePath)));
                    File.Delete(filePath);
                }
            }
            foreach (string filePath in Directory.GetFiles(dstFolder, "*", SearchOption.AllDirectories))
            {
                if (Path.GetExtension(filePath).Contains(".cryptosoft"))
                {
                    EncryptFile(filePath, Path.GetDirectoryName(Uri.UnescapeDataString(filePath)));
                    File.Delete(filePath);
                }
            }


            List<string> changedFiles = new List<string>();
            List<string> changedDstFiles = new List<string>();
            List<string> sameFiles = new List<string>();
            List<string> newFiles = new List<string>();

            foreach (string srcFilePath in Directory.GetFiles(srcFolder, "*", SearchOption.AllDirectories))
            {
                Uri AbsFilePath = new Uri(srcFilePath, UriKind.Absolute);

                string relFilePath = GetRelativePath(sourcePath, AbsFilePath);
                string dstFilePath = Uri.UnescapeDataString(GetCombinedPath(dstFolder, relFilePath));
                if (File.Exists(dstFilePath)) // Check if the file exist, if true, check if modifications has been added between srcFile and the existing dstFile
                                              // replace the file if modification detected, else don't replace. If the file doesn't exist, created it
                {
                    using SHA256 mySHA256 = SHA256.Create();
                    // Get the FileInfo objects for every file in the directory
                    FileStream srcFileStream = File.Open(srcFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    FileStream dstFileStream = File.Open(dstFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                    srcFileStream.Position = 0;
                    dstFileStream.Position = 0;

                    // Compute the hash of the fileStream
                    byte[] srcFileHashByte = mySHA256.ComputeHash(srcFileStream);
                    byte[] dstFileHashByte = mySHA256.ComputeHash(dstFileStream);
                    var srcFileHash = System.Text.Encoding.Default.GetString(srcFileHashByte);
                    var dstFileHash = System.Text.Encoding.Default.GetString(dstFileHashByte);
                    srcFileStream.Close();
                    dstFileStream.Close();

                    if (!srcFileHash.Equals(dstFileHash))
                    {
                        changedFiles.Add(srcFilePath);
                    }
                    else
                    {
                        if (extensions != null && ExtensionIsEncryptExtension(extensions, srcFilePath))
                        {
                            sameFiles.Add(srcFilePath);
                            changedDstFiles.Add(dstFilePath);
                        }
                    }
                }
                else
                {
                    newFiles.Add(srcFilePath);
                }
                bool isSorted = false;
                if (priorityExtensions.Count != 0)
                {
                    foreach (string extension in priorityExtensions)
                    {
                        if (isSorted)
                        {
                            break;
                        }
                        else if (Path.GetExtension(srcFilePath).Equals(extension))
                        {
                            _priorityFilesList.Add(srcFilePath);
                            isSorted = true;
                        }
                        else
                        {
                            if (!_notPriorityFilesList.Contains(srcFilePath) && (priorityExtensions.IndexOf(extension) == priorityExtensions.Count - 1))
                            {
                                _notPriorityFilesList.Add(srcFilePath);
                            }

                        }
                    }
                }
                else
                {
                    _notPriorityFilesList.Add(srcFilePath);
                }

            }

            _stateFileContent.InitStateFile(_name, srcFolder, dstFolder, changedFiles, sameFiles, newFiles);

            RunSave(changedFiles, extensions, sourcePath, dstFolder, _stateFileContent, _priorityFilesList);
            RunSave(newFiles, extensions, sourcePath, dstFolder, _stateFileContent, _priorityFilesList);
            RunSave(sameFiles, extensions, sourcePath, dstFolder, _stateFileContent, _priorityFilesList);
            barrier.SignalAndWait();
            RunSave(changedFiles, extensions, sourcePath, dstFolder, _stateFileContent, _notPriorityFilesList);
            RunSave(newFiles, extensions, sourcePath, dstFolder, _stateFileContent, _notPriorityFilesList);
            RunSave(sameFiles, extensions, sourcePath, dstFolder, _stateFileContent, _notPriorityFilesList);
            _stateFileContent.SetEndState(_name, srcFolder, dstFolder);
            isStop = false;
        }

        /// <summary>
        /// Save a list of path in a destination folder wether the differential savework is encrypt or not.
        /// </summary>
        /// <param name="allFiles">The list of files saved</param>
        /// <param name="extensions">The list of cryptosoft extensions</param>
        /// <param name="sourcePath">The path of the source folder</param>
        /// <param name="dstFolder">The path of the destination folder</param>
        /// <param name="stateFileContent">The instance of StateFileContent class used</param>
        /// <param name="priorityList">The list of priority files</param>
        private void RunSave(List<string> allFiles, List<string> extensions, Uri sourcePath, string dstFolder, StateFileContent stateFileContent, List<string> priorityList)
        {
            foreach (string srcFile in allFiles)
            {
                if(!isStop)
                {
                    Pause();
                    _business.CheckBusinessSoft();
                    if (priorityList.Contains(srcFile))
                    {
                        using FileStream fs = File.Open(srcFile, FileMode.Open);
                        if (fs.Length >= LimitSizeTranfer.maxSize)
                        {
                            fs.Close();
                            Monitor.Enter(LimitSizeTranfer._lock);
                            try
                            {
                                Stopwatch copyTime = new Stopwatch();
                                int encryptionTime = 0;
                                int transferTime = 0;
                                Uri AbsFilePath = new Uri(srcFile, UriKind.Absolute);
                                string relFilePath = GetRelativePath(sourcePath, AbsFilePath);
                                string dstFilePath = Uri.UnescapeDataString(GetCombinedPath(dstFolder, relFilePath));

                                if (extensions != null && ExtensionIsEncryptExtension(extensions, dstFilePath))
                                {
                                    encryptionTime = EncryptFile(srcFile, Path.GetDirectoryName(Uri.UnescapeDataString(dstFilePath)));
                                }
                                else
                                {
                                    copyTime.Start();
                                    File.Copy(srcFile, dstFilePath);
                                    copyTime.Stop();
                                    transferTime = copyTime.Elapsed.Milliseconds;
                                }
                                stateFileContent.Update(_name, srcFile, dstFilePath, transferTime, encryptionTime);
                            }
                            finally
                            {
                                Monitor.Exit(LimitSizeTranfer._lock);
                            }
                        }
                        else
                        {
                            fs.Close();
                            Stopwatch copyTime = new Stopwatch();
                            int encryptionTime = 0;
                            int transferTime = 0;
                            Uri AbsFilePath = new Uri(srcFile, UriKind.Absolute);
                            string relFilePath = GetRelativePath(sourcePath, AbsFilePath);
                            string dstFilePath = Uri.UnescapeDataString(GetCombinedPath(dstFolder, relFilePath));

                            if (extensions != null && ExtensionIsEncryptExtension(extensions, dstFilePath))
                            {
                                encryptionTime = EncryptFile(srcFile, Path.GetDirectoryName(Uri.UnescapeDataString(dstFilePath)));
                            }
                            else
                            {
                                copyTime.Start();
                                File.Copy(srcFile, dstFilePath);
                                copyTime.Stop();
                                transferTime = copyTime.Elapsed.Milliseconds;
                            }
                            stateFileContent.Update(_name, srcFile, dstFilePath, transferTime, encryptionTime);
                        }

                    }
                }
                
            }
        }

        /// <summary>
        /// Return hash code of fi file.
        /// </summary>
        /// <param name="fi">file info of the file to get the hash</param>
        public int GetHashCode(FileInfo fi)
        {
            string s = $"{fi.Name}{fi.Length}";
            return s.GetHashCode();
        }
        /// <summary>
        /// Check if the filePath extension is a part of the extensions list, given by the User.
        /// </summary>
        /// <param name="extensions">the extension name</param>
        /// <param name="filePath">the file path</param>
        private bool ExtensionIsEncryptExtension(List<string> extensions, string filePath)
        {
            return extensions.Contains(Path.GetExtension(filePath));
        }

        /// <summary>
        /// Returns the stateFileContent linked with the save
        /// </summary>
        /// <returns>The stateFileContent</returns>
        public override StateFileContent GetStateFileContent()
        {
            return _stateFileContent;
        }
        /// <summary>
        /// Pause the savework.
        /// </summary>
        public void Pause()
        {
            while (isPaused)
            {
                Thread.Sleep(1000);
            }
        }
    }
}