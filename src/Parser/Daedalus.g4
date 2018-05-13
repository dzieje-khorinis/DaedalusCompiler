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

Identifier : [a-zA-Z_] ([0-9] | [a-zA-Z_])*;
IntegerLiteral : [0-9]+;
FloatLiteral : [0-9]+ '.' [0-9]+;
StringLiteral : '"' (~["\\\r\n] | '\\' (. | EOF))* '"';

Whitespace : [ \t]+ -> skip;
Newline : ('\r''\n'?| '\n') -> skip;
BlockComment :   '/*' .*? '*/' -> skip;
LineComment :   '//' ~[\r\n]* -> skip ;

//parser
daedalusFile: (( functionDef | constDef | varDecl | classDef | prototypeDef | instanceDef | instanceDecl )';')*?;

functionDef: Func typeReference nameNode parameterList statementBlock;
constDef: Const typeReference (constValueDef | constArrayDef) (',' (constValueDef | constArrayDef) )*;
classDef: Class nameNode '{' ( varDecl ';' )*? '}';
prototypeDef: Prototype nameNode '(' referenceNode ')' statementBlock;
instanceDef: Instance nameNode '(' referenceNode ')' statementBlock;
instanceDecl: Instance nameNode ( ',' referenceNode )*? '(' nameNode ')';
varDecl: Var typeReference (varValueDecl | varArrayDecl) (',' (varValueDecl | varArrayDecl) )* ;

constArrayDef: nameNode '[' simpleValue ']' constArrayAssignment;
constArrayAssignment: '=' '{' ( expressionBlock (',' expressionBlock)*? ) '}';

constValueDef: nameNode constValueAssignment;
constValueAssignment: '=' expressionBlock;

varArrayDecl: nameNode '[' simpleValue ']';
varValueDecl: nameNode;

parameterList: '(' (parameterDecl (',' parameterDecl)*? )? ')';
parameterDecl: Var typeReference nameNode ('[' simpleValue ']')?;
statementBlock: '{' ( ( (statement ';')  | ( ifBlockStatement ( ';' )? ) ) )*? '}';
statement: assignment | returnStatement | constDef | varDecl | expression;
funcCall: nameNode '(' ( funcArgExpression ( ',' funcArgExpression )*? )? ')';
assignment: complexReference assigmentOperator expressionBlock;
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
    | expression multOperators expression #multExpression
    | expression addOperators expression #addExpression
    | expression ('<<' | '>>') expression #bitMoveExpression
    | expression ('<' | '>' | '<=' | '>=') expression #compExpression
    | expression ('==' | '!=') expression #eqExpression
    | expression ('&' | '|' | '&&' | '||') expression #bitExpression
    | value #valExpression
    ;

simpleValue: IntegerLiteral | referenceNode;
value
    : IntegerLiteral #integerLiteralValue
    | FloatLiteral #floatLiteralValue
    | StringLiteral #stringLiteralValue
    | Null #nullLiteralValue
    | funcCall #funcCallValue
    | complexReference #complexReferenceValue
    ;
complexReference: complexReferenceNode ( '.' complexReferenceNode )?;
complexReferenceNode: referenceNode ( '[' simpleValue ']')?;
typeReference:  ( referenceNode  | Void | Int | Float | String | Func | Instance);
nameNode: Identifier;
referenceNode: Identifier;
assigmentOperator:  '=' | '+=' | '-=' | '*=' | '/=';
addOperators: '+' | '-';
oneArgOperator: '-' | '!' | '~' | '+';
multOperators: '*' | '/' | '%';