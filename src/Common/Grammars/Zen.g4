grammar Zen;

/*
TODO think about making separate Lexer and Parser grammars.
That approach allows usage of Lexer MODES, which could be
used for extra validation and generation of specialized
attribute value tokens (separate token types for each data
type, without risk of too greedy string token type).
*/

// Lexer rules
/*
Note:
There may be binary data inside ASCII Zen (in [MeshAndBsp % 0 0] section).
It does cause problems in some languages when trying to open file.
For example in Python, you won't be able to open file in text mode.
Solution is to open file in binary mode, read bytes, remove MeshAndBsp
section, convert data to text and pass it to ANTLR4 Lexer.
In C# There is no need to do that and everything works as is.
*/
MeshAndBsp: '[MeshAndBsp % 0 0]' .*? '[]\n\t' -> skip;
Whitespace: [ \t]+ -> skip;
Newline: ('\r''\n'?| '\n') -> skip;

Int: '-'? Digit+;
Date: Digit Digit? '.' Digit Digit? '.' Digit Digit? Digit? Digit?;
Time: Digit Digit? ':' Digit Digit? ':' Digit Digit?;
Value: Type ':' (~[\n])*;
Name: FirstChar NextChar*;

// https://en.wikipedia.org/wiki/Unicode_block
fragment Latin1Supplement: [\u0080-\u00FF];
fragment LatinExtendedA: [\u0100-\u017F]; 
fragment LatinExtendedB: [\u0180-\u024F];
fragment Cyrillic: [\u0400-\u04FF];
fragment CyrillicSupplement: [\u0500-\u052F];
fragment Letter:
    [a-zA-Z]
    | Latin1Supplement
    | LatinExtendedA
    | LatinExtendedB
    | Cyrillic
    | CyrillicSupplement
;

fragment Digit: [0-9];
fragment FirstChar: Letter | '_';
fragment NextChar: FirstChar | Digit | [-\\.];
fragment Type: 'bool' | 'color' | 'enum' | 'float' | 'int' | 'raw' | 'rawFloat' | 'string' |  'vec3';


// Parser rules
main: head (body=block) EOF;
head:
    Name Name
    Name (version=Int)
    Name
    (zenType=Name)  // supported: ASCII, unsupported: BIN_SAFE, BINARY
    Name (saveGame=Int)
    Name (date=Date) (time=Time)
    Name (user=Name)
    Name
    Name (objectsCount=Int)
    Name
;

block:
    '[' blockName classPath (leftIndex=Int) (rightIndex=Int) ']'
        (block | attr)*
    '[]'
;

blockName: Name | '%';
classPath: Name (':' Name)* | '§' | '%';
attr: Name '=' Value;