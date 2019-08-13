using System.Collections.Generic;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    public class TypeResolver
    {
        private readonly Dictionary <string, Symbol> _symbolTable;

        public TypeResolver(Dictionary<string, Symbol> symbolTable)
        {
            _symbolTable = symbolTable;
        }

        public void Resolve(List<ITypedSymbol> typedSymbols)
        {
            foreach (var typedSymbol in typedSymbols)
            {
                Resolve(typedSymbol);    
            }
        }
        
        public void Resolve(ITypedSymbol typedSymbol)
        {
            SymbolType symbolType = ((Symbol)typedSymbol).BuiltinType;
            ASTNode typedSymbolNode = ((Symbol) typedSymbol).Node;
            
            if (symbolType == SymbolType.Uninitialized)
            {
                if (_symbolTable.ContainsKey(typedSymbol.TypeName))
                {
                    Symbol typeSymbol = _symbolTable[typedSymbol.TypeName];
                    
                    if (typeSymbol is ClassSymbol)
                    {
                        typedSymbol.ComplexType = typeSymbol;
                        symbolType = SymbolType.Instance;    
                    }
                    else
                    {
                        typedSymbolNode.Annotations.Add(new UnsupportedTypeAnnotation());
                        return;
                    }
                }
                else
                {
                    typedSymbolNode.Annotations.Add(new UndefinedTypeAnnotation());
                    return;
                }
            }
            
            
            switch (typedSymbol)
            {
                case FunctionSymbol _:
                    switch (symbolType)
                    {
                        case SymbolType.Class:
                        case SymbolType.Prototype:
                        case SymbolType.Func:
                            typedSymbolNode.Annotations.Add(new UnsupportedTypeAnnotation());
                            return;
                    }

                    break;
                case IArraySymbol _:
                    switch (symbolType)
                    {
                        case SymbolType.Int:
                        case SymbolType.String:
                        case SymbolType.Func:
                            break;
                        default:
                            typedSymbolNode.Annotations.Add(new UnsupportedTypeAnnotation());
                            return;
                    }
                    break;
                case NestableSymbol _:
                    switch (symbolType)
                    {
                        case SymbolType.Class:
                        case SymbolType.Prototype:
                        case SymbolType.Void:
                            typedSymbolNode.Annotations.Add(new UnsupportedTypeAnnotation());
                            return;
                    }
                    break;
            }
        }
    }
}