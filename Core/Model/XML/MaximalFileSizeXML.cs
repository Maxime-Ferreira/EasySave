using System.IO;
using System.Xml;

namespace Core.Model.XML
{
    public class MaximalFileSizeXML : ParseXML
    {
        private readonly string _path = Path.Combine(ParseXML.appPath, $"maxSize.xml");

        /// <summary>
        /// Constructor of the class, which calls the SetFile() method from ParseXML
        /// </summary>
        public MaximalFileSizeXML()
        {
            //SetFile(_path);
        }

        /// <summary>
        /// Save the maximal file size into a xml file
        /// </summary>
        /// <param name="extensions">the maximal file size</param>
        public void Save(string size)
        {
            System.Diagnostics.Debug.WriteLine(_path);
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t"
            };
            if (!File.Exists(_path))
            {
                using XmlWriter writer = XmlWriter.Create(_path, settings);
                writer.WriteStartElement("MaxSize");
                writer.WriteElementString("size", size);
                writer.WriteEndElement();
                writer.Close();
                writer.Flush();
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(_path);
                doc.SelectSingleNode("MaxSize/size").InnerText = size;
                doc.Save(_path);
            }
        }

        /// <summary>
        /// Recovers the maximal file size from the xml file
        /// </summary>
        /// <returns>the maximal file size</returns>
        public string Recover()
        {
            string maxFileSize = "";
            XmlDocument docxml = new XmlDocument();
            try
            {
                docxml.Load(_path);
                XmlElement MaxSize = docxml.DocumentElement;
                foreach (XmlNode name in MaxSize)
                {
                    maxFileSize = name.InnerText;
                }
            }
            catch
            {

            }
            return maxFileSize;
        }
    }
}
