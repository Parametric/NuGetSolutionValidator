﻿using System.IO;

namespace NugetSolutionValidator.Services
{
    public interface IFileSystem
    {
        string FindFullFilePath(string fileName);

        string[] ReadFile(string filePath);
        string GetDirectory(string filePath);
        bool Exists(string path);
        TextReader OpenText(string path);
    }
}