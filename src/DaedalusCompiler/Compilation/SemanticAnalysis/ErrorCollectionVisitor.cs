using System;
using System.Collections.Generic;

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

        public ErrorLogger ErrorLogger;
        public List<string> FilesPaths;
        
        public ErrorCollectionVisitor(ErrorLogger errorLogger, List<string> filesPaths)
        {
            CurrentFileNode = null;
            ErrorLogger = errorLogger;
            FilesPaths = filesPaths;
        }

        protected override void Visit(ASTNode node)
        {
            foreach (var annotation in node.Annotations)
            {
                PrintAnnotation(annotation, node);
            }
            base.Visit(node);
        }

        protected override void VisitFile(FileNode node)
        {
            CurrentFileNode = node;
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
        
        
        
        
        public void PrintAnnotation(NodeAnnotation annotation, ASTNode node)
        {
            
            string message = annotation.GetMessage();
            NodeLocation messageLocation = node.Location;
            

            ErrorLogger.LogLine($"{FilesPaths[messageLocation.FileIndex]}:{messageLocation.Line}:{messageLocation.Column}: {message}");
            ErrorLogger.LogLine(GetErrorPointerLine(messageLocation));
        
            
            
            if (annotation is NodeAnnotationNoted annotationNoted)
            {
                string note = annotationNoted.GetNote();
                NodeLocation noteLocation = annotationNoted.GetNoteLocation();
                
                ErrorLogger.LogLine(note);

                if (noteLocation != null)
                {
                    ErrorLogger.LogLine(GetErrorPointerLine(noteLocation));
                }
                
            }
        }
        
        
        private string GetErrorPointerLine(NodeLocation nodeLocation)
        {
            string line = CurrentFileNode.Content[nodeLocation.Line];

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
    }
}