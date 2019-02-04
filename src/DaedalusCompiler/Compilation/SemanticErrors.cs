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
                buffor = $"{buffor}\n{message}";
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

        public abstract void Print(ErrorLogger logger);

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
    }

    public abstract class CompilationError : CompilationMessage
    {
    }

    public abstract class CompilationWarning : CompilationMessage
    {
    }
    
    public class UndeclaredIdentifierError : CompilationError {
        
        protected string _identifier;

        public UndeclaredIdentifierError(ErrorContext errorContext) {
            ParserRuleContext referenceContext = errorContext.Context;
            
            _identifier = referenceContext.GetText();
            
            ExecBlockName = null;

            RuleContext context = referenceContext.Parent;
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
            _lineNo = referenceContext.Start.Line;
            _columnNo = referenceContext.Start.Column;
            _line = errorContext.FileContentLines[_lineNo - 1];
        }

        public override void Print(ErrorLogger logger)
        {
            PrintMessage(logger);
            logger.Log($"{_line}");
            PrintErrorPointer(logger);
        }

        protected virtual void PrintMessage(ErrorLogger logger)
        {
            logger.Log($"{FileName}:{_lineNo}:{_columnNo}: error: ‘{_identifier.Split(".").First()}’ undeclared");
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
    
}