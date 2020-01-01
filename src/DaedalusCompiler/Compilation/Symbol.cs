using System;
using System.Collections.Generic;
using System.Linq;
using DaedalusCompiler.Compilation.SemanticAnalysis;

namespace DaedalusCompiler.Compilation
{
    public enum SymbolType
    {
        Uninitialized = -1,
        Void = 0,
        Float = 1,
        Int = 2,
        String = 3,
        Class = 4,
        Func = 5,
        Prototype = 6,
        Instance = 7,
        Undefined = 8,
    }
    
    [Flags]
    public enum SymbolFlag
    {
        Const = 1,
        Return = 2,
        ClassVar = 4,
        External = 8,
        Merged = 16,
    }

    
    public abstract class Symbol
    {
        public int Index;
        public readonly int SubIndex; // only used by prefix attributes
        public string Name;
        public string Path;
        public readonly ASTNode Node;

        public SymbolType BuiltinType;
        public SymbolFlag Flags;
        
        public object[] Content;
        public bool IsExternal;

        protected Symbol(string name, ASTNode node)
        {
            Index = -1;
            SubIndex = -1;
            Name = name;
            Path = name.ToUpper();
            Node = node;
            BuiltinType = SymbolType.Uninitialized;

            Content = new object[]{};
            IsExternal = false;
        }

        protected static SymbolType GetBuiltinType(string typeName)
        {
            string capitalizedTypeName = typeName.First().ToString().ToUpper() + typeName.Substring(1).ToLower();
            if(Enum.TryParse(capitalizedTypeName, out SymbolType symbolType))
            {
                return symbolType;
            }

            return SymbolType.Uninitialized;
        }
    }
    
    
    public abstract class BlockSymbol : Symbol
    {
        public readonly Dictionary<string, NestableSymbol> BodySymbols;
        public readonly List<AssemblyElement> Instructions;
        
        public int FirstTokenAddress;
        public readonly Dictionary<string, int> Label2Address;
        
        protected BlockSymbol(string name, ASTNode node) : base(name, node)
        {
            BodySymbols = new Dictionary<string, NestableSymbol>();
            Instructions = new List<AssemblyElement>();
            
            FirstTokenAddress = -1;
            Label2Address = new Dictionary<string, int>();
        }

        public void AddBodySymbol(NestableSymbol nestableSymbol)
        {
            string name = nestableSymbol.Name.ToUpper();
            if (!BodySymbols.ContainsKey(name))
            {
                BodySymbols[name] = nestableSymbol;
            }
            else
            {
                throw new Exception();
            }
        }
    }
    
    public abstract class NestableSymbol : Symbol, ITypedSymbol
    {
        public readonly BlockSymbol ParentBlockSymbol;
        public string TypeName { get; set; }
        public Symbol ComplexType { get; set; }

        protected NestableSymbol(BlockSymbol parentBlockSymbol, string typeName, string name, ASTNode node) : base(name, node)
        {
            ParentBlockSymbol = parentBlockSymbol;
            TypeName = typeName.ToUpper();
            BuiltinType = GetBuiltinType(TypeName);
            ComplexType = null;
            if (parentBlockSymbol != null)
            {
                Path = $"{parentBlockSymbol.Name}.{name}".ToUpper();
                if (!parentBlockSymbol.BodySymbols.ContainsKey(name.ToUpper()))
                {
                    parentBlockSymbol.AddBodySymbol(this);
                }
            }
        }
    }

    public abstract class InheritanceSymbol : BlockSymbol
    {
        protected InheritanceSymbol(string name, ASTNode node) : base(name, node)
        {
        }
    }

    public abstract class SubclassSymbol : InheritanceSymbol
    {
        public InheritanceSymbol InheritanceParentSymbol;
        public ClassSymbol BaseClassSymbol;

        protected SubclassSymbol(string name, ASTNode node) : base(name, node)
        {
        }
    }
    
    
    public class FunctionSymbol : BlockSymbol, ITypedSymbol
    {
        public string TypeName { get; set; }
        public Symbol ComplexType { get; set; }
        public int ParametersCount { get; set; }

        public FunctionSymbol(string typeName, string name, bool isExternal, ASTNode node) : base(name, node)
        {
            TypeName = typeName.ToUpper();
            IsExternal = isExternal;
            BuiltinType = GetBuiltinType(TypeName);
            ComplexType = null;
        }
    }

    public interface IArraySymbol
    {
        int Size { get; set; }
    }

    public interface ITypedSymbol
    {
        string TypeName { get; set; }
        Symbol ComplexType { get; set; }
    }

    public interface IAttributeSymbol
    {
        int Offset { get; set; }
    }
    
    public class ClassSymbol : InheritanceSymbol
    {
        public int Size { get; set; }
        public int Offset { get; set; }
        
        public int AttributesCount { get; set; }
        public ClassSymbol(string name, ASTNode node) : base(name, node)
        {
            BuiltinType = SymbolType.Class;
        }
    }

    public class PrototypeSymbol : SubclassSymbol
    {
        public PrototypeSymbol(string name, ASTNode node) : base(name, node)
        {
            BuiltinType = SymbolType.Prototype;
        }
    }

    public class InstanceSymbol : SubclassSymbol
    {
        public InstanceSymbol(string name, ASTNode node) : base(name, node)
        {
            BuiltinType = SymbolType.Instance;
        }
    }

    public class VarSymbol : NestableSymbol
    {
        public VarSymbol(BlockSymbol parentBlockSymbol, string typeName, string name, ASTNode node) : base(parentBlockSymbol, typeName, name, node)
        {
        }
    }
    
    public class VarArraySymbol : VarSymbol, IArraySymbol
    {
        public int Size { get; set; }

        public VarArraySymbol(BlockSymbol parentBlockSymbol, string typeName, string name, ASTNode node) : base(parentBlockSymbol, typeName, name, node)
        {
        }
    }

    public class AttributeSymbol : VarSymbol, IAttributeSymbol
    {
        public int Offset { get; set; }
        
        public AttributeSymbol(BlockSymbol parentBlockSymbol, string typeName, string name, ASTNode node) : base(parentBlockSymbol, typeName, name, node)
        {
        }
    }
    
    public class AttributeArraySymbol : AttributeSymbol, IArraySymbol
    {
        public int Size { get; set; }

        public AttributeArraySymbol(BlockSymbol parentBlockSymbol, string typeName, string name, ASTNode node) : base(parentBlockSymbol, typeName, name, node)
        {
        }
    }
    
    public class ConstSymbol : NestableSymbol
    {
        public ConstSymbol(BlockSymbol parentBlockSymbol, string typeName, string name, ASTNode node) : base(parentBlockSymbol, typeName, name, node)
        {
        }
    }
    
    public class ConstArraySymbol : ConstSymbol, IArraySymbol
    {
        public int Size { get; set; }
        
        

        public ConstArraySymbol(BlockSymbol parentBlockSymbol, string typeName, string name, ASTNode node) : base(parentBlockSymbol, typeName, name, node)
        {
        }
    }
    
    public class ParameterSymbol : NestableSymbol
    {
        public ParameterSymbol(BlockSymbol parentBlockSymbol, string typeName, string name, ASTNode node) : base(parentBlockSymbol, typeName, name, node)
        {
        }
    }
    
    public class ParameterArraySymbol : ParameterSymbol, IArraySymbol
    {
        public int Size { get; set; }


        public ParameterArraySymbol(BlockSymbol parentBlockSymbol, string typeName, string name, ASTNode node) : base(parentBlockSymbol, typeName, name, node)
        {
        }
    }
    
    public class ExternalParameterSymbol : ParameterSymbol
    {
        public ExternalParameterSymbol(BlockSymbol parentBlockSymbol, string typeName, string name, ASTNode node) : base(parentBlockSymbol, typeName, name, node)
        {
        }
    }
    
    public class ExternalParameterArraySymbol : ExternalParameterSymbol, IArraySymbol
    {
        public int Size { get; set; }


        public ExternalParameterArraySymbol(BlockSymbol parentBlockSymbol, string typeName, string name, ASTNode node) : base(parentBlockSymbol, typeName, name, node)
        {
        }
    }
    
    public class StringConstSymbol : Symbol
    {
        public StringConstSymbol(string content, string name, ASTNode node) : base(name, node)
        {
            Content = new object[]{content};
            BuiltinType = SymbolType.String;
            Path = name;
        }
    }
}