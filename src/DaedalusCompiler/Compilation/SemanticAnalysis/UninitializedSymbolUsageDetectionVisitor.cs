using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;


namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    class ReferenceData
    {
        public bool HasDotInPath;
        public string PreDotPath; // if '.' apprears in path, this will only contain part before '.'
        public string BaseName; // if '[' appears in PreDotPath, this will only contain part before '['
        public long Index; 
        public ASTNode Node;

        public ReferenceData(ASTNode node, string fullPath, long index = -1)
        {
            Node = node;
            HasDotInPath = fullPath.Contains(".");
            PreDotPath = fullPath.Split(".").First();
            BaseName = PreDotPath.Split("[").First();
            Index = index;
            
            PreDotPath = PreDotPath.ToUpper();
        }
    }

    
    public class UninitializedSymbolUsageDetectionVisitor : AbstractSyntaxTreeBaseVisitor
    {
        private readonly Dictionary<SubclassNode, HashSet<string>> _node2initializedAttributesPaths;
        private readonly Dictionary<DeclarationNode, HashSet<string>> _node2initializedLocalsPaths;
        
        private HashSet<string> _initializedAttributesPaths;
        private HashSet<string> _initializedLocalsPaths;

        private BlockSymbol _currentBlockSymbol;
        
        public UninitializedSymbolUsageDetectionVisitor()
        {
            _node2initializedAttributesPaths = new Dictionary<SubclassNode, HashSet<string>>();
            _node2initializedLocalsPaths = new Dictionary<DeclarationNode, HashSet<string>>();
            _initializedAttributesPaths = null;
            _initializedLocalsPaths = null;
            _currentBlockSymbol = null;
        }
        

        private HashSet<string> GetInitializedAttributesPaths(SubclassNode initialSubclassNode)
        {
            HashSet<string> initializedAttributesPaths = new HashSet<string>();
            
            ASTNode node = initialSubclassNode;
            while (node is SubclassNode subclassNode)
            {
                initializedAttributesPaths.UnionWith(_node2initializedAttributesPaths[subclassNode]);
                SubclassSymbol subclassSymbol = (SubclassSymbol) subclassNode.Symbol;
                node = subclassSymbol.InheritanceParentSymbol.Node;
            }

            return initializedAttributesPaths;
        }
        

        private ReferenceData GetDataFromReferenceNode(ReferenceNode referenceNode)
        {
            
            
            string path = referenceNode.Name;
            long index = -1;


            int iteration_no = -1;
            foreach (ReferencePartNode partNode in referenceNode.PartNodes)
            {
                iteration_no++;
                
                switch (partNode)
                {
                    case AttributeNode attributeNode:
                        path = $"{path}.{attributeNode.Name}";
                        break;
                    
                    case ArrayIndexNode arrayIndexNode:
                        NodeValue nodeValue = arrayIndexNode.Value;
                        switch (nodeValue)
                        {
                            case IntValue intValue:
                                path = $"{path}[{(int) intValue.Value}]";
                                if (iteration_no == 0)
                                {
                                    index = intValue.Value;
                                }
                                break;
                            default:
                                return null;
                        }
                        
                        break;
                    default:
                        throw new Exception();
                }
            }
            
            return new ReferenceData(referenceNode, path, index);
        }
        
        
        protected override void VisitAssignment(AssignmentNode node)
        {
            ReferenceNode referenceNode = node.LeftSideNode;
            NestableSymbol nestableSymbol = (NestableSymbol) referenceNode.Symbol;

            switch (nestableSymbol?.Node)
            {
                case ConstDefinitionNode _:
                case PrototypeDefinitionNode _:
                    break;
                case VarDeclarationNode _:

                   
                    
                    if (_currentBlockSymbol == null)
                    {
                        throw new Exception();
                    }

                    NestableSymbol baseSymbol = referenceNode.BaseSymbol as NestableSymbol;
                    if (baseSymbol == null)
                    {
                        break;
                    }
                    
                    ReferenceData referenceData = GetDataFromReferenceNode(referenceNode);
                    if (referenceData == null || referenceData.HasDotInPath)
                    {
                        break;
                    }

                    string relativePath = referenceData.PreDotPath;

                    if (_currentBlockSymbol is SubclassSymbol currentSubclassSymbol)
                    {
                        if (baseSymbol.ParentBlockSymbol == currentSubclassSymbol.BaseClassSymbol)
                        {
                            _initializedAttributesPaths.Add(relativePath);
                        }
                    }
                    else if (_currentBlockSymbol is FunctionSymbol)
                    {
                        if (nestableSymbol.ParentBlockSymbol == _currentBlockSymbol)
                        {
                            _initializedLocalsPaths.Add(relativePath);
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }
                    break;
            }
            
            base.VisitAssignment(node);
        }

        protected override void VisitReference(ReferenceNode referenceNode)
        {
            NestableSymbol nestableSymbol = referenceNode.Symbol as NestableSymbol;
            switch (nestableSymbol?.Node)
            {
                case ConstDefinitionNode _:
                case PrototypeDefinitionNode _:
                    break;
                case VarDeclarationNode _:
                    
                    if (_currentBlockSymbol == null)
                    {
                        throw new Exception();
                    }

                    NestableSymbol baseSymbol = referenceNode.BaseSymbol as NestableSymbol;
                    if (baseSymbol == null)
                    {
                        break;
                    }

                    ReferenceData referenceData = GetDataFromReferenceNode(referenceNode);
                    if (referenceData == null)
                    {
                        break;
                    }

                    if (_currentBlockSymbol is SubclassSymbol currentSubclassSymbol)
                    {
                        if (baseSymbol.ParentBlockSymbol == currentSubclassSymbol.BaseClassSymbol)
                        {
                            HashSet<string> initializedAttributesPaths = GetInitializedAttributesPaths((SubclassNode) _currentBlockSymbol.Node);
                            
                            if (!initializedAttributesPaths.Contains(referenceData.PreDotPath))
                            {
                                referenceNode.Annotations.Add(new UsageOfNonInitializedVariableWarning(
                                    referenceData.BaseName, referenceData.Index, true));
                            }
                            

                        }
                    }
                    else if (_currentBlockSymbol is FunctionSymbol)
                    {
                        if (nestableSymbol.ParentBlockSymbol == _currentBlockSymbol)
                        {
                            if (!_initializedLocalsPaths.Contains(referenceData.PreDotPath))
                            {
                                referenceNode.Annotations.Add(new UsageOfNonInitializedVariableWarning(
                                    referenceData.BaseName, referenceData.Index,false));
                            }
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }
                    break;
            }
            
            
            
            base.VisitReference(referenceNode);
        }
        
        protected override void VisitConstDefinition(ConstDefinitionNode node) { }

        protected override void VisitConstArrayDefinition(ConstArrayDefinitionNode node) { }
        

        protected override void VisitPrototypeDefinition(PrototypeDefinitionNode node)
        {
            _initializedAttributesPaths = new HashSet<string>();
            _initializedLocalsPaths = new HashSet<string>();
            _node2initializedAttributesPaths[node] = _initializedAttributesPaths;
            _node2initializedLocalsPaths[node] = _initializedLocalsPaths;
            
            _currentBlockSymbol = (BlockSymbol) node.Symbol;
            
            base.VisitPrototypeDefinition(node);
            
            _currentBlockSymbol = null;
        }

        protected override void VisitInstanceDefinition(InstanceDefinitionNode node)
        {
            _initializedAttributesPaths = new HashSet<string>();
            _initializedLocalsPaths = new HashSet<string>();
            _node2initializedAttributesPaths[node] = _initializedAttributesPaths;
            _node2initializedLocalsPaths[node] = _initializedLocalsPaths;
            
            _currentBlockSymbol = (BlockSymbol) node.Symbol;
            base.VisitInstanceDefinition(node);
            _currentBlockSymbol = null;
        }

        protected override void VisitFunctionDefinition(FunctionDefinitionNode node)
        {
            _initializedLocalsPaths = new HashSet<string>();
            _node2initializedLocalsPaths[node] = _initializedLocalsPaths;
            
            _currentBlockSymbol = (BlockSymbol) node.Symbol;
            base.VisitFunctionDefinition(node);
            _currentBlockSymbol = null;
        }
    }
}