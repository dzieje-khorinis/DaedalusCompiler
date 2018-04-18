using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using DaedalusParser;
using DaedalusCompiler.Dat;
using System.Linq;
using DaedalusCompiler.Compilation;

namespace DaedalusCompiler
{
    class Program
    {
        private const string version = "0.1";
        private const string compiler_name = "daedalus-compiler";

        static void showHelp()
        {
            Console.WriteLine("Daedalus Compiler Version {0}", version);
            Console.WriteLine(
                "usage: {0} file_path [<args>]", compiler_name
            );
            Console.WriteLine(
                "Args description:\n" +
                "--load-dat      loads Gothic DAT file and make analyze of that, in that case file_path should be DAT file"
            );
        }
        
        static void handleOptionsParser(string[] args)
        {
            var loadHelp = false;
            var loadDat = false;

            var p = new NDesk.Options.OptionSet () {
                { "h|?|help",   v => loadHelp = true },
                { "load-dat", v => loadDat = true }
            };

            List<string> extra;
            try {
                extra = p.Parse (args);
            }
            catch (NDesk.Options.OptionException e) {
                Console.WriteLine (e.Message);
                return;
            }

            if ( loadHelp || extra.Count == 0 )
            {
                showHelp();
            }
            else
            {
                var filePath = extra[0];

                if (loadDat)
                {
                    analyzeDATFile(filePath);
                }
                else
                {
                    compileDaedalus(filePath);
                }
            }
            
            return;
        }

        static void analyzeDATFile(string path)
        {
            var dat = new DatFile();
            dat.Load(path);
        }

        static void compileDaedalus(string path)
        {
            var compiler = new Compiler(path);
            compiler.Compile();

            Console.WriteLine($"Compilation completed successfully");

            //ICharStream stream = CharStreams.fromPath(path);
            //ITokenSource lexer = new DaedalusParserLexer(stream);
            //ITokenStream tokens = new CommonTokenStream(lexer);
            //DaedalusParserParser parser = new DaedalusParserParser(tokens);
            //var fileContext = parser.file();
            //parser.Context = fileContext;
            //parser.BuildParseTree = true;
            //IParseTree tree = parser.expression();

            //Console.WriteLine($"\n\nParseTree result: {tree.ToStringTree()}");
        }

        static void Main(string[] args)
        {
            handleOptionsParser(args);
        }
    }
}
