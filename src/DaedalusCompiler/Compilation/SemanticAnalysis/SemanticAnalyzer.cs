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
        private AbstractSyntaxTree _abstractSyntaxTree;
        private Dictionary<string, Symbol> _symbolTable;
        
        public int ErrorsCount;
        public int WarningsCount;

        private bool _strictSyntax;

        public SemanticAnalyzer(List<IParseTree> parseTrees, int externalFilesCount, List<string> filesPaths, List<string[]> filesContents, List<HashSet<string>> suppressedWarningCodes, bool strictSyntax)
        {
            _symbolTable = null;
            ErrorsCount = 0;
            WarningsCount = 0;
            _strictSyntax = strictSyntax;
            
            Stopwatch timer = new Stopwatch();
            timer.Start();
            _abstractSyntaxTree = new AbstractSyntaxTree(parseTrees, externalFilesCount, filesPaths, filesContents, suppressedWarningCodes);
            timer.Stop();
            Console.WriteLine($"AbstractSyntaxTree creation time: {timer.Elapsed}");
        }


        public void CreateSymbolTable()
        {
            //Stopwatch timer = new Stopwatch();
            //timer.Start();
            //SymbolTableCreationVisitor symbolTableCreationVisitor = new SymbolTableCreationVisitor();
            //symbolTableCreationVisitor.VisitTree(_abstractSyntaxTree);
            //timer.Stop();
            //Console.WriteLine($"SymbolTable creation time: {timer.Elapsed}");
            //_symbolTable = visitor.SymbolTable;
            
            
            // annotates:
            // RedefinedIdentifierAnnotation
            SymbolTableCreationVisitor symbolTableCreationVisitor = new SymbolTableCreationVisitor();
            symbolTableCreationVisitor.VisitTree(_abstractSyntaxTree);
            _symbolTable = symbolTableCreationVisitor.SymbolTable;
            
            // annotates:
            // UnsupportedTypeAnnotation
            // UndefinedTypeAnnotation
            // UnsupportedArrayTypeAnnotation
            TypeResolver typeResolver = new TypeResolver(_symbolTable);
            typeResolver.Resolve(symbolTableCreationVisitor.TypedSymbols);
            
            // NotClassOrPrototypeReferenceAnnotation
            // UndeclaredIdentifierAnnotation
            // InfiniteReferenceLoopAnnotation
            InheritanceResolver inheritanceResolver = new InheritanceResolver(_symbolTable);
            inheritanceResolver.Resolve(symbolTableCreationVisitor.SubclassSymbols);
            
            // annotates:
            // UndeclaredIdentifierAnnotation
            // AccessToAttributeOfArrayElementNotSupportedAnnotation
            // AttributeOfNonInstanceAnnotation
            // ClassDoesNotHaveAttributeAnnotation
            // ReferencedSymbolIsNotArrayAnnotation
            ReferenceResolvingVisitor referenceResolvingVisitor = new ReferenceResolvingVisitor(_symbolTable);
            referenceResolvingVisitor.Visit(_abstractSyntaxTree.ReferenceNodes);
            
            // annotates:
            // InfiniteReferenceLoopAnnotation
            // InconsistentSizeAnnotation
            // UnsupportedTypeAnnotation
            // ConstIntegerExpectedAnnotation
            // IndexOutOfRangeAnnotation
            // NotConstReferenceAnnotation
            // InvalidUnaryOperationAnnotation
            // InvalidBinaryOperationAnnotation
            // IncompatibleTypesAnnotation
            ConstEvaluationVisitor constEvaluationVisitor = new ConstEvaluationVisitor(_symbolTable);
            constEvaluationVisitor.Visit(symbolTableCreationVisitor.ConstDefinitionNodes);
            constEvaluationVisitor.Visit(symbolTableCreationVisitor.ArrayDeclarationNodes);
            constEvaluationVisitor.Visit(referenceResolvingVisitor.ArrayIndexNodes);

            
            
            // TypeCheckingVisitor
            
            
            //constEvaluationVisitor.VisitTree(_abstractSyntaxTree);
            /*
             *
             * Dodaje pozostałe adnotacje
             */
            // Error o rozmiarze C_NPC (800), warningi o tym, ze nazwy uzywamy np. małymi, a zadeklaorwaliśy duzymi, albo, ze sa nieuzywane funkcje
            Console.WriteLine("---------");
            RemainingAnnotationsAdditionVisitor remainingAnnotationsAdditionVisitor = new RemainingAnnotationsAdditionVisitor();
            remainingAnnotationsAdditionVisitor.VisitTree(_abstractSyntaxTree);
            
            
            ErrorCollectionVisitor errorCollectionVisitor = new ErrorCollectionVisitor(new StdErrorLogger(), _strictSyntax);
            errorCollectionVisitor.VisitTree(_abstractSyntaxTree);
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