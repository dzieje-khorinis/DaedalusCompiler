using System;
using System.Collections.Generic;


namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    public class RemainingAnnotationsAdditionVisitor : AbstractSyntaxTreeBaseVisitor
    {
        private readonly HashSet<string> _initializedSymbolsPaths;
        
        private static readonly Dictionary<string, long> Class2RequiredSize = new Dictionary<string, long>
        {
            {"C_NPC", 800},
            {"C_INFO", 48},
            {"C_ITEMREACT", 28},
        };

        protected override void VisitCompoundAssignment(CompoundAssignmentNode node)
        {
            if (node.LeftSideNode.Symbol?.Node is ConstDefinitionNode)
            {
                node.Annotations.Add(new ConstValueChangedWarning(node.LeftSideNode.Name));
            }
            base.VisitCompoundAssignment(node);
        }

        protected override void VisitAssignment(AssignmentNode node)
        {
            ReferenceNode referenceNode = node.LeftSideNode;
            Symbol symbol = referenceNode.Symbol;
            
            if (symbol?.Node is ConstDefinitionNode)
            {
                node.Annotations.Add(new ConstValueChangedWarning(referenceNode.Name));
            } 
            
            base.VisitAssignment(node);
        }
        protected override void VisitClassDefinition(ClassDefinitionNode node)
        {
            string classNameUpper = node.NameNode.Value.ToUpper();
            if (!Class2RequiredSize.ContainsKey(classNameUpper))
            {
                return;
            }
            
            long size = 0;
            long requiredSize = Class2RequiredSize[classNameUpper];
            foreach (DeclarationNode attributeNode in node.AttributeNodes)
            {
                if (attributeNode is VarArrayDeclarationNode varArrayDeclarationNode)
                {
                    NodeValue arraySizeValue = varArrayDeclarationNode.ArraySizeValue;
                    if (arraySizeValue is IntValue intValue)
                    {
                        size += (attributeNode.Symbol.BuiltinType == SymbolType.String ? 20 : 4) * intValue.Value;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    size += (attributeNode.Symbol.BuiltinType == SymbolType.String ? 20 : 4);
                }
            }

            if (size != requiredSize)
            {
                node.Annotations.Add(new WrongClassSizeError(node.NameNode.Location, node.NameNode.Value, size, requiredSize));
            }
        }

        protected override void VisitConstDefinition(ConstDefinitionNode node) { }

        protected override void VisitConstArrayDefinition(ConstArrayDefinitionNode node) { }
        

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
            if (node.ElseNodeBodyNodes != null)
            {
                CheckStatementsForSingleExpressionHack(node.ElseNodeBodyNodes);
            }
            
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