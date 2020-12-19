using Core.Model.Type;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Core.Model.XML
{
    public class SaveWorkXML : ParseXML
    {
        private readonly string _path = Path.Combine(ParseXML.appPath, @"Saveworks\");
        private readonly CryptosoftExtensionsXML cryptosoftExtension = CryptosoftExtensionsXML.GetCryptosfotExtensionsInstance();
        private readonly ExtensionsPriorityXML extensionPriority = new ExtensionsPriorityXML();

        /// <summary>
        /// Save a savework in a xml file
        /// </summary>
        /// <param name="saveWork">The savework to save</param>
        public void Save(SaveWorkModel saveWork)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t"
            };
            Directory.SetCurrentDirectory(_path);
            using XmlWriter writer = XmlWriter.Create(_path + "Saveworks" + saveWork.name + ".xml", settings);
            writer.WriteStartElement("SaveWorkModel");
            writer.WriteElementString("name", saveWork.name);
            writer.WriteElementString("sourceFolder", saveWork.srcFolder);
            writer.WriteElementString("destinationFolder", saveWork.dstFolder);
            writer.WriteElementString("type", saveWork.type);
            writer.WriteElementString("encrypt", saveWork.encrypt.ToString());
            writer.WriteEndElement();
            writer.Close();
            writer.Flush();
        }

        /// <summary>
        /// Recovers all the saveworks from the xml files in the Savework folder 
        /// </summary>
        /// <returns>The list of saveworks</returns>
        public List<SaveWorkModel> Recover()
        {
            string[] files = Directory.GetFiles(_path, "*.xml");
            List<SaveWorkModel> saveWorks = new List<SaveWorkModel>();
            foreach (string file in files)
            {
                XmlDocument docxml = new XmlDocument();
                docxml.Load(file);
                XmlElement SaveWorkModel = docxml.DocumentElement;
                XmlNode name = SaveWorkModel.SelectSingleNode("name");
                XmlNode sourceFolder = SaveWorkModel.SelectSingleNode("sourceFolder");
                XmlNode destinationFolder = SaveWorkModel.SelectSingleNode("destinationFolder");
                XmlNode type = SaveWorkModel.SelectSingleNode("type");
                XmlNode encryptNode = SaveWorkModel.SelectSingleNode("encrypt");
                bool encrypt = Convert.ToBoolean(encryptNode.InnerText);
                SaveWorkModel saveWork;
                if (type.InnerText == "diff")
                {
                    if (encrypt)
                    {
                        saveWork = new SaveWorkModel(name.InnerText, sourceFolder.InnerText, destinationFolder.InnerText, new Differential(name.InnerText), cryptosoftExtension.Recover(), extensionPriority.Recover());
                        saveWorks.Add(saveWork);
                    }
                    else
                    {
                        saveWork = new SaveWorkModel(name.InnerText, sourceFolder.InnerText, destinationFolder.InnerText, new Differential(name.InnerText), extensionPriority.Recover());
                        saveWorks.Add(saveWork);
                    }

                }
                else if (type.InnerText == "complete")
                {
                    if (encrypt)
                    {
                        saveWork = new SaveWorkModel(name.InnerText, sourceFolder.InnerText, destinationFolder.InnerText, new Complete(name.InnerText), cryptosoftExtension.Recover(), extensionPriority.Recover());
                        saveWorks.Add(saveWork);
                    }
                    else
                    {
                        saveWork = new SaveWorkModel(name.InnerText, sourceFolder.InnerText, destinationFolder.InnerText, new Complete(name.InnerText), extensionPriority.Recover());
                        saveWorks.Add(saveWork);
                    }
                }

            }
            return saveWorks;
        }

        /// <summary>
        /// Delete the xml file of a savework
        /// </summary>
        /// <param name="saveWork">The savework of the xml file</param>
        public void Delete(SaveWorkModel saveWork)
        {
            File.Delete(_path + "Saveworks" + saveWork.name + ".xml");
        }

        /// <summary>
        /// Check if the xml file has been created
        /// </summary>
        /// <param name="saveWork">The savework of the xml file</param>
        /// <returns>True if it has been created, false otherwise</returns>
        public bool Check(SaveWorkModel saveWork)
        {
            string path = _path + "Saveworks" + saveWork.name + ".xml";
            if (File.Exists(path))
            {
                return File.ReadAllText(path).Contains(saveWork.name);
            }
            else
            {
                return false;
            }
        }
    }
}
