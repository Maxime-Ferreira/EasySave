
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Core.Model.Type
{

    public class BusinessSoft
    {
        readonly List<string> _businessSoftList = new List<string>();
        private readonly XML.BusinessSoftwareXML _businessSoft = new XML.BusinessSoftwareXML();
        private static readonly System.Object _lock = new System.Object();
        private static BusinessSoft _instance;

        /// <summary>
        /// Constructor
        /// </summary>
        private BusinessSoft()
        {
            _businessSoftList = _businessSoft.Recover();
        }

        /// <summary>
        /// Set the business soft file
        /// </summary>
        public void SetBusinessSoft(string businessSoft)
        {
            string businessSoftName = Path.GetFileNameWithoutExtension(businessSoft);
            if (!_businessSoftList.Contains(businessSoftName))
            {
                _businessSoftList.Add(businessSoftName);
            }
            _businessSoft.Save(_businessSoftList);
        }

        /// <summary>
        /// check in business soft list
        /// </summary>
        public void CheckBusinessSoft()
        {
            lock (_lock)
            {
                bool temp = CheckProcess();
                while (!temp)
                {
                    System.Windows.Forms.MessageBox.Show("Please close the business soft and click OK");
                    temp = CheckProcess();
                }
            }
        }

        /// <summary>
        /// check if the business soft is running
        /// </summary>
        public bool CheckProcess()
        {
            foreach (string name in _businessSoftList)
            {
                foreach (Process p in Process.GetProcesses())
                {
                    if (name.Equals(p.ProcessName))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Get an instance of business soft (singleton)
        /// </summary>
        public static BusinessSoft GetInstance()
        {
            if (_instance == null)
            {
                _instance = new BusinessSoft();
            }
            return _instance;
        }
    }
}