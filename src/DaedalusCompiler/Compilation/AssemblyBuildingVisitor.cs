using System;
using System.Collections.Generic;
using System.Linq;
using DaedalusCompiler.Compilation.SemanticAnalysis;

namespace DaedalusCompiler.Compilation
{
    public class LabelManager
    {
        private int _nextIfLabelIndex;
        private int _nextWhileLabelIndex;

        public LabelManager()
        {
            _nextIfLabelIndex = 0;
            _nextWhileLabelIndex = 0;
        }
        
        public string GetIfLabel()
        {
            return $"label_{_nextIfLabelIndex++}";
        }
        
        public string GetWhileLabel()
        {
            return $"label_while_{_nextWhileLabelIndex++}";
        }
    }
    
    public class AssemblyBuildingVisitor : AbstractSyntaxTreeBaseGenericVisitor<List<AssemblyElement>>
    {
        private Dictionary<string, Symbol> _symbolTable;
        private LabelManager _labelManager;

        public AssemblyBuildingVisitor(Dictionary<string, Symbol> symbolTable)
        {
            _symbolTable = symbolTable;
            _labelManager = new LabelManager();
        }


        protected override List<AssemblyElement> VisitFunctionDefinition(FunctionDefinitionNode node)
        {
            List<AssemblyElement> instructions = ((BlockSymbol)node.Symbol).Instructions;
            for (int i = node.ParameterNodes.Count - 1; i >= 0; i--)
            {
                ParameterDeclarationNode parameterNode = node.ParameterNodes[i];
                instructions.AddRange(Visit(parameterNode));
            }
            foreach (StatementNode bodyNode in node.BodyNodes)
            {
                instructions.AddRange(Visit(bodyNode));
            }
            instructions.Add(new Ret());
            return instructions;
        }

        protected override List<AssemblyElement> VisitInstanceDefinition(InstanceDefinitionNode node)
        {
            List<AssemblyElement> instructions = ((BlockSymbol)node.Symbol).Instructions;
            foreach (StatementNode bodyNode in node.BodyNodes)
            {
                instructions.AddRange(Visit(bodyNode));
            }
            return instructions;
        }

        protected override List<AssemblyElement> VisitPrototypeDefinition(PrototypeDefinitionNode node)
        {
            List<AssemblyElement> instructions = ((BlockSymbol)node.Symbol).Instructions;
            foreach (StatementNode bodyNode in node.BodyNodes)
            {
                instructions.AddRange(Visit(bodyNode));
            }
            return instructions;
        }

        protected override List<AssemblyElement> VisitConditional(ConditionalNode node)
        {
            return base.VisitConditional(node);
        }

        protected override List<AssemblyElement> VisitIfStatement(IfStatementNode node)
        {
            string statementEndLabel = _labelManager.GetIfLabel();
            string elseStartLabel = "";
            
            List<AssemblyElement> instructions = new List<AssemblyElement>();
            
            List<ConditionalNode> conditionalNodes = new List<ConditionalNode>();
            conditionalNodes.Add(node.IfNode);
            conditionalNodes.AddRange(node.ElseIfNodes);
            foreach (ConditionalNode conditionalNode in conditionalNodes)
            {
                instructions.AddRange(Visit(conditionalNode.ConditionNode));
                bool isLastIteration = conditionalNode == conditionalNodes.Last();
                if (isLastIteration)
                {
                    if (node.ElseNodeBodyNodes.Count > 0)
                    {
                        elseStartLabel = _labelManager.GetIfLabel();
                        instructions.Add(new JumpIfToLabel(elseStartLabel));
                        foreach (StatementNode bodyNode in conditionalNode.BodyNodes)
                        {
                            instructions.AddRange(Visit(bodyNode));
                        }
                        
                    }
                    else
                    {
                        instructions.Add(new JumpIfToLabel(statementEndLabel));
                        foreach (StatementNode bodyNode in conditionalNode.BodyNodes)
                        {
                            instructions.AddRange(Visit(bodyNode));
                        }
                    }
                }
                else
                {
                    string nextJumpLabel = _labelManager.GetIfLabel();
                    instructions.Add(new JumpIfToLabel(nextJumpLabel));
                    foreach (StatementNode bodyNode in conditionalNode.BodyNodes)
                    {
                        instructions.AddRange(Visit(bodyNode));
                    }
                    instructions.Add(new JumpToLabel(statementEndLabel));
                    instructions.Add(new AssemblyLabel(nextJumpLabel));
                }
            }

            if (node.ElseNodeBodyNodes != null)
            {
                instructions.Add(new AssemblyLabel(elseStartLabel));
                foreach (StatementNode bodyNode in node.ElseNodeBodyNodes)
                {
                    instructions.AddRange(Visit(bodyNode));
                }
            }
            instructions.Add(new AssemblyLabel(statementEndLabel));
            
            
            /*
             *
                public ConditionalNode IfNode;
                public List<ConditionalNode> ElseIfNodes;
                public List<StatementNode> ElseNodeBodyNodes;
             */

            return instructions;
            /*
             
             string statementEndLabel = GetNextLabel();
            string elseStartLabel = "";
            
            List<AssemblyElement> instructions = new List<AssemblyElement>();
            
            
            List<ConditionalBlockContext> conditionalBlocks = new List<ConditionalBlockContext>();
            
            conditionalBlocks.Add(IfBlock);
            conditionalBlocks.AddRange(ElseIfBlocks);
            
            foreach (var conditionalBlock in conditionalBlocks)
            {
                instructions.AddRange(conditionalBlock.Condition);
                
                bool isLastIteration = (conditionalBlock == conditionalBlocks.Last());
                if (isLastIteration)
                {
                    if (ElseBlock != null)
                    {
                        elseStartLabel = GetNextLabel();
                        instructions.Add(new JumpIfToLabel(elseStartLabel));
                        instructions.AddRange(conditionalBlock.Body);
                        instructions.Add(new JumpToLabel(statementEndLabel));
                    }
                    else
                    {
                        instructions.Add(new JumpIfToLabel(statementEndLabel));
                        instructions.AddRange(conditionalBlock.Body);
                    }
                }
                else
                {
                    string nextJumpLabel = GetNextLabel();
                    instructions.Add(new JumpIfToLabel(nextJumpLabel));
                    instructions.AddRange(conditionalBlock.Body);
                    instructions.Add(new JumpToLabel(statementEndLabel));
                    instructions.Add(new AssemblyLabel(nextJumpLabel));
                }
            }

            if (ElseBlock != null)
            {
                instructions.Add(new AssemblyLabel(elseStartLabel));
                instructions.AddRange(ElseBlock.Body);
            }

            instructions.Add(new AssemblyLabel(statementEndLabel));

            return instructions;
             */
        }

        protected override List<AssemblyElement> VisitWhileStatement(WhileStatementNode node)
        {
            string startLabel = _labelManager.GetWhileLabel();
            string endLabel = _labelManager.GetWhileLabel();

            List<AssemblyElement> instructions = new List<AssemblyElement>();
            instructions.Add(new AssemblyLabel(startLabel));
            instructions.AddRange(Visit(node.ConditionNode));
            instructions.Add(new JumpIfToLabel(endLabel));
            
            
            foreach (StatementNode bodyNode in node.BodyNodes)
            {
                List<AssemblyElement> bodyInstructions = Visit(bodyNode);
                foreach (AssemblyElement instruction in bodyInstructions)
                {
                    if (instruction is JumpToLoopStart)
                    {
                        instructions.Add(new JumpToLabel(startLabel));
                    }
                    else if (instruction is JumpToLoopEnd)
                    {
                        instructions.Add(new JumpToLabel(endLabel));
                    }
                    else
                    {
                        instructions.Add(instruction);
                    }
                }
            }

            instructions.Add(new AssemblyLabel(endLabel));
            return instructions;
        }

        protected override List<AssemblyElement> VisitParameterDeclaration(ParameterDeclarationNode node)
        {
            return new List<AssemblyElement>
            {
                new PushVar(node.Symbol),
                new Assign()
            };
        }

        protected override List<AssemblyElement> VisitParameterArrayDeclaration(ParameterArrayDeclarationNode node)
        {
            return new List<AssemblyElement>
            {
                new PushVar(node.Symbol),
                new Assign()
            };
        }

        protected override List<AssemblyElement> VisitReference(ReferenceNode referenceNode)
        {
            int index = -1;
            if (referenceNode.IndexNode != null)
            {
                index = (int) ((IntValue) referenceNode.IndexNode.Value).Value;
            }

            if (index > 0)
            {
                return new List<AssemblyElement> { new PushArrayVar(referenceNode.Symbol, index) };
            }
            return new List<AssemblyElement> { new PushVar(referenceNode.Symbol) };
        }

        protected override List<AssemblyElement> VisitIntegerLiteral(IntegerLiteralNode node)
        {
            return new List<AssemblyElement>{ new PushInt((int) node.Value) };
        }

        protected override List<AssemblyElement> VisitFloatLiteral(FloatLiteralNode node)
        {
            float floatValue = node.Value;
            int intValue = BitConverter.ToInt32(BitConverter.GetBytes(floatValue), 0);
            return new List<AssemblyElement>{ new PushInt(intValue) };
        }

        protected override List<AssemblyElement> VisitStringLiteral(StringLiteralNode node)
        {
            return new List<AssemblyElement> { new PushVar(node.Symbol) };
        }

        protected override List<AssemblyElement> VisitAssignment(AssignmentNode node)
        {
            List<AssemblyElement> instructions = new List<AssemblyElement>();
            instructions.AddRange(Visit(node.RightSideNode));
            instructions.AddRange(Visit(node.LeftSideNode));
            instructions.Add(new Assign());
            return instructions;
        }

        protected override List<AssemblyElement> VisitCompoundAssignment(CompoundAssignmentNode node)
        {
            List<AssemblyElement> instructions = new List<AssemblyElement>();
            instructions.AddRange(Visit(node.RightSideNode));
            instructions.AddRange(Visit(node.LeftSideNode));
            switch (node.Operator)
            {
                case CompoundAssignmentOperator.Add:
                    instructions.Add(new AssignAdd());
                    break;
                case CompoundAssignmentOperator.Sub:
                    instructions.Add(new AssignSubtract());
                    break;
                case CompoundAssignmentOperator.Mult:
                    instructions.Add(new AssignMultiply());
                    break;
                case CompoundAssignmentOperator.Div:
                    instructions.Add(new AssignDivide());
                    break;
            }
            return instructions;
        }

        protected override List<AssemblyElement> VisitBinaryExpression(BinaryExpressionNode node)
        {
            List<AssemblyElement> instructions = new List<AssemblyElement>();
            instructions.AddRange(Visit(node.RightSideNode));
            instructions.AddRange(Visit(node.LeftSideNode));

            switch (node.Operator)
            {
                case BinaryOperator.Mult:
                    instructions.Add(new Multiply());
                    break;
                case BinaryOperator.Div:
                    instructions.Add(new Divide());
                    break;
                case BinaryOperator.Modulo:
                    instructions.Add(new Modulo());
                    break;
                
                case BinaryOperator.Add:
                    instructions.Add(new Add());
                    break;
                case BinaryOperator.Sub:
                    instructions.Add(new Subtract());
                    break;
                
                case BinaryOperator.ShiftLeft:
                    instructions.Add(new ShiftLeft());
                    break;
                case BinaryOperator.ShiftRight:
                    instructions.Add(new ShiftRight());
                    break;
                
                case BinaryOperator.Less:
                    instructions.Add(new Less());
                    break;
                case BinaryOperator.Greater:
                    instructions.Add(new Greater());
                    break;
                case BinaryOperator.LessOrEqual:
                    instructions.Add(new LessOrEqual());
                    break;
                case BinaryOperator.GreaterOrEqual:
                    instructions.Add(new GreaterOrEqual());
                    break;
                
                case BinaryOperator.Equal:
                    instructions.Add(new Equal());
                    break;
                case BinaryOperator.NotEqual:
                    instructions.Add(new NotEqual());
                    break;
                
                case BinaryOperator.BinAnd:
                    instructions.Add(new BitAnd());
                    break;
                case BinaryOperator.BinOr:
                    instructions.Add(new BitOr());
                    break;
                case BinaryOperator.LogAnd:
                    instructions.Add(new LogAnd());
                    break;
                case BinaryOperator.LogOr:
                    instructions.Add(new LogOr());
                    break;
            }
            return instructions;
        }

        protected override List<AssemblyElement> VisitUnaryExpression(UnaryExpressionNode node)
        {
            List<AssemblyElement> instructions = new List<AssemblyElement>();
            instructions.AddRange(Visit(node.ExpressionNode));
            switch (node.Operator)
            {
                case UnaryOperator.Minus:
                    instructions.Add(new Minus());
                    break;
                case UnaryOperator.Not:
                    instructions.Add(new Not());
                    break;
                case UnaryOperator.Negate:
                    instructions.Add(new Negate());
                    break;
                case UnaryOperator.Plus:
                    instructions.Add(new Plus());
                    break;
            }
            return instructions;
        }

        protected override List<AssemblyElement> VisitReturnStatement(ReturnStatementNode node)
        {
            return new List<AssemblyElement>{ new Ret() };
        }

        protected override List<AssemblyElement> VisitBreakStatement(BreakStatementNode node)
        {
            return new List<AssemblyElement> { new JumpToLoopEnd() };
        }

        protected override List<AssemblyElement> VisitContinueStatement(ContinueStatementNode node)
        {
            return new List<AssemblyElement> { new JumpToLoopStart() };
        }
        
        protected override List<AssemblyElement> VisitFunctionCall(FunctionCallNode node)
        {
            List<AssemblyElement> instructions = new List<AssemblyElement>();
            foreach (ExpressionNode argumentNode in node.ArgumentNodes)
            {
                instructions.AddRange(Visit(argumentNode));
            }
            instructions.Add(new Call(node.FunctionReferenceNode.Symbol));
            return instructions;
        }

        protected override List<AssemblyElement> VisitVarDeclaration(VarDeclarationNode node)
        {
            return new List<AssemblyElement>();
        }

        protected override List<AssemblyElement> VisitVarArrayDeclaration(VarArrayDeclarationNode node)
        {
            return new List<AssemblyElement>();
        }

        protected override List<AssemblyElement> VisitConstDefinition(ConstDefinitionNode node)
        {
            return new List<AssemblyElement>();
        }

        protected override List<AssemblyElement> VisitConstArrayDefinition(ConstArrayDefinitionNode node)
        {
            return new List<AssemblyElement>();
        }

        protected override List<AssemblyElement> VisitClassDefinition(ClassDefinitionNode node)
        {
            return new List<AssemblyElement>();
        }
    }
}