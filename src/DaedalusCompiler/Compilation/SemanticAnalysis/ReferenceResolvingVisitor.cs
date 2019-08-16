using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Security.Cryptography;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    public class ReferenceResolvingVisitor : AbstractSyntaxTreeBaseVisitor
    {
        private readonly Dictionary <string, Symbol> _symbolTable;
        public readonly List<ArrayIndexNode> ArrayIndexNodes;

        public ReferenceResolvingVisitor(Dictionary<string, Symbol> symbolTable)
        {
            _symbolTable = symbolTable;
            ArrayIndexNodes = new List<ArrayIndexNode>();
        }

        protected override void VisitReference(ReferenceNode referenceNode)
        {

            Symbol symbol = GetBaseReferenceSymbol(referenceNode);
            if (symbol == null)
            {
                referenceNode.Annotations.Add(new UndeclaredIdentifierError(referenceNode.Name));
                return;
            }

            if (symbol.Node.Annotations.Count > 0)
            {
                return;
            }

            bool arrayIndexNodeFound = false;
            
            foreach (ReferencePartNode partNode in referenceNode.PartNodes)
            {
                if (arrayIndexNodeFound)
                {
                    partNode.Annotations.Add(new AccessToAttributeOfArrayElementNotSupportedError());
                    return;
                }
                
                switch (partNode)
                {
                    case AttributeNode attributeNode:

                        switch (symbol)
                        {
                            case InstanceSymbol instanceSymbol:
                                ClassSymbol classSymbol = instanceSymbol.BaseClassSymbol;
                                if (classSymbol != null)
                                {
                                    if (classSymbol.BodySymbols.TryGetValue(attributeNode.Name.ToUpper(), out NestableSymbol nestableSymbol))
                                    {
                                        symbol = nestableSymbol;
                                    }
                                    else
                                    {
                                        // CLASS X doesn't have attribute Y
                                        attributeNode.Annotations.Add(new ClassDoesNotHaveAttributeError(symbol.Name, classSymbol.Name, attributeNode.Name));
                                        return;
                                    }
                                }
                                break;
                            default:
                                attributeNode.Annotations.Add(new AttributeOfNonInstanceError());
                                // error: cannot access attribute y of non instance object
                                // x.y
                                //   ^
                                return;
                        }
                        break;
                    
                    case ArrayIndexNode arrayIndexNode:
                        arrayIndexNodeFound = true;
                        
                        if (!(symbol.Node is IArrayDeclarationNode))
                        {
                            referenceNode.Annotations.Add(new ReferencedSymbolIsNotArrayError(referenceNode.Name));
                            return;
                        }
                        
                        ArrayIndexNodes.Add(arrayIndexNode);
                        break;
                    default:
                        throw new Exception();
                }
            }
            referenceNode.Symbol = symbol;
        }
        
        private Symbol GetBaseReferenceSymbol(ReferenceNode referenceNode)
        {
            ASTNode ancestor = referenceNode.GetFirstSignificantAncestor();
            string referenceNameUpper = referenceNode.Name.ToUpper();
            NestableSymbol nestableSymbol;

            switch (ancestor)
            {
                case SubclassNode subclassNode: // Instance/Prototype
                    
                    // look for local variable
                    SubclassSymbol subclassSymbol = (PrototypeSymbol) subclassNode.Symbol;
                    if (subclassSymbol.BodySymbols.TryGetValue(referenceNameUpper, out nestableSymbol))
                    {
                        return nestableSymbol;
                    }

                    // look for class variable
                    if (subclassSymbol.BaseClassSymbol != null)
                    {
                        var classSymbol = subclassSymbol.BaseClassSymbol;
                        if (classSymbol.BodySymbols.TryGetValue(referenceNameUpper, out nestableSymbol))
                        {
                            return nestableSymbol;
                        }
                    }
                    break;
                
                
                case FunctionDefinitionNode functionDefinitionNode:
                    // look for local variable
                    FunctionSymbol functionSymbol = (FunctionSymbol) functionDefinitionNode.Symbol;
                    if (functionSymbol.BodySymbols.TryGetValue(referenceNameUpper, out nestableSymbol))
                    {
                        return nestableSymbol;
                    }
                    break;
                case FileNode _:
                    break;
                
                default:
                    throw new Exception();
            }
            
            if (_symbolTable.ContainsKey(referenceNameUpper))
            {
                return _symbolTable[referenceNameUpper];
            }

            return null;
        }
    }
}