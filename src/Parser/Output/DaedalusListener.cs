//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.7
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from C:/Users/grzeg/Source/Repos/DaedalusCompiler/src/Parser\Daedalus.g4 by ANTLR 4.7

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
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.7")]
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
	/// Enter a parse tree produced by <see cref="DaedalusParser.funcCall"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFuncCall([NotNull] DaedalusParser.FuncCallContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.funcCall"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFuncCall([NotNull] DaedalusParser.FuncCallContext context);
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
	/// Enter a parse tree produced by <see cref="DaedalusParser.constAssignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterConstAssignment([NotNull] DaedalusParser.ConstAssignmentContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.constAssignment"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitConstAssignment([NotNull] DaedalusParser.ConstAssignmentContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.arrayLiteral"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterArrayLiteral([NotNull] DaedalusParser.ArrayLiteralContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.arrayLiteral"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitArrayLiteral([NotNull] DaedalusParser.ArrayLiteralContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression([NotNull] DaedalusParser.ExpressionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression([NotNull] DaedalusParser.ExpressionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.simpleValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSimpleValue([NotNull] DaedalusParser.SimpleValueContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.simpleValue"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSimpleValue([NotNull] DaedalusParser.SimpleValueContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.value"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterValue([NotNull] DaedalusParser.ValueContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.value"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitValue([NotNull] DaedalusParser.ValueContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.complexReference"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterComplexReference([NotNull] DaedalusParser.ComplexReferenceContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.complexReference"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitComplexReference([NotNull] DaedalusParser.ComplexReferenceContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.complexReferenceNode"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterComplexReferenceNode([NotNull] DaedalusParser.ComplexReferenceNodeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.complexReferenceNode"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitComplexReferenceNode([NotNull] DaedalusParser.ComplexReferenceNodeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="DaedalusParser.typeReference"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTypeReference([NotNull] DaedalusParser.TypeReferenceContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.typeReference"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTypeReference([NotNull] DaedalusParser.TypeReferenceContext context);
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
	/// Enter a parse tree produced by <see cref="DaedalusParser.referenceNode"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterReferenceNode([NotNull] DaedalusParser.ReferenceNodeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="DaedalusParser.referenceNode"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitReferenceNode([NotNull] DaedalusParser.ReferenceNodeContext context);
}
