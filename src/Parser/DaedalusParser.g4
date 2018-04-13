grammar DaedalusParser;

// lexer
Identifier: (Letter |'_')  ( Nondigit | Digit )*;

Nondigit: '_' | '@' | Letter ;


Digit: '0' | Non_zero_digit ;

StringLiteral: '"' (~["\\\r\n] | '\\' (. | EOF))* '"';

IntegerLiteral: Digit+ ;

FloatLiteral: ('+' | '-') (Digit+ '.' Digit* | Digit* '.' Digit+ | Digit+);

Non_zero_digit: '1' | '2' | '3' | '4' | '5' | '6' | '7' | '8' | '9' ;

Letter: 'A' | 'B' | 'C' | 'D' | 'E' | 'F' | 'G' | 'H' | 'I' | 'J' | 'K' | 'L' | 'M' | 'N'
 | 'O' | 'P' | 'Q' | 'R' | 'S' | 'T' | 'U' | 'V' | 'W' | 'X' | 'Y' | 'Z'
 | 'a' | 'b' | 'c' | 'd' | 'e' | 'f' | 'g' | 'h' | 'i' | 'j' | 'k' | 'l' | 'm' | 'n'
 | 'o' | 'p' | 'q' | 'r' | 's' | 't' | 'u' | 'v' | 'w' | 'x' | 'y' | 'z' ;

WS:[ \r\t\n]+ -> skip ;


COMMENT: '/*' .*? '*/' -> skip;

LINE_COMMENT: '//' ~[\r\n]* -> skip;

//parser

file: ( ( functionDef | constDef | varDecl | ifBlock | classDef | prototypeDef | instanceDef | instanceDecl ) ';' )*?;

functionDef: 'func' typeReference nameNode parameterList statementBlock;

parameterList: '(' ( varDecl (',' varDecl )*? )? ')';

statementBlock: '{' ( ( ( statement ';' ) | ( ifBlock ( ';' )? ) ) )*? '}';

statement: funcCall | assignment | returnStatement | constDef | varDecl | expression;

funcCall: referenceNode '(' ( expression ( ',' expression )*? )? ')';

assignment: complexReference ( '=' | '+=' | '-=' | '*=' | '/=' ) expression;

ifBlock: 'if' expression statementBlock ( 'else' 'if' expression statementBlock )*? ( 'else' statementBlock )?;

returnStatement: 'return' ( expression );

varDecl: 'var' typeReference nameNode ( ('[' simpleValue ']') | (',' nameNode )* )? ;
constDef: 'const' typeReference nameNode ( '[' simpleValue ']' )? constAssignment;
classDef: 'class' nameNode '{' ( varDecl ';' )*? '}';
prototypeDef: 'prototype' nameNode '(' referenceNode ')' '{' ( ( assignment | funcCall ) ';' )*? '}';
instanceDef: 'instance' nameNode '(' referenceNode ')' '{' ( ( assignment | funcCall ) ';' )*? '}';
instanceDecl: 'instance' nameNode ( ',' nameNode )*? '(' referenceNode ')';

constAssignment: '=' ( expression | arrayLiteral );
arrayLiteral: '{' ( expression (',' expression)*? ) '}';

expression: logicAnd ( '||' expression )? ;

logicAnd: bitOr ( '&&' logicAnd )? ;
bitOr: bitAnd ( '|' bitOr )? ;
bitAnd: equal ( '&' bitAnd )? ;
equal: comparison ( ( '==' | '!=' ) equal )? ;
comparison: bitShift ( ( '<' | '>' | '<=' | '>=' ) comparison )? ;
bitShift: add ( ( '<<' | '>>' ) bitShift )? ;
add: mult ( ( '+' | '-' ) add )? ;
mult: unary ( ( '*' | '/' | '%' ) mult )? ;
unary: ( '-' | '!' | '~' | '+' )? value;

simpleValue: IntegerLiteral | referenceNode;
value: IntegerLiteral | FloatLiteral | StringLiteral | 'null' | funcCall | complexReference | ( '(' expression ')' );

complexReference: complexReferenceNode ( '.' complexReferenceNode )?;

complexReferenceNode: referenceNode ( '[' simpleValue ']' )?;
typeReference:  ( referenceNode | 'void' | 'int' | 'float' | 'string' | 'func' | 'instance');
referenceNode: Identifier;

nameNode: Identifier;