using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

        private static IEnumerable<string> LoadScriptsFilePaths(string basePath, string[] srcLines, List<string> alreadyLoadedSrcs)
        {
            List<string> result = new List<string>();

            foreach (var l in srcLines.Where(x => String.IsNullOrWhiteSpace(x) == false))
            {
                try
                {
                    var containsWildcard = l.Contains("*");
                    var fullPath = Path.Combine(basePath, l).Replace('*', 'x').Trim();
                    var pathExtension = Path.GetExtension(fullPath).ToLower();

                    if (containsWildcard && pathExtension == ".d")
                    {
                        var dirPath = Path.GetDirectoryName(fullPath);
                        var dirFiles = Directory.GetFiles(dirPath);
                        var wildcardMath = fullPath.Substring(0, fullPath.Length - 3);

                        var matchFiles = dirFiles
                            .Where(x => Path.GetExtension(x).ToLower() == ".d" && x.IndexOf(wildcardMath) == 0);

                        result.AddRange(matchFiles);
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
                    throw new Exception($"Invalid line {Array.IndexOf(srcLines, l) + 1}: '{l}'", exc);
                }
            }

            return result;
        }
    }
}
