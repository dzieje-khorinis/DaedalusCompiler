using System.Collections.Generic;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    public abstract class AbstractSyntaxTreeBaseVisitor
    {
        public virtual void VisitFile(FileNode node) {
            Visit(node.DefinitionNodes);
        }

        public virtual void VisitConditional(ConditionalNode node)
        {
            Visit(node.ConditionNode);
            Visit(node.BodyNodes);
        }

        public virtual void VisitFunctionDefinition(FunctionDefinitionNode node)
        {
            Visit(node.NameNode);
            Visit(node.ParameterNodes);
            Visit(node.BodyNodes);
        }

        public virtual void VisitAssignment(AssignmentNode node)
        {
            Visit(node.LeftSideNode);
            Visit(node.RightSideNode);
        }

        public virtual void VisitCompoundAssignment(CompoundAssignmentNode node)
        {
            Visit(node.LeftSideNode);
            Visit(node.RightSideNode);
        }
        
        public virtual void VisitUnaryExpression(UnaryExpressionNode node)
        {
            Visit(node.ExpressionNode);
        }

        public virtual void VisitBinaryExpression(BinaryExpressionNode node)
        {
            Visit(node.LeftSideNode);
            Visit(node.RightSideNode);
        }

        public virtual void VisitClassDefinition(ClassDefinitionNode node)
        {
            Visit(node.AttributeNodes);
        }

        public virtual void VisitPrototypeDefinition(PrototypeDefinitionNode node)
        {
            Visit(node.NameNode);
            Visit(node.ParentReferenceNode);
            Visit(node.BodyNodes);
        }

        public virtual void VisitInstanceDefinition(InstanceDefinitionNode node)
        {
            Visit(node.NameNode);
            Visit(node.ParentReferenceNode);
            Visit(node.BodyNodes);
        }

        public virtual void VisitConstDefinition(ConstDefinitionNode node)
        {
            Visit(node.NameNode);
            Visit(node.RightSideNode);
        }

        public virtual void VisitConstArrayDefinition(ConstArrayDefinitionNode node)
        {
            Visit(node.NameNode);
            Visit(node.ArraySizeNode);
            Visit(node.ElementNodes);
        }

        public virtual void VisitVarDeclaration(VarDeclarationNode node)
        {
            Visit(node.NameNode);
        }

        public virtual void VisitVarArrayDeclaration(VarArrayDeclarationNode node)
        {
            Visit(node.NameNode);
            Visit(node.ArraySizeNode);
        }
        public virtual void VisitReturnStatement(ReturnStatementNode node) {}
        public virtual void VisitBreakStatement(BreakStatementNode node) {}
        public virtual void VisitContinueStatement(ContinueStatementNode node) {}

        public virtual void VisitFunctionCall(FunctionCallNode node)
        {
            Visit(node.FunctionReferenceNode);
            Visit(node.ArgumentNodes);
        }

        public virtual void VisitIfStatement(IfStatementNode node)
        {
            Visit(node.IfNode);
            Visit(node.ElseIfNodes);
            Visit(node.ElseNodeBodyNodes);
        }

        public virtual void VisitWhileStatement(WhileStatementNode node)
        {
            Visit(node.ConditionNode);
            Visit(node.BodyNodes);
        }
        public virtual void VisitIntegerLiteral(IntegerLiteralNode node) {}
        public virtual void VisitFloatLiteral(FloatLiteralNode node) {}
        public virtual void VisitStringLiteral(StringLiteralNode node) {}

        public virtual void VisitNoFunc(NoFuncNode node) {}

        public virtual void VisitNull(NullNode node) {}

        public virtual void VisitReference(ReferenceNode node)
        {
            Visit(node.ArrayIndexNode);
            Visit(node.AttributeNode);
        }
        public virtual void VisitName(NameNode node) {}
        
        
        private void Visit(List<ConditionalNode> nodes)
        {
            foreach (var node in nodes)
            {
                Visit(node);
            }
        }
        
        private void Visit(List<DeclarationNode> nodes)
        {
            foreach (var node in nodes)
            {
                Visit(node);
            }
        }
        
        private void Visit(List<ExpressionNode> nodes)
        {
            foreach (var node in nodes)
            {
                Visit(node);
            }
        }
        
        private void Visit(List<StatementNode> nodes)
        {
            foreach (var node in nodes)
            {
                Visit(node);
            }
        }

        public void Visit(ASTNode node)
        {
            switch (node)
            {
                case FileNode fileNode:
                    VisitFile(fileNode);
                    break;

                case FunctionDefinitionNode functionDefinitionNode:
                    VisitFunctionDefinition(functionDefinitionNode);
                    break;
                
                case AssignmentNode assignmentNode:
                    VisitAssignment(assignmentNode);
                    break;
                
                case CompoundAssignmentNode compoundAssignmentNode:
                    VisitCompoundAssignment(compoundAssignmentNode);
                    break;
                
                case UnaryExpressionNode unaryExpressionNode:
                    VisitUnaryExpression(unaryExpressionNode);
                    break;
                
                case BinaryExpressionNode binaryExpressionNode:
                    VisitBinaryExpression(binaryExpressionNode);
                    break;
                
                case ClassDefinitionNode classDefinitionNode:
                    VisitClassDefinition(classDefinitionNode);
                    break;
                
                case PrototypeDefinitionNode prototypeDefinitionNode:
                    VisitPrototypeDefinition(prototypeDefinitionNode);
                    break;
                
                case InstanceDefinitionNode instanceDefinitionNode:
                    VisitInstanceDefinition(instanceDefinitionNode);
                    break;
                
                case ConstDefinitionNode constDefinitionNode:
                    VisitConstDefinition(constDefinitionNode);
                    break;
                
                case ConstArrayDefinitionNode constArrayDefinitionNode:
                    VisitConstArrayDefinition(constArrayDefinitionNode);
                    break;
                
                case VarDeclarationNode varDeclarationNode:
                    VisitVarDeclaration(varDeclarationNode);
                    break;
                
                case VarArrayDeclarationNode varArrayDeclarationNode:
                    VisitVarArrayDeclaration(varArrayDeclarationNode);
                    break;
                
                case ReturnStatementNode returnStatementNode:
                    VisitReturnStatement(returnStatementNode);
                    break;
                
                case BreakStatementNode breakStatementNode:
                    VisitBreakStatement(breakStatementNode);
                    break;
                
                case ContinueStatementNode continueStatementNode:
                    VisitContinueStatement(continueStatementNode);
                    break;
                
                case FunctionCallNode functionCallNode:
                    VisitFunctionCall(functionCallNode);
                    break;
                
                case IfStatementNode ifStatementNode:
                    VisitIfStatement(ifStatementNode);
                    break;
                
                case WhileStatementNode whileStatementNode:
                    VisitWhileStatement(whileStatementNode);
                    break;
                
                case IntegerLiteralNode integerLiteralNode:
                    VisitIntegerLiteral(integerLiteralNode);
                    break;
                
                case FloatLiteralNode floatLiteralNode:
                    VisitFloatLiteral(floatLiteralNode);
                    break;
                
                case StringLiteralNode stringLiteralNode:
                    VisitStringLiteral(stringLiteralNode);
                    break;
                
                case NoFuncNode noFuncNode:
                    VisitNoFunc(noFuncNode);
                    break;
                
                case NullNode nullNode:
                    VisitNull(nullNode);
                    break;
                
                case ReferenceNode referenceNode:
                    VisitReference(referenceNode);
                    break;
                
                case NameNode nameNode:
                    VisitName(nameNode);
                    break;
                
                case ConditionalNode conditionalNode:
                    VisitConditional(conditionalNode);
                    break;
            }
        }
    }
}