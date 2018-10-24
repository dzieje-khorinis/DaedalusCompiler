// Generated from /Users/artur/dev/dzieje/DaedalusCompiler/src/Parser/Daedalus.g4 by ANTLR 4.7
import org.antlr.v4.runtime.tree.ParseTreeVisitor;

/**
 * This interface defines a complete generic visitor for a parse tree produced
 * by {@link DaedalusParser}.
 *
 * @param <T> The return type of the visit operation. Use {@link Void} for
 * operations with no return type.
 */
public interface DaedalusVisitor<T> extends ParseTreeVisitor<T> {
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#daedalusFile}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitDaedalusFile(DaedalusParser.DaedalusFileContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#functionDef}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitFunctionDef(DaedalusParser.FunctionDefContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#constDef}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitConstDef(DaedalusParser.ConstDefContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#classDef}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitClassDef(DaedalusParser.ClassDefContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#prototypeDef}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitPrototypeDef(DaedalusParser.PrototypeDefContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#instanceDef}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitInstanceDef(DaedalusParser.InstanceDefContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#instanceDecl}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitInstanceDecl(DaedalusParser.InstanceDeclContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#varDecl}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitVarDecl(DaedalusParser.VarDeclContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#constArrayDef}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitConstArrayDef(DaedalusParser.ConstArrayDefContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#constArrayAssignment}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitConstArrayAssignment(DaedalusParser.ConstArrayAssignmentContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#constValueDef}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitConstValueDef(DaedalusParser.ConstValueDefContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#constValueAssignment}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitConstValueAssignment(DaedalusParser.ConstValueAssignmentContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#varArrayDecl}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitVarArrayDecl(DaedalusParser.VarArrayDeclContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#varValueDecl}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitVarValueDecl(DaedalusParser.VarValueDeclContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#parameterList}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitParameterList(DaedalusParser.ParameterListContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#parameterDecl}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitParameterDecl(DaedalusParser.ParameterDeclContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#statementBlock}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitStatementBlock(DaedalusParser.StatementBlockContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#statement}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitStatement(DaedalusParser.StatementContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#funcCall}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitFuncCall(DaedalusParser.FuncCallContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#assignment}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitAssignment(DaedalusParser.AssignmentContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#ifCondition}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitIfCondition(DaedalusParser.IfConditionContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#elseBlock}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitElseBlock(DaedalusParser.ElseBlockContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#elseIfBlock}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitElseIfBlock(DaedalusParser.ElseIfBlockContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#ifBlock}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitIfBlock(DaedalusParser.IfBlockContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#ifBlockStatement}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitIfBlockStatement(DaedalusParser.IfBlockStatementContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#returnStatement}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitReturnStatement(DaedalusParser.ReturnStatementContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#funcArgExpression}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitFuncArgExpression(DaedalusParser.FuncArgExpressionContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#expressionBlock}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitExpressionBlock(DaedalusParser.ExpressionBlockContext ctx);
	/**
	 * Visit a parse tree produced by the {@code bitMoveExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitBitMoveExpression(DaedalusParser.BitMoveExpressionContext ctx);
	/**
	 * Visit a parse tree produced by the {@code oneArgExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitOneArgExpression(DaedalusParser.OneArgExpressionContext ctx);
	/**
	 * Visit a parse tree produced by the {@code eqExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitEqExpression(DaedalusParser.EqExpressionContext ctx);
	/**
	 * Visit a parse tree produced by the {@code valExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitValExpression(DaedalusParser.ValExpressionContext ctx);
	/**
	 * Visit a parse tree produced by the {@code addExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitAddExpression(DaedalusParser.AddExpressionContext ctx);
	/**
	 * Visit a parse tree produced by the {@code compExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitCompExpression(DaedalusParser.CompExpressionContext ctx);
	/**
	 * Visit a parse tree produced by the {@code logOrExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitLogOrExpression(DaedalusParser.LogOrExpressionContext ctx);
	/**
	 * Visit a parse tree produced by the {@code binAndExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitBinAndExpression(DaedalusParser.BinAndExpressionContext ctx);
	/**
	 * Visit a parse tree produced by the {@code binOrExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitBinOrExpression(DaedalusParser.BinOrExpressionContext ctx);
	/**
	 * Visit a parse tree produced by the {@code multExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitMultExpression(DaedalusParser.MultExpressionContext ctx);
	/**
	 * Visit a parse tree produced by the {@code bracketExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitBracketExpression(DaedalusParser.BracketExpressionContext ctx);
	/**
	 * Visit a parse tree produced by the {@code logAndExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitLogAndExpression(DaedalusParser.LogAndExpressionContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#arrayIndex}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitArrayIndex(DaedalusParser.ArrayIndexContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#arraySize}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitArraySize(DaedalusParser.ArraySizeContext ctx);
	/**
	 * Visit a parse tree produced by the {@code integerLiteralValue}
	 * labeled alternative in {@link DaedalusParser#value}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitIntegerLiteralValue(DaedalusParser.IntegerLiteralValueContext ctx);
	/**
	 * Visit a parse tree produced by the {@code floatLiteralValue}
	 * labeled alternative in {@link DaedalusParser#value}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitFloatLiteralValue(DaedalusParser.FloatLiteralValueContext ctx);
	/**
	 * Visit a parse tree produced by the {@code stringLiteralValue}
	 * labeled alternative in {@link DaedalusParser#value}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitStringLiteralValue(DaedalusParser.StringLiteralValueContext ctx);
	/**
	 * Visit a parse tree produced by the {@code nullLiteralValue}
	 * labeled alternative in {@link DaedalusParser#value}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitNullLiteralValue(DaedalusParser.NullLiteralValueContext ctx);
	/**
	 * Visit a parse tree produced by the {@code funcCallValue}
	 * labeled alternative in {@link DaedalusParser#value}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitFuncCallValue(DaedalusParser.FuncCallValueContext ctx);
	/**
	 * Visit a parse tree produced by the {@code referenceValue}
	 * labeled alternative in {@link DaedalusParser#value}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitReferenceValue(DaedalusParser.ReferenceValueContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#referenceAtom}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitReferenceAtom(DaedalusParser.ReferenceAtomContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#reference}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitReference(DaedalusParser.ReferenceContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#referenceLeftSide}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitReferenceLeftSide(DaedalusParser.ReferenceLeftSideContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#typeReference}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitTypeReference(DaedalusParser.TypeReferenceContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#nameNode}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitNameNode(DaedalusParser.NameNodeContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#parentReference}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitParentReference(DaedalusParser.ParentReferenceContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#assignmentOperator}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitAssignmentOperator(DaedalusParser.AssignmentOperatorContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#addOperator}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitAddOperator(DaedalusParser.AddOperatorContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#bitMoveOperator}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitBitMoveOperator(DaedalusParser.BitMoveOperatorContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#compOperator}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitCompOperator(DaedalusParser.CompOperatorContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#eqOperator}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitEqOperator(DaedalusParser.EqOperatorContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#oneArgOperator}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitOneArgOperator(DaedalusParser.OneArgOperatorContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#multOperator}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitMultOperator(DaedalusParser.MultOperatorContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#binAndOperator}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitBinAndOperator(DaedalusParser.BinAndOperatorContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#binOrOperator}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitBinOrOperator(DaedalusParser.BinOrOperatorContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#logAndOperator}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitLogAndOperator(DaedalusParser.LogAndOperatorContext ctx);
	/**
	 * Visit a parse tree produced by {@link DaedalusParser#logOrOperator}.
	 * @param ctx the parse tree
	 * @return the visitor result
	 */
	T visitLogOrOperator(DaedalusParser.LogOrOperatorContext ctx);
}