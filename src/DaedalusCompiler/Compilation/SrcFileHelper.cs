using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DaedalusCompiler.Compilation
{
    public static class SrcFileHelper
    {
        public static IEnumerable<string> LoadScriptsFilePaths(string srcFilePath)
        {
            return LoadScriptsFilePaths(srcFilePath, new List<string>());
        }

        private static IEnumerable<string> LoadScriptsFilePaths(string srcFilePath, List<string> alreadyLoadedSrcs)
        {
            if (Path.GetExtension(srcFilePath).ToLower() != ".src")
                throw new Exception($"Invalid SRC file: '{srcFilePath}'.");

            if (alreadyLoadedSrcs.Contains(srcFilePath))
                throw new Exception($"Cyclic dependency detected. SRC file '{srcFilePath}' is already loaded");
            alreadyLoadedSrcs.Add(srcFilePath);

            try
            {
                var lines = File.ReadAllLines(srcFilePath);
                var basePath = Path.GetDirectoryName(srcFilePath);
                var result = LoadScriptsFilePaths(basePath, lines, alreadyLoadedSrcs);
                return result;
            }
            catch (Exception exc)
            {
                throw new Exception($"Error while loading scripts file paths from SRC file '{srcFilePath}'", exc);
            }
        }

        private static IEnumerable<string> LoadScriptsFilePaths(string basePath, string[] srcLines,
            List<string> alreadyLoadedSrcs)
        {
            List<string> result = new List<string>();

            foreach (string line in srcLines.Where(x => String.IsNullOrWhiteSpace(x) == false))
            {
                try
                {
                    bool containsWildcard = line.Contains("*");
                    string fullPath = Path.Combine(basePath, line).Trim();
                    string pathExtension = Path.GetExtension(fullPath).ToLower();

                    if (containsWildcard && pathExtension == ".d")
                    {
                        string dirPath = Path.GetDirectoryName(fullPath);
                        string filenamePattern = Path.GetFileName(fullPath);
                        
                        string[] fileNames = Directory.GetFiles(dirPath, filenamePattern);
                        result.AddRange(fileNames);
                    }
                    else if (pathExtension == ".d")
                    {
                        result.Add(fullPath);
                    }
                    else if (pathExtension == ".src")
                    {
                        result.AddRange(LoadScriptsFilePaths(fullPath, alreadyLoadedSrcs));
                    }
                    else
                    {
                        throw new Exception("Unsupported script file format");
                    }
                }
                catch (Exception exc)
                {
                    throw new Exception($"Invalid line {Array.IndexOf(srcLines, line) + 1}: '{line}'", exc);
                }
            }

            return result;
        }
    }
}