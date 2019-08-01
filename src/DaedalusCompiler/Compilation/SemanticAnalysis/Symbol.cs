using System.Collections.Generic;

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


    public abstract class Symbol
    {
        public ASTNode Node;
    }
    
    
    public abstract class BlockSymbol : Symbol
    {
        public List<NestableSymbol> BodySymbols;
    }
    
    public abstract class NestableSymbol : Symbol
    {
        public BlockSymbol ParentBlockSymbol;
        public string TypeName;
    }

    public abstract class InheritanceSymbol : Symbol
    {
    }

    public abstract class SubclassSymbol : InheritanceSymbol //Prototype / Instance
    {
        public InheritanceSymbol InheritanceParentSymbol;
        public ClassSymbol BaseClassSymbol;
    }
    
    
    public class FunctionSymbol : Symbol
    {
        public List<ParameterSymbol> ParametersSymbols;
        
    }

    public interface IArraySymbol
    {
        int Size { get; set; }
    }
    
    public class ClassSymbol : InheritanceSymbol
    {
        public List<NestableSymbol> AttributesSymbols;
    }

    public class PrototypeSymbol : SubclassSymbol
    {
    }

    public class InstanceSymbol : SubclassSymbol
    {
    }

    public class VarSymbol : NestableSymbol
    {
    }
    
    public class VarArraySymbol : VarSymbol, IArraySymbol
    {
        public int Size { get; set; }
    }

    public class ConstSymbol : NestableSymbol
    {
    }
    
    public class ConstArraySymbol : ConstSymbol, IArraySymbol
    {
        public int Size { get; set; }
    }
    
    public class ParameterSymbol : Symbol
    {
    }
    
    public class ParameterArraySymbol : ParameterSymbol, IArraySymbol
    {
        public int Size { get; set; }
    }
    
    public class ExternalParameterSymbol : ParameterSymbol
    {
    }
    
    public class ExternalParameterArraySymbol : ExternalParameterSymbol, IArraySymbol
    {
        public int Size { get; set; }
    }
    
    public class StringConstSymbol : Symbol
    {
    }
}