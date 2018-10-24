// Generated from /Users/artur/dev/dzieje/DaedalusCompiler/src/Parser/Daedalus.g4 by ANTLR 4.7
import org.antlr.v4.runtime.tree.ParseTreeListener;

/**
 * This interface defines a complete listener for a parse tree produced by
 * {@link DaedalusParser}.
 */
public interface DaedalusListener extends ParseTreeListener {
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#daedalusFile}.
	 * @param ctx the parse tree
	 */
	void enterDaedalusFile(DaedalusParser.DaedalusFileContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#daedalusFile}.
	 * @param ctx the parse tree
	 */
	void exitDaedalusFile(DaedalusParser.DaedalusFileContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#functionDef}.
	 * @param ctx the parse tree
	 */
	void enterFunctionDef(DaedalusParser.FunctionDefContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#functionDef}.
	 * @param ctx the parse tree
	 */
	void exitFunctionDef(DaedalusParser.FunctionDefContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#constDef}.
	 * @param ctx the parse tree
	 */
	void enterConstDef(DaedalusParser.ConstDefContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#constDef}.
	 * @param ctx the parse tree
	 */
	void exitConstDef(DaedalusParser.ConstDefContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#classDef}.
	 * @param ctx the parse tree
	 */
	void enterClassDef(DaedalusParser.ClassDefContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#classDef}.
	 * @param ctx the parse tree
	 */
	void exitClassDef(DaedalusParser.ClassDefContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#prototypeDef}.
	 * @param ctx the parse tree
	 */
	void enterPrototypeDef(DaedalusParser.PrototypeDefContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#prototypeDef}.
	 * @param ctx the parse tree
	 */
	void exitPrototypeDef(DaedalusParser.PrototypeDefContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#instanceDef}.
	 * @param ctx the parse tree
	 */
	void enterInstanceDef(DaedalusParser.InstanceDefContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#instanceDef}.
	 * @param ctx the parse tree
	 */
	void exitInstanceDef(DaedalusParser.InstanceDefContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#instanceDecl}.
	 * @param ctx the parse tree
	 */
	void enterInstanceDecl(DaedalusParser.InstanceDeclContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#instanceDecl}.
	 * @param ctx the parse tree
	 */
	void exitInstanceDecl(DaedalusParser.InstanceDeclContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#varDecl}.
	 * @param ctx the parse tree
	 */
	void enterVarDecl(DaedalusParser.VarDeclContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#varDecl}.
	 * @param ctx the parse tree
	 */
	void exitVarDecl(DaedalusParser.VarDeclContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#constArrayDef}.
	 * @param ctx the parse tree
	 */
	void enterConstArrayDef(DaedalusParser.ConstArrayDefContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#constArrayDef}.
	 * @param ctx the parse tree
	 */
	void exitConstArrayDef(DaedalusParser.ConstArrayDefContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#constArrayAssignment}.
	 * @param ctx the parse tree
	 */
	void enterConstArrayAssignment(DaedalusParser.ConstArrayAssignmentContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#constArrayAssignment}.
	 * @param ctx the parse tree
	 */
	void exitConstArrayAssignment(DaedalusParser.ConstArrayAssignmentContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#constValueDef}.
	 * @param ctx the parse tree
	 */
	void enterConstValueDef(DaedalusParser.ConstValueDefContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#constValueDef}.
	 * @param ctx the parse tree
	 */
	void exitConstValueDef(DaedalusParser.ConstValueDefContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#constValueAssignment}.
	 * @param ctx the parse tree
	 */
	void enterConstValueAssignment(DaedalusParser.ConstValueAssignmentContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#constValueAssignment}.
	 * @param ctx the parse tree
	 */
	void exitConstValueAssignment(DaedalusParser.ConstValueAssignmentContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#varArrayDecl}.
	 * @param ctx the parse tree
	 */
	void enterVarArrayDecl(DaedalusParser.VarArrayDeclContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#varArrayDecl}.
	 * @param ctx the parse tree
	 */
	void exitVarArrayDecl(DaedalusParser.VarArrayDeclContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#varValueDecl}.
	 * @param ctx the parse tree
	 */
	void enterVarValueDecl(DaedalusParser.VarValueDeclContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#varValueDecl}.
	 * @param ctx the parse tree
	 */
	void exitVarValueDecl(DaedalusParser.VarValueDeclContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#parameterList}.
	 * @param ctx the parse tree
	 */
	void enterParameterList(DaedalusParser.ParameterListContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#parameterList}.
	 * @param ctx the parse tree
	 */
	void exitParameterList(DaedalusParser.ParameterListContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#parameterDecl}.
	 * @param ctx the parse tree
	 */
	void enterParameterDecl(DaedalusParser.ParameterDeclContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#parameterDecl}.
	 * @param ctx the parse tree
	 */
	void exitParameterDecl(DaedalusParser.ParameterDeclContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#statementBlock}.
	 * @param ctx the parse tree
	 */
	void enterStatementBlock(DaedalusParser.StatementBlockContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#statementBlock}.
	 * @param ctx the parse tree
	 */
	void exitStatementBlock(DaedalusParser.StatementBlockContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#statement}.
	 * @param ctx the parse tree
	 */
	void enterStatement(DaedalusParser.StatementContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#statement}.
	 * @param ctx the parse tree
	 */
	void exitStatement(DaedalusParser.StatementContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#funcCall}.
	 * @param ctx the parse tree
	 */
	void enterFuncCall(DaedalusParser.FuncCallContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#funcCall}.
	 * @param ctx the parse tree
	 */
	void exitFuncCall(DaedalusParser.FuncCallContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#assignment}.
	 * @param ctx the parse tree
	 */
	void enterAssignment(DaedalusParser.AssignmentContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#assignment}.
	 * @param ctx the parse tree
	 */
	void exitAssignment(DaedalusParser.AssignmentContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#ifCondition}.
	 * @param ctx the parse tree
	 */
	void enterIfCondition(DaedalusParser.IfConditionContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#ifCondition}.
	 * @param ctx the parse tree
	 */
	void exitIfCondition(DaedalusParser.IfConditionContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#elseBlock}.
	 * @param ctx the parse tree
	 */
	void enterElseBlock(DaedalusParser.ElseBlockContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#elseBlock}.
	 * @param ctx the parse tree
	 */
	void exitElseBlock(DaedalusParser.ElseBlockContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#elseIfBlock}.
	 * @param ctx the parse tree
	 */
	void enterElseIfBlock(DaedalusParser.ElseIfBlockContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#elseIfBlock}.
	 * @param ctx the parse tree
	 */
	void exitElseIfBlock(DaedalusParser.ElseIfBlockContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#ifBlock}.
	 * @param ctx the parse tree
	 */
	void enterIfBlock(DaedalusParser.IfBlockContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#ifBlock}.
	 * @param ctx the parse tree
	 */
	void exitIfBlock(DaedalusParser.IfBlockContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#ifBlockStatement}.
	 * @param ctx the parse tree
	 */
	void enterIfBlockStatement(DaedalusParser.IfBlockStatementContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#ifBlockStatement}.
	 * @param ctx the parse tree
	 */
	void exitIfBlockStatement(DaedalusParser.IfBlockStatementContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#returnStatement}.
	 * @param ctx the parse tree
	 */
	void enterReturnStatement(DaedalusParser.ReturnStatementContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#returnStatement}.
	 * @param ctx the parse tree
	 */
	void exitReturnStatement(DaedalusParser.ReturnStatementContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#funcArgExpression}.
	 * @param ctx the parse tree
	 */
	void enterFuncArgExpression(DaedalusParser.FuncArgExpressionContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#funcArgExpression}.
	 * @param ctx the parse tree
	 */
	void exitFuncArgExpression(DaedalusParser.FuncArgExpressionContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#expressionBlock}.
	 * @param ctx the parse tree
	 */
	void enterExpressionBlock(DaedalusParser.ExpressionBlockContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#expressionBlock}.
	 * @param ctx the parse tree
	 */
	void exitExpressionBlock(DaedalusParser.ExpressionBlockContext ctx);
	/**
	 * Enter a parse tree produced by the {@code bitMoveExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 */
	void enterBitMoveExpression(DaedalusParser.BitMoveExpressionContext ctx);
	/**
	 * Exit a parse tree produced by the {@code bitMoveExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 */
	void exitBitMoveExpression(DaedalusParser.BitMoveExpressionContext ctx);
	/**
	 * Enter a parse tree produced by the {@code oneArgExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 */
	void enterOneArgExpression(DaedalusParser.OneArgExpressionContext ctx);
	/**
	 * Exit a parse tree produced by the {@code oneArgExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 */
	void exitOneArgExpression(DaedalusParser.OneArgExpressionContext ctx);
	/**
	 * Enter a parse tree produced by the {@code eqExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 */
	void enterEqExpression(DaedalusParser.EqExpressionContext ctx);
	/**
	 * Exit a parse tree produced by the {@code eqExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 */
	void exitEqExpression(DaedalusParser.EqExpressionContext ctx);
	/**
	 * Enter a parse tree produced by the {@code valExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 */
	void enterValExpression(DaedalusParser.ValExpressionContext ctx);
	/**
	 * Exit a parse tree produced by the {@code valExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 */
	void exitValExpression(DaedalusParser.ValExpressionContext ctx);
	/**
	 * Enter a parse tree produced by the {@code addExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 */
	void enterAddExpression(DaedalusParser.AddExpressionContext ctx);
	/**
	 * Exit a parse tree produced by the {@code addExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 */
	void exitAddExpression(DaedalusParser.AddExpressionContext ctx);
	/**
	 * Enter a parse tree produced by the {@code compExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 */
	void enterCompExpression(DaedalusParser.CompExpressionContext ctx);
	/**
	 * Exit a parse tree produced by the {@code compExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 */
	void exitCompExpression(DaedalusParser.CompExpressionContext ctx);
	/**
	 * Enter a parse tree produced by the {@code logOrExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 */
	void enterLogOrExpression(DaedalusParser.LogOrExpressionContext ctx);
	/**
	 * Exit a parse tree produced by the {@code logOrExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 */
	void exitLogOrExpression(DaedalusParser.LogOrExpressionContext ctx);
	/**
	 * Enter a parse tree produced by the {@code binAndExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 */
	void enterBinAndExpression(DaedalusParser.BinAndExpressionContext ctx);
	/**
	 * Exit a parse tree produced by the {@code binAndExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 */
	void exitBinAndExpression(DaedalusParser.BinAndExpressionContext ctx);
	/**
	 * Enter a parse tree produced by the {@code binOrExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 */
	void enterBinOrExpression(DaedalusParser.BinOrExpressionContext ctx);
	/**
	 * Exit a parse tree produced by the {@code binOrExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 */
	void exitBinOrExpression(DaedalusParser.BinOrExpressionContext ctx);
	/**
	 * Enter a parse tree produced by the {@code multExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 */
	void enterMultExpression(DaedalusParser.MultExpressionContext ctx);
	/**
	 * Exit a parse tree produced by the {@code multExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 */
	void exitMultExpression(DaedalusParser.MultExpressionContext ctx);
	/**
	 * Enter a parse tree produced by the {@code bracketExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 */
	void enterBracketExpression(DaedalusParser.BracketExpressionContext ctx);
	/**
	 * Exit a parse tree produced by the {@code bracketExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 */
	void exitBracketExpression(DaedalusParser.BracketExpressionContext ctx);
	/**
	 * Enter a parse tree produced by the {@code logAndExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 */
	void enterLogAndExpression(DaedalusParser.LogAndExpressionContext ctx);
	/**
	 * Exit a parse tree produced by the {@code logAndExpression}
	 * labeled alternative in {@link DaedalusParser#expression}.
	 * @param ctx the parse tree
	 */
	void exitLogAndExpression(DaedalusParser.LogAndExpressionContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#arrayIndex}.
	 * @param ctx the parse tree
	 */
	void enterArrayIndex(DaedalusParser.ArrayIndexContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#arrayIndex}.
	 * @param ctx the parse tree
	 */
	void exitArrayIndex(DaedalusParser.ArrayIndexContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#arraySize}.
	 * @param ctx the parse tree
	 */
	void enterArraySize(DaedalusParser.ArraySizeContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#arraySize}.
	 * @param ctx the parse tree
	 */
	void exitArraySize(DaedalusParser.ArraySizeContext ctx);
	/**
	 * Enter a parse tree produced by the {@code integerLiteralValue}
	 * labeled alternative in {@link DaedalusParser#value}.
	 * @param ctx the parse tree
	 */
	void enterIntegerLiteralValue(DaedalusParser.IntegerLiteralValueContext ctx);
	/**
	 * Exit a parse tree produced by the {@code integerLiteralValue}
	 * labeled alternative in {@link DaedalusParser#value}.
	 * @param ctx the parse tree
	 */
	void exitIntegerLiteralValue(DaedalusParser.IntegerLiteralValueContext ctx);
	/**
	 * Enter a parse tree produced by the {@code floatLiteralValue}
	 * labeled alternative in {@link DaedalusParser#value}.
	 * @param ctx the parse tree
	 */
	void enterFloatLiteralValue(DaedalusParser.FloatLiteralValueContext ctx);
	/**
	 * Exit a parse tree produced by the {@code floatLiteralValue}
	 * labeled alternative in {@link DaedalusParser#value}.
	 * @param ctx the parse tree
	 */
	void exitFloatLiteralValue(DaedalusParser.FloatLiteralValueContext ctx);
	/**
	 * Enter a parse tree produced by the {@code stringLiteralValue}
	 * labeled alternative in {@link DaedalusParser#value}.
	 * @param ctx the parse tree
	 */
	void enterStringLiteralValue(DaedalusParser.StringLiteralValueContext ctx);
	/**
	 * Exit a parse tree produced by the {@code stringLiteralValue}
	 * labeled alternative in {@link DaedalusParser#value}.
	 * @param ctx the parse tree
	 */
	void exitStringLiteralValue(DaedalusParser.StringLiteralValueContext ctx);
	/**
	 * Enter a parse tree produced by the {@code nullLiteralValue}
	 * labeled alternative in {@link DaedalusParser#value}.
	 * @param ctx the parse tree
	 */
	void enterNullLiteralValue(DaedalusParser.NullLiteralValueContext ctx);
	/**
	 * Exit a parse tree produced by the {@code nullLiteralValue}
	 * labeled alternative in {@link DaedalusParser#value}.
	 * @param ctx the parse tree
	 */
	void exitNullLiteralValue(DaedalusParser.NullLiteralValueContext ctx);
	/**
	 * Enter a parse tree produced by the {@code funcCallValue}
	 * labeled alternative in {@link DaedalusParser#value}.
	 * @param ctx the parse tree
	 */
	void enterFuncCallValue(DaedalusParser.FuncCallValueContext ctx);
	/**
	 * Exit a parse tree produced by the {@code funcCallValue}
	 * labeled alternative in {@link DaedalusParser#value}.
	 * @param ctx the parse tree
	 */
	void exitFuncCallValue(DaedalusParser.FuncCallValueContext ctx);
	/**
	 * Enter a parse tree produced by the {@code referenceValue}
	 * labeled alternative in {@link DaedalusParser#value}.
	 * @param ctx the parse tree
	 */
	void enterReferenceValue(DaedalusParser.ReferenceValueContext ctx);
	/**
	 * Exit a parse tree produced by the {@code referenceValue}
	 * labeled alternative in {@link DaedalusParser#value}.
	 * @param ctx the parse tree
	 */
	void exitReferenceValue(DaedalusParser.ReferenceValueContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#referenceAtom}.
	 * @param ctx the parse tree
	 */
	void enterReferenceAtom(DaedalusParser.ReferenceAtomContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#referenceAtom}.
	 * @param ctx the parse tree
	 */
	void exitReferenceAtom(DaedalusParser.ReferenceAtomContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#reference}.
	 * @param ctx the parse tree
	 */
	void enterReference(DaedalusParser.ReferenceContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#reference}.
	 * @param ctx the parse tree
	 */
	void exitReference(DaedalusParser.ReferenceContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#referenceLeftSide}.
	 * @param ctx the parse tree
	 */
	void enterReferenceLeftSide(DaedalusParser.ReferenceLeftSideContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#referenceLeftSide}.
	 * @param ctx the parse tree
	 */
	void exitReferenceLeftSide(DaedalusParser.ReferenceLeftSideContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#typeReference}.
	 * @param ctx the parse tree
	 */
	void enterTypeReference(DaedalusParser.TypeReferenceContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#typeReference}.
	 * @param ctx the parse tree
	 */
	void exitTypeReference(DaedalusParser.TypeReferenceContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#nameNode}.
	 * @param ctx the parse tree
	 */
	void enterNameNode(DaedalusParser.NameNodeContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#nameNode}.
	 * @param ctx the parse tree
	 */
	void exitNameNode(DaedalusParser.NameNodeContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#parentReference}.
	 * @param ctx the parse tree
	 */
	void enterParentReference(DaedalusParser.ParentReferenceContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#parentReference}.
	 * @param ctx the parse tree
	 */
	void exitParentReference(DaedalusParser.ParentReferenceContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#assignmentOperator}.
	 * @param ctx the parse tree
	 */
	void enterAssignmentOperator(DaedalusParser.AssignmentOperatorContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#assignmentOperator}.
	 * @param ctx the parse tree
	 */
	void exitAssignmentOperator(DaedalusParser.AssignmentOperatorContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#addOperator}.
	 * @param ctx the parse tree
	 */
	void enterAddOperator(DaedalusParser.AddOperatorContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#addOperator}.
	 * @param ctx the parse tree
	 */
	void exitAddOperator(DaedalusParser.AddOperatorContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#bitMoveOperator}.
	 * @param ctx the parse tree
	 */
	void enterBitMoveOperator(DaedalusParser.BitMoveOperatorContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#bitMoveOperator}.
	 * @param ctx the parse tree
	 */
	void exitBitMoveOperator(DaedalusParser.BitMoveOperatorContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#compOperator}.
	 * @param ctx the parse tree
	 */
	void enterCompOperator(DaedalusParser.CompOperatorContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#compOperator}.
	 * @param ctx the parse tree
	 */
	void exitCompOperator(DaedalusParser.CompOperatorContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#eqOperator}.
	 * @param ctx the parse tree
	 */
	void enterEqOperator(DaedalusParser.EqOperatorContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#eqOperator}.
	 * @param ctx the parse tree
	 */
	void exitEqOperator(DaedalusParser.EqOperatorContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#oneArgOperator}.
	 * @param ctx the parse tree
	 */
	void enterOneArgOperator(DaedalusParser.OneArgOperatorContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#oneArgOperator}.
	 * @param ctx the parse tree
	 */
	void exitOneArgOperator(DaedalusParser.OneArgOperatorContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#multOperator}.
	 * @param ctx the parse tree
	 */
	void enterMultOperator(DaedalusParser.MultOperatorContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#multOperator}.
	 * @param ctx the parse tree
	 */
	void exitMultOperator(DaedalusParser.MultOperatorContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#binAndOperator}.
	 * @param ctx the parse tree
	 */
	void enterBinAndOperator(DaedalusParser.BinAndOperatorContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#binAndOperator}.
	 * @param ctx the parse tree
	 */
	void exitBinAndOperator(DaedalusParser.BinAndOperatorContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#binOrOperator}.
	 * @param ctx the parse tree
	 */
	void enterBinOrOperator(DaedalusParser.BinOrOperatorContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#binOrOperator}.
	 * @param ctx the parse tree
	 */
	void exitBinOrOperator(DaedalusParser.BinOrOperatorContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#logAndOperator}.
	 * @param ctx the parse tree
	 */
	void enterLogAndOperator(DaedalusParser.LogAndOperatorContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#logAndOperator}.
	 * @param ctx the parse tree
	 */
	void exitLogAndOperator(DaedalusParser.LogAndOperatorContext ctx);
	/**
	 * Enter a parse tree produced by {@link DaedalusParser#logOrOperator}.
	 * @param ctx the parse tree
	 */
	void enterLogOrOperator(DaedalusParser.LogOrOperatorContext ctx);
	/**
	 * Exit a parse tree produced by {@link DaedalusParser#logOrOperator}.
	 * @param ctx the parse tree
	 */
	void exitLogOrOperator(DaedalusParser.LogOrOperatorContext ctx);
}