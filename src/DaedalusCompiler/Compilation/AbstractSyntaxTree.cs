using System;
using System.Collections.Generic;
using Antlr4.Runtime.Tree;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    
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
                FileNodes.Add((FileNode)visitor.Visit(parseTree));
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
        
        protected ASTNode(NodeLocation location)
        {
            Location = location;
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

        protected ArrayDeclarationNode(NodeLocation location, string type, NameNode nameNode, ExpressionNode arraySizeNode) : base(location, type, nameNode)
        {
            ArraySizeNode = arraySizeNode;
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
    
    public abstract class ConditionalBlock : ASTNode
    {
        public ExpressionNode ConditionNode;
        public List<StatementNode> BodyNodes;

        protected ConditionalBlock(NodeLocation location, ExpressionNode conditionNode, List<StatementNode> bodyNodes) : base(location)
        {
            ConditionNode = conditionNode;
            BodyNodes = bodyNodes;
        }
    }
    
    
    
    public abstract class ValueNode : ExpressionNode
    {
        protected ValueNode(NodeLocation location) : base(location)
        {
        }
    }






    public abstract class TemporaryNode : ASTNode
    {
        public List<DeclarationNode> Nodes;
        protected TemporaryNode(NodeLocation location, List<DeclarationNode> nodes) : base(location)
        {
            Nodes = nodes;
        }
    }

    public class ConstDefinitionsTemporaryNode : TemporaryNode
    {
        public ConstDefinitionsTemporaryNode(NodeLocation location, List<DeclarationNode> nodes) : base(location, nodes)
        {
        }
    }
    
    public class VarDeclarationsTemporaryNode : TemporaryNode
    {
        public VarDeclarationsTemporaryNode(NodeLocation location, List<DeclarationNode> nodes) : base(location, nodes)
        {
        }
    }

    
    public class InstanceDeclarationsTemporaryNode : TemporaryNode
    {
        public InstanceDeclarationsTemporaryNode(NodeLocation location, List<DeclarationNode> nodes) : base(location, nodes)
        {
        }
    }
    

    public class FileNode : ASTNode
    {
        public List<DeclarationNode> DefinitionNodes;
        public bool IsExternal;

        public FileNode(NodeLocation location, List<DeclarationNode> definitionNodes, bool isExternal=false) : base(location)
        {
            DefinitionNodes = definitionNodes;
            IsExternal = isExternal;
        }
    }

    public class FunctionDefinitionNode : DeclarationNode
    {
        public List<ParameterDeclarationNode> ParameterNodes;
        public List<StatementNode> BodyNodes;

        public FunctionDefinitionNode(NodeLocation location, string type, NameNode nameNode, List<ParameterDeclarationNode> parameterNodes, List<StatementNode> bodyNodes) : base(location, type, nameNode)
        {
            ParameterNodes = parameterNodes;
            BodyNodes = bodyNodes;
        }
    }

    public class AssignmentNode : StatementNode
    {
        public ReferenceNode LeftSideNode;
        public ExpressionNode RightSideNode;

        public AssignmentNode(NodeLocation location, ReferenceNode leftSideNode, ExpressionNode rightSideNode) : base(location)
        {
            LeftSideNode = leftSideNode;
            RightSideNode = rightSideNode;
        }
    }

    public class CompoundAssignmentNode : StatementNode
    {
        public string Operator;
        public ReferenceNode LeftSideNode;
        public ExpressionNode RightSideNode;

        public CompoundAssignmentNode(NodeLocation location, string @operator, ReferenceNode leftSideNode, ExpressionNode rightSideNode) : base(location)
        {
            Operator = @operator;
            LeftSideNode = leftSideNode;
            RightSideNode = rightSideNode;
        }
    }

    
    public class UnaryExpressionNode : ExpressionNode
    {
        public string Operator;
        public ExpressionNode ExpressionNode;

        public UnaryExpressionNode(NodeLocation location, string @operator, ExpressionNode expressionNode) : base(location)
        {
            Operator = @operator;
            ExpressionNode = expressionNode;
        }
    }

    public class BinaryExpressionNode : ExpressionNode
    {
        public string Operator;
        public ExpressionNode LeftSideNode;
        public ExpressionNode RightSideNode;

        public BinaryExpressionNode(NodeLocation location, string @operator, ExpressionNode leftSideNode, ExpressionNode rightSideNode) : base(location)
        {
            Operator = @operator;
            LeftSideNode = leftSideNode;
            RightSideNode = rightSideNode;
        }
    }

    
    
    
    public class ClassDefinitionNode : DeclarationNode
    {
        public List<DeclarationNode> AttributeNodes;

        public ClassDefinitionNode(NodeLocation location, NameNode nameNode, List<DeclarationNode> attributeNodes) : base(location, "class", nameNode)
        {
            AttributeNodes = attributeNodes;
        }
    }
    
    public class PrototypeDefinitionNode : DeclarationNode
    {
        public ReferenceNode ParentReferenceNode;
        public List<StatementNode> BodyNodes;

        public PrototypeDefinitionNode(NodeLocation location, NameNode nameNode, ReferenceNode parentReferenceNode, List<StatementNode> bodyNodes) : base(location, "prototype", nameNode)
        {
            ParentReferenceNode = parentReferenceNode;
            BodyNodes = bodyNodes;
        }
    }
    
    public class InstanceDefinitionNode : DeclarationNode
    {
        public ReferenceNode ParentReferenceNode;
        public List<StatementNode> BodyNodes;

        public InstanceDefinitionNode(NodeLocation location, NameNode nameNode, ReferenceNode parentReferenceNode, List<StatementNode> bodyNodes) : base(location, "instance", nameNode)
        {
            ParentReferenceNode = parentReferenceNode;
            BodyNodes = bodyNodes;
        }
    }

    public class ConstDefinitionNode : DeclarationNode
    {
        public ExpressionNode RightSideNode;

        public ConstDefinitionNode(NodeLocation location, string type, NameNode nameNode, ExpressionNode rightSideNode) : base(location, type, nameNode)
        {
            RightSideNode = rightSideNode;
        }
    }
    
    public class ConstArrayDefinitionNode : ArrayDeclarationNode
    {
        public List<ExpressionNode> ElementNodes;

        public ConstArrayDefinitionNode(NodeLocation location, string type, NameNode nameNode, ExpressionNode arraySizeNode, List<ExpressionNode> elementNodes) : base(location, type, nameNode, arraySizeNode)
        {
            ElementNodes = elementNodes;
        }
    }

    public class VarDeclarationNode : DeclarationNode
    {
        public VarDeclarationNode(NodeLocation location, string type, NameNode nameNode) : base(location, type, nameNode)
        {
        }
    }
    
    public class VarArrayDeclarationNode : ArrayDeclarationNode
    {
        public VarArrayDeclarationNode(NodeLocation location, string type, NameNode nameNode, ExpressionNode arraySizeNode) : base(location, type, nameNode, arraySizeNode)
        {
        }
    }

    public class ParameterDeclarationNode : DeclarationNode
    {
        public ParameterDeclarationNode(NodeLocation location, string type, NameNode nameNode) : base(location, type, nameNode)
        {
        }
    }
    
    public class ParameterArrayDeclarationNode : ArrayDeclarationNode
    {
        public ParameterArrayDeclarationNode(NodeLocation location, string type, NameNode nameNode, ExpressionNode arraySizeNode) : base(location, type, nameNode, arraySizeNode)
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

        public FunctionCallNode(NodeLocation location, ReferenceNode functionReferenceNode, List<ExpressionNode> argumentNodes) : base(location)
        {
            FunctionReferenceNode = functionReferenceNode;
            ArgumentNodes = argumentNodes;
        }
    }

    public class IfStatementNode : StatementNode
    {
        public IfBlockNode IfBlockNode;
        public List<ElseIfBlockNode> ElseIfBlockNodes;
        public ElseBlockNode ElseBlockNode;

        public IfStatementNode(NodeLocation location, IfBlockNode ifBlockNode, List<ElseIfBlockNode> elseIfBlockNodes, ElseBlockNode elseBlockNode) : base(location)
        {
            IfBlockNode = ifBlockNode;
            ElseIfBlockNodes = ElseIfBlockNodes;
            ElseBlockNode = ElseBlockNode;
        }
    }
    
    
    public class IfBlockNode : ConditionalBlock
    {
        public IfBlockNode(NodeLocation location, ExpressionNode condition, List<StatementNode> body) : base(location, condition, body)
        {
        }
    }

    public class ElseIfBlockNode : ConditionalBlock
    {
        public ElseIfBlockNode(NodeLocation location, ExpressionNode condition, List<StatementNode> body) : base(location, condition, body)
        {
        }
    }

    public class ElseBlockNode : ASTNode
    {
        public List<StatementNode> BodyNodes;

        public ElseBlockNode(NodeLocation location, List<StatementNode> bodyNodes) : base(location)
        {
            BodyNodes = bodyNodes;
        }
    }

    public class WhileStatementNode : StatementNode
    {
        public ExpressionNode ConditionNode;
        public List<StatementNode> BodyNodes;

        public WhileStatementNode(NodeLocation location, ExpressionNode conditionNode, List<StatementNode> bodyNodes) : base(location)
        {
            ConditionNode = conditionNode;
            BodyNodes = bodyNodes;
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
        public ExpressionNode ArrayIndexNode;
        public ReferenceNode AttributeNode;

        public ReferenceNode(NodeLocation location, string name, ExpressionNode arrayIndexNode=null, ReferenceNode attributeNode=null) : base(location)
        {
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
    
}