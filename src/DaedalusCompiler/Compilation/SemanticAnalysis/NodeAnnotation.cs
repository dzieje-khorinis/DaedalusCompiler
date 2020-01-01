using System;
using System.Collections.Generic;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    public abstract class NodeAnnotation
    {
        public NodeLocation PointerLocation;
        public readonly List<NodeLocation> UnderlineLocations;

        protected NodeAnnotation()
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
    
    public class NamesNotMatchingCaseWiseWarning : WarningAnnotationNoted, IWithCode
    {
        public static string WCode = "W2";
        private readonly string _declaredName;
        private readonly string _usedName;
        
        public string Code { get; set; } = WCode;

        public NamesNotMatchingCaseWiseWarning(NodeLocation noteLocation, string declaredName, string usedName) : base(noteLocation)
        {
            _declaredName = declaredName;
            _usedName = usedName;
        }
        public override string GetMessage()
        {
            return $"name '{_usedName}' doesn't match declared name '{_declaredName}' case wise";
        }
        
        public override string GetNote()
        {
            return $"'{_declaredName}' declared here";
        }
    }
    
    public class UnusedSymbolWarning : WarningAnnotation, IWithCode
    {
        public static string WCode = "W3";
        public string Code { get; set; } = WCode;
        
        public override string GetMessage()
        {
            return "unused symbol";
        }
    }
    
    public class ConstValueChangedWarning : WarningAnnotation, IWithCode
    {
        public string Code { get; set; } = "W4";
        
        private readonly string _identifier;
        
        public ConstValueChangedWarning(string identifier)
        {
            _identifier = identifier;
        }
        
        public override string GetMessage()
        {
            return $"'{_identifier}' is a const and shouldn't have its value changed";
        }
    }
    
    public class UsageOfNonInitializedVariableWarning : WarningAnnotation, IWithCode
    {
        /*
         * - use of non-initialized local variables
         * - use of non-initialized class attributes in prototype / instance
         */
        public string Code { get; set; } = "W5";

        private readonly string _identifier;
        private readonly long _index;
        private readonly bool _isAttribute;

        public UsageOfNonInitializedVariableWarning(string identifier, long index, bool isAttribute)
        {
            _identifier = identifier;
            _index = index;
            _isAttribute = isAttribute;
        }

        public override string GetMessage()
        {
            string typeOfVariable = "variable";
            if (_isAttribute)
            {
                typeOfVariable = "attribute";
            }

            string arrayElementText = "";
            if (_index != -1)
            {
                arrayElementText = $"element (index '{_index}') of array ";
            }

            return $"usage of non-initialized {arrayElementText}{typeOfVariable} '{_identifier}'";
        }
    }

    
    
    public class IncompatibleTypesError : ErrorAnnotation
    {
        private readonly SymbolType _expectedSymbolType;
        private readonly SymbolType _actualSymbolType;

        public IncompatibleTypesError(SymbolType expectedSymbolType, SymbolType actualSymbolType)
        {
            _expectedSymbolType = expectedSymbolType;
            _actualSymbolType = actualSymbolType;
        }

        public override string GetMessage()
        {
            return $"{_expectedSymbolType} type expected (actual type: {_actualSymbolType})".ToLower();
        }
    }
    
    
    public class CannotInitializeConstWithValueOfDifferentTypeError : ErrorAnnotation
    {
        private readonly SymbolType _expectedSymbolType;
        private readonly SymbolType _actualSymbolType;

        public CannotInitializeConstWithValueOfDifferentTypeError(SymbolType expectedSymbolType, SymbolType actualSymbolType, NodeLocation pointerLocation, NodeLocation underlineLocation)
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

    public class CannotInitializeArrayElementWithValueOfDifferentTypeError : ErrorAnnotation
    {
        private readonly SymbolType _expectedSymbolType;
        private readonly SymbolType _actualSymbolType;

        public CannotInitializeArrayElementWithValueOfDifferentTypeError(NodeLocation pointerLocation, SymbolType expectedSymbolType, SymbolType actualSymbolType)
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
    
    public class SymbolIsNotAFunctionError : ErrorAnnotationNoted
    {
        private readonly string _identifier;

        public SymbolIsNotAFunctionError(string identifier, NodeLocation noteLocation) : base(noteLocation)
        {
            _identifier = identifier;
        }

        public override string GetMessage()
        {
            return $"'{_identifier}' is not a function and cannot be called";
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
    
    
    public class WrongClassSizeError : ErrorAnnotation
    {
        private readonly string _identifier;
        private readonly long _currentSize;
        private readonly long _requiredSize;

        public WrongClassSizeError(NodeLocation pointerLocation, string identifier, long currentSize, long requiredSize)
        {
            PointerLocation = pointerLocation;
            _identifier = identifier;
            _currentSize = currentSize;
            _requiredSize = requiredSize;
        }

        public override string GetMessage()
        {
            return $"size of class '{_identifier}' must be {_requiredSize} bytes (currently it's {_currentSize} bytes)";
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

    public class NotConstReferenceError : ErrorAnnotation
    {
        public override string GetMessage()
        {
            return "const reference required";
        }
    }
    
    public class ArraySizeNotConstIntegerError : ErrorAnnotation
    {
        public override string GetMessage()
        {
            return "array size must be of const integer type";
        }
    }
    
    public class ArrayIndexNotConstIntegerError : ErrorAnnotation
    {
        public override string GetMessage()
        {
            return "array index must be of const integer type";
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
        public InvalidBinaryOperationError(NodeLocation pointerLocation, NodeLocation leftLocation, NodeLocation rightLocation)
        {
            PointerLocation = pointerLocation;
            UnderlineLocations.Add(leftLocation);
            UnderlineLocations.Add(rightLocation);
        }
        
        public override string GetMessage()
        {
            return "invalid binary operation";
        }
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
    
    
    public class DivideByZeroError : ErrorAnnotation
    {
        
        public DivideByZeroError(NodeLocation pointerLocation, NodeLocation underlineLocation)
        {
            PointerLocation = pointerLocation;
            UnderlineLocations.Add(underlineLocation);
        }
        public override string GetMessage()
        {
            return "cannot divide by zero";
        }
    }

    public class InvalidArgumentTypeToUnaryExpressionError : ErrorAnnotation
    {
        private readonly string _typeName;

        public InvalidArgumentTypeToUnaryExpressionError(string typeName)
        {
            _typeName = typeName.ToLower();
        }

        public override string GetMessage()
        {
            return $"invalid argument type '{_typeName}' to unary expression";
        }
    }
    
    public class BinaryOperationsNotAllowedInsideFloatExpression : ErrorAnnotation
    {
        public BinaryOperationsNotAllowedInsideFloatExpression(NodeLocation pointerLocation)
        {
            PointerLocation = pointerLocation;
        }
        public override string GetMessage()
        {
            return "binary operations not allowed inside 'float' expression";
        }
    }
    
    
    public class FloatDoesntSupportCompoundAssignments : ErrorAnnotation
    {
        public FloatDoesntSupportCompoundAssignments(NodeLocation pointerLocation)
        {
            PointerLocation = pointerLocation;
        }
        public override string GetMessage()
        {
            return "'float' type doesn't have support for compound assignments";
        }
    }
    
    public class UnsupportedOperationError : ErrorAnnotation
    {
        public UnsupportedOperationError(NodeLocation pointerLocation)
        {
            PointerLocation = pointerLocation;
        }
        public override string GetMessage()
        {
            return "unsupported operation";
        }
    }
    

    
    public class IncompatibleTypeAssignmentError : ErrorAnnotation
    {
        private readonly string _leftTypeName;
        private readonly string _rightTypeName;
        public IncompatibleTypeAssignmentError(NodeLocation pointerLocation, string leftTypeName, string rightTypeName)
        {
            _leftTypeName = leftTypeName;
            _rightTypeName = rightTypeName;
            PointerLocation = pointerLocation;
        }
        public override string GetMessage()
        {
            return $"assigning to '{_leftTypeName}' from incompatible type '{_rightTypeName}'";
        }
    }

    public class InvalidOperandsToBinaryExpressionError : ErrorAnnotation
    {
        private readonly string _leftTypeName;
        private readonly string _rightTypeName;
        public InvalidOperandsToBinaryExpressionError(NodeLocation pointerLocation, string leftTypeName, string rightTypeName)
        {
            _leftTypeName = leftTypeName;
            _rightTypeName = rightTypeName;
            PointerLocation = pointerLocation;
        }
        public override string GetMessage()
        {
            return $"invalid operands to binary expression ('{_leftTypeName}' and '{_rightTypeName}')";
        }
    }

    
    public class InvalidUnaryOperationError : ErrorAnnotation
    {
        public override string GetMessage()
        {
            return "invalid unary operation";
        }
    }

    public class AccessToAttributeOfArrayElementNotSupportedError : ErrorAnnotation
    {
        public override string GetMessage()
        {
            return "access to attribute of array element not supported";
        }
    }


    public class InconsistentConstArraySizeError : ErrorAnnotation
    {
        private readonly string _identifier;
        private readonly int _declaredSize;
        private readonly int _elementsCount;

        public InconsistentConstArraySizeError(string identifier, int declaredSize, int elementsCount)
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