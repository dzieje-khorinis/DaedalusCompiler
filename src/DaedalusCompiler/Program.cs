using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using DaedalusParser;
using DaedalusCompiler.Dat;
using System.Linq;

namespace DaedalusCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            //String inputPath = @"test.d";
            //ICharStream stream = CharStreams.fromPath(inputPath);
            //ITokenSource lexer = new DaedalusParserLexer(stream);
            //ITokenStream tokens = new CommonTokenStream(lexer);
            //DaedalusParserParser parser = new DaedalusParserParser(tokens);
            //var fileContext = parser.file();
            //parser.Context = fileContext;
            //parser.BuildParseTree = true;
            //IParseTree tree = parser.expression();

            //Console.WriteLine($"\n\nParseTree result: {tree.ToStringTree()}");

            //var dat_old = new DatFile_OLD(@"C:\Program Files (x86)\CDP Games\Gothic 2 Gold\_Work\data\Scripts\_compiled\GOTHIC.DAT");

            var dat = new DatFile();
            dat.Load(@"C:\Program Files (x86)\CDP Games\Gothic 2 Gold\_Work\data\Scripts\_compiled\GOTHIC.DAT");

            Console.ReadKey();
        }
    }
}
