using Antlr4.Runtime;
using DaedalusCompiler.Dat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DaedalusCompiler.Compilation
{
    public class Compiler
    {
        public void CompileFromSrc(string srcFilePath)
        {
            try
            {
                var paths = (SrcFileHelper.LoadScriptsFilePaths(srcFilePath));

                foreach (var scriptPath in paths)
                {
                    Console.WriteLine($"Compiling: {scriptPath}");

                    var parser = GetParser(scriptPath);

                    var context = parser.daedalusFile();
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

        private List<DatSymbol> LoadSymbols(DaedalusParser parser)
        {
            throw new NotImplementedException();
        }
    }
}
