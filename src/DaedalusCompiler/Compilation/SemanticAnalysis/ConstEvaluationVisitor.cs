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
        public readonly Dictionary<ASTNode, NodeValue> VisitedNodesValuesCache;


        public ConstEvaluationVisitor(Dictionary <string, SymbolContext> symbolTable)
        {
            _symbolTable = symbolTable;
            VisitedNodesValuesCache = new Dictionary<ASTNode, NodeValue>();
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
            if (VisitedNodesValuesCache.ContainsKey(node))
            {
                PrintVisit(node, true);
                if (VisitedNodesValuesCache[node] is UninitializedValue)
                {
                    VisitedNodesValuesCache[node] = new InfiniteReferenceLoopErrorValue();
                    if (node is ReferenceNode referenceNode)
                    {
                        Console.WriteLine($"Add InfiniteReferenceLoopErrorValue to referenceNode {referenceNode.Name}");
                    }
                    else
                    {
                        Console.WriteLine($"Add InfiniteReferenceLoopErrorValue to {node.GetType().ToString().Split(".").Last()}");
                    }
                }
                return VisitedNodesValuesCache[node];
            }
            
            PrintVisit(node, false);
            VisitedNodesValuesCache[node] = new UninitializedValue();
            VisitedNodesValuesCache[node] = base.Visit(node);
            return VisitedNodesValuesCache[node];
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
                referenceNode.Errors.Add(new UndeclaredIdentifierAnnotation());
                return new UndeclaredIdentifierErrorValue();
            }
            
            SymbolContext symbolContext = _symbolTable[nodeName];
            
            switch (symbolContext.Node)
            {
                case ConstDefinitionNode constDefinitionNode:
                    return Visit(constDefinitionNode.RightSideNode);
                
            }


            return new UninitializedValue();
        }
        
        
        
        protected override NodeValue VisitBinaryExpression(BinaryExpressionNode node)
        {
            NodeValue leftNodeValue = Visit(node.LeftSideNode); 
            NodeValue rightNodeValue = Visit(node.RightSideNode);
            if (leftNodeValue is ErrorValue || rightNodeValue is ErrorValue)
            {
                return new UndefinedErrorValue();
            }
            
            NodeValue result = EvaluationHelper.EvaluateBinaryOperation(node.Operator, leftNodeValue,rightNodeValue);
            return result;
        }
        
        
        protected override NodeValue VisitFloatLiteral(FloatLiteralNode node)
        {
            return new FloatValue(node.Value);
        }

        protected override NodeValue VisitIntegerLiteral(IntegerLiteralNode node)
        {
            return new IntValue(node.Value);
        }

        protected override NodeValue VisitStringLiteral(StringLiteralNode node)
        {
            return new StringValue(node.Value);
        }

    }
}