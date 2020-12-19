using System.Diagnostics;
using System.IO;

namespace Core.Model.Type
{
    public class Cryptosoft
    {
        /// <summary>
        /// Create a processus which call Cryptosoft.exe, to encrypt or decrypt a file and put the result it in the specified destination folder.
        /// </summary>
        /// <param name="srcFile">The file to encrypt</param>
        /// <param name="dstFolder">The destination folder</param>
        public int Run(string srcFile, string dstFolder)
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;

            path = Path.Combine(Path.GetDirectoryName(path), @"Plugins\Cryptosoft\Cryptosoft.exe");

            int eCode;
            using (Process cryptosoftProcess = new Process())
            {
                cryptosoftProcess.StartInfo.FileName = path;
                cryptosoftProcess.StartInfo.Arguments = $"\"{srcFile}\" \"{dstFolder}\" 1";
                cryptosoftProcess.StartInfo.RedirectStandardOutput = true;
                cryptosoftProcess.Start();
                cryptosoftProcess.WaitForExit();
                eCode = cryptosoftProcess.ExitCode;
            }
            return eCode;
        }

    }
}