// Generated from /Users/artur/dev/dzieje/DaedalusCompiler/src/Parser/Daedalus.g4 by ANTLR 4.7
import org.antlr.v4.runtime.atn.*;
import org.antlr.v4.runtime.dfa.DFA;
import org.antlr.v4.runtime.*;
import org.antlr.v4.runtime.misc.*;
import org.antlr.v4.runtime.tree.*;
import java.util.List;
import java.util.Iterator;
import java.util.ArrayList;

@SuppressWarnings({"all", "warnings", "unchecked", "unused", "cast"})
public class DaedalusParser extends Parser {
	static { RuntimeMetaData.checkVersion("4.7", RuntimeMetaData.VERSION); }

	protected static final DFA[] _decisionToDFA;
	protected static final PredictionContextCache _sharedContextCache =
		new PredictionContextCache();
	public static final int
		T__0=1, T__1=2, T__2=3, T__3=4, T__4=5, T__5=6, T__6=7, T__7=8, T__8=9, 
		T__9=10, T__10=11, T__11=12, T__12=13, T__13=14, T__14=15, T__15=16, T__16=17, 
		T__17=18, T__18=19, T__19=20, T__20=21, T__21=22, T__22=23, T__23=24, 
		T__24=25, T__25=26, T__26=27, T__27=28, T__28=29, T__29=30, T__30=31, 
		T__31=32, T__32=33, Const=34, Var=35, If=36, Int=37, Else=38, Func=39, 
		String=40, Class=41, Void=42, Return=43, Float=44, Prototype=45, Instance=46, 
		Null=47, Identifier=48, IntegerLiteral=49, FloatLiteral=50, StringLiteral=51, 
		Whitespace=52, Newline=53, BlockComment=54, LineComment=55;
	public static final int
		RULE_daedalusFile = 0, RULE_functionDef = 1, RULE_constDef = 2, RULE_classDef = 3, 
		RULE_prototypeDef = 4, RULE_instanceDef = 5, RULE_instanceDecl = 6, RULE_varDecl = 7, 
		RULE_constArrayDef = 8, RULE_constArrayAssignment = 9, RULE_constValueDef = 10, 
		RULE_constValueAssignment = 11, RULE_varArrayDecl = 12, RULE_varValueDecl = 13, 
		RULE_parameterList = 14, RULE_parameterDecl = 15, RULE_statementBlock = 16, 
		RULE_statement = 17, RULE_funcCall = 18, RULE_assignment = 19, RULE_ifCondition = 20, 
		RULE_elseBlock = 21, RULE_elseIfBlock = 22, RULE_ifBlock = 23, RULE_ifBlockStatement = 24, 
		RULE_returnStatement = 25, RULE_funcArgExpression = 26, RULE_expressionBlock = 27, 
		RULE_expression = 28, RULE_arrayIndex = 29, RULE_arraySize = 30, RULE_value = 31, 
		RULE_referenceAtom = 32, RULE_reference = 33, RULE_referenceLeftSide = 34, 
		RULE_typeReference = 35, RULE_nameNode = 36, RULE_parentReference = 37, 
		RULE_assignmentOperator = 38, RULE_addOperator = 39, RULE_bitMoveOperator = 40, 
		RULE_compOperator = 41, RULE_eqOperator = 42, RULE_oneArgOperator = 43, 
		RULE_multOperator = 44, RULE_binAndOperator = 45, RULE_binOrOperator = 46, 
		RULE_logAndOperator = 47, RULE_logOrOperator = 48;
	public static final String[] ruleNames = {
		"daedalusFile", "functionDef", "constDef", "classDef", "prototypeDef", 
		"instanceDef", "instanceDecl", "varDecl", "constArrayDef", "constArrayAssignment", 
		"constValueDef", "constValueAssignment", "varArrayDecl", "varValueDecl", 
		"parameterList", "parameterDecl", "statementBlock", "statement", "funcCall", 
		"assignment", "ifCondition", "elseBlock", "elseIfBlock", "ifBlock", "ifBlockStatement", 
		"returnStatement", "funcArgExpression", "expressionBlock", "expression", 
		"arrayIndex", "arraySize", "value", "referenceAtom", "reference", "referenceLeftSide", 
		"typeReference", "nameNode", "parentReference", "assignmentOperator", 
		"addOperator", "bitMoveOperator", "compOperator", "eqOperator", "oneArgOperator", 
		"multOperator", "binAndOperator", "binOrOperator", "logAndOperator", "logOrOperator"
	};

	private static final String[] _LITERAL_NAMES = {
		null, "';'", "','", "'{'", "'}'", "'('", "')'", "'['", "']'", "'='", "'.'", 
		"'+='", "'-='", "'*='", "'/='", "'+'", "'-'", "'<<'", "'>>'", "'<'", "'>'", 
		"'<='", "'>='", "'=='", "'!='", "'!'", "'~'", "'*'", "'/'", "'%'", "'&'", 
		"'|'", "'&&'", "'||'"
	};
	private static final String[] _SYMBOLIC_NAMES = {
		null, null, null, null, null, null, null, null, null, null, null, null, 
		null, null, null, null, null, null, null, null, null, null, null, null, 
		null, null, null, null, null, null, null, null, null, null, "Const", "Var", 
		"If", "Int", "Else", "Func", "String", "Class", "Void", "Return", "Float", 
		"Prototype", "Instance", "Null", "Identifier", "IntegerLiteral", "FloatLiteral", 
		"StringLiteral", "Whitespace", "Newline", "BlockComment", "LineComment"
	};
	public static final Vocabulary VOCABULARY = new VocabularyImpl(_LITERAL_NAMES, _SYMBOLIC_NAMES);

	/**
	 * @deprecated Use {@link #VOCABULARY} instead.
	 */
	@Deprecated
	public static final String[] tokenNames;
	static {
		tokenNames = new String[_SYMBOLIC_NAMES.length];
		for (int i = 0; i < tokenNames.length; i++) {
			tokenNames[i] = VOCABULARY.getLiteralName(i);
			if (tokenNames[i] == null) {
				tokenNames[i] = VOCABULARY.getSymbolicName(i);
			}

			if (tokenNames[i] == null) {
				tokenNames[i] = "<INVALID>";
			}
		}
	}

	@Override
	@Deprecated
	public String[] getTokenNames() {
		return tokenNames;
	}

	@Override

	public Vocabulary getVocabulary() {
		return VOCABULARY;
	}

	@Override
	public String getGrammarFileName() { return "Daedalus.g4"; }

	@Override
	public String[] getRuleNames() { return ruleNames; }

	@Override
	public String getSerializedATN() { return _serializedATN; }

	@Override
	public ATN getATN() { return _ATN; }

	public DaedalusParser(TokenStream input) {
		super(input);
		_interp = new ParserATNSimulator(this,_ATN,_decisionToDFA,_sharedContextCache);
	}
	public static class DaedalusFileContext extends ParserRuleContext {
		public List<FunctionDefContext> functionDef() {
			return getRuleContexts(FunctionDefContext.class);
		}
		public FunctionDefContext functionDef(int i) {
			return getRuleContext(FunctionDefContext.class,i);
		}
		public List<ConstDefContext> constDef() {
			return getRuleContexts(ConstDefContext.class);
		}
		public ConstDefContext constDef(int i) {
			return getRuleContext(ConstDefContext.class,i);
		}
		public List<VarDeclContext> varDecl() {
			return getRuleContexts(VarDeclContext.class);
		}
		public VarDeclContext varDecl(int i) {
			return getRuleContext(VarDeclContext.class,i);
		}
		public List<ClassDefContext> classDef() {
			return getRuleContexts(ClassDefContext.class);
		}
		public ClassDefContext classDef(int i) {
			return getRuleContext(ClassDefContext.class,i);
		}
		public List<PrototypeDefContext> prototypeDef() {
			return getRuleContexts(PrototypeDefContext.class);
		}
		public PrototypeDefContext prototypeDef(int i) {
			return getRuleContext(PrototypeDefContext.class,i);
		}
		public List<InstanceDefContext> instanceDef() {
			return getRuleContexts(InstanceDefContext.class);
		}
		public InstanceDefContext instanceDef(int i) {
			return getRuleContext(InstanceDefContext.class,i);
		}
		public List<InstanceDeclContext> instanceDecl() {
			return getRuleContexts(InstanceDeclContext.class);
		}
		public InstanceDeclContext instanceDecl(int i) {
			return getRuleContext(InstanceDeclContext.class,i);
		}
		public DaedalusFileContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_daedalusFile; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterDaedalusFile(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitDaedalusFile(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitDaedalusFile(this);
			else return visitor.visitChildren(this);
		}
	}

	public final DaedalusFileContext daedalusFile() throws RecognitionException {
		DaedalusFileContext _localctx = new DaedalusFileContext(_ctx, getState());
		enterRule(_localctx, 0, RULE_daedalusFile);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(111);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,1,_ctx);
			while ( _alt!=1 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1+1 ) {
					{
					{
					setState(105);
					_errHandler.sync(this);
					switch ( getInterpreter().adaptivePredict(_input,0,_ctx) ) {
					case 1:
						{
						setState(98);
						functionDef();
						}
						break;
					case 2:
						{
						setState(99);
						constDef();
						}
						break;
					case 3:
						{
						setState(100);
						varDecl();
						}
						break;
					case 4:
						{
						setState(101);
						classDef();
						}
						break;
					case 5:
						{
						setState(102);
						prototypeDef();
						}
						break;
					case 6:
						{
						setState(103);
						instanceDef();
						}
						break;
					case 7:
						{
						setState(104);
						instanceDecl();
						}
						break;
					}
					setState(107);
					match(T__0);
					}
					} 
				}
				setState(113);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,1,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class FunctionDefContext extends ParserRuleContext {
		public TerminalNode Func() { return getToken(DaedalusParser.Func, 0); }
		public TypeReferenceContext typeReference() {
			return getRuleContext(TypeReferenceContext.class,0);
		}
		public NameNodeContext nameNode() {
			return getRuleContext(NameNodeContext.class,0);
		}
		public ParameterListContext parameterList() {
			return getRuleContext(ParameterListContext.class,0);
		}
		public StatementBlockContext statementBlock() {
			return getRuleContext(StatementBlockContext.class,0);
		}
		public FunctionDefContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_functionDef; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterFunctionDef(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitFunctionDef(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitFunctionDef(this);
			else return visitor.visitChildren(this);
		}
	}

	public final FunctionDefContext functionDef() throws RecognitionException {
		FunctionDefContext _localctx = new FunctionDefContext(_ctx, getState());
		enterRule(_localctx, 2, RULE_functionDef);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(114);
			match(Func);
			setState(115);
			typeReference();
			setState(116);
			nameNode();
			setState(117);
			parameterList();
			setState(118);
			statementBlock();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ConstDefContext extends ParserRuleContext {
		public TerminalNode Const() { return getToken(DaedalusParser.Const, 0); }
		public TypeReferenceContext typeReference() {
			return getRuleContext(TypeReferenceContext.class,0);
		}
		public List<ConstValueDefContext> constValueDef() {
			return getRuleContexts(ConstValueDefContext.class);
		}
		public ConstValueDefContext constValueDef(int i) {
			return getRuleContext(ConstValueDefContext.class,i);
		}
		public List<ConstArrayDefContext> constArrayDef() {
			return getRuleContexts(ConstArrayDefContext.class);
		}
		public ConstArrayDefContext constArrayDef(int i) {
			return getRuleContext(ConstArrayDefContext.class,i);
		}
		public ConstDefContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_constDef; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterConstDef(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitConstDef(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitConstDef(this);
			else return visitor.visitChildren(this);
		}
	}

	public final ConstDefContext constDef() throws RecognitionException {
		ConstDefContext _localctx = new ConstDefContext(_ctx, getState());
		enterRule(_localctx, 4, RULE_constDef);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(120);
			match(Const);
			setState(121);
			typeReference();
			setState(124);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,2,_ctx) ) {
			case 1:
				{
				setState(122);
				constValueDef();
				}
				break;
			case 2:
				{
				setState(123);
				constArrayDef();
				}
				break;
			}
			setState(133);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==T__1) {
				{
				{
				setState(126);
				match(T__1);
				setState(129);
				_errHandler.sync(this);
				switch ( getInterpreter().adaptivePredict(_input,3,_ctx) ) {
				case 1:
					{
					setState(127);
					constValueDef();
					}
					break;
				case 2:
					{
					setState(128);
					constArrayDef();
					}
					break;
				}
				}
				}
				setState(135);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ClassDefContext extends ParserRuleContext {
		public TerminalNode Class() { return getToken(DaedalusParser.Class, 0); }
		public NameNodeContext nameNode() {
			return getRuleContext(NameNodeContext.class,0);
		}
		public List<VarDeclContext> varDecl() {
			return getRuleContexts(VarDeclContext.class);
		}
		public VarDeclContext varDecl(int i) {
			return getRuleContext(VarDeclContext.class,i);
		}
		public ClassDefContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_classDef; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterClassDef(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitClassDef(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitClassDef(this);
			else return visitor.visitChildren(this);
		}
	}

	public final ClassDefContext classDef() throws RecognitionException {
		ClassDefContext _localctx = new ClassDefContext(_ctx, getState());
		enterRule(_localctx, 6, RULE_classDef);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(136);
			match(Class);
			setState(137);
			nameNode();
			setState(138);
			match(T__2);
			setState(144);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,5,_ctx);
			while ( _alt!=1 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1+1 ) {
					{
					{
					setState(139);
					varDecl();
					setState(140);
					match(T__0);
					}
					} 
				}
				setState(146);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,5,_ctx);
			}
			setState(147);
			match(T__3);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class PrototypeDefContext extends ParserRuleContext {
		public TerminalNode Prototype() { return getToken(DaedalusParser.Prototype, 0); }
		public NameNodeContext nameNode() {
			return getRuleContext(NameNodeContext.class,0);
		}
		public ParentReferenceContext parentReference() {
			return getRuleContext(ParentReferenceContext.class,0);
		}
		public StatementBlockContext statementBlock() {
			return getRuleContext(StatementBlockContext.class,0);
		}
		public PrototypeDefContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_prototypeDef; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterPrototypeDef(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitPrototypeDef(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitPrototypeDef(this);
			else return visitor.visitChildren(this);
		}
	}

	public final PrototypeDefContext prototypeDef() throws RecognitionException {
		PrototypeDefContext _localctx = new PrototypeDefContext(_ctx, getState());
		enterRule(_localctx, 8, RULE_prototypeDef);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(149);
			match(Prototype);
			setState(150);
			nameNode();
			setState(151);
			match(T__4);
			setState(152);
			parentReference();
			setState(153);
			match(T__5);
			setState(154);
			statementBlock();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class InstanceDefContext extends ParserRuleContext {
		public TerminalNode Instance() { return getToken(DaedalusParser.Instance, 0); }
		public NameNodeContext nameNode() {
			return getRuleContext(NameNodeContext.class,0);
		}
		public ParentReferenceContext parentReference() {
			return getRuleContext(ParentReferenceContext.class,0);
		}
		public StatementBlockContext statementBlock() {
			return getRuleContext(StatementBlockContext.class,0);
		}
		public InstanceDefContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_instanceDef; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterInstanceDef(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitInstanceDef(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitInstanceDef(this);
			else return visitor.visitChildren(this);
		}
	}

	public final InstanceDefContext instanceDef() throws RecognitionException {
		InstanceDefContext _localctx = new InstanceDefContext(_ctx, getState());
		enterRule(_localctx, 10, RULE_instanceDef);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(156);
			match(Instance);
			setState(157);
			nameNode();
			setState(158);
			match(T__4);
			setState(159);
			parentReference();
			setState(160);
			match(T__5);
			setState(161);
			statementBlock();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class InstanceDeclContext extends ParserRuleContext {
		public TerminalNode Instance() { return getToken(DaedalusParser.Instance, 0); }
		public List<NameNodeContext> nameNode() {
			return getRuleContexts(NameNodeContext.class);
		}
		public NameNodeContext nameNode(int i) {
			return getRuleContext(NameNodeContext.class,i);
		}
		public ParentReferenceContext parentReference() {
			return getRuleContext(ParentReferenceContext.class,0);
		}
		public InstanceDeclContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_instanceDecl; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterInstanceDecl(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitInstanceDecl(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitInstanceDecl(this);
			else return visitor.visitChildren(this);
		}
	}

	public final InstanceDeclContext instanceDecl() throws RecognitionException {
		InstanceDeclContext _localctx = new InstanceDeclContext(_ctx, getState());
		enterRule(_localctx, 12, RULE_instanceDecl);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(163);
			match(Instance);
			setState(164);
			nameNode();
			setState(169);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,6,_ctx);
			while ( _alt!=1 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1+1 ) {
					{
					{
					setState(165);
					match(T__1);
					setState(166);
					nameNode();
					}
					} 
				}
				setState(171);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,6,_ctx);
			}
			setState(172);
			match(T__4);
			setState(173);
			parentReference();
			setState(174);
			match(T__5);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class VarDeclContext extends ParserRuleContext {
		public TerminalNode Var() { return getToken(DaedalusParser.Var, 0); }
		public TypeReferenceContext typeReference() {
			return getRuleContext(TypeReferenceContext.class,0);
		}
		public List<VarValueDeclContext> varValueDecl() {
			return getRuleContexts(VarValueDeclContext.class);
		}
		public VarValueDeclContext varValueDecl(int i) {
			return getRuleContext(VarValueDeclContext.class,i);
		}
		public List<VarArrayDeclContext> varArrayDecl() {
			return getRuleContexts(VarArrayDeclContext.class);
		}
		public VarArrayDeclContext varArrayDecl(int i) {
			return getRuleContext(VarArrayDeclContext.class,i);
		}
		public VarDeclContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_varDecl; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterVarDecl(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitVarDecl(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitVarDecl(this);
			else return visitor.visitChildren(this);
		}
	}

	public final VarDeclContext varDecl() throws RecognitionException {
		VarDeclContext _localctx = new VarDeclContext(_ctx, getState());
		enterRule(_localctx, 14, RULE_varDecl);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(176);
			match(Var);
			setState(177);
			typeReference();
			setState(180);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,7,_ctx) ) {
			case 1:
				{
				setState(178);
				varValueDecl();
				}
				break;
			case 2:
				{
				setState(179);
				varArrayDecl();
				}
				break;
			}
			setState(189);
			_errHandler.sync(this);
			_la = _input.LA(1);
			while (_la==T__1) {
				{
				{
				setState(182);
				match(T__1);
				setState(185);
				_errHandler.sync(this);
				switch ( getInterpreter().adaptivePredict(_input,8,_ctx) ) {
				case 1:
					{
					setState(183);
					varValueDecl();
					}
					break;
				case 2:
					{
					setState(184);
					varArrayDecl();
					}
					break;
				}
				}
				}
				setState(191);
				_errHandler.sync(this);
				_la = _input.LA(1);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ConstArrayDefContext extends ParserRuleContext {
		public NameNodeContext nameNode() {
			return getRuleContext(NameNodeContext.class,0);
		}
		public ArraySizeContext arraySize() {
			return getRuleContext(ArraySizeContext.class,0);
		}
		public ConstArrayAssignmentContext constArrayAssignment() {
			return getRuleContext(ConstArrayAssignmentContext.class,0);
		}
		public ConstArrayDefContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_constArrayDef; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterConstArrayDef(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitConstArrayDef(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitConstArrayDef(this);
			else return visitor.visitChildren(this);
		}
	}

	public final ConstArrayDefContext constArrayDef() throws RecognitionException {
		ConstArrayDefContext _localctx = new ConstArrayDefContext(_ctx, getState());
		enterRule(_localctx, 16, RULE_constArrayDef);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(192);
			nameNode();
			setState(193);
			match(T__6);
			setState(194);
			arraySize();
			setState(195);
			match(T__7);
			setState(196);
			constArrayAssignment();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ConstArrayAssignmentContext extends ParserRuleContext {
		public List<ExpressionBlockContext> expressionBlock() {
			return getRuleContexts(ExpressionBlockContext.class);
		}
		public ExpressionBlockContext expressionBlock(int i) {
			return getRuleContext(ExpressionBlockContext.class,i);
		}
		public ConstArrayAssignmentContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_constArrayAssignment; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterConstArrayAssignment(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitConstArrayAssignment(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitConstArrayAssignment(this);
			else return visitor.visitChildren(this);
		}
	}

	public final ConstArrayAssignmentContext constArrayAssignment() throws RecognitionException {
		ConstArrayAssignmentContext _localctx = new ConstArrayAssignmentContext(_ctx, getState());
		enterRule(_localctx, 18, RULE_constArrayAssignment);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(198);
			match(T__8);
			setState(199);
			match(T__2);
			{
			setState(200);
			expressionBlock();
			setState(205);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,10,_ctx);
			while ( _alt!=1 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1+1 ) {
					{
					{
					setState(201);
					match(T__1);
					setState(202);
					expressionBlock();
					}
					} 
				}
				setState(207);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,10,_ctx);
			}
			}
			setState(208);
			match(T__3);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ConstValueDefContext extends ParserRuleContext {
		public NameNodeContext nameNode() {
			return getRuleContext(NameNodeContext.class,0);
		}
		public ConstValueAssignmentContext constValueAssignment() {
			return getRuleContext(ConstValueAssignmentContext.class,0);
		}
		public ConstValueDefContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_constValueDef; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterConstValueDef(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitConstValueDef(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitConstValueDef(this);
			else return visitor.visitChildren(this);
		}
	}

	public final ConstValueDefContext constValueDef() throws RecognitionException {
		ConstValueDefContext _localctx = new ConstValueDefContext(_ctx, getState());
		enterRule(_localctx, 20, RULE_constValueDef);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(210);
			nameNode();
			setState(211);
			constValueAssignment();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ConstValueAssignmentContext extends ParserRuleContext {
		public ExpressionBlockContext expressionBlock() {
			return getRuleContext(ExpressionBlockContext.class,0);
		}
		public ConstValueAssignmentContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_constValueAssignment; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterConstValueAssignment(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitConstValueAssignment(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitConstValueAssignment(this);
			else return visitor.visitChildren(this);
		}
	}

	public final ConstValueAssignmentContext constValueAssignment() throws RecognitionException {
		ConstValueAssignmentContext _localctx = new ConstValueAssignmentContext(_ctx, getState());
		enterRule(_localctx, 22, RULE_constValueAssignment);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(213);
			match(T__8);
			setState(214);
			expressionBlock();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class VarArrayDeclContext extends ParserRuleContext {
		public NameNodeContext nameNode() {
			return getRuleContext(NameNodeContext.class,0);
		}
		public ArraySizeContext arraySize() {
			return getRuleContext(ArraySizeContext.class,0);
		}
		public VarArrayDeclContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_varArrayDecl; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterVarArrayDecl(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitVarArrayDecl(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitVarArrayDecl(this);
			else return visitor.visitChildren(this);
		}
	}

	public final VarArrayDeclContext varArrayDecl() throws RecognitionException {
		VarArrayDeclContext _localctx = new VarArrayDeclContext(_ctx, getState());
		enterRule(_localctx, 24, RULE_varArrayDecl);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(216);
			nameNode();
			setState(217);
			match(T__6);
			setState(218);
			arraySize();
			setState(219);
			match(T__7);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class VarValueDeclContext extends ParserRuleContext {
		public NameNodeContext nameNode() {
			return getRuleContext(NameNodeContext.class,0);
		}
		public VarValueDeclContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_varValueDecl; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterVarValueDecl(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitVarValueDecl(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitVarValueDecl(this);
			else return visitor.visitChildren(this);
		}
	}

	public final VarValueDeclContext varValueDecl() throws RecognitionException {
		VarValueDeclContext _localctx = new VarValueDeclContext(_ctx, getState());
		enterRule(_localctx, 26, RULE_varValueDecl);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(221);
			nameNode();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ParameterListContext extends ParserRuleContext {
		public List<ParameterDeclContext> parameterDecl() {
			return getRuleContexts(ParameterDeclContext.class);
		}
		public ParameterDeclContext parameterDecl(int i) {
			return getRuleContext(ParameterDeclContext.class,i);
		}
		public ParameterListContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_parameterList; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterParameterList(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitParameterList(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitParameterList(this);
			else return visitor.visitChildren(this);
		}
	}

	public final ParameterListContext parameterList() throws RecognitionException {
		ParameterListContext _localctx = new ParameterListContext(_ctx, getState());
		enterRule(_localctx, 28, RULE_parameterList);
		int _la;
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(223);
			match(T__4);
			setState(232);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==Var) {
				{
				setState(224);
				parameterDecl();
				setState(229);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,11,_ctx);
				while ( _alt!=1 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
					if ( _alt==1+1 ) {
						{
						{
						setState(225);
						match(T__1);
						setState(226);
						parameterDecl();
						}
						} 
					}
					setState(231);
					_errHandler.sync(this);
					_alt = getInterpreter().adaptivePredict(_input,11,_ctx);
				}
				}
			}

			setState(234);
			match(T__5);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ParameterDeclContext extends ParserRuleContext {
		public TerminalNode Var() { return getToken(DaedalusParser.Var, 0); }
		public TypeReferenceContext typeReference() {
			return getRuleContext(TypeReferenceContext.class,0);
		}
		public NameNodeContext nameNode() {
			return getRuleContext(NameNodeContext.class,0);
		}
		public ArraySizeContext arraySize() {
			return getRuleContext(ArraySizeContext.class,0);
		}
		public ParameterDeclContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_parameterDecl; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterParameterDecl(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitParameterDecl(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitParameterDecl(this);
			else return visitor.visitChildren(this);
		}
	}

	public final ParameterDeclContext parameterDecl() throws RecognitionException {
		ParameterDeclContext _localctx = new ParameterDeclContext(_ctx, getState());
		enterRule(_localctx, 30, RULE_parameterDecl);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(236);
			match(Var);
			setState(237);
			typeReference();
			setState(238);
			nameNode();
			setState(243);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==T__6) {
				{
				setState(239);
				match(T__6);
				setState(240);
				arraySize();
				setState(241);
				match(T__7);
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class StatementBlockContext extends ParserRuleContext {
		public List<StatementContext> statement() {
			return getRuleContexts(StatementContext.class);
		}
		public StatementContext statement(int i) {
			return getRuleContext(StatementContext.class,i);
		}
		public List<IfBlockStatementContext> ifBlockStatement() {
			return getRuleContexts(IfBlockStatementContext.class);
		}
		public IfBlockStatementContext ifBlockStatement(int i) {
			return getRuleContext(IfBlockStatementContext.class,i);
		}
		public StatementBlockContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_statementBlock; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterStatementBlock(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitStatementBlock(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitStatementBlock(this);
			else return visitor.visitChildren(this);
		}
	}

	public final StatementBlockContext statementBlock() throws RecognitionException {
		StatementBlockContext _localctx = new StatementBlockContext(_ctx, getState());
		enterRule(_localctx, 32, RULE_statementBlock);
		int _la;
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(245);
			match(T__2);
			setState(257);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,16,_ctx);
			while ( _alt!=1 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1+1 ) {
					{
					{
					setState(253);
					_errHandler.sync(this);
					switch (_input.LA(1)) {
					case T__4:
					case T__14:
					case T__15:
					case T__24:
					case T__25:
					case Const:
					case Var:
					case Return:
					case Null:
					case Identifier:
					case IntegerLiteral:
					case FloatLiteral:
					case StringLiteral:
						{
						{
						setState(246);
						statement();
						setState(247);
						match(T__0);
						}
						}
						break;
					case If:
						{
						{
						setState(249);
						ifBlockStatement();
						setState(251);
						_errHandler.sync(this);
						_la = _input.LA(1);
						if (_la==T__0) {
							{
							setState(250);
							match(T__0);
							}
						}

						}
						}
						break;
					default:
						throw new NoViableAltException(this);
					}
					}
					} 
				}
				setState(259);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,16,_ctx);
			}
			setState(260);
			match(T__3);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class StatementContext extends ParserRuleContext {
		public AssignmentContext assignment() {
			return getRuleContext(AssignmentContext.class,0);
		}
		public ReturnStatementContext returnStatement() {
			return getRuleContext(ReturnStatementContext.class,0);
		}
		public ConstDefContext constDef() {
			return getRuleContext(ConstDefContext.class,0);
		}
		public VarDeclContext varDecl() {
			return getRuleContext(VarDeclContext.class,0);
		}
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public StatementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_statement; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterStatement(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitStatement(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitStatement(this);
			else return visitor.visitChildren(this);
		}
	}

	public final StatementContext statement() throws RecognitionException {
		StatementContext _localctx = new StatementContext(_ctx, getState());
		enterRule(_localctx, 34, RULE_statement);
		try {
			setState(267);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,17,_ctx) ) {
			case 1:
				enterOuterAlt(_localctx, 1);
				{
				setState(262);
				assignment();
				}
				break;
			case 2:
				enterOuterAlt(_localctx, 2);
				{
				setState(263);
				returnStatement();
				}
				break;
			case 3:
				enterOuterAlt(_localctx, 3);
				{
				setState(264);
				constDef();
				}
				break;
			case 4:
				enterOuterAlt(_localctx, 4);
				{
				setState(265);
				varDecl();
				}
				break;
			case 5:
				enterOuterAlt(_localctx, 5);
				{
				setState(266);
				expression(0);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class FuncCallContext extends ParserRuleContext {
		public NameNodeContext nameNode() {
			return getRuleContext(NameNodeContext.class,0);
		}
		public List<FuncArgExpressionContext> funcArgExpression() {
			return getRuleContexts(FuncArgExpressionContext.class);
		}
		public FuncArgExpressionContext funcArgExpression(int i) {
			return getRuleContext(FuncArgExpressionContext.class,i);
		}
		public FuncCallContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_funcCall; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterFuncCall(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitFuncCall(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitFuncCall(this);
			else return visitor.visitChildren(this);
		}
	}

	public final FuncCallContext funcCall() throws RecognitionException {
		FuncCallContext _localctx = new FuncCallContext(_ctx, getState());
		enterRule(_localctx, 36, RULE_funcCall);
		int _la;
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(269);
			nameNode();
			setState(270);
			match(T__4);
			setState(279);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << T__4) | (1L << T__14) | (1L << T__15) | (1L << T__24) | (1L << T__25) | (1L << Null) | (1L << Identifier) | (1L << IntegerLiteral) | (1L << FloatLiteral) | (1L << StringLiteral))) != 0)) {
				{
				setState(271);
				funcArgExpression();
				setState(276);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,18,_ctx);
				while ( _alt!=1 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
					if ( _alt==1+1 ) {
						{
						{
						setState(272);
						match(T__1);
						setState(273);
						funcArgExpression();
						}
						} 
					}
					setState(278);
					_errHandler.sync(this);
					_alt = getInterpreter().adaptivePredict(_input,18,_ctx);
				}
				}
			}

			setState(281);
			match(T__5);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class AssignmentContext extends ParserRuleContext {
		public ReferenceLeftSideContext referenceLeftSide() {
			return getRuleContext(ReferenceLeftSideContext.class,0);
		}
		public AssignmentOperatorContext assignmentOperator() {
			return getRuleContext(AssignmentOperatorContext.class,0);
		}
		public ExpressionBlockContext expressionBlock() {
			return getRuleContext(ExpressionBlockContext.class,0);
		}
		public AssignmentContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_assignment; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterAssignment(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitAssignment(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitAssignment(this);
			else return visitor.visitChildren(this);
		}
	}

	public final AssignmentContext assignment() throws RecognitionException {
		AssignmentContext _localctx = new AssignmentContext(_ctx, getState());
		enterRule(_localctx, 38, RULE_assignment);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(283);
			referenceLeftSide();
			setState(284);
			assignmentOperator();
			setState(285);
			expressionBlock();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IfConditionContext extends ParserRuleContext {
		public ExpressionBlockContext expressionBlock() {
			return getRuleContext(ExpressionBlockContext.class,0);
		}
		public IfConditionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_ifCondition; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterIfCondition(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitIfCondition(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitIfCondition(this);
			else return visitor.visitChildren(this);
		}
	}

	public final IfConditionContext ifCondition() throws RecognitionException {
		IfConditionContext _localctx = new IfConditionContext(_ctx, getState());
		enterRule(_localctx, 40, RULE_ifCondition);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(287);
			expressionBlock();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ElseBlockContext extends ParserRuleContext {
		public TerminalNode Else() { return getToken(DaedalusParser.Else, 0); }
		public StatementBlockContext statementBlock() {
			return getRuleContext(StatementBlockContext.class,0);
		}
		public ElseBlockContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_elseBlock; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterElseBlock(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitElseBlock(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitElseBlock(this);
			else return visitor.visitChildren(this);
		}
	}

	public final ElseBlockContext elseBlock() throws RecognitionException {
		ElseBlockContext _localctx = new ElseBlockContext(_ctx, getState());
		enterRule(_localctx, 42, RULE_elseBlock);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(289);
			match(Else);
			setState(290);
			statementBlock();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ElseIfBlockContext extends ParserRuleContext {
		public TerminalNode Else() { return getToken(DaedalusParser.Else, 0); }
		public TerminalNode If() { return getToken(DaedalusParser.If, 0); }
		public IfConditionContext ifCondition() {
			return getRuleContext(IfConditionContext.class,0);
		}
		public StatementBlockContext statementBlock() {
			return getRuleContext(StatementBlockContext.class,0);
		}
		public ElseIfBlockContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_elseIfBlock; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterElseIfBlock(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitElseIfBlock(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitElseIfBlock(this);
			else return visitor.visitChildren(this);
		}
	}

	public final ElseIfBlockContext elseIfBlock() throws RecognitionException {
		ElseIfBlockContext _localctx = new ElseIfBlockContext(_ctx, getState());
		enterRule(_localctx, 44, RULE_elseIfBlock);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(292);
			match(Else);
			setState(293);
			match(If);
			setState(294);
			ifCondition();
			setState(295);
			statementBlock();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IfBlockContext extends ParserRuleContext {
		public TerminalNode If() { return getToken(DaedalusParser.If, 0); }
		public IfConditionContext ifCondition() {
			return getRuleContext(IfConditionContext.class,0);
		}
		public StatementBlockContext statementBlock() {
			return getRuleContext(StatementBlockContext.class,0);
		}
		public IfBlockContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_ifBlock; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterIfBlock(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitIfBlock(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitIfBlock(this);
			else return visitor.visitChildren(this);
		}
	}

	public final IfBlockContext ifBlock() throws RecognitionException {
		IfBlockContext _localctx = new IfBlockContext(_ctx, getState());
		enterRule(_localctx, 46, RULE_ifBlock);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(297);
			match(If);
			setState(298);
			ifCondition();
			setState(299);
			statementBlock();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class IfBlockStatementContext extends ParserRuleContext {
		public IfBlockContext ifBlock() {
			return getRuleContext(IfBlockContext.class,0);
		}
		public List<ElseIfBlockContext> elseIfBlock() {
			return getRuleContexts(ElseIfBlockContext.class);
		}
		public ElseIfBlockContext elseIfBlock(int i) {
			return getRuleContext(ElseIfBlockContext.class,i);
		}
		public ElseBlockContext elseBlock() {
			return getRuleContext(ElseBlockContext.class,0);
		}
		public IfBlockStatementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_ifBlockStatement; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterIfBlockStatement(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitIfBlockStatement(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitIfBlockStatement(this);
			else return visitor.visitChildren(this);
		}
	}

	public final IfBlockStatementContext ifBlockStatement() throws RecognitionException {
		IfBlockStatementContext _localctx = new IfBlockStatementContext(_ctx, getState());
		enterRule(_localctx, 48, RULE_ifBlockStatement);
		int _la;
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(301);
			ifBlock();
			setState(305);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,20,_ctx);
			while ( _alt!=1 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1+1 ) {
					{
					{
					setState(302);
					elseIfBlock();
					}
					} 
				}
				setState(307);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,20,_ctx);
			}
			setState(309);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==Else) {
				{
				setState(308);
				elseBlock();
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ReturnStatementContext extends ParserRuleContext {
		public TerminalNode Return() { return getToken(DaedalusParser.Return, 0); }
		public ExpressionBlockContext expressionBlock() {
			return getRuleContext(ExpressionBlockContext.class,0);
		}
		public ReturnStatementContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_returnStatement; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterReturnStatement(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitReturnStatement(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitReturnStatement(this);
			else return visitor.visitChildren(this);
		}
	}

	public final ReturnStatementContext returnStatement() throws RecognitionException {
		ReturnStatementContext _localctx = new ReturnStatementContext(_ctx, getState());
		enterRule(_localctx, 50, RULE_returnStatement);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(311);
			match(Return);
			setState(313);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if ((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << T__4) | (1L << T__14) | (1L << T__15) | (1L << T__24) | (1L << T__25) | (1L << Null) | (1L << Identifier) | (1L << IntegerLiteral) | (1L << FloatLiteral) | (1L << StringLiteral))) != 0)) {
				{
				setState(312);
				expressionBlock();
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class FuncArgExpressionContext extends ParserRuleContext {
		public ExpressionBlockContext expressionBlock() {
			return getRuleContext(ExpressionBlockContext.class,0);
		}
		public FuncArgExpressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_funcArgExpression; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterFuncArgExpression(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitFuncArgExpression(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitFuncArgExpression(this);
			else return visitor.visitChildren(this);
		}
	}

	public final FuncArgExpressionContext funcArgExpression() throws RecognitionException {
		FuncArgExpressionContext _localctx = new FuncArgExpressionContext(_ctx, getState());
		enterRule(_localctx, 52, RULE_funcArgExpression);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(315);
			expressionBlock();
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ExpressionBlockContext extends ParserRuleContext {
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public ExpressionBlockContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_expressionBlock; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterExpressionBlock(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitExpressionBlock(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitExpressionBlock(this);
			else return visitor.visitChildren(this);
		}
	}

	public final ExpressionBlockContext expressionBlock() throws RecognitionException {
		ExpressionBlockContext _localctx = new ExpressionBlockContext(_ctx, getState());
		enterRule(_localctx, 54, RULE_expressionBlock);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(317);
			expression(0);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ExpressionContext extends ParserRuleContext {
		public ExpressionContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_expression; }
	 
		public ExpressionContext() { }
		public void copyFrom(ExpressionContext ctx) {
			super.copyFrom(ctx);
		}
	}
	public static class BitMoveExpressionContext extends ExpressionContext {
		public List<ExpressionContext> expression() {
			return getRuleContexts(ExpressionContext.class);
		}
		public ExpressionContext expression(int i) {
			return getRuleContext(ExpressionContext.class,i);
		}
		public BitMoveOperatorContext bitMoveOperator() {
			return getRuleContext(BitMoveOperatorContext.class,0);
		}
		public BitMoveExpressionContext(ExpressionContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterBitMoveExpression(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitBitMoveExpression(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitBitMoveExpression(this);
			else return visitor.visitChildren(this);
		}
	}
	public static class OneArgExpressionContext extends ExpressionContext {
		public OneArgOperatorContext oneArgOperator() {
			return getRuleContext(OneArgOperatorContext.class,0);
		}
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public OneArgExpressionContext(ExpressionContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterOneArgExpression(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitOneArgExpression(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitOneArgExpression(this);
			else return visitor.visitChildren(this);
		}
	}
	public static class EqExpressionContext extends ExpressionContext {
		public List<ExpressionContext> expression() {
			return getRuleContexts(ExpressionContext.class);
		}
		public ExpressionContext expression(int i) {
			return getRuleContext(ExpressionContext.class,i);
		}
		public EqOperatorContext eqOperator() {
			return getRuleContext(EqOperatorContext.class,0);
		}
		public EqExpressionContext(ExpressionContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterEqExpression(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitEqExpression(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitEqExpression(this);
			else return visitor.visitChildren(this);
		}
	}
	public static class ValExpressionContext extends ExpressionContext {
		public ValueContext value() {
			return getRuleContext(ValueContext.class,0);
		}
		public ValExpressionContext(ExpressionContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterValExpression(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitValExpression(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitValExpression(this);
			else return visitor.visitChildren(this);
		}
	}
	public static class AddExpressionContext extends ExpressionContext {
		public List<ExpressionContext> expression() {
			return getRuleContexts(ExpressionContext.class);
		}
		public ExpressionContext expression(int i) {
			return getRuleContext(ExpressionContext.class,i);
		}
		public AddOperatorContext addOperator() {
			return getRuleContext(AddOperatorContext.class,0);
		}
		public AddExpressionContext(ExpressionContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterAddExpression(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitAddExpression(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitAddExpression(this);
			else return visitor.visitChildren(this);
		}
	}
	public static class CompExpressionContext extends ExpressionContext {
		public List<ExpressionContext> expression() {
			return getRuleContexts(ExpressionContext.class);
		}
		public ExpressionContext expression(int i) {
			return getRuleContext(ExpressionContext.class,i);
		}
		public CompOperatorContext compOperator() {
			return getRuleContext(CompOperatorContext.class,0);
		}
		public CompExpressionContext(ExpressionContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterCompExpression(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitCompExpression(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitCompExpression(this);
			else return visitor.visitChildren(this);
		}
	}
	public static class LogOrExpressionContext extends ExpressionContext {
		public List<ExpressionContext> expression() {
			return getRuleContexts(ExpressionContext.class);
		}
		public ExpressionContext expression(int i) {
			return getRuleContext(ExpressionContext.class,i);
		}
		public LogOrOperatorContext logOrOperator() {
			return getRuleContext(LogOrOperatorContext.class,0);
		}
		public LogOrExpressionContext(ExpressionContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterLogOrExpression(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitLogOrExpression(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitLogOrExpression(this);
			else return visitor.visitChildren(this);
		}
	}
	public static class BinAndExpressionContext extends ExpressionContext {
		public List<ExpressionContext> expression() {
			return getRuleContexts(ExpressionContext.class);
		}
		public ExpressionContext expression(int i) {
			return getRuleContext(ExpressionContext.class,i);
		}
		public BinAndOperatorContext binAndOperator() {
			return getRuleContext(BinAndOperatorContext.class,0);
		}
		public BinAndExpressionContext(ExpressionContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterBinAndExpression(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitBinAndExpression(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitBinAndExpression(this);
			else return visitor.visitChildren(this);
		}
	}
	public static class BinOrExpressionContext extends ExpressionContext {
		public List<ExpressionContext> expression() {
			return getRuleContexts(ExpressionContext.class);
		}
		public ExpressionContext expression(int i) {
			return getRuleContext(ExpressionContext.class,i);
		}
		public BinOrOperatorContext binOrOperator() {
			return getRuleContext(BinOrOperatorContext.class,0);
		}
		public BinOrExpressionContext(ExpressionContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterBinOrExpression(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitBinOrExpression(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitBinOrExpression(this);
			else return visitor.visitChildren(this);
		}
	}
	public static class MultExpressionContext extends ExpressionContext {
		public List<ExpressionContext> expression() {
			return getRuleContexts(ExpressionContext.class);
		}
		public ExpressionContext expression(int i) {
			return getRuleContext(ExpressionContext.class,i);
		}
		public MultOperatorContext multOperator() {
			return getRuleContext(MultOperatorContext.class,0);
		}
		public MultExpressionContext(ExpressionContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterMultExpression(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitMultExpression(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitMultExpression(this);
			else return visitor.visitChildren(this);
		}
	}
	public static class BracketExpressionContext extends ExpressionContext {
		public ExpressionContext expression() {
			return getRuleContext(ExpressionContext.class,0);
		}
		public BracketExpressionContext(ExpressionContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterBracketExpression(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitBracketExpression(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitBracketExpression(this);
			else return visitor.visitChildren(this);
		}
	}
	public static class LogAndExpressionContext extends ExpressionContext {
		public List<ExpressionContext> expression() {
			return getRuleContexts(ExpressionContext.class);
		}
		public ExpressionContext expression(int i) {
			return getRuleContext(ExpressionContext.class,i);
		}
		public LogAndOperatorContext logAndOperator() {
			return getRuleContext(LogAndOperatorContext.class,0);
		}
		public LogAndExpressionContext(ExpressionContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterLogAndExpression(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitLogAndExpression(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitLogAndExpression(this);
			else return visitor.visitChildren(this);
		}
	}

	public final ExpressionContext expression() throws RecognitionException {
		return expression(0);
	}

	private ExpressionContext expression(int _p) throws RecognitionException {
		ParserRuleContext _parentctx = _ctx;
		int _parentState = getState();
		ExpressionContext _localctx = new ExpressionContext(_ctx, _parentState);
		ExpressionContext _prevctx = _localctx;
		int _startState = 56;
		enterRecursionRule(_localctx, 56, RULE_expression, _p);
		try {
			int _alt;
			enterOuterAlt(_localctx, 1);
			{
			setState(328);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case T__4:
				{
				_localctx = new BracketExpressionContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;

				setState(320);
				match(T__4);
				setState(321);
				expression(0);
				setState(322);
				match(T__5);
				}
				break;
			case T__14:
			case T__15:
			case T__24:
			case T__25:
				{
				_localctx = new OneArgExpressionContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;
				setState(324);
				oneArgOperator();
				setState(325);
				expression(11);
				}
				break;
			case Null:
			case Identifier:
			case IntegerLiteral:
			case FloatLiteral:
			case StringLiteral:
				{
				_localctx = new ValExpressionContext(_localctx);
				_ctx = _localctx;
				_prevctx = _localctx;
				setState(327);
				value();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			_ctx.stop = _input.LT(-1);
			setState(368);
			_errHandler.sync(this);
			_alt = getInterpreter().adaptivePredict(_input,25,_ctx);
			while ( _alt!=2 && _alt!=org.antlr.v4.runtime.atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					if ( _parseListeners!=null ) triggerExitRuleEvent();
					_prevctx = _localctx;
					{
					setState(366);
					_errHandler.sync(this);
					switch ( getInterpreter().adaptivePredict(_input,24,_ctx) ) {
					case 1:
						{
						_localctx = new MultExpressionContext(new ExpressionContext(_parentctx, _parentState));
						pushNewRecursionContext(_localctx, _startState, RULE_expression);
						setState(330);
						if (!(precpred(_ctx, 10))) throw new FailedPredicateException(this, "precpred(_ctx, 10)");
						setState(331);
						multOperator();
						setState(332);
						expression(11);
						}
						break;
					case 2:
						{
						_localctx = new AddExpressionContext(new ExpressionContext(_parentctx, _parentState));
						pushNewRecursionContext(_localctx, _startState, RULE_expression);
						setState(334);
						if (!(precpred(_ctx, 9))) throw new FailedPredicateException(this, "precpred(_ctx, 9)");
						setState(335);
						addOperator();
						setState(336);
						expression(10);
						}
						break;
					case 3:
						{
						_localctx = new BitMoveExpressionContext(new ExpressionContext(_parentctx, _parentState));
						pushNewRecursionContext(_localctx, _startState, RULE_expression);
						setState(338);
						if (!(precpred(_ctx, 8))) throw new FailedPredicateException(this, "precpred(_ctx, 8)");
						setState(339);
						bitMoveOperator();
						setState(340);
						expression(9);
						}
						break;
					case 4:
						{
						_localctx = new CompExpressionContext(new ExpressionContext(_parentctx, _parentState));
						pushNewRecursionContext(_localctx, _startState, RULE_expression);
						setState(342);
						if (!(precpred(_ctx, 7))) throw new FailedPredicateException(this, "precpred(_ctx, 7)");
						setState(343);
						compOperator();
						setState(344);
						expression(8);
						}
						break;
					case 5:
						{
						_localctx = new EqExpressionContext(new ExpressionContext(_parentctx, _parentState));
						pushNewRecursionContext(_localctx, _startState, RULE_expression);
						setState(346);
						if (!(precpred(_ctx, 6))) throw new FailedPredicateException(this, "precpred(_ctx, 6)");
						setState(347);
						eqOperator();
						setState(348);
						expression(7);
						}
						break;
					case 6:
						{
						_localctx = new BinAndExpressionContext(new ExpressionContext(_parentctx, _parentState));
						pushNewRecursionContext(_localctx, _startState, RULE_expression);
						setState(350);
						if (!(precpred(_ctx, 5))) throw new FailedPredicateException(this, "precpred(_ctx, 5)");
						setState(351);
						binAndOperator();
						setState(352);
						expression(6);
						}
						break;
					case 7:
						{
						_localctx = new BinOrExpressionContext(new ExpressionContext(_parentctx, _parentState));
						pushNewRecursionContext(_localctx, _startState, RULE_expression);
						setState(354);
						if (!(precpred(_ctx, 4))) throw new FailedPredicateException(this, "precpred(_ctx, 4)");
						setState(355);
						binOrOperator();
						setState(356);
						expression(5);
						}
						break;
					case 8:
						{
						_localctx = new LogAndExpressionContext(new ExpressionContext(_parentctx, _parentState));
						pushNewRecursionContext(_localctx, _startState, RULE_expression);
						setState(358);
						if (!(precpred(_ctx, 3))) throw new FailedPredicateException(this, "precpred(_ctx, 3)");
						setState(359);
						logAndOperator();
						setState(360);
						expression(4);
						}
						break;
					case 9:
						{
						_localctx = new LogOrExpressionContext(new ExpressionContext(_parentctx, _parentState));
						pushNewRecursionContext(_localctx, _startState, RULE_expression);
						setState(362);
						if (!(precpred(_ctx, 2))) throw new FailedPredicateException(this, "precpred(_ctx, 2)");
						setState(363);
						logOrOperator();
						setState(364);
						expression(3);
						}
						break;
					}
					} 
				}
				setState(370);
				_errHandler.sync(this);
				_alt = getInterpreter().adaptivePredict(_input,25,_ctx);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			unrollRecursionContexts(_parentctx);
		}
		return _localctx;
	}

	public static class ArrayIndexContext extends ParserRuleContext {
		public TerminalNode IntegerLiteral() { return getToken(DaedalusParser.IntegerLiteral, 0); }
		public ReferenceAtomContext referenceAtom() {
			return getRuleContext(ReferenceAtomContext.class,0);
		}
		public ArrayIndexContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_arrayIndex; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterArrayIndex(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitArrayIndex(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitArrayIndex(this);
			else return visitor.visitChildren(this);
		}
	}

	public final ArrayIndexContext arrayIndex() throws RecognitionException {
		ArrayIndexContext _localctx = new ArrayIndexContext(_ctx, getState());
		enterRule(_localctx, 58, RULE_arrayIndex);
		try {
			setState(373);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case IntegerLiteral:
				enterOuterAlt(_localctx, 1);
				{
				setState(371);
				match(IntegerLiteral);
				}
				break;
			case Identifier:
				enterOuterAlt(_localctx, 2);
				{
				setState(372);
				referenceAtom();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ArraySizeContext extends ParserRuleContext {
		public TerminalNode IntegerLiteral() { return getToken(DaedalusParser.IntegerLiteral, 0); }
		public ReferenceAtomContext referenceAtom() {
			return getRuleContext(ReferenceAtomContext.class,0);
		}
		public ArraySizeContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_arraySize; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterArraySize(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitArraySize(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitArraySize(this);
			else return visitor.visitChildren(this);
		}
	}

	public final ArraySizeContext arraySize() throws RecognitionException {
		ArraySizeContext _localctx = new ArraySizeContext(_ctx, getState());
		enterRule(_localctx, 60, RULE_arraySize);
		try {
			setState(377);
			_errHandler.sync(this);
			switch (_input.LA(1)) {
			case IntegerLiteral:
				enterOuterAlt(_localctx, 1);
				{
				setState(375);
				match(IntegerLiteral);
				}
				break;
			case Identifier:
				enterOuterAlt(_localctx, 2);
				{
				setState(376);
				referenceAtom();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ValueContext extends ParserRuleContext {
		public ValueContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_value; }
	 
		public ValueContext() { }
		public void copyFrom(ValueContext ctx) {
			super.copyFrom(ctx);
		}
	}
	public static class IntegerLiteralValueContext extends ValueContext {
		public TerminalNode IntegerLiteral() { return getToken(DaedalusParser.IntegerLiteral, 0); }
		public IntegerLiteralValueContext(ValueContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterIntegerLiteralValue(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitIntegerLiteralValue(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitIntegerLiteralValue(this);
			else return visitor.visitChildren(this);
		}
	}
	public static class FloatLiteralValueContext extends ValueContext {
		public TerminalNode FloatLiteral() { return getToken(DaedalusParser.FloatLiteral, 0); }
		public FloatLiteralValueContext(ValueContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterFloatLiteralValue(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitFloatLiteralValue(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitFloatLiteralValue(this);
			else return visitor.visitChildren(this);
		}
	}
	public static class StringLiteralValueContext extends ValueContext {
		public TerminalNode StringLiteral() { return getToken(DaedalusParser.StringLiteral, 0); }
		public StringLiteralValueContext(ValueContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterStringLiteralValue(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitStringLiteralValue(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitStringLiteralValue(this);
			else return visitor.visitChildren(this);
		}
	}
	public static class NullLiteralValueContext extends ValueContext {
		public TerminalNode Null() { return getToken(DaedalusParser.Null, 0); }
		public NullLiteralValueContext(ValueContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterNullLiteralValue(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitNullLiteralValue(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitNullLiteralValue(this);
			else return visitor.visitChildren(this);
		}
	}
	public static class FuncCallValueContext extends ValueContext {
		public FuncCallContext funcCall() {
			return getRuleContext(FuncCallContext.class,0);
		}
		public FuncCallValueContext(ValueContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterFuncCallValue(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitFuncCallValue(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitFuncCallValue(this);
			else return visitor.visitChildren(this);
		}
	}
	public static class ReferenceValueContext extends ValueContext {
		public ReferenceContext reference() {
			return getRuleContext(ReferenceContext.class,0);
		}
		public ReferenceValueContext(ValueContext ctx) { copyFrom(ctx); }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterReferenceValue(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitReferenceValue(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitReferenceValue(this);
			else return visitor.visitChildren(this);
		}
	}

	public final ValueContext value() throws RecognitionException {
		ValueContext _localctx = new ValueContext(_ctx, getState());
		enterRule(_localctx, 62, RULE_value);
		try {
			setState(385);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,28,_ctx) ) {
			case 1:
				_localctx = new IntegerLiteralValueContext(_localctx);
				enterOuterAlt(_localctx, 1);
				{
				setState(379);
				match(IntegerLiteral);
				}
				break;
			case 2:
				_localctx = new FloatLiteralValueContext(_localctx);
				enterOuterAlt(_localctx, 2);
				{
				setState(380);
				match(FloatLiteral);
				}
				break;
			case 3:
				_localctx = new StringLiteralValueContext(_localctx);
				enterOuterAlt(_localctx, 3);
				{
				setState(381);
				match(StringLiteral);
				}
				break;
			case 4:
				_localctx = new NullLiteralValueContext(_localctx);
				enterOuterAlt(_localctx, 4);
				{
				setState(382);
				match(Null);
				}
				break;
			case 5:
				_localctx = new FuncCallValueContext(_localctx);
				enterOuterAlt(_localctx, 5);
				{
				setState(383);
				funcCall();
				}
				break;
			case 6:
				_localctx = new ReferenceValueContext(_localctx);
				enterOuterAlt(_localctx, 6);
				{
				setState(384);
				reference();
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ReferenceAtomContext extends ParserRuleContext {
		public TerminalNode Identifier() { return getToken(DaedalusParser.Identifier, 0); }
		public ArrayIndexContext arrayIndex() {
			return getRuleContext(ArrayIndexContext.class,0);
		}
		public ReferenceAtomContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_referenceAtom; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterReferenceAtom(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitReferenceAtom(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitReferenceAtom(this);
			else return visitor.visitChildren(this);
		}
	}

	public final ReferenceAtomContext referenceAtom() throws RecognitionException {
		ReferenceAtomContext _localctx = new ReferenceAtomContext(_ctx, getState());
		enterRule(_localctx, 64, RULE_referenceAtom);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(387);
			match(Identifier);
			setState(392);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,29,_ctx) ) {
			case 1:
				{
				setState(388);
				match(T__6);
				setState(389);
				arrayIndex();
				setState(390);
				match(T__7);
				}
				break;
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ReferenceContext extends ParserRuleContext {
		public List<ReferenceAtomContext> referenceAtom() {
			return getRuleContexts(ReferenceAtomContext.class);
		}
		public ReferenceAtomContext referenceAtom(int i) {
			return getRuleContext(ReferenceAtomContext.class,i);
		}
		public ReferenceContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_reference; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterReference(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitReference(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitReference(this);
			else return visitor.visitChildren(this);
		}
	}

	public final ReferenceContext reference() throws RecognitionException {
		ReferenceContext _localctx = new ReferenceContext(_ctx, getState());
		enterRule(_localctx, 66, RULE_reference);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(394);
			referenceAtom();
			setState(397);
			_errHandler.sync(this);
			switch ( getInterpreter().adaptivePredict(_input,30,_ctx) ) {
			case 1:
				{
				setState(395);
				match(T__9);
				setState(396);
				referenceAtom();
				}
				break;
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ReferenceLeftSideContext extends ParserRuleContext {
		public List<ReferenceAtomContext> referenceAtom() {
			return getRuleContexts(ReferenceAtomContext.class);
		}
		public ReferenceAtomContext referenceAtom(int i) {
			return getRuleContext(ReferenceAtomContext.class,i);
		}
		public ReferenceLeftSideContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_referenceLeftSide; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterReferenceLeftSide(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitReferenceLeftSide(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitReferenceLeftSide(this);
			else return visitor.visitChildren(this);
		}
	}

	public final ReferenceLeftSideContext referenceLeftSide() throws RecognitionException {
		ReferenceLeftSideContext _localctx = new ReferenceLeftSideContext(_ctx, getState());
		enterRule(_localctx, 68, RULE_referenceLeftSide);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(399);
			referenceAtom();
			setState(402);
			_errHandler.sync(this);
			_la = _input.LA(1);
			if (_la==T__9) {
				{
				setState(400);
				match(T__9);
				setState(401);
				referenceAtom();
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class TypeReferenceContext extends ParserRuleContext {
		public TerminalNode Identifier() { return getToken(DaedalusParser.Identifier, 0); }
		public TerminalNode Void() { return getToken(DaedalusParser.Void, 0); }
		public TerminalNode Int() { return getToken(DaedalusParser.Int, 0); }
		public TerminalNode Float() { return getToken(DaedalusParser.Float, 0); }
		public TerminalNode String() { return getToken(DaedalusParser.String, 0); }
		public TerminalNode Func() { return getToken(DaedalusParser.Func, 0); }
		public TerminalNode Instance() { return getToken(DaedalusParser.Instance, 0); }
		public TypeReferenceContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_typeReference; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterTypeReference(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitTypeReference(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitTypeReference(this);
			else return visitor.visitChildren(this);
		}
	}

	public final TypeReferenceContext typeReference() throws RecognitionException {
		TypeReferenceContext _localctx = new TypeReferenceContext(_ctx, getState());
		enterRule(_localctx, 70, RULE_typeReference);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(404);
			_la = _input.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << Int) | (1L << Func) | (1L << String) | (1L << Void) | (1L << Float) | (1L << Instance) | (1L << Identifier))) != 0)) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class NameNodeContext extends ParserRuleContext {
		public TerminalNode Identifier() { return getToken(DaedalusParser.Identifier, 0); }
		public NameNodeContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_nameNode; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterNameNode(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitNameNode(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitNameNode(this);
			else return visitor.visitChildren(this);
		}
	}

	public final NameNodeContext nameNode() throws RecognitionException {
		NameNodeContext _localctx = new NameNodeContext(_ctx, getState());
		enterRule(_localctx, 72, RULE_nameNode);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(406);
			match(Identifier);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class ParentReferenceContext extends ParserRuleContext {
		public TerminalNode Identifier() { return getToken(DaedalusParser.Identifier, 0); }
		public ParentReferenceContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_parentReference; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterParentReference(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitParentReference(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitParentReference(this);
			else return visitor.visitChildren(this);
		}
	}

	public final ParentReferenceContext parentReference() throws RecognitionException {
		ParentReferenceContext _localctx = new ParentReferenceContext(_ctx, getState());
		enterRule(_localctx, 74, RULE_parentReference);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(408);
			match(Identifier);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class AssignmentOperatorContext extends ParserRuleContext {
		public AssignmentOperatorContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_assignmentOperator; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterAssignmentOperator(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitAssignmentOperator(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitAssignmentOperator(this);
			else return visitor.visitChildren(this);
		}
	}

	public final AssignmentOperatorContext assignmentOperator() throws RecognitionException {
		AssignmentOperatorContext _localctx = new AssignmentOperatorContext(_ctx, getState());
		enterRule(_localctx, 76, RULE_assignmentOperator);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(410);
			_la = _input.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << T__8) | (1L << T__10) | (1L << T__11) | (1L << T__12) | (1L << T__13))) != 0)) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class AddOperatorContext extends ParserRuleContext {
		public AddOperatorContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_addOperator; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterAddOperator(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitAddOperator(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitAddOperator(this);
			else return visitor.visitChildren(this);
		}
	}

	public final AddOperatorContext addOperator() throws RecognitionException {
		AddOperatorContext _localctx = new AddOperatorContext(_ctx, getState());
		enterRule(_localctx, 78, RULE_addOperator);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(412);
			_la = _input.LA(1);
			if ( !(_la==T__14 || _la==T__15) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class BitMoveOperatorContext extends ParserRuleContext {
		public BitMoveOperatorContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_bitMoveOperator; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterBitMoveOperator(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitBitMoveOperator(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitBitMoveOperator(this);
			else return visitor.visitChildren(this);
		}
	}

	public final BitMoveOperatorContext bitMoveOperator() throws RecognitionException {
		BitMoveOperatorContext _localctx = new BitMoveOperatorContext(_ctx, getState());
		enterRule(_localctx, 80, RULE_bitMoveOperator);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(414);
			_la = _input.LA(1);
			if ( !(_la==T__16 || _la==T__17) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class CompOperatorContext extends ParserRuleContext {
		public CompOperatorContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_compOperator; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterCompOperator(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitCompOperator(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitCompOperator(this);
			else return visitor.visitChildren(this);
		}
	}

	public final CompOperatorContext compOperator() throws RecognitionException {
		CompOperatorContext _localctx = new CompOperatorContext(_ctx, getState());
		enterRule(_localctx, 82, RULE_compOperator);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(416);
			_la = _input.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << T__18) | (1L << T__19) | (1L << T__20) | (1L << T__21))) != 0)) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class EqOperatorContext extends ParserRuleContext {
		public EqOperatorContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_eqOperator; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterEqOperator(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitEqOperator(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitEqOperator(this);
			else return visitor.visitChildren(this);
		}
	}

	public final EqOperatorContext eqOperator() throws RecognitionException {
		EqOperatorContext _localctx = new EqOperatorContext(_ctx, getState());
		enterRule(_localctx, 84, RULE_eqOperator);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(418);
			_la = _input.LA(1);
			if ( !(_la==T__22 || _la==T__23) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class OneArgOperatorContext extends ParserRuleContext {
		public OneArgOperatorContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_oneArgOperator; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterOneArgOperator(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitOneArgOperator(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitOneArgOperator(this);
			else return visitor.visitChildren(this);
		}
	}

	public final OneArgOperatorContext oneArgOperator() throws RecognitionException {
		OneArgOperatorContext _localctx = new OneArgOperatorContext(_ctx, getState());
		enterRule(_localctx, 86, RULE_oneArgOperator);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(420);
			_la = _input.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << T__14) | (1L << T__15) | (1L << T__24) | (1L << T__25))) != 0)) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class MultOperatorContext extends ParserRuleContext {
		public MultOperatorContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_multOperator; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterMultOperator(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitMultOperator(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitMultOperator(this);
			else return visitor.visitChildren(this);
		}
	}

	public final MultOperatorContext multOperator() throws RecognitionException {
		MultOperatorContext _localctx = new MultOperatorContext(_ctx, getState());
		enterRule(_localctx, 88, RULE_multOperator);
		int _la;
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(422);
			_la = _input.LA(1);
			if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & ((1L << T__26) | (1L << T__27) | (1L << T__28))) != 0)) ) {
			_errHandler.recoverInline(this);
			}
			else {
				if ( _input.LA(1)==Token.EOF ) matchedEOF = true;
				_errHandler.reportMatch(this);
				consume();
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class BinAndOperatorContext extends ParserRuleContext {
		public BinAndOperatorContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_binAndOperator; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterBinAndOperator(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitBinAndOperator(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitBinAndOperator(this);
			else return visitor.visitChildren(this);
		}
	}

	public final BinAndOperatorContext binAndOperator() throws RecognitionException {
		BinAndOperatorContext _localctx = new BinAndOperatorContext(_ctx, getState());
		enterRule(_localctx, 90, RULE_binAndOperator);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(424);
			match(T__29);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class BinOrOperatorContext extends ParserRuleContext {
		public BinOrOperatorContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_binOrOperator; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterBinOrOperator(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitBinOrOperator(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitBinOrOperator(this);
			else return visitor.visitChildren(this);
		}
	}

	public final BinOrOperatorContext binOrOperator() throws RecognitionException {
		BinOrOperatorContext _localctx = new BinOrOperatorContext(_ctx, getState());
		enterRule(_localctx, 92, RULE_binOrOperator);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(426);
			match(T__30);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class LogAndOperatorContext extends ParserRuleContext {
		public LogAndOperatorContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_logAndOperator; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterLogAndOperator(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitLogAndOperator(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitLogAndOperator(this);
			else return visitor.visitChildren(this);
		}
	}

	public final LogAndOperatorContext logAndOperator() throws RecognitionException {
		LogAndOperatorContext _localctx = new LogAndOperatorContext(_ctx, getState());
		enterRule(_localctx, 94, RULE_logAndOperator);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(428);
			match(T__31);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public static class LogOrOperatorContext extends ParserRuleContext {
		public LogOrOperatorContext(ParserRuleContext parent, int invokingState) {
			super(parent, invokingState);
		}
		@Override public int getRuleIndex() { return RULE_logOrOperator; }
		@Override
		public void enterRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).enterLogOrOperator(this);
		}
		@Override
		public void exitRule(ParseTreeListener listener) {
			if ( listener instanceof DaedalusListener ) ((DaedalusListener)listener).exitLogOrOperator(this);
		}
		@Override
		public <T> T accept(ParseTreeVisitor<? extends T> visitor) {
			if ( visitor instanceof DaedalusVisitor ) return ((DaedalusVisitor<? extends T>)visitor).visitLogOrOperator(this);
			else return visitor.visitChildren(this);
		}
	}

	public final LogOrOperatorContext logOrOperator() throws RecognitionException {
		LogOrOperatorContext _localctx = new LogOrOperatorContext(_ctx, getState());
		enterRule(_localctx, 96, RULE_logOrOperator);
		try {
			enterOuterAlt(_localctx, 1);
			{
			setState(430);
			match(T__32);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			_errHandler.reportError(this, re);
			_errHandler.recover(this, re);
		}
		finally {
			exitRule();
		}
		return _localctx;
	}

	public boolean sempred(RuleContext _localctx, int ruleIndex, int predIndex) {
		switch (ruleIndex) {
		case 28:
			return expression_sempred((ExpressionContext)_localctx, predIndex);
		}
		return true;
	}
	private boolean expression_sempred(ExpressionContext _localctx, int predIndex) {
		switch (predIndex) {
		case 0:
			return precpred(_ctx, 10);
		case 1:
			return precpred(_ctx, 9);
		case 2:
			return precpred(_ctx, 8);
		case 3:
			return precpred(_ctx, 7);
		case 4:
			return precpred(_ctx, 6);
		case 5:
			return precpred(_ctx, 5);
		case 6:
			return precpred(_ctx, 4);
		case 7:
			return precpred(_ctx, 3);
		case 8:
			return precpred(_ctx, 2);
		}
		return true;
	}

	public static final String _serializedATN =
		"\3\u608b\ua72a\u8133\ub9ed\u417c\u3be7\u7786\u5964\39\u01b3\4\2\t\2\4"+
		"\3\t\3\4\4\t\4\4\5\t\5\4\6\t\6\4\7\t\7\4\b\t\b\4\t\t\t\4\n\t\n\4\13\t"+
		"\13\4\f\t\f\4\r\t\r\4\16\t\16\4\17\t\17\4\20\t\20\4\21\t\21\4\22\t\22"+
		"\4\23\t\23\4\24\t\24\4\25\t\25\4\26\t\26\4\27\t\27\4\30\t\30\4\31\t\31"+
		"\4\32\t\32\4\33\t\33\4\34\t\34\4\35\t\35\4\36\t\36\4\37\t\37\4 \t \4!"+
		"\t!\4\"\t\"\4#\t#\4$\t$\4%\t%\4&\t&\4\'\t\'\4(\t(\4)\t)\4*\t*\4+\t+\4"+
		",\t,\4-\t-\4.\t.\4/\t/\4\60\t\60\4\61\t\61\4\62\t\62\3\2\3\2\3\2\3\2\3"+
		"\2\3\2\3\2\5\2l\n\2\3\2\3\2\7\2p\n\2\f\2\16\2s\13\2\3\3\3\3\3\3\3\3\3"+
		"\3\3\3\3\4\3\4\3\4\3\4\5\4\177\n\4\3\4\3\4\3\4\5\4\u0084\n\4\7\4\u0086"+
		"\n\4\f\4\16\4\u0089\13\4\3\5\3\5\3\5\3\5\3\5\3\5\7\5\u0091\n\5\f\5\16"+
		"\5\u0094\13\5\3\5\3\5\3\6\3\6\3\6\3\6\3\6\3\6\3\6\3\7\3\7\3\7\3\7\3\7"+
		"\3\7\3\7\3\b\3\b\3\b\3\b\7\b\u00aa\n\b\f\b\16\b\u00ad\13\b\3\b\3\b\3\b"+
		"\3\b\3\t\3\t\3\t\3\t\5\t\u00b7\n\t\3\t\3\t\3\t\5\t\u00bc\n\t\7\t\u00be"+
		"\n\t\f\t\16\t\u00c1\13\t\3\n\3\n\3\n\3\n\3\n\3\n\3\13\3\13\3\13\3\13\3"+
		"\13\7\13\u00ce\n\13\f\13\16\13\u00d1\13\13\3\13\3\13\3\f\3\f\3\f\3\r\3"+
		"\r\3\r\3\16\3\16\3\16\3\16\3\16\3\17\3\17\3\20\3\20\3\20\3\20\7\20\u00e6"+
		"\n\20\f\20\16\20\u00e9\13\20\5\20\u00eb\n\20\3\20\3\20\3\21\3\21\3\21"+
		"\3\21\3\21\3\21\3\21\5\21\u00f6\n\21\3\22\3\22\3\22\3\22\3\22\3\22\5\22"+
		"\u00fe\n\22\5\22\u0100\n\22\7\22\u0102\n\22\f\22\16\22\u0105\13\22\3\22"+
		"\3\22\3\23\3\23\3\23\3\23\3\23\5\23\u010e\n\23\3\24\3\24\3\24\3\24\3\24"+
		"\7\24\u0115\n\24\f\24\16\24\u0118\13\24\5\24\u011a\n\24\3\24\3\24\3\25"+
		"\3\25\3\25\3\25\3\26\3\26\3\27\3\27\3\27\3\30\3\30\3\30\3\30\3\30\3\31"+
		"\3\31\3\31\3\31\3\32\3\32\7\32\u0132\n\32\f\32\16\32\u0135\13\32\3\32"+
		"\5\32\u0138\n\32\3\33\3\33\5\33\u013c\n\33\3\34\3\34\3\35\3\35\3\36\3"+
		"\36\3\36\3\36\3\36\3\36\3\36\3\36\3\36\5\36\u014b\n\36\3\36\3\36\3\36"+
		"\3\36\3\36\3\36\3\36\3\36\3\36\3\36\3\36\3\36\3\36\3\36\3\36\3\36\3\36"+
		"\3\36\3\36\3\36\3\36\3\36\3\36\3\36\3\36\3\36\3\36\3\36\3\36\3\36\3\36"+
		"\3\36\3\36\3\36\3\36\3\36\7\36\u0171\n\36\f\36\16\36\u0174\13\36\3\37"+
		"\3\37\5\37\u0178\n\37\3 \3 \5 \u017c\n \3!\3!\3!\3!\3!\3!\5!\u0184\n!"+
		"\3\"\3\"\3\"\3\"\3\"\5\"\u018b\n\"\3#\3#\3#\5#\u0190\n#\3$\3$\3$\5$\u0195"+
		"\n$\3%\3%\3&\3&\3\'\3\'\3(\3(\3)\3)\3*\3*\3+\3+\3,\3,\3-\3-\3.\3.\3/\3"+
		"/\3\60\3\60\3\61\3\61\3\62\3\62\3\62\nq\u0092\u00ab\u00cf\u00e7\u0103"+
		"\u0116\u0133\3:\63\2\4\6\b\n\f\16\20\22\24\26\30\32\34\36 \"$&(*,.\60"+
		"\62\64\668:<>@BDFHJLNPRTVXZ\\^`b\2\n\b\2\'\')*,,..\60\60\62\62\4\2\13"+
		"\13\r\20\3\2\21\22\3\2\23\24\3\2\25\30\3\2\31\32\4\2\21\22\33\34\3\2\35"+
		"\37\2\u01b5\2q\3\2\2\2\4t\3\2\2\2\6z\3\2\2\2\b\u008a\3\2\2\2\n\u0097\3"+
		"\2\2\2\f\u009e\3\2\2\2\16\u00a5\3\2\2\2\20\u00b2\3\2\2\2\22\u00c2\3\2"+
		"\2\2\24\u00c8\3\2\2\2\26\u00d4\3\2\2\2\30\u00d7\3\2\2\2\32\u00da\3\2\2"+
		"\2\34\u00df\3\2\2\2\36\u00e1\3\2\2\2 \u00ee\3\2\2\2\"\u00f7\3\2\2\2$\u010d"+
		"\3\2\2\2&\u010f\3\2\2\2(\u011d\3\2\2\2*\u0121\3\2\2\2,\u0123\3\2\2\2."+
		"\u0126\3\2\2\2\60\u012b\3\2\2\2\62\u012f\3\2\2\2\64\u0139\3\2\2\2\66\u013d"+
		"\3\2\2\28\u013f\3\2\2\2:\u014a\3\2\2\2<\u0177\3\2\2\2>\u017b\3\2\2\2@"+
		"\u0183\3\2\2\2B\u0185\3\2\2\2D\u018c\3\2\2\2F\u0191\3\2\2\2H\u0196\3\2"+
		"\2\2J\u0198\3\2\2\2L\u019a\3\2\2\2N\u019c\3\2\2\2P\u019e\3\2\2\2R\u01a0"+
		"\3\2\2\2T\u01a2\3\2\2\2V\u01a4\3\2\2\2X\u01a6\3\2\2\2Z\u01a8\3\2\2\2\\"+
		"\u01aa\3\2\2\2^\u01ac\3\2\2\2`\u01ae\3\2\2\2b\u01b0\3\2\2\2dl\5\4\3\2"+
		"el\5\6\4\2fl\5\20\t\2gl\5\b\5\2hl\5\n\6\2il\5\f\7\2jl\5\16\b\2kd\3\2\2"+
		"\2ke\3\2\2\2kf\3\2\2\2kg\3\2\2\2kh\3\2\2\2ki\3\2\2\2kj\3\2\2\2lm\3\2\2"+
		"\2mn\7\3\2\2np\3\2\2\2ok\3\2\2\2ps\3\2\2\2qr\3\2\2\2qo\3\2\2\2r\3\3\2"+
		"\2\2sq\3\2\2\2tu\7)\2\2uv\5H%\2vw\5J&\2wx\5\36\20\2xy\5\"\22\2y\5\3\2"+
		"\2\2z{\7$\2\2{~\5H%\2|\177\5\26\f\2}\177\5\22\n\2~|\3\2\2\2~}\3\2\2\2"+
		"\177\u0087\3\2\2\2\u0080\u0083\7\4\2\2\u0081\u0084\5\26\f\2\u0082\u0084"+
		"\5\22\n\2\u0083\u0081\3\2\2\2\u0083\u0082\3\2\2\2\u0084\u0086\3\2\2\2"+
		"\u0085\u0080\3\2\2\2\u0086\u0089\3\2\2\2\u0087\u0085\3\2\2\2\u0087\u0088"+
		"\3\2\2\2\u0088\7\3\2\2\2\u0089\u0087\3\2\2\2\u008a\u008b\7+\2\2\u008b"+
		"\u008c\5J&\2\u008c\u0092\7\5\2\2\u008d\u008e\5\20\t\2\u008e\u008f\7\3"+
		"\2\2\u008f\u0091\3\2\2\2\u0090\u008d\3\2\2\2\u0091\u0094\3\2\2\2\u0092"+
		"\u0093\3\2\2\2\u0092\u0090\3\2\2\2\u0093\u0095\3\2\2\2\u0094\u0092\3\2"+
		"\2\2\u0095\u0096\7\6\2\2\u0096\t\3\2\2\2\u0097\u0098\7/\2\2\u0098\u0099"+
		"\5J&\2\u0099\u009a\7\7\2\2\u009a\u009b\5L\'\2\u009b\u009c\7\b\2\2\u009c"+
		"\u009d\5\"\22\2\u009d\13\3\2\2\2\u009e\u009f\7\60\2\2\u009f\u00a0\5J&"+
		"\2\u00a0\u00a1\7\7\2\2\u00a1\u00a2\5L\'\2\u00a2\u00a3\7\b\2\2\u00a3\u00a4"+
		"\5\"\22\2\u00a4\r\3\2\2\2\u00a5\u00a6\7\60\2\2\u00a6\u00ab\5J&\2\u00a7"+
		"\u00a8\7\4\2\2\u00a8\u00aa\5J&\2\u00a9\u00a7\3\2\2\2\u00aa\u00ad\3\2\2"+
		"\2\u00ab\u00ac\3\2\2\2\u00ab\u00a9\3\2\2\2\u00ac\u00ae\3\2\2\2\u00ad\u00ab"+
		"\3\2\2\2\u00ae\u00af\7\7\2\2\u00af\u00b0\5L\'\2\u00b0\u00b1\7\b\2\2\u00b1"+
		"\17\3\2\2\2\u00b2\u00b3\7%\2\2\u00b3\u00b6\5H%\2\u00b4\u00b7\5\34\17\2"+
		"\u00b5\u00b7\5\32\16\2\u00b6\u00b4\3\2\2\2\u00b6\u00b5\3\2\2\2\u00b7\u00bf"+
		"\3\2\2\2\u00b8\u00bb\7\4\2\2\u00b9\u00bc\5\34\17\2\u00ba\u00bc\5\32\16"+
		"\2\u00bb\u00b9\3\2\2\2\u00bb\u00ba\3\2\2\2\u00bc\u00be\3\2\2\2\u00bd\u00b8"+
		"\3\2\2\2\u00be\u00c1\3\2\2\2\u00bf\u00bd\3\2\2\2\u00bf\u00c0\3\2\2\2\u00c0"+
		"\21\3\2\2\2\u00c1\u00bf\3\2\2\2\u00c2\u00c3\5J&\2\u00c3\u00c4\7\t\2\2"+
		"\u00c4\u00c5\5> \2\u00c5\u00c6\7\n\2\2\u00c6\u00c7\5\24\13\2\u00c7\23"+
		"\3\2\2\2\u00c8\u00c9\7\13\2\2\u00c9\u00ca\7\5\2\2\u00ca\u00cf\58\35\2"+
		"\u00cb\u00cc\7\4\2\2\u00cc\u00ce\58\35\2\u00cd\u00cb\3\2\2\2\u00ce\u00d1"+
		"\3\2\2\2\u00cf\u00d0\3\2\2\2\u00cf\u00cd\3\2\2\2\u00d0\u00d2\3\2\2\2\u00d1"+
		"\u00cf\3\2\2\2\u00d2\u00d3\7\6\2\2\u00d3\25\3\2\2\2\u00d4\u00d5\5J&\2"+
		"\u00d5\u00d6\5\30\r\2\u00d6\27\3\2\2\2\u00d7\u00d8\7\13\2\2\u00d8\u00d9"+
		"\58\35\2\u00d9\31\3\2\2\2\u00da\u00db\5J&\2\u00db\u00dc\7\t\2\2\u00dc"+
		"\u00dd\5> \2\u00dd\u00de\7\n\2\2\u00de\33\3\2\2\2\u00df\u00e0\5J&\2\u00e0"+
		"\35\3\2\2\2\u00e1\u00ea\7\7\2\2\u00e2\u00e7\5 \21\2\u00e3\u00e4\7\4\2"+
		"\2\u00e4\u00e6\5 \21\2\u00e5\u00e3\3\2\2\2\u00e6\u00e9\3\2\2\2\u00e7\u00e8"+
		"\3\2\2\2\u00e7\u00e5\3\2\2\2\u00e8\u00eb\3\2\2\2\u00e9\u00e7\3\2\2\2\u00ea"+
		"\u00e2\3\2\2\2\u00ea\u00eb\3\2\2\2\u00eb\u00ec\3\2\2\2\u00ec\u00ed\7\b"+
		"\2\2\u00ed\37\3\2\2\2\u00ee\u00ef\7%\2\2\u00ef\u00f0\5H%\2\u00f0\u00f5"+
		"\5J&\2\u00f1\u00f2\7\t\2\2\u00f2\u00f3\5> \2\u00f3\u00f4\7\n\2\2\u00f4"+
		"\u00f6\3\2\2\2\u00f5\u00f1\3\2\2\2\u00f5\u00f6\3\2\2\2\u00f6!\3\2\2\2"+
		"\u00f7\u0103\7\5\2\2\u00f8\u00f9\5$\23\2\u00f9\u00fa\7\3\2\2\u00fa\u0100"+
		"\3\2\2\2\u00fb\u00fd\5\62\32\2\u00fc\u00fe\7\3\2\2\u00fd\u00fc\3\2\2\2"+
		"\u00fd\u00fe\3\2\2\2\u00fe\u0100\3\2\2\2\u00ff\u00f8\3\2\2\2\u00ff\u00fb"+
		"\3\2\2\2\u0100\u0102\3\2\2\2\u0101\u00ff\3\2\2\2\u0102\u0105\3\2\2\2\u0103"+
		"\u0104\3\2\2\2\u0103\u0101\3\2\2\2\u0104\u0106\3\2\2\2\u0105\u0103\3\2"+
		"\2\2\u0106\u0107\7\6\2\2\u0107#\3\2\2\2\u0108\u010e\5(\25\2\u0109\u010e"+
		"\5\64\33\2\u010a\u010e\5\6\4\2\u010b\u010e\5\20\t\2\u010c\u010e\5:\36"+
		"\2\u010d\u0108\3\2\2\2\u010d\u0109\3\2\2\2\u010d\u010a\3\2\2\2\u010d\u010b"+
		"\3\2\2\2\u010d\u010c\3\2\2\2\u010e%\3\2\2\2\u010f\u0110\5J&\2\u0110\u0119"+
		"\7\7\2\2\u0111\u0116\5\66\34\2\u0112\u0113\7\4\2\2\u0113\u0115\5\66\34"+
		"\2\u0114\u0112\3\2\2\2\u0115\u0118\3\2\2\2\u0116\u0117\3\2\2\2\u0116\u0114"+
		"\3\2\2\2\u0117\u011a\3\2\2\2\u0118\u0116\3\2\2\2\u0119\u0111\3\2\2\2\u0119"+
		"\u011a\3\2\2\2\u011a\u011b\3\2\2\2\u011b\u011c\7\b\2\2\u011c\'\3\2\2\2"+
		"\u011d\u011e\5F$\2\u011e\u011f\5N(\2\u011f\u0120\58\35\2\u0120)\3\2\2"+
		"\2\u0121\u0122\58\35\2\u0122+\3\2\2\2\u0123\u0124\7(\2\2\u0124\u0125\5"+
		"\"\22\2\u0125-\3\2\2\2\u0126\u0127\7(\2\2\u0127\u0128\7&\2\2\u0128\u0129"+
		"\5*\26\2\u0129\u012a\5\"\22\2\u012a/\3\2\2\2\u012b\u012c\7&\2\2\u012c"+
		"\u012d\5*\26\2\u012d\u012e\5\"\22\2\u012e\61\3\2\2\2\u012f\u0133\5\60"+
		"\31\2\u0130\u0132\5.\30\2\u0131\u0130\3\2\2\2\u0132\u0135\3\2\2\2\u0133"+
		"\u0134\3\2\2\2\u0133\u0131\3\2\2\2\u0134\u0137\3\2\2\2\u0135\u0133\3\2"+
		"\2\2\u0136\u0138\5,\27\2\u0137\u0136\3\2\2\2\u0137\u0138\3\2\2\2\u0138"+
		"\63\3\2\2\2\u0139\u013b\7-\2\2\u013a\u013c\58\35\2\u013b\u013a\3\2\2\2"+
		"\u013b\u013c\3\2\2\2\u013c\65\3\2\2\2\u013d\u013e\58\35\2\u013e\67\3\2"+
		"\2\2\u013f\u0140\5:\36\2\u01409\3\2\2\2\u0141\u0142\b\36\1\2\u0142\u0143"+
		"\7\7\2\2\u0143\u0144\5:\36\2\u0144\u0145\7\b\2\2\u0145\u014b\3\2\2\2\u0146"+
		"\u0147\5X-\2\u0147\u0148\5:\36\r\u0148\u014b\3\2\2\2\u0149\u014b\5@!\2"+
		"\u014a\u0141\3\2\2\2\u014a\u0146\3\2\2\2\u014a\u0149\3\2\2\2\u014b\u0172"+
		"\3\2\2\2\u014c\u014d\f\f\2\2\u014d\u014e\5Z.\2\u014e\u014f\5:\36\r\u014f"+
		"\u0171\3\2\2\2\u0150\u0151\f\13\2\2\u0151\u0152\5P)\2\u0152\u0153\5:\36"+
		"\f\u0153\u0171\3\2\2\2\u0154\u0155\f\n\2\2\u0155\u0156\5R*\2\u0156\u0157"+
		"\5:\36\13\u0157\u0171\3\2\2\2\u0158\u0159\f\t\2\2\u0159\u015a\5T+\2\u015a"+
		"\u015b\5:\36\n\u015b\u0171\3\2\2\2\u015c\u015d\f\b\2\2\u015d\u015e\5V"+
		",\2\u015e\u015f\5:\36\t\u015f\u0171\3\2\2\2\u0160\u0161\f\7\2\2\u0161"+
		"\u0162\5\\/\2\u0162\u0163\5:\36\b\u0163\u0171\3\2\2\2\u0164\u0165\f\6"+
		"\2\2\u0165\u0166\5^\60\2\u0166\u0167\5:\36\7\u0167\u0171\3\2\2\2\u0168"+
		"\u0169\f\5\2\2\u0169\u016a\5`\61\2\u016a\u016b\5:\36\6\u016b\u0171\3\2"+
		"\2\2\u016c\u016d\f\4\2\2\u016d\u016e\5b\62\2\u016e\u016f\5:\36\5\u016f"+
		"\u0171\3\2\2\2\u0170\u014c\3\2\2\2\u0170\u0150\3\2\2\2\u0170\u0154\3\2"+
		"\2\2\u0170\u0158\3\2\2\2\u0170\u015c\3\2\2\2\u0170\u0160\3\2\2\2\u0170"+
		"\u0164\3\2\2\2\u0170\u0168\3\2\2\2\u0170\u016c\3\2\2\2\u0171\u0174\3\2"+
		"\2\2\u0172\u0170\3\2\2\2\u0172\u0173\3\2\2\2\u0173;\3\2\2\2\u0174\u0172"+
		"\3\2\2\2\u0175\u0178\7\63\2\2\u0176\u0178\5B\"\2\u0177\u0175\3\2\2\2\u0177"+
		"\u0176\3\2\2\2\u0178=\3\2\2\2\u0179\u017c\7\63\2\2\u017a\u017c\5B\"\2"+
		"\u017b\u0179\3\2\2\2\u017b\u017a\3\2\2\2\u017c?\3\2\2\2\u017d\u0184\7"+
		"\63\2\2\u017e\u0184\7\64\2\2\u017f\u0184\7\65\2\2\u0180\u0184\7\61\2\2"+
		"\u0181\u0184\5&\24\2\u0182\u0184\5D#\2\u0183\u017d\3\2\2\2\u0183\u017e"+
		"\3\2\2\2\u0183\u017f\3\2\2\2\u0183\u0180\3\2\2\2\u0183\u0181\3\2\2\2\u0183"+
		"\u0182\3\2\2\2\u0184A\3\2\2\2\u0185\u018a\7\62\2\2\u0186\u0187\7\t\2\2"+
		"\u0187\u0188\5<\37\2\u0188\u0189\7\n\2\2\u0189\u018b\3\2\2\2\u018a\u0186"+
		"\3\2\2\2\u018a\u018b\3\2\2\2\u018bC\3\2\2\2\u018c\u018f\5B\"\2\u018d\u018e"+
		"\7\f\2\2\u018e\u0190\5B\"\2\u018f\u018d\3\2\2\2\u018f\u0190\3\2\2\2\u0190"+
		"E\3\2\2\2\u0191\u0194\5B\"\2\u0192\u0193\7\f\2\2\u0193\u0195\5B\"\2\u0194"+
		"\u0192\3\2\2\2\u0194\u0195\3\2\2\2\u0195G\3\2\2\2\u0196\u0197\t\2\2\2"+
		"\u0197I\3\2\2\2\u0198\u0199\7\62\2\2\u0199K\3\2\2\2\u019a\u019b\7\62\2"+
		"\2\u019bM\3\2\2\2\u019c\u019d\t\3\2\2\u019dO\3\2\2\2\u019e\u019f\t\4\2"+
		"\2\u019fQ\3\2\2\2\u01a0\u01a1\t\5\2\2\u01a1S\3\2\2\2\u01a2\u01a3\t\6\2"+
		"\2\u01a3U\3\2\2\2\u01a4\u01a5\t\7\2\2\u01a5W\3\2\2\2\u01a6\u01a7\t\b\2"+
		"\2\u01a7Y\3\2\2\2\u01a8\u01a9\t\t\2\2\u01a9[\3\2\2\2\u01aa\u01ab\7 \2"+
		"\2\u01ab]\3\2\2\2\u01ac\u01ad\7!\2\2\u01ad_\3\2\2\2\u01ae\u01af\7\"\2"+
		"\2\u01afa\3\2\2\2\u01b0\u01b1\7#\2\2\u01b1c\3\2\2\2\"kq~\u0083\u0087\u0092"+
		"\u00ab\u00b6\u00bb\u00bf\u00cf\u00e7\u00ea\u00f5\u00fd\u00ff\u0103\u010d"+
		"\u0116\u0119\u0133\u0137\u013b\u014a\u0170\u0172\u0177\u017b\u0183\u018a"+
		"\u018f\u0194";
	public static final ATN _ATN =
		new ATNDeserializer().deserialize(_serializedATN.toCharArray());
	static {
		_decisionToDFA = new DFA[_ATN.getNumberOfDecisions()];
		for (int i = 0; i < _ATN.getNumberOfDecisions(); i++) {
			_decisionToDFA[i] = new DFA(_ATN.getDecisionState(i), i);
		}
	}
}