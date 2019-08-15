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
        private readonly ErrorLogger _errorLogger;
        private readonly bool _strictSyntax;

        public TestsHelper(ErrorLogger errorLogger, bool strictSyntax)
        {
            _errorLogger = errorLogger;
            _strictSyntax = strictSyntax;
        }

        public void RunCode(string code)
        {
            string[] fileContentLines = code.Split(Environment.NewLine);
            List<IParseTree> parseTrees = new List<IParseTree>();
            List<string> filesPaths = new List<string>{"test.d"};
            List<string[]> filesContents = new List<string[]>{fileContentLines};
            List<HashSet<string>> suppressedWarningCodes = new List<HashSet<string>>
            {
                Compiler.GetWarningCodesToSuppress(fileContentLines[0])
            };
            
            DaedalusParser parser = Compiler.GetParserForText(code);
            SyntaxErrorListener syntaxErrorListener = new SyntaxErrorListener();
            parser.AddErrorListener(syntaxErrorListener);
            parseTrees.Add(parser.daedalusFile());
            
            SyntaxErrorsCount = syntaxErrorListener.ErrorsCount;

            if (SyntaxErrorsCount > 0)
            {
                _errorLogger.LogLine($"{SyntaxErrorsCount} syntax {(SyntaxErrorsCount == 1 ? "error" : "errors")} generated.");
                return;
            }

            SemanticAnalyzer semanticAnalyzer = new SemanticAnalyzer(parseTrees, 0, filesPaths, filesContents, suppressedWarningCodes);
            semanticAnalyzer.Run();
            
            SemanticErrorsCollectingVisitor semanticErrorsCollectingVisitor = new SemanticErrorsCollectingVisitor(_errorLogger, _strictSyntax);
            semanticErrorsCollectingVisitor.FilePathDisplayStatus = FilePathDisplayStatus.NeverDisplay;
            semanticErrorsCollectingVisitor.VisitTree(semanticAnalyzer.AbstractSyntaxTree);
        }
    }
}