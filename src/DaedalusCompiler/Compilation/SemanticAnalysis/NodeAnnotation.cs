using System;
using System.Collections.Generic;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    public abstract class NodeAnnotation
    {
        public NodeLocation PointerLocation;
        public List<NodeLocation> UnderlineLocations;
        
        public NodeAnnotation()
        {
            PointerLocation = null;
            UnderlineLocations = new List<NodeLocation>();
        }
        
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
    
    
    public class CannotInitializeConstWithValueOfDifferentType : ErrorAnnotation
    {
        private readonly SymbolType _expectedSymbolType;
        private readonly SymbolType _actualSymbolType;

        public CannotInitializeConstWithValueOfDifferentType(SymbolType expectedSymbolType, SymbolType actualSymbolType, NodeLocation pointerLocation, NodeLocation underlineLocation)
        {
            PointerLocation = pointerLocation;
            UnderlineLocations.Add(underlineLocation);
            _expectedSymbolType = expectedSymbolType;
            _actualSymbolType = actualSymbolType;
        }
        
        public override string GetMessage()
        {
            return $"cannot initialize a constant of type '{_expectedSymbolType}' with an rvalue of type '{_actualSymbolType}'".ToLower();
        }
    }

    public class CannotInitializeArrayElementWithValueOfDifferentType : ErrorAnnotation
    {
        private readonly SymbolType _expectedSymbolType;
        private readonly SymbolType _actualSymbolType;

        public CannotInitializeArrayElementWithValueOfDifferentType(NodeLocation pointerLocation, SymbolType expectedSymbolType, SymbolType actualSymbolType)
        {
            PointerLocation = pointerLocation;
            UnderlineLocations.Add(pointerLocation);
            _expectedSymbolType = expectedSymbolType;
            _actualSymbolType = actualSymbolType;
        }
        
        public override string GetMessage()
        {
            return $"cannot initialize an array element of type '{_expectedSymbolType}' with an rvalue of type '{_actualSymbolType}'".ToLower();
        }
    }

    public class ArgumentsCountDoesNotMatchError : ErrorAnnotationNoted
    {
        private readonly string _identifier;
        private readonly int _parametersCount;
        private readonly int _argumentsCount;

        public ArgumentsCountDoesNotMatchError(string identifier, int parametersCount, int argumentsCount, NodeLocation noteLocation) : base(noteLocation)
        {
            _identifier = identifier;
            _parametersCount = parametersCount;
            _argumentsCount = argumentsCount;
        }

        public override string GetMessage()
        {
            return
                $"too {(_parametersCount > _argumentsCount ? "few" : "many")} arguments to function call, expected {_parametersCount}, have {_argumentsCount}";
        }

        public override string GetNote()
        {
            return $"'{_identifier}' declared here";
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
            return $"'{_identifier}' undeclared";
        }
    }
    
    public class UnknownTypeNameError : ErrorAnnotation
    {
        private readonly string _identifier;

        public UnknownTypeNameError(string identifier, NodeLocation pointerLocation)
        {
            _identifier = identifier;
            PointerLocation = pointerLocation;
        }

        public override string GetMessage()
        {
            return $"unknown type name '{_identifier}'";
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

    public class KeywordUsedAsNameError : ErrorAnnotation
    {
        private readonly string _identifier;

        public KeywordUsedAsNameError(string identifier, NodeLocation pointerLocation)
        {
            _identifier = identifier;
            PointerLocation = pointerLocation;
        }

        public override string GetMessage()
        {
            return $"'{_identifier}' is keyword and shouldn't be used as an identifier";
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
            return $"'{_identifier}' statement not allowed outside of loop statement";
        }
    }
    
    public class ConstIntegerExpectedError : ErrorAnnotation
    {
    }


    public class NotConstReferenceError : ErrorAnnotation
    {
        public override string GetMessage()
        {
            return "const reference required";
        }
    }
    
    public class ReferencedSymbolIsNotArrayError : ErrorAnnotation
    {
        private readonly string _identifier;

        public ReferencedSymbolIsNotArrayError(string identifier)
        {
            _identifier = identifier;
        }

        public override string GetMessage()
        {
            return $"cannot access array element because '{_identifier}' is not an array";
        }
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
        private readonly string _identifier;
        private readonly string _objectIdentifier;

        public AttributeOfNonInstanceError(string identifier, string objectIdentifier)
        {
            _identifier = identifier;
            _objectIdentifier = objectIdentifier;
        }

        public override string GetMessage()
        {
            return $"cannot access attribute '{_identifier}' because '{_objectIdentifier}' is not an instance of a class";
        }
    }
    public class RedefinedIdentifierError : ErrorAnnotationNoted
    {
        private readonly string _identifier;

        public RedefinedIdentifierError(string identifier, NodeLocation noteLocation, NodeLocation pointerLocation) : base(noteLocation)
        {
            _identifier = identifier;
            PointerLocation = pointerLocation;
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

    public class InfiniteInheritanceReferenceLoopError : ErrorAnnotation
    {
        public override string GetMessage()
        {
            return "circular inheritance dependency detected";
        }
    }
    
    public class InfiniteAttributeReferenceLoopError : ErrorAnnotation
    {
        public override string GetMessage()
        {
            return "circular attribute reference dependency detected";
        }
    }
    
    public class InfiniteConstReferenceLoopError : ErrorAnnotation
    {
        public override string GetMessage()
        {
            return "circular const reference dependency detected";
        }
    }

    public class InvalidBinaryOperationError : ErrorAnnotation
    {
    }



    public class ArithmeticOperationOverflowError : ErrorAnnotation
    {
        public ArithmeticOperationOverflowError(NodeLocation pointerLocation)
        {
            PointerLocation = pointerLocation;
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
        public override string GetMessage()
        {
            return "access to attribute of array element not supported";
        }
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
            return $"array '{_identifier}' has inconsistent size (declared size: {_declaredSize}, elements count: {_elementsCount})";
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
            return $"size of array '{_identifier}' cannot equal zero";
        }
    }

    public class IntegerLiteralTooLargeError : ErrorAnnotation
    {
        public override string GetMessage()
        {
            return "integer literal is too large to be represented in an integer type (min: -2147483648, max: 2147483647)";
        }
    }

    public class UnsupportedArrayTypeError : ErrorAnnotation
    {
        public UnsupportedArrayTypeError(NodeLocation pointerLocation)
        {
            PointerLocation = pointerLocation;
        }
        public override string GetMessage()
        {
            return "unsupported array type";
        }
    }
    
    public class UnsupportedFunctionTypeError : ErrorAnnotation
    {
        public UnsupportedFunctionTypeError(NodeLocation pointerLocation)
        {
            PointerLocation = pointerLocation;
        }
        public override string GetMessage()
        {
            return "unsupported function return type";
        }
    }

    public class UnsupportedTypeError : ErrorAnnotation
    {
        public UnsupportedTypeError(NodeLocation pointerLocation)
        {
            PointerLocation = pointerLocation;
        }
        public override string GetMessage()
        {
            return "unsupported type";
        }
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
            return $"object '{_objectName}' of type '{_className}' has no member named '{_attributeName}'";
        }

    }
}