using System;
using System.Collections.Generic;


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
            referenceNode.BaseSymbol = symbol;
            if (symbol == null)
            {
                referenceNode.Annotations.Add(new UndeclaredIdentifierError(referenceNode.Name));
                return;
            }

            if (symbol.Node.Annotations.Count > 0)
            {
                return;
            }

            DeclarationNode declarationNode;
            
            declarationNode = (DeclarationNode) symbol.Node;
            declarationNode.Usages.Add(referenceNode);
            
            int attributeDepth = 0;
            bool arrayIndexNodeFound = false;
            ArrayIndexNode firstArrayIndexNode = null;

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


                        NestableSymbol resultNestableSymbol;
                        switch (symbol)
                        {
                            case InstanceSymbol instanceSymbol:
                                ClassSymbol baseClassSymbol = instanceSymbol.BaseClassSymbol;
                                if (baseClassSymbol != null)
                                {
                                    if (baseClassSymbol.BodySymbols.TryGetValue(attributeNode.Name.ToUpper(), out resultNestableSymbol))
                                    {
                                        symbol = resultNestableSymbol;
                                        symbolLocalPath = $"{symbolLocalPath}.{attributeNode.Name}";
                                        attributeNode.Symbol = symbol;
                                        attributeDepth++;
                                        
                                        declarationNode = (DeclarationNode) symbol.Node;
                                        declarationNode.Usages.Add(attributeNode);
                                        
                                    }
                                    else
                                    {
                                        attributeNode.Annotations.Add(new ClassDoesNotHaveAttributeError(symbolLocalPath, baseClassSymbol.Name, attributeNode.Name));
                                        return;
                                    }
                                }
                                else
                                {
                                    // TODO shouldn't we annotate something here?
                                }
                                break;
                            
                            case NestableSymbol nestableSymbol:
                                if (nestableSymbol.ComplexType is ClassSymbol classSymbol)
                                {
                                    if (classSymbol.BodySymbols.TryGetValue(attributeNode.Name.ToUpper(), out resultNestableSymbol))
                                    {
                                        symbol = resultNestableSymbol;
                                        symbolLocalPath = $"{symbolLocalPath}.{attributeNode.Name}";
                                        attributeNode.Symbol = symbol;
                                        attributeDepth++;
                                        
                                        declarationNode = (DeclarationNode) symbol.Node;
                                        declarationNode.Usages.Add(attributeNode);
                                    }
                                    else
                                    {
                                        attributeNode.Annotations.Add(new ClassDoesNotHaveAttributeError(symbolLocalPath, classSymbol.Name, attributeNode.Name)); //TODO does it ever occur?
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
                        firstArrayIndexNode = arrayIndexNode;
                        
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
            referenceNode.IndexNode = firstArrayIndexNode;
            if (attributeDepth > 1)
            {
                referenceNode.DoesHaveNestedAttributes = true;
            }
        }
        
        private Symbol GetBaseReferenceSymbol(ReferenceNode referenceNode)
        {
            ASTNode ancestor = referenceNode.GetFirstSignificantAncestorNode();
            string referenceNameUpper = referenceNode.Name.ToUpper();

            if (ancestor is InstanceDefinitionNode instanceDefinitionNode)
            {
                /*
                 TODO once transpiler is done, make only THIS to be available keywords since SELF makes name collision with self global object
                 */
                if (referenceNameUpper == "THIS" || referenceNameUpper == "SELF") 
                {
                    referenceNode.Name = instanceDefinitionNode.NameNode.Value;
                    referenceNameUpper = referenceNode.Name.ToUpper();
                    return _symbolTable[referenceNameUpper];
                }
            }
            
            
            NestableSymbol nestableSymbol;

            switch (ancestor)
            {
                case SubclassNode subclassNode:
                    
                    // look for local variable
                    SubclassSymbol subclassSymbol = (SubclassSymbol) subclassNode.Symbol;
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
                
                case ClassDefinitionNode _:
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