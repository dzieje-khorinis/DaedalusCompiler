using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using Antlr4.Runtime.Tree;
using DaedalusCompiler.Compilation.SemanticAnalysis;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    public class SymbolContext
    {
        public DatSymbol Symbol;
        public ASTNode Node; //DeclarationNode or StringLiteralNode
    }

    public class SemanticAnalyzer
    {
        private AbstractSyntaxTree _abstractSyntaxTree;
        private Dictionary<string, SymbolContext> _symbolTable;

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
            /*
            long a = -2147483648;
            long c = ~2147483648;
            long d = 2147483690;
            long x = 21474836900;

            int a2 = (int) a;
            int c2 = (int) c;
            int d2 = (int) d;
            int x2 = (int) x;
            
            Console.WriteLine(a2);
            Console.WriteLine(c2);
            Console.WriteLine(d2);
            Console.WriteLine(x2);
            return;
            */
            
            //Stopwatch timer = new Stopwatch();
            //timer.Start();
            //SymbolTableCreationVisitor symbolTableCreationVisitor = new SymbolTableCreationVisitor();
            //symbolTableCreationVisitor.VisitTree(_abstractSyntaxTree);
            //timer.Stop();
            //Console.WriteLine($"SymbolTable creation time: {timer.Elapsed}");
            //_symbolTable = visitor.SymbolTable;
            
            
            // annotates:
            // RedefinedIdentifierAnnotation
            // UnsupportedArrayTypeAnnotation
            SymbolTableCreationVisitor symbolTableCreationVisitor = new SymbolTableCreationVisitor();
            symbolTableCreationVisitor.VisitTree(_abstractSyntaxTree);
            _symbolTable = symbolTableCreationVisitor.SymbolTable;
            
            // annotates:
            // InfiniteReferenceLoopAnnotation (Class/Prototype)
            // NotClassOrPrototypeReferenceAnnotation
            // UndeclaredIdentifierAnnotation
            // AccessToAttributeOfArrayElementNotSupportedAnnotation
            ReferenceResolver referenceResolver = new ReferenceResolver(_symbolTable);
            referenceResolver.Resolve(symbolTableCreationVisitor.ParentReferenceNodes);
            referenceResolver.Resolve(symbolTableCreationVisitor.ReferenceNodes);

            
            /*
            ConstEvaluationVisitor constEvaluationVisitor = new ConstEvaluationVisitor(_symbolTable);
            constEvaluationVisitor.Visit(symbolTableCreationVisitor.ConstDefinitionNodes);
            constEvaluationVisitor.Visit(symbolTableCreationVisitor.ArrayDeclarationNodes);
            */
            /*
             *
             *myślę, że to powinno byc tak:
             * 1. resolving zwykłych referencji
             * 2. obliczenie constów
             * 3. resolving array referencji? bo jak bedzie np. x[5].y to trzeba sprawdzić co znajduje sie w x[5], a może nie trzeba? moze wystarczy typ?
             */
            
            
            
            //constEvaluationVisitor.VisitTree(_abstractSyntaxTree);
            Console.WriteLine("---------");
            AnnotationsAdditionVisitor annotationsAdditionVisitor = new AnnotationsAdditionVisitor();
            annotationsAdditionVisitor.VisitTree(_abstractSyntaxTree);
            
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
             */
        }
        
    }
}