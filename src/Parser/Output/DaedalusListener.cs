//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.7.2
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from Daedalus.g4 by ANTLR 4.7.2

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using Antlr4.Runtime.Misc;
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="DaedalusParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7.2")]
[System.CLSCompliant(false)]
public interface IDaedalusListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.daedalusFile"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDaedalusFile([NotNull] DaedalusParser.DaedalusFileContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.daedalusFile"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDaedalusFile([NotNull] DaedalusParser.DaedalusFileContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.blockDef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBlockDef([NotNull] DaedalusParser.BlockDefContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.blockDef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBlockDef([NotNull] DaedalusParser.BlockDefContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.inlineDef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInlineDef([NotNull] DaedalusParser.InlineDefContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.inlineDef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInlineDef([NotNull] DaedalusParser.InlineDefContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.functionDef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFunctionDef([NotNull] DaedalusParser.FunctionDefContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.functionDef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFunctionDef([NotNull] DaedalusParser.FunctionDefContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.constDef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConstDef([NotNull] DaedalusParser.ConstDefContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.constDef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConstDef([NotNull] DaedalusParser.ConstDefContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.classDef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterClassDef([NotNull] DaedalusParser.ClassDefContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.classDef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitClassDef([NotNull] DaedalusParser.ClassDefContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.prototypeDef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPrototypeDef([NotNull] DaedalusParser.PrototypeDefContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.prototypeDef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPrototypeDef([NotNull] DaedalusParser.PrototypeDefContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.instanceDef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInstanceDef([NotNull] DaedalusParser.InstanceDefContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.instanceDef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInstanceDef([NotNull] DaedalusParser.InstanceDefContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.instanceDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterInstanceDecl([NotNull] DaedalusParser.InstanceDeclContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.instanceDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitInstanceDecl([NotNull] DaedalusParser.InstanceDeclContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.varDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterVarDecl([NotNull] DaedalusParser.VarDeclContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.varDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitVarDecl([NotNull] DaedalusParser.VarDeclContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.constArrayDef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConstArrayDef([NotNull] DaedalusParser.ConstArrayDefContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.constArrayDef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConstArrayDef([NotNull] DaedalusParser.ConstArrayDefContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.constArrayAssignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConstArrayAssignment([NotNull] DaedalusParser.ConstArrayAssignmentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.constArrayAssignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConstArrayAssignment([NotNull] DaedalusParser.ConstArrayAssignmentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.constValueDef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConstValueDef([NotNull] DaedalusParser.ConstValueDefContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.constValueDef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConstValueDef([NotNull] DaedalusParser.ConstValueDefContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.constValueAssignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConstValueAssignment([NotNull] DaedalusParser.ConstValueAssignmentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.constValueAssignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConstValueAssignment([NotNull] DaedalusParser.ConstValueAssignmentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.varArrayDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterVarArrayDecl([NotNull] DaedalusParser.VarArrayDeclContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.varArrayDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitVarArrayDecl([NotNull] DaedalusParser.VarArrayDeclContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.varValueDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterVarValueDecl([NotNull] DaedalusParser.VarValueDeclContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.varValueDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitVarValueDecl([NotNull] DaedalusParser.VarValueDeclContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.parameterList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParameterList([NotNull] DaedalusParser.ParameterListContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.parameterList"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParameterList([NotNull] DaedalusParser.ParameterListContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.parameterDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParameterDecl([NotNull] DaedalusParser.ParameterDeclContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.parameterDecl"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParameterDecl([NotNull] DaedalusParser.ParameterDeclContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.statementBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStatementBlock([NotNull] DaedalusParser.StatementBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.statementBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStatementBlock([NotNull] DaedalusParser.StatementBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStatement([NotNull] DaedalusParser.StatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStatement([NotNull] DaedalusParser.StatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.functionCall"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFunctionCall([NotNull] DaedalusParser.FunctionCallContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.functionCall"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFunctionCall([NotNull] DaedalusParser.FunctionCallContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.assignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAssignment([NotNull] DaedalusParser.AssignmentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.assignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAssignment([NotNull] DaedalusParser.AssignmentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.elseBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterElseBlock([NotNull] DaedalusParser.ElseBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.elseBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitElseBlock([NotNull] DaedalusParser.ElseBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.elseIfBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterElseIfBlock([NotNull] DaedalusParser.ElseIfBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.elseIfBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitElseIfBlock([NotNull] DaedalusParser.ElseIfBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.ifBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIfBlock([NotNull] DaedalusParser.IfBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.ifBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIfBlock([NotNull] DaedalusParser.IfBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.ifBlockStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIfBlockStatement([NotNull] DaedalusParser.IfBlockStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.ifBlockStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIfBlockStatement([NotNull] DaedalusParser.IfBlockStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.returnStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterReturnStatement([NotNull] DaedalusParser.ReturnStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.returnStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitReturnStatement([NotNull] DaedalusParser.ReturnStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.whileStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterWhileStatement([NotNull] DaedalusParser.WhileStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.whileStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitWhileStatement([NotNull] DaedalusParser.WhileStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.breakStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBreakStatement([NotNull] DaedalusParser.BreakStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.breakStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBreakStatement([NotNull] DaedalusParser.BreakStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.continueStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterContinueStatement([NotNull] DaedalusParser.ContinueStatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.continueStatement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitContinueStatement([NotNull] DaedalusParser.ContinueStatementContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>bitMoveExpression</c>
	/// labeled alternative in <see cref="DaedalusParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBitMoveExpression([NotNull] DaedalusParser.BitMoveExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>bitMoveExpression</c>
	/// labeled alternative in <see cref="DaedalusParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBitMoveExpression([NotNull] DaedalusParser.BitMoveExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>valueExpression</c>
	/// labeled alternative in <see cref="DaedalusParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterValueExpression([NotNull] DaedalusParser.ValueExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>valueExpression</c>
	/// labeled alternative in <see cref="DaedalusParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitValueExpression([NotNull] DaedalusParser.ValueExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>eqExpression</c>
	/// labeled alternative in <see cref="DaedalusParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterEqExpression([NotNull] DaedalusParser.EqExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>eqExpression</c>
	/// labeled alternative in <see cref="DaedalusParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitEqExpression([NotNull] DaedalusParser.EqExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>addExpression</c>
	/// labeled alternative in <see cref="DaedalusParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAddExpression([NotNull] DaedalusParser.AddExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>addExpression</c>
	/// labeled alternative in <see cref="DaedalusParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAddExpression([NotNull] DaedalusParser.AddExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>compExpression</c>
	/// labeled alternative in <see cref="DaedalusParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCompExpression([NotNull] DaedalusParser.CompExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>compExpression</c>
	/// labeled alternative in <see cref="DaedalusParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCompExpression([NotNull] DaedalusParser.CompExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>logOrExpression</c>
	/// labeled alternative in <see cref="DaedalusParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLogOrExpression([NotNull] DaedalusParser.LogOrExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>logOrExpression</c>
	/// labeled alternative in <see cref="DaedalusParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLogOrExpression([NotNull] DaedalusParser.LogOrExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>binAndExpression</c>
	/// labeled alternative in <see cref="DaedalusParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBinAndExpression([NotNull] DaedalusParser.BinAndExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>binAndExpression</c>
	/// labeled alternative in <see cref="DaedalusParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBinAndExpression([NotNull] DaedalusParser.BinAndExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>binOrExpression</c>
	/// labeled alternative in <see cref="DaedalusParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBinOrExpression([NotNull] DaedalusParser.BinOrExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>binOrExpression</c>
	/// labeled alternative in <see cref="DaedalusParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBinOrExpression([NotNull] DaedalusParser.BinOrExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>multExpression</c>
	/// labeled alternative in <see cref="DaedalusParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMultExpression([NotNull] DaedalusParser.MultExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>multExpression</c>
	/// labeled alternative in <see cref="DaedalusParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMultExpression([NotNull] DaedalusParser.MultExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>bracketExpression</c>
	/// labeled alternative in <see cref="DaedalusParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBracketExpression([NotNull] DaedalusParser.BracketExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>bracketExpression</c>
	/// labeled alternative in <see cref="DaedalusParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBracketExpression([NotNull] DaedalusParser.BracketExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>unaryExpression</c>
	/// labeled alternative in <see cref="DaedalusParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterUnaryExpression([NotNull] DaedalusParser.UnaryExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>unaryExpression</c>
	/// labeled alternative in <see cref="DaedalusParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitUnaryExpression([NotNull] DaedalusParser.UnaryExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>logAndExpression</c>
	/// labeled alternative in <see cref="DaedalusParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLogAndExpression([NotNull] DaedalusParser.LogAndExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>logAndExpression</c>
	/// labeled alternative in <see cref="DaedalusParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLogAndExpression([NotNull] DaedalusParser.LogAndExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.arrayIndex"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterArrayIndex([NotNull] DaedalusParser.ArrayIndexContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.arrayIndex"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitArrayIndex([NotNull] DaedalusParser.ArrayIndexContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.arraySize"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterArraySize([NotNull] DaedalusParser.ArraySizeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.arraySize"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitArraySize([NotNull] DaedalusParser.ArraySizeContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>integerLiteralValue</c>
	/// labeled alternative in <see cref="DaedalusParser.value"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIntegerLiteralValue([NotNull] DaedalusParser.IntegerLiteralValueContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>integerLiteralValue</c>
	/// labeled alternative in <see cref="DaedalusParser.value"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIntegerLiteralValue([NotNull] DaedalusParser.IntegerLiteralValueContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>floatLiteralValue</c>
	/// labeled alternative in <see cref="DaedalusParser.value"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFloatLiteralValue([NotNull] DaedalusParser.FloatLiteralValueContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>floatLiteralValue</c>
	/// labeled alternative in <see cref="DaedalusParser.value"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFloatLiteralValue([NotNull] DaedalusParser.FloatLiteralValueContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>stringLiteralValue</c>
	/// labeled alternative in <see cref="DaedalusParser.value"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStringLiteralValue([NotNull] DaedalusParser.StringLiteralValueContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>stringLiteralValue</c>
	/// labeled alternative in <see cref="DaedalusParser.value"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStringLiteralValue([NotNull] DaedalusParser.StringLiteralValueContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>nullLiteralValue</c>
	/// labeled alternative in <see cref="DaedalusParser.value"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNullLiteralValue([NotNull] DaedalusParser.NullLiteralValueContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>nullLiteralValue</c>
	/// labeled alternative in <see cref="DaedalusParser.value"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNullLiteralValue([NotNull] DaedalusParser.NullLiteralValueContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>noFuncLiteralValue</c>
	/// labeled alternative in <see cref="DaedalusParser.value"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNoFuncLiteralValue([NotNull] DaedalusParser.NoFuncLiteralValueContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>noFuncLiteralValue</c>
	/// labeled alternative in <see cref="DaedalusParser.value"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNoFuncLiteralValue([NotNull] DaedalusParser.NoFuncLiteralValueContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>functionCallValue</c>
	/// labeled alternative in <see cref="DaedalusParser.value"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFunctionCallValue([NotNull] DaedalusParser.FunctionCallValueContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>functionCallValue</c>
	/// labeled alternative in <see cref="DaedalusParser.value"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFunctionCallValue([NotNull] DaedalusParser.FunctionCallValueContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>referenceValue</c>
	/// labeled alternative in <see cref="DaedalusParser.value"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterReferenceValue([NotNull] DaedalusParser.ReferenceValueContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>referenceValue</c>
	/// labeled alternative in <see cref="DaedalusParser.value"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitReferenceValue([NotNull] DaedalusParser.ReferenceValueContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.referenceAtom"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterReferenceAtom([NotNull] DaedalusParser.ReferenceAtomContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.referenceAtom"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitReferenceAtom([NotNull] DaedalusParser.ReferenceAtomContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.reference"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterReference([NotNull] DaedalusParser.ReferenceContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.reference"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitReference([NotNull] DaedalusParser.ReferenceContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.dataType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDataType([NotNull] DaedalusParser.DataTypeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.dataType"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDataType([NotNull] DaedalusParser.DataTypeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.nameNode"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNameNode([NotNull] DaedalusParser.NameNodeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.nameNode"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNameNode([NotNull] DaedalusParser.NameNodeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.parentReference"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParentReference([NotNull] DaedalusParser.ParentReferenceContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.parentReference"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParentReference([NotNull] DaedalusParser.ParentReferenceContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.assignmentOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAssignmentOperator([NotNull] DaedalusParser.AssignmentOperatorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.assignmentOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAssignmentOperator([NotNull] DaedalusParser.AssignmentOperatorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.addOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAddOperator([NotNull] DaedalusParser.AddOperatorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.addOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAddOperator([NotNull] DaedalusParser.AddOperatorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.bitMoveOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBitMoveOperator([NotNull] DaedalusParser.BitMoveOperatorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.bitMoveOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBitMoveOperator([NotNull] DaedalusParser.BitMoveOperatorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.compOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCompOperator([NotNull] DaedalusParser.CompOperatorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.compOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCompOperator([NotNull] DaedalusParser.CompOperatorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.eqOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterEqOperator([NotNull] DaedalusParser.EqOperatorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.eqOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitEqOperator([NotNull] DaedalusParser.EqOperatorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.unaryOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterUnaryOperator([NotNull] DaedalusParser.UnaryOperatorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.unaryOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitUnaryOperator([NotNull] DaedalusParser.UnaryOperatorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.multOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMultOperator([NotNull] DaedalusParser.MultOperatorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.multOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMultOperator([NotNull] DaedalusParser.MultOperatorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.binAndOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBinAndOperator([NotNull] DaedalusParser.BinAndOperatorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.binAndOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBinAndOperator([NotNull] DaedalusParser.BinAndOperatorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.binOrOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBinOrOperator([NotNull] DaedalusParser.BinOrOperatorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.binOrOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBinOrOperator([NotNull] DaedalusParser.BinOrOperatorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.logAndOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLogAndOperator([NotNull] DaedalusParser.LogAndOperatorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.logAndOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLogAndOperator([NotNull] DaedalusParser.LogAndOperatorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.logOrOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLogOrOperator([NotNull] DaedalusParser.LogOrOperatorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.logOrOperator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLogOrOperator([NotNull] DaedalusParser.LogOrOperatorContext context);
}
