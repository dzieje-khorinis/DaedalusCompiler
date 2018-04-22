using Antlr4.Runtime;
using DaedalusCompiler.Dat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Antlr4.Runtime.Tree;

namespace DaedalusCompiler.Compilation
{
    public class Compiler
    {
        public void CompileFromSrc(string srcFilePath)
        {
            try
            {
                var paths = SrcFileHelper.LoadScriptsFilePaths(srcFilePath).ToArray();
                var assemblyBuilder = new AssemblyBuilder();

                for (int i = 0; i < paths.Length; i++)
                {
                    Console.WriteLine($"[{i + 1}/{paths.Length}]Compiling: {paths[i]}");

                    // create parser for specific file
                    var parser = GetParser(paths[i]);

                    ParseTreeWalker.Default.Walk(new DaedalusParserListener(assemblyBuilder, i), parser.daedalusFile());
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("SRC compilation failed");
                Console.WriteLine($"{exc.ToString()}");
            }
        }

        private DaedalusParser GetParser(string scriptFilePath)
        {
            AntlrFileStream inputStream = new AntlrFileStream(scriptFilePath);
            DaedalusLexer lexer = new DaedalusLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            return new DaedalusParser(commonTokenStream);
        }
    }
}
