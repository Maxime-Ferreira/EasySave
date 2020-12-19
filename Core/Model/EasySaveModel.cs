using Core.Model.Type;
using Core.Model.XML;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Core.Model
{
    public class EasySaveModel
    {
        private readonly LogsXML _log = new LogsXML();
        private readonly StateFileXML _state = new StateFileXML();
        private readonly CryptosoftExtensionsXML _cryptosoftExtensionsXML = CryptosoftExtensionsXML.GetCryptosfotExtensionsInstance();
        private readonly SaveWorkXML _saveWorkXML = new SaveWorkXML();
        private readonly ExtensionsPriorityXML _extensionsPriorityXML = new ExtensionsPriorityXML();
        private readonly MaximalFileSizeXML _maximalFileSizeXML = new MaximalFileSizeXML();
        private readonly List<SaveWorkModel> _saveWorks = new List<SaveWorkModel>();
        private readonly MultithreadingActions _multithreadingActions = new MultithreadingActions();
        private protected BusinessSoft _businessSoft = BusinessSoft.GetInstance();
        private List<string> _cryptosoftExtensionList = new List<string>();
        private List<string> _extensionsPriorityList = new List<string>();
        public static SocketServer socketServer = new SocketServer();

        /// <summary>
        /// Initialize the list of extention, the business software.
        /// Check if a business software exist.
        /// </summary>
        public EasySaveModel()
        {
            //XML.ParseXML init = new XML.ParseXML();
            _extensionsPriorityList = _extensionsPriorityXML.Recover();
            _cryptosoftExtensionList = _cryptosoftExtensionsXML.Recover();
            //_businessSoftList = _businessXML.Recover();
            try
            {
                LimitSizeTranfer.maxSize = int.Parse(RecoverMaxSizeFile());
            }
            catch
            {
                LimitSizeTranfer.maxSize = 100000;
            }

            _businessSoft.CheckBusinessSoft();
            foreach (SaveWorkModel saveWork in _saveWorkXML.Recover())
            {
                saveWork.SetPriorityExtensions(_extensionsPriorityList);
                //  saveWork.SetBusinessSoft(_businessSoft);
                this._saveWorks.Add(saveWork);
            }
        }

        /// <summary>
        /// Initialize the socket server.
        /// </summary>
        public void StartSocketServer()
        {
            socketServer.CreateSocket();
            Thread socketThread = new Thread(() =>
            {
                socketServer.AcceptConnexion();
                socketServer.ListenNetwork();
            });
            socketThread.Start();

        }
        /// <summary>
        /// Returns the size of the list of saveworks, which be also the number of saveworks.
        /// </summary>
        /// <returns>The size of the list of saveworks</returns>
        public int GetSaveWorksSize()
        {
            return _saveWorks.Count;
        }
        public StateFileContent GetStateFileContent(int i)
        {
            return _saveWorks[i].GetStateFileContent();
        }

        /// <summary>
        /// Run a specified save work load in user settings.
        /// </summary>
        /// <param name="name">choice of the save work made by the user</param>
        /// <param name="barrier">barrier between priority and non priority files</param>
        public void RunSaveWork(int name, Barrier barrier)
        {
            SaveWorkModel _saveWork = _saveWorks[name];
            _saveWork.SaveData(barrier);
        }

        /// <summary>
        /// Add a savework in user settings, wether it is a complete or differential, encrypt or not encrypit save work.
        /// </summary>
        /// <param name="name">choice of the save work made by the user</param>
        /// <param name="destinationFolder">destination folder</param>
        /// <param name="sourceFolder">source folder</param>
        /// <param name="type">type of the save work</param>
        /// <param name="encrypt">if we have to encrypt file or not</param>
        public void CreateSaveWork(string name, string sourceFolder, string destinationFolder, string type, bool encrypt)
        {
            SaveWorkModel _saveWork;
            if (type == "Complete")
            {
                if (encrypt)
                {
                    _saveWork = new SaveWorkModel(name, sourceFolder, destinationFolder, new Complete(name), _cryptosoftExtensionList, _extensionsPriorityList);
                }
                else
                {
                    _saveWork = new SaveWorkModel(name, sourceFolder, destinationFolder, new Complete(name), _extensionsPriorityList);
                }
            }
            else
            {
                if (encrypt)
                {
                    _saveWork = new SaveWorkModel(name, sourceFolder, destinationFolder, new Differential(name), _cryptosoftExtensionList, _extensionsPriorityList);
                }
                else
                {
                    _saveWork = new SaveWorkModel(name, sourceFolder, destinationFolder, new Differential(name), _extensionsPriorityList);
                }
            }
            //_saveWork.SetBusinessSoft(_businessSoft);
            _saveWorks.Add(_saveWork);
            _saveWorkXML.Save(_saveWork);
            _saveWorkXML.Check(_saveWork);
        }

        /// <summary>
        /// Modify a savework in user settings.
        /// </summary>
        public void EditSaveWork()
        {

        }

        /// <summary>
        /// Get the log file.
        /// </summary>
        public void SeelogFile()
        {
            _log.Recover();
        }

        /// <summary>
        /// Get the state file.
        /// </summary>
        public void SeeStateFile()
        {
            _state.Recover();
        }

        /// <summary>
        /// Definitely delete a savework from user settings and update the save work list.
        /// </summary>
        /// <param name="choice">The savework we want to delete</param>
        public void DeleteSaveWork(int choice)
        {
            if (choice == -1)
            {
                foreach (SaveWorkModel saveWork in _saveWorks)
                {
                    _saveWorkXML.Delete(saveWork);
                }
                _saveWorks.Clear();
            }
            else
            {
                SaveWorkModel _saveWork = _saveWorks[choice];
                _saveWorks.Remove(_saveWorks[choice]);
                _saveWorkXML.Delete(_saveWork);
            }
        }

        /// <summary>
        /// Set the specified business soft in user settings.
        /// </summary>
        /// <param name="businessSoft">The business software to add to the list</param>
        public void SetBusinessSoft(string businessSoft)
        {
            _businessSoft.SetBusinessSoft(businessSoft);
        }

        /// <summary>
        /// Get all extensions from user settings.
        /// </summary>
        /// <returns>the list of saveworks</returns>
        public List<SaveWorkModel> GetSaveWorks()
        {
            return _saveWorks;
        }

        /// <summary>
        /// Get all extensions from user settings.
        /// </summary>
        /// <param name="name">get the extension list for cryptosoft or priority files</param>
        /// <returns>the list of extensions</returns>
        public List<string> GetExtensions(string name)
        {
            if (name == "cryptosoft")
            {
                return _cryptosoftExtensionList;
            }
            else
            {
                return _extensionsPriorityList;
            }
        }

        /// <summary>
        /// Set the specified extension in user settings.
        /// </summary>
        /// <param name="extensions">The list of extensions chosen by the user</param>
        /// <param name="name">The name of modified list</param>
        public void SetExtensionList(List<string> extensions, string name)
        {
            List<string> chosenList = ChooseList(name);

            foreach (string extension in extensions)
            {
                if (!chosenList.Contains(extension))
                {
                    chosenList.Add(extension);
                    Debug.WriteLine(chosenList);
                }
            }
            foreach (SaveWorkModel saveWork in _saveWorks)
            {
                if (name == "cryptosoft")
                {
                    saveWork.SetExtensions(chosenList);
                }
                else
                {
                    saveWork.SetPriorityExtensions(chosenList);
                }
            }

            if (name == "cryptosoft")
            {
                _cryptosoftExtensionsXML.Save(chosenList);
                _cryptosoftExtensionList = chosenList;
            }
            else
            {
                _extensionsPriorityXML.Save(chosenList);
                _extensionsPriorityList = chosenList;
            }
        }

        /// <summary>
        /// Choose the right list between _cryptosoftExtensionList and _extensionsPriorityList
        /// </summary>
        /// <param name="name">The name of the chosen list</param>
        /// <returns>The chosen list</returns>
        private List<string> ChooseList(string name)
        {
            if (name.Equals("cryptosoft"))
            {
                return _cryptosoftExtensionList;
            }
            else
            {
                return _extensionsPriorityList;
            }
        }

        /// <summary>
        /// Remove the specified extension in user settings.
        /// </summary>
        /// <param name="extension">The extension chosen by the user</param>
        /// <param name="name">The name of modified list</param>
        public void DeleteExtension(string extension, string name)
        {
            if (name == "cryptosoft")
            {
                _cryptosoftExtensionsXML.Delete(extension);
                _cryptosoftExtensionList.Remove(extension);
            }
            else
            {
                _extensionsPriorityXML.Delete(extension);
                _extensionsPriorityList.Remove(extension);
            }
        }

        /// <summary>
        /// Sets the specified size in the user settings
        /// </summary>
        /// <param name="size">The size specified by the user</param>
        public void SetMaxSizeFile(string size)
        {
            _maximalFileSizeXML.Save(size);
            LimitSizeTranfer.maxSize = int.Parse(size);
        }

        /// <summary>
        /// Call the Recover() function of the MaximalFileSize class, to recover the size contained in the xml file
        /// </summary>
        /// <returns>The maximal size contained in the xml file</returns>
        public string RecoverMaxSizeFile()
        {
            return _maximalFileSizeXML.Recover();
        }

        /// <summary>
        /// Pause all the saveworks.
        /// </summary>
        /// <param name="isPaused">The size specified by the user</param>
        public void ThreadState(bool isPaused)
        {
            Complete.isPaused = isPaused;
            Differential.isPaused = isPaused;
        }

        /// <summary>
        /// Stop all the saveworks.
        /// </summary>
        public void ThreadStop()
        {
            _multithreadingActions.Stop();
        }
    }
}