using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using DaedalusCompiler.Compilation;

namespace DaedalusCompiler.Tests
{
    public static class Utils
    {
        public static void WalkSourceCode(string code, AssemblyBuilder assemblyBuilder)
        {
            var inputStream = new AntlrInputStream(code);
            var lexer = new DaedalusLexer(inputStream);
            var commonTokenStream = new CommonTokenStream(lexer);
            var parser = new DaedalusParser(commonTokenStream);

            ParseTreeWalker.Default.Walk(new DaedalusParserListener(assemblyBuilder, 0), parser.daedalusFile());
        }
    }
}