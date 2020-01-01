using System;
using System.Collections.Generic;
using System.Diagnostics;
using Antlr4.Runtime.Tree;
using DaedalusCompiler.Compilation.SemanticAnalysis;

namespace DaedalusCompiler.Compilation
{
    public class SemanticAnalyzer
    {
        public readonly AbstractSyntaxTree AbstractSyntaxTree;
        public Dictionary<string, Symbol> SymbolTable;
        public List<BlockSymbol> SymbolsWithInstructions;
        
        public SemanticAnalyzer(List<IParseTree> parseTrees, int externalFilesCount, List<string> filesPaths, List<string[]> filesContents, List<HashSet<string>> suppressedWarningCodes)
        {
            SymbolTable = null;
            SymbolsWithInstructions = null;

            Stopwatch timer = new Stopwatch();
            timer.Start();
            AbstractSyntaxTree = new AbstractSyntaxTree(parseTrees, externalFilesCount, filesPaths, filesContents, suppressedWarningCodes);
            timer.Stop();
            Console.WriteLine($"AbstractSyntaxTree creation time: {timer.Elapsed}");
        }


        public void Run()
        {
            SymbolTableCreationVisitor symbolTableCreationVisitor = new SymbolTableCreationVisitor();
            symbolTableCreationVisitor.VisitTree(AbstractSyntaxTree);
            SymbolTable = symbolTableCreationVisitor.SymbolTable;
            SymbolsWithInstructions = symbolTableCreationVisitor.SymbolsWithInstructions;
            
            // UnknownTypeNameError
            // UnsupportedTypeError
            // UnsupportedArrayTypeError
            // UnsupportedFunctionTypeError
            TypeResolver typeResolver = new TypeResolver(SymbolTable);
            typeResolver.Resolve(symbolTableCreationVisitor.TypedSymbols);

            // NotClassOrPrototypeReferenceError
            // UndeclaredIdentifierError
            // InfiniteReferenceLoopError
            InheritanceResolver inheritanceResolver = new InheritanceResolver(SymbolTable);
            inheritanceResolver.Resolve(symbolTableCreationVisitor.SubclassSymbols);
            
            // UndeclaredIdentifierError
            // AccessToAttributeOfArrayElementNotSupportedError
            // AttributeOfNonInstanceError
            // ClassDoesNotHaveAttributeError
            // ReferencedSymbolIsNotArrayError
            ReferenceResolvingVisitor referenceResolvingVisitor = new ReferenceResolvingVisitor(SymbolTable);
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
            ConstEvaluationVisitor constEvaluationVisitor = new ConstEvaluationVisitor();
            constEvaluationVisitor.Visit(symbolTableCreationVisitor.ConstDefinitionNodes);
            constEvaluationVisitor.Visit(symbolTableCreationVisitor.ArrayDeclarationNodes);
            constEvaluationVisitor.Visit(referenceResolvingVisitor.ArrayIndexNodes);

            // ArgumentsCountDoesNotMatchError
            TypeCheckingVisitor typeCheckingVisitor = new TypeCheckingVisitor(SymbolTable);
            typeCheckingVisitor.VisitTree(AbstractSyntaxTree);
            
            // UnusedSymbolWarning
            // NamesNotMatchingCaseWiseWarning
            DeclarationUsagesChecker declarationUsagesChecker = new DeclarationUsagesChecker();
            declarationUsagesChecker.Check(symbolTableCreationVisitor.DeclarationNodes);
            
            UninitializedSymbolUsageDetectionVisitor uninitializedSymbolUsageDetectionVisitor = new UninitializedSymbolUsageDetectionVisitor();
            uninitializedSymbolUsageDetectionVisitor.VisitTree(AbstractSyntaxTree);
            
            // annotates:
            // IterationStatementNotInLoopError
            // IntegerLiteralTooLargeError
            // SingleExpressionWarning
            // WrongClassSizeError
            // ConstValueChangedWarning
            // UsageOfNonInitializedVariableWarning
            RemainingAnnotationsAdditionVisitor remainingAnnotationsAdditionVisitor = new RemainingAnnotationsAdditionVisitor(SymbolTable);
            remainingAnnotationsAdditionVisitor.VisitTree(AbstractSyntaxTree);
        }
    }
}