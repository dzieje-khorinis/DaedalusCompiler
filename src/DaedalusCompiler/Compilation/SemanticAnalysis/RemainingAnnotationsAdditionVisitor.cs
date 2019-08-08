using System;
using System.Collections.Generic;
using System.Linq;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    public class RemainingAnnotationsAdditionVisitor : AbstractSyntaxTreeBaseVisitor
    {
        protected override void Visit(ASTNode node)
        {
            /*
            if (_visitedNodesValuesCache.ContainsKey(node))
            {
                NodeValue nodeValue = _visitedNodesValuesCache[node];
                if (nodeValue is ErrorValue)
                {
                    string errorValueType = nodeValue.GetType().ToString().Split(".").Last();
                    
                    string message = node.GetType().ToString().Split(".").Last();
                    switch (node)
                    {
                        case ConstDefinitionNode constDefinitionNode:
                            message = $"{message} {constDefinitionNode.NameNode.Value}";
                            break;
                        case ReferenceNode referenceNode:
                            message = $"{message} {referenceNode.Name}";
                            break;
                    }
                    Console.WriteLine($"{message}: {errorValueType}");
                }
            }
            */
            
            
            if (node.Annotations.Count > 0)
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
                    case AttributeNode attributeNode:
                        message = $"{message} {attributeNode.Name}";
                        break;
                }

                Console.WriteLine($"{message}");
                foreach (var annotation in node.Annotations)
                {
                    Console.WriteLine(annotation.GetType());
                }
            }
            
           
            
            base.Visit(node);
            

        }
        
        
        /*
        protected override void VisitConstDefinition(ConstDefinitionNode node)
        {
            Console.WriteLine($"VisitConstDefinition {node.NameNode.Value}");
            foreach (var annotation in node.Annotations)
            {
                Console.WriteLine(annotation.GetType());
            }

            base.VisitConstDefinition(node);
        }

        protected override void VisitReference(ReferenceNode referenceNode)
        {
            Console.WriteLine($"VisitReference {referenceNode.Name}");
            foreach (var annotation in referenceNode.Annotations)
            {
                Console.WriteLine(annotation.GetType());
            }
            
            base.VisitReference(referenceNode);
        }
*/
        
        
        
        
        
        /*
        protected override void VisitBinaryExpression(BinaryExpressionNode binaryExpressionNode)
        {
            DatSymbolType leftType = binaryExpressionNode.LeftSideNode.BuiltinType;
            DatSymbolType rightType = binaryExpressionNode.RightSideNode.BuiltinType;
            
            if ((leftType == DatSymbolType.Int || leftType == DatSymbolType.Instance)
                && (rightType == DatSymbolType.Int || rightType == DatSymbolType.Instance))
            {
                
                
                 // TODO: original compiler didn't allow to have lhs or rhs to be functionCall of type Instance,
                 
                //if ((leftType == DatSymbolType.Instance && binaryExpressionNode.LeftSideNode is FunctionCallNode)
                //    || (rightType == DatSymbolType.Instance && binaryExpressionNode.RightSideNode is FunctionCallNode))
                //{
                //    binaryExpressionNode.BuiltinType = DatSymbolType.Undefined;
                //}
                
                binaryExpressionNode.BuiltinType = DatSymbolType.Int;
            }
            else
            {
                binaryExpressionNode.BuiltinType = DatSymbolType.Undefined;
            }
        }

        protected override void VisitUnaryExpression(UnaryExpressionNode node)
        {
            //if (node.)
            
            
            
             
             switch (lValueType)
            {
                case DatSymbolType.Int:
                case DatSymbolType.Instance:
                    if (rValue.Type == DatSymbolType.Int)
                    {
                        return Compability.Full;
                    }
                    
                    if (rValue.Type == DatSymbolType.Instance && rValue.Category == DataCategory.Variable)
                    {
                        // TODO, check if this is worth supporting
                        return Compability.Full;
                    }
                    
                    break;
            }
            
            return Compability.None;
             
        }
    
     */
        
        
    }
}