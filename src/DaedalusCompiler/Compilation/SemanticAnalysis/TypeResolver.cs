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
            SymbolType symbolBuiltinType = ((Symbol)typedSymbol).BuiltinType;
            ASTNode typedSymbolNode = ((Symbol) typedSymbol).Node;
            
            if (symbolBuiltinType == SymbolType.Uninitialized) // symbolType isn't one of the simple builtin types
            {
                CustomTypeDeclarationNode customTypeDeclarationNode = (CustomTypeDeclarationNode) typedSymbolNode;


                Symbol symbol = ReferenceResolvingVisitor.GetBaseReferenceSymbol(_symbolTable, customTypeDeclarationNode);
                /*
                // TODO use GetBaseReferenceSymbol if you want to differentiate between NotAClassError and UnknownTypeNameError
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
                        customTypeDeclarationNode.TypeNameNode.Annotations.Add(new UnknownTypeNameError(customTypeDeclarationNode.TypeNameNode.Value));
                        return;
                    }
                }
                else
                {
                    customTypeDeclarationNode.TypeNameNode.Annotations.Add(new UnknownTypeNameError(customTypeDeclarationNode.TypeNameNode.Value));
                    return;
                }
                */
            }
            
            
            switch (typedSymbol)
            {
                case FunctionSymbol _:
                    switch (symbolBuiltinType)
                    {
                        case SymbolType.Class:
                        case SymbolType.Prototype:
                        case SymbolType.Func:
                            typedSymbolNode.Annotations.Add(new UnsupportedFunctionTypeError());
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
                            typedSymbolNode.Annotations.Add(new UnsupportedArrayTypeError());
                            return;
                    }
                    break;
                case NestableSymbol _:
                    switch (symbolBuiltinType)
                    {
                        case SymbolType.Class:
                        case SymbolType.Prototype:
                        case SymbolType.Void:
                            typedSymbolNode.Annotations.Add(new UnsupportedTypeError());
                            return;
                    }
                    break;
            }
        }
    }
}