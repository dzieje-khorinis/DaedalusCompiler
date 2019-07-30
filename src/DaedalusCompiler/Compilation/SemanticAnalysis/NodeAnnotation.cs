using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    public abstract class NodeAnnotation
    {
    }


    public class IncompatibleTypesAnnotation : NodeAnnotation
    {
        public DatSymbolType LeftSymbolType;
        public DatSymbolType RightSymbolType;

        public IncompatibleTypesAnnotation(DatSymbolType leftSymbolType, DatSymbolType rightSymbolType)
        {
            LeftSymbolType = leftSymbolType;
            RightSymbolType = rightSymbolType;
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

    public class SquareBracketsExpectedAnnotation : NodeAnnotation
    {
    }
    
    public class SquareBracketsNotExpectedAnnotation : NodeAnnotation
    {
    }
    
    public class NotConstReferenceAnnotation : NodeAnnotation
    {
    }
    
    public class NoAttributeAllowedHereAnnotation : NodeAnnotation
    {
    }

    public class NotClassOrPrototypeReferenceAnnotation : NodeAnnotation
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

    public class UnsupportedArrayTypeAnnotation : NodeAnnotation
    {
        
    }
    
    
}