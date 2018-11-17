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
Null: 'null' | 'Null';

Identifier : IdStart IdContinue*;
IntegerLiteral : Digit+;
FloatLiteral : PointFloat | ExponentFloat;
StringLiteral : '"' (~["\\\r\n] | '\\' (. | EOF))* '"';

Whitespace : [ \t]+ -> skip;
Newline : ('\r''\n'?| '\n') -> skip;
BlockComment :   '/*' .*? '*/' -> skip;
LineComment :   '//' ~[\r\n]* -> skip ;

// fragments
fragment IdStart : GermanCharacter | [a-zA-Z_];
fragment IdContinue : IdStart | Digit;
fragment GermanCharacter : [\u00DF\u00E4\u00F6\u00FC];
fragment Digit : [0-9];
fragment PointFloat : Digit* '.' Digit+ | Digit+ '.';
fragment ExponentFloat : (Digit+ | PointFloat) Exponent;
fragment Exponent : [eE] [+-]? Digit+;


//parser
daedalusFile:  (blockDef | inlineDef)*? EOF;
blockDef : (functionDef  | classDef | prototypeDef | instanceDef)';'?;
inlineDef :  (constDef | varDecl | instanceDecl )';';


functionDef: Func typeReference nameNode parameterList statementBlock;
constDef: Const typeReference (constValueDef | constArrayDef) (',' (constValueDef | constArrayDef) )*;
classDef: Class nameNode '{' ( varDecl ';' )*? '}';
prototypeDef: Prototype nameNode '(' parentReference ')' statementBlock;
instanceDef: Instance nameNode '(' parentReference ')' statementBlock;
instanceDecl: Instance nameNode ( ',' nameNode )*? '(' parentReference ')';
varDecl: Var typeReference (varValueDecl | varArrayDecl) (',' (varValueDecl | varArrayDecl) )* ;

constArrayDef: nameNode '[' arraySize ']' constArrayAssignment;
constArrayAssignment: '=' '{' ( expressionBlock (',' expressionBlock)*? ) '}';

constValueDef: nameNode constValueAssignment;
constValueAssignment: '=' expressionBlock;

varArrayDecl: nameNode '[' arraySize ']';
varValueDecl: nameNode;

parameterList: '(' (parameterDecl (',' parameterDecl)*? )? ')';
parameterDecl: Var typeReference nameNode ('[' arraySize ']')?;
statementBlock: '{' ( ( (statement ';')  | ( ifBlockStatement ( ';' )? ) ) )*? '}';
statement: assignment | returnStatement | constDef | varDecl | funcCall;
funcCall: nameNode '(' ( funcArgExpression ( ',' funcArgExpression )*? )? ')';
assignment: referenceLeftSide assignmentOperator expressionBlock;
ifCondition: expressionBlock;
elseBlock: Else statementBlock;
elseIfBlock: Else If ifCondition statementBlock;
ifBlock: If ifCondition statementBlock;
ifBlockStatement: ifBlock ( elseIfBlock )*? ( elseBlock )?;
returnStatement: Return ( expressionBlock )?;

funcArgExpression: expressionBlock; // we use that to detect func call args
expressionBlock: expression; // we use that expression to force parser threat expression as a block

expression
    : '(' expression ')' #bracketExpression
    | oneArgOperator expression #oneArgExpression
    | expression multOperator expression #multExpression
    | expression addOperator expression #addExpression
    | expression bitMoveOperator expression #bitMoveExpression
    | expression compOperator expression #compExpression
    | expression eqOperator expression #eqExpression
    | expression binAndOperator expression #binAndExpression
    | expression binOrOperator expression #binOrExpression
    | expression logAndOperator expression #logAndExpression
    | expression logOrOperator expression #logOrExpression
    | value #valExpression
    ;

arrayIndex : IntegerLiteral | referenceAtom;
arraySize : IntegerLiteral | referenceAtom;

value
    : IntegerLiteral #integerLiteralValue
    | FloatLiteral #floatLiteralValue
    | StringLiteral #stringLiteralValue
    | Null #nullLiteralValue
    | funcCall #funcCallValue
    | reference #referenceValue
    ;
    
referenceAtom: Identifier ( '[' arrayIndex ']')?;
reference: referenceAtom ( '.' referenceAtom )?;
referenceLeftSide: referenceAtom ( '.' referenceAtom )?;

typeReference:  ( Identifier | Void | Int | Float | String | Func | Instance);

nameNode: Identifier;

parentReference: Identifier;

assignmentOperator:  '=' | '+=' | '-=' | '*=' | '/=';
addOperator: '+' | '-';
bitMoveOperator: '<<' | '>>';
compOperator: '<' | '>' | '<=' | '>=';
eqOperator: '==' | '!=';
oneArgOperator: '-' | '!' | '~' | '+';
multOperator: '*' | '/' | '%';
binAndOperator: '&';
binOrOperator: '|';
logAndOperator: '&&';
logOrOperator: '||';