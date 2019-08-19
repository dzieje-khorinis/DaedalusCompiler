using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    
public enum FilePathDisplayStatus
{
    NeverDisplay = 0,
    AlwaysDisplay = 1,
    DisplayOncePerFile = 2,
}  
    
public class SemanticErrorsCollectingVisitor : AbstractSyntaxTreeBaseVisitor
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
        
        public ASTNode LastParentBlockNode;
        public FilePathDisplayStatus FilePathDisplayStatus;
        private bool _wasFilePathDisplayed;
        
        private readonly bool _strictSyntax;
        
        public SemanticErrorsCollectingVisitor(ErrorLogger errorLogger, bool strictSyntax)
        {
            CurrentFileNode = null;
            ErrorLogger = errorLogger;
            _strictSyntax = strictSyntax;
            LastParentBlockNode = null;

            ErrorsCount = 0;
            WarningsCount = 0;

            FilePathDisplayStatus = FilePathDisplayStatus.DisplayOncePerFile;
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
                _wasFilePathDisplayed = true;
            }
            base.Visit(node);
        }

        protected override void VisitFile(FileNode node)
        {
            CurrentFileNode = node;
            _wasFilePathDisplayed = false;
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
            if (annotation is WarningAnnotation _ && annotation is IWithCode withCode)
            {
                int fileIndex = node.Location.FileIndex;
                string line = FilesContents[fileIndex][node.Location.Line - 1];
                HashSet<string> suppressedLineWarningCodes = Compiler.GetWarningCodesToSuppress(line);
                HashSet<string> suppressedFileWarningCodes = SuppressedWarningCodes[fileIndex];
                HashSet<string> suppressedWarningCodes = suppressedLineWarningCodes.Union(suppressedFileWarningCodes).ToHashSet();
                string code = withCode.Code;//annotation.GetType().GetField("Code").GetValue(null).ToString();
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
            else if (annotation is WarningAnnotation)
            {
                WarningsCount++;
                annotationType = $"warning";
            }
            else
            {
                throw new Exception();
            }

            if (annotation is IWithCode withCode)
            {
                annotationType = $"{annotationType} {withCode.Code}";
            }
            
            string message = annotation.GetMessage();
            if (message == String.Empty) //TODO remove it in the final version
            {
                message = annotation.GetType().ToString();
            }
            
            NodeLocation pointerLocation = annotation.PointerLocation ?? node.Location;
            List<NodeLocation> underlineLocations = annotation.UnderlineLocations ?? new List<NodeLocation>();

            if ((!_wasFilePathDisplayed && FilePathDisplayStatus == FilePathDisplayStatus.DisplayOncePerFile) || FilePathDisplayStatus == FilePathDisplayStatus.AlwaysDisplay)
            {
                ErrorLogger.LogLine(FilesPaths[pointerLocation.FileIndex]);
            }


            string parentBlockName = String.Empty;
            ASTNode ancestor = node.GetFirstSignificantAncestor();
            if (ancestor != LastParentBlockNode)
            {
                switch (ancestor)
                {
                    case PrototypeDefinitionNode prototypeDefinitionNode:
                        parentBlockName = $"prototype '{prototypeDefinitionNode.NameNode.Value}'";
                        break;
                    case InstanceDefinitionNode instanceDefinitionNode:
                        parentBlockName = $"instance '{instanceDefinitionNode.NameNode.Value}'";
                        break;
                    case FunctionDefinitionNode functionDefinitionNode:
                        parentBlockName = $"function '{functionDefinitionNode.NameNode.Value}'";
                        break;
                    case ClassDefinitionNode classDefinitionNode:
                        parentBlockName = $"class '{classDefinitionNode.NameNode.Value}'";
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
                ErrorLogger.LogLine($"{FileNames[pointerLocation.FileIndex]}: In {parentBlockName}:");
            }
            
            
            ErrorLogger.LogLine($"{FileNames[pointerLocation.FileIndex]}:{pointerLocation.Line}:{pointerLocation.Column}: {annotationType}: {message}");
            ErrorLogger.LogLine(FilesContents[pointerLocation.FileIndex][pointerLocation.Line - 1].Replace("\t", "    "));
            ErrorLogger.LogLine(GetErrorPointerLine(pointerLocation, underlineLocations));

            if (annotation is INotedAnnotation annotationNoted)
            {
                string note = annotationNoted.GetNote();
                NodeLocation noteLocation = annotationNoted.NoteLocation;
                
                ErrorLogger.LogLine($"{FileNames[noteLocation.FileIndex]}:{noteLocation.Line}:{noteLocation.Column}: note: {note}");
                ErrorLogger.LogLine(FilesContents[noteLocation.FileIndex][noteLocation.Line - 1].Replace("\t", "    "));
                ErrorLogger.LogLine(GetErrorPointerLine(noteLocation, new List<NodeLocation>()));
            }
        }
        
        private string GetErrorPointerLine(NodeLocation pointerLocation, List<NodeLocation> locations)
        {
            List<UnderlineExactLineLocation> underlineExactLineLocations = CovertToUnderlineLineLocations(pointerLocation.Line, locations);

            int endColumn = pointerLocation.Column + 1;
            if (underlineExactLineLocations.Any())
            {
                endColumn = Math.Max(endColumn, underlineExactLineLocations.Max(x => x.EndColumn));
            }

            string lineContent = CurrentFileNode.Content[pointerLocation.Line - 1];

            string[] buffer = new string[endColumn];

            for (int i = 0; i < endColumn; i++)
            {
                if (lineContent[i] == '\t')
                {
                    if (ShouldBeUnderlined(i, underlineExactLineLocations))
                    {
                        buffer[i] = "~~~~";
                    }
                    else
                    {
                        buffer[i] = "    ";
                    }
                }
                else
                {
                    if (ShouldBeUnderlined(i, underlineExactLineLocations))
                    {
                        buffer[i] = "~";
                    }
                    else
                    {
                        buffer[i] = " ";
                    }
                }
            }
            buffer[pointerLocation.Column] = "^";
            
            return String.Join("", buffer);
        }

        private bool ShouldBeUnderlined(int column,  List<UnderlineExactLineLocation> underlineExactLineLocations)
        {
            foreach (var lineExactLocation in underlineExactLineLocations)
            {
                if (lineExactLocation.DoesCoverColumn(column))
                {
                    return true;
                }
            }

            return false;
        }
        
        private class UnderlineExactLineLocation
        {
            private readonly bool _isInCurrentLine;
            private readonly int _startColumn;
            public readonly int EndColumn;

            public UnderlineExactLineLocation(bool isInCurrentLine, int startColumn, int endColumn)
            {
                _isInCurrentLine = isInCurrentLine;
                _startColumn = startColumn;
                EndColumn = endColumn;
            }

            public bool DoesCoverColumn(int column)
            {
                if (_isInCurrentLine && _startColumn <= column && column <= EndColumn)
                {
                    return true;
                }
                return false;
            }
        }

        private List<UnderlineExactLineLocation> CovertToUnderlineLineLocations(int line, List<NodeLocation> locations)
        {
            string lineContent = CurrentFileNode.Content[line - 1];
            
            int firstSignificantColumn = 0;
            for (int i = 0; i < lineContent.Length; ++i)
            {
                if (!Char.IsWhiteSpace(lineContent[i]))
                {
                    firstSignificantColumn = i;
                    break;
                }
            }

            int lastSignificantColumn = 0;
            for (int i = lineContent.Length - 1; i >= 0; --i)
            {
                if (!Char.IsWhiteSpace(lineContent[i]))
                {
                    lastSignificantColumn = i;
                    break;
                }
            }


            List<UnderlineExactLineLocation> underlineLineLocations = new List<UnderlineExactLineLocation>();
            
            foreach (var location in locations)
            {
                bool isInCurrentLine = true;
                int startColumn = 0;
                int endColumn = 0;

                if (location.Line == line)
                {
                    if (location.LinesCount == 1)
                    {
                        startColumn = location.Column;
                        endColumn = location.Column + location.CharsCount;
                    }
                    else
                    {
                        startColumn = location.Column;
                        endColumn = lastSignificantColumn;
                    }
                }
                else if (location.Line < line && location.LinesCount > 1)
                {
                    if (location.Line + location.LinesCount == line)
                    {
                        startColumn = firstSignificantColumn;
                        endColumn = location.EndColumn;
                    }
                    else if (location.Line + location.LinesCount > line)
                    {
                        startColumn = firstSignificantColumn;
                        endColumn = lastSignificantColumn;
                    }
                }
                else
                {
                    isInCurrentLine = false;
                }
                
                
                underlineLineLocations.Add(new UnderlineExactLineLocation(isInCurrentLine, startColumn, endColumn));
            }

            return underlineLineLocations;
        }
        
    }
}