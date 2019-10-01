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
            
            // UnusedSymbolWarning
            // NamesNotMatchingCaseWiseWarning
            DeclarationUsagesChecker declarationUsagesChecker = new DeclarationUsagesChecker();
            declarationUsagesChecker.Check(symbolTableCreationVisitor.DeclarationNodes);
            
            // annotates:
            // IterationStatementNotInLoopError
            // IntegerLiteralTooLargeError
            // SingleExpressionWarning
            // WrongClassSizeError
            // ConstValueChangedWarning
            // UsageOfNonInitializedVariableWarning
            RemainingAnnotationsAdditionVisitor remainingAnnotationsAdditionVisitor = new RemainingAnnotationsAdditionVisitor();
            remainingAnnotationsAdditionVisitor.VisitTree(AbstractSyntaxTree);
            
            // TODO
            // add warning when somebody changes const, since it's possible but it's value isntr stored in savefiles (look daedalus compiler planned features docs)
            // add warning if function doesn't return anything but it's type isn't void. Also check if all paths return
            
            // extern keyword
            // inline keyword and inline comment for backwards compability
            // tenary operator
            // for and foreach loops
            // methods in classes
            // boolean
            // make builting symbols list
            // make unused symbols
            // assignment on var declaration (inside functions)
            // let grammar allow const not to have assignment and detect that error in semantic analysis
            // add documentation: why created, differences to original compiler, tutorial, transpiler, all supported errors
            // dynamic array accesss (with custom assembly) if > 255
            // check if there are always true / always false blocks (unreachable code)
            // check if there is code after return
            // make casting possible
            // add float arithmetics
            // make ou files be generated using hidden channel, not regexes (it may be fast)
            // translation strings, t"Hello world", also think about different way of writing dialogs (not in comments)
        }
    }
}