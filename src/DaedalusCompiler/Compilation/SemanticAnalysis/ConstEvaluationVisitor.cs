using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    //evaluate const content, array elements, array index, array reference index 
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
        
        public override NodeValue Visit(ASTNode node)
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
            
            NodeValue resultValue = base.Visit(node);
            VisitedNodesValuesCache[node] = resultValue;
            if (resultValue is ErrorValue)
            {
                return new UndefinedErrorValue();
            }
            return VisitedNodesValuesCache[node];
        }


        protected override NodeValue VisitVarArrayDeclaration(VarArrayDeclarationNode node)
        {
            if (node.ArraySizeValue is ErrorValue)
            {
                return new UndefinedErrorValue();
            }
            node.ArraySizeValue = Visit(node.ArraySizeNode);
            
            return null;
        }

        protected override NodeValue VisitConstArrayDefinition(ConstArrayDefinitionNode node)
        {
            if (node.ArraySizeValue is ErrorValue)
            {
                return new UndefinedErrorValue();
            }
            node.ArraySizeValue = Visit(node.ArraySizeNode);
            return null;
        }

        protected override NodeValue VisitParameterArrayDeclaration(ParameterArrayDeclarationNode node)
        {
            if (node.ArraySizeValue is ErrorValue)
            {
                return new UndefinedErrorValue();
            }
            node.ArraySizeValue = Visit(node.ArraySizeNode);
            return null;
        }

        protected override NodeValue VisitConstDefinition(ConstDefinitionNode node)
        {
            if (node.RightSideValue is ErrorValue)
            {
                return new UndefinedErrorValue();
            }
            node.RightSideValue = Visit(node.RightSideNode);
            return null; // TODO what to return here?
        }
        

        protected override NodeValue VisitReference(ReferenceNode referenceNode)
        {
            if (referenceNode.ArrayIndexNode != null)
            {
                referenceNode.ArrayIndexValue = Visit(referenceNode.ArrayIndexNode);
            }
            
            string nodeName = referenceNode.Name.ToUpper();
            if (!_symbolTable.ContainsKey(nodeName))
            {
                return new UndeclaredIdentifierErrorValue();
            }
            
            SymbolContext symbolContext = _symbolTable[nodeName];


            switch (symbolContext.Node)
            {
                case ConstArrayDefinitionNode constArrayDefinitionNode:
                    if (referenceNode.ArrayIndexNode == null)
                    {
                        //orinal compiler actually accesses index 0 if no square brackets are provided
                        return new SquareBracketsExpectedErrorValue();
                    }
                    else
                    {
                        switch (referenceNode.ArrayIndexValue)
                        {
                            case ErrorValue _:
                                return new UndefinedErrorValue();
                            case IntValue intValue:
                                if (intValue.Value >= constArrayDefinitionNode.ElementNodes.Count)
                                {
                                    return new IndexOutOfRangeErrorValue();
                                }
                                return Visit(constArrayDefinitionNode.ElementNodes[intValue.Value]);
                            default:
                                referenceNode.ArrayIndexValue = new ConstIntegerExpectedErrorValue();
                                return new UndefinedErrorValue();
                        }
                    }

                case ConstDefinitionNode constDefinitionNode:
                    if (referenceNode.ArrayIndexNode != null)
                    {
                        return new SquareBracketsNotExpectedErrorValue();
                    }
                    return Visit(constDefinitionNode.RightSideNode);
                
                default:
                    return new NotConstReferenceErrorValue();
                
            }
            
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