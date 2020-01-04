using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DaedalusCompiler.Compilation.SemanticAnalysis;

namespace DaedalusCompiler.Compilation
{
    
public enum FilePathDisplayStatus
{
    NeverDisplay = 0,
    AlwaysDisplay = 1,
    DisplayOncePerFile = 2,
}  
    
public class SemanticErrorsCollectingVisitor : AbstractSyntaxTreeBaseVisitor
    {
        private readonly ErrorLogger _errorLogger;
        private List<string> _filesPaths;
        private List<string[]> _filesContents;
        private List<string> _fileNames;
        private List<HashSet<string>> _suppressedWarningCodes;

        public int ErrorsCount;
        public int WarningsCount;

        private ASTNode _lastParentBlockNode;
        public FilePathDisplayStatus FilePathDisplayStatus;
        private bool _wasFilePathDisplayed;
        
        private readonly bool _strictSyntax;
        private readonly HashSet<string> _globallySuppressedCodes;
        
        public SemanticErrorsCollectingVisitor(ErrorLogger errorLogger, bool strictSyntax, HashSet<string> globallySuppressedCodes)
        {
            _errorLogger = errorLogger;
            _strictSyntax = strictSyntax;
            _globallySuppressedCodes = globallySuppressedCodes;
            _lastParentBlockNode = null;

            ErrorsCount = 0;
            WarningsCount = 0;

            FilePathDisplayStatus = FilePathDisplayStatus.DisplayOncePerFile;
        }


        public override void VisitTree(AbstractSyntaxTree tree)
        {
            _filesPaths = tree.FilesPaths;
            _filesContents = tree.FilesContents;
            _fileNames = new List<string>();
            foreach (var filePath in tree.FilesPaths)
            {
                _fileNames.Add(Path.GetFileName(filePath));
            }
            _suppressedWarningCodes = tree.SuppressedWarningCodes;
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
            _wasFilePathDisplayed = false;
            base.VisitFile(node);
        }

        private bool IsAnnotationSuppressed(NodeAnnotation annotation, ASTNode node)
        {
            if (annotation is WarningAnnotation && annotation is IWithCode withCode)
            {
                int fileIndex = node.Location.FileIndex;
                string line = _filesContents[fileIndex][node.Location.Line - 1];
                HashSet<string> suppressedLineWarningCodes = Compiler.GetWarningCodesToSuppress(line);
                HashSet<string> suppressedFileWarningCodes = _suppressedWarningCodes[fileIndex];
                
                HashSet<string> suppressedWarningCodes = suppressedLineWarningCodes.Union(suppressedFileWarningCodes).ToHashSet();
                if (_globallySuppressedCodes != null)
                {
                    suppressedWarningCodes.UnionWith(_globallySuppressedCodes);
                }
                string code = withCode.Code;
                return suppressedWarningCodes.Contains(code);
            }

            return false;
        }

        private void PrintAnnotation(NodeAnnotation annotation, ASTNode node)
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
                annotationType = "warning";
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
                _errorLogger.LogLine(_filesPaths[pointerLocation.FileIndex]);
            }


            string parentBlockName = String.Empty;
            ASTNode ancestor = node.GetFirstSignificantAncestorNode();
            if (ancestor != _lastParentBlockNode)
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
                        if (_lastParentBlockNode != null)
                        {
                            parentBlockName = "global scope";
                        }
                        break;
                    default:
                        throw new Exception();
                }
                
                _lastParentBlockNode = ancestor;
            }

            if (parentBlockName != String.Empty)
            {
                _errorLogger.LogLine($"{_fileNames[pointerLocation.FileIndex]}: In {parentBlockName}:");
            }
            
            
            _errorLogger.LogLine($"{_fileNames[pointerLocation.FileIndex]}:{pointerLocation.Line}:{pointerLocation.Column}: {annotationType}: {message}");
            _errorLogger.LogLine(_filesContents[pointerLocation.FileIndex][pointerLocation.Line - 1].Replace("\t", "    "));
            _errorLogger.LogLine(GetErrorPointerLine(pointerLocation, underlineLocations));

            if (annotation is INotedAnnotation annotationNoted)
            {
                string note = annotationNoted.GetNote();
                NodeLocation noteLocation = annotationNoted.NoteLocation;
                
                _errorLogger.LogLine($"{_fileNames[noteLocation.FileIndex]}:{noteLocation.Line}:{noteLocation.Column}: note: {note}");
                _errorLogger.LogLine(_filesContents[noteLocation.FileIndex][noteLocation.Line - 1].Replace("\t", "    "));
                _errorLogger.LogLine(GetErrorPointerLine(noteLocation, new List<NodeLocation>()));
            }
        }
        
        private string GetErrorPointerLine(NodeLocation pointerLocation, List<NodeLocation> locations)
        {
            List<UnderlineExactLineLocation> underlineExactLineLocations = CovertToUnderlineLineLocations(pointerLocation, locations);

            int endColumn = pointerLocation.Column + 1;
            if (underlineExactLineLocations.Any())
            {
                endColumn = Math.Max(endColumn, underlineExactLineLocations.Max(x => x.EndColumn));
            }

            string lineContent = _filesContents[pointerLocation.FileIndex][pointerLocation.Line - 1];

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
                if (_isInCurrentLine && _startColumn <= column && column < EndColumn)
                {
                    return true;
                }
                return false;
            }
        }

        private List<UnderlineExactLineLocation> CovertToUnderlineLineLocations(NodeLocation pointerLocation, List<NodeLocation> locations)
        {
            string lineContent = _filesContents[pointerLocation.FileIndex][pointerLocation.Line - 1];

            int line = pointerLocation.Line;
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