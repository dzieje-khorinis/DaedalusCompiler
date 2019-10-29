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

        private int _floatExpressionDepth;
        private bool _isInsideFloatExpression;
        
        
        
        private bool _isInsideConditional;

        private Stack<SymbolTypePair> _currentExpressionTypes; //return/assignment/parameter
        
        
        /*
         * arguments vs parameters types
         * operations types
         * assignment / compound assignment type
         * return type
         * ONLY FLOAT/INTEGER LITERALS ALLOWED INSIDE FLOAT EXPRESSIONS
         */
        //private Dictionary<ASTNode, SymbolType>
        private readonly HashSet<ASTNode> _visitedNodes;

        public TypeCheckingVisitor(Dictionary<string, Symbol> symbolTable)
        {
            _symbolTable = symbolTable;
            _visitedNodes = new HashSet<ASTNode>();
            //_floatExpressionDepth = 0;
            //_isInsideFloatExpression = false;

            _isInsideConditional = false;
            _currentExpressionTypes = new Stack<SymbolTypePair>();
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

                    if (parameterType.BuiltinType == SymbolType.Float)
                    {
                        _isInsideFloatExpression = true;
                    }

                    _currentExpressionTypes.Push(parameterType);
                    Visit(argumentNode);
                    _currentExpressionTypes.Pop();
                    
                    
                    _isInsideFloatExpression = false;
                    
                    SymbolTypePair argumentType = GetSymbolTypePairFromExpressionNode(argumentNode);
                    
                    Console.Write("x");
                }
            }
        }

        protected override void VisitReturnStatement(ReturnStatementNode node)
        {
            ASTNode parentBlockNode = node.GetFirstSignificantAncestorNode();
            FunctionDefinitionNode functionDefinitionNode = (FunctionDefinitionNode) parentBlockNode;
            SymbolTypePair functionType = GetSymbolTypePairFromSymbol(functionDefinitionNode.Symbol);
            

            /*
            if (functionDefinitionNode.Symbol.BuiltinType == SymbolType.Float)
            {
                _isInsideFloatExpression = true;
            }
            */

            _currentExpressionTypes.Push(functionType);
            base.VisitReturnStatement(node);
            _currentExpressionTypes.Pop();
            
            //_isInsideFloatExpression = false;
                

            
        }

        protected override void VisitAssignment(AssignmentNode node)
        {
            Visit(node.LeftSideNode);
            
            SymbolTypePair assignmentType = GetSymbolTypePairFromSymbol(node.LeftSideNode.Symbol);
            
            /*
            if (node.LeftSideNode.BuiltinType == SymbolType.Float)
            {
                _isInsideFloatExpression = true;
            }
            */

            _currentExpressionTypes.Push(assignmentType);
            Visit(node.RightSideNode);
            _currentExpressionTypes.Pop();

            //_isInsideFloatExpression = false;

            /*
             * IncompatibleTypeAssignmentError
             * TODO check assignment types compability
             */
        }

        protected override void VisitCompoundAssignment(CompoundAssignmentNode node)
        {
            Visit(node.LeftSideNode);
            switch (node.LeftSideNode.BuiltinType)
            {
                case SymbolType.Float:
                    node.Annotations.Add(new FloatDoesntSupportCompoundAssignments(node.OperatorLocation));
                    return;
                case SymbolType.Class:
                case SymbolType.Prototype:    
                case SymbolType.Instance:
                case SymbolType.Func:
                    node.Annotations.Add(new UnsupportedOperationError(node.OperatorLocation));
                    return;
                case SymbolType.String:
                    if (node.Operator != CompoundAssignmentOperator.Add || !_symbolTable.ContainsKey("CONCATSTRINGS")) {
                        node.Annotations.Add(new UnsupportedOperationError(node.OperatorLocation));
                        return;
                    }
                    break;
            }
            
            Visit(node.RightSideNode);
            
            /*
             * InvalidOperandsToBinaryExpressionError
             * TODO check assignment types compability
             */
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
            if (_isInsideFloatExpression)
            {
                node.Annotations.Add(new BinaryOperationsNotAllowedInsideFloatExpression(node.OperatorLocation));
                return;
            }
            
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
                                    if (_symbolTable.ContainsKey("HLP_STRCMP"))
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
                                    if (_symbolTable.ContainsKey("CONCATSTRINGS"))
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
                        //  TODO allow str * int, CONCATSTRINGS required, loop used
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
                                    if (_symbolTable.ContainsKey("CONCATSTRINGS"))
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
                
                
                
                if (_currentExpressionTypes.Count > 0)
                {
                    SymbolTypePair currentExpressionType = _currentExpressionTypes.Peek();
                    
                    SymbolType asType = currentExpressionType.BuiltinType;
                    if (asType == SymbolType.Func || (asType == SymbolType.Int && symbol.BuiltinType != SymbolType.Int)) // || _isInsideConditional but not always
                    {
                        referenceNode.CastToInt = true;
                    }
                }
                else
                {
                    if (_isInsideConditional)
                    {
                        referenceNode.CastToInt = true;
                    }
                }
                
                
                
                
                if (symbol is FunctionSymbol)
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


        protected override void VisitConditional(ConditionalNode node)
        {
            _isInsideConditional = true;
            base.VisitConditional(node);
            _isInsideConditional = false;
        }
    }
}