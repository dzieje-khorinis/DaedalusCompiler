using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using DaedalusParser;

namespace DaedalusCompilerCsharp
{
    class Program
    {
        static void Main(string[] args)
        {
            String inputPath = @"Parser/Inputs/script.txt";
            ICharStream stream = CharStreams.fromPath(inputPath);
            ITokenSource lexer = new DaedalusParserLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            DaedalusParserParser parser = new DaedalusParserParser(tokens);
            var fileContext = parser.file();
            parser.Context = fileContext;
            parser.BuildParseTree = true;
            IParseTree tree = parser.expression();

            Console.WriteLine($"\n\nParseTree result: {tree.ToStringTree()}");
            Console.ReadKey();
        }
    }
}
