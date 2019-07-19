using System.Collections.Generic;
using Antlr4.Runtime.Tree;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    
    public enum UnaryOperator
    {
        Minus,
        Not,
        Negate,
        Plus,
    }
    
    public enum CompoundAssignmentOperator
    {
        Add,
        Sub,
        Mult,
        Div,
    }

    public enum BinaryOperator
    {
        Mult,
        Div,
        Modulo,
        
        Add,
        Sub,
        
        ShiftLeft,
        ShiftRight,
        
        Less,
        Greater,
        LessOrEqual,
        GreaterOrEqual,
        
        Equal,
        NotEqual,
        
        BinAnd,
        
        BinOr,
        
        LogAnd,
        
        LogOr,
    }
    
    public class NodeLocation
    {
        public int FileIndex;
        public int Line;
        public int Column;
        public int CharsCount;
        public int LinesCount;
    }

    public class AbstractSyntaxTree
    {
        public List<FileNode> FileNodes;

        public AbstractSyntaxTree(List<IParseTree> parseTrees, int externalFilesCount)
        {
            FileNodes = new List<FileNode>();

            int index = 0;
            foreach (IParseTree parseTree in parseTrees)
            {
                ParseTreeVisitor visitor = new ParseTreeVisitor(index);
                FileNodes.Add((FileNode) visitor.Visit(parseTree));
                index++;
            }

            for (int i = 0; i < externalFilesCount; ++i)
            {
                FileNodes[i].IsExternal = true;
            }
        }
    }

    public abstract class ASTNode
    {
        public NodeLocation Location;
        public ASTNode Parent;

        protected ASTNode(NodeLocation location)
        {
            Location = location;
        }
    }
    
    public abstract class StatementNode : ASTNode
    {
        protected StatementNode(NodeLocation location) : base(location)
        {
        }
    }
    
    public abstract class ExpressionNode : StatementNode
    {
        protected ExpressionNode(NodeLocation location) : base(location)
        {
        }
    }

    public abstract class DeclarationNode : StatementNode
    {
        public string Type;
        public NameNode NameNode;

        protected DeclarationNode(NodeLocation location, string type, NameNode nameNode) : base(location)
        {
            Type = type;
            NameNode = nameNode;
        }
    }

    public abstract class ArrayDeclarationNode : DeclarationNode
    {
        public ExpressionNode ArraySizeNode;

        protected ArrayDeclarationNode(NodeLocation location, string type, NameNode nameNode,
            ExpressionNode arraySizeNode) : base(location, type, nameNode)
        {
            ArraySizeNode = arraySizeNode;
        }
    }
    
    public abstract class ValueNode : ExpressionNode
    {
        protected ValueNode(NodeLocation location) : base(location)
        {
        }
    }
    
    public class FileNode : ASTNode
    {
        public List<DeclarationNode> DefinitionNodes;
        public bool IsExternal;

        public FileNode(NodeLocation location, List<DeclarationNode> definitionNodes, bool isExternal = false) :
            base(location)
        {
            foreach (var node in definitionNodes)
            {
                node.Parent = this;
            }
            
            DefinitionNodes = definitionNodes;
            IsExternal = isExternal;
        }
    }
    
    public class ConditionalNode : ASTNode
    {
        public ExpressionNode ConditionNode;
        public List<StatementNode> BodyNodes;

        public ConditionalNode(NodeLocation location, ExpressionNode conditionNode, List<StatementNode> bodyNodes) :
            base(location)
        {
            conditionNode.Parent = this;
            foreach (var node in bodyNodes)
            {
                node.Parent = this;
            }
            
            ConditionNode = conditionNode;
            BodyNodes = bodyNodes;
        }
    }

    public class FunctionDefinitionNode : DeclarationNode
    {
        public List<DeclarationNode> ParameterNodes;
        public List<StatementNode> BodyNodes;

        public FunctionDefinitionNode(NodeLocation location, string type, NameNode nameNode,
            List<DeclarationNode> parameterNodes, List<StatementNode> bodyNodes) : base(location, type, nameNode)
        {
            foreach (var node in parameterNodes)
            {
                node.Parent = this;
            }
            foreach (var node in bodyNodes)
            {
                node.Parent = this;
            }
            
            ParameterNodes = parameterNodes;
            BodyNodes = bodyNodes;
        }
    }

    public class AssignmentNode : StatementNode
    {
        public ReferenceNode LeftSideNode;
        public ExpressionNode RightSideNode;

        public AssignmentNode(NodeLocation location, ReferenceNode leftSideNode, ExpressionNode rightSideNode) :
            base(location)
        {
            leftSideNode.Parent = this;
            rightSideNode.Parent = this;
            
            LeftSideNode = leftSideNode;
            RightSideNode = rightSideNode;
        }
    }

    public class CompoundAssignmentNode : StatementNode
    {
        public CompoundAssignmentOperator Operator;
        public ReferenceNode LeftSideNode;
        public ExpressionNode RightSideNode;

        public CompoundAssignmentNode(NodeLocation location, CompoundAssignmentOperator @operator, ReferenceNode leftSideNode,
            ExpressionNode rightSideNode) : base(location)
        {
            leftSideNode.Parent = this;
            rightSideNode.Parent = this;
            
            Operator = @operator;
            LeftSideNode = leftSideNode;
            RightSideNode = rightSideNode;
        }
    }
    
    public class UnaryExpressionNode : ExpressionNode
    {
        public UnaryOperator Operator;
        public ExpressionNode ExpressionNode;

        public UnaryExpressionNode(NodeLocation location, UnaryOperator @operator, ExpressionNode expressionNode) : base(
            location)
        {
            expressionNode.Parent = this;
            
            Operator = @operator;
            ExpressionNode = expressionNode;
        }
    }

    public class BinaryExpressionNode : ExpressionNode
    {
        public BinaryOperator Operator;
        public ExpressionNode LeftSideNode;
        public ExpressionNode RightSideNode;

        public BinaryExpressionNode(NodeLocation location, BinaryOperator @operator, ExpressionNode leftSideNode,
            ExpressionNode rightSideNode) : base(location)
        {
            leftSideNode.Parent = this;
            rightSideNode.Parent = this;
            
            Operator = @operator;
            LeftSideNode = leftSideNode;
            RightSideNode = rightSideNode;
        }
    }


    public class ClassDefinitionNode : DeclarationNode
    {
        public List<DeclarationNode> AttributeNodes;

        public ClassDefinitionNode(NodeLocation location, NameNode nameNode, List<DeclarationNode> attributeNodes) :
            base(location, "class", nameNode)
        {
            foreach (var node in attributeNodes)
            {
                node.Parent = this;
            }
            
            AttributeNodes = attributeNodes;
        }
    }

    public class PrototypeDefinitionNode : DeclarationNode
    {
        public ReferenceNode ParentReferenceNode;
        public List<StatementNode> BodyNodes;

        public PrototypeDefinitionNode(NodeLocation location, NameNode nameNode, ReferenceNode parentReferenceNode,
            List<StatementNode> bodyNodes) : base(location, "prototype", nameNode)
        {
            parentReferenceNode.Parent = this;
            foreach (var node in bodyNodes)
            {
                node.Parent = this;
            }
            
            ParentReferenceNode = parentReferenceNode;
            BodyNodes = bodyNodes;
        }
    }

    public class InstanceDefinitionNode : DeclarationNode
    {
        public ReferenceNode ParentReferenceNode;
        public List<StatementNode> BodyNodes;

        public InstanceDefinitionNode(NodeLocation location, NameNode nameNode, ReferenceNode parentReferenceNode,
            List<StatementNode> bodyNodes) : base(location, "instance", nameNode)
        {
            parentReferenceNode.Parent = this;
            foreach (var node in bodyNodes)
            {
                node.Parent = this;
            }
            
            ParentReferenceNode = parentReferenceNode;
            BodyNodes = bodyNodes;
        }
    }

    public class ConstDefinitionNode : DeclarationNode
    {
        public ExpressionNode RightSideNode;

        public ConstDefinitionNode(NodeLocation location, string type, NameNode nameNode,
            ExpressionNode rightSideNode) : base(location, type, nameNode)
        {
            rightSideNode.Parent = this;
            
            RightSideNode = rightSideNode;
        }
    }

    public class ConstArrayDefinitionNode : ArrayDeclarationNode
    {
        public List<ExpressionNode> ElementNodes;

        public ConstArrayDefinitionNode(NodeLocation location, string type, NameNode nameNode,
            ExpressionNode arraySizeNode, List<ExpressionNode> elementNodes) : base(location, type, nameNode,
            arraySizeNode)
        {
            foreach (var node in elementNodes)
            {
                node.Parent = this;
            }
            
            ElementNodes = elementNodes;
        }
    }

    public class VarDeclarationNode : DeclarationNode
    {
        public VarDeclarationNode(NodeLocation location, string type, NameNode nameNode) : base(location, type,
            nameNode)
        {
        }
    }

    public class VarArrayDeclarationNode : ArrayDeclarationNode
    {
        public VarArrayDeclarationNode(NodeLocation location, string type, NameNode nameNode,
            ExpressionNode arraySizeNode) : base(location, type, nameNode, arraySizeNode)
        {
        }
    }

    public class ReturnStatementNode : StatementNode
    {
        public ReturnStatementNode(NodeLocation location) : base(location)
        {
        }
    }

    public class BreakStatementNode : StatementNode
    {
        public BreakStatementNode(NodeLocation location) : base(location)
        {
        }
    }

    public class ContinueStatementNode : StatementNode
    {
        public ContinueStatementNode(NodeLocation location) : base(location)
        {
        }
    }

    public class FunctionCallNode : ExpressionNode
    {
        public ReferenceNode FunctionReferenceNode;
        public List<ExpressionNode> ArgumentNodes;

        public FunctionCallNode(NodeLocation location, ReferenceNode functionReferenceNode,
            List<ExpressionNode> argumentNodes) : base(location)
        {
            functionReferenceNode.Parent = this;
            foreach (var node in argumentNodes)
            {
                node.Parent = this;
            }
            
            
            FunctionReferenceNode = functionReferenceNode;
            ArgumentNodes = argumentNodes;
        }
    }

    public class IfStatementNode : StatementNode
    {
        public ConditionalNode IfNode;
        public List<ConditionalNode> ElseIfNodes;
        public List<StatementNode> ElseNodeBodyNodes;

        public IfStatementNode(NodeLocation location, ConditionalNode ifNode,
            List<ConditionalNode> elseIfNodes, List<StatementNode> elseNodeBodyNodes) : base(location)
        {
            ifNode.Parent = this;
            foreach (var node in elseIfNodes)
            {
                node.Parent = this;
            }
            foreach (var node in elseNodeBodyNodes)
            {
                node.Parent = this;
            }
            
            IfNode = ifNode;
            ElseIfNodes = elseIfNodes;
            ElseNodeBodyNodes = elseNodeBodyNodes;
        }
    }

    public class WhileStatementNode : ConditionalNode
    {
        public WhileStatementNode(NodeLocation location, ExpressionNode conditionNode,
            List<StatementNode> bodyNodes) : base(location, conditionNode, bodyNodes)
        {
        }
    }


    public class IntegerLiteralNode : ValueNode
    {
        public int Value;

        public IntegerLiteralNode(NodeLocation location, int value) : base(location)
        {
            Value = value;
        }
    }

    public class FloatLiteralNode : ValueNode
    {
        public float Value;

        public FloatLiteralNode(NodeLocation location, float value) : base(location)
        {
            Value = value;
        }
    }

    public class StringLiteralNode : ValueNode
    {
        public string Value;

        public StringLiteralNode(NodeLocation location, string value) : base(location)
        {
            Value = value;
        }
    }

    public class NoFuncNode : ValueNode
    {
        public NoFuncNode(NodeLocation location) : base(location)
        {
        }
    }

    public class NullNode : ValueNode
    {
        public NullNode(NodeLocation location) : base(location)
        {
        }
    }


    public class ReferenceNode : ExpressionNode
    {
        public string Name;
        public ExpressionNode ArrayIndexNode; // optional
        public ReferenceNode AttributeNode; // optional

        public ReferenceNode(NodeLocation location, string name, ExpressionNode arrayIndexNode = null,
            ReferenceNode attributeNode = null) : base(location)
        {
            if (arrayIndexNode != null)
            {
                arrayIndexNode.Parent = this;
            }

            if (attributeNode != null)
            {
                attributeNode.Parent = this;
            }
            
            Name = name;
            ArrayIndexNode = arrayIndexNode;
            AttributeNode = attributeNode;
        }
    }

    public class NameNode : ASTNode
    {
        public string Value;

        public NameNode(NodeLocation location, string value) : base(location)
        {
            Value = value;
        }
    }
    
    
    public abstract class TemporaryNode : ASTNode
        /*
         * Helper nodes used only during AST construction. These nodes don't appear in the result AST.
         */
    {
        public List<DeclarationNode> Nodes;

        protected TemporaryNode(NodeLocation location, List<DeclarationNode> nodes) : base(location)
        {
            Nodes = nodes;
        }
    }

    public class ConstDefinitionsTemporaryNode : TemporaryNode
    {
        public ConstDefinitionsTemporaryNode(NodeLocation location, List<DeclarationNode> nodes) : base(location,
            nodes)
        {
        }
    }

    public class VarDeclarationsTemporaryNode : TemporaryNode
    {
        public VarDeclarationsTemporaryNode(NodeLocation location, List<DeclarationNode> nodes) : base(location,
            nodes)
        {
        }
    }
    
    public class InstanceDeclarationsTemporaryNode : TemporaryNode
    {
        public InstanceDeclarationsTemporaryNode(NodeLocation location, List<DeclarationNode> nodes) : base(
            location, nodes)
        {
        }
    }

}