using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Security.Cryptography;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    public class ReferenceResolver
    {
        private readonly Dictionary <string, Symbol> _symbolTable;

        private HashSet<InheritanceParentReferenceNode> _visitedNodes;
        private HashSet<InheritanceParentReferenceNode> _visitedNodesCurrentIteration;
        private readonly HashSet<InheritanceParentReferenceNode> _nodesInCycle;

        public ReferenceResolver(Dictionary<string, Symbol> symbolTable)
        {
            _symbolTable = symbolTable;
            _visitedNodes = new HashSet<InheritanceParentReferenceNode>();
            _nodesInCycle = new HashSet<InheritanceParentReferenceNode>();
        }
        
        public void Resolve(List<InheritanceParentReferenceNode> parentReferenceNodes)
        {
            foreach (var parentReferenceNode in parentReferenceNodes)
            {
                _visitedNodesCurrentIteration = new HashSet<InheritanceParentReferenceNode>();
                Resolve(parentReferenceNode);
            }
        }

        public void Resolve(InheritanceParentReferenceNode inheritanceParentReferenceNode)
        {
            if (_visitedNodesCurrentIteration.Contains(inheritanceParentReferenceNode))
            {
                inheritanceParentReferenceNode.Annotations.Add(new InfiniteReferenceLoopAnnotation());
                _nodesInCycle.UnionWith(_visitedNodesCurrentIteration);
                return;
            }
            
            if (_visitedNodes.Contains(inheritanceParentReferenceNode))
            {
                return;
            }

            if (inheritanceParentReferenceNode.PartNodes.Count > 0)
            {
                inheritanceParentReferenceNode.Annotations.Add(new NotClassOrPrototypeReferenceAnnotation());
                return;
            }
            
            string path = inheritanceParentReferenceNode.Name.ToUpper();
            if (!_symbolTable.ContainsKey(path))
            {
                inheritanceParentReferenceNode.Annotations.Add(new UndeclaredIdentifierAnnotation());
                return;
            }

            SubclassSymbol symbol = (SubclassSymbol) _symbolTable[path];
            switch (symbol.Node)
            {
                case PrototypeDefinitionNode prototypeDefinitionNode:
                    PrototypeSymbol prototypeSymbol = (PrototypeSymbol) prototypeDefinitionNode.Symbol;
                    symbol.InheritanceParentSymbol = prototypeSymbol;
                    Resolve(prototypeDefinitionNode.InheritanceParentReferenceNode);
                    break;
                case ClassDefinitionNode classDefinitionNode:
                    ClassSymbol classSymbol = (ClassSymbol) classDefinitionNode.Symbol;
                    symbol.BaseClassSymbol = classSymbol;
                    symbol.InheritanceParentSymbol = classSymbol;

                    foreach (var node in _visitedNodesCurrentIteration)
                    {
                        SubclassSymbol subclassSymbol = (SubclassSymbol) node.Symbol;
                        subclassSymbol.BaseClassSymbol = classSymbol;
                    }
                    
                    break;
                default:
                    inheritanceParentReferenceNode.Annotations.Add(new NotClassOrPrototypeReferenceAnnotation());
                    return;
            }

            inheritanceParentReferenceNode.Symbol = symbol;
            _visitedNodesCurrentIteration.Add(inheritanceParentReferenceNode);
            _visitedNodes.Add(inheritanceParentReferenceNode);
            ((DeclarationNode) inheritanceParentReferenceNode.ParentNode).Symbol.Parent = inheritanceParentReferenceNode.Symbol;
        }

        public void Resolve(List<ReferenceNode> referenceNodes)
        {
            foreach (var referenceNode in referenceNodes)
            {
                Resolve(referenceNode);
            }
        }

        private Symbol GetSymbol(string parentName, string symbolName)
        {
            string path =  $"{parentName}.{symbolName}".ToUpper();
            if (_symbolTable.ContainsKey(path))
            {
                return _symbolTable[path];
            }

            return null;
        }

        public void Resolve(ReferenceNode referenceNode)
        {
            ASTNode ancestor = GetFirstSignificantAncestor(referenceNode);
            Symbol symbol = null;
            Symbol classSymbol = null;
            ClassDefinitionNode classDefinitionNode;
            
            switch (ancestor)
            {
                case PrototypeDefinitionNode prototypeDefinitionNode:
                    //look for local variable
                    symbol = GetSymbol(prototypeDefinitionNode.NameNode.Value, referenceNode.Name);
                    if (symbol != null)
                    {
                        break;
                    }

                    // look for class variable
                    classDefinitionNode = GetClassDefinitionNode(prototypeDefinitionNode);
                    if (classDefinitionNode == null)
                    {
                        // TODO think about adding UndeclatedIdentifierAnnotation
                        return;
                    }
                    symbol = GetSymbol(classDefinitionNode.NameNode.Value, referenceNode.Name);
                    break;

                    
                case InstanceDefinitionNode instanceDefinitionNode:
                    //look for local variable
                    symbol = GetSymbol(instanceDefinitionNode.NameNode.Value, referenceNode.Name);
                    if (symbol != null)
                    {
                        return;
                    }

                    // look for class variable
                    classDefinitionNode = GetClassDefinitionNode(instanceDefinitionNode);
                    if (classDefinitionNode == null)
                    {
                        // TODO think about adding UndeclatedIdentifierAnnotation
                        return;
                    }
                    symbol = GetSymbol(classDefinitionNode.NameNode.Value, referenceNode.Name);
                    break;
                
                case FunctionDefinitionNode functionDefinitionNode:
                    // check local
                    symbol = GetSymbol(functionDefinitionNode.NameNode.Value, referenceNode.Name);
                    break;
                case FileNode _:
                    break;
                
                default:
                    throw new Exception();
            }

            if (symbol == null)
            {
                string nameUpper = referenceNode.Name.ToUpper();
                if (!_symbolTable.ContainsKey(nameUpper))
                {
                    referenceNode.Annotations.Add(new UndeclaredIdentifierAnnotation());
                    return;
                }

                symbol = _symbolTable[nameUpper];
            }
            
            bool arrayIndexNodeFound = false;
            
            foreach (ReferencePartNode partNode in referenceNode.PartNodes)
            {
                if (arrayIndexNodeFound)
                {
                    partNode.Annotations.Add(new AccessToAttributeOfArrayElementNotSupportedAnnotation());
                    return;
                }
                
                switch (partNode)
                {
                    case AttributeNode attributeNode:
                        classSymbol = GetClassSymbol(symbol);

                        if (classSymbol == null)
                        {
                            attributeNode.Annotations.Add(new AttributeOfNonInstanceAnnotation());
                            // error: cannot access attribute y of non instance object
                            // x.y
                            //   ^
                            return;
                        }
                        
                        /*
                        if (symbol.BuiltinType != SymbolType.Instance)
                        {
                            attributeNode.Annotations.Add(new NotInstanceAnnotation());
                            // error: cannot access attribute of x, x is not an instance of a clasds
                            // x.y
                            // ^ is not an instance of a class
                            return;
                        }
                        */
                        
                        symbol = GetSymbol(classSymbol.Name, attributeNode.Name);
                        if (symbol == null) {
                            // CLASS X doesn't have attribute Y
                            attributeNode.Annotations.Add(new ClassDoesNotHaveAttributeAnnotation(classSymbol.Name));
                            return;
                        }


                        break;
                    case ArrayIndexNode _:
                        arrayIndexNodeFound = true;
                        
                        if (!(_symbolTable[symbol.Name].Node is IArrayDeclarationNode))
                        {
                            referenceNode.Annotations.Add(new ReferencedSymbolIsNotArrayAnnotation());
                            return;
                        }
                        break;
                    default:
                        throw new Exception();
                }
            }


            referenceNode.Symbol = symbol;
        }

        private Symbol GetClassSymbol(Symbol symbol)
        {
            symbol = symbol.Parent;
            while (symbol != null)
            {
                if (symbol.BuiltinType == SymbolType.Class)
                {
                    return symbol;
                }
                symbol = symbol.Parent;
            }
            return null;
        }
        

        private ASTNode GetFirstSignificantAncestor(ASTNode node)
        {
            while (node != null)
            {
                node = node.ParentNode;
                switch (node)
                {
                    case PrototypeDefinitionNode prototypeDefinitionNode:
                        return prototypeDefinitionNode;
                    case InstanceDefinitionNode instanceDefinitionNode:
                        return instanceDefinitionNode;
                    case FunctionDefinitionNode functionDefinitionNode:
                        return functionDefinitionNode;
                    case FileNode fileNode:
                        return fileNode;
                }
            }
            throw new Exception();
        }
        
        
    }
}