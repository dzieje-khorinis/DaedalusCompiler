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

        protected override void VisitIntegerLiteral(IntegerLiteralNode node)
        {
            if (node.EvaluatedCorrectly)
            {
                if (node.Value <= Int32.MaxValue && node.Value >= Int32.MinValue)
                {
                    return;
                }
            }
            node.Annotations.Add(new IntegerLiteralTooLargeError());
        }
    }
}