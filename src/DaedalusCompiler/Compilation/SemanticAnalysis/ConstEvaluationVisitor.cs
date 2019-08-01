/*using System;
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
                return new UndefinedValue();
            }

            SymbolContext symbolContext = _symbolTable[node.NameNode.Value.ToUpper()];
            DatSymbol symbol = symbolContext.Symbol;
            DatSymbolType rightSideType = NodeValueToBuiltinType(node.RightSideValue);
            
            
            
            switch (symbol.BuiltinType)
            {
                case DatSymbolType.Int:

                    switch (rightSideType)
                    {
                        case DatSymbolType.Int:
                            break;
                        
                        default:
                            node.Annotations.Add(new IncompatibleTypesAnnotation(symbol.BuiltinType, rightSideType));
                            break;

                    }
                    break;
                
                case DatSymbolType.Float:
                    switch (rightSideType)
                    {
                        case DatSymbolType.Int:
                        case DatSymbolType.Float:
                            break;
                        
                        default:
                            node.Annotations.Add(new IncompatibleTypesAnnotation(symbol.BuiltinType, rightSideType));
                            break;

                    }
                    break;
                
                case DatSymbolType.String:
                    switch (rightSideType)
                    {
                        case DatSymbolType.String:
                            break;
                        
                        default:
                            node.Annotations.Add(new IncompatibleTypesAnnotation(symbol.BuiltinType, rightSideType));
                            break;
                    }
                    break;
                
                
                case DatSymbolType.Func:
                    switch (rightSideType)
                    {
                        case DatSymbolType.Func:
                            break;
                        
                        default:
                            node.Annotations.Add(new IncompatibleTypesAnnotation(symbol.BuiltinType, rightSideType));
                            break;
                    }
                    break;

                default:
                    throw new Exception();
            }

            return null; // TODO what to return here?
        }
        
        

        protected override NodeValue VisitReference(ReferenceNode referenceNode)
        {
            /*
            if (referenceNode.ArrayIndexNode != null)
            {
                referenceNode.ArrayIndexValue = Visit(referenceNode.ArrayIndexNode);
            }
            #1#
            
            string nodeName = referenceNode.Name.ToUpper();
            if (!_symbolTable.ContainsKey(nodeName))
            {
                referenceNode.Annotations.Add(new UndeclaredIdentifierAnnotation());
                return new UndefinedValue();
            }
            
            SymbolContext symbolContext = _symbolTable[nodeName];

            switch (symbolContext.Node)
            {
                /*
                case ConstArrayDefinitionNode constArrayDefinitionNode:
                    if (referenceNode.ArrayIndexNode == null)
                    {
                        //orignal compiler actually accesses index 0 if no square brackets are provided
                        referenceNode.Annotations.Add(new SquareBracketsExpectedAnnotation());
                        return new UndefinedValue();
                    }
                    else
                    {
                        switch (referenceNode.ArrayIndexValue)
                        {
                            case UndefinedValue _:
                                return new UndefinedValue();
                            case IntValue intValue:
                                if (intValue.Value >= constArrayDefinitionNode.ElementNodes.Count)
                                {
                                    referenceNode.Annotations.Add(new IndexOutOfRangeAnnotation());
                                    return new UndefinedValue();
                                }
                                return Visit(constArrayDefinitionNode.ElementNodes[(int)intValue.Value]);
                            default:
                                referenceNode.ArrayIndexNode.Annotations.Add(new ConstIntegerExpectedAnnotation());
                                referenceNode.ArrayIndexValue = new UndefinedValue();
                                return new UndefinedValue();
                        }
                    }
                #1#

                case ConstDefinitionNode constDefinitionNode:
                    /*
                    if (referenceNode.ArrayIndexNode != null)
                    {
                        referenceNode.Annotations.Add(new SquareBracketsNotExpectedAnnotation());
                        return new UndefinedValue();
                    }
                    #1#
                    return Visit(constDefinitionNode.RightSideNode);
                
                case FunctionDefinitionNode _:
                    return new FunctionValue(symbolContext.Symbol);
                
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
        
        
        
        private DatSymbolType NodeValueToBuiltinType(NodeValue nodeValue)
        {
            switch (nodeValue)
            {
                case IntValue _:
                    return DatSymbolType.Int;
                
                case FloatValue _:
                    return DatSymbolType.Float;
                
                case StringValue _:
                    return DatSymbolType.String;

                case FunctionValue _:
                    return DatSymbolType.Func;
                
                default:
                    throw new Exception();
            }
        }

    }
}*/