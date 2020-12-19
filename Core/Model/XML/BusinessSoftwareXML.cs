using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Core.Model.XML
{
    public class BusinessSoftwareXML : ParseXML
    {
        private readonly string _path = Path.Combine(ParseXML.appPath, @"BusinessSoftwares\BusinessSoftwares.xml");
        private static BusinessSoftwareXML _instance;

        /// <summary>
        /// Constructor of the class, which calls the SetFile() method from ParseXML
        /// </summary>
        public BusinessSoftwareXML()
        {
            SetFile(_path);
        }

        /// <summary>
        /// Save a list of business softwares into a xml file
        /// </summary>
        /// <param name="businessSofts">The list of business softwares</param>
        public void Save(List<string> businessSofts)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t"
            };
            //Directory.SetCurrentDirectory(_path);
            using XmlWriter writer = XmlWriter.Create(_path, settings);
            writer.WriteStartElement("Extensions");
            foreach (string ext in businessSofts)
            {
                writer.WriteElementString("name", ext);
            }
            writer.WriteEndElement();
            writer.Close();
            writer.Flush();
        }

        /// <summary>
        /// Recovers the list of business softwares from the xml file
        /// </summary>
        /// <returns>the list of business softwares recovered</returns>
        public List<string> Recover()
        {
            List<string> business = new List<string>();
            XmlDocument docxml = new XmlDocument();
            /*            if (new FileInfo(_path).Length != 0)
                        {
                            Debug.WriteLine("lenght of businessSoftwareFile {0}", new FileInfo(_path).Length);
                            docxml.Load(_path);
                            XmlElement BusinessSoftwares = docxml.DocumentElement;
                            foreach (XmlNode name in BusinessSoftwares)
                            {
                                business.Add(name.InnerText);
                            }
                        }*/

            try
            {
                docxml.Load(_path);
                XmlElement BusinessSoftwares = docxml.DocumentElement;
                foreach (XmlNode name in BusinessSoftwares)
                {
                    business.Add(name.InnerText);
                }
            }
            catch
            {

            }
            return business;
        }

        /// <summary>
        /// Delete an business software from the xml file
        /// </summary>
        public void Delete()
        {
            //Not implemented
        }

        /// <summary>
        /// Gets the instance of the class, and creates it if it doesn't exists
        /// </summary>
        /// <returns>The instance of the class</returns>
        public static BusinessSoftwareXML GetInstance()
        {
            if (_instance == null)
            {
                _instance = new BusinessSoftwareXML();
            }
            return _instance;
        }
    }
}
