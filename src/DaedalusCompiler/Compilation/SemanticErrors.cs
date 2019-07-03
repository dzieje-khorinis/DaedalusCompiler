using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Antlr4.Runtime;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    public abstract class ErrorLogger
    {
        public abstract void Log(string message);
    }

    public class StdErrorLogger : ErrorLogger
    {
        public override void Log(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class StringBufforErrorLogger : ErrorLogger
    {
        private string buffor;

        public override void Log(string message)
        {
            if (buffor == null)
            {
                buffor = message;
            }
            else
            {
                buffor = $"{buffor}{Environment.NewLine}{message}";
            }
        }

        public string GetBuffor()
        {
            return buffor;
        }
    }

    public class UndeclaredIdentifierException : Exception
    {
        
    }
    public class AttributeNotFoundException : Exception
    {
        
    }

    public class ErrorContext
    {
        public ParserRuleContext Context;
        public string FilePath;
        public string[] FileContentLines;
        public int FileIndex;
        public readonly AssemblyBuilder AssemblyBuilder;
        
        public static readonly DatSymbol UndeclaredSymbol = new DatSymbol();

        public ErrorContext(AssemblyBuilder assemblyBuilder)
        {
            AssemblyBuilder = assemblyBuilder;
        }

        public ErrorContext(ErrorContext errorContext)
        {
            Context = errorContext.Context;
            FilePath = errorContext.FilePath;
            FileContentLines = errorContext.FileContentLines;
            FileIndex = errorContext.FileIndex;
            AssemblyBuilder = errorContext.AssemblyBuilder;
        }
    }
    
    public abstract class CompilationMessage {
        public string FilePath;
        public string FileName;
        protected int _fileNo;
        protected int _lineNo;
        protected int _columnNo;
        protected string _line;
        
        public string ExecBlockName;
        public string ExecBlockType;

        protected abstract void PrintMessage(ErrorLogger logger);
        
        public void Print(ErrorLogger logger)
        {
            PrintMessage(logger);
            logger.Log($"{_line}");
            PrintErrorPointer(logger);
        }

        protected CompilationMessage(ErrorContext errorContext)
        {
            ParserRuleContext parserContext = errorContext.Context;
            
            ExecBlockName = null;
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
                }

                context = context.Parent;
                if (context is DaedalusParser.DaedalusFileContext)
                {
                    break;
                }
            }
            
            FilePath = errorContext.FilePath;
            FileName = Path.GetFileName(FilePath);
            _fileNo = errorContext.FileIndex;
            _lineNo = parserContext.Start.Line;
            _columnNo = parserContext.Start.Column;
            _line = errorContext.FileContentLines[_lineNo - 1];
        }
        public int CompareTo(CompilationMessage message)
        {
            int result = _fileNo.CompareTo(message._fileNo);
            
            if (result == 0)
            {
                result = _lineNo.CompareTo(message._lineNo);
            }

            if (result == 0)
            {
                result = _columnNo.CompareTo(message._columnNo);
            }

            return result;
        }
        
        protected string GetErrorPointerLine(int whitespaceCount)
        {
            string errorPointerLine = "";
            for (int i = 0; i < whitespaceCount; i++)
            {
                if (_line[i] == '\t')
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
        
        protected virtual void PrintErrorPointer(ErrorLogger logger)
        {
            logger.Log($"{GetErrorPointerLine(_columnNo)}");
        }
    }

    public abstract class CompilationError : CompilationMessage
    {
        protected CompilationError(ErrorContext errorContext) : base(errorContext)
        {
        }
    }

    public abstract class CompilationWarning : CompilationMessage
    {
        protected CompilationWarning(ErrorContext errorContext) : base(errorContext)
        {
        }
    }
    
    public class UndeclaredIdentifierError : CompilationError {
        
        protected string _identifier;

        public UndeclaredIdentifierError(ErrorContext errorContext) : base(errorContext) {
            ParserRuleContext referenceContext = errorContext.Context;
            _identifier = referenceContext.GetText();
        }


        protected override void PrintMessage(ErrorLogger logger)
        {
            logger.Log($"{FileName}:{_lineNo}:{_columnNo}: error: ‘{_identifier.Split(".").First()}’ undeclared");
        }
    }


    public class AttributeNotFoundError : UndeclaredIdentifierError
    {

        private readonly string _identifierPrefix;
        private readonly string _className;
        
        
        public AttributeNotFoundError(ErrorContext errorContext) : base(errorContext)
        {           
            string[] identifier = _identifier.Split(".");
            string objectName = identifier[0];
            _identifierPrefix = objectName + '.';
            _identifier = identifier[1];

            
            DatSymbol symbol = errorContext.AssemblyBuilder.GetSymbolByName(objectName);
            while (symbol.ParentIndex != -1)
            {
                if (symbol.Type == DatSymbolType.Class)
                {
                    _className = symbol.Name;
                    break;
                }
                symbol = errorContext.AssemblyBuilder.Symbols[symbol.ParentIndex];
            }          
        }
        
        
        protected override void PrintMessage(ErrorLogger logger)
        {
            if (_className == null)
            {
                string[] identifier = _identifierPrefix.Split(".");
                string objectName = identifier[0];
                logger.Log($"{FileName}:{_lineNo}:{_columnNo}: error: ‘object {objectName}’ has no member named ‘{_identifier}’");
            }
            else
            {
                logger.Log($"{FileName}:{_lineNo}:{_columnNo}: error: ‘class {_className}’ has no member named ‘{_identifier}’");
            }
            
        }

        protected override void PrintErrorPointer(ErrorLogger logger)
        {
            logger.Log($"{GetErrorPointerLine(_columnNo + _identifierPrefix.Length)}");
        }
    }
    
    public class InconsistentArraySizeError : CompilationError {
        private readonly string _identifier;
        private readonly int _declaredSize;
        private readonly int _elementsCount;
 
        public InconsistentArraySizeError(ErrorContext errorContext, string identifier, int declaredSize, int elementsCount) : base(errorContext)
        {
            _identifier = identifier;
            _declaredSize = declaredSize;
            _elementsCount = elementsCount;
        }


        protected override void PrintMessage(ErrorLogger logger)
        {
            logger.Log($"{FileName}:{_lineNo}:{_columnNo}: error: array ‘{_identifier}’ has inconsistent size (declared size: {_declaredSize}, elements count: {_elementsCount})");
        }
    }
    
    public class InvalidArraySizeError : CompilationError {
        private readonly string _identifier;
        private readonly int _declaredSize;
 
        public InvalidArraySizeError(ErrorContext errorContext, string identifier, int declaredSize) : base(errorContext)
        {
            _identifier = identifier;
            _declaredSize = declaredSize;
        }


        protected override void PrintMessage(ErrorLogger logger)
        {
            logger.Log($"{FileName}:{_lineNo}:{_columnNo}: error: array ‘{_identifier}’ has invalid size ‘{_declaredSize}’");
        }
    }
    
    
    public class IntegerLiteralTooLargeError : CompilationError {
 
        public IntegerLiteralTooLargeError(ErrorContext errorContext) : base(errorContext)
        {
        }


        protected override void PrintMessage(ErrorLogger logger)
        {
            logger.Log($"{FileName}:{_lineNo}:{_columnNo}: error: integer literal is too large to be represented in an integer type");
        }
    }
}