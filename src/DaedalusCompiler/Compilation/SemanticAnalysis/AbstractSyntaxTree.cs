using System;
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

        public int EndColumn;
    }

    public class AbstractSyntaxTree
    {
        public List<FileNode> RootNodes;

        public readonly List<InheritanceParentReferenceNode> InheritanceReferenceNodes;
        public readonly List<ReferenceNode> ReferenceNodes;
        public readonly List<ConstDefinitionNode> ConstDefinitionNodes;
        public readonly List<IArrayDeclarationNode> ArrayDeclarationNodes;

        public List<string> FilesPaths;
        public List<string[]> FilesContents;
        public List<HashSet<string>> SuppressedWarningCodes;
        
        public AbstractSyntaxTree(List<IParseTree> parseTrees, int externalFilesCount, List<string> filesPaths, List<string[]> filesContents, List<HashSet<string>> suppressedWarningCodes)
        {
            RootNodes = new List<FileNode>();
            ConstDefinitionNodes = new List<ConstDefinitionNode>();
            ArrayDeclarationNodes = new List<IArrayDeclarationNode>();
            InheritanceReferenceNodes = new List<InheritanceParentReferenceNode>();
            ReferenceNodes = new List<ReferenceNode>();

            FilesPaths = filesPaths;
            FilesContents = filesContents;
            SuppressedWarningCodes = suppressedWarningCodes;
            
            int index = 0;
            foreach (IParseTree parseTree in parseTrees)
            {
                ParseTreeVisitor visitor = new ParseTreeVisitor(index);
                RootNodes.Add((FileNode) visitor.Visit(parseTree));
                ConstDefinitionNodes.AddRange(visitor.ConstDefinitionNodes);
                ArrayDeclarationNodes.AddRange(visitor.ArrayDeclarationNodes);
                InheritanceReferenceNodes.AddRange(visitor.InheritanceReferenceNodes);
                ReferenceNodes.AddRange(visitor.ReferenceNodes);
                index++;
            }

            for (int i = 0; i < externalFilesCount; ++i)
            {
                RootNodes[i].IsExternal = true;
            }

            for (int i = 0; i < RootNodes.Count; ++i)
            {
                //RootNodes[i].Index = i;
                //RootNodes[i].Path = FilesPaths[i];
                RootNodes[i].Content = FilesContents[i];
                //RootNodes[i].SuppressedWarningCodes = SuppressedWarningCodes[i];
            }
        }
    }

 
    public abstract class ASTNode
    {
        public NodeLocation Location;

        public readonly List<NodeAnnotation> Annotations; //warnings & errors
        public ASTNode ParentNode { get; set; }
        
        //public ASTNode ParentBlockNode { get; set; }

        protected ASTNode(NodeLocation location)
        {
            Annotations = new List<NodeAnnotation>();
            Location = location;
        }
        
        public ASTNode GetFirstSignificantAncestorNode()
        {
            ASTNode node = this;
            while (node != null)
            {
                node = node.ParentNode;
                switch (node)
                {
                    case PrototypeDefinitionNode prototypeDefinitionNode:
                        return prototypeDefinitionNode;
                    case InstanceDefinitionNode instanceDefinitionNode:
                        return instanceDefinitionNode;
                    case FunctionDefinitionNode functionDefinitionNode:
                        return functionDefinitionNode;
                    case ClassDefinitionNode classDefinitionNode:
                        return classDefinitionNode;
                    case FileNode fileNode:
                        return fileNode;
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
            TypeNameCapitalized = typeName.First().ToString().ToUpper() + typeName.Substring(1).ToLower(); //capitalized
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
        public InheritanceParentReferenceNode InheritanceParentReferenceNode;
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
        public List<DeclarationNode> DefinitionNodes;
        public bool IsExternal;


        //public int Index;
        //public string Path;
        public string[] Content;
        //public string[] SuppressedWarningCodes;
        
        
        public FileNode(NodeLocation location, List<DeclarationNode> definitionNodes, bool isExternal = false) :
            base(location)
        {
            foreach (var node in definitionNodes)
            {
                node.ParentNode = this;
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

        public FunctionDefinitionNode(NodeLocation location, NameNode typeNameNode, NameNode nameNode,
            List<ParameterDeclarationNode> parameterNodes, List<StatementNode> bodyNodes) : base(location, typeNameNode, nameNode)
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
        }
    }

    public class AssignmentNode : StatementNode
    {
        public NodeLocation OperatorLocation;
        
        public ReferenceNode LeftSideNode;
        public ExpressionNode RightSideNode;

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
        public CompoundAssignmentOperator Operator;
        public NodeLocation OperatorLocation;
        public ReferenceNode LeftSideNode;
        public ExpressionNode RightSideNode;

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
        public UnaryOperator Operator;
        public NodeLocation OperatorLocation;
        public ExpressionNode ExpressionNode;

        public UnaryExpressionNode(NodeLocation location, UnaryOperator @operator, NodeLocation operatorLocation, ExpressionNode expressionNode) : base(
            location)
        {
            expressionNode.ParentNode = this;
            
            Operator = @operator;
            ExpressionNode = expressionNode;
            OperatorLocation = operatorLocation;
        }
    }

    public class BinaryExpressionNode : ExpressionNode
    {
        public BinaryOperator Operator;
        public NodeLocation OperatorLocation;
        public ExpressionNode LeftSideNode;
        public ExpressionNode RightSideNode;

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
        public List<StatementNode> BodyNodes;

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
        public List<StatementNode> BodyNodes;

        public InstanceDefinitionNode(NodeLocation location, NameNode nameNode, InheritanceParentReferenceNode inheritanceParentReferenceNode,
            List<StatementNode> bodyNodes) : base(location, "instance", nameNode, inheritanceParentReferenceNode)
        {
            foreach (var node in bodyNodes)
            {
                node.ParentNode = this;
            }
            BodyNodes = bodyNodes;
        }
    }
    
    public class ConstDefinitionNode : VarDeclarationNode
    {
        public ExpressionNode RightSideNode;
        public NodeValue RightSideValue;

        public ConstDefinitionNode(NodeLocation location, NameNode typeNameNode, NameNode nameNode,
            ExpressionNode rightSideNode) : base(location, typeNameNode, nameNode)
        {
            rightSideNode.ParentNode = this;
            
            RightSideNode = rightSideNode;
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
        public VarDeclarationNode(NodeLocation location, NameNode typeNameNode, NameNode nameNode) : base(location, typeNameNode,
            nameNode)
        {
        }
    }

    public class VarArrayDeclarationNode : VarDeclarationNode, IArrayDeclarationNode
    {
        public ExpressionNode ArraySizeNode { get; set; }
        public NodeValue ArraySizeValue { get; set; }
        
        public VarArrayDeclarationNode(NodeLocation location, NameNode typeNameNode, NameNode nameNode,
            ExpressionNode arraySizeNode) : base(location, typeNameNode, nameNode)
        {
            ArraySizeNode = arraySizeNode;
            ArraySizeNode.ParentNode = this;
        }
    }
    
    public class ParameterDeclarationNode : VarDeclarationNode
    {
        //public new FunctionDefinitionNode Parent { get; set; }
        
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
        public ReferenceNode FunctionReferenceNode;
        public List<ExpressionNode> ArgumentNodes;

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
        public ConditionalNode IfNode;
        public List<ConditionalNode> ElseIfNodes;
        public List<StatementNode> ElseNodeBodyNodes;

        public IfStatementNode(NodeLocation location, ConditionalNode ifNode,
            List<ConditionalNode> elseIfNodes, List<StatementNode> elseNodeBodyNodes) : base(location)
        {
            ifNode.ParentNode = this;
            foreach (var node in elseIfNodes)
            {
                node.ParentNode = this;
            }
            foreach (var node in elseNodeBodyNodes)
            {
                node.ParentNode = this;
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
        public bool EvaluatedCorrectly;

        public IntegerLiteralNode(NodeLocation location, long value, bool evaluatedCorrectly=true) : base(location)
        {
            Value = value;
            EvaluatedCorrectly = evaluatedCorrectly;
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
        public string Name;
        public Symbol Symbol;
        
        public AttributeNode(string name, NodeLocation location) : base(location)
        {
            Name = name;
        }
    }

    public class ArrayIndexNode : ReferencePartNode
    {
        public ExpressionNode ExpressionNode;
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
        public List<ReferencePartNode> PartNodes;

        public Symbol Symbol; // TODO filled in ???
        public Symbol BaseSymbol; // if for example we have a.b.c, base symbol is a, and symbol is c
        

        public ReferenceNode(string name, List<ReferencePartNode> partNodes, NodeLocation location) : base(location)
        {
            Symbol = null;
            BaseSymbol = null;
            Name = name;
            PartNodes = partNodes;

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