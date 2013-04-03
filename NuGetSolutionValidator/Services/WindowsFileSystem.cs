using System;
using System.IO;
using System.Linq;

namespace NugetSolutionValidator.Services
{
    public class WindowsFileSystem:IFileSystem
    {
        public string FindFullFilePath(string fileName)
        {
            var currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
            var file = currentDirectory.GetFiles(fileName).FirstOrDefault();
            while (file == null && currentDirectory.Parent != null)
            {
                currentDirectory = currentDirectory.Parent;
                file = currentDirectory.GetFiles(fileName).FirstOrDefault();
            }
            return file == null ? null : file.FullName;
        }

        public string[] ReadFile(string filePath)
        {
            return File.ReadAllLines(filePath);
        }

        public string GetDirectory(string filePath)
        {
            var file = new FileInfo(filePath);
            return file.DirectoryName;
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public TextReader OpenText(string path)
        {
            return File.OpenText(path);
        }
    }
}