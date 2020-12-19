using Core.Model.XML;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Core.Model
{
    public class StateFileContent : INotifyPropertyChanged
    {
        private int _nbFileTransfered;
        private int _nbFileToTransfer;
        private long _sizeToTransfer;
        private long _sizeTransfered;
        private string _state;
        private readonly StateFileXML stateFile = new StateFileXML();
        private readonly SocketServer _socketServer = EasySaveModel.socketServer;
        private readonly LogsXML log = new LogsXML();

        public StateFileContent()
        {
            this.OutSaveworkName = "SWnull";
            this.OutProgression = "Prgsnull";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string stateFileContent)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(stateFileContent));
        }

        private string _outSaveworkName;
        public string OutSaveworkName
        {
            get { return _outSaveworkName; }
            set
            {
                _outSaveworkName = value.ToString();
                OnPropertyChanged("outSaveworkName");
            }
        }

        private string _outProgression;
        public string OutProgression
        {
            get { return _outProgression; }
            set
            {
                _outProgression = value.ToString();
                OnPropertyChanged("outProgression");
            }
        }

        private string _outRemainingFiles;
        public string OutRemainingFiles
        {
            get { return _outRemainingFiles; }
            set
            {
                _outRemainingFiles = value.ToString();
                OnPropertyChanged("outRemainingFiles");
            }
        }

        private string _outRemainingSize;
        public string OutRemainingSize
        {
            get { return _outRemainingSize; }
            set
            {
                _outRemainingSize = value.ToString();
                OnPropertyChanged("outRemainingSize");
            }
        }

        private string _outSrcFile;
        public string OutSrcFile
        {
            get { return _outSrcFile; }
            set
            {
                _outSrcFile = value.ToString();
                OnPropertyChanged("outSrcFile");
            }
        }
        private string _outDestFile;
        public string OutDestFile
        {
            get { return _outDestFile; }
            set
            {
                _outDestFile = value.ToString();
                OnPropertyChanged("outDestFile");
            }
        }
        private string _outType;
        public string OutType
        {
            get { return _outType; }
            set
            {
                _outType = value.ToString();
                OnPropertyChanged("outType");
            }
        }

        /// <summary>
        ///  Complete constructor.
        /// </summary>
        /// <param name="saveWorkName">save work name</param>
        /// <param name="srcFolder">source file copy</param>
        /// <param name="dstFolder">destination file copy</param>
        public void InitStateFile(string saveWorkName, string srcFolder, string dstFolder)
        {
            OutSaveworkName = saveWorkName;
            OutType = "complete";
            _state = "Active";
            _sizeToTransfer = SrcFileSize(srcFolder);
            _nbFileToTransfer = NbFile(srcFolder);
            _socketServer.Send(saveWorkName, "begin");
            stateFile.Save(saveWorkName, TimeStamp(), _state, _nbFileToTransfer, _sizeToTransfer, 0, _nbFileToTransfer, _sizeToTransfer, srcFolder, dstFolder);
        }

        /// <summary>
        /// Differential constructor  string saveworkName, string timestamp, string state, int nbFileToTransfer, long sizeToTransfer, int progression, int remainingFile, long remainingSize, string srcFile, string destFile
        /// </summary>
        /// <param name="saveWorkName">save work name</param>
        /// <param name="srcFolder">source file copy</param>
        /// <param name="dstFolder">destination file copy</param>
        /// <param name="type">type of the save</param>
        /// <param name="sameFiles">All same files</param>
        /// <param name="changedFiles">All changed files to copy</param>
        /// <param name="newFiles">All new files to copy</param>
        public void InitStateFile(string saveWorkName, string srcFolder, string dstFolder, List<string> changedFiles, List<string> sameFiles, List<string> newFiles)
        {
            OutSaveworkName = saveWorkName;
            OutType = "differential";

            _state = "Active";
            _sizeToTransfer = SrcFileSize(changedFiles, sameFiles, newFiles);
            _nbFileToTransfer = NbFile(changedFiles, sameFiles, newFiles);
            _socketServer.Send(saveWorkName, "begin");
            stateFile.Save(saveWorkName, TimeStamp(), _state, _nbFileToTransfer, _sizeToTransfer, 0, _nbFileToTransfer, _sizeToTransfer, srcFolder, dstFolder);
        }

        /// <summary>
        /// Reset the count of all files types.
        /// </summary>
        public void ResetCount()
        {
            _nbFileTransfered = 0;
            _nbFileToTransfer = 0;
            _sizeToTransfer = 0;
            _sizeTransfered = 0;
            _state = "null";
        }

        /// <summary>
        /// Update the state file content with specified params.
        /// </summary>
        /// <param name="saveWorkName">save work name</param>
        /// <param name="srcFile">source file copy</param>
        /// <param name="dstFile">destination file copy</param>
        /// <param name="transferTime">trasfert time of file copying</param>
        /// <param name="encryptionTime">save work name</param>
        public void Update(string saveWorkName, string srcFile, string dstFile, int transferTime, int encryptionTime)
        {
            long remainingSize = RemainingSize(srcFile);
            int remainingFiles = RemainingFiles();
            int progression = Progression();
            long fileSize = FileSize(srcFile);

            OutProgression = progression.ToString();
            OutRemainingFiles = remainingFiles.ToString();
            OutRemainingSize = remainingSize.ToString();
            OutSrcFile = srcFile;
            OutDestFile = dstFile;
            _socketServer.Send(saveWorkName, progression.ToString());
            log.Save(saveWorkName, transferTime, fileSize, srcFile, dstFile, encryptionTime);
            stateFile.Save(saveWorkName, TimeStamp(), _state, _nbFileToTransfer, _sizeToTransfer, progression, remainingFiles, remainingSize, srcFile, dstFile);
        }

        /// <summary>
        ///  Get the initial file number to transfer.
        /// </summary>
        /// <param name="srcFolder">source file</param>
        /// <returns> total number of files in the source folder</returns>
        private int NbFile(string srcFolder)
        {
            return Directory.GetFiles(srcFolder, "*.*", SearchOption.AllDirectories).Length;
        }

        /// <summary>
        /// Get the number of file to transfer.
        /// </summary>
        /// <param name="sameFiles">All same files</param>
        /// <param name="changedFiles">All changed files to copy</param>
        /// <param name="newFiles">All new files to copy</param>
        /// <returns>A number of file</returns>
        private int NbFile(List<string> changedFiles, List<string> sameFiles, List<string> newFiles)
        {
            return changedFiles.Count + sameFiles.Count + newFiles.Count;
        }

        /// <summary>
        /// Get the sile of the specifiedfile
        /// </summary>
        /// <param name="srcFile">source folder</param>
        /// <returns>size (octets) of the file</returns>
        private long FileSize(string srcFile)
        {
            long size = new FileInfo(srcFile).Length;
            return size;
        }

        /// <summary>
        ///  Get the initial size to transfer for complete save.
        /// </summary>.
        /// <param name="srcFolder">source file</param>
        /// <returns>size (octets) of the source folder</returns>
        private long SrcFileSize(String srcFolder)
        {
            long totalSize = 0;
            foreach (string dir in Directory.GetDirectories(srcFolder, "*", SearchOption.AllDirectories))
            {
                DirectoryInfo dirinfo = new DirectoryInfo(dir);
                foreach (FileInfo file in dirinfo.GetFiles())
                {
                    totalSize += file.Length;
                }
            }
            return totalSize;
        }

        /// <summary>
        /// Get the initial size to transfer for differential save.
        /// </summary>
        /// <param name="changedFiles">All source files to copy</param>
        /// <param name="sameFiles">All same files</param>
        /// <param name="changedDstFiles">All changed destination files to copy</param>
        /// <param name="newFiles">All new files to copy</param>
        /// <returns>Src octets size of source file</returns>
        private long SrcFileSize(List<string> changedFiles, List<string> sameFiles, List<string> newFiles)
        {
            long totalSize = 0;
            if (changedFiles.Count != 0)
            {
                for (int i = 0; i < changedFiles.Count; i++)
                {
                    FileInfo srcSize = new FileInfo(changedFiles[i]);
                    totalSize += srcSize.Length;
                }
            }

            if (sameFiles.Count != 0)
            {
                foreach (string sameFile in sameFiles)
                {
                    FileInfo srcSize = new FileInfo(sameFile);
                    totalSize += srcSize.Length;
                }
            }

            if (newFiles.Count != 0)
            {
                foreach (string newFile in newFiles)
                {
                    FileInfo srcSize = new FileInfo(newFile);
                    totalSize += srcSize.Length;
                }
            }
            return totalSize;
        }

        /// <summary>
        /// Get the actual hour, minute and second, format like HH:MM:SS
        /// </summary>
        /// <returns>string with a complete timestamp</returns>
        private string TimeStamp()
        {
            return DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString();
        }

        /// <summary>
        ///  Calculates the percentage of progress of a savework.
        /// </summary>
        /// <returns>Progression of the savework.</returns>
        private int Progression()
        {
            decimal temp1 = Convert.ToDecimal(_nbFileTransfered);
            decimal temp2 = Convert.ToDecimal(_nbFileToTransfer);
            if (temp1 == 0 || temp2 == 0)
            {
                return 0;
            }
            else
            {
                decimal temp = (temp1 / temp2);
                return Convert.ToInt32(temp * 100);
            }

        }

        /// <summary>
        /// Calculates remaining files's number.
        /// </summary>
        /// <returns>file number</returns>
        private int RemainingFiles()
        {
            _nbFileTransfered += 1;
            return _nbFileToTransfer - _nbFileTransfered;
        }

        /// <summary>
        ///  Calculates the size of the remaining files.
        /// </summary>
        /// <param name="srcFile">save source folder</param>
        /// <returns>remaining File size</returns>
        private long RemainingSize(string srcFile)
        {
            _sizeTransfered += new FileInfo(srcFile).Length;
            return _sizeToTransfer - _sizeTransfered;
        }

        /// <summary>
        /// Set the last entry of state file and close the form.
        /// </summary>
        /// <param name="saveWorkName">save name</param>
        /// <param name="srcFoler">save source folder</param>
        /// <param name="destFolder">save destination folder</param>
        public void SetEndState(string saveWorkName, string srcFoler, string destFolder)
        {
            _state = "Not_Active";
            _socketServer.Send(saveWorkName, "End");
            stateFile.Save(saveWorkName, TimeStamp(), _state, _nbFileToTransfer, _sizeToTransfer, 100, 0, 0, srcFoler, destFolder);

        }
    }
}