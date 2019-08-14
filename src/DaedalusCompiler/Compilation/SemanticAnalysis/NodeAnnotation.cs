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

    public interface ILocatedAnnotation
    {
        NodeLocation Location { get; set; }
    }
    
    public abstract class ErrorAnnotation : NodeAnnotation
    {
        
    }

    public abstract class WarningAnnotation : NodeAnnotation
    {
        //public const string Code = "WARNING";
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

    public interface IWithCode
    {
        string Code { get; set; }
    }


    public class SingleExpressionWarning : WarningAnnotation, IWithCode
    {
        public string Code { get; set; } = "W1";
        public override string GetMessage()
        {
            return "usage of single-expression statement hack";
        }
    }
    
    
    public class IncompatibleTypesError : ErrorAnnotation
    {
        public SymbolType ExpectedSymbolType;
        public SymbolType ActualSymbolType;

        public IncompatibleTypesError(SymbolType expectedSymbolType, SymbolType actualSymbolType)
        {
            ExpectedSymbolType = expectedSymbolType;
            ActualSymbolType = actualSymbolType;
        }

        public override string GetMessage()
        {
            return $"{ExpectedSymbolType} type expected (actual type: {ActualSymbolType})".ToLower();
        }
    }

    public class UndeclaredIdentifierError : ErrorAnnotation
    {
        private readonly string _identifier;

        public UndeclaredIdentifierError(string identifier)
        {
            _identifier = identifier;
        }

        public override string GetMessage()
        {
            return $"‘{_identifier}’ undeclared";
        }
    }

    public class IndexOutOfRangeError : ErrorAnnotation
    {
        private readonly long _arraySize;

        public IndexOutOfRangeError(long arraySize)
        {
            _arraySize = arraySize;
        }

        public override string GetMessage()
        {
            return $"array index out of range (max index for this array is {_arraySize - 1})";
        }
    }

    public class TooBigArrayIndex : ErrorAnnotation
    {
        public override string GetMessage()
        {
            return "too big array index (max: 255)";
        }
    }

    public class KeywordUsedAsNameError : ErrorAnnotation, ILocatedAnnotation
    {
        public NodeLocation Location { get; set; }
        private readonly string _identifier;

        public KeywordUsedAsNameError(string identifier, NodeLocation location)
        {
            _identifier = identifier;
            Location = location;
        }

        public override string GetMessage()
        {
            return $"‘{_identifier}’ is keyword and shouldn't be used as an identifier";
        }
    }

    public class IterationStatementNotInLoopError : ErrorAnnotation
    {
        private readonly string _identifier;

        public IterationStatementNotInLoopError(string identifier)
        {
            _identifier = identifier;
        }

        public override string GetMessage()
        {
            return $"‘{_identifier}’ statement not allowed outside of loop statement";
        }
    }
    
    public class ConstIntegerExpectedError : ErrorAnnotation
    {
    }


    public class NotConstReferenceError : ErrorAnnotation
    {
    }
    
    public class ReferencedSymbolIsNotArrayError : ErrorAnnotation
    {
    }

    public class NotClassOrPrototypeReferenceError : ErrorAnnotation
    {
        public override string GetMessage()
        {
            return "not a valid class or prototype";
        }
    }

    public class TooBigArraySizeError : ErrorAnnotation
    {
        public override string GetMessage()
        {
            return "too big array size (max: 4095)";
        }
    }

    public class AttributeOfNonInstanceError : ErrorAnnotation
    {
        
    }
    public class RedefinedIdentifierError : ErrorAnnotationNoted, ILocatedAnnotation
    {
        private readonly string _identifier;
        public NodeLocation Location { get; set; }

        public RedefinedIdentifierError(string identifier, NodeLocation noteLocation, NodeLocation location) : base(noteLocation)
        {
            _identifier = identifier;
            Location = location;
        }

        public override string GetMessage()
        {
            return $"redefinition of ‘{_identifier}’";
        }

        public override string GetNote()
        {
            return "previous definition is here";
        }

        
    }

    public class InfiniteReferenceLoopError : ErrorAnnotation
    {
    }

    public class InvalidBinaryOperationError : ErrorAnnotation
    {
    }

    public class ArithmeticOperationOverflowError : ErrorAnnotation, ILocatedAnnotation
    {
        public NodeLocation Location { get; set; }

        public ArithmeticOperationOverflowError(NodeLocation location)
        {
            Location = location;
        }
        
        public override string GetMessage()
        {
            return "arithmetic operation resulted in an overflow";
        }

        
    }
    
    public class InvalidUnaryOperationError : ErrorAnnotation
    {
    }

    public class AccessToAttributeOfArrayElementNotSupportedError : ErrorAnnotation
    {
    }


    public class InconsistentSizeError : ErrorAnnotation
    {
        private readonly string _identifier;
        private readonly int _declaredSize;
        private readonly int _elementsCount;

        public InconsistentSizeError(string identifier, int declaredSize, int elementsCount)
        {
            _declaredSize = declaredSize;
            _elementsCount = elementsCount;
            _identifier = identifier;
        }

        public override string GetMessage()
        {
            return $"array ‘{_identifier}’ has inconsistent size (declared size: {_declaredSize}, elements count: {_elementsCount})";
        }
    }

    public class ArraySizeEqualsZeroError : ErrorAnnotation
    {
        private readonly string _identifier;

        public ArraySizeEqualsZeroError(string identifier)
        {
            _identifier = identifier;
        }

        public override string GetMessage()
        {
            return $"size of array ‘{_identifier}’ cannot equal zero";
        }
    }

    public class IntegerLiteralTooLargeError : ErrorAnnotation
    {
        public override string GetMessage()
        {
            return "literal is too large to be represented in an integer type (min: -2147483648, max: 2147483647)";
        }
    }

    public class UnsupportedArrayTypeError : ErrorAnnotation
    {
        
    }

    public class UnsupportedTypeError : ErrorAnnotation
    {
        
    }
    
    public class UndefinedTypeError : ErrorAnnotation
    {
        
    }


    public class ClassDoesNotHaveAttributeError : ErrorAnnotation
    {
        private readonly string _objectName;
        private readonly string _className;
        private readonly string _attributeName;

        public ClassDoesNotHaveAttributeError(string objectName, string className, string attributeName)
        {
            _objectName = objectName;
            _className = className;
            _attributeName = attributeName;
        }

        public override string GetMessage()
        {
            return $"object ‘{_objectName}’ of type ‘{_className}’ has no member named ‘{_attributeName}’";
        }

    }
}