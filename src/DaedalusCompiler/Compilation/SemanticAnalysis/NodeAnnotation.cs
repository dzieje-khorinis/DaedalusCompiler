using System;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    public abstract class NodeAnnotation
    {
        public virtual string GetMessage()
        {
            return String.Empty;
        }
    }

    
    public interface INotedAnnotation
    {
        NodeLocation NoteLocation { get; set; }
        string GetNote();
    }
    
    
    public abstract class ErrorAnnotation : NodeAnnotation
    {
        
    }

    public abstract class WarningAnnotation : NodeAnnotation
    {
        public const string Code = "WARNING";
    }

    public abstract class ErrorAnnotationNoted : ErrorAnnotation, INotedAnnotation
    {
        public NodeLocation NoteLocation { get; set; }

        protected ErrorAnnotationNoted(NodeLocation noteLocation)
        {
            NoteLocation = noteLocation;
        }
        
        public virtual string GetNote()
        {
            return String.Empty;
        }
    }


    public abstract class WarningAnnotationNoted : WarningAnnotation, INotedAnnotation
    {
        public NodeLocation NoteLocation { get; set; }
        
        protected WarningAnnotationNoted(NodeLocation noteLocation)
        {
            NoteLocation = noteLocation;
        }
        
        public virtual string GetNote()
        {
            return String.Empty;
        }
    }
    
    
    

    public class IncompatibleTypesAnnotation : NodeAnnotation
    {
        public SymbolType ExpectedSymbolType;
        public SymbolType ActualSymbolType;

        public IncompatibleTypesAnnotation(SymbolType expectedSymbolType, SymbolType actualSymbolType)
        {
            ExpectedSymbolType = expectedSymbolType;
            ActualSymbolType = actualSymbolType;
        }
    }

    public class UndeclaredIdentifierAnnotation : NodeAnnotation
    {
    }

    public class IndexOutOfRangeAnnotation : NodeAnnotation
    {
    }
    
    public class ConstIntegerExpectedAnnotation : NodeAnnotation
    {
    }


    public class NotConstReferenceAnnotation : NodeAnnotation
    {
    }
    
    public class ReferencedSymbolIsNotArrayAnnotation : NodeAnnotation
    {
    }

    public class NotClassOrPrototypeReferenceAnnotation : NodeAnnotation
    {
        
    }

    public class AttributeOfNonInstanceAnnotation : NodeAnnotation
    {
        
    }
    public class RedefinedIdentifierAnnotation : ErrorAnnotationNoted
    {
        private readonly string _identifier;

        public RedefinedIdentifierAnnotation(string identifier, NodeLocation noteLocation) : base(noteLocation)
        {
            _identifier = identifier;
        }

        public override string GetMessage()
        {
            return $"redefinition of '{_identifier}'";
        }

        public override string GetNote()
        {
            return "previous definition is here";
        }
    }
    
    
     
    /*
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
     */

    
    public class InfiniteReferenceLoopAnnotation : NodeAnnotation
    {
    }

    public class InvalidBinaryOperationAnnotation : NodeAnnotation
    {
    }
    
    public class InvalidUnaryOperationAnnotation : NodeAnnotation
    {
    }

    public class AccessToAttributeOfArrayElementNotSupportedAnnotation : NodeAnnotation
    {
    }


    public class InconsistentSizeAnnotation : NodeAnnotation
    {
        
    }
    
    public class UnsupportedArrayTypeAnnotation : NodeAnnotation
    {
        
    }

    public class UnsupportedTypeAnnotation : NodeAnnotation
    {
        
    }
    
    public class UndefinedTypeAnnotation : NodeAnnotation
    {
        
    }

    
    public class ClassDoesNotHaveAttributeAnnotation : NodeAnnotation
    {
        public string ClassName;
        public ClassDoesNotHaveAttributeAnnotation(string className)
        {
            ClassName = className;
        }
    }
    
    
}