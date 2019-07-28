using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Antlr4.Runtime.Tree;
using DaedalusCompiler.Dat;

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
    
    [DebuggerDisplay("FileIndex:{FileIndex} Line:{Line} Column:{Column} Index:{Index} CharsCount:{CharsCount} LinesCount:{LinesCount}")]
    public class NodeLocation
    {
        public int FileIndex;
        public int Line;
        public int Column;
        public int Index;
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

    public abstract class NodeAnnotation
    {
        
    }


    public class TextAnnotation : NodeAnnotation
    {
        private string Message;

        public TextAnnotation(string message)
        {
            Message = message;
        }
    }

    public class UndeclaredIdentifierAnnotation : NodeAnnotation
    {
        
    }

    public class CycleAnnotation : NodeAnnotation
    {
        
    }


    public abstract class ASTNode
    {
        public NodeLocation Location;

        public List<NodeAnnotation> Errors; //warnings & errors
        public ASTNode Parent { get; set; }

        protected ASTNode(NodeLocation location)
        {
            Errors = new List<NodeAnnotation>();
            Location = location;
        }

        public bool HasErrors()
        {
            return Errors.Count > 0;
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
        public DatSymbolType BuiltinType;
        protected ExpressionNode(NodeLocation location) : base(location)
        {
            BuiltinType = DatSymbolType.Undefined;
        }
    }

    public abstract class DeclarationNode : StatementNode
    {
        public string TypeName;
        public NameNode NameNode;
        public DatSymbol Symbol; //filled in SymbolTableCreationVisitor

        protected DeclarationNode(NodeLocation location, string type, NameNode nameNode) : base(location)
        {
            TypeName = type.First().ToString().ToUpper() + type.Substring(1).ToLower(); //capitalized
            NameNode = nameNode;
        }
    }


    public interface IArrayDeclarationNode
    {
        ExpressionNode ArraySizeNode { get; set; }
    }
    
    /*
    public abstract class ArrayDeclarationNode : DeclarationNode, IArrayDeclarationNode
    {
        public ExpressionNode ArraySizeNode { get; set; }

        protected ArrayDeclarationNode(NodeLocation location, string type, NameNode nameNode,
            ExpressionNode arraySizeNode) : base(location, type, nameNode)
        {
            ArraySizeNode = arraySizeNode;
        }
    }
    */
    
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
        public readonly List<ParameterDeclarationNode> ParameterNodes;
        public readonly List<StatementNode> BodyNodes;

        public FunctionDefinitionNode(NodeLocation location, string type, NameNode nameNode,
            List<ParameterDeclarationNode> parameterNodes, List<StatementNode> bodyNodes) : base(location, type, nameNode)
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

    public class ConstDefinitionNode : VarDeclarationNode
    {
        public ExpressionNode RightSideNode;

        public ConstDefinitionNode(NodeLocation location, string type, NameNode nameNode,
            ExpressionNode rightSideNode) : base(location, type, nameNode)
        {
            rightSideNode.Parent = this;
            
            RightSideNode = rightSideNode;
        }

        protected ConstDefinitionNode(NodeLocation location, string type, NameNode nameNode) : base(location, type, nameNode)
        {
        }
    }

    public class ConstArrayDefinitionNode : ConstDefinitionNode, IArrayDeclarationNode
    {
        public readonly List<ExpressionNode> ElementNodes;
        public ExpressionNode ArraySizeNode { get; set; }
        
        public ConstArrayDefinitionNode(NodeLocation location, string type, NameNode nameNode,
            ExpressionNode arraySizeNode, List<ExpressionNode> elementNodes) : base(location, type, nameNode)
        {
            ArraySizeNode = arraySizeNode;
            
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

    public class VarArrayDeclarationNode : VarDeclarationNode, IArrayDeclarationNode
    {
        public ExpressionNode ArraySizeNode { get; set; }
        
        public VarArrayDeclarationNode(NodeLocation location, string type, NameNode nameNode,
            ExpressionNode arraySizeNode) : base(location, type, nameNode)
        {
            ArraySizeNode = arraySizeNode;
        }
    }
    
    public class ParameterDeclarationNode : VarDeclarationNode
    {
        public new FunctionDefinitionNode Parent { get; set; }
        
        public ParameterDeclarationNode(NodeLocation location, string type, NameNode nameNode) : base(location, type,
            nameNode)
        {
        }
    }

    public class ParameterArrayDeclarationNode : ParameterDeclarationNode, IArrayDeclarationNode
    {
        public ExpressionNode ArraySizeNode { get; set; }
        
        public ParameterArrayDeclarationNode(NodeLocation location, string type, NameNode nameNode,
            ExpressionNode arraySizeNode) : base(location, type, nameNode)
        {
            ArraySizeNode = arraySizeNode;
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
        public DatSymbol Symbol; //filled in SymbolTableCreationVisitor

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

        public string FullName;    //filled in ConstEvalutationVisitor
        public DatSymbol Symbol; //filled in AnnotationsAdditionVisitor

        public ReferenceNode(NodeLocation location, string name, ExpressionNode arrayIndexNode = null,
            ReferenceNode attributeNode = null) : base(location)
        {
            Symbol = null;
            FullName = "";
            
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
    
    
    /// <remarks>Helper nodes used only during AST construction. These nodes don't appear in the result AST.</remarks>
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