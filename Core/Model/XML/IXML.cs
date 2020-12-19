namespace Core.Model.XML
{
    public interface IXML
    {

        /// <summary>
        /// Save data into the XML file.
        /// </summary>
        public void Save();
        /// <summary>
        /// Save data from the XML file.
        /// </summary>
        public void Recover();
        /// <summary>
        /// Delete data in the XML file.
        /// </summary>
        public void Delete();
    }
}
