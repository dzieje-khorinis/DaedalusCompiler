using System;
using System.IO;
using System.Collections.Generic;
using Antlr4.Runtime.Tree;
using Common;
using Common.SemanticAnalysis;
using Common.Zen;
using DaedalusCompiler.Compilation;


namespace DaedalusCompiler.Tests
{
    public class TestsHelper
    {
        private int _syntaxErrorsCount;

        private List<List<SyntaxError>> _syntaxErrorsPerFile;
        public Dictionary<string, Symbol> SymbolTable;
        private readonly ErrorLogger _errorLogger;
        private readonly bool _strictSyntax;
        private readonly HashSet<string> _globallySuppressedCodes;

        public TestsHelper(ErrorLogger errorLogger, bool strictSyntax, bool detectUnused)
        {
            _errorLogger = errorLogger;
            _strictSyntax = strictSyntax;
            _globallySuppressedCodes = new HashSet<string>();

            if (!detectUnused)
            {
                _globallySuppressedCodes.Add(UnusedSymbolWarning.WCode);
            }
        }

        public void RunCode(string code, string zenContent)
        {
            ZenLoader zenLoader = new ZenLoader();
            if (zenLoader.Load(zenContent) != 0)
            {
                return;
            }
            List<ZenFileNode> zenFileNodes = zenLoader.ZenFileNodes;

            List<IParseTree> parseTrees = new List<IParseTree>();
            List<string> filesPaths = new List<string>();
            List<string[]> filesContentsLines = new List<string[]>();
            List<HashSet<string>> suppressedWarningCodes = new List<HashSet<string>>();

            _syntaxErrorsCount = 0;
            _syntaxErrorsPerFile = new List<List<SyntaxError>>();

            if (code != "")
            {
                DaedalusParser parser = Compiler.GetParserForText(code);
                SyntaxErrorListener syntaxErrorListener = new SyntaxErrorListener();
                parser.AddErrorListener(syntaxErrorListener);
                parseTrees.Add(parser.daedalusFile());
                _syntaxErrorsCount += syntaxErrorListener.SyntaxErrors.Count;
                _syntaxErrorsPerFile.Add(syntaxErrorListener.SyntaxErrors);
                
                string[] fileContentLines = code.Split(Environment.NewLine);
                filesPaths.Add("test.d");
                filesContentsLines.Add(fileContentLines);
                suppressedWarningCodes.Add(SemanticErrorsCollectingVisitor.GetWarningCodesToSuppress(fileContentLines[0]));
            }
            
            
            if (_syntaxErrorsCount > 0)
            {
                for(int i=0; i<_syntaxErrorsPerFile.Count; ++i) {
                    List<SyntaxError> syntaxErrors = _syntaxErrorsPerFile[i];
                    if (syntaxErrors.Count > 0) {
                        string filePath = filesPaths[i];
                        string fileName = Path.GetFileName(filePath);
                        _errorLogger.LogLine(filePath);
                        foreach(SyntaxError syntaxError in syntaxErrors) {
                            string line = filesContentsLines[i][syntaxError.LineNo-1];
                            syntaxError.Print(fileName, line, _errorLogger);
                        }
                    }
                }

                _errorLogger.LogLine($"{_syntaxErrorsCount} syntax {(_syntaxErrorsCount == 1 ? "error" : "errors")} generated.");
                return;
            }

            SemanticAnalyzer semanticAnalyzer = new SemanticAnalyzer(
                zenFileNodes,
                parseTrees,
                filesPaths,
                filesContentsLines,
                suppressedWarningCodes
            );
            semanticAnalyzer.Run();
            SymbolTable = semanticAnalyzer.SymbolTable;
            
            SemanticErrorsCollectingVisitor semanticErrorsCollectingVisitor = new SemanticErrorsCollectingVisitor(_errorLogger, _strictSyntax, _globallySuppressedCodes);
            semanticErrorsCollectingVisitor.FilePathDisplayStatus = FilePathDisplayStatus.NeverDisplay;
            semanticErrorsCollectingVisitor.VisitTree(semanticAnalyzer.AbstractSyntaxTree);
            
            int errorsCount = semanticErrorsCollectingVisitor.ErrorsCount;
            int warningsCount =semanticErrorsCollectingVisitor.WarningsCount;
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
                return;
            }

            if (warningsCount > 0)
            {
                _errorLogger.LogLine($"{warningsCount} {warning} generated.");
            }
                

            SymbolUpdatingVisitor symbolUpdatingVisitor = new SymbolUpdatingVisitor();
            symbolUpdatingVisitor.VisitTree(semanticAnalyzer.AbstractSyntaxTree);
            
            AssemblyBuildingVisitor assemblyBuildingVisitor = new AssemblyBuildingVisitor(semanticAnalyzer.SymbolTable);
            assemblyBuildingVisitor.VisitTree(semanticAnalyzer.AbstractSyntaxTree);
        }
    }
}