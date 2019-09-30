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
            
            if (referenceNode.Name != symbol.Name)
            {
                DeclarationNode declarationNode = (DeclarationNode) symbol.Node;
                referenceNode.Annotations.Add(new NamesNotMatchingCaseWiseWarning(declarationNode.NameNode.Location, symbol.Name, referenceNode.Name));
            }

            bool arrayIndexNodeFound = false;


            string symbolLocalPath = referenceNode.Name;
            
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


                        NestableSymbol nestableSymbol;
                        switch (symbol)
                        {
                            case InstanceSymbol instanceSymbol:
                                ClassSymbol baseClassSymbol = instanceSymbol.BaseClassSymbol;
                                if (baseClassSymbol != null)
                                {
                                    if (baseClassSymbol.BodySymbols.TryGetValue(attributeNode.Name.ToUpper(), out nestableSymbol))
                                    {
                                        symbol = nestableSymbol;
                                        symbolLocalPath = $"{symbolLocalPath}.{attributeNode.Name}";
                                        
                                        if (attributeNode.Name != symbol.Name)
                                        {
                                            DeclarationNode declarationNode = (DeclarationNode) symbol.Node;
                                            attributeNode.Annotations.Add(new NamesNotMatchingCaseWiseWarning(declarationNode.NameNode.Location, symbol.Name, attributeNode.Name));
                                        }
                                    }
                                    else
                                    {
                                        attributeNode.Annotations.Add(new ClassDoesNotHaveAttributeError(symbolLocalPath, baseClassSymbol.Name, attributeNode.Name));
                                        return;
                                    }
                                }
                                break;
                            
                            case VarSymbol varSymbol:
                                if (varSymbol.ComplexType is ClassSymbol classSymbol)
                                {
                                    if (classSymbol.BodySymbols.TryGetValue(attributeNode.Name.ToUpper(), out nestableSymbol))
                                    {
                                        symbol = nestableSymbol;
                                        symbolLocalPath = $"{symbolLocalPath}.{attributeNode.Name}";
                                        
                                        if (attributeNode.Name != symbol.Name)
                                        {
                                            DeclarationNode declarationNode = (DeclarationNode) symbol.Node;
                                            attributeNode.Annotations.Add(new NamesNotMatchingCaseWiseWarning(declarationNode.NameNode.Location, symbol.Name, attributeNode.Name));
                                        }
                                    }
                                    else
                                    {
                                        attributeNode.Annotations.Add(new ClassDoesNotHaveAttributeError(symbolLocalPath, classSymbol.Name, attributeNode.Name)); //TODO test
                                        return;
                                    }
                                }
                                else
                                {
                                    attributeNode.Annotations.Add(new AttributeOfNonInstanceError(attributeNode.Name, symbolLocalPath));
                                }
                                break;
                            
                            
                            default:
                                attributeNode.Annotations.Add(new AttributeOfNonInstanceError(attributeNode.Name, symbolLocalPath));
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
            
            //NamesNotMatchingCaseWiseWarning
            //referenceNode.Name
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