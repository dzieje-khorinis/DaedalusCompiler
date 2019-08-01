using System;
using System.Collections.Generic;
using System.Linq;
using DaedalusCompiler.Dat;

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


        public bool IsTypeAllowed(FunctionSymbol functionSymbol)
        {
            DatSymbolType symbolType = GetBuiltinType(functionSymbol.TypeName);
            return true;
        }
        
        public void Resolve(ITypedSymbol typedSymbol)
        {
            /*
            switch (typedSymbol.TypeName)
            {
                case "VOID":
                    break;
                
                case "FLOAT":
                    break;
                
                case "STRING":
                case "FUNC":
                    break;
                default:
                    ((Symbol)typedSymbol).Node.Annotations.Add(new UnsupportedArrayTypeAnnotation());
                    break;
            }
            break;
            */

            
            ASTNode node = (Symbol) typedSymbol;
            string capitalizedTypeName = typedSymbol.TypeName[0].ToUpper() + typedSymbol.TypeName.Substring(1).ToLower();
            if(Enum.TryParse(capitalizedTypeName, out DatSymbolType symbolType))
            {
                switch (symbolType)
                {
                    case DatSymbolType.Class:
                    case DatSymbolType.Prototype:
                        node.Annotations.Add(new UnsupportedTypeAnnotation());
                        break;
                        
                    case DatSymbolType.Instance:
                        node.Annotations.Add(new UnsupportedTypeAnnotation());
                        break;
                    
                    case DatSymbolType.Void:
                        if (!(typedSymbol is FunctionSymbol))
                        {
                            node.Annotations.Add(new UnsupportedTypeAnnotation());
                        }
                        break;
                }
            }
            
            
            
            /*
            switch (typedSymbol.TypeName)
            {
                 
            }
            */
            

            
            /*
             
             
             string capitalizedTypeName = typeName.First().ToString().ToUpper() + typeName.Substring(1).ToLower();
            if(Enum.TryParse(capitalizedTypeName, out DatSymbolType symbolType))
            {
                return symbolType;
            }
             
             */
            
            /*
            Void = 0, //tylko dla funkcji
            Float = 1, // dla funkcji i zmiennych
            Int = 2, // dla wszystkiego
            String = 3, //dla wszystkiego
            Class = 4, //dla niczego
            Func = 5, //tylko dla zmiennych i tablic
            Prototype = 6, //dla niczego
            Instance = 7, //dla funkcji i zmiennych
            Undefined = 8,
            */
            /*
             * 
             */
            
            
            
            switch (typedSymbol)
            {
                case IArraySymbol _:
                    switch (typedSymbol.TypeName.ToUpper())
                    {
                        case "INT":
                        case "STRING":
                        case "FUNC":
                            break;
                        default:
                            ((Symbol)typedSymbol).Node.Annotations.Add(new UnsupportedArrayTypeAnnotation());
                            break;
                    }
                    break;

            }
            
            
        }
        
                
                
        
        /*
        private bool IsArrayTypeSupported(string typeName) // TODO this can be done in TypeResolver
        {
            switch (typeName.ToUpper())
            {
                case "INT":
                case "STRING":
                case "FUNC":
                    return true;
                default:
                    return false;
            }
        }
        */
        
        
        private DatSymbolType? GetBuiltinType(string typeName)
        {
            string capitalizedTypeName = typeName.First().ToString().ToUpper() + typeName.Substring(1).ToLower();
            if(Enum.TryParse(capitalizedTypeName, out DatSymbolType symbolType))
            {
                return symbolType;
            }

            return null;
        }
        
    }
}