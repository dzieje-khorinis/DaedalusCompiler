grammar Daedalus;

// lexer
Const : 'const' | 'CONST';
Var: 'var' | 'VAR';
If : 'if' | 'IF';
Int: 'int' | 'INT';
Else: 'else' | 'ELSE';
Func: 'func' | 'FUNC';
String: 'string' | 'STRING';
Class: 'class' | 'CLASS';
Void: 'void' | 'VOID';
Return: 'return' | 'RETURN';
Float: 'float' | 'FLOAT';
Prototype: 'prototype' | 'PROTOTYPE';
Instance: 'instance' | 'INSTANCE';
Null: 'null' | 'Null' | 'NULL' ;
NoFunc: 'nofunc' | 'NoFunc' | 'NOFUNC';
While: 'while';
Break: 'break';
Continue: 'continue';
Extern: 'extern';

Identifier : IdStart IdContinue*;
IntegerLiteral : Digit+;
FloatLiteral : PointFloat | ExponentFloat;
StringLiteral : '"' (~["\\\r\n] | '\\' (. | EOF))* '"';

Whitespace : [ \t]+ -> skip;
Newline : ('\r''\n'?| '\n') -> skip;
BlockComment :   '/*' .*? '*/' -> skip;
LineComment :   '//' ~[\r\n]* -> skip ;

UnexpectedCharacter : . ;

// fragments
fragment IdStart : GermanCharacter | [a-zA-Z_];
fragment IdContinue : IdStart | Digit | SpecialCharacter;
fragment SpecialCharacter : [@^];
fragment GermanCharacter : [\u00DF\u00E4\u00F6\u00FC];
fragment Digit : [0-9];
fragment PointFloat : Digit* '.' Digit+ | Digit+ '.';
fragment ExponentFloat : (Digit+ | PointFloat) Exponent;
fragment Exponent : [eE] [+-]? Digit+;


//parser
daedalusFile:  (blockDef | inlineDef)*? EOF;
blockDef : (functionDef | classDef | prototypeDef | instanceDef)';'?;
inlineDef :  (externFunctionDecl | constDef | varDecl | instanceDecl)';'; 

externFunctionDecl: Extern Func dataType nameNode parameterList;
functionDef: Func dataType nameNode parameterList statementBlock;
constDef: Const dataType (constValueDef | constArrayDef) (',' (constValueDef | constArrayDef) )*;
classDef: Class nameNode '{' ( varDecl ';' )*? '}';
prototypeDef: Prototype nameNode '(' parentReference ')' statementBlock;
instanceDef: Instance nameNode '(' parentReference ')' statementBlock;
instanceDecl: Instance nameNode ( ',' nameNode )*? '(' parentReference ')';
varDecl: Var dataType (varValueDecl | varArrayDecl) (',' (varValueDecl | varArrayDecl) )* ;

constArrayDef: nameNode '[' arraySize ']' constArrayAssignment;
constArrayAssignment: '=' '{' ( expression (',' expression)*? ) '}';

constValueDef: nameNode constValueAssignment;
constValueAssignment: '=' expression;

varArrayDecl: nameNode '[' arraySize ']' varArrayAssignment?;
varArrayAssignment: '=' '{' expression (',' expression)*  '}';

varValueDecl: nameNode varValueAssignment?;
varValueAssignment: '=' expression;

parameterList: '(' (parameterDecl (',' parameterDecl)*? )? ')';
parameterDecl: Var dataType nameNode ('[' arraySize ']')?;
statementBlock: '{' ( ( (statement ';')  | ( (ifBlockStatement | whileStatement) ';'? ) ) )*? '}';
statement: assignment | returnStatement | constDef | varDecl | breakStatement | continueStatement | expression;
functionCall: nameNode '(' ( expression ( ',' expression )*? )? ')';
assignment: reference assignmentOperator expression;
elseBlock: Else statementBlock;
elseIfBlock: Else If expression statementBlock;
ifBlock: If expression statementBlock;
ifBlockStatement: ifBlock ( elseIfBlock )*? ( elseBlock )?;
returnStatement: Return ( expression )?;
whileStatement: While '(' expression ')' statementBlock;
breakStatement: Break;
continueStatement: Continue;

expression
    : '(' expression ')' #bracketExpression
    | (oper=unaryOperator) expression #unaryExpression
    | expression (oper=multOperator) expression #multExpression
    | expression (oper=addOperator) expression #addExpression
    | expression (oper=bitMoveOperator) expression #bitMoveExpression
    | expression (oper=compOperator) expression #compExpression
    | expression (oper=eqOperator) expression #eqExpression
    | expression (oper=binAndOperator) expression #binAndExpression
    | expression (oper=binOrOperator) expression #binOrExpression
    | expression (oper=logAndOperator) expression #logAndExpression
    | expression (oper=logOrOperator) expression #logOrExpression
    | value #valueExpression
    ;

arrayIndex : IntegerLiteral | reference;
arraySize : IntegerLiteral | reference;

value
    : IntegerLiteral #integerLiteralValue
    | FloatLiteral #floatLiteralValue
    | StringLiteral #stringLiteralValue
    | Null #nullLiteralValue
    | NoFunc #noFuncLiteralValue
    | functionCall #functionCallValue
    | reference #referenceValue
    ;
    
referenceAtom: nameNode ( '[' arrayIndex ']')?;
reference: referenceAtom ( '.' referenceAtom )*;

dataType: Identifier | Void | Int | Float | String | Func | Instance;

nameNode: Identifier | While | Break | Continue;

parentReference: Identifier;

assignmentOperator:  '=' | '+=' | '-=' | '*=' | '/=';
addOperator: '+' | '-';
bitMoveOperator: '<<' | '>>';
compOperator: '<' | '>' | '<=' | '>=';
eqOperator: '==' | '!=';
unaryOperator: '-' | '!' | '~' | '+';
multOperator: '*' | '/' | '%';
binAndOperator: '&';
binOrOperator: '|';
logAndOperator: '&&';
logOrOperator: '||';