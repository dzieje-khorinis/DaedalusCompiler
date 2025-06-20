using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Antlr4.Runtime.Tree;
using Common;
using Common.SemanticAnalysis;
using Common.Zen;
using DaedalusCompiler.Dat;
using DaedalusCompiler.Resources;

namespace DaedalusCompiler.Compilation
{
    public class Compiler
    {
        private readonly List<ScriptResource> _scriptResources = new();

        private readonly CompilationOptions _compilerOptions;

        private readonly OutputUnitsBuilder _ouBuilder;
        public DatFile DatFile;
        private readonly StdErrorLogger _errorLogger;

        public Compiler(CompilationOptions options)
        {
            _compilerOptions = options;
            var absoluteSrcFilePath = Path.GetFullPath(options.SrcFilePath);
            AddBuiltinTypesToScriptList(absoluteSrcFilePath);
            var scriptPaths = SrcFileHelper.LoadScriptsFilePaths(absoluteSrcFilePath);
            _scriptResources.AddRange(scriptPaths.Select(ScriptResource.Of));

            if (_compilerOptions.GenerateOutputUnits)
            {
                _ouBuilder = new OutputUnitsBuilder(_compilerOptions.Verbose);
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
            ZenLoader zenLoader = new ZenLoader(_compilerOptions.ZenPaths, _compilerOptions.Verbose);
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

            for (int i = 0; i < _scriptResources.Count; i++)
            {
                var scriptResource = _scriptResources[i];
                if (_compilerOptions.Verbose)
                    Console.WriteLine(
                        $"[{i + 1}/{_scriptResources.Count}]Parsing{(scriptResource.IsBuiltIn ? " runtime" : "")}: {scriptResource.Path}");

                string fileContent = GetFileContent(scriptResource);
                DaedalusParser parser = GetParserForText(fileContent);

                SyntaxErrorListener syntaxErrorListener = new SyntaxErrorListener();
                parser.RemoveErrorListeners();
                parser.AddErrorListener(syntaxErrorListener);
                parseTrees.Add(parser.daedalusFile());
                syntaxErrorsCount += syntaxErrorListener.SyntaxErrors.Count;
                syntaxErrorsPerFile.Add(syntaxErrorListener.SyntaxErrors);

                string[] fileContentLines = fileContent.Split(Environment.NewLine);
                filesPaths.Add(scriptResource.Path);
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
                _compilerOptions.StrictSyntax,
                _compilerOptions.GloballySuppressedCodes);

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

            if (_compilerOptions.GenerateOutputUnits)
            {
                foreach (string filesContent in options.FilesContents)
                {
                    _ouBuilder.ParseText(filesContent);
                }

                _ouBuilder.SaveOutputUnits(_compilerOptions.OutputPathOuDir);
            }

            DatBuilder datBuilder = new DatBuilder(options.SymbolTable, options.SymbolsWithInstructions);
            DatFile = datBuilder.GetDatFile();
            DatFile.Save(_compilerOptions.OutputPathDat, Encoding.GetEncoding(_compilerOptions.DstEncoding));
        }


        public void SetCompilationDateTimeText(string compilationDateTimeText)
        {
            _ouBuilder.SetGenerationDateTimeText(compilationDateTimeText);
        }

        public void SetCompilationUserName(string userName)
        {
            _ouBuilder.SetGenerationUserName(userName);
        }

        private string GetFileContent(ScriptResource scriptResource)
        {
            return scriptResource.IsBuiltIn
                ? ResourcesHelper.Get(scriptResource.Path)
                : File.ReadAllText(scriptResource.Path, Encoding.GetEncoding(_compilerOptions.SrcEncoding));
        }

        public static DaedalusParser GetParserForText(string input)
        {
            AntlrInputStream inputStream = new AntlrInputStream(input);
            DaedalusLexer lexer = new DaedalusLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            return new DaedalusParser(commonTokenStream);
        }

        private void AddBuiltinTypesToScriptList(string srcPath)
        {
            if (_compilerOptions.SkipBuiltinTypes) return;
            switch (Path.GetFileNameWithoutExtension(srcPath).ToLower())
            {
                case "menu":
                    _scriptResources.Add(MenuD);
                    break;
                case "gothic":
                    _scriptResources.Add(GothicD);
                    break;
            }
        }

        private static readonly ScriptResource MenuD = new()
        {
            IsBuiltIn = true,
            Path = ResourcesHelper.GetPath(ResourceType.MenuD)
        };

        private static readonly ScriptResource GothicD = new()
        {
            IsBuiltIn = true,
            Path = ResourcesHelper.GetPath(ResourceType.GothicD)
        };
    }


    public class CompilationOptions
    {
        public string SrcFilePath = string.Empty;
        public string OutputPathDat = string.Empty;
        public bool GenerateOutputUnits = false;
        public string OutputPathOuDir = "output";
        public List<string> ZenPaths = new();
        public bool StrictSyntax = false;
        public bool SkipBuiltinTypes = false;

        public HashSet<string> GloballySuppressedCodes = new()
        {
            NamesNotMatchingCaseWiseWarning.WCode,
            UnusedSymbolWarning.WCode
        };

        public bool Verbose = false;
        public string SrcEncoding = "Windows-1250";
        public string DstEncoding = "Windows-1250";
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

    public class ScriptResource
    {
        public string Path;
        public bool IsBuiltIn;
        public static ScriptResource Of(string path) => new() { Path = path };
    }
}