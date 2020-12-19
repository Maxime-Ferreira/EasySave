using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Core.Model.XML
{
    public class StateFileXML : ParseXML
    {
        private static readonly System.Object _lock = new System.Object();

        /// <summary>
        /// Constructor to state file, create the file
        /// </summary>
        public StateFileXML()
        {

        }

        /// <summary>
        /// Fill the state file with the currents states.
        /// </summary>
        /// <param name="saveworkName">The savework name</param>
        /// <param name="timestamp">the current timestamp</param>
        /// <param name="state">the current sate of the save</param>
        /// <param name="nbFileToTransfer">the number file to transfer</param>
        /// <param name="sizeToTransfer">the size to transfer</param>
        /// <param name="progression">the current progression</param>
        /// <param name="remainingFile">The remaining files to transfer</param>
        /// <param name="remainingSize">the remaining size to transfer</param>
        /// <param name="srcFile">the actual source file</param>
        /// <param name="destFile">the actual destination file</param>
        public void Save(string saveworkName, string timestamp, string state, int nbFileToTransfer, long sizeToTransfer, int progression, int remainingFile, long remainingSize, string srcFile, string destFile)
        {
            lock (_lock)
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "\t"
                };
                string _path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..") + $"/Logs/state.xml");
                if (!File.Exists(_path))
                {
                    using XmlWriter writer = XmlWriter.Create(_path, settings);
                    writer.WriteStartElement("States");
                    writer.WriteStartElement("State");
                    writer.WriteElementString("Name", saveworkName);
                    writer.WriteElementString("Timestamp", timestamp + " ms");
                    writer.WriteElementString("Active", state);
                    writer.WriteElementString("TotalFiles", nbFileToTransfer.ToString());
                    writer.WriteElementString("TotalLenght", sizeToTransfer + " octets");
                    writer.WriteElementString("Progression", progression + "%");
                    writer.WriteElementString("RemainingFiles", remainingFile.ToString());
                    writer.WriteElementString("RemainingLength", remainingSize + " octets");
                    writer.WriteElementString("SourceFile", srcFile);
                    writer.WriteElementString("DestinationFile", destFile);
                    writer.WriteEndElement();
                    writer.Close();
                    writer.Flush();
                }
                else
                {
                    XDocument xmlDoc = new XDocument();
                    xmlDoc = XDocument.Load(_path);
                    XElement states = xmlDoc.Element("States");
                    states.Add(new XElement("State",
                        new XElement("Name", saveworkName),
                        new XElement("Timestamp", timestamp + " ms"),
                        new XElement("Active", state),
                        new XElement("TotalFiles", nbFileToTransfer.ToString()),
                        new XElement("TotalLenght", sizeToTransfer + " octets"),
                        new XElement("Progression", progression + "%"),
                        new XElement("RemainingFiles", remainingFile.ToString()),
                        new XElement("RemainingLength", remainingSize + " octets"),
                        new XElement("SourceFile", srcFile),
                        new XElement("DestinationFile", destFile)));
                    xmlDoc.Save(_path);
                }
            }
        }

        /// <summary>
        /// Open the state file with notepad.
        /// </summary>
        public void Recover()
        {
            string _path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..") + $"/Logs/state.xml");
            System.Diagnostics.Process.Start("notepad.exe", _path);
        }
    }
}
