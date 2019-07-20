using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using DaedalusCompiler.Compilation.SemanticAnalysis;

namespace DaedalusCompiler.Compilation
{
    public class ParseTreeVisitor : DaedalusBaseVisitor<ASTNode>
    {
	    private readonly int _sourceFileNumber;
	    public bool IsVisitingExternal;

	    public ParseTreeVisitor(int sourceFileNumber)
	    {
		    _sourceFileNumber = sourceFileNumber;
	    }

	    public override ASTNode VisitDaedalusFile([NotNull] DaedalusParser.DaedalusFileContext context)
	    {
		    List<DeclarationNode> definitionNodes = new List<DeclarationNode>();
		    foreach (IParseTree childContext in context.children)
		    {
			    if (childContext is TerminalNodeImpl)
			    {
				    continue;
			    }
			    
			    if (childContext is DaedalusParser.InlineDefContext inlineDefContext)
			    {
				    definitionNodes.AddRange(((TemporaryNode) Visit(inlineDefContext.GetChild(0))).Nodes);
			    }
			    
			    else if (childContext is DaedalusParser.BlockDefContext blockDefContext)
			    {
				    definitionNodes.Add((DeclarationNode) Visit(blockDefContext.GetChild(0)));
			    }
		    }
		    return new FileNode(GetLocation(context), definitionNodes);
	    }
	    

	    public override ASTNode VisitFunctionDef([NotNull] DaedalusParser.FunctionDefContext context)
	    {
		    string type = context.dataType().GetText();
		    NameNode nameNode = new NameNode(GetLocation(context.nameNode()),context.nameNode().GetText());

		    List<ParameterDeclarationNode> varDeclarationNodes = new List<ParameterDeclarationNode>();
			foreach (DaedalusParser.ParameterDeclContext parameterDeclContext in context.parameterList().parameterDecl())
			{
				varDeclarationNodes.Add((ParameterDeclarationNode) VisitParameterDecl(parameterDeclContext));
			}

			List<StatementNode> statementNodes = GetStatementNodes(context.statementBlock());
			
			return new FunctionDefinitionNode(GetLocation(context), type, nameNode, varDeclarationNodes, statementNodes);
	    }

		public override ASTNode VisitConstDef([NotNull] DaedalusParser.ConstDefContext context)
		{
			return GetConstDefinitionsTemporaryNode(context);
		}
		
		public override ASTNode VisitInstanceDecl([NotNull] DaedalusParser.InstanceDeclContext context)
		{
			return GetInstanceDeclarationsTemporaryNode(context);
		}

		public override ASTNode VisitVarDecl([NotNull] DaedalusParser.VarDeclContext context)
		{
			return GetVarDeclarationsTemporaryNode(context);
		}

		public override ASTNode VisitClassDef([NotNull] DaedalusParser.ClassDefContext context)
		{
			NameNode nameNode = new NameNode(GetLocation(context.nameNode()),context.nameNode().GetText());
			List<DeclarationNode> varDeclarationNodes = new List<DeclarationNode>();
			foreach (DaedalusParser.VarDeclContext varDeclContext in context.varDecl())
			{
				ASTNode node = VisitVarDecl(varDeclContext);
				
				
				if (node is TemporaryNode temporaryNode)
				{
					varDeclarationNodes.AddRange(temporaryNode.Nodes);
				}
				else
				{
					varDeclarationNodes.Add((VarDeclarationNode) node);
				}
			}
			return new ClassDefinitionNode(GetLocation(context), nameNode, varDeclarationNodes);
		}

		public override ASTNode VisitPrototypeDef([NotNull] DaedalusParser.PrototypeDefContext prototypeDefContext)
		{
			NameNode nameNode = new NameNode(GetLocation(prototypeDefContext.nameNode()), prototypeDefContext.nameNode().GetText());
			DaedalusParser.ParentReferenceContext parentReferenceContext = prototypeDefContext.parentReference();
			ReferenceNode parentReferenceNode = new ReferenceNode(GetLocation(parentReferenceContext), parentReferenceContext.GetText());
			List<StatementNode> statementNodes = GetStatementNodes(prototypeDefContext.statementBlock());
			return new PrototypeDefinitionNode(GetLocation(prototypeDefContext), nameNode, parentReferenceNode, statementNodes);
		}

		public override ASTNode VisitInstanceDef([NotNull] DaedalusParser.InstanceDefContext instanceDefContext)
		{
			NameNode nameNode = new NameNode(GetLocation(instanceDefContext.nameNode()), instanceDefContext.nameNode().GetText());
			DaedalusParser.ParentReferenceContext parentReferenceContext = instanceDefContext.parentReference();
			ReferenceNode parentReferenceNode = new ReferenceNode(GetLocation(parentReferenceContext), parentReferenceContext.GetText());
			List<StatementNode> statementNodes = GetStatementNodes(instanceDefContext.statementBlock());
			return new InstanceDefinitionNode(GetLocation(instanceDefContext), nameNode, parentReferenceNode, statementNodes);
		}
		
		public override ASTNode VisitParameterDecl([NotNull] DaedalusParser.ParameterDeclContext context)
		{
			NodeLocation location = GetLocation(context);
			string type = context.dataType().GetText();
			NameNode name = new NameNode(GetLocation(context.nameNode()),context.nameNode().GetText());
			if (context.arraySize() != null)
			{
				var arraySize = (ExpressionNode) VisitArraySize(context.arraySize());
				return new ParameterArrayDeclarationNode(location, type, name, arraySize);
			}
			return new ParameterDeclarationNode(location, type, name);
		}

		public override ASTNode VisitStatement([NotNull] DaedalusParser.StatementContext context)
		{
			return Visit(context.GetChild(0));
		}

		public override ASTNode VisitFunctionCall([NotNull] DaedalusParser.FunctionCallContext context)
		{
			DaedalusParser.NameNodeContext nameNodeContext = context.nameNode();
			ReferenceNode referenceNode = new ReferenceNode(GetLocation(nameNodeContext), nameNodeContext.GetText());
			List<ExpressionNode> expressionNodes = new List<ExpressionNode>();
			foreach (DaedalusParser.ExpressionContext expressionContext in context.expression())
			{
				expressionNodes.Add((ExpressionNode) Visit(expressionContext));
			}

			return new FunctionCallNode(GetLocation(context), referenceNode, expressionNodes);

		}

		public override ASTNode VisitAssignment([NotNull] DaedalusParser.AssignmentContext context)
		{
			string oper = context.assignmentOperator().GetText();
			ReferenceNode referenceNode = (ReferenceNode) VisitReference(context.reference());
			ExpressionNode expressionNode = (ExpressionNode) Visit(context.expression());
			
			if (oper == "=")
			{
				return new AssignmentNode(GetLocation(context), referenceNode, expressionNode);
			}
			return new CompoundAssignmentNode(GetLocation(context), GetCompoundAssignmentOperator(oper), referenceNode, expressionNode);
		}
		
		public override ASTNode VisitElseIfBlock([NotNull] DaedalusParser.ElseIfBlockContext context)
		{
			ExpressionNode conditionNode = (ExpressionNode) Visit(context.expression());
			List<StatementNode> statementNodes = GetStatementNodes(context.statementBlock());
			return new ConditionalNode(GetLocation(context), conditionNode, statementNodes);
		}

		public override ASTNode VisitIfBlock([NotNull] DaedalusParser.IfBlockContext context)
		{
			ExpressionNode conditionNode = (ExpressionNode) Visit(context.expression());
			List<StatementNode> statementNodes = GetStatementNodes(context.statementBlock());
			return new ConditionalNode(GetLocation(context), conditionNode, statementNodes);
		}

		public override ASTNode VisitIfBlockStatement([NotNull] DaedalusParser.IfBlockStatementContext context)
		{
			ConditionalNode ConditionalNode = (ConditionalNode) VisitIfBlock(context.ifBlock());
			List<ConditionalNode> ConditionalNodes = new List<ConditionalNode>();
			List<StatementNode> elseNodeBodyNodes = new List<StatementNode>();
			
			foreach (DaedalusParser.ElseIfBlockContext elseIfBlockContext in context.elseIfBlock())
			{
				ConditionalNodes.Add((ConditionalNode) VisitElseIfBlock(elseIfBlockContext));
			}
			
			if (context.elseBlock() != null)
			{
				elseNodeBodyNodes = GetStatementNodes(context.elseBlock().statementBlock());
			}
			
			return new IfStatementNode(GetLocation(context), ConditionalNode, ConditionalNodes, elseNodeBodyNodes);
		}

		public override ASTNode VisitWhileStatement([NotNull] DaedalusParser.WhileStatementContext context)
		{
			ExpressionNode conditionNode = (ExpressionNode) Visit(context.expression());
			List<StatementNode> statementNodes = GetStatementNodes(context.statementBlock());
			return new WhileStatementNode(GetLocation(context), conditionNode, statementNodes);
		}
		
		public override ASTNode VisitReturnStatement([NotNull] DaedalusParser.ReturnStatementContext context)
		{
			return new ReturnStatementNode(GetLocation(context));
		}

		public override ASTNode VisitBreakStatement([NotNull] DaedalusParser.BreakStatementContext context)
		{
			return new BreakStatementNode(GetLocation(context));
		}

		public override ASTNode VisitContinueStatement([NotNull] DaedalusParser.ContinueStatementContext context)
		{
			return new ContinueStatementNode(GetLocation(context));
		}

		public override ASTNode VisitBracketExpression([NotNull] DaedalusParser.BracketExpressionContext context)
		{
			return Visit(context.expression());
		}

		public override ASTNode VisitUnaryExpression([NotNull] DaedalusParser.UnaryExpressionContext context)
		{
			ExpressionNode expressionNode = (ExpressionNode) Visit(context.expression());
			return new UnaryExpressionNode(GetLocation(context), GetUnaryOperator(context.oper.GetText()), expressionNode);
		}

		public override ASTNode VisitBitMoveExpression([NotNull] DaedalusParser.BitMoveExpressionContext context)
		{
			return CreateBinaryExpressionNode(GetLocation(context), context.oper.GetText(), context.expression());
		}

		public override ASTNode VisitEqExpression([NotNull] DaedalusParser.EqExpressionContext context)
		{
			return CreateBinaryExpressionNode(GetLocation(context), context.oper.GetText(), context.expression());
		}

		public override ASTNode VisitAddExpression([NotNull] DaedalusParser.AddExpressionContext context)
		{
			return CreateBinaryExpressionNode(GetLocation(context), context.oper.GetText(), context.expression());
		}

		public override ASTNode VisitCompExpression([NotNull] DaedalusParser.CompExpressionContext context)
		{
			return CreateBinaryExpressionNode(GetLocation(context), context.oper.GetText(), context.expression());
		}

		public override ASTNode VisitLogOrExpression([NotNull] DaedalusParser.LogOrExpressionContext context)
		{
			return CreateBinaryExpressionNode(GetLocation(context), context.oper.GetText(), context.expression());
		}

		public override ASTNode VisitBinAndExpression([NotNull] DaedalusParser.BinAndExpressionContext context)
		{
			return CreateBinaryExpressionNode(GetLocation(context), context.oper.GetText(), context.expression());
		}

		public override ASTNode VisitBinOrExpression([NotNull] DaedalusParser.BinOrExpressionContext context)
		{
			return CreateBinaryExpressionNode(GetLocation(context), context.oper.GetText(), context.expression());
		}

		public override ASTNode VisitMultExpression([NotNull] DaedalusParser.MultExpressionContext context)
		{
			return CreateBinaryExpressionNode(GetLocation(context), context.oper.GetText(), context.expression());
		}


		public override ASTNode VisitLogAndExpression([NotNull] DaedalusParser.LogAndExpressionContext context)
		{
			return CreateBinaryExpressionNode(GetLocation(context), context.oper.GetText(), context.expression());
		}


		public override ASTNode VisitValueExpression([NotNull] DaedalusParser.ValueExpressionContext context)
		{
			return Visit(context.GetChild(0));
		}

		public override ASTNode VisitArrayIndex([NotNull] DaedalusParser.ArrayIndexContext context)
		{
			if (context.referenceAtom() != null)
			{
				return VisitReferenceAtom(context.referenceAtom());
			}
			return new IntegerLiteralNode(GetLocation(context), int.Parse(context.GetText()));
		}

		public override ASTNode VisitArraySize([NotNull] DaedalusParser.ArraySizeContext context)
		{
			if (context.referenceAtom() != null)
			{
				return VisitReferenceAtom(context.referenceAtom());
			}
			return new IntegerLiteralNode(GetLocation(context), int.Parse(context.GetText()));
		}

		public override ASTNode VisitIntegerLiteralValue([NotNull] DaedalusParser.IntegerLiteralValueContext context)
		{
			return new IntegerLiteralNode(GetLocation(context), int.Parse(context.GetText()));
		}

		public override ASTNode VisitFloatLiteralValue([NotNull] DaedalusParser.FloatLiteralValueContext context)
		{
			return new FloatLiteralNode(GetLocation(context), float.Parse(context.GetText()));
		}

		public override ASTNode VisitStringLiteralValue([NotNull] DaedalusParser.StringLiteralValueContext context)
		{
			return new StringLiteralNode(GetLocation(context), context.GetText().Replace("\"", ""));
		}

		public override ASTNode VisitNullLiteralValue([NotNull] DaedalusParser.NullLiteralValueContext context)
		{
			return new NullNode(GetLocation(context));
		}

		public override ASTNode VisitNoFuncLiteralValue([NotNull] DaedalusParser.NoFuncLiteralValueContext context)
		{
			return new NoFuncNode(GetLocation(context));
		}

		public override ASTNode VisitFunctionCallValue([NotNull] DaedalusParser.FunctionCallValueContext context)
		{
			return VisitFunctionCall(context.functionCall());
		}

		public override ASTNode VisitReferenceValue([NotNull] DaedalusParser.ReferenceValueContext context)
		{
			return VisitReference(context.reference());
		}

		public override ASTNode VisitReferenceAtom([NotNull] DaedalusParser.ReferenceAtomContext context)
		{
			ExpressionNode arrayIndex = null;
			if (context.arrayIndex() != null)
			{
				arrayIndex = (ExpressionNode) VisitArrayIndex(context.arrayIndex());
			}
			return new ReferenceNode(GetLocation(context), context.nameNode().GetText(), arrayIndex);
		}

		public override ASTNode VisitReference([NotNull] DaedalusParser.ReferenceContext context)
		{
			ReferenceNode firstReferenceNode = null;
			ReferenceNode lastReferenceNode = null;
			
			foreach (var referenceAtomContext in context.referenceAtom())
			{
				ReferenceNode referenceNode = (ReferenceNode) VisitReferenceAtom(referenceAtomContext);
				if (firstReferenceNode == null)
				{
					firstReferenceNode = referenceNode;
				}
				
				if (lastReferenceNode != null)
				{
					lastReferenceNode.AttributeNode = referenceNode;
				}

				lastReferenceNode = referenceNode;
			}

			return firstReferenceNode;
		}
		
		/*
		public override ASTNode VisitDataType([NotNull] DaedalusParser.DataTypeContext context) { return VisitChildren(context); }
		*/
		
		/*
		public override ASTNode VisitNameNode([NotNull] DaedalusParser.NameNodeContext context) { return VisitChildren(context); }
		*/


		
		private InstanceDeclarationsTemporaryNode GetInstanceDeclarationsTemporaryNode(DaedalusParser.InstanceDeclContext instanceDeclContext)
		{
			DaedalusParser.ParentReferenceContext parentReferenceContext = instanceDeclContext.parentReference();
			ReferenceNode parentReferenceNode = new ReferenceNode(GetLocation(parentReferenceContext), parentReferenceContext.GetText());
			
			List<DeclarationNode> instanceDeclarationNodes = new List<DeclarationNode>();
			
			foreach (DaedalusParser.NameNodeContext nameNodeContext in instanceDeclContext.nameNode())
			{
				NameNode nameNode = new NameNode(GetLocation(nameNodeContext), nameNodeContext.GetText());
				instanceDeclarationNodes.Add(new InstanceDefinitionNode(GetLocation(instanceDeclContext), nameNode, parentReferenceNode, new List<StatementNode>()));
			}
			
			return new InstanceDeclarationsTemporaryNode(GetLocation(instanceDeclContext), instanceDeclarationNodes);
		}
		
		
		
		private ConstDefinitionsTemporaryNode GetConstDefinitionsTemporaryNode(DaedalusParser.ConstDefContext constDefContext)
		{
			string type = constDefContext.dataType().GetText();
			
			List<DeclarationNode> constDefinitionNodes = new List<DeclarationNode>();

			foreach (IParseTree childContext in constDefContext.children)
			{
				if (childContext is DaedalusParser.ConstValueDefContext constValueDefContext)
				{
					DaedalusParser.NameNodeContext nameNodeContext = constValueDefContext.nameNode();
					NameNode nameNode = new NameNode(GetLocation(nameNodeContext), nameNodeContext.GetText());
					ExpressionNode rightSideNode = (ExpressionNode) Visit(constValueDefContext.constValueAssignment().expression());
					constDefinitionNodes.Add(new ConstDefinitionNode(GetLocation(constValueDefContext), type, nameNode, rightSideNode));
				}
				else if (childContext is DaedalusParser.ConstArrayDefContext constArrayDefContext)
				{	
					DaedalusParser.NameNodeContext nameNodeContext = constArrayDefContext.nameNode();
					NameNode nameNode = new NameNode(GetLocation(nameNodeContext), nameNodeContext.GetText());
					ExpressionNode arraySizeNode = (ExpressionNode) VisitArraySize(constArrayDefContext.arraySize());
					
					List<ExpressionNode> elementNodes = new List<ExpressionNode>();
					foreach (DaedalusParser.ExpressionContext expressionContext in constArrayDefContext.constArrayAssignment().expression())
					{
						elementNodes.Add((ExpressionNode) Visit(expressionContext));
					}
					
					constDefinitionNodes.Add(new ConstArrayDefinitionNode(GetLocation(nameNodeContext), type, nameNode, arraySizeNode, elementNodes));
				}
			}
			return new ConstDefinitionsTemporaryNode(GetLocation(constDefContext), constDefinitionNodes);
		}

		
		private VarDeclarationsTemporaryNode GetVarDeclarationsTemporaryNode(DaedalusParser.VarDeclContext varDeclContext)
		{
			string type = varDeclContext.dataType().GetText();
			
			List<DeclarationNode> varDeclarationNodes = new List<DeclarationNode>();
			foreach (IParseTree childContext in varDeclContext.children)
			{
				if (childContext is DaedalusParser.VarValueDeclContext varValueDeclContext)
				{
					DaedalusParser.NameNodeContext nameNodeContext = varValueDeclContext.nameNode();
					NameNode nameNode = new NameNode(GetLocation(nameNodeContext), nameNodeContext.GetText());
					varDeclarationNodes.Add(new VarDeclarationNode(GetLocation(varValueDeclContext), type, nameNode));
				}
				else if (childContext is DaedalusParser.VarArrayDeclContext varArrayDeclContext)
				{	
					DaedalusParser.NameNodeContext nameNodeContext = varArrayDeclContext.nameNode();
					NameNode nameNode = new NameNode(GetLocation(nameNodeContext), nameNodeContext.GetText());
					ExpressionNode arraySizeNode = (ExpressionNode) VisitArraySize(varArrayDeclContext.arraySize());
					varDeclarationNodes.Add(new VarArrayDeclarationNode(GetLocation(nameNodeContext), type, nameNode, arraySizeNode));
				}
			}
			return new VarDeclarationsTemporaryNode(GetLocation(varDeclContext), varDeclarationNodes);
		}
		private List<StatementNode> GetStatementNodes(DaedalusParser.StatementBlockContext statementBlockContext)
		{
			List<StatementNode> statementNodes = new List<StatementNode>();
			foreach (IParseTree childContext in statementBlockContext.children)
			{
				if (childContext is TerminalNodeImpl)
				{
					continue;
				}

				ASTNode node = Visit(childContext);
				if (node is TemporaryNode temporaryNode)
				{
					statementNodes.AddRange(temporaryNode.Nodes);
				}
				else
				{
					statementNodes.Add((StatementNode) node);
				}
			}
			return statementNodes;
		}

		private UnaryOperator GetUnaryOperator(string oper)
		{
			switch (oper)
			{
				case "-":
					return UnaryOperator.Minus;
				case "!":
					return UnaryOperator.Not;
				case "~":
					return UnaryOperator.Negate;
				case "+":
					return UnaryOperator.Plus;
				default:
					throw new Exception();
			}
		}
		
		private CompoundAssignmentOperator GetCompoundAssignmentOperator(string oper)
		{
			switch (oper)
			{
				case "+=":
					return CompoundAssignmentOperator.Add;
				case "-=":
					return CompoundAssignmentOperator.Sub;
				case "*=":
					return CompoundAssignmentOperator.Mult;
				case "/=":
					return CompoundAssignmentOperator.Div;
				default:
					throw new Exception();
			}
		}
		
		private BinaryOperator GetBinaryOperator(string oper)
		{
			switch (oper)
			{
				case "*":
					return BinaryOperator.Mult;
				case "/":
					return BinaryOperator.Div;
				case "%":
					return BinaryOperator.Modulo;
				
				case "+":
					return BinaryOperator.Add;
				case "-":
					return BinaryOperator.Sub;
				
				case "<<":
					return BinaryOperator.ShiftLeft;
				case ">>":
					return BinaryOperator.ShiftRight;
				
				case "<":
					return BinaryOperator.Less;
				case ">":
					return BinaryOperator.Greater;
				case "<=":
					return BinaryOperator.LessOrEqual;
				case ">=":
					return BinaryOperator.GreaterOrEqual;
				
				case "==":
					return BinaryOperator.Equal;
				case "!=":
					return BinaryOperator.NotEqual;
				
				case "&":
					return BinaryOperator.BinAnd;
				case "|":
					return BinaryOperator.BinOr;
				
				case "&&":
					return BinaryOperator.LogAnd;
				case "||":
					return BinaryOperator.LogOr;
				
				default:
					throw new Exception();
			}
		}
		
		private BinaryExpressionNode CreateBinaryExpressionNode(NodeLocation location, string oper, DaedalusParser.ExpressionContext[] expressionContexts)
		{
			ExpressionNode leftSide = (ExpressionNode) Visit(expressionContexts[0]);
			ExpressionNode rightSide = (ExpressionNode) Visit(expressionContexts[1]);
			return new BinaryExpressionNode(location, GetBinaryOperator(oper), leftSide, rightSide );
		}
		
		private NodeLocation GetLocation(ParserRuleContext context)
		{
			return new NodeLocation
			{
				FileIndex = _sourceFileNumber,
				Line = context.Start.Line,
				Column = context.Start.Column,
				Index = context.Start.StartIndex,
				LinesCount = context.Stop.Line - context.Start.Line + 1,
				CharsCount = context.Stop.StopIndex - context.Start.StartIndex + 1
			};
		}
    }
}