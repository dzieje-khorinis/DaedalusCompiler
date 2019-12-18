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
        public Dictionary<string, Symbol> SymbolTable;
        
        public SemanticAnalyzer(List<IParseTree> parseTrees, int externalFilesCount, List<string> filesPaths, List<string[]> filesContents, List<HashSet<string>> suppressedWarningCodes)
        {
            SymbolTable = null;

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
            SymbolTable = symbolTableCreationVisitor.SymbolTable;
            
            // UnknownTypeNameError
            // UnsupportedTypeError
            // UnsupportedArrayTypeError
            // UnsupportedFunctionTypeError
            TypeResolver typeResolver = new TypeResolver(SymbolTable);
            typeResolver.Resolve(symbolTableCreationVisitor.TypedSymbols);
            //typeResolver.Resolve(symbolTableCreationVisitor.FunctionSymbols);
            
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
            ConstEvaluationVisitor constEvaluationVisitor = new ConstEvaluationVisitor(SymbolTable);
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
            RemainingAnnotationsAdditionVisitor remainingAnnotationsAdditionVisitor = new RemainingAnnotationsAdditionVisitor();
            remainingAnnotationsAdditionVisitor.VisitTree(AbstractSyntaxTree);
            
            // TODO
            // keyword self jednak był magiczny w instancjach, dodać keyword this , ktory bedzie znaczyl to samo co self
            // slf wasn't special, it you could always write dupa.dlsdsd
            // backward incompability: keywordy tylko malymi literkami, a wczesniej nie bylo to wazne
            
            // moze wypierdolić nofunc i zostawic tylko null, ktore bedzie mozna wpisywac tylko do zmiennych typy klasowego i funkcyjnego?
            // write to srderr instead of stdout
            // add warning if if-statement condition is always true or always false (it may be HARD)
            // add warning accessing array without square brackets
            // addd warning code is after return
            // add warning if function doesn't return anything but it's type isn't void. Also check if all paths return
            
            // doc: wyjebane magiczne słowo self z instancji bo sie myliło z globalnym obiektem self, a zamiast tego bedzie uzywane slowo this
            // test ingame (zSpy print) return float / float assignment / float argument (func call, literal, variable), 
            // doc: we have hoisting, when original compiler only have hoisting when there is global variable with same name
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