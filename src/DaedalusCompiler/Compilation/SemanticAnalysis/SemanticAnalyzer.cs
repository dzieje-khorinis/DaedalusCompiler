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
        private AbstractSyntaxTree _abstractSyntaxTree;
        private Dictionary<string, DatSymbol> _symbolTable;

        public SemanticAnalyzer(List<IParseTree> parseTrees, int externalFilesCount)
        {
            _symbolTable = null;
            
            Stopwatch timer = new Stopwatch();
            timer.Start();
            _abstractSyntaxTree = new AbstractSyntaxTree(parseTrees, externalFilesCount);
            timer.Stop();
            Console.WriteLine($"AbstractSyntaxTree creation time: {timer.Elapsed}");
        }


        public void CreateSymbolTable()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            SymbolTableCreationVisitor visitor = new SymbolTableCreationVisitor();
            visitor.VisitTree(_abstractSyntaxTree);
            timer.Stop();
            Console.WriteLine($"SymbolTable creation time: {timer.Elapsed}");
        }
        
        public void EvaluateReferencesAndTypesAndArraySize()
        {
            /*
             Calculate DatSymbol properties:
             
             Array: ArrayLength - must be const
             Const: Content
             * For every local and global const (not class attributes) if it has Class as Type,
             set ParentIndex to that Class. 
             */
        }

        public void DetectErrors()
        {
            /*
             New errors:
                * typechecking
                * C_NPC size has to be 800 bytes if declared
             New warnings:
                * used name isn't exacly the same as declared (match-case)
             */
        }
        
    }
}