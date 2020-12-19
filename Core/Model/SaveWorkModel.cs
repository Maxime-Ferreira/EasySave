using Core.Model.Type;
using System.Collections.Generic;
using System.Threading;

namespace Core.Model

{
    public class SaveWorkModel : ISaveWorkModel
    {
        public string name;
        public string srcFolder;
        public string dstFolder;
        private readonly ISaveWorkType tempSaveWorkType;
        public string type;
        private List<string> _extensions = null;
        public bool encrypt = false;
        private List<string> _priorityExtensions = null;

        // private BusinessSoft _business = BusinessSoft.GetInstance();

        /// <summary>
        /// Initialisation of the SaveWorkModel if we not have to encrypt.
        /// </summary>
        ///<param name="name">name of the save work</param>
        ///<param name="sourceFolder">source folder of the save work</param>
        ///<param name="destinationFolder">destination folder of the save work</param>
        ///<param name="tempSaveWorkType">type of save work</param>
        ///<param name="priorityExtensions">List of all priority extensions</param>
        public SaveWorkModel(string name, string sourceFolder, string destinationFolder, ISaveWorkType tempSaveWorkType, List<string> priorityExtensions)
        {
            this.name = name;
            this.srcFolder = sourceFolder;
            this.dstFolder = destinationFolder;
            this.tempSaveWorkType = tempSaveWorkType;
            this.type = tempSaveWorkType.GetType();
            this._priorityExtensions = priorityExtensions;
        }

        /// <summary>
        /// Initialisation of the SaveWorkModel if we have to encrypt.
        /// </summary>
        ///<param name="name">name of the save work</param>
        ///<param name="sourceFolder">source folder of the save work</param>
        ///<param name="destinationFolder">destination folder of the save work</param>
        ///<param name="tempSaveWorkType">type of save work</param>
        ///<param name="extensions">list of extensions files to encrypt</param>
        ///<param name = "priorityExtensions" > list of priority extensions files to encrypt</param>
        public SaveWorkModel(string name, string sourceFolder, string destinationFolder, ISaveWorkType tempSaveWorkType, List<string> extensions, List<string> priorityExtensions)
        {
            this.name = name;
            this.srcFolder = sourceFolder;
            this.dstFolder = destinationFolder;
            this.tempSaveWorkType = tempSaveWorkType;
            this.type = tempSaveWorkType.GetType();
            this._extensions = extensions;
            this.encrypt = true;
            this._priorityExtensions = priorityExtensions;

        }

        /// <summary>
        /// Set the list of extensions files to encrypt
        /// </summary>
        /// <param name="extensions"></param>
        public void SetExtensions(List<string> extensions)
        {
            _extensions = extensions;
        }

        /*        public void SetBusinessSoft(BusinessSoft business)
                {
                    this._business = business;
                }
        */
        /// <summary>
        /// Set the list of extensions files to encrypt
        /// </summary>
        /// <param name="extensions"></param>
        public void SetPriorityExtensions(List<string> extensions)
        {
            _priorityExtensions = extensions;
        }

        /// <summary>
        /// Save files.
        /// </summary>
        /// <param name="barrier">Use to wait all priority files before non priority</param>
        public void SaveData(Barrier barrier)
        {
            tempSaveWorkType.SaveData(srcFolder, dstFolder, _extensions, _priorityExtensions, barrier);
        }
        /// <summary>
        /// Get the state file content
        /// </summary>
        public StateFileContent GetStateFileContent()
        {
            return tempSaveWorkType.GetStateFileContent();
        }

        /// <summary>
        /// Get the savework's type.
        /// </summary>
        /// <returns>the type of the savework</returns>
        string ISaveWorkModel.GetType()
        {
            return type;
        }

        /// <summary>
        /// Get the savework's name.
        /// </summary>
        /// <returns>the name of the savework</returns>
        string ISaveWorkModel.GetName()
        {
            return name;
        }
    }
}
