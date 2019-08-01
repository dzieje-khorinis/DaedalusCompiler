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
        private readonly Dictionary <string, SymbolContext> _symbolTable;
        
        private HashSet<InheritanceReferenceNode> _nodesVisitedThisIteration;
        private readonly HashSet<InheritanceReferenceNode> _nodesInCycle;
        private readonly Dictionary<ASTNode, ClassDefinitionNode> _node2ParentClassNode;
        
        public ReferenceResolver(Dictionary<string, SymbolContext> symbolTable)
        {
            _symbolTable = symbolTable;
            _nodesInCycle = new HashSet<InheritanceReferenceNode>();
            _node2ParentClassNode = new Dictionary<ASTNode, ClassDefinitionNode>();
        }
        
        public void Resolve(List<InheritanceReferenceNode> parentReferenceNodes)
        {
            foreach (var parentReferenceNode in parentReferenceNodes)
            {
                _nodesVisitedThisIteration = new HashSet<InheritanceReferenceNode>();
                Resolve(parentReferenceNode);
            }
        }

        public void Resolve(InheritanceReferenceNode inheritanceReferenceNode)
        {

            if (_nodesVisitedThisIteration.Contains(inheritanceReferenceNode))
            {
                inheritanceReferenceNode.Annotations.Add(new InfiniteReferenceLoopAnnotation());
                _nodesInCycle.UnionWith(_nodesVisitedThisIteration);
                return;
            }
            _nodesVisitedThisIteration.Add(inheritanceReferenceNode);
            
            if (inheritanceReferenceNode.Symbol != null)
            {
                return;
            }

            if (inheritanceReferenceNode.PartNodes.Count > 0)
            {
                inheritanceReferenceNode.Annotations.Add(new NotClassOrPrototypeReferenceAnnotation());
                return;
            }

            string path = inheritanceReferenceNode.Name.ToUpper();
            if (!_symbolTable.ContainsKey(path))
            {
                inheritanceReferenceNode.Annotations.Add(new UndeclaredIdentifierAnnotation());
                return;
            }

            DeclarationNode definitionNode = (DeclarationNode) _symbolTable[path].Node;
            switch (definitionNode)
            {
                case PrototypeDefinitionNode prototypeDefinitionNode:
                    //inheritanceReferenceNode.DefinitionNode = prototypeDefinitionNode;
                    Resolve(prototypeDefinitionNode.InheritanceReferenceNode);
                    break;
                case ClassDefinitionNode classDefinitionNode:
                    foreach (var node in _nodesVisitedThisIteration)
                    {
                        _node2ParentClassNode[node] = classDefinitionNode;
                    }
                    //inheritanceReferenceNode.DefinitionNode = classDefinitionNode;
                    break;
                default:
                    inheritanceReferenceNode.Annotations.Add(new NotClassOrPrototypeReferenceAnnotation());
                    return;
            }
            
            
            
            //inheritanceReferenceNode.Path = path;
            inheritanceReferenceNode.Symbol = _symbolTable[path].Symbol;

            ((DeclarationNode) inheritanceReferenceNode.ParentNode).Symbol.Parent = inheritanceReferenceNode.Symbol;
        }

        public void Resolve(List<ReferenceNode> referenceNodes)
        {
            foreach (var referenceNode in referenceNodes)
            {
                Resolve(referenceNode);
            }
        }

        private DatSymbol GetSymbol(string parentName, string symbolName)
        {
            string path =  $"{parentName}.{symbolName}".ToUpper();
            if (_symbolTable.ContainsKey(path))
            {
                return _symbolTable[path].Symbol;
            }

            return null;
        }

        public void Resolve(ReferenceNode referenceNode)
        {
            ASTNode ancestor = GetFirstSignificantAncestor(referenceNode);
            DatSymbol symbol = null;
            DatSymbol classSymbol = null;
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

                symbol = _symbolTable[nameUpper].Symbol;
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
                        if (symbol.BuiltinType != DatSymbolType.Instance)
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

        private DatSymbol GetClassSymbol(DatSymbol symbol)
        {
            symbol = symbol.Parent;
            while (symbol != null)
            {
                if (symbol.BuiltinType == DatSymbolType.Class)
                {
                    return symbol;
                }
                symbol = symbol.Parent;
            }
            return null;
        }

        private ClassDefinitionNode GetClassDefinitionNode(ASTNode node)
        {
            if (_node2ParentClassNode.ContainsKey(node))
            {
                return _node2ParentClassNode[node];
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