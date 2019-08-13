using System;
using System.Collections.Generic;
using System.Linq;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    public class RemainingAnnotationsAdditionVisitor : AbstractSyntaxTreeBaseVisitor
    {
        /*
        protected override void Visit(ASTNode node)
        {
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
        }*/


        protected override void VisitConstDefinition(ConstDefinitionNode node)
        {
        }

        protected override void VisitConstArrayDefinition(ConstArrayDefinitionNode node)
        {
        }

        protected override void Visit(ASTNode node)
        {
            switch (node)
            {
                case FunctionCallNode _:
                    break;
                case ExpressionNode _:
                    if (!IsInsideStatement(node))
                    {
                        node.Annotations.Add(new SingleExpressionWarning());
                    }

                    break;
            }
            base.Visit(node);
        }
        
        protected override void VisitFunctionCall(FunctionCallNode node)
        {
            
        }

        protected override void VisitIntegerLiteral(IntegerLiteralNode node)
        {
            if (!node.EvaluatedCorrectly || node.Value < Int32.MinValue || node.Value > Int32.MaxValue)
            {
                node.Annotations.Add(new IntegerLiteralTooLargeError());
            }
        }


        private bool IsInsideStatement(ASTNode node)
        {
            while (node != null)
            {
                node = node.ParentNode;
                switch (node)
                {
                    case FileNode _:
                    case DeclarationNode _:
                        return false;
                    case StatementNode _:
                        return true;
                }
            }
            throw new Exception();
        }

    }
}