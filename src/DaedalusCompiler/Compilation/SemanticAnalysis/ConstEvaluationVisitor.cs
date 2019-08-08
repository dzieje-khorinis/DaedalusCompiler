using System;
using System.Collections.Generic;
using System.Linq;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    //evaluate const content, array elements, array index, array reference index 
    public class ConstEvaluationVisitor : AbstractSyntaxTreeBaseGenericVisitor<NodeValue>
    {
        private readonly Dictionary <string, Symbol> _symbolTable;
        private readonly Dictionary<ASTNode, NodeValue> _visitedNodesValuesCache;


        public ConstEvaluationVisitor(Dictionary <string, Symbol> symbolTable)
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
        
        public override NodeValue Visit(ASTNode node)
        {
            if (_visitedNodesValuesCache.ContainsKey(node))
            {
                PrintVisit(node, true);
                if (_visitedNodesValuesCache[node] is UninitializedValue)
                {
                    node.Annotations.Add(new InfiniteReferenceLoopAnnotation());
                    _visitedNodesValuesCache[node] = new UndefinedValue();
                    if (node is ReferenceNode referenceNode)
                    {
                        Console.WriteLine($"Add InfiniteReferenceLoopUndefinedValue to referenceNode {referenceNode.Name}");
                    }
                    else
                    {
                        Console.WriteLine($"Add InfiniteReferenceLoopUndefinedValue to {node.GetType().ToString().Split(".").Last()}");
                    }
                }
                return _visitedNodesValuesCache[node];
            }
            
            PrintVisit(node, false);
            _visitedNodesValuesCache[node] = new UninitializedValue();
            
            NodeValue resultValue = base.Visit(node);
            _visitedNodesValuesCache[node] = resultValue;
            if (resultValue is UndefinedValue)
            {
                return new UndefinedValue();
            }
            return _visitedNodesValuesCache[node];
        }


        protected override NodeValue VisitVarArrayDeclaration(VarArrayDeclarationNode node)
        {
            if (node.ArraySizeValue is UndefinedValue)
            {
                return new UndefinedValue();
            }
            node.ArraySizeValue = Visit(node.ArraySizeNode);
            return null;
        }

        protected override NodeValue VisitConstArrayDefinition(ConstArrayDefinitionNode node)
        {
            if (node.ArraySizeValue is UndefinedValue)
            {
                return new UndefinedValue();
            }
            node.ArraySizeValue = Visit(node.ArraySizeNode);

            
            SymbolType builtinType = node.Symbol.BuiltinType;
            foreach (var elementNode in node.ElementNodes)
            {
                NodeValue elementValue = Visit(elementNode);
                node.ElementValues.Add(elementValue);

                if (elementValue is UndefinedValue)
                {
                    continue;
                }
                SymbolType elementType = NodeValueToBuiltinType(elementValue);
                CheckType(builtinType, elementType, elementNode);
            }


            switch (node.ArraySizeValue)
            {
                case UndefinedValue _:
                    break;
                case IntValue intValue:
                    if (intValue.Value != node.ElementNodes.Count)
                    {
                        node.Annotations.Add(new InconsistentSizeAnnotation());
                    }
                    break;
                default:
                    node.ArraySizeNode.Annotations.Add(new UnsupportedTypeAnnotation());
                    break;
            }
            
            return null;
        }

        protected override NodeValue VisitParameterArrayDeclaration(ParameterArrayDeclarationNode node)
        {
            if (node.ArraySizeValue is UndefinedValue)
            {
                return new UndefinedValue();
            }
            node.ArraySizeValue = Visit(node.ArraySizeNode);
            return null;
        }
        

        protected override NodeValue VisitConstDefinition(ConstDefinitionNode node)
        {
            node.RightSideValue = Visit(node.RightSideNode);
            if (node.RightSideValue is UndefinedValue)
            {
                return null;
            }
            
            SymbolType rightSideType = NodeValueToBuiltinType(node.RightSideValue);
            CheckType(node.Symbol.BuiltinType, rightSideType, node.RightSideNode);
            return null;
        }


        protected override NodeValue VisitArrayIndexNode(ArrayIndexNode arrayIndexNode)
        {
            arrayIndexNode.Value = Visit(arrayIndexNode.ExpressionNode);

            switch (arrayIndexNode.Value)
            {
                case IntValue _:
                case UndefinedValue _:
                    break;
                default:
                    arrayIndexNode.Annotations.Add(new ConstIntegerExpectedAnnotation());
                    return new UndefinedValue();
            }
            
            return arrayIndexNode.Value;
        }

        protected override NodeValue VisitReference(ReferenceNode referenceNode)
        {
            if (referenceNode.Symbol == null)
            {
                return new UndefinedValue();
            }

            
            NodeValue arrayIndexValue = new IntValue(0);
            foreach (var partNode in referenceNode.PartNodes)
            {
                switch (partNode)
                {
                    case ArrayIndexNode arrayIndexNode:
                        arrayIndexValue = Visit(arrayIndexNode);
                        break;
                }
            }
            
            switch (referenceNode.Symbol.Node)
            {
                case ConstArrayDefinitionNode constArrayDefinitionNode:
                    
                    switch (arrayIndexValue)
                    {
                        case IntValue intValue:
                            if (intValue.Value >= constArrayDefinitionNode.ElementNodes.Count)
                            {
                                referenceNode.Annotations.Add(new IndexOutOfRangeAnnotation());
                                return new UndefinedValue();
                            }
                            else if (intValue.Value > 255)
                            {
                                //referenceNode.Annotations.Add(new TooBigIndexValue());
                                return new UndefinedValue();
                            }
                            return Visit(constArrayDefinitionNode.ElementNodes[(int)intValue.Value]);
                        
                        case UndefinedValue _:
                            return new UndefinedValue();
                        
                        default:
                            throw new Exception();
                    }

                case ConstDefinitionNode constDefinitionNode:
                    return Visit(constDefinitionNode.RightSideNode);
                
                case FunctionDefinitionNode _:
                    return new FunctionValue(referenceNode.Symbol);
                
                default:
                    referenceNode.Annotations.Add(new NotConstReferenceAnnotation());
                    return new UndefinedValue();
            }
        }
        

        protected override NodeValue VisitUnaryExpression(UnaryExpressionNode node)
        {
            NodeValue expressionValue = Visit(node.ExpressionNode);
            if (expressionValue is UndefinedValue)
            {
                return new UndefinedValue();
            }
            
            try
            {
                return ConstEvaluationHelper.EvaluateUnaryOperation(node.Operator, expressionValue);
            }
            catch (InvalidUnaryOperationException)
            {
                node.Annotations.Add(new InvalidUnaryOperationAnnotation());
                return new UndefinedValue();
            }
        }

        protected override NodeValue VisitBinaryExpression(BinaryExpressionNode node)
        {
            NodeValue leftNodeValue = Visit(node.LeftSideNode); 
            NodeValue rightNodeValue = Visit(node.RightSideNode);
            if (leftNodeValue is UndefinedValue || rightNodeValue is UndefinedValue)
            {
                return new UndefinedValue();
            }

            try
            {
                return ConstEvaluationHelper.EvaluateBinaryOperation(node.Operator, leftNodeValue, rightNodeValue);
            }
            catch (InvalidBinaryOperationException)
            {
                node.Annotations.Add(new InvalidBinaryOperationAnnotation());
                return new UndefinedValue();
            }
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
        
        
        private void CheckType(SymbolType expectedType, SymbolType actualType, ASTNode node)
        {
            switch (expectedType)
            {
                case SymbolType.Int:

                    switch (actualType)
                    {
                        case SymbolType.Int:
                            break;
                        
                        default:
                            node.Annotations.Add(new IncompatibleTypesAnnotation(expectedType, actualType));
                            break;

                    }
                    break;
                
                case SymbolType.Float:
                    switch (actualType)
                    {
                        case SymbolType.Int:
                        case SymbolType.Float:
                            break;
                        
                        default:
                            node.Annotations.Add(new IncompatibleTypesAnnotation(expectedType, actualType));
                            break;

                    }
                    break;
                
                case SymbolType.String:
                    switch (actualType)
                    {
                        case SymbolType.String:
                            break;
                        
                        default:
                            node.Annotations.Add(new IncompatibleTypesAnnotation(expectedType, actualType));
                            break;
                    }
                    break;
                
                
                case SymbolType.Func:
                    switch (actualType)
                    {
                        case SymbolType.Func:
                            break;
                        
                        default:
                            node.Annotations.Add(new IncompatibleTypesAnnotation(expectedType, actualType));
                            break;
                    }
                    break;

                default:
                    throw new Exception();
            }
        }
        
        private SymbolType NodeValueToBuiltinType(NodeValue nodeValue)
        {
            switch (nodeValue)
            {
                case IntValue _:
                    return SymbolType.Int;
                
                case FloatValue _:
                    return SymbolType.Float;
                
                case StringValue _:
                    return SymbolType.String;

                case FunctionValue _:
                    return SymbolType.Func;
                
                default:
                    throw new Exception();
            }
        }
    }
}