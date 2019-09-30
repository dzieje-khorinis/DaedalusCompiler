using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Numerics;
using Antlr4.Runtime.Tree;
using DaedalusCompiler.Compilation.SemanticAnalysis;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    /*
    public class SymbolContext
    {
        public DatSymbol Symbol;
        public ASTNode Node; //DeclarationNode or StringLiteralNode
    }
    */

    public class SemanticAnalyzer
    {
        public readonly AbstractSyntaxTree AbstractSyntaxTree;
        private Dictionary<string, Symbol> _symbolTable;
        
        public SemanticAnalyzer(List<IParseTree> parseTrees, int externalFilesCount, List<string> filesPaths, List<string[]> filesContents, List<HashSet<string>> suppressedWarningCodes)
        {
            _symbolTable = null;

            Stopwatch timer = new Stopwatch();
            timer.Start();
            AbstractSyntaxTree = new AbstractSyntaxTree(parseTrees, externalFilesCount, filesPaths, filesContents, suppressedWarningCodes);
            timer.Stop();
            Console.WriteLine($"AbstractSyntaxTree creation time: {timer.Elapsed}");
        }


        public void Run()
        {
            //Stopwatch timer = new Stopwatch();
            //timer.Start();
            //SymbolTableCreationVisitor symbolTableCreationVisitor = new SymbolTableCreationVisitor();
            //symbolTableCreationVisitor.VisitTree(_abstractSyntaxTree);
            //timer.Stop();
            //Console.WriteLine($"SymbolTable creation time: {timer.Elapsed}");
            //_symbolTable = visitor.SymbolTable;
            
            
            // RedefinedIdentifierError
            // KeywordUsedAsNameError
            SymbolTableCreationVisitor symbolTableCreationVisitor = new SymbolTableCreationVisitor();
            symbolTableCreationVisitor.VisitTree(AbstractSyntaxTree);
            _symbolTable = symbolTableCreationVisitor.SymbolTable;
            
            // UnknownTypeNameError
            // UnsupportedTypeError
            // UnsupportedArrayTypeError
            // UnsupportedFunctionTypeError
            TypeResolver typeResolver = new TypeResolver(_symbolTable);
            typeResolver.Resolve(symbolTableCreationVisitor.TypedSymbols);
            
            // NotClassOrPrototypeReferenceError
            // UndeclaredIdentifierError
            // InfiniteReferenceLoopError
            InheritanceResolver inheritanceResolver = new InheritanceResolver(_symbolTable);
            inheritanceResolver.Resolve(symbolTableCreationVisitor.SubclassSymbols);

            // InfiniteAttributeReferenceLoopError
            PrefixAttributesSymbolCreator prefixAttributesSymbolCreator = new PrefixAttributesSymbolCreator(_symbolTable);
            prefixAttributesSymbolCreator.Scan(symbolTableCreationVisitor.ClassSymbols);

            // UndeclaredIdentifierError
            // AccessToAttributeOfArrayElementNotSupportedError
            // AttributeOfNonInstanceError
            // ClassDoesNotHaveAttributeError
            // ReferencedSymbolIsNotArrayError
            ReferenceResolvingVisitor referenceResolvingVisitor = new ReferenceResolvingVisitor(_symbolTable);
            referenceResolvingVisitor.Visit(AbstractSyntaxTree.ReferenceNodes);
            
            // InfiniteConstReferenceLoopError
            // ArraySizeEqualsZeroError
            // TooBigArraySizeError
            // ArraySizeNotConstIntegerError
            // InconsistentConstArraySizeError
            // IndexOutOfRangeError
            // TooBigArrayIndex
            // ConstIntegerExpectedError
            // ArrayIndexNotConstIntegerError
            // ArithmeticOperationOverflowError
            // DivideByZeroError
            // InvalidUnaryOperationError
            // InvalidBinaryOperationError
            // IntegerLiteralTooLargeError
            // CannotInitializeConstWithValueOfDifferentTypeError
            // CannotInitializeArrayElementWithValueOfDifferentTypeError
            ConstEvaluationVisitor constEvaluationVisitor = new ConstEvaluationVisitor(_symbolTable);
            constEvaluationVisitor.Visit(symbolTableCreationVisitor.ConstDefinitionNodes);
            constEvaluationVisitor.Visit(symbolTableCreationVisitor.ArrayDeclarationNodes);
            constEvaluationVisitor.Visit(referenceResolvingVisitor.ArrayIndexNodes);

            // ArgumentsCountDoesNotMatchError
            TypeCheckingVisitor typeCheckingVisitor = new TypeCheckingVisitor();
            typeCheckingVisitor.VisitTree(AbstractSyntaxTree);
            
            
            DeclarationUsagesChecker declarationUsagesChecker = new DeclarationUsagesChecker();
            declarationUsagesChecker.Check(symbolTableCreationVisitor.DeclarationNodes);
            
            
            // Error o rozmiarze C_NPC (800), warningi o tym, ze nazwy uzywamy np. małymi, a zadeklaorwaliśy duzymi, albo, ze sa nieuzywane funkcje
            // annotates:
            // IterationStatementNotInLoopError
            // IntegerLiteralTooLargeError
            // SingleExpressionWarning
            // WrongClassSizeError
            RemainingAnnotationsAdditionVisitor remainingAnnotationsAdditionVisitor = new RemainingAnnotationsAdditionVisitor();
            remainingAnnotationsAdditionVisitor.VisitTree(AbstractSyntaxTree);
        }
    }
}