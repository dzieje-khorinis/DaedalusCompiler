using System;
using System.Collections.Generic;
using System.Linq;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    public class IncompatibleTypesException : Exception
    {
        
    }
    
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
            return;
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
                    node.Annotations.Add(new InfiniteReferenceLoopError());
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
            
            switch (node.ArraySizeValue)
            {
                case UndefinedValue _:
                    break;
                case IntValue intValue:
                    if (intValue.Value == 0)
                    {
                        node.ArraySizeNode.Annotations.Add(new ArraySizeEqualsZeroError(node.NameNode.Value));
                    }
                    else if (intValue.Value > 4095)
                    {
                        node.ArraySizeNode.Annotations.Add(new TooBigArraySizeError());
                    }
                    break;
                default:
                    node.ArraySizeNode.Annotations.Add(new UnsupportedTypeError());
                    break;
            }
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
                CheckArrayElementType(builtinType, elementType, elementNode);
            }


            switch (node.ArraySizeValue)
            {
                case UndefinedValue _:
                    break;
                case IntValue intValue:
                    if (intValue.Value == 0)
                    {
                        node.ArraySizeNode.Annotations.Add(new ArraySizeEqualsZeroError(node.NameNode.Value));
                    }
                    else if (intValue.Value > 4095)
                    {
                        node.ArraySizeNode.Annotations.Add(new TooBigArraySizeError());
                    }
                    else if (intValue.Value != node.ElementNodes.Count)
                    {
                        node.ArraySizeNode.Annotations.Add(new InconsistentSizeError(node.NameNode.Value, (int) intValue.Value, node.ElementNodes.Count));
                    }
                    break;
                default:
                    node.ArraySizeNode.Annotations.Add(new UnsupportedTypeError());
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
            
            switch (node.ArraySizeValue)
            {
                case UndefinedValue _:
                    break;
                case IntValue intValue:
                    if (intValue.Value == 0)
                    {
                        node.ArraySizeNode.Annotations.Add(new ArraySizeEqualsZeroError(node.NameNode.Value));
                    }
                    else if (intValue.Value > 4095)
                    {
                        node.ArraySizeNode.Annotations.Add(new TooBigArraySizeError());
                    }
                    break;
                default:
                    node.ArraySizeNode.Annotations.Add(new UnsupportedTypeError());
                    break;
            }
            return null;
        }
        

        protected override NodeValue VisitConstDefinition(ConstDefinitionNode node)
        {
            node.RightSideValue = Visit(node.RightSideNode);
            if (node.RightSideValue is UndefinedValue)
            {
                return null;
            }
            
            CheckValueType(node);
            return null;
        }
        

        protected override NodeValue VisitArrayIndexNode(ArrayIndexNode arrayIndexNode)
        {
            arrayIndexNode.Value = Visit(arrayIndexNode.ExpressionNode);

            switch (arrayIndexNode.Value)
            {
                case IntValue arrayIndexValue:
                    ReferenceNode referenceNode = (ReferenceNode) arrayIndexNode.ParentNode;
                    if (referenceNode.Symbol.Node is IArrayDeclarationNode arrayDeclarationNode)
                    {
                        Visit(referenceNode.Symbol.Node);
                        if (arrayDeclarationNode.ArraySizeValue is IntValue arraySizeValue)
                        {
                            if (arrayIndexValue.Value >= arraySizeValue.Value)
                            {
                                arrayIndexNode.Annotations.Add(new IndexOutOfRangeError(arraySizeValue.Value));
                                return new UndefinedValue();
                            }

                            if (arrayIndexValue.Value > 255)
                            {
                                arrayIndexNode.Annotations.Add(new TooBigArrayIndex());
                                return new UndefinedValue();
                            }
                        }
                    }
                    break;
                case UndefinedValue _:
                    break;
                default:
                    arrayIndexNode.Annotations.Add(new ConstIntegerExpectedError());
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
            //ArrayIndexNode arrayIndexNode = null;
            
            foreach (var partNode in referenceNode.PartNodes)
            {
                switch (partNode)
                {
                    case ArrayIndexNode node:
                        //arrayIndexNode = node;
                        arrayIndexValue = Visit(node);
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
                                //arrayIndexNode?.Annotations.Add(new IndexOutOfRangeError());
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
                    referenceNode.Annotations.Add(new NotConstReferenceError());
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
                NodeValue nodeValue = ConstEvaluationHelper.EvaluateUnaryOperation(node.Operator, expressionValue);
                if (nodeValue is IntValue intValue)
                {
                    if (intValue.Value < Int32.MinValue || intValue.Value > Int32.MaxValue)
                    {
                        node.Annotations.Add(new ArithmeticOperationOverflowError(node.OperatorLocation));
                        return new UndefinedValue();
                    }
                }
                return nodeValue;
                
            }
            catch (InvalidUnaryOperationException)
            {
                node.Annotations.Add(new InvalidUnaryOperationError());
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
                NodeValue nodeValue = ConstEvaluationHelper.EvaluateBinaryOperation(node.Operator, leftNodeValue, rightNodeValue);
                if (nodeValue is IntValue intValue)
                {
                    if (intValue.Value < Int32.MinValue || intValue.Value > Int32.MaxValue)
                    {
                        node.Annotations.Add(new ArithmeticOperationOverflowError(node.OperatorLocation));
                        return new UndefinedValue();
                    }
                }
                return nodeValue;
            }
            catch (InvalidBinaryOperationException)
            {
                node.Annotations.Add(new InvalidBinaryOperationError());
                return new UndefinedValue();
            }
            catch (OverflowException)
            {
                node.Annotations.Add(new ArithmeticOperationOverflowError(node.OperatorLocation));
                return new UndefinedValue();
            }
        }
        
        
        protected override NodeValue VisitFloatLiteral(FloatLiteralNode node)
        {
            return new FloatValue(node.Value);
        }

        protected override NodeValue VisitIntegerLiteral(IntegerLiteralNode node)
        {
            long value = node.Value;
            ASTNode annotateToNode = node;

            if (node.ParentNode is UnaryExpressionNode unaryExpressionNode)
            {
                switch (unaryExpressionNode.Operator)
                {
                    case UnaryOperator.Minus:
                        value = -value;
                        annotateToNode = node.ParentNode;
                        break;
                    case UnaryOperator.Plus:
                        annotateToNode = node.ParentNode;
                        break;
                }
            }
                
            if (!node.EvaluatedCorrectly || value < Int32.MinValue || value > Int32.MaxValue)
            {
                annotateToNode.Annotations.Add(new IntegerLiteralTooLargeError());
                return new UndefinedValue();
            }

            return new IntValue(node.Value);
        }

        protected override NodeValue VisitStringLiteral(StringLiteralNode node)
        {
            return new StringValue(node.Value);
        }


        private void CheckValueType(ConstDefinitionNode node) // SymbolType expectedType, SymbolType actualType, ASTNode node
        {
            SymbolType expectedType = node.Symbol.BuiltinType;
            SymbolType actualType = NodeValueToBuiltinType(node.RightSideValue);
            ASTNode rightSideNode = node.RightSideNode
   ;         
            try
            {
                CheckType(expectedType, actualType);
            }
            catch (IncompatibleTypesException)
            {
                rightSideNode.Annotations.Add(new CannotInitializeConstWithValueOfDifferentType(expectedType, actualType, node.NameNode.Location, rightSideNode.Location));
            }

            
        }

        private void CheckArrayElementType(SymbolType expectedType, SymbolType actualType, ASTNode node)
        {
            try
            {
                CheckType(expectedType, actualType);
            }
            catch (IncompatibleTypesException)
            {
                node.Annotations.Add(new CannotInitializeArrayElementWithValueOfDifferentType(expectedType, actualType));
            }
        }
        
        private void CheckType(SymbolType expectedType, SymbolType actualType)
        {
            switch (expectedType)
            {
                case SymbolType.Int:

                    switch (actualType)
                    {
                        case SymbolType.Int:
                            break;
                        
                        default:
                            throw new IncompatibleTypesException();
                    }
                    break;
                
                case SymbolType.Float:
                    switch (actualType)
                    {
                        case SymbolType.Int:
                        case SymbolType.Float:
                            break;
                        
                        default:
                            throw new IncompatibleTypesException();

                    }
                    break;
                
                case SymbolType.String:
                    switch (actualType)
                    {
                        case SymbolType.String:
                            break;
                        
                        default:
                            throw new IncompatibleTypesException();
                    }
                    break;
                
                
                case SymbolType.Func:
                    switch (actualType)
                    {
                        case SymbolType.Func:
                            break;
                        
                        default:
                            throw new IncompatibleTypesException();
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