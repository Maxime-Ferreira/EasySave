using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace Core.Model.XML
{
    public class CryptosoftExtensionsXML : ParseXML
    {
        private readonly string _path = Path.Combine(ParseXML.appPath, @"Extensions\user_settings_extensions.xml");
        private static CryptosoftExtensionsXML cryptosoftExtensions;

        /// <summary>
        /// Constructor of the class, which calls the SetFile() method from ParseXML.
        /// </summary>
        private CryptosoftExtensionsXML()
        {
            SetFile(_path);
        }

        /// <summary>
        /// Gets the instance of the class, and creates it if it doesn't exists.
        /// </summary>
        /// <returns>The instance of the class</returns>
        public static CryptosoftExtensionsXML GetCryptosfotExtensionsInstance()
        {
            if (cryptosoftExtensions == null)
            {
                cryptosoftExtensions = new CryptosoftExtensionsXML();
                return cryptosoftExtensions;
            }
            else
            {
                return cryptosoftExtensions;
            }
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

        /// <summary>
        /// Recovers the list of cryptosoft extensions from the xml file
        /// </summary>
        /// <returns>the list of cryptosoft extensions recovered</returns>
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
            catch (XmlException exc)
            {
                Debug.WriteLine(exc);
            }
            return ext;
        }

        /// <summary>
        /// Save a list of cryptosoft extensions into a xml file
        /// </summary>
        /// <param name="extensions">The list of cryptosoft extensions</param>
        public void Save(List<string> extensions)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t"
            };
            //Directory.SetCurrentDirectory(_path);
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
    }
}
