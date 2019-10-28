using System;
using System.Collections.Generic;
using Antlr4.Runtime.Tree;
using DaedalusCompiler.Compilation;
using DaedalusCompiler.Compilation.SemanticAnalysis;

namespace DaedalusCompiler.Tests
{
    public class TestsHelper
    {
        public int SyntaxErrorsCount;
        public Dictionary<string, Symbol> SymbolTable;
        private readonly ErrorLogger _errorLogger;
        private readonly bool _strictSyntax;
        private readonly bool _detectUnused;
        private readonly HashSet<string> _globallySuppressedCodes;

        public TestsHelper(ErrorLogger errorLogger, bool strictSyntax, bool detectUnused)
        {
            _errorLogger = errorLogger;
            _strictSyntax = strictSyntax;
            _detectUnused = detectUnused;
            _globallySuppressedCodes = new HashSet<string>();

            if (!_detectUnused)
            {
                _globallySuppressedCodes.Add(UnusedSymbolWarning.WCode);
            }
        }

        public void RunCode(string code, string externalCode="")
        {
            List<IParseTree> parseTrees = new List<IParseTree>();
            List<string> filesPaths = new List<string>();
            List<string[]> filesContents = new List<string[]>();
            List<HashSet<string>> suppressedWarningCodes = new List<HashSet<string>>();
            int externalFilesCount = 0;
            
            SyntaxErrorsCount = 0;
            
            if (externalCode != "")
            {
                externalFilesCount++;
                
                DaedalusParser parser = Compiler.GetParserForText(externalCode);
                SyntaxErrorListener syntaxErrorListener = new SyntaxErrorListener();
                parser.AddErrorListener(syntaxErrorListener);
                parseTrees.Add(parser.daedalusFile());
                
                string[] fileContentLines = externalCode.Split(Environment.NewLine);
                filesPaths.Add("runtime.d");
                filesContents.Add(fileContentLines);
                suppressedWarningCodes.Add(Compiler.GetWarningCodesToSuppress(fileContentLines[0]));

                SyntaxErrorsCount += syntaxErrorListener.ErrorsCount;
            }


            if (code != "")
            {
                DaedalusParser parser = Compiler.GetParserForText(code);
                SyntaxErrorListener syntaxErrorListener = new SyntaxErrorListener();
                parser.AddErrorListener(syntaxErrorListener);
                parseTrees.Add(parser.daedalusFile());
                
                string[] fileContentLines = code.Split(Environment.NewLine);
                filesPaths.Add("test.d");
                filesContents.Add(fileContentLines);
                suppressedWarningCodes.Add(Compiler.GetWarningCodesToSuppress(fileContentLines[0]));

                SyntaxErrorsCount += syntaxErrorListener.ErrorsCount;
            }
            
            
            if (SyntaxErrorsCount > 0)
            {
                _errorLogger.LogLine($"{SyntaxErrorsCount} syntax {(SyntaxErrorsCount == 1 ? "error" : "errors")} generated.");
                return;
            }

            SemanticAnalyzer semanticAnalyzer = new SemanticAnalyzer(parseTrees, externalFilesCount, filesPaths, filesContents, suppressedWarningCodes);
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