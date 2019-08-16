using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
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
            
            
            // annotates:
            // RedefinedIdentifierError
            // KeywordUsedAsNameError
            SymbolTableCreationVisitor symbolTableCreationVisitor = new SymbolTableCreationVisitor();
            symbolTableCreationVisitor.VisitTree(AbstractSyntaxTree);
            _symbolTable = symbolTableCreationVisitor.SymbolTable;
            
            // annotates:
            // UnknownTypeNameError
            // UndefinedTypeError
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
            
            // annotates:
            // UndeclaredIdentifierError
            // AccessToAttributeOfArrayElementNotSupportedError
            // AttributeOfNonInstanceError
            // ClassDoesNotHaveAttributeError
            // ReferencedSymbolIsNotArrayError
            ReferenceResolvingVisitor referenceResolvingVisitor = new ReferenceResolvingVisitor(_symbolTable);
            referenceResolvingVisitor.Visit(AbstractSyntaxTree.ReferenceNodes);
            
            // annotates:
            // InfiniteReferenceLoopError
            // ArraySizeEqualsZeroError
            // TooBigArraySizeError
            // UnsupportedTypeError
            // InconsistentSizeError
            // IndexOutOfRangeError
            // TooBigArrayIndex
            // ConstIntegerExpectedError
            // NotConstReferenceError
            // ArithmeticOperationOverflowError
            // InvalidUnaryOperationError
            // InvalidBinaryOperationError
            // IntegerLiteralTooLargeError
            // CannotInitializeConstWithValueOfDifferentType
            // CannotInitializeArrayElementWithValueOfDifferentType
            ConstEvaluationVisitor constEvaluationVisitor = new ConstEvaluationVisitor(_symbolTable);
            constEvaluationVisitor.Visit(symbolTableCreationVisitor.ConstDefinitionNodes);
            constEvaluationVisitor.Visit(symbolTableCreationVisitor.ArrayDeclarationNodes);
            constEvaluationVisitor.Visit(referenceResolvingVisitor.ArrayIndexNodes);
            
            // annotates:
            // ArgumentsCountDoesNotMatchError
            TypeCheckingVisitor typeCheckingVisitor = new TypeCheckingVisitor();
            typeCheckingVisitor.VisitTree(AbstractSyntaxTree);
            
            
            
            // Error o rozmiarze C_NPC (800), warningi o tym, ze nazwy uzywamy np. małymi, a zadeklaorwaliśy duzymi, albo, ze sa nieuzywane funkcje
            // annotates:
            // IterationStatementNotInLoopError
            // IntegerLiteralTooLargeError
            // SingleExpressionWarning
            RemainingAnnotationsAdditionVisitor remainingAnnotationsAdditionVisitor = new RemainingAnnotationsAdditionVisitor();
            remainingAnnotationsAdditionVisitor.VisitTree(AbstractSyntaxTree);
            
            
            
            
        }
        
        public void EvaluateReferencesAndTypesAndArraySize()
        {
           
            
            
            /*
             Steps:
             1. Add types?
             2. Resolve references?
             
             3. Evaluate constants content
             4. Evaluate array Sizes
             5. Evaluate const arrays elements
             
             6. For every variable and parameter that is not classVar and has complex type, set ParentInde
             
             
             
             Calculate DatSymbol properties:
             
             Array: ArrayLength - must be const
             Const: Content
             * For every local and global const and var (not class attributes) if it has Class as Type,
             set ParentIndex to that Class. 
             
             Evaluate references.
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
                * function / class /prototype isn't used
             
             */
        }
        
    }
}