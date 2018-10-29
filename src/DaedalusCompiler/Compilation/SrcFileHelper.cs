using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DaedalusCompiler.Compilation
{
    public static class SrcFileHelper
    {

        public static string[] GetLines(string srcFilePath)
        {
            string[] lines = File.ReadAllLines(srcFilePath);
            for (int i = 0; i < lines.Length; ++i)
            {
                if (lines[i].Contains("//"))
                {
                    lines[i] = lines[i].Split("//").First();
                }
            }
            return lines;
        }
        
        public static IEnumerable<string> LoadScriptsFilePaths(string srcFilePath)
        {
            Path.GetFileName(srcFilePath);
            return LoadScriptsFilePaths(srcFilePath, new HashSet<string>());
        }

        private static IEnumerable<string> LoadScriptsFilePaths(string srcFilePath, HashSet<string> alreadyLoadedFiles)
        {
            if (Path.GetExtension(srcFilePath).ToLower() != ".src")
                throw new Exception($"Invalid SRC file: '{srcFilePath}'.");
            
            if (alreadyLoadedFiles.Contains(srcFilePath))
                throw new Exception($"Cyclic dependency detected. SRC file '{srcFilePath}' is already loaded");
            
            alreadyLoadedFiles.Add(srcFilePath.ToLower());

            try
            {
                var lines = GetLines(srcFilePath);
                var basePath = Path.GetDirectoryName(srcFilePath);
                var result = LoadScriptsFilePaths(basePath, lines, alreadyLoadedFiles);
                return result;
            }
            catch (Exception exc)
            {
                throw new Exception($"Error while loading scripts file paths from SRC file '{srcFilePath}'", exc);
            }
        }

        private static IEnumerable<string> LoadScriptsFilePaths(string basePath, string[] srcLines, HashSet<string> alreadyLoadedFiles)
        {
            List<string> result = new List<string>();

            foreach (string line in srcLines.Where(x => String.IsNullOrWhiteSpace(x) == false).Select(item => item.Replace("\\", "/")))
            {
                try
                {
                    bool containsWildcard = line.Contains("*");
                    string fullPath = Path.Combine(basePath, line).Trim().ToLower().Replace("\\", "/");
                    string pathExtension = Path.GetExtension(fullPath).ToLower();

                    if (containsWildcard && pathExtension == ".d")
                    {
                        string dirPath = Path.GetDirectoryName(fullPath);
                        string filenamePattern = Path.GetFileName(fullPath);

                        List<string> filePaths = Directory.GetFiles(dirPath, filenamePattern, new EnumerationOptions
                        {
                            MatchCasing = MatchCasing.CaseInsensitive
                        }).ToList();
                        
                        // we make custom sort to achieve same sort results independent from OS 
                        filePaths.Sort((a, b) =>
                        {
                            if (a.StartsWith(b))
                            {
                                return a.Length > b.Length ? -1 : 1;
                            }

                            return string.Compare(a, b, StringComparison.OrdinalIgnoreCase);
                        });

                        foreach (string filePath in filePaths)
                        {
                            string filePathLower = filePath.ToLower().Replace("\\", "/");
                            if (!alreadyLoadedFiles.Contains(filePathLower))
                            {
                                alreadyLoadedFiles.Add(filePathLower);
                                result.Add(filePathLower);
                            }
                        }
                    }
                    else if (pathExtension == ".d")
                    {
                        if (!alreadyLoadedFiles.Contains(fullPath))
                        {
                            alreadyLoadedFiles.Add(fullPath);
                            result.Add(fullPath);
                        }
                    }
                    else if (pathExtension == ".src")
                    {
                        result.AddRange(LoadScriptsFilePaths(fullPath, alreadyLoadedFiles));
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
