using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Antlr4.Runtime;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    public abstract class ErrorLogger
    {
        public abstract void LogLine(string message);
    }

    public class StdErrorLogger : ErrorLogger
    {
        public override void LogLine(string message)
        {
            Console.WriteLine(message);
        }
        
        public void Log(string message)
        {
            Console.Write(message);
        }
    }

    public class StringBufforErrorLogger : ErrorLogger
    {
        private string buffor;

        public override void LogLine(string message)
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

    public abstract class BaseErrorContext
    {
        public ParserRuleContext ParserContext;
        public string FilePath;
    }

    public class ErrorLineContext : BaseErrorContext
    {
        public readonly string FileContentLine;

        public ErrorLineContext(ErrorFileContext errorFileContext)
        {
            ParserContext = errorFileContext.ParserContext;
            FilePath = errorFileContext.FilePath;

            FileContentLine = errorFileContext.FileContentLines[errorFileContext.ParserContext.Start.Line - 1];
        }
    }
    
    public class ErrorFileContext : BaseErrorContext
    {
        public int FileIndex;
        public readonly AssemblyBuilder AssemblyBuilder;
        public string[] FileContentLines;
        public string[] SuppressedWarningCodes;
        
        public static readonly DatSymbol UndeclaredSymbol = new DatSymbol();

        public ErrorFileContext(AssemblyBuilder assemblyBuilder)
        {
            AssemblyBuilder = assemblyBuilder;
        }

        public ErrorFileContext(ErrorFileContext errorFileContext)
        {
            ParserContext = errorFileContext.ParserContext;
            FilePath = errorFileContext.FilePath;
            
            FileIndex = errorFileContext.FileIndex;
            AssemblyBuilder = errorFileContext.AssemblyBuilder;
            FileContentLines = errorFileContext.FileContentLines;
            SuppressedWarningCodes = errorFileContext.SuppressedWarningCodes;
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

        
        public static string GetIdentifierRelativeName(string identifier) {
            if (identifier.Contains("."))
            {
                identifier = identifier.Split(".")[1];
            }

            return identifier;
        }
        
        protected abstract void PrintMessage(ErrorLogger logger);

        protected virtual void PrintNote(ErrorLogger logger)
        {
            
        }
        
        public void Print(ErrorLogger logger)
        {
            PrintMessage(logger);
            logger.LogLine($"{_line}");
            PrintErrorPointer(logger);
            PrintNote(logger);
        }

        protected CompilationMessage(ErrorFileContext errorFileContext)
        {
            ParserRuleContext parserContext = errorFileContext.ParserContext;
            
            ExecBlockName = null;


            bool displayExecBlock = true;
            
            if (parserContext is DaedalusParser.NameNodeContext)
            {
                if (parserContext.Parent is DaedalusParser.FunctionDefContext
                    || parserContext.Parent is DaedalusParser.InstanceDefContext
                    || parserContext.Parent is DaedalusParser.PrototypeDefContext
                    || parserContext.Parent is DaedalusParser.ClassDefContext)
                {
                    displayExecBlock = false;
                }
            }

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

            FilePath = errorFileContext.FilePath;
            FileName = Path.GetFileName(FilePath);
            _fileNo = errorFileContext.FileIndex;
            _lineNo = parserContext.Start.Line;
            _columnNo = parserContext.Start.Column;
            _line = errorFileContext.FileContentLines[_lineNo - 1];
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
        
        protected string GetErrorPointerLine(string line, int whitespaceCount)
        {
            string errorPointerLine = "";
            for (int i = 0; i < whitespaceCount; i++)
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
        
        protected virtual void PrintErrorPointer(ErrorLogger logger, int columnNo, string line)
        {
            logger.LogLine($"{GetErrorPointerLine(line, columnNo)}");
        }
        
        protected virtual void PrintErrorPointer(ErrorLogger logger, int columnNo)
        {
            PrintErrorPointer(logger, columnNo, _line);
        }
        
        protected virtual void PrintErrorPointer(ErrorLogger logger)
        {
            PrintErrorPointer(logger, _columnNo, _line);
        }
        
    }

    public abstract class CompilationError : CompilationMessage
    {
        protected CompilationError(ErrorFileContext errorFileContext) : base(errorFileContext)
        {
        }
    }

    public abstract class CompilationWarning : CompilationMessage
    {
        protected readonly string MessageType;
        public bool IsSuppressed;
        public const string Code = "UNKNOWN";

        protected CompilationWarning(ErrorFileContext errorFileContext, bool strictSyntax) : base(errorFileContext)
        {
            IsSuppressed = false;
            MessageType = strictSyntax ? "error" : "warning";

            string[] suppressedLineWarningCodes = Compiler.GetWarningCodesToSuppress(_line);
            string[] suppressedFileWarningCodes = errorFileContext.SuppressedWarningCodes;
            string[] suppressedWarningCodes = suppressedLineWarningCodes.Concat(suppressedFileWarningCodes).ToArray();
            string code = GetType().GetField("Code").GetValue(null).ToString();

            foreach (var warningCode in suppressedWarningCodes)
            {
                if (warningCode.Equals(code))
                {
                    IsSuppressed = true;
                    break;
                }
            }
        }
    }

    public class SingleExpressionWarning : CompilationWarning
    {
        public new const string Code = "W1";
        public SingleExpressionWarning(ErrorFileContext errorFileContext, bool strictSyntax) : base(errorFileContext, strictSyntax) { }
        protected override void PrintMessage(ErrorLogger logger)
        {
            logger.LogLine($"{FileName}:{_lineNo}:{_columnNo}: {MessageType} {Code}: usage of single-expression statement hack");
        }
    }
    
    public class UndeclaredIdentifierError : CompilationError {
        
        protected string _identifier;

        public UndeclaredIdentifierError(ErrorFileContext errorFileContext) : base(errorFileContext) {
            ParserRuleContext referenceContext = errorFileContext.ParserContext;
            _identifier = referenceContext.GetText();
        }

        protected override void PrintMessage(ErrorLogger logger)
        {
            logger.LogLine($"{FileName}:{_lineNo}:{_columnNo}: error: ‘{_identifier.Split(".").First()}’ undeclared");
        }
    }


    public class AttributeNotFoundError : UndeclaredIdentifierError
    {

        private readonly string _identifierPrefix;
        private readonly string _className;
        
        
        public AttributeNotFoundError(ErrorFileContext errorFileContext) : base(errorFileContext)
        {           
            string[] identifier = _identifier.Split(".");
            string objectName = identifier[0];
            _identifierPrefix = objectName + '.';
            _identifier = identifier[1];

            
            DatSymbol symbol = errorFileContext.AssemblyBuilder.GetSymbolByName(objectName);
            while (symbol.ParentIndex != -1)
            {
                if (symbol.Type == DatSymbolType.Class)
                {
                    _className = symbol.Name;
                    break;
                }
                symbol = errorFileContext.AssemblyBuilder.Symbols[symbol.ParentIndex];
            }          
        }
        
        
        protected override void PrintMessage(ErrorLogger logger)
        {
            if (_className == null)
            {
                string[] identifier = _identifierPrefix.Split(".");
                string objectName = identifier[0];
                logger.LogLine($"{FileName}:{_lineNo}:{_columnNo}: error: ‘object {objectName}’ has no member named ‘{_identifier}’");
            }
            else
            {
                logger.LogLine($"{FileName}:{_lineNo}:{_columnNo}: error: ‘class {_className}’ has no member named ‘{_identifier}’");
            }
            
        }

        protected override void PrintErrorPointer(ErrorLogger logger)
        {
            PrintErrorPointer(logger, _columnNo + _identifierPrefix.Length);
        }
    }
    
    public class InconsistentArraySizeError : CompilationError {
        private readonly string _identifier;
        private readonly int _declaredSize;
        private readonly int _elementsCount;
 
        public InconsistentArraySizeError(ErrorFileContext errorFileContext, string identifier, int declaredSize, int elementsCount) : base(errorFileContext)
        {
            _identifier = GetIdentifierRelativeName(identifier);
            _declaredSize = declaredSize;
            _elementsCount = elementsCount;
        }


        protected override void PrintMessage(ErrorLogger logger)
        {
            logger.LogLine($"{FileName}:{_lineNo}:{_columnNo}: error: array ‘{_identifier}’ has inconsistent size (declared size: {_declaredSize}, elements count: {_elementsCount})");
        }
    }
    
    public class TooBigArraySizeError : CompilationError {
        
        public TooBigArraySizeError(ErrorFileContext errorFileContext) : base(errorFileContext)
        {

        }

        protected override void PrintMessage(ErrorLogger logger)
        {
            logger.LogLine($"{FileName}:{_lineNo}:{_columnNo}: error: too big array size (max: {AssemblyBuilder.MAX_ARRAY_SIZE})");
        }
    }
    
    public class InvalidArraySizeError : CompilationError {
        private readonly string _identifier;
        private readonly int _declaredSize;
 
        public InvalidArraySizeError(ErrorFileContext errorFileContext, string identifier, int declaredSize) : base(errorFileContext)
        {
            _identifier = GetIdentifierRelativeName(identifier);
            _declaredSize = declaredSize;
        }


        protected override void PrintMessage(ErrorLogger logger)
        {
            logger.LogLine($"{FileName}:{_lineNo}:{_columnNo}: error: array ‘{_identifier}’ has invalid size ‘{_declaredSize}’");
        }
    }
    
    
    public class IntegerLiteralTooLargeError : CompilationError {
 
        public IntegerLiteralTooLargeError(ErrorFileContext errorFileContext) : base(errorFileContext)
        {
        }


        protected override void PrintMessage(ErrorLogger logger)
        {
            logger.LogLine($"{FileName}:{_lineNo}:{_columnNo}: error: integer literal is too large to be represented in an integer type");
        }
    }

    public class UnableToEvaluateConstError : CompilationError {
 
        public UnableToEvaluateConstError(ErrorFileContext errorFileContext) : base(errorFileContext)
        {
        }


        protected override void PrintMessage(ErrorLogger logger)
        {
            logger.LogLine($"{FileName}:{_lineNo}:{_columnNo}: error: unable to evaluate const value");
        }
    }
    
    public class NotValidClassOrPrototype : CompilationError {
 
        public NotValidClassOrPrototype(ErrorFileContext errorFileContext) : base(errorFileContext)
        {
        }


        protected override void PrintMessage(ErrorLogger logger)
        {
            logger.LogLine($"{FileName}:{_lineNo}:{_columnNo}: error: not a valid class or prototype");
        }
    }
    
    public class ArgumentsCountDoesNotMatchError : CompilationError {
        private readonly uint _parametersCount;
        private readonly uint _argumentsCount;
        private readonly ErrorLineContext _errorLineContext;

        public ArgumentsCountDoesNotMatchError(
            ErrorFileContext errorFileContext,
            ErrorLineContext errorLineContext,
            uint parametersCount,
            uint argumentsCount) : base(errorFileContext)
        {
            _errorLineContext = errorLineContext;
            _parametersCount = parametersCount;
            _argumentsCount = argumentsCount;
        }

        protected override void PrintMessage(ErrorLogger logger)
        {
            logger.LogLine($"{FileName}:{_lineNo}:{_columnNo}: error: too {(_parametersCount > _argumentsCount ?  "few" : "many")} arguments to function call, expected {_parametersCount}, have {_argumentsCount}");
        }

        protected override void PrintNote(ErrorLogger logger)
        {
            ParserRuleContext parserContext = _errorLineContext.ParserContext;
            
            string fileName = Path.GetFileName(_errorLineContext.FilePath);
            int lineNo = parserContext.Start.Line;
            int columnNo = parserContext.Start.Column;
            string identifier = GetIdentifierRelativeName(parserContext.GetText());
            string line = _errorLineContext.FileContentLine;
            logger.LogLine($"{fileName}:{lineNo}:{columnNo}: note: '{identifier}' declared here");
            logger.LogLine(line);
            PrintErrorPointer(logger, columnNo, line);
        }
    }
    
    public class RedefinitionError : CompilationError
    {
        private readonly string _identifier;
        private readonly ErrorLineContext _errorLineContext;

        public RedefinitionError(
            ErrorFileContext errorFileContext,
            ErrorLineContext errorLineContext,
            string identifier) : base(errorFileContext)
        {
            _identifier = GetIdentifierRelativeName(identifier);
            _errorLineContext = errorLineContext;
        }

        protected override void PrintMessage(ErrorLogger logger)
        {
            logger.LogLine($"{FileName}:{_lineNo}:{_columnNo}: error: redefinition of '{_identifier}'");
        }

        protected override void PrintNote(ErrorLogger logger)
        {
            ParserRuleContext parserContext = _errorLineContext.ParserContext;
            
            string fileName = Path.GetFileName(_errorLineContext.FilePath);
            int lineNo = parserContext.Start.Line;
            int columnNo = parserContext.Start.Column;
            string line = _errorLineContext.FileContentLine;
            logger.LogLine($"{fileName}:{lineNo}:{columnNo}: note: previous definition is here");
            logger.LogLine(line);
            PrintErrorPointer(logger, columnNo, line);
        }
    }
}