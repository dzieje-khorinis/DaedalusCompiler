using System;
using System.Collections.Generic;
using System.Diagnostics;
using Antlr4.Runtime.Tree;
using Common.SemanticAnalysis;
using Common.Zen;

namespace Common
{
    public class SemanticAnalyzer
    {
        public readonly AbstractSyntaxTree AbstractSyntaxTree;
        public Dictionary<string, Symbol> SymbolTable;
        public List<BlockSymbol> SymbolsWithInstructions;
        
        private readonly List<ZenFileNode> _zenFileNodes;

        public SemanticAnalyzer(
            List<ZenFileNode> zenFileNodes,
            List<IParseTree> parseTrees,
            List<string> filesPaths,
            List<string[]> filesContents,
            List<HashSet<string>> suppressedWarningCodes
        )
        {
            DaedalusParseTreeVisitor visitor = new DaedalusParseTreeVisitor();
            
            _zenFileNodes = zenFileNodes;
            SymbolTable = null;
            SymbolsWithInstructions = null;

            Stopwatch timer = new Stopwatch();
            timer.Start();
            AbstractSyntaxTree = new AbstractSyntaxTree(filesPaths, filesContents, suppressedWarningCodes);
            
            int index = 0;
            foreach (IParseTree parseTree in parseTrees)
            {
                ((ICommonDaedalusParseTreeVisitor)visitor).Reset(index);
                AbstractSyntaxTree.Extend(parseTree, visitor, index);
                index++;
            }
            
            timer.Stop();
            // Console.WriteLine($"AbstractSyntaxTree creation time: {timer.Elapsed}");
        }


        public void Run()
        {
            // VarAssignmentNotAllowedHereError
            RemainingSyntaxErrorsDetectionVisitor remainingSyntaxErrorsDetectionVisitor = new RemainingSyntaxErrorsDetectionVisitor();
            remainingSyntaxErrorsDetectionVisitor.VisitTree(AbstractSyntaxTree);
            
            // RedefinedIdentifierError
            // KeywordUsedAsNameError
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
            // InconsistentArraySizeError
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
            DeclarationUsagesChecker declarationUsagesChecker = new DeclarationUsagesChecker(SymbolTable, _zenFileNodes);
            declarationUsagesChecker.Check(symbolTableCreationVisitor.DeclarationNodes);
            
            // UsageOfNonInitializedVariableWarning
            UninitializedSymbolUsageDetectionVisitor uninitializedSymbolUsageDetectionVisitor = new UninitializedSymbolUsageDetectionVisitor();
            uninitializedSymbolUsageDetectionVisitor.VisitTree(AbstractSyntaxTree);
            
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