using System.IO;

namespace AnagramServer
{
    public static class FileSearcher
    {
        public static string FindFile(string root, string fileName)
        {
            string[] files = Directory.GetFiles(
                root,
                fileName,
                SearchOption.AllDirectories
            );

            if (files.Length == 0)
            {
                return null;
            }

            return files[0];
        }
    }
}