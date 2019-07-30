using System.Collections.Generic;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    public class SymbolTree
    {
        public List<Symbol> Symbols;

        public SymbolTree()
        {
            Symbols = new List<Symbol>();
        }
    }


    public interface ILocalSymbol
    {
        Symbol Parent { get; set; }
    }
    
    
    public abstract class Symbol
    {
        public ASTNode Node;
    }
    
    
    
    public class FunctionSymbol : Symbol
    {
        public List<ParameterSymbol> ParametersSymbols;
        public List<ILocalSymbol> VarsSymbols;
    }
    
        
    public class ClassSymbol : Symbol
    {
        public List<VarSymbol> Attributes;
    }
    
    
    
    public class PrototypeSymbol : Symbol
    {
        public Symbol Parent;
    }

    public class InstanceSymbol : Symbol
    {
        
    }
    

    public class StringConstSymbol : Symbol
    {
        
    }
    
    
    public class VarSymbol : Symbol
    {
        
    }

    public class ConstSymbol : Symbol
    {

    }
    
    
    
    public class ParameterSymbol : Symbol
    {
        public bool IsExternal;
    }



    public class ClassVarSymbol : Symbol
    {
        
    }
    
    public class ClassConstSymbol : Symbol
    {
        // TODO is this even ever used?
    }
}