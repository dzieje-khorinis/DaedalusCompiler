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

namespace DaedalusCompiler.Compilation
{
    public class Compiler
    {
        private readonly OutputUnitsBuilder _ouBuilder;
        private readonly string _outputPathOuDir;
        private readonly bool _strictSyntax;
        private readonly HashSet<string> _globallySuppressedCodes;

        public DatFile DatFile;


        public Compiler(string outputPathOuDir, bool verbose, bool strictSyntax,
            HashSet<string> globallySuppressedCodes)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _ouBuilder = new OutputUnitsBuilder(verbose);
            _outputPathOuDir = outputPathOuDir;
            _strictSyntax = strictSyntax;
            _globallySuppressedCodes = globallySuppressedCodes;
            DatFile = null;
        }

        private string GetBuiltinsPath()
        {
            string programStartPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            return Path.Combine(Path.GetDirectoryName(programStartPath), "DaedalusBuiltins");
        }

        public bool CompileFromSrc(
            List<string> zenPaths,
            string srcFilePath,
            string runtimePath,
            string outputPathDat,
            bool verbose = true,
            bool generateOutputUnits = true
        )
        {
            ZenLoader zenLoader = new ZenLoader(zenPaths, verbose);
            if (zenLoader.Load() != 0)
            {
                return false;
            }

            List<ZenFileNode> zenFileNodes = zenLoader.ZenFileNodes;

            bool isRunTimePathSpecified = runtimePath != String.Empty;

            var absoluteSrcFilePath = Path.GetFullPath(srcFilePath);

            string[] paths = SrcFileHelper.LoadScriptsFilePaths(absoluteSrcFilePath).ToArray();
            string srcFileName = Path.GetFileNameWithoutExtension(absoluteSrcFilePath).ToLower();

            if (!isRunTimePathSpecified)
            {
                runtimePath = Path.Combine(GetBuiltinsPath(), srcFileName + ".d");
            }

            List<IParseTree> parseTrees = new List<IParseTree>();

            List<string> filesPaths = new List<string>();
            List<string[]> filesContentsLines = new List<string[]>();
            List<string> filesContents = new List<string>();
            List<HashSet<string>> suppressedWarningCodes = new List<HashSet<string>>();

            int syntaxErrorsCount = 0;
            List<List<SyntaxError>> syntaxErrorsPerFile = new List<List<SyntaxError>>();

            if (File.Exists(runtimePath))
            {
                if (verbose) Console.WriteLine($"[0/{paths.Length}]Parsing runtime: {runtimePath}");

                string fileContent = GetFileContent(runtimePath);
                DaedalusParser parser = GetParserForText(fileContent);

                SyntaxErrorListener syntaxErrorListener = new SyntaxErrorListener();
                parser.RemoveErrorListeners();
                parser.AddErrorListener(syntaxErrorListener);
                parseTrees.Add(parser.daedalusFile());
                syntaxErrorsCount += syntaxErrorListener.SyntaxErrors.Count;
                syntaxErrorsPerFile.Add(syntaxErrorListener.SyntaxErrors);

                string[] fileContentLines = fileContent.Split(Environment.NewLine);
                filesPaths.Add(runtimePath);
                filesContentsLines.Add(fileContentLines);
                suppressedWarningCodes.Add(
                    SemanticErrorsCollectingVisitor.GetWarningCodesToSuppress(fileContentLines[0]));
            }
            else if (isRunTimePathSpecified)
            {
                if (verbose) Console.WriteLine($"Specified runtime {runtimePath} doesn't exist.");
            }

            for (int i = 0; i < paths.Length; i++)
            {
                if (verbose) Console.WriteLine($"[{i + 1}/{paths.Length}]Parsing: {paths[i]}");

                string fileContent = GetFileContent(paths[i]);
                DaedalusParser parser = GetParserForText(fileContent);

                SyntaxErrorListener syntaxErrorListener = new SyntaxErrorListener();
                parser.RemoveErrorListeners();
                parser.AddErrorListener(syntaxErrorListener);
                parseTrees.Add(parser.daedalusFile());
                syntaxErrorsCount += syntaxErrorListener.SyntaxErrors.Count;
                syntaxErrorsPerFile.Add(syntaxErrorListener.SyntaxErrors);

                string[] fileContentLines = fileContent.Split(Environment.NewLine);
                filesPaths.Add(paths[i]);
                filesContentsLines.Add(fileContentLines);
                filesContents.Add(fileContent);

                suppressedWarningCodes.Add(
                    SemanticErrorsCollectingVisitor.GetWarningCodesToSuppress(fileContentLines[0]));
            }

            StdErrorLogger logger = new StdErrorLogger();

            if (syntaxErrorsCount > 0)
            {
                for (int i = 0; i < syntaxErrorsPerFile.Count; ++i)
                {
                    List<SyntaxError> syntaxErrors = syntaxErrorsPerFile[i];
                    if (syntaxErrors.Count > 0)
                    {
                        string filePath = filesPaths[i];
                        string fileName = Path.GetFileName(filePath);
                        logger.LogLine(filePath);
                        foreach (SyntaxError syntaxError in syntaxErrors)
                        {
                            string line = filesContentsLines[i][syntaxError.LineNo - 1];
                            syntaxError.Print(fileName, line, logger);
                        }
                    }
                }

                logger.LogLine(
                    $"{syntaxErrorsCount} syntax {(syntaxErrorsCount == 1 ? "error" : "errors")} generated.");
                return false;
            }

            if (verbose) Console.WriteLine("parseTrees created");

            SemanticAnalyzer semanticAnalyzer = new SemanticAnalyzer(
                zenFileNodes,
                parseTrees,
                new DaedalusParseTreeVisitor(),
                filesPaths,
                filesContentsLines,
                suppressedWarningCodes
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
                    logger.LogLine($"{errorsCount} {error}, {warningsCount} {warning} generated.");
                }
                else
                {
                    logger.LogLine($"{errorsCount} {error} generated.");
                }

                return false;
            }

            if (warningsCount > 0)
            {
                logger.LogLine($"{warningsCount} {warning} generated.");
            }

            SymbolUpdatingVisitor symbolUpdatingVisitor = new SymbolUpdatingVisitor();
            symbolUpdatingVisitor.VisitTree(semanticAnalyzer.AbstractSyntaxTree);

            AssemblyBuildingVisitor assemblyBuildingVisitor = new AssemblyBuildingVisitor(semanticAnalyzer.SymbolTable);
            assemblyBuildingVisitor.VisitTree(semanticAnalyzer.AbstractSyntaxTree);

            if (verbose) Console.WriteLine($"parseTrees.Count: {parseTrees.Count}");


            if (generateOutputUnits)
            {
                foreach (string filesContent in filesContents)
                {
                    _ouBuilder.ParseText(filesContent);
                }

                _ouBuilder.SaveOutputUnits(_outputPathOuDir);
            }

            DatBuilder datBuilder =
                new DatBuilder(semanticAnalyzer.SymbolTable, semanticAnalyzer.SymbolsWithInstructions);
            DatFile = datBuilder.GetDatFile();
            DatFile.Save(outputPathDat);

            return true;
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
}