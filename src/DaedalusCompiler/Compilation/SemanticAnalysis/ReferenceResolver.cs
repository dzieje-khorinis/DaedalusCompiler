using System;
using System.Collections.Generic;
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
        
        public ReferenceResolver(Dictionary<string, SymbolContext> symbolTable)
        {
            _symbolTable = symbolTable;
            _nodesInCycle = new HashSet<InheritanceReferenceNode>();
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

            ASTNode node = _symbolTable[path].Node;
            switch (node)
            {
                case PrototypeDefinitionNode prototypeDefinitionNode:
                    //inheritanceReferenceNode.DefinitionNode = prototypeDefinitionNode;
                    Resolve(prototypeDefinitionNode.InheritanceReferenceNode);
                    break;
                case ClassDefinitionNode classDefinitionNode:
                    //inheritanceReferenceNode.DefinitionNode = classDefinitionNode;
                    break;
                default:
                    inheritanceReferenceNode.Annotations.Add(new NotClassOrPrototypeReferenceAnnotation());
                    return;
            }
            
            
            
            inheritanceReferenceNode.Path = path;
            inheritanceReferenceNode.Symbol = _symbolTable[path].Symbol;
        }

        public void Resolve(List<ReferenceNode> referenceNodes)
        {
            foreach (var referenceNode in referenceNodes)
            {
                Resolve(referenceNode);
            }
        }

        private DatSymbol GetLocalSymbol(string parentBlockName, string symbolName)
        {
            string path =  $"{parentBlockName}.{symbolName}".ToUpper();
            if (_symbolTable.ContainsKey(path))
            {
                return _symbolTable[path].Symbol;
            }

            return null;
        }

        public void Resolve(ReferenceNode referenceNode)
        {
            ASTNode ancestor = GetFirstSignificantAncestor(referenceNode);
            string path;
            DatSymbol symbol;
            
            switch (ancestor)
            {
                case PrototypeDefinitionNode prototypeDefinitionNode:
                    symbol = GetLocalSymbol(prototypeDefinitionNode.NameNode.Value, referenceNode.Name);
                    if (symbol != null)
                    {
                        return;
                    }

                    ClassDefinitionNode classDefinitionNode = GetClassDefinitionNode(prototypeDefinitionNode.InheritanceReferenceNode.Name);
                    if (classDefinitionNode == null)
                    {
                        // TODO think about adding UndeclatedIdentifierAnnotation
                        return;
                    }

                    symbol = GetLocalSymbol(classDefinitionNode.NameNode.Value, referenceNode.Name);
                    if (symbol != null)
                    {
                        return;
                    }

                    
                    
                    /*
                    path = $"{prototypeDefinitionNode.NameNode.Value.ToUpper()}.{referenceNode.Name}";
                    if (_symbolTable.ContainsKey(path))
                    {
                        referenceNode.Path = path;
                        referenceNode.Symbol = _symbolTable[referenceNode.Path].Symbol;
                        return;
                    }
                    */
                    
                    //check classvar
                    
                    //check global
                    
                    break;
                case InstanceDefinitionNode instanceDefinitionNode:
                    //check local
                    path = $"{instanceDefinitionNode.NameNode.Value.ToUpper()}.{referenceNode.Name}";
                    if (_symbolTable.ContainsKey(path))
                    {
                        referenceNode.Path = path;
                        referenceNode.Symbol = _symbolTable[referenceNode.Path].Symbol;
                        return;
                    }
                    
                    //check classvar
                    
                    //check global
                    break;
                case FunctionDefinitionNode functionDefinitionNode:
                    // check local
                    path = $"{functionDefinitionNode.NameNode.Value.ToUpper()}.{referenceNode.Name}";
                    if (_symbolTable.ContainsKey(path))
                    {
                        referenceNode.Path = path;
                        referenceNode.Symbol = _symbolTable[referenceNode.Path].Symbol;
                        return;
                    }
                    
                    // check global
                    path = referenceNode.Name.ToUpper();
                    
                    
                    break;
                case FileNode _:
                    // check global
                    path = referenceNode.Name.ToUpper();
                    if (!_symbolTable.ContainsKey(path))
                    {
                        referenceNode.Annotations.Add(new UndeclaredIdentifierAnnotation());
                    }
                    else
                    {
                        referenceNode.Symbol = _symbolTable[path].Symbol;
                        referenceNode.Path = path;
                    }
                    break;
            }
            
            /*
if (referenceNode.ArrayIndexNode != null)
{
    if (referenceNode.AttributeNode != null)
    {
        referenceNode.AttributeNode.Annotations.Add(new AccessToAttributeOfArrayElementNotSupportedAnnotation());
        //AccessToAttributeOfArrayElementNotSupported
        return;
    }
}
*/
            
            /*
             *
             jeżeli jest arrayIndex to nie może już być atrybutu
             
             
             
             */
            // maybe first resolve instance and prototype's parent class reference?
            
            // reference . attribute TODO
            // TODO take attribute into account
            
        }


        
        private ClassDefinitionNode GetClassDefinitionNode(string parentName)
        {
            while (true)
            {
                ASTNode node = _symbolTable[parentName].Node;

                switch (node)
                {
                    case ClassDefinitionNode classDefinitionNode:
                        return classDefinitionNode;
                    case PrototypeDefinitionNode prototypeDefinitionNode:
                        if (_nodesInCycle.Contains(prototypeDefinitionNode.InheritanceReferenceNode))
                        {
                            return null;
                        }

                        parentName = prototypeDefinitionNode.InheritanceReferenceNode.Name;
                        break;
                    default:
                        return null;
                }
            }
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