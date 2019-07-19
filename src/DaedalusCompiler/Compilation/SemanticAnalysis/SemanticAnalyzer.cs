using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using Antlr4.Runtime.Tree;
using DaedalusCompiler.Compilation.SemanticAnalysis;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    public class SemanticAnalyzer
    {
        public AbstractSyntaxTree AbstractSyntaxTree;     

        
        public readonly List<DatSymbol> Symbols;                        // SymbolTable part 1
        private readonly Dictionary<string, DatSymbol> _symbolsDict;    // SymbolTable part 2

        public SemanticAnalyzer(List<IParseTree> parseTrees, int externalFilesCount)
        {
            Symbols = new List<DatSymbol>();
            _symbolsDict = new Dictionary<string, DatSymbol>();
            
            Stopwatch timer = new Stopwatch();
            timer.Start();
            AbstractSyntaxTree = new AbstractSyntaxTree(parseTrees, externalFilesCount);
            timer.Stop();
            Console.WriteLine($"AbstractSyntaxTree creation time: {timer.Elapsed}");
        }


        public void CreateSymbolTable()
        {
            SymbolTableCreationVisitor astVisitor;
        }

        public void EvaluateReferencesAndTypes()
        {
            
        }

        public void DetectErrors()
        {
            
        }
        
    }
}