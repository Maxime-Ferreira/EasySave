using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Core.Model.XML
{
    public class ParseXML
    {
        public static string appPath = Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).ToString()).ToString();

        public List<string> folderList = new List<string>() { @"Saveworks\", @"BusinessSoftwares\", @"Extensions\", @"Logs\", @"Logs\Log" };


        /// <summary>
        /// Constructor of the class, which calls the SetFolder() method for each folder in the folderList
        /// </summary>
        public ParseXML()
        {
            foreach (string folder in folderList)
            {
                SetFolder(folder);
            }
        }

        /// <summary>
        /// Create the xml file if it doesn't exist
        /// </summary>
        /// <param name="file">The xml file we wan't to create</param>
        public void SetFile(string file)
        {
            string tempXmlFilePath = Path.Combine(appPath, file);

            if (!File.Exists(tempXmlFilePath))
            {
                XmlWriter writer = XmlWriter.Create(tempXmlFilePath);
                writer.Close();
                writer.Flush();
            }
        }

        /// <summary>
        /// Create the folder if it doesn't exists
        /// </summary>
        /// <param name="folder">The folder we wan't to create</param>
        public void SetFolder(string folder)
        {
            string tempXmlPath = Path.Combine(appPath, folder);
            if (!Directory.Exists(tempXmlPath))
            {
                Directory.CreateDirectory(tempXmlPath);
            }
        }

    }
}
