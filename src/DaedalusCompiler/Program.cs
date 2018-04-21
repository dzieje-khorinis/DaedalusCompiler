using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using DaedalusCompiler.Dat;
using System.Linq;
using DaedalusCompiler.Compilation;
using System.Diagnostics;

namespace DaedalusCompiler
{
    class Program
    {
        private const string version = "0.1";
        private const string compiler_name = "daedalus-compiler";

        static void ShowHelp()
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
        
        static void HandleOptionsParser(string[] args)
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
                ShowHelp();
            }
            else
            {
                var filePath = extra[0];

                if (loadDat)
                {
                    AnalyzeDATFile(filePath);
                }
                else
                {
                    CompileDaedalus(filePath);
                }
            }
            
            return;
        }

        static void AnalyzeDATFile(string path)
        {
            var dat = new DatFile();
            dat.Load(path);
        }

        static void CompileDaedalus(string path)
        {
            var compiler = new Compiler();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            compiler.CompileFromSrc(path);

            Console.WriteLine($"Compilation completed successfully. Total time: {stopwatch.Elapsed}");
        }

        static void Main(string[] args)
        {
            HandleOptionsParser(args);
        }
    }
}
