using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Common.SemanticAnalysis;

namespace Common.Zen
{
    public class SyntaxErrorException : Exception
    {
    }

    public class ZenLoader
    {
        private readonly bool _verbose;
        private List<string> _zenPaths;
        private List<IParseTree> _parseTrees;

        private List<string[]> _filesContentsLines;
        private int _syntaxErrorsCount;
        private List<List<SyntaxError>> _syntaxErrorsPerFile = new List<List<SyntaxError>>();
        public readonly List<ZenFileNode> ZenFileNodes;

        public ZenLoader(bool verbose = false)
        {
            _zenPaths = new List<string> {"test.zen"};
            _parseTrees = new List<IParseTree>();
            _filesContentsLines = new List<string[]>();
            ZenFileNodes = new List<ZenFileNode>();
            _verbose = verbose;
        }

        public ZenLoader(List<string> zenPaths, bool verbose = false) : this(verbose)
        {
            try
            {
                _zenPaths = LoadFilePaths(zenPaths);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }
        }

        public int Load()
        {
            for (int i = 0; i < _zenPaths.Count; i++)
            {
                if (_verbose) Console.WriteLine($"[{i + 1}/{_zenPaths.Count}]Parsing ZEN: {_zenPaths[i]}");
                string path = _zenPaths[i];
                string zenContent = File.ReadAllText(path, Encoding.GetEncoding(1250));
                _parseTrees.Add(Parse(zenContent));
            }

            try
            {
                HandleSyntaxErrors();
            }
            catch (SyntaxErrorException)
            {
                return 1;
            }

            GenerateZenFileNodes();
            return 0;
        }

        public int Load(string zenContent)
        {
            if (zenContent == null)
            {
                return 0;
            }

            if (_verbose) Console.WriteLine("[1/1]Parsing ZEN: test.zen");
            _parseTrees.Add(Parse(zenContent));
            try
            {
                HandleSyntaxErrors();
            }
            catch (SyntaxErrorException)
            {
                return 1;
            }

            GenerateZenFileNodes();
            return 0;
        }

        private IParseTree Parse(string zenContent)
        {
            string[] fileContentLines = zenContent.Split(Environment.NewLine);
            _filesContentsLines.Add(fileContentLines);

            AntlrInputStream inputStream = new AntlrInputStream(zenContent);
            ZenLexer lexer = new ZenLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            ZenParser parser = new ZenParser(commonTokenStream);
            SyntaxErrorListener syntaxErrorListener = new SyntaxErrorListener();
            parser.RemoveErrorListeners();
            parser.AddErrorListener(syntaxErrorListener);
            IParseTree parseTree = parser.main();
            _syntaxErrorsCount += syntaxErrorListener.SyntaxErrors.Count;
            _syntaxErrorsPerFile.Add(syntaxErrorListener.SyntaxErrors);
            return parseTree;
        }

        private void HandleSyntaxErrors()
        {
            StdErrorLogger logger = new StdErrorLogger();

            if (_syntaxErrorsCount > 0)
            {
                for (int i = 0; i < _syntaxErrorsPerFile.Count; ++i)
                {
                    List<SyntaxError> syntaxErrors = _syntaxErrorsPerFile[i];
                    if (syntaxErrors.Count > 0)
                    {
                        string filePath = _zenPaths[i];
                        string fileName = Path.GetFileName(filePath);
                        logger.LogLine(filePath);
                        foreach (SyntaxError syntaxError in syntaxErrors)
                        {
                            string line = _filesContentsLines[i][syntaxError.LineNo - 1];
                            if (line.Length > 255)
                            {
                                line = line.Substring(0, 255) + "...";
                                if (syntaxError.ColumnNo > 255)
                                {
                                    syntaxError.ColumnNo = 257;
                                }
                            }

                            syntaxError.Print(fileName, line, logger);
                        }
                    }
                }

                logger.LogLine(
                    $"{_syntaxErrorsCount} syntax {(_syntaxErrorsCount == 1 ? "error" : "errors")} generated.");
                throw new SyntaxErrorException();
            }

            _zenPaths = null;
            _filesContentsLines = null;
            _syntaxErrorsPerFile = null;
        }

        private void GenerateZenFileNodes()
        {
            foreach (IParseTree parseTree in _parseTrees)
            {
                ZenParseTreeVisitor visitor = new ZenParseTreeVisitor();
                ZenFileNode fileNode = (ZenFileNode) visitor.Visit(parseTree);
                ZenFileNodes.Add(fileNode);
            }

            _parseTrees = null;
        }

        private static List<string> GetFilesInsensitive(string dirPath, string filenamePattern)
        {
            EnumerationOptions options = new EnumerationOptions {MatchCasing = MatchCasing.CaseInsensitive};
            List<string> filePaths = Directory.GetFiles(dirPath, filenamePattern, options).ToList();
            if (filePaths.Count == 0)
            {
                throw new FileNotFoundException(
                    $"ZenLoader ERROR: Could not find any files in '{dirPath}' that matches pattern '{filenamePattern}'");
            }

            return filePaths;
        }

        private static string GetFileInsensitive(string dirPath, string filenamePattern)
        {
            List<string> filePaths = GetFilesInsensitive(dirPath, filenamePattern);
            if (filePaths.Count > 1)
            {
                throw new DirectoryNotFoundException(
                    $"ZenLoader ERROR: Ambiguous path '{Path.Combine(dirPath, filenamePattern)}'. Matches {String.Join(";", filePaths)}");
            }

            return filePaths.First();
        }

        private static List<string> LoadFilePaths(List<string> zenPaths)
        {
            List<string> result = new List<string>();

            foreach (string path in zenPaths)
            {
                string dirPath = Path.GetDirectoryName(path);
                string filenamePattern = Path.GetFileName(path);
                bool containsWildcard = filenamePattern.Contains("*");

                string pathExtensionLower = Path.GetExtension(filenamePattern).ToLower();

                if (containsWildcard && pathExtensionLower == ".zen")
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
                        result.Add(filePath);
                    }
                }
                else if (pathExtensionLower == ".zen")
                {
                    string fullPath = GetFileInsensitive(dirPath, filenamePattern);
                    result.Add(fullPath);
                }
                else
                {
                    throw new Exception(
                        $"ZenLoader ERROR: invalid file format '{pathExtensionLower}', required '.zen'");
                }
            }

            return result;
        }
    }
}