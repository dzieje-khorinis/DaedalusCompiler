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
            Symbol symbol = (Symbol) typedSymbol;
            
            SymbolType symbolBuiltinType = symbol.BuiltinType;
            ASTNode typedSymbolNode = symbol.Node;
            
            CustomTypeDeclarationNode customTypeDeclarationNode = (CustomTypeDeclarationNode) typedSymbolNode;
            
            if (symbolBuiltinType == SymbolType.Uninitialized) // symbolType isn't one of the simple builtin types
            {
                // TODO think of: differentiate NotAClassError and UnknownTypeNameError
                if (_symbolTable.ContainsKey(typedSymbol.TypeName))
                {
                    Symbol typeSymbol = _symbolTable[typedSymbol.TypeName];
                    
                    if (typeSymbol is ClassSymbol)
                    {
                        typedSymbol.ComplexType = typeSymbol;
                        symbolBuiltinType = SymbolType.Instance;    
                    }
                    else
                    {
                        typedSymbolNode.Annotations.Add(new UnknownTypeNameError(customTypeDeclarationNode.TypeNameNode.Value, customTypeDeclarationNode.TypeNameNode.Location));
                        return;
                    }
                }
                else
                {
                    typedSymbolNode.Annotations.Add(new UnknownTypeNameError(customTypeDeclarationNode.TypeNameNode.Value, customTypeDeclarationNode.TypeNameNode.Location));
                    return;
                }

            }

            
            switch (typedSymbol)
            {
                case FunctionSymbol _:
                    //FunctionDefinitionNode functionDefinitionNode = (FunctionDefinitionNode) typedSymbolNode;
                    
                    switch (symbolBuiltinType)
                    {
                        case SymbolType.Class:
                        case SymbolType.Prototype:
                        case SymbolType.Func:
                            symbol.BuiltinType = SymbolType.Uninitialized;
                            typedSymbolNode.Annotations.Add(new UnsupportedFunctionTypeError(customTypeDeclarationNode.TypeNameNode.Location));
                            return;
                    }

                    break;
                case IArraySymbol _:
                    switch (symbolBuiltinType)
                    {
                        case SymbolType.Int:
                        case SymbolType.String:
                        case SymbolType.Func:
                            break;
                        default:
                            symbol.BuiltinType = SymbolType.Uninitialized;
                            typedSymbolNode.Annotations.Add(new UnsupportedArrayTypeError(customTypeDeclarationNode.TypeNameNode.Location));
                            return;
                    }
                    break;
                case NestableSymbol _:
                    switch (symbolBuiltinType)
                    {
                        case SymbolType.Class:
                        case SymbolType.Prototype:
                        case SymbolType.Void:
                            symbol.BuiltinType = SymbolType.Uninitialized;
                            typedSymbolNode.Annotations.Add(new UnsupportedTypeError(customTypeDeclarationNode.TypeNameNode.Location));
                            return;
                    }
                    break;
            }
        }
    }
}