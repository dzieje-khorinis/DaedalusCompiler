using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Antlr4.Runtime.Tree;
using Common;

namespace Common.SemanticAnalysis
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

        public int EndColumn;
    }

    public class AbstractSyntaxTree
    {
        public readonly List<FileNode> RootNodes;

        public readonly List<ReferenceNode> ReferenceNodes;

        public readonly List<string> FilesPaths;
        public readonly List<string[]> FilesContents;
        public readonly List<HashSet<string>> SuppressedWarningCodes;
        
        public AbstractSyntaxTree(List<string> filesPaths, List<string[]> filesContents, List<HashSet<string>> suppressedWarningCodes)
        {
            RootNodes = new List<FileNode>();
            ReferenceNodes = new List<ReferenceNode>();

            FilesPaths = filesPaths;
            FilesContents = filesContents;
            SuppressedWarningCodes = suppressedWarningCodes;
        }

        public void Extend(IParseTree tree, AbstractParseTreeVisitor<ASTNode> visitor, int index)
        {
            FileNode fileNode = (FileNode) visitor.Visit(tree);
            fileNode.Content = FilesContents[index];
            RootNodes.Add(fileNode);
            ReferenceNodes.AddRange(((ICommonDaedalusParseTreeVisitor)visitor).ReferenceNodes);
        }
    }

 
    public abstract class ASTNode
    {
        public readonly NodeLocation Location;

        public readonly List<NodeAnnotation> Annotations; //warnings & errors
        public ASTNode ParentNode { get; set; }
        private ASTNode _firstSignificantAncestorNode;

        protected ASTNode(NodeLocation location)
        {
            Annotations = new List<NodeAnnotation>();
            Location = location;
            _firstSignificantAncestorNode = null;
        }
        
        public ASTNode GetFirstSignificantAncestorNode()
        {
            if (_firstSignificantAncestorNode != null)
            {
                return _firstSignificantAncestorNode;
            }
            
            ASTNode node = this;
            while (node != null)
            {
                node = node.ParentNode;
                switch (node)
                {
                    case PrototypeDefinitionNode prototypeDefinitionNode:
                        _firstSignificantAncestorNode = prototypeDefinitionNode;
                        return _firstSignificantAncestorNode;
                    
                    case InstanceDefinitionNode instanceDefinitionNode:
                        _firstSignificantAncestorNode = instanceDefinitionNode;
                        return _firstSignificantAncestorNode;
                    
                    case FunctionDefinitionNode functionDefinitionNode:
                        _firstSignificantAncestorNode = functionDefinitionNode;
                        return _firstSignificantAncestorNode;
                    
                    case ClassDefinitionNode classDefinitionNode:
                        _firstSignificantAncestorNode = classDefinitionNode;
                        return _firstSignificantAncestorNode;
                    
                    case FileNode fileNode:
                        _firstSignificantAncestorNode = fileNode;
                        return _firstSignificantAncestorNode;
                }
            }
            throw new Exception();
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
        public SymbolType BuiltinType;
        public Symbol ComplexType;
        protected ExpressionNode(NodeLocation location) : base(location)
        {
            BuiltinType = SymbolType.Uninitialized;
            ComplexType = null;
        }
    }

    public abstract class DeclarationNode : StatementNode
    {
        public readonly string TypeNameCapitalized;
        public readonly NameNode NameNode;
        public readonly List<ASTNode> Usages; //ReferenceNode, AttributeNode(ReferencePartNode), CustomTypeDeclarationNode // TODO maybe NameNode only?

        public Symbol Symbol;

        protected DeclarationNode(NodeLocation location, string typeName, NameNode nameNode) : base(location)
        {
            TypeNameCapitalized = typeName.First().ToString().ToUpper() + typeName.Substring(1).ToLower();
            NameNode = nameNode;
            NameNode.ParentNode = this;
            Symbol = null;
            Usages = new List<ASTNode>();
        }
    }

    public abstract class CustomTypeDeclarationNode : DeclarationNode
    {
        public readonly NameNode TypeNameNode;

        protected CustomTypeDeclarationNode(NodeLocation location, NameNode typeNameNode, NameNode nameNode) : base(location, typeNameNode.Value, nameNode)
        {
            TypeNameNode = typeNameNode;
            TypeNameNode.ParentNode = this;
        }
    }

    public abstract class InheritanceNode : DeclarationNode
    {
        protected InheritanceNode(NodeLocation location, string type, NameNode nameNode) : base(location, type, nameNode)
        {
        }
    }

    public abstract class SubclassNode : InheritanceNode
    {
        public readonly InheritanceParentReferenceNode InheritanceParentReferenceNode;
        protected SubclassNode(NodeLocation location, string type, NameNode nameNode, InheritanceParentReferenceNode inheritanceParentReferenceNode) : base(location, type, nameNode)
        {
            inheritanceParentReferenceNode.ParentNode = this;
            InheritanceParentReferenceNode = inheritanceParentReferenceNode;
        }
    }

    public interface IArrayDeclarationNode
    {
        ExpressionNode ArraySizeNode { get; set; }
        NodeValue ArraySizeValue { get; set; }
    }

    public abstract class ValueNode : ExpressionNode
    {
        protected ValueNode(NodeLocation location) : base(location)
        {
        }
    }
    
    public class FileNode : ASTNode
    {
        public readonly List<DeclarationNode> DefinitionNodes;

        public string[] Content;

        public FileNode(NodeLocation location, List<DeclarationNode> definitionNodes) :
            base(location)
        {
            foreach (var node in definitionNodes)
            {
                node.ParentNode = this;
            }
            
            DefinitionNodes = definitionNodes;
        }
    }
    
    public class ConditionalNode : StatementNode
    {
        public readonly ExpressionNode ConditionNode;
        public readonly List<StatementNode> BodyNodes;

        public ConditionalNode(NodeLocation location, ExpressionNode conditionNode, List<StatementNode> bodyNodes) :
            base(location)
        {
            conditionNode.ParentNode = this;
            foreach (var node in bodyNodes)
            {
                node.ParentNode = this;
            }
            
            ConditionNode = conditionNode;
            BodyNodes = bodyNodes;
        }
    }

    public class FunctionDefinitionNode : CustomTypeDeclarationNode
    {
        public readonly List<ParameterDeclarationNode> ParameterNodes;
        public readonly List<StatementNode> BodyNodes;
        public readonly bool IsExternal;

        public FunctionDefinitionNode(NodeLocation location, NameNode typeNameNode, NameNode nameNode,
            List<ParameterDeclarationNode> parameterNodes, List<StatementNode> bodyNodes, bool isExternal) : base(location, typeNameNode, nameNode)
        {
            foreach (var node in parameterNodes)
            {
                node.ParentNode = this;
            }
            foreach (var node in bodyNodes)
            {
                node.ParentNode = this;
            }
            
            ParameterNodes = parameterNodes;
            BodyNodes = bodyNodes;
            IsExternal = isExternal;
        }
    }

    public class AssignmentNode : StatementNode
    {
        public NodeLocation OperatorLocation;
        
        public readonly ReferenceNode LeftSideNode;
        public readonly ExpressionNode RightSideNode;

        public AssignmentNode(NodeLocation location, NodeLocation operatorLocation, ReferenceNode leftSideNode, ExpressionNode rightSideNode) :
            base(location)
        {
            leftSideNode.ParentNode = this;
            rightSideNode.ParentNode = this;

            OperatorLocation = operatorLocation;
            LeftSideNode = leftSideNode;
            RightSideNode = rightSideNode;
        }
    }

    public class CompoundAssignmentNode : StatementNode
    {
        public readonly CompoundAssignmentOperator Operator;
        public readonly NodeLocation OperatorLocation;
        public readonly ReferenceNode LeftSideNode;
        public readonly ExpressionNode RightSideNode;

        public CompoundAssignmentNode(NodeLocation location, CompoundAssignmentOperator @operator, NodeLocation operatorLocation, ReferenceNode leftSideNode,
            ExpressionNode rightSideNode) : base(location)
        {
            leftSideNode.ParentNode = this;
            rightSideNode.ParentNode = this;
            
            Operator = @operator;
            OperatorLocation = operatorLocation;
            LeftSideNode = leftSideNode;
            RightSideNode = rightSideNode;
        }
    }
    
    public class UnaryExpressionNode : ExpressionNode
    {
        public readonly UnaryOperator Operator;
        public readonly NodeLocation OperatorLocation;
        public readonly ExpressionNode ExpressionNode;

        public bool DoGenerateOperatorInstruction;

        public UnaryExpressionNode(NodeLocation location, UnaryOperator @operator, NodeLocation operatorLocation, ExpressionNode expressionNode) : base(
            location)
        {
            expressionNode.ParentNode = this;
            
            Operator = @operator;
            ExpressionNode = expressionNode;
            OperatorLocation = operatorLocation;

            DoGenerateOperatorInstruction = true;
        }
    }

    public class BinaryExpressionNode : ExpressionNode
    {
        public readonly BinaryOperator Operator;
        public readonly NodeLocation OperatorLocation;
        public readonly ExpressionNode LeftSideNode;
        public readonly ExpressionNode RightSideNode;

        public BinaryExpressionNode(NodeLocation location, BinaryOperator @operator, NodeLocation operatorLocation, ExpressionNode leftSideNode,
            ExpressionNode rightSideNode) : base(location)
        {
            leftSideNode.ParentNode = this;
            rightSideNode.ParentNode = this;
            
            Operator = @operator;
            LeftSideNode = leftSideNode;
            RightSideNode = rightSideNode;
            OperatorLocation = operatorLocation;
        }
    }


    public class ClassDefinitionNode : InheritanceNode
    {
        public readonly List<DeclarationNode> AttributeNodes;

        public ClassDefinitionNode(NodeLocation location, NameNode nameNode, List<DeclarationNode> attributeNodes) :
            base(location, "class", nameNode)
        {
            foreach (var node in attributeNodes)
            {
                node.ParentNode = this;
            }
            
            AttributeNodes = attributeNodes;
        }
    }

    public class PrototypeDefinitionNode : SubclassNode
    {
        public readonly List<StatementNode> BodyNodes;

        public PrototypeDefinitionNode(NodeLocation location, NameNode nameNode, InheritanceParentReferenceNode inheritanceParentReferenceNode,
            List<StatementNode> bodyNodes) : base(location, "prototype", nameNode, inheritanceParentReferenceNode)
        {
            foreach (var node in bodyNodes)
            {
                node.ParentNode = this;
            }
            BodyNodes = bodyNodes;
        }
    }

    public class InstanceDefinitionNode : SubclassNode
    {
        public readonly List<StatementNode> BodyNodes;
        public readonly bool DefinedWithoutBody;

        public InstanceDefinitionNode(NodeLocation location, NameNode nameNode, InheritanceParentReferenceNode inheritanceParentReferenceNode,
            List<StatementNode> bodyNodes, bool definedWithoutBody) : base(location, "instance", nameNode, inheritanceParentReferenceNode)
        {
            foreach (var node in bodyNodes)
            {
                node.ParentNode = this;
            }
            BodyNodes = bodyNodes;
            DefinedWithoutBody = definedWithoutBody;

        }
    }
    
    public class ConstDefinitionNode : VarDeclarationNode
    {
        public NodeValue RightSideValue;

        public ConstDefinitionNode(NodeLocation location, NameNode typeNameNode, NameNode nameNode,
            ExpressionNode rightSideNode) : base(location, typeNameNode, nameNode, rightSideNode)
        {
        }
        
        protected ConstDefinitionNode(NodeLocation location, NameNode typeNameNode, NameNode nameNode) : base(location, typeNameNode, nameNode)
        {
        }
    }

    public class ConstArrayDefinitionNode : ConstDefinitionNode, IArrayDeclarationNode
    {
        public readonly List<ExpressionNode> ElementNodes;
        public readonly List<NodeValue> ElementValues;
        public ExpressionNode ArraySizeNode { get; set; }
        public NodeValue ArraySizeValue { get; set; }
        
        public ConstArrayDefinitionNode(NodeLocation location, NameNode typeNameNode, NameNode nameNode,
            ExpressionNode arraySizeNode, List<ExpressionNode> elementNodes) : base(location, typeNameNode, nameNode)
        {
            ArraySizeNode = arraySizeNode;
            ArraySizeNode.ParentNode = this;
            
            foreach (var node in elementNodes)
            {
                node.ParentNode = this;
            }
            
            ElementNodes = elementNodes;
            ElementValues = new List<NodeValue>();
        }
    }

    public class VarDeclarationNode : CustomTypeDeclarationNode
    {
        public readonly ExpressionNode RightSideNode;
        public VarDeclarationNode(NodeLocation location, NameNode typeNameNode, NameNode nameNode, ExpressionNode rightSideNode) : base(location, typeNameNode,
            nameNode)
        {
            if (rightSideNode != null)
            {
                rightSideNode.ParentNode = this;
            }
            RightSideNode = rightSideNode;
        }
        
        public VarDeclarationNode(NodeLocation location, NameNode typeNameNode, NameNode nameNode) : base(location, typeNameNode,
            nameNode)
        {
        }
    }

    public class VarArrayDeclarationNode : VarDeclarationNode, IArrayDeclarationNode
    {
        public readonly List<ExpressionNode> ElementNodes;
        public ExpressionNode ArraySizeNode { get; set; }
        public NodeValue ArraySizeValue { get; set; }
        
        public VarArrayDeclarationNode(NodeLocation location, NameNode typeNameNode, NameNode nameNode,
            ExpressionNode arraySizeNode, List<ExpressionNode> elementNodes) : base(location, typeNameNode, nameNode)
        {
            ArraySizeNode = arraySizeNode;
            ArraySizeNode.ParentNode = this;

            if (elementNodes != null)
            {
                foreach (var node in elementNodes)
                {
                    node.ParentNode = this;
                }

            }
            
            ElementNodes = elementNodes;
        }
    }
    
    public class ParameterDeclarationNode : VarDeclarationNode
    {
        public ParameterDeclarationNode(NodeLocation location, NameNode typeNameNode, NameNode nameNode) : base(location, typeNameNode,
            nameNode)
        {
        }
    }

    public class ParameterArrayDeclarationNode : ParameterDeclarationNode, IArrayDeclarationNode
    {
        public ExpressionNode ArraySizeNode { get; set; }
        public NodeValue ArraySizeValue { get; set; }
        public ParameterArrayDeclarationNode(NodeLocation location, NameNode typeNameNode, NameNode nameNode,
            ExpressionNode arraySizeNode) : base(location, typeNameNode, nameNode)
        {
            ArraySizeNode = arraySizeNode;
            ArraySizeNode.ParentNode = this;
        }
    }
    public class ReturnStatementNode : StatementNode
    {
        public readonly ExpressionNode ExpressionNode;

        public ReturnStatementNode(NodeLocation location, ExpressionNode expressionNode) : base(location)
        {
            ExpressionNode = expressionNode;
            if (ExpressionNode != null)
            {
                ExpressionNode.ParentNode = this;
            }
            
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
        public readonly ReferenceNode FunctionReferenceNode;
        public readonly List<ExpressionNode> ArgumentNodes;

        public FunctionCallNode(NodeLocation location, ReferenceNode functionReferenceNode,
            List<ExpressionNode> argumentNodes) : base(location)
        {
            functionReferenceNode.ParentNode = this;
            foreach (var node in argumentNodes)
            {
                node.ParentNode = this;
            }
            
            
            FunctionReferenceNode = functionReferenceNode;
            ArgumentNodes = argumentNodes;
        }
    }

    public class IfStatementNode : StatementNode
    {
        public readonly ConditionalNode IfNode;
        public readonly List<ConditionalNode> ElseIfNodes;
        public readonly List<StatementNode> ElseNodeBodyNodes;

        public IfStatementNode(NodeLocation location, ConditionalNode ifNode,
            List<ConditionalNode> elseIfNodes, List<StatementNode> elseNodeBodyNodes) : base(location)
        {
            ifNode.ParentNode = this;
            foreach (var node in elseIfNodes)
            {
                node.ParentNode = this;
            }

            if (elseNodeBodyNodes != null)
            {
                foreach (var node in elseNodeBodyNodes)
                {
                    node.ParentNode = this;
                }
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
        public long Value;
        public readonly bool EvaluatedCorrectly;

        public bool DoCastToFloat;

        public IntegerLiteralNode(NodeLocation location, long value, bool evaluatedCorrectly=true) : base(location)
        {
            Value = value;
            EvaluatedCorrectly = evaluatedCorrectly;
            DoCastToFloat = false;
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
        public readonly string Value;
        public StringConstSymbol Symbol; //filled in SymbolTableCreationVisitor

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

    public abstract class ReferencePartNode : ASTNode {
        protected ReferencePartNode(NodeLocation location) : base(location)
        {
        }
    }
    
    class AttributeNode : ReferencePartNode
    {
        public readonly string Name;
        public Symbol Symbol;
        public ArrayIndexNode ArrayIndexNode;
        
        public AttributeNode(string name, NodeLocation location) : base(location)
        {
            Name = name;
            ArrayIndexNode = null;
        }
    }

    public class ArrayIndexNode : ReferencePartNode
    {
        public readonly ExpressionNode ExpressionNode;
        public NodeValue Value;
        public ArrayIndexNode(ExpressionNode expressionNode, NodeLocation location) : base(location)
        {
            ExpressionNode = expressionNode;
            ExpressionNode.ParentNode = this;
        }
    }

    

    public class ReferenceNode : ExpressionNode
    {
        public string Name;
        public readonly List<ReferencePartNode> PartNodes;
        
        public Symbol Symbol; // TODO when should it be filled?
        public Symbol BaseSymbol; // if for example we have a.b.c, base symbol is a, and symbol is c
        public ArrayIndexNode IndexNode;

        public bool DoCastToInt;
        public bool DoesHaveNestedAttributes;

        public ReferenceNode(string name, List<ReferencePartNode> partNodes, NodeLocation location) : base(location)
        {
            Symbol = null;
            BaseSymbol = null;
            Name = name;
            PartNodes = partNodes;
            IndexNode = null;

            DoCastToInt = false;
            DoesHaveNestedAttributes = false;
            /*
             When CastToInt = true, this node should produce PushInt instruction.
             It should happen in following situations:
              - assignment/return/parameter of type func
              - assignment/return/parameter of type int && symbol's builtin type isn't int
              - inside conditional
              */

            foreach (var partNode in partNodes)
            {
                partNode.ParentNode = this;
            }
        }
        
        public ReferenceNode(string name, NodeLocation location) : base(location)
        {
            Symbol = null;
            Name = name;
            PartNodes = new List<ReferencePartNode>();
        }
    }

    public class InheritanceParentReferenceNode : ReferenceNode
    {
        public InheritanceParentReferenceNode(string name, NodeLocation location) : base(name, location)
        {
        }
    }

    public class NameNode : ASTNode
    {
        public readonly string Value;

        public NameNode(NodeLocation location, string value) : base(location)
        {
            Value = value;
        }
    }
    
    
    /// <remarks>Helper nodes used only during AST construction. These nodes don't appear in the result AST.</remarks>
    public abstract class TemporaryNode : ASTNode
    {
        public readonly List<DeclarationNode> Nodes;

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