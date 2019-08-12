using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    
    /*
    public abstract class CompilationMessage
    {
        public string Content;
    }

    public class CompilationWarning : CompilationMessage
    {
    }

    public class CompilationError : CompilationMessage
    {
    }
    */

    
public class ErrorCollectionVisitor : AbstractSyntaxTreeBaseVisitor
    {
        public FileNode CurrentFileNode;
        public ASTNode CurrentBlockNode; //function, instance, prototype, class

        public readonly ErrorLogger ErrorLogger;
        public List<string> FilesPaths;
        public List<string[]> FilesContents;
        public List<string> FileNames;
        public List<HashSet<string>> SuppressedWarningCodes;

        public int ErrorsCount;
        public int WarningsCount;

        public int FileAnnotationsCount;
        public ASTNode LastParentBlockNode;

        private readonly bool _strictSyntax;
        
        public ErrorCollectionVisitor(ErrorLogger errorLogger, bool strictSyntax)
        {
            CurrentFileNode = null;
            ErrorLogger = errorLogger;
            _strictSyntax = strictSyntax;
            LastParentBlockNode = null;

            ErrorsCount = 0;
            WarningsCount = 0;
        }


        public override void VisitTree(AbstractSyntaxTree tree)
        {
            FilesPaths = tree.FilesPaths;
            FilesContents = tree.FilesContents;
            FileNames = new List<string>();
            foreach (var filePath in tree.FilesPaths)
            {
                FileNames.Add(Path.GetFileName(filePath));
            }
            SuppressedWarningCodes = tree.SuppressedWarningCodes;
            base.VisitTree(tree);
        }

        protected override void Visit(ASTNode node)
        {
            foreach (var annotation in node.Annotations)
            {
                if (IsAnnotationSuppressed(annotation, node))
                {
                    continue;
                }
                PrintAnnotation(annotation, node);
                FileAnnotationsCount++;
            }
            base.Visit(node);
        }

        protected override void VisitFile(FileNode node)
        {
            CurrentFileNode = node;
            FileAnnotationsCount = 0;
            base.VisitFile(node);
        }

        protected override void VisitFunctionDefinition(FunctionDefinitionNode node)
        {
            CurrentBlockNode = node;
            base.VisitFunctionDefinition(node);
        }

        protected override void VisitClassDefinition(ClassDefinitionNode node)
        {
            CurrentBlockNode = node;
            base.VisitClassDefinition(node);
        }

        protected override void VisitPrototypeDefinition(PrototypeDefinitionNode node)
        {
            CurrentBlockNode = node;
            base.VisitPrototypeDefinition(node);
        }

        protected override void VisitInstanceDefinition(InstanceDefinitionNode node)
        {
            CurrentBlockNode = node;
            base.VisitInstanceDefinition(node);
        }


        public bool IsAnnotationSuppressed(NodeAnnotation annotation, ASTNode node)
        {
            if (annotation is WarningAnnotation warningAnnotation)
            {
                int fileIndex = node.Location.FileIndex;
                string line = FilesContents[fileIndex][node.Location.Line - 1];
                HashSet<string> suppressedLineWarningCodes = Compiler.GetWarningCodesToSuppress(line);
                HashSet<string> suppressedFileWarningCodes = SuppressedWarningCodes[fileIndex];
                HashSet<string> suppressedWarningCodes = suppressedLineWarningCodes.Union(suppressedFileWarningCodes).ToHashSet();
                string code = annotation.GetType().GetField("Code").GetValue(null).ToString();
                return suppressedWarningCodes.Contains(code);
            }

            return false;
        }
        
        public void PrintAnnotation(NodeAnnotation annotation, ASTNode node)
        {
            string annotationType;
            if (annotation is ErrorAnnotation || _strictSyntax)
            {
                ErrorsCount++;
                annotationType = "error";
            }
            else
            {
                WarningsCount++;
                annotationType = "warning";
            }
            
            string message = annotation.GetMessage();
            if (message == String.Empty) //TODO remove it in the final version
            {
                message = annotation.GetType().ToString();
            }
            
            NodeLocation messageLocation = node.Location;

            if (FileAnnotationsCount == 0)
            {
                ErrorLogger.LogLine(FilesPaths[messageLocation.FileIndex]);
            }


            string parentBlockName = String.Empty;
            ASTNode ancestor = node.GetFirstSignificantAncestor();
            if (ancestor != LastParentBlockNode)
            {
                switch (ancestor)
                {
                    case PrototypeDefinitionNode prototypeDefinitionNode:
                        parentBlockName = $"prototype ‘{prototypeDefinitionNode.NameNode.Value}‘";
                        break;
                    case InstanceDefinitionNode instanceDefinitionNode:
                        parentBlockName = $"instance ‘{instanceDefinitionNode.NameNode.Value}‘";
                        break;
                    case FunctionDefinitionNode functionDefinitionNode:
                        parentBlockName = $"function ‘{functionDefinitionNode.NameNode.Value}‘";
                        break;
                    case FileNode _:
                        if (LastParentBlockNode != null)
                        {
                            parentBlockName = "global scope";
                        }
                        break;
                    default:
                        throw new Exception();
                }
                
                LastParentBlockNode = ancestor;
            }

            if (parentBlockName != String.Empty)
            {
                ErrorLogger.LogLine($"{FileNames[messageLocation.FileIndex]}: In {parentBlockName}:");
            }
            
            
            ErrorLogger.LogLine($"{FileNames[messageLocation.FileIndex]}:{messageLocation.Line}:{messageLocation.Column}: {annotationType}: {message}");
            ErrorLogger.LogLine(FilesContents[messageLocation.FileIndex][messageLocation.Line - 1]);
            ErrorLogger.LogLine(GetErrorPointerLine(messageLocation));

            if (annotation is INotedAnnotation annotationNoted)
            {
                string note = annotationNoted.GetNote();
                NodeLocation noteLocation = annotationNoted.NoteLocation;
                
                ErrorLogger.LogLine($"{FileNames[noteLocation.FileIndex]}:{noteLocation.Line}:{noteLocation.Column}: note: {note}");
                ErrorLogger.LogLine(FilesContents[noteLocation.FileIndex][noteLocation.Line - 1]);
                ErrorLogger.LogLine(GetErrorPointerLine(noteLocation));
            }
        }
        
        
        private string GetErrorPointerLine(NodeLocation nodeLocation)
        {
            string line = CurrentFileNode.Content[nodeLocation.Line - 1];

            string errorPointerLine = "";
            for (int i = 0; i < nodeLocation.Column; i++)
            {
                if (line[i] == '\t')
                {
                    errorPointerLine += "\t";
                }
                else
                {
                    errorPointerLine += " ";
                }
            }
            errorPointerLine += "^";
            return errorPointerLine;
        }
        
        
        /*
         *
         *
         if (displayExecBlock)
            {
                RuleContext context = parserContext.Parent;
                while (ExecBlockName == null)
                {
                    switch (context)
                    {
                        case DaedalusParser.FunctionDefContext functionDefContext:
                            ExecBlockName = functionDefContext.nameNode().GetText();
                            ExecBlockType = "function";
                            break;
                        case DaedalusParser.InstanceDefContext instanceDefContext:
                            ExecBlockName = instanceDefContext.nameNode().GetText();
                            ExecBlockType = "instance";
                            break;
                        case DaedalusParser.PrototypeDefContext prototypeDefContext:
                            ExecBlockName = prototypeDefContext.nameNode().GetText();
                            ExecBlockType = "prototype";
                            break;
                        case DaedalusParser.ClassDefContext classDefContext:
                            ExecBlockName = classDefContext.nameNode().GetText();
                            ExecBlockType = "class";
                            break;
                    }

                    context = context.Parent;
                    if (context is DaedalusParser.DaedalusFileContext)
                    {
                        break;
                    }
                }
            }
         */
    }
}