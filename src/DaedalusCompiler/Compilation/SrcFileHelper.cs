using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            
            if (alreadyLoadedFiles.Contains(srcFilePath.ToLower()))
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


        private static string GetDirPathInsensitive(string basePath, string relativePath)
        {
            string resultPath = basePath;

            EnumerationOptions options = new EnumerationOptions {MatchCasing = MatchCasing.CaseInsensitive};
            string[] relativePathSplitted = relativePath.Split(Path.DirectorySeparatorChar);
            int lastIndex = relativePathSplitted.Length - 1;

            for (int i = 0; i < lastIndex; i++)
            {
                string relativePathPart = relativePathSplitted[i];
                string[] directories = Directory.GetDirectories(resultPath, relativePathPart, options);
                if (directories.Length == 0)
                {
                    throw new DirectoryNotFoundException($"ERROR: Could not find a part of the path '{Path.Combine(resultPath, relativePathPart)}'");
                }

                if (directories.Length > 1)
                {
                    throw new DirectoryNotFoundException($"ERROR: Ambigous path '{Path.Combine(resultPath, relativePathPart)}'. Matches {String.Join(";", directories)}");;
                }

                resultPath = Path.Combine(resultPath, directories.First());
            }
            return resultPath;
        }

        private static List<string> GetFilesInsensitive(string dirPath, string filenamePattern)
        {
            EnumerationOptions options = new EnumerationOptions {MatchCasing = MatchCasing.CaseInsensitive};
            List<string> filePaths = Directory.GetFiles(dirPath, filenamePattern, options).ToList();
            if (filePaths.Count == 0)
            {
                throw new FileNotFoundException($"ERROR: Could not find any files in '{dirPath}' that matches pattern '{filenamePattern}'");
            }

            return filePaths;
        }

        private static string GetFileInsensitive(string dirPath, string filenamePattern)
        {
            List<string> filePaths = GetFilesInsensitive(dirPath, filenamePattern);
            if (filePaths.Count > 1)
            {
                throw new DirectoryNotFoundException($"ERROR: Ambigous path '{Path.Combine(dirPath, filenamePattern)}'. Matches {String.Join(";", filePaths)}");;
            }

            return filePaths.First();
        }

        private static IEnumerable<string> LoadScriptsFilePaths(string basePath, string[] srcLines, HashSet<string> alreadyLoadedFiles)
        {
            List<string> result = new List<string>();

            foreach (string line in srcLines.Where(x => String.IsNullOrWhiteSpace(x) == false).Select(item => Path.Combine(item.Trim().Split("\\").ToArray())))
            {
                try
                {
                    bool containsWildcard = line.Contains("*");
                    string relativePath = line;
                    string dirPath = GetDirPathInsensitive(basePath, relativePath);
                    string filenamePattern = Path.GetFileName(relativePath);
                    string pathExtensionLower = Path.GetExtension(filenamePattern).ToLower();

                    if (containsWildcard && pathExtensionLower == ".d")
                    {
                        List<string> filePaths = GetFilesInsensitive(dirPath, filenamePattern);
                        
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
                            string filePathLower = filePath.ToLower();
                            if (!alreadyLoadedFiles.Contains(filePathLower))
                            {
                                alreadyLoadedFiles.Add(filePathLower);
                                result.Add(filePath);
                            }
                        }
                    }
                    else if (pathExtensionLower == ".d")
                    {
                        string fullPath = GetFileInsensitive(dirPath, filenamePattern);

                        string fullPathLower = fullPath.ToLower();
                        if (!alreadyLoadedFiles.Contains(fullPathLower))
                        {
                            alreadyLoadedFiles.Add(fullPathLower);
                            result.Add(fullPath);
                        }
                    }
                    else if (pathExtensionLower == ".src")
                    {
                        string fullPath = GetFileInsensitive(dirPath, filenamePattern);
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
