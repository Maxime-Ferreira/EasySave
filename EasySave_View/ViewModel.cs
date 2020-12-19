using Core.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace EasySave_View
{
    public class ViewModel
    {
        private readonly EasySaveModel _easySaveModel;
        private readonly MainWindow _mainWindow;

        /// <summary>
        /// The constructor creates an instance of the model and the view, then run it
        /// </summary>
        public ViewModel(MainWindow mainWindow)
        {
            _easySaveModel = new EasySaveModel();
            _mainWindow = mainWindow;
        }

        public void StartSocketServer()
        {
            _easySaveModel.StartSocketServer();
        }

        /// <summary>
        /// This method convert the name of the savework to a number for the model
        /// </summary>
        /// <param name="name">The name of the savework</param>
        /// <returns>The index of the savework in the list, and returns -1 if it's all saveworks</returns>
        private int ConvertName(string name)
        {
            List<SaveWorkModel> SaveWorks = RecoverSaveWorks();
            for (int i = 0; i < SaveWorks.Count; i++)
            {
                if (SaveWorks[i].name == name)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// This method collect the array of saveWorks by instanciating a new thread and run the chosen savework, then when finished calls the main menu of the view
        /// </summary>
        /// <param name="name">The name of the savework in letter</param>
        public void RunSaveWork(string name)
        {
            int number = ConvertName(name);
            if (number == -1)
            {
                int swSize = _easySaveModel.GetSaveWorksSize();
                Barrier barrier = new Barrier(swSize);
                for (int i = 0; i < swSize; i++)
                {
                    int intFor = i;
                    Thread thread = new Thread(() =>
                    {
                        _mainWindow.DisplayState(_easySaveModel.GetStateFileContent(intFor));
                        _easySaveModel.RunSaveWork(intFor, barrier);
                    });
                    thread.Start();
                }
            }
            else if (number >= 0)
            {
                Barrier barrier = new Barrier(1);
                Thread thread = new Thread(() =>
                {
                    _mainWindow.DisplayState(_easySaveModel.GetStateFileContent(number));
                    _easySaveModel.RunSaveWork(number, barrier);
                });
                thread.Start();
            }
        }

        /// <summary>
        /// This method calls the model and the view to change a savework
        /// </summary>
        public void EditSaveWork()
        {
            _easySaveModel.EditSaveWork();
        }

        /// <summary>
        /// This method convert the name to a number and call the model with this number to delete a save work
        /// </summary>
        /// <param name="name">The name of the savework in letter</param>
        public void DeleteSaveWork(string name)
        {
            int number = ConvertName(name);
            _easySaveModel.DeleteSaveWork(number);
        }

        /// <summary>
        /// This method calls the method SeeLogFile() of the model, to show the log file
        /// </summary>
        public void SeeLogFile()
        {
            _easySaveModel.SeelogFile();
        }

        /// <summary>
        /// This method calls the method SeeStateFile() of the model, to show the state file
        /// </summary>
        public void SeeStateFile()
        {
            _easySaveModel.SeeStateFile();
        }

        /// <summary>
        /// This method quit the software
        /// </summary>
        public void Quit()
        {
            System.Windows.Application.Current.Shutdown();
        }

        /// <summary>
        /// This method call the model for add the selected business software in a XML file
        /// </summary>
        /// <param name="businessSoftware">The name of the business softwarer</param>
        public void AddBusinessSoft(string businessSoftware)
        {
            _easySaveModel.SetBusinessSoft(businessSoftware);
        }

        /// <summary>
        /// This method calls the CreateSaveWork() method of the model with parameters given by the view, then when finished calls the main menu of the view
        /// </summary>
        /// <param name="name">The name of the savework</param>
        /// <param name="sourceFolder">The source folder of the savework</param>
        /// <param name="destinationFolder">The destination folder of the savework</param>
        /// <param name="type">The type of the savework</param>
        /// <param name="encrypt">If we want to encrypt files with this savework</param>
        public void CreateSaveWork(string name, string sourceFolder, string destinationFolder, string type, bool encrypt)
        {
            _easySaveModel.CreateSaveWork(name, sourceFolder, destinationFolder, type, encrypt);
        }

        /// <summary>
        /// Call the model to get the save works list.
        /// </summary>
        public List<SaveWorkModel> RecoverSaveWorks()
        {
            return _easySaveModel.GetSaveWorks();
        }

        /// <summary>
        /// Call the model for set new extension for cryptosoft in the list and the XML.
        /// </summary>
        /// <param name="ext">The list of cryptosoft extensions chosen by the user</param>
        public void SetCryptosoftExtension(List<string> ext)
        {
            _easySaveModel.SetExtensionList(ext, "cryptosoft");
        }

        /// <summary>
        /// Call the model for set new extension for priority files in the list and the XML.
        /// </summary>
        /// <param name="ext">The list of priority extensions chosen by the user</param>
        public void SetExtensionPriority(List<string> ext)
        {
            _easySaveModel.SetExtensionList(ext, "priority");
        }

        /// <summary>
        /// Call the model for delete extension of cryptosoft files in the list and the XML.
        /// </summary>
        /// <param name="extension">The cryptosoft extension we want to add</param>
        public void DeleteCryptosoftExtension(string extension)
        {
            _easySaveModel.DeleteExtension(extension, "cryptosoft");
        }

        /// <summary>
        /// Call the model for delete extension of priority files in the list and the XML.
        /// </summary>
        /// <param name="extension">The priority extension we want to add</param>
        public void DeleteEntensionPriority(string extension)
        {
            _easySaveModel.DeleteExtension(extension, "priority");
        }

        /// <summary>
        /// Call the model to get the list of extensions we want to show.
        /// </summary>
        /// <param name="name">The name of the list</param>
        /// <returns>The list of extensions to show</returns>
        public List<String> RecoverExtensions(string name)
        {
            return _easySaveModel.GetExtensions(name);
        }

        /// <summary>
        /// Call the model to set the maximum file size chosen by the user.
        /// </summary>
        /// <param name="size">The size specified by the user</param>
        public void SetMaximumFileSize(string size)
        {
            _easySaveModel.SetMaxSizeFile(size);
        }

        /// <summary>
        /// Call the model to get the size contained in the xml file.
        /// </summary>
        public string RecoverMaximumFileSize()
        {
            return _easySaveModel.RecoverMaxSizeFile();
        }

        /// <summary>
        /// Pause the thread with the isPaused value.
        /// </summary>
        /// <param name="isPaused">if the user click in pause, the value is true</param>
        public void ThreadState(bool isPaused)
        {
            _easySaveModel.ThreadState(isPaused);
        }

        /// <summary>
        /// Stop the thread.
        /// </summary>
        /// <param name="isPaused">if the user click in stop, the value is true</param>
        public void ThreadStop()
        {
            _easySaveModel.ThreadStop();
        }
    }
}