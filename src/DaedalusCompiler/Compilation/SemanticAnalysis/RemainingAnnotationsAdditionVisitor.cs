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
        

        protected override void VisitPrototypeDefinition(PrototypeDefinitionNode node)
        {
            CheckStatementsForSingleExpressionHack(node.BodyNodes);
            base.VisitPrototypeDefinition(node);
        }

        protected override void VisitInstanceDefinition(InstanceDefinitionNode node)
        {
            CheckStatementsForSingleExpressionHack(node.BodyNodes);
            base.VisitInstanceDefinition(node);
        }

        protected override void VisitFunctionDefinition(FunctionDefinitionNode node)
        {
            CheckStatementsForSingleExpressionHack(node.BodyNodes);
            base.VisitFunctionDefinition(node);
        }

        protected override void VisitIfStatement(IfStatementNode node)
        {
            CheckStatementsForSingleExpressionHack(node.ElseNodeBodyNodes);
            base.VisitIfStatement(node);
        }

        protected override void VisitConditional(ConditionalNode node)
        {
            CheckStatementsForSingleExpressionHack(node.BodyNodes);
            base.VisitConditional(node);
        }

        protected override void VisitBreakStatement(BreakStatementNode node)
        {
            if (!IsStatementInsideLoop(node))
            {
                node.Annotations.Add(new IterationStatementNotInLoopError("break"));
            }
        }

        protected override void VisitContinueStatement(ContinueStatementNode node)
        {
            if (!IsStatementInsideLoop(node))
            {
                node.Annotations.Add(new IterationStatementNotInLoopError("continue"));
            }
        }

        protected override void VisitIntegerLiteral(IntegerLiteralNode node)
        {
            if (!node.EvaluatedCorrectly || node.Value < Int32.MinValue || node.Value > Int32.MaxValue)
            {
                node.Annotations.Add(new IntegerLiteralTooLargeError());
            }
        }

        private void CheckStatementsForSingleExpressionHack(List<StatementNode> statementNodes)
        {
            foreach (var statementNode in statementNodes)
            {
                switch (statementNode)
                {
                    case FunctionCallNode _:
                        break;
                    case ExpressionNode _:
                        statementNode.Annotations.Add(new SingleExpressionWarning());
                        break;
                }
            }
        }

        private bool IsStatementInsideLoop(ASTNode node)
        {
            while (node != null)
            {
                node = node.ParentNode;
                switch (node)
                {
                    case WhileStatementNode _:
                        return true;
                    case FileNode _:
                        return false;
                }
            }
            throw new Exception();
        }
    }
}