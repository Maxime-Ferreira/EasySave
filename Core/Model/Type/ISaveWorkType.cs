using System.Collections.Generic;
using System.Threading;

namespace Core.Model.Type
{
    public interface ISaveWorkType
    {
        /// <summary>
        /// Sorts files in priority files lists and run a ISaveWorkType save.
        /// </summary>
        /// <param name="srcFolder">source folder</param>
        /// <param name="dstFolder">destination folder</param>
        /// <param name="extensions">The list of cryptosoft extensions</param>
        /// <param name="priorityExtensions">The list of priority extensions</param>
        /// <param name="barrier"></param>
        public void SaveData(string srcFolder, string dstFolder, List<string> extensions, List<string> priorityExtensions, Barrier barrier);
        public StateFileContent GetStateFileContent();
        public string GetType();
    }
}
