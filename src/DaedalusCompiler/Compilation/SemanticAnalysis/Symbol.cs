using System;
using System.Collections.Generic;
using System.Linq;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    /*
    public class SymbolTree
    {
        public List<Symbol> Symbols;

        public SymbolTree()
        {
            Symbols = new List<Symbol>();
        }
    }
    */

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
    }

    public abstract class Symbol
    {
        public int Index;
        public string Name;
        public string Path;
        public ASTNode Node;

        public SymbolType? BuiltinType;

        protected Symbol(string name, ASTNode node)
        {
            Index = -1;
            Name = name;
            Path = name.ToUpper();
            Node = node;
            BuiltinType = null;
        }
        
        public static SymbolType? GetBuiltinType(string typeName)
        {
            string capitalizedTypeName = typeName.First().ToString().ToUpper() + typeName.Substring(1).ToLower();
            if(Enum.TryParse(capitalizedTypeName, out SymbolType symbolType))
            {
                return symbolType;
            }

            return null;
        }
    }
    
    
    public abstract class BlockSymbol : Symbol
    {
        public readonly Dictionary<string, NestableSymbol> BodySymbols;

        protected BlockSymbol(string name, ASTNode node) : base(name, node)
        {
            BodySymbols = new Dictionary<string, NestableSymbol>();
        }

        public void AddBodySymbol(NestableSymbol nestableSymbol)
        {
            string name = nestableSymbol.Name.ToUpper();
            if (!BodySymbols.ContainsKey(name))
            {
                BodySymbols[name] = nestableSymbol;
            }
        }

        public NestableSymbol GetBodySymbolByName(string name)
        {
            name = name.ToUpper();
            if (BodySymbols.ContainsKey(name))
            {
                return BodySymbols[name.ToUpper()];
            }

            return null;
        }
    }
    
    public abstract class NestableSymbol : Symbol, ITypedSymbol
    {
        public BlockSymbol ParentBlockSymbol;
        public string TypeName { get; set; }
        //public SymbolType? BuiltinType { get; set; }
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
                parentBlockSymbol.AddBodySymbol(this);
            }
        }
    }

    public abstract class InheritanceSymbol : BlockSymbol
    {
        protected InheritanceSymbol(string name, ASTNode node) : base(name, node)
        {
        }
    }

    public abstract class SubclassSymbol : InheritanceSymbol //Prototype / Instance
    {
        public InheritanceSymbol InheritanceParentSymbol;
        public ClassSymbol BaseClassSymbol;

        protected SubclassSymbol(string name, ASTNode node) : base(name, node)
        {
        }
    }
    
    
    public class FunctionSymbol : BlockSymbol, ITypedSymbol
    {
        //public List<ParameterSymbol> ParametersSymbols;

        public string TypeName { get; set; }
        public SymbolType? BuiltinType { get; set; }
        public Symbol ComplexType { get; set; }

        public FunctionSymbol(string typeName, string name, ASTNode node) : base(name, node)
        {
            TypeName = typeName.ToUpper();
            BuiltinType = GetBuiltinType(TypeName);
            ComplexType = null;
        }

        /*
        public void AddParameterSymbol(ParameterSymbol parameterSymbol)
        {
            ParametersSymbols.Add(parameterSymbol);
            AddBodySymbol(parameterSymbol);
        }
        */
    }

    public interface IArraySymbol
    {
        int Size { get; set; }
    }

    public interface ITypedSymbol
    {
        string TypeName { get; set; }
        //SymbolType? BuiltinType { get; set; }
        
        Symbol ComplexType { get; set; }
    }
    
    public class ClassSymbol : InheritanceSymbol
    {
        public ClassSymbol(string name, ASTNode node) : base(name, node)
        {
        }
    }

    public class PrototypeSymbol : SubclassSymbol
    {
        public PrototypeSymbol(string name, ASTNode node) : base(name, node)
        {
        }
    }

    public class InstanceSymbol : SubclassSymbol
    {
        public InstanceSymbol(string name, ASTNode node) : base(name, node)
        {
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
        public string Content;
        public StringConstSymbol(string content, string name, ASTNode node) : base(name, node)
        {
            Content = content;
        }
    }
}