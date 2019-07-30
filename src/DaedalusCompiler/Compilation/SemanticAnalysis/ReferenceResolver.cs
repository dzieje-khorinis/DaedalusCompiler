using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    public class ReferenceResolver
    {
        private readonly Dictionary <string, SymbolContext> _symbolTable;
        
        public ReferenceResolver(Dictionary<string, SymbolContext> symbolTable)
        {
            _symbolTable = symbolTable;
        }
        
        public void Resolve(List<ParentReferenceNode> parentReferenceNodes)
        {
            foreach (var parentReferenceNode in parentReferenceNodes)
            {
                Resolve(parentReferenceNode);
            }
        }

        public void Resolve(ParentReferenceNode parentReferenceNode)
        {
            if (parentReferenceNode.AttributeNode != null || parentReferenceNode.ArrayIndexNode != null)
            {
                parentReferenceNode.Annotations.Add(new NotClassOrPrototypeReferenceAnnotation());
                return;
            }

            string path = parentReferenceNode.Name.ToUpper();
            if (!_symbolTable.ContainsKey(path))
            {
                parentReferenceNode.Annotations.Add(new NotClassOrPrototypeReferenceAnnotation());
                return;
            }
            
            parentReferenceNode.Path = path;
            parentReferenceNode.Symbol = _symbolTable[path].Symbol;
        }

        public void Resolve(List<ReferenceNode> referenceNodes)
        {
            foreach (var referenceNode in referenceNodes)
            {
                Resolve(referenceNode);
            }
        }

        public void Resolve(ReferenceNode referenceNode)
        {
            if (referenceNode.ArrayIndexNode != null)
            {
                if (referenceNode.AttributeNode != null)
                {
                    referenceNode.AttributeNode.Annotations.Add(new AccessToAttributeOfArrayElementNotSupportedAnnotation());
                    //AccessToAttributeOfArrayElementNotSupported
                    return;
                }
            }
            
            /*
             *
             jeżeli jest arrayIndex to nie może już być atrybutu
             
             
             
             */
            // maybe first resolve instance and prototype's parent class reference?
            
            // reference . attribute TODO
            // TODO take attribute into account
            
            
            ASTNode ancestor = GetFirstSignificantAncestor(referenceNode);
            string path;
            
            switch (ancestor)
            {
                case PrototypeDefinitionNode prototypeDefinitionNode:
                    //check local
                    path = $"{prototypeDefinitionNode.NameNode.Value.ToUpper()}.{referenceNode.Name}";
                    if (_symbolTable.ContainsKey(path))
                    {
                        referenceNode.Path = path;
                        referenceNode.Symbol = _symbolTable[referenceNode.Path].Symbol;
                        return;
                    }
                    
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
            
        }
        
        
        


        private ASTNode GetFirstSignificantAncestor(ASTNode node)
        {
            while (node != null)
            {
                node = node.Parent;
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