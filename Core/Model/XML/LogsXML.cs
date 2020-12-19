using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Core.Model.XML
{
    public class LogsXML : ParseXML
    {
        private readonly string _path = Path.Combine(appPath, @"Logs\Log");
        private string _logFilePath;
        private string _logFileDate;
        private static readonly System.Object _lock = new System.Object();

        /// <summary>
        /// Save informations into a log file
        /// </summary>
        /// <param name="saveworkName">The name of the savework</param>
        /// <param name="transferTime">The time of the tranfert of the file</param>
        /// <param name="fileSize">The size of the file saved</param>
        /// <param name="srcFile">The name of the source file</param>
        /// <param name="destFile">The name of the destination file</param>
        /// <param name="encryptTime">The encryption time of the file</param>
        public void Save(string saveworkName, int transferTime, long fileSize, string srcFile, string destFile, int encryptTime)
        {

            _logFileDate = GetDate();
            _logFilePath = Path.Combine(_path, String.Concat(_logFileDate, ".xml"));
            lock (_lock)
            {
                string run_time = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString();
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "\t"
                };
                if (!File.Exists(_logFilePath))
                {
                    using XmlWriter writer = XmlWriter.Create(_logFilePath, settings);
                    writer.WriteStartElement("Logs");
                    writer.WriteStartElement("Log");
                    writer.WriteElementString("Timestamp", run_time);
                    writer.WriteElementString("Name", saveworkName);
                    writer.WriteElementString("SourceFile", srcFile);
                    writer.WriteElementString("DestinationFile", destFile);
                    writer.WriteElementString("Lenght", fileSize.ToString() + " octets");
                    writer.WriteElementString("TransferTime", transferTime.ToString() + " ms");
                    writer.WriteElementString("EncryptionTime", encryptTime.ToString() + " ms");
                    writer.WriteEndElement();
                    writer.Close();
                    writer.Flush();
                }
                else
                {
                    XDocument xmlDoc = new XDocument();
                    xmlDoc = XDocument.Load(_logFilePath);
                    XElement logs = xmlDoc.Element("Logs");
                    logs.Add(new XElement("Log",
                        new XElement("Timestamp", run_time),
                        new XElement("Name", saveworkName),
                        new XElement("SourceFile", srcFile),
                        new XElement("DestinationFile", destFile),
                        new XElement("Length", fileSize.ToString() + " octets"),
                        new XElement("TransferTime", transferTime.ToString() + " ms")));
                    xmlDoc.Save(_logFilePath);
                }
            }
        }

        /// <summary>
        /// Get the date of the day
        /// </summary>
        public string GetDate()
        {
            return DateTime.Now.Day.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Year.ToString();
        }

        /// <summary>
        /// display the actual day log.
        /// </summary>
        public void Recover()
        {
            _logFileDate = GetDate();
            _logFilePath = Path.Combine(_path, String.Concat(_logFileDate, ".xml"));
            System.Diagnostics.Process.Start("notepad.exe", _logFilePath);
        }

        public void Delete()
        {
            //NOT USEFUL FOR NOW
        }


    }
}
