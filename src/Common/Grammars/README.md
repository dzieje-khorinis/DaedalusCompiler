Generate Daedalus Lexer, Parser and Visitor with the following command:
```sh
antlr4 -Dlanguage=CSharp -encoding utf8 -o Output -visitor -no-listener Daedalus.g4
```

Generate Zen Lexer, Parser and Visitor with the following command:
```sh
antlr4 -Dlanguage=CSharp -encoding utf8 -o Output -visitor -no-listener Zen.g4
```

When it comes to ZenParser, as of now, only ASCII Zens are supported.
In the future we could also add support for BINARY and BIN_SAFE Zens.

Parsers for these are already written in Cpp:
https://github.com/ataulien/ZenLib/tree/master/zenload 

So we could either utilize ZenLib or rewrite Zen parsers in C#.