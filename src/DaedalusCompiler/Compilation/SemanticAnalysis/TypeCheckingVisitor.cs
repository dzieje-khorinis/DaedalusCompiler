using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{

    class SymbolTypePair
    {
        public SymbolType BuiltinType;
        public Symbol ComplexType;

        public SymbolTypePair(SymbolType builtinType, Symbol complexType)
        {
            BuiltinType = builtinType;
            ComplexType = complexType;
        }
    }
    
    public class TypeCheckingVisitor : AbstractSyntaxTreeBaseVisitor
    {
        private readonly Dictionary <string, Symbol> _symbolTable;
        
        /*
         * arguments vs parameters types
         * operations types
         * assignment / compound assignment type
         * return type
         */
        //private Dictionary<ASTNode, SymbolType>
        private readonly HashSet<ASTNode> _visitedNodes;

        public TypeCheckingVisitor(Dictionary<string, Symbol> symbolTable)
        {
            _symbolTable = symbolTable;
            _visitedNodes = new HashSet<ASTNode>();
        }

        
        private SymbolTypePair GetSymbolTypePairFromExpressionNode(ExpressionNode expressionNode)
        {
            return new SymbolTypePair(expressionNode.BuiltinType, expressionNode.ComplexType);
        }
        
        private SymbolTypePair GetSymbolTypePairFromSymbol(Symbol symbol)
        {
            if (symbol is NestableSymbol nestableSymbol)
            {
                return new SymbolTypePair(nestableSymbol.BuiltinType, nestableSymbol.ComplexType);
            }
            return new SymbolTypePair(symbol.BuiltinType, null);
        }


        protected override void Visit(ASTNode node)
        {
            if (!_visitedNodes.Contains(node))
            {
                _visitedNodes.Add(node);
                base.Visit(node);
            }
        }

        protected override void VisitFunctionCall(FunctionCallNode node)
        {
            if (node.FunctionReferenceNode.Symbol != null)
            {
                FunctionDefinitionNode functionDefinitionNode = (FunctionDefinitionNode) node.FunctionReferenceNode.Symbol.Node;
                Symbol symbol = functionDefinitionNode.Symbol;

                if (symbol != null)
                {
                    node.BuiltinType = symbol.BuiltinType;
                    if (symbol is NestableSymbol nestableSymbol)
                    {
                        node.ComplexType = nestableSymbol.ComplexType;
                    }
                }

                string identifier = functionDefinitionNode.NameNode.Value;

                List<ParameterDeclarationNode> parameterNodes = functionDefinitionNode.ParameterNodes;
                List<ExpressionNode> argumentNodes = node.ArgumentNodes;
                
                int parametersCount = parameterNodes.Count;
                int argumentsCount = argumentNodes.Count;
            
                if (parametersCount != argumentsCount)
                {
                    node.Annotations.Add(new ArgumentsCountDoesNotMatchError(identifier, parametersCount, argumentsCount, functionDefinitionNode.NameNode.Location));
                }

                int iterationsCount = Math.Min(parametersCount, argumentsCount);
                for(int i=0; i<iterationsCount; i++)
                {
                    ParameterDeclarationNode parameterNode = parameterNodes[i];
                    SymbolTypePair parameterType = GetSymbolTypePairFromSymbol(parameterNode.Symbol);
                    
                    ExpressionNode argumentNode = argumentNodes[i];
                    Visit(argumentNode);
                    SymbolTypePair argumentType = GetSymbolTypePairFromExpressionNode(argumentNode);
                    
                    Console.Write("x");
                }
            }
        }
        
        
        protected override void VisitUnaryExpression(UnaryExpressionNode node)
        {
            base.VisitUnaryExpression(node);
            
            switch (node.ExpressionNode.BuiltinType)
            {
                case SymbolType.Uninitialized:
                case SymbolType.Undefined:
                    node.BuiltinType = SymbolType.Undefined;
                    break;
                
                case SymbolType.Int:
                    node.BuiltinType = SymbolType.Int;
                    // TODO think about changing !int type to bool
                    break;
                
                case SymbolType.Float:
                    switch (node.Operator)
                    {
                        case UnaryOperator.Plus:
                        case UnaryOperator.Minus:
                        case UnaryOperator.Not:
                            node.BuiltinType = SymbolType.Float;
                            break;
                        default:
                            node.BuiltinType = SymbolType.Undefined;
                            node.Annotations.Add(new InvalidArgumentTypeToUnaryExpressionError(node.ExpressionNode.BuiltinType.ToString()));
                            break;
                    }
                    break;
                
                default:
                    node.BuiltinType = SymbolType.Undefined;
                    node.Annotations.Add(new InvalidArgumentTypeToUnaryExpressionError(node.ExpressionNode.BuiltinType.ToString()));
                    break;
            }
            
        }

        protected override void VisitBinaryExpression(BinaryExpressionNode node)
        {
            base.VisitBinaryExpression(node);

            /*
                     Uninitialized = -1,
        Void = 0,
        Float = 1,
        Int = 2,
        String = 3,
        Class = 4,
        Func = 5,
        Prototype = 6,
        Instance = 7,
        Undefined = 8,
             */
            switch (node.LeftSideNode.BuiltinType)
            {
                case SymbolType.Uninitialized:
                case SymbolType.Undefined:
                    node.BuiltinType = SymbolType.Undefined;
                    break;
                
                case SymbolType.Int:
                    switch (node.RightSideNode.BuiltinType)
                    {
                        case SymbolType.Uninitialized:
                        case SymbolType.Undefined:
                            node.BuiltinType = SymbolType.Undefined;
                            break;
                        
                        case SymbolType.Int:
                            node.BuiltinType = SymbolType.Int;
                            break;
                        
                        case SymbolType.String:
                            switch (node.Operator)
                            {
                                case BinaryOperator.Equal:
                                    // it will be ignorecase since HLP_StrCmp is ignorecase
                                    if (_symbolTable.ContainsKey("HLP_StrCmp"))
                                    {
                                        node.BuiltinType = SymbolType.Int;
                                    }
                                    else
                                    {
                                        node.BuiltinType = SymbolType.Undefined;
                                        //ANNOTATION
                                    }
                                    break;
                                case BinaryOperator.Add:
                                    if (_symbolTable.ContainsKey("ConcatStrings"))
                                    {
                                        node.BuiltinType = SymbolType.String;
                                    }
                                    else
                                    {
                                        node.BuiltinType = SymbolType.Undefined;
                                        //ANNOTATION
                                    }
                                    break;
                                default:
                                    node.BuiltinType = SymbolType.Undefined;
                                    // ANNOTATION
                                    break;
                            }
                            
                            break;
                        
                        default:
                            //annotation
                            node.BuiltinType = SymbolType.Undefined;
                            break;
                    }
                    break;

                case SymbolType.Float:
                    switch (node.RightSideNode.BuiltinType)
                    {
                        case SymbolType.Uninitialized:
                        case SymbolType.Undefined:
                            node.BuiltinType = SymbolType.Undefined;
                            break;
                        
                        default:
                            //annotation
                            node.BuiltinType = SymbolType.Undefined;
                            break;
                    }
                    break;
                
                case SymbolType.String:

                    switch (node.RightSideNode.BuiltinType)
                    {
                        //  TODO allow str * int, ConcatStrings required, loop used
                        case SymbolType.Uninitialized:
                        case SymbolType.Undefined:
                            node.BuiltinType = SymbolType.Undefined;
                            break;
                        
                        case SymbolType.String:
                            switch (node.Operator)
                            {
                                case BinaryOperator.Equal:
                                    // it will be ignorecase since HLP_StrCmp is ignorecase
                                    if (_symbolTable.ContainsKey("HLP_StrCmp"))
                                    {
                                        node.BuiltinType = SymbolType.Int;
                                    }
                                    else
                                    {
                                        node.BuiltinType = SymbolType.Undefined;
                                        //ANNOTATION
                                    }
                                    break;
                                case BinaryOperator.Add:
                                    if (_symbolTable.ContainsKey("ConcatStrings"))
                                    {
                                        node.BuiltinType = SymbolType.String;
                                    }
                                    else
                                    {
                                        node.BuiltinType = SymbolType.Undefined;
                                        //ANNOTATION
                                    }
                                    break;
                                default:
                                    node.BuiltinType = SymbolType.Undefined;
                                    // ANNOTATION
                                    break;
                            }
                            
                            break;
                        
                        default:
                            node.BuiltinType = SymbolType.Undefined;
                            // ANNOTATION
                            break;
                    }
                    
                        

                    break;
                
                default:
                    //annotation
                    node.BuiltinType = SymbolType.Undefined;
                    break;
            }
            
        }

        protected override void VisitReference(ReferenceNode referenceNode)
        {
            base.VisitReference(referenceNode);
            
            Symbol symbol = referenceNode.Symbol;
            if (symbol != null)
            {
                if (symbol is FunctionSymbol functionSymbol)
                {
                    referenceNode.BuiltinType = SymbolType.Func;
                    return;
                }
                
                if (symbol.BuiltinType == SymbolType.Uninitialized && symbol is NestableSymbol tmpSymbol && tmpSymbol.ComplexType is ClassSymbol)
                {
                    symbol.BuiltinType = SymbolType.Instance;
                }
                
                referenceNode.BuiltinType = symbol.BuiltinType;
                if (symbol is NestableSymbol nestableSymbol)
                {
                    referenceNode.ComplexType = nestableSymbol.ComplexType;
                }
            }
        }

        protected override void VisitIntegerLiteral(IntegerLiteralNode node)
        {
            node.BuiltinType = SymbolType.Int;
        }

        protected override void VisitFloatLiteral(FloatLiteralNode node)
        {
            node.BuiltinType = SymbolType.Float;
        }

        protected override void VisitStringLiteral(StringLiteralNode node)
        {
            node.BuiltinType = SymbolType.String;
        }

        protected override void VisitNoFunc(NoFuncNode node)
        {
            node.BuiltinType = SymbolType.Func; // TODO maybe int?
        }

        protected override void VisitNull(NullNode node)
        {
            node.BuiltinType = SymbolType.Int;
        }
/*
         *
         *         Uninitialized = -1,
        Void = 0,
        Float = 1,
        Int = 2,
        String = 3,
        Class = 4,
        Func = 5,
        Prototype = 6,
        Instance = 7,
         */

        protected override void VisitConstDefinition(ConstDefinitionNode node) { }
        protected override void VisitConstArrayDefinition(ConstArrayDefinitionNode node) { }



        
    }
}