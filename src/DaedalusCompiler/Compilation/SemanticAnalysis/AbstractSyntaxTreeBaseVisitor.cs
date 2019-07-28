using System.Collections.Generic;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    public abstract class AbstractSyntaxTreeBaseVisitor
    {
        public void VisitTree(AbstractSyntaxTree tree)
        {
            foreach (var fileNode in tree.FileNodes)
            {
                VisitFile(fileNode);
            }
        }
        
        protected virtual void VisitFile(FileNode node) {
            Visit(node.DefinitionNodes);
        }

        protected virtual void VisitConditional(ConditionalNode node)
        {
            Visit(node.ConditionNode);
            Visit(node.BodyNodes);
        }

        protected virtual void VisitFunctionDefinition(FunctionDefinitionNode node)
        {
            Visit(node.NameNode);
            Visit(node.ParameterNodes);
            Visit(node.BodyNodes);
        }

        protected virtual void VisitAssignment(AssignmentNode node)
        {
            Visit(node.LeftSideNode);
            Visit(node.RightSideNode);
        }

        protected virtual void VisitCompoundAssignment(CompoundAssignmentNode node)
        {
            Visit(node.LeftSideNode);
            Visit(node.RightSideNode);
        }
        
        protected virtual void VisitUnaryExpression(UnaryExpressionNode node)
        {
            Visit(node.ExpressionNode);
        }

        protected virtual void VisitBinaryExpression(BinaryExpressionNode node)
        {
            Visit(node.LeftSideNode);
            Visit(node.RightSideNode);
        }

        protected virtual void VisitClassDefinition(ClassDefinitionNode node)
        {
            Visit(node.AttributeNodes);
        }

        protected virtual void VisitPrototypeDefinition(PrototypeDefinitionNode node)
        {
            Visit(node.NameNode);
            Visit(node.ParentReferenceNode);
            Visit(node.BodyNodes);
        }

        protected virtual void VisitInstanceDefinition(InstanceDefinitionNode node)
        {
            Visit(node.NameNode);
            Visit(node.ParentReferenceNode);
            Visit(node.BodyNodes);
        }

        protected virtual void VisitConstDefinition(ConstDefinitionNode node)
        {
            Visit(node.NameNode);
            Visit(node.RightSideNode);
        }

        protected virtual void VisitConstArrayDefinition(ConstArrayDefinitionNode node)
        {
            Visit(node.NameNode);
            Visit(node.ArraySizeNode);
            Visit(node.ElementNodes);
        }

        protected virtual void VisitVarDeclaration(VarDeclarationNode node)
        {
            Visit(node.NameNode);
        }

        protected virtual void VisitVarArrayDeclaration(VarArrayDeclarationNode node)
        {
            Visit(node.NameNode);
            Visit(node.ArraySizeNode);
        }
        
        protected virtual void VisitParameterDeclaration(ParameterDeclarationNode node)
        {
            Visit(node.NameNode);
        }

        protected virtual void VisitParameterArrayDeclaration(ParameterArrayDeclarationNode node)
        {
            Visit(node.NameNode);
            Visit(node.ArraySizeNode);
        }
        protected virtual void VisitReturnStatement(ReturnStatementNode node) {}
        protected virtual void VisitBreakStatement(BreakStatementNode node) {}
        protected virtual void VisitContinueStatement(ContinueStatementNode node) {}

        protected virtual void VisitFunctionCall(FunctionCallNode node)
        {
            Visit(node.FunctionReferenceNode);
            Visit(node.ArgumentNodes);
        }

        protected virtual void VisitIfStatement(IfStatementNode node)
        {
            Visit(node.IfNode);
            Visit(node.ElseIfNodes);
            Visit(node.ElseNodeBodyNodes);
        }

        protected virtual void VisitWhileStatement(WhileStatementNode node)
        {
            Visit(node.ConditionNode);
            Visit(node.BodyNodes);
        }
        protected virtual void VisitIntegerLiteral(IntegerLiteralNode node) {}
        protected virtual void VisitFloatLiteral(FloatLiteralNode node) {}
        protected virtual void VisitStringLiteral(StringLiteralNode node) {}

        protected virtual void VisitNoFunc(NoFuncNode node) {}

        protected virtual void VisitNull(NullNode node) {}

        protected virtual void VisitReference(ReferenceNode referenceNode)
        {
            if (referenceNode.ArrayIndexNode != null)
            {
                Visit(referenceNode.ArrayIndexNode);
            }

            if (referenceNode.AttributeNode != null)
            {
                Visit(referenceNode.AttributeNode);
            }
        }
        protected virtual void VisitName(NameNode node) {}
        
        
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
        
        private void Visit(List<ParameterDeclarationNode> nodes)
        {
            foreach (var node in nodes)
            {
                Visit(node);
            }
        }

        protected virtual void Visit(ASTNode node)
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
                
                case ConstArrayDefinitionNode constArrayDefinitionNode:
                    VisitConstArrayDefinition(constArrayDefinitionNode);
                    break;
                
                case ConstDefinitionNode constDefinitionNode:
                    VisitConstDefinition(constDefinitionNode);
                    break;

                case ParameterArrayDeclarationNode parameterArrayDeclarationNode:
                    VisitParameterArrayDeclaration(parameterArrayDeclarationNode);
                    break;
                
                case ParameterDeclarationNode parameterDeclarationNode:
                    VisitParameterDeclaration(parameterDeclarationNode);
                    break;
                
                case VarArrayDeclarationNode varArrayDeclarationNode:
                    VisitVarArrayDeclaration(varArrayDeclarationNode);
                    break;
                
                case VarDeclarationNode varDeclarationNode:
                    VisitVarDeclaration(varDeclarationNode);
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