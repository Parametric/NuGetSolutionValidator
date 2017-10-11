using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NugetSolutionValidator.Services
{
    public class WindowsFileSystem:IFileSystem
    {
        public string FindFullFilePath(string fileName)
        {
            return SearchPathProviders()
                .Select(searchpath => FindFullFilePath(fileName, searchpath))
                .FirstOrDefault(filepath => filepath != null);
        }

        protected IEnumerable<FileSystemInfo> SearchPathProviders()
        {

            yield return new DirectoryInfo(Environment.CurrentDirectory);

            var executingAssembly = Assembly.GetExecutingAssembly();
            yield return GetPathAsseblyCodePath(executingAssembly);

        }

        private static FileInfo GetPathAsseblyCodePath(Assembly assembly)
        {
            if (assembly == null) return null;

            var codepath = assembly.CodeBase;
            var escapedCodepath = Uri.EscapeUriString(codepath);
            var uri = new Uri(escapedCodepath);

            var unescapedPathAndQuery = Uri.UnescapeDataString(uri.PathAndQuery);
            var unescapedFragment = Uri.UnescapeDataString(uri.Fragment);

            var stringPath = $"{unescapedPathAndQuery}{unescapedFragment}";
            var fileInfo = new FileInfo(stringPath);
            return fileInfo;
        }

        private static string FindFullFilePath(string fileName, FileSystemInfo searchFrom)
        {
            var fileInfo = searchFrom as FileInfo;
            var currentDirectory = fileInfo?.Directory ?? searchFrom as DirectoryInfo;

            FileInfo file = null;
            while (file == null && currentDirectory != null) {
                file = currentDirectory.GetFiles(fileName).FirstOrDefault();
                currentDirectory = currentDirectory.Parent;
            }
            return file?.FullName;

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