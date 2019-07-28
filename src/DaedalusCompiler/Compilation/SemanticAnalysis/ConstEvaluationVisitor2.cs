/*using System;
using System.Collections.Generic;
using System.Linq;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    public class ConstEvaluationVisitor2 : AbstractSyntaxTreeBaseVisitor
    {
        /*
         // a może się powinien nazywać Evaluate References?
         
         Ten visitor będzie wywoływany z AnnotationsAdditionVisitora czy moze lepiej nie?
         
         // evaluate array sizes and constants
         
         #1#
        //private const int A = B;
        //private const int B = Z + A + X;
        //private const int C = B + 2;
        
        //private const int A = B;
        //private const int B = C;
        //private const int C = B;

        /*
         *
         const int a = b;
         const int b = c + 5;
         const int c = 10;
         
         
         EDGE CASE:
         a)
         func void myFunc() {
             const int a = b;
             const int b = c + 5;
             const int c = 10;
         }
         
         0)
         const int a = 5;
         func void myFunc() {
            const int a = a;
         }
         
         1)
         
         const int a = b;
                       ^
                       error: cycle in constant value computation
         const int b = a;
                       ^
                       error: cycle in constant value computation
         
         2)
         
         const int a = b;
                       ^
                       error: constant initializer must be compile-time constant
         const int b = c + a;
    
         
         3)
         
         const int a = b;
                       ^
                       error: cycle in constant value computation
         const int b = a + d;
                       ^ error: cycle in constant value computation
                           ^ cannot resolve symbol d
         
         
         
         2)
         const int a = a;
         #1#
        private readonly Dictionary <string, SymbolContext> _symbolTable;
        
        private HashSet<ASTNode> _visitedNodes;
        private Dictionary<ASTNode, NodeValue> _nodesValues;
        
        
        public ConstEvaluationVisitor2(Dictionary <string, SymbolContext> symbolTable)
        {
            _symbolTable = symbolTable;
            _visitedNodes = new HashSet<ASTNode>();
            _nodesValues = new Dictionary<ASTNode, NodeValue>();
        }
        
        protected override void VisitConstDefinition(ConstDefinitionNode node)
        {
            node.Symbol.Content = new object[] {GetNodeValue(node.RightSideNode)};
            Console.WriteLine("aha");
        }
        
        
        protected override void VisitReference(ReferenceNode referenceNode)
        {
            // TODO: Add support of nested references (i.e. `a.b`)
            string nodeName = referenceNode.Name.ToUpper();

            if (!_symbolTable.ContainsKey(nodeName))
            {
                referenceNode.Annotations.Add(new TextAnnotation($"Cannot resolve symbol {nodeName}"));
                _nodesValues[referenceNode] = new HandledValue();
                //referenceNode.Symbol = UndefinedSymbol
                return;
            }
            
            SymbolContext symbolContext = _symbolTable[nodeName];
            
            switch (symbolContext.Node)
            {
                /*
                case ConstArrayDefinitionNode constArrayDefinitionNode:
                    throw new UnableToEvaluateException();
                #1#
                
                case ConstDefinitionNode constDefinitionNode:
                    NodeValue nodeValue = GetConstDefinitionValue(constDefinitionNode);
                    _nodesValues[referenceNode] = nodeValue;
                    break;
                
                default:
                    throw new UnableToEvaluateException();
            }
            
            /*
            if (referenceNode.ArrayIndexNode != null)
            {
                NodeValue arrayIndex = GetNodeValue(referenceNode.AttributeNode);
                switch (symbolContext.Node)
                {
                    case ConstArrayDefinitionNode constArrayDefinitionNode:
                        break;
                    default:
                        throw new UnableToEvaluateException();
                }
            }
            else
            {
                switch (symbolContext.Node)
                {
                    case ConstArrayDefinitionNode constArrayDefinitionNode:
                        throw new UnableToEvaluateException();
                
                    case ConstDefinitionNode constDefinitionNode:
                        break;
                
                    default:
                        throw new UnableToEvaluateException();
                }

            }
            #1#
        }

        

        
        /*
        protected override void VisitConstArrayDefinition(ConstArrayDefinitionNode node)
        {
            //GetV
        }
        #1#

        protected override void VisitBinaryExpression(BinaryExpressionNode node)
        {
            if (_nodesValues.ContainsKey(node))
            {
                return;
            }
            
            Visit(node.LeftSideNode);
            Visit(node.RightSideNode);
            
            _nodesValues[node] = EvaluationHelper.EvaluateBinaryOperation(node.Operator, _nodesValues[node.LeftSideNode],_nodesValues[node.RightSideNode]);
        }

        protected override void VisitUnaryExpression(UnaryExpressionNode node)
        {
            if (_nodesValues.ContainsKey(node))
            {
                return;
            }
            
            Visit(node.ExpressionNode);

            _nodesValues[node] = EvaluationHelper.EvaluateUnaryOperation(node.Operator, _nodesValues[node.ExpressionNode]);
        }
        

        private object GetObjectValue(ASTNode node)
        {
            NodeValue nodeValue = GetNodeValue(node);
            switch (nodeValue)
            {
                case IntValue intValue:
                    return intValue.Value;
                case FloatValue floatValue:
                    return floatValue.Value;
                case StringValue stringValue:
                    return stringValue.Value;
                default:
                    return null;
            }
        }
        private NodeValue GetNodeValue(ASTNode node)
        {
            if (node == null)
            {
                throw new Exception();
            }
            
            if (_visitedNodes.Contains(node))
            {
                if (_nodesValues.ContainsKey(node))
                {
                    return _nodesValues[node];
                }

                return new InfiniteReferenceLoopValue(true);
            }

            _visitedNodes.Add(node);
            Visit(node);
            return _nodesValues[node];
        }
        
        

        private NodeValue GetIndex(ReferenceNode referenceNode)
        {
            if (referenceNode.ArrayIndexNode != null)
            {
                return GetNodeValue(referenceNode.ArrayIndexNode);
            }

            return null;
        }
        
        /*
        private NodeValue GetArraySize(IArrayDeclarationNode arrayDeclarationNode)
        {
            return GetNodeValue(arrayDeclarationNode.ArraySizeNode);
        }
        #1#

        private NodeValue GetConstDefinitionValue(ConstDefinitionNode constDefinitionNode)
        {
            return GetNodeValue(constDefinitionNode.RightSideNode);
        }
        
        /*
        private NodeValue GetConstArrayDefinitionElementValue(ConstArrayDefinitionNode constArrayDefinitionNode, int index)
        {
            int arraySize = GetArraySize(constArrayDefinitionNode);
            if (index >= arraySize || index >= constArrayDefinitionNode.ElementNodes.Count)
            {
                return new UndefinedValue();
            }

            return GetNodeValue(constArrayDefinitionNode.ElementNodes[index]);
        }
        #1#
        
        
        protected override void VisitFloatLiteral(FloatLiteralNode node)
        {
            _nodesValues[node] = new FloatValue(node.Value);
        }

        protected override void VisitIntegerLiteral(IntegerLiteralNode node)
        {
            _nodesValues[node] = new IntValue(node.Value);
        }

        protected override void VisitStringLiteral(StringLiteralNode node)
        {
            _nodesValues[node] = new StringValue(node.Value);
        }
        
    }
}*/