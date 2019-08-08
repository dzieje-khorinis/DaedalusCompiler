using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    public abstract class NodeAnnotation
    {
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
    public class RedefinedIdentifierAnnotation : NodeAnnotation
    {
    }
    
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