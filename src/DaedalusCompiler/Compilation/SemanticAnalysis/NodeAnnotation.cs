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

    public class UndeclaredIdentifierAnnotation : ErrorAnnotation
    {
        private readonly string _identifier;

        public UndeclaredIdentifierAnnotation(string identifier)
        {
            _identifier = identifier;
        }

        public override string GetMessage()
        {
            return $"‘{_identifier}’ undeclared";
        }
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
            return $"redefinition of ‘{_identifier}’";
        }

        public override string GetNote()
        {
            return "previous definition is here";
        }
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


    public class InconsistentSizeAnnotation : ErrorAnnotation
    {
        private readonly string _identifier;
        private readonly int _declaredSize;
        private readonly int _elementsCount;

        public InconsistentSizeAnnotation(string identifier, int declaredSize, int elementsCount)
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
    
    public class UnsupportedArrayTypeAnnotation : NodeAnnotation
    {
        
    }

    public class UnsupportedTypeAnnotation : NodeAnnotation
    {
        
    }
    
    public class UndefinedTypeAnnotation : NodeAnnotation
    {
        
    }


    public class ClassDoesNotHaveAttributeAnnotation : ErrorAnnotation
    {
        private readonly string _objectName;
        private readonly string _className;
        private readonly string _attributeName;

        public ClassDoesNotHaveAttributeAnnotation(string objectName, string className, string attributeName)
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