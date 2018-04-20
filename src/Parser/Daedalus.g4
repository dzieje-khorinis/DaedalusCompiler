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
constDef: Const typeReference nameNode ( '[' simpleValue ']')? constAssignment;
classDef: Class nameNode '{' ( varDecl ';' )*? '}';
prototypeDef: Prototype nameNode '(' referenceNode ')' '{' ( ( assignment | funcCall ) ';' )*? '}';
instanceDef: Instance nameNode '(' referenceNode ')' '{' ( ( assignment | funcCall ) ';' )*? '}';
instanceDecl: Instance nameNode ( ',' referenceNode )*? '(' nameNode ')';
varDecl: Var typeReference nameNode ( ('[' simpleValue ']') | (',' nameNode )* )? ;

parameterList: '(' (parameterDecl (',' parameterDecl)*? )? ')';
parameterDecl: Var typeReference nameNode ('[' simpleValue ']')?;
statementBlock: '{' ( ( (statement ';')  | ( ifBlock ( ';' )? ) ) )*? '}';
statement: funcCall | assignment | returnStatement | constDef | varDecl | expression;
funcCall: nameNode '(' ( expression ( ',' expression )*? )? ')';
assignment: complexReference ( '=' | '+=' | '-=' | '*=' | '/=' ) expression;
ifBlock: If expression statementBlock ( Else If expression statementBlock )*? ( Else statementBlock )?;
returnStatement: Return ( expression )?;

constAssignment: '=' ( expression | arrayLiteral );
arrayLiteral: '{' ( expression (',' expression)*? ) '}';

expression
    : '(' expression ')'
    | ('-' | '!' | '~' | '+') expression
    | expression ('*' | '/' | '%') expression
    | expression ('+' | '-') expression
    | expression ('<<' | '>>') expression
    | expression ('<' | '>' | '<=' | '>=') expression
    | expression ('==' | '!=') expression
    | expression ('&' | '|' | '&&' | '||') expression
    | value
    | nameNode
    ;

simpleValue: IntegerLiteral | referenceNode;
value: IntegerLiteral | FloatLiteral | StringLiteral | Null | funcCall | complexReference;
complexReference: complexReferenceNode ( '.' complexReferenceNode )?;
complexReferenceNode: referenceNode ( '[' simpleValue ']')?;
typeReference:  ( referenceNode  | Void | Int | Float | String | Func | Instance);
nameNode: Identifier;
referenceNode: Identifier;