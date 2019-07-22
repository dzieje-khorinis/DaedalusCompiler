using System.Collections.Generic;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    public abstract class AbstractSyntaxTreeBaseGenericVisitor<T>
    {
        protected internal virtual T DefaultResult => default (T);
        
        public T VisitTree(AbstractSyntaxTree tree)
        {
            foreach (var fileNode in tree.FileNodes)
            {
                VisitFile(fileNode);
            }

            return DefaultResult;
        }
        

        protected virtual T VisitFile(FileNode node) {
            Visit(node.DefinitionNodes);
            return DefaultResult;
        }

        protected virtual T VisitConditional(ConditionalNode node)
        {
            Visit(node.ConditionNode);
            Visit(node.BodyNodes);
            return DefaultResult;
        }

        protected virtual T VisitFunctionDefinition(FunctionDefinitionNode node)
        {
            Visit(node.NameNode);
            Visit(node.ParameterNodes);
            Visit(node.BodyNodes);
            return DefaultResult;
        }

        protected virtual T VisitAssignment(AssignmentNode node)
        {
            Visit(node.LeftSideNode);
            Visit(node.RightSideNode);
            return DefaultResult;
        }

        protected virtual T VisitCompoundAssignment(CompoundAssignmentNode node)
        {
            Visit(node.LeftSideNode);
            Visit(node.RightSideNode);
            return DefaultResult;
        }
        
        protected virtual T VisitUnaryExpression(UnaryExpressionNode node)
        {
            Visit(node.ExpressionNode);
            return DefaultResult;
        }

        protected virtual T VisitBinaryExpression(BinaryExpressionNode node)
        {
            Visit(node.LeftSideNode);
            Visit(node.RightSideNode);
            return DefaultResult;
        }

        protected virtual T VisitClassDefinition(ClassDefinitionNode node)
        {
            Visit(node.AttributeNodes);
            return DefaultResult;
        }

        protected virtual T VisitPrototypeDefinition(PrototypeDefinitionNode node)
        {
            Visit(node.NameNode);
            Visit(node.ParentReferenceNode);
            Visit(node.BodyNodes);
            return DefaultResult;
        }

        protected virtual T VisitInstanceDefinition(InstanceDefinitionNode node)
        {
            Visit(node.NameNode);
            Visit(node.ParentReferenceNode);
            Visit(node.BodyNodes);
            return DefaultResult;
        }

        protected virtual T VisitConstDefinition(ConstDefinitionNode node)
        {
            Visit(node.NameNode);
            Visit(node.RightSideNode);
            return DefaultResult;
        }

        protected virtual T VisitConstArrayDefinition(ConstArrayDefinitionNode node)
        {
            Visit(node.NameNode);
            Visit(node.ArraySizeNode);
            Visit(node.ElementNodes);
            return DefaultResult;
        }

        protected virtual T VisitVarDeclaration(VarDeclarationNode node)
        {
            Visit(node.NameNode);
            return DefaultResult;
        }

        protected virtual T VisitVarArrayDeclaration(VarArrayDeclarationNode node)
        {
            Visit(node.NameNode);
            Visit(node.ArraySizeNode);
            return DefaultResult;
        }
        
        protected virtual T VisitParameterDeclaration(ParameterDeclarationNode node)
        {
            Visit(node.NameNode);
            return DefaultResult;
        }

        protected virtual T VisitParameterArrayDeclaration(ParameterArrayDeclarationNode node)
        {
            Visit(node.NameNode);
            Visit(node.ArraySizeNode);
            return DefaultResult;
        }
        protected virtual T VisitReturnStatement(ReturnStatementNode node) { return DefaultResult; }
        protected virtual T VisitBreakStatement(BreakStatementNode node) { return DefaultResult; }
        protected virtual T VisitContinueStatement(ContinueStatementNode node) { return DefaultResult; }

        protected virtual T VisitFunctionCall(FunctionCallNode node)
        {
            Visit(node.FunctionReferenceNode);
            Visit(node.ArgumentNodes);
            return DefaultResult;
        }

        protected virtual T VisitIfStatement(IfStatementNode node)
        {
            Visit(node.IfNode);
            Visit(node.ElseIfNodes);
            Visit(node.ElseNodeBodyNodes);
            return DefaultResult;
        }

        protected virtual T VisitWhileStatement(WhileStatementNode node)
        {
            Visit(node.ConditionNode);
            Visit(node.BodyNodes);
            return DefaultResult;
        }
        protected virtual T VisitIntegerLiteral(IntegerLiteralNode node) { return DefaultResult; }
        protected virtual T VisitFloatLiteral(FloatLiteralNode node) { return DefaultResult; }
        protected virtual T VisitStringLiteral(StringLiteralNode node) { return DefaultResult; }

        protected virtual T VisitNoFunc(NoFuncNode node) { return DefaultResult; }

        protected virtual T VisitNull(NullNode node) { return DefaultResult; }

        protected virtual T VisitReference(ReferenceNode referenceNode)
        {
            Visit(referenceNode.ArrayIndexNode);
            Visit(referenceNode.AttributeNode);
            return DefaultResult;
        }
        protected virtual T VisitName(NameNode node) { return DefaultResult; }
        
        
        private T Visit(List<ConditionalNode> nodes)
        {
            foreach (var node in nodes)
            {
                Visit(node);
            }
            return DefaultResult;
        }
        
        private T Visit(List<DeclarationNode> nodes)
        {
            foreach (var node in nodes)
            {
                Visit(node);
            }
            return DefaultResult;
        }
        
        private T Visit(List<ExpressionNode> nodes)
        {
            foreach (var node in nodes)
            {
                Visit(node);
            }
            return DefaultResult;
        }
        
        private T Visit(List<StatementNode> nodes)
        {
            foreach (var node in nodes)
            {
                Visit(node);
            }
            return DefaultResult;
        }
        
        private T Visit(List<ParameterDeclarationNode> nodes)
        {
            foreach (var node in nodes)
            {
                Visit(node);
            }
            return DefaultResult;
        }

        protected T Visit(ASTNode node)
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
            return DefaultResult;
        }
    }
}