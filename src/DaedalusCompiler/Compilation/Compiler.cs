using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Antlr4.Runtime.Tree;
using Common;
using Common.SemanticAnalysis;
using Common.Zen;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    public class Compiler
    {
        private readonly List<string> _scriptPaths;
        private readonly string _runtimePath;
        private readonly string _outputPathDat;
        private readonly bool _generateOutputUnits;
        private readonly string _outputPathOuDir;
        private readonly List<string> _zenPaths;
        private readonly bool _strictSyntax;
        private readonly HashSet<string> _globallySuppressedCodes;
        private readonly bool _verbose;

        private readonly OutputUnitsBuilder _ouBuilder;
        public DatFile DatFile;
        private readonly StdErrorLogger _errorLogger;

        public Compiler(CompilationOptions options)
        {
            _scriptPaths = new List<string>();
            
            string absoluteSrcFilePath = Path.GetFullPath(options.SrcFilePath);

            _runtimePath = options.RuntimePath;
            if (_runtimePath == String.Empty)
            {
                string srcFileNameLowerWithoutExtension = Path.GetFileNameWithoutExtension(absoluteSrcFilePath).ToLower();
                _runtimePath = Path.Combine(GetBuiltinsPath(), srcFileNameLowerWithoutExtension + ".d");
            }
    
            if (File.Exists(_runtimePath))
            {
                _scriptPaths.Add(_runtimePath);
            }
            else
            {
                if (_verbose && options.RuntimePath != String.Empty)
                {
                    Console.WriteLine($"Specified runtime {_runtimePath} doesn't exist.");
                }
                _runtimePath = null;
            }
            
            _scriptPaths.AddRange(SrcFileHelper.LoadScriptsFilePaths(absoluteSrcFilePath));

            _outputPathDat = options.OutputPathDat;
            _generateOutputUnits = options.GenerateOutputUnits;
            _outputPathOuDir = options.OutputPathOuDir;
            _zenPaths = options.ZenPaths;
            _strictSyntax = options.StrictSyntax;
            _globallySuppressedCodes = options.GloballySuppressedCodes;
            _verbose = options.Verbose;


            if (_generateOutputUnits)
            {
                _ouBuilder = new OutputUnitsBuilder(_verbose);
            }

            DatFile = null;
            _errorLogger = new StdErrorLogger();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public bool Compile()
        {
            ParseResult parseResult = Parse();
            if (parseResult == null)
            {
                return false;
            }

            AnalyzeResult analyzeResult = Analyze(parseResult);
            if (analyzeResult == null)
            {
                return false;
            }

            Generate(analyzeResult);
            return true;
        }

        public ParseResult Parse()
        {
            ZenLoader zenLoader = new ZenLoader(_zenPaths, _verbose);
            if (zenLoader.Load() != 0)
            {
                return null;
            }

            List<IParseTree> parseTrees = new List<IParseTree>();
            List<string> filesPaths = new List<string>();
            List<string[]> filesContentsLines = new List<string[]>();
            List<string> filesContents = new List<string>();
            List<HashSet<string>> suppressedWarningCodes = new List<HashSet<string>>();

            int syntaxErrorsCount = 0;
            List<List<SyntaxError>> syntaxErrorsPerFile = new List<List<SyntaxError>>();
            
            int runtimeIndex = -1;
            if (File.Exists(_runtimePath))
            {
                runtimeIndex = 0;
            }
            
            for (int i = 0; i < _scriptPaths.Count; i++)
            {
                if (_verbose) Console.WriteLine($"[{i + 1}/{_scriptPaths.Count}]Parsing{(runtimeIndex==i ? " runtime":"")}: {_scriptPaths[i]}");

                string fileContent = GetFileContent(_scriptPaths[i]);
                DaedalusParser parser = GetParserForText(fileContent);

                SyntaxErrorListener syntaxErrorListener = new SyntaxErrorListener();
                parser.RemoveErrorListeners();
                parser.AddErrorListener(syntaxErrorListener);
                parseTrees.Add(parser.daedalusFile());
                syntaxErrorsCount += syntaxErrorListener.SyntaxErrors.Count;
                syntaxErrorsPerFile.Add(syntaxErrorListener.SyntaxErrors);

                string[] fileContentLines = fileContent.Split(Environment.NewLine);
                filesPaths.Add(_scriptPaths[i]);
                filesContentsLines.Add(fileContentLines);
                filesContents.Add(fileContent);

                suppressedWarningCodes.Add(
                    SemanticErrorsCollectingVisitor.GetWarningCodesToSuppress(fileContentLines[0]));
            }

            if (syntaxErrorsCount > 0)
            {
                for (int i = 0; i < syntaxErrorsPerFile.Count; ++i)
                {
                    List<SyntaxError> syntaxErrors = syntaxErrorsPerFile[i];
                    if (syntaxErrors.Count > 0)
                    {
                        string filePath = filesPaths[i];
                        string fileName = Path.GetFileName(filePath);
                        _errorLogger.LogLine(filePath);
                        foreach (SyntaxError syntaxError in syntaxErrors)
                        {
                            string line = filesContentsLines[i][syntaxError.LineNo - 1];
                            syntaxError.Print(fileName, line, _errorLogger);
                        }
                    }
                }

                _errorLogger.LogLine(
                    $"{syntaxErrorsCount} syntax {(syntaxErrorsCount == 1 ? "error" : "errors")} generated.");
                return null;
            }

            return new ParseResult
            {
                ZenFileNodes = zenLoader.ZenFileNodes,
                ParseTrees = parseTrees,
                FilesPaths = filesPaths,
                FilesContentsLines = filesContentsLines,
                FilesContents = filesContents,
                SuppressedWarningCodes = suppressedWarningCodes
            };
        }

        public AnalyzeResult Analyze(ParseResult options)
        {
            SemanticAnalyzer semanticAnalyzer = new SemanticAnalyzer(
                options.ZenFileNodes,
                options.ParseTrees,
                options.FilesPaths,
                options.FilesContentsLines,
                options.SuppressedWarningCodes
            );
            semanticAnalyzer.Run();

            SemanticErrorsCollectingVisitor semanticErrorsCollectingVisitor = new SemanticErrorsCollectingVisitor(
                new StdErrorLogger(),
                _strictSyntax,
                _globallySuppressedCodes);

            semanticErrorsCollectingVisitor.VisitTree(semanticAnalyzer.AbstractSyntaxTree);

            int errorsCount = semanticErrorsCollectingVisitor.ErrorsCount;
            int warningsCount = semanticErrorsCollectingVisitor.WarningsCount;

            string error = errorsCount == 1 ? "error" : "errors";
            string warning = warningsCount == 1 ? "warning" : "warnings";

            if (errorsCount > 0)
            {
                if (warningsCount > 0)
                {
                    _errorLogger.LogLine($"{errorsCount} {error}, {warningsCount} {warning} generated.");
                }
                else
                {
                    _errorLogger.LogLine($"{errorsCount} {error} generated.");
                }

                return null;
            }

            if (warningsCount > 0)
            {
                _errorLogger.LogLine($"{warningsCount} {warning} generated.");
            }

            return new AnalyzeResult
            {
                AbstractSyntaxTree = semanticAnalyzer.AbstractSyntaxTree,
                SymbolTable = semanticAnalyzer.SymbolTable,
                SymbolsWithInstructions = semanticAnalyzer.SymbolsWithInstructions,
                FilesContents = options.FilesContents,
            };
        }

        public void Generate(AnalyzeResult options)
        {
            SymbolUpdatingVisitor symbolUpdatingVisitor = new SymbolUpdatingVisitor();
            symbolUpdatingVisitor.VisitTree(options.AbstractSyntaxTree);

            AssemblyBuildingVisitor assemblyBuildingVisitor = new AssemblyBuildingVisitor(options.SymbolTable);
            assemblyBuildingVisitor.VisitTree(options.AbstractSyntaxTree);

            if (_generateOutputUnits)
            {
                foreach (string filesContent in options.FilesContents)
                {
                    _ouBuilder.ParseText(filesContent);
                }

                _ouBuilder.SaveOutputUnits(_outputPathOuDir);
            }

            DatBuilder datBuilder = new DatBuilder(options.SymbolTable, options.SymbolsWithInstructions);
            DatFile = datBuilder.GetDatFile();
            DatFile.Save(_outputPathDat);
        }


        private string GetBuiltinsPath()
        {
            string programStartPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            return Path.Combine(Path.GetDirectoryName(programStartPath), "DaedalusBuiltins");
        }

        public void SetCompilationDateTimeText(string compilationDateTimeText)
        {
            _ouBuilder.SetGenerationDateTimeText(compilationDateTimeText);
        }

        public void SetCompilationUserName(string userName)
        {
            _ouBuilder.SetGenerationUserName(userName);
        }

        private string GetFileContent(string filePath)
        {
            return File.ReadAllText(filePath, Encoding.GetEncoding(1250));
        }

        public static DaedalusParser GetParserForText(string input)
        {
            AntlrInputStream inputStream = new AntlrInputStream(input);
            DaedalusLexer lexer = new DaedalusLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            return new DaedalusParser(commonTokenStream);
        }
    }

    public class CompilationOptions
    {
        public string SrcFilePath;
        public string RuntimePath;
        public string OutputPathDat;
        public bool GenerateOutputUnits;
        public string OutputPathOuDir;
        public List<string> ZenPaths;
        public bool StrictSyntax;
        public HashSet<string> GloballySuppressedCodes;
        public bool Verbose;
    }

    public class ParseResult
    {
        public List<ZenFileNode> ZenFileNodes;
        public List<IParseTree> ParseTrees;
        public List<string> FilesPaths;
        public List<string[]> FilesContentsLines;
        public List<string> FilesContents;
        public List<HashSet<string>> SuppressedWarningCodes;
    }

    public class AnalyzeResult
    {
        public AbstractSyntaxTree AbstractSyntaxTree;
        public Dictionary<string, Symbol> SymbolTable;
        public List<BlockSymbol> SymbolsWithInstructions;
        public List<string> FilesContents;
    }
}