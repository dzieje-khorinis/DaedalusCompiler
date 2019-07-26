using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    public class ConstEvaluationVisitor : AbstractSyntaxTreeBaseGenericVisitor<NodeValue>
    {
        private readonly Dictionary <string, SymbolContext> _symbolTable;
        private readonly Dictionary<ASTNode, NodeValue> _visitedNodesValuesCache;


        private ReferenceNode referenceChainHead;
        private HashSet<ReferenceNode> referenceChain;
        
        
        
        public ConstEvaluationVisitor(Dictionary <string, SymbolContext> symbolTable)
        {
            _symbolTable = symbolTable;
            _visitedNodesValuesCache = new Dictionary<ASTNode, NodeValue>();
        }


        private void PrintVisit(ASTNode node, bool cached=false)
        {
            string message = node.GetType().ToString().Split(".").Last();
            switch (node)
            {
                case ConstDefinitionNode constDefinitionNode:
                    message = $"{message} {constDefinitionNode.NameNode.Value}";
                    break;
                case ReferenceNode referenceNode:
                    message = $"{message} {referenceNode.Name}";
                    break;
                case BinaryExpressionNode _:
                    break;
                default:
                    return;
            }

            if (cached)
            {
                Console.WriteLine($"Visit CACHED: {message}");
            }
            else
            {
                Console.WriteLine($"Visit: {message}");
            }
            

        }
        
        protected override NodeValue Visit(ASTNode node)
        {
            if (_visitedNodesValuesCache.ContainsKey(node))
            {
                PrintVisit(node, true);
                if (_visitedNodesValuesCache[node] == null)
                {
                    if (node is ReferenceNode referenceNode)
                    {
                        Console.WriteLine($"Add cycle annotation to referenceNode {referenceNode.Name}");
                    }
                    else
                    {
                        Console.WriteLine($"Add cycle annotation to {node.GetType().ToString().Split(".").Last()}");
                    }
                    
                    node.Annotations.Add(new CycleAnnotation());
                }
                return _visitedNodesValuesCache[node];
            }
            
            PrintVisit(node, false);
            _visitedNodesValuesCache[node] = null;
            _visitedNodesValuesCache[node] = base.Visit(node);
            
            /*
            if (_visitedNodesValuesCache[node] == null && node is ReferenceNode)
            {
                node.Annotations.Add(new CycleAnnotation());
            }
            */
            
            return _visitedNodesValuesCache[node];
        }
        
        
        protected override NodeValue VisitConstDefinition(ConstDefinitionNode node)
        {
            NodeValue nodeValue = Visit(node.RightSideNode);
            return null;
        }

        protected override NodeValue VisitReference(ReferenceNode referenceNode)
        {
            string nodeName = referenceNode.Name.ToUpper();

            if (!_symbolTable.ContainsKey(nodeName))
            {
                referenceNode.Annotations.Add(new UndeclaredIdentifierAnnotation());
                return new UndeclaredIdentifierValue();
            }
            
            SymbolContext symbolContext = _symbolTable[nodeName];
            
            switch (symbolContext.Node)
            {
                case ConstDefinitionNode constDefinitionNode:
                    return Visit(constDefinitionNode.RightSideNode);
                
            }


            return null;
        }
        
        
        
        protected override NodeValue VisitBinaryExpression(BinaryExpressionNode node)
        {
            NodeValue leftNodeValue = Visit(node.LeftSideNode); 
            NodeValue rightNodeValue = Visit(node.RightSideNode);
            
            NodeValue result = EvaluationHelper.EvaluateBinaryOperation(node.Operator, leftNodeValue,rightNodeValue);
            return result;
        }
    }
}