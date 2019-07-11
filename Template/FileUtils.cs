using System.IO;

namespace templated {
    public class FileUtils
    {
        public static string UniquePath(string fullPath){
            int count = 1;

            string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
            string extension = Path.GetExtension(fullPath);
            string path = Path.GetDirectoryName(fullPath);
            string newFullPath = fullPath;

            while(File.Exists(newFullPath)) 
            {
                string tempFileName = string.Format("{0}({1}){2}", fileNameOnly, count++, extension);
                newFullPath = Path.Combine(path, tempFileName);
            }
            return newFullPath;
        }
    }
}