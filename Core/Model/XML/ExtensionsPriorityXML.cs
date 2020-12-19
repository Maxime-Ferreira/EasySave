using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Core.Model.XML
{
    class ExtensionsPriorityXML : ParseXML
    {
        private readonly string _path = Path.Combine(ParseXML.appPath, @"Extensions\priority.xml");

        public ExtensionsPriorityXML()
        {
            SetFile(_path);
        }

        /// <summary>
        /// Save a list of priority extensions into a xml file
        /// </summary>
        /// <param name="extensions">The list of priority extensions</param>
        public void Save(List<string> extensions)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t"
            };
            using XmlWriter writer = XmlWriter.Create(_path, settings);
            writer.WriteStartElement("Extensions");

            foreach (string ext in extensions)
            {
                writer.WriteElementString(ext.Replace('.', '_'), ext);
            }
            writer.WriteEndElement();
            writer.Close();

            writer.Flush();
        }

        /// <summary>
        /// Recovers the list of priority extensions from the xml file
        /// </summary>
        /// <returns>the list of priority extensions recovered</returns>
        public List<string> Recover()
        {
            List<string> ext = new List<string>();
            XmlDocument docxml = new XmlDocument();
            try
            {
                docxml.Load(_path);
                XmlElement Extensions = docxml.DocumentElement;
                foreach (XmlNode name in Extensions)
                {
                    ext.Add(name.InnerText);
                }
            }
            catch
            {

            }
            return ext;
        }

        /// <summary>
        /// Delete an extension from the xml file
        /// </summary>
        /// <param name="extension">The extension we want to delete</param>
        public void Delete(string extension)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(_path);
            XmlElement Extensions = xmlDoc.DocumentElement;
            foreach (XmlNode name in Extensions)
            {
                if (name.InnerText == extension)
                {
                    name.ParentNode.RemoveChild(name);
                }
            }
            xmlDoc.Save(_path);
        }
    }
}
