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
            Visit(node.InheritanceReferenceNode);
            Visit(node.BodyNodes);
            return DefaultResult;
        }

        protected virtual T VisitInstanceDefinition(InstanceDefinitionNode node)
        {
            Visit(node.NameNode);
            Visit(node.InheritanceReferenceNode);
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
            foreach (var partNode in referenceNode.PartNodes)
            {
                switch (partNode)
                {
                    case ArrayIndexNode arrayIndexNode:
                        Visit(arrayIndexNode.ExpressionNode);
                        break;
                }
            }
            return DefaultResult;
        }
        protected virtual T VisitName(NameNode node) { return DefaultResult; }
        
        
        public T Visit(List<ConstDefinitionNode> nodes)
        {
            foreach (var node in nodes)
            {
                Visit(node);
            }
            return DefaultResult;
        }
        
        public T Visit(List<IArrayDeclarationNode> nodes)
        {
            foreach (var node in nodes)
            {
                Visit(node);
            }
            return DefaultResult;
        }

        public T Visit(List<ConditionalNode> nodes)
        {
            foreach (var node in nodes)
            {
                Visit(node);
            }
            return DefaultResult;
        }
        
        public T Visit(List<DeclarationNode> nodes)
        {
            foreach (var node in nodes)
            {
                Visit(node);
            }
            return DefaultResult;
        }
        
        public T Visit(List<ExpressionNode> nodes)
        {
            foreach (var node in nodes)
            {
                Visit(node);
            }
            return DefaultResult;
        }
        
        public T Visit(List<StatementNode> nodes)
        {
            foreach (var node in nodes)
            {
                Visit(node);
            }
            return DefaultResult;
        }
        
        public T Visit(List<ParameterDeclarationNode> nodes)
        {
            foreach (var node in nodes)
            {
                Visit(node);
            }
            return DefaultResult;
        }


        public T Visit(IArrayDeclarationNode node)
        {
            switch (node)
            {
                case VarArrayDeclarationNode varArrayDeclarationNode:
                    return VisitVarArrayDeclaration(varArrayDeclarationNode);
                
                case ConstArrayDefinitionNode constArrayDefinitionNode:
                    return VisitConstArrayDefinition(constArrayDefinitionNode);
                
                case ParameterArrayDeclarationNode parameterArrayDeclarationNode:
                    return VisitParameterArrayDeclaration(parameterArrayDeclarationNode);
                
            }

            return DefaultResult;
        }

        public virtual T Visit(ASTNode node)
        {
            switch (node)
            {
                case FileNode fileNode:
                    return VisitFile(fileNode);

                case FunctionDefinitionNode functionDefinitionNode:
                    return VisitFunctionDefinition(functionDefinitionNode);

                case AssignmentNode assignmentNode:
                    return VisitAssignment(assignmentNode);

                case CompoundAssignmentNode compoundAssignmentNode:
                    return VisitCompoundAssignment(compoundAssignmentNode);

                case UnaryExpressionNode unaryExpressionNode:
                    return VisitUnaryExpression(unaryExpressionNode);

                case BinaryExpressionNode binaryExpressionNode:
                    return VisitBinaryExpression(binaryExpressionNode);

                case ClassDefinitionNode classDefinitionNode:
                    return VisitClassDefinition(classDefinitionNode);
                                    
                case PrototypeDefinitionNode prototypeDefinitionNode:
                    return VisitPrototypeDefinition(prototypeDefinitionNode);
                                    
                case InstanceDefinitionNode instanceDefinitionNode:
                    return VisitInstanceDefinition(instanceDefinitionNode);
                                    
                case ConstArrayDefinitionNode constArrayDefinitionNode:
                    return VisitConstArrayDefinition(constArrayDefinitionNode);
                                    
                case ConstDefinitionNode constDefinitionNode:
                    return VisitConstDefinition(constDefinitionNode);
                    
                case ParameterArrayDeclarationNode parameterArrayDeclarationNode:
                    return VisitParameterArrayDeclaration(parameterArrayDeclarationNode);
                                    
                case ParameterDeclarationNode parameterDeclarationNode:
                    return VisitParameterDeclaration(parameterDeclarationNode);
                                    
                case VarArrayDeclarationNode varArrayDeclarationNode:
                    return VisitVarArrayDeclaration(varArrayDeclarationNode);
                                    
                case VarDeclarationNode varDeclarationNode:
                    return VisitVarDeclaration(varDeclarationNode);
                    
                case ReturnStatementNode returnStatementNode:
                    return VisitReturnStatement(returnStatementNode);
                                    
                case BreakStatementNode breakStatementNode:
                    return VisitBreakStatement(breakStatementNode);
                                    
                case ContinueStatementNode continueStatementNode:
                    return VisitContinueStatement(continueStatementNode);
                                    
                case FunctionCallNode functionCallNode:
                    return VisitFunctionCall(functionCallNode);
                                    
                case IfStatementNode ifStatementNode:
                    return VisitIfStatement(ifStatementNode);
                                    
                case WhileStatementNode whileStatementNode:
                    return VisitWhileStatement(whileStatementNode);
                                    
                case IntegerLiteralNode integerLiteralNode:
                    return VisitIntegerLiteral(integerLiteralNode);
                                    
                case FloatLiteralNode floatLiteralNode:
                    return VisitFloatLiteral(floatLiteralNode);
                                    
                case StringLiteralNode stringLiteralNode:
                    return VisitStringLiteral(stringLiteralNode);
                                    
                case NoFuncNode noFuncNode:
                    return VisitNoFunc(noFuncNode);
                                    
                case NullNode nullNode:
                    return VisitNull(nullNode);
                                    
                case ReferenceNode referenceNode:
                    return VisitReference(referenceNode);
                                    
                case NameNode nameNode:
                    return VisitName(nameNode);
                                    
                case ConditionalNode conditionalNode:
                    return VisitConditional(conditionalNode);
            }
            return DefaultResult;
        }
    }
}