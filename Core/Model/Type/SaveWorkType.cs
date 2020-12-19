using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Core.Model.Type
{
    public abstract class SaveWorkType : ISaveWorkType
    {
        readonly Cryptosoft cryptosoft = new Cryptosoft();

        /// <summary>
        /// Gets the relative path of the source folder
        /// </summary>
        /// <param name="srcFolder">the source folder</param>
        /// <param name="srcSubFolder"></param>
        /// <returns>the relative path of the source folder</returns>
        public string GetRelativePath(Uri srcFolder, Uri srcSubFolder)
        {
            return srcFolder.MakeRelativeUri(srcSubFolder).ToString();
        }

        /// <summary>
        /// Gets the combined path given by this order : srcFolder where the path contains a head and relativePath.
        /// </summary>
        /// <param name="srcFolder"></param>
        /// <param name="relativePath"></param>
        /// <returns>an absolute path made with the two path</returns>
        public string GetCombinedPath(string srcFolder, string relativePath)
        {
            return Path.Combine(srcFolder, relativePath).Replace("/", @"\");
        }

        /// <summary>
        /// Get the saveWork type.
        /// </summary>
        /// <returns>A saveWork type.</returns>
        //public abstract string ISaveWorkType.GetType();

        /// <summary>
        /// Encrypt specified file and copy it in specified dstFolder.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="dstFolder"></param>
        /// <returns>Encryption time.</returns>
        public int EncryptFile(string file, string dstFolder)
        {
            return cryptosoft.Run(file, dstFolder);
        }

        /// <summary>
        /// Get the stateFileContent of the saveWork.
        /// </summary>
        /// <returns>A stateFileContent</returns>
        public abstract StateFileContent GetStateFileContent();

        /// <summary>
        /// Copy all the folders between source folder and destination folder.
        /// Extensions list set to null is a non encrypt differential
        /// extensions list not empty is a encrypt differential.
        /// </summary>
        /// <param name="srcFolder">source folder</param>
        /// <param name="dstFolder">destination folder</param>
        /// <param name="extensions">The list of cryptosoft extensions</param>
        /// <param name="priorityExtensions">The list of priority extensions</param>
        /// <param name="barrier"></param>
        /// <param name="businessSoft">Instance of the businessSoft class</param>
        public abstract void SaveData(string srcFolder, string dstFolder, List<string> extensions, List<string> priorityExtensions, Barrier barrier);


        /// <summary>
        /// Get the type of the saveWork.
        /// </summary>
        /// <returns>A saveWork type</returns>
        string ISaveWorkType.GetType()
        {
            return "";
        }
    }
}
