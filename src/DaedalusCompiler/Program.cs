using System;
using System.Collections.Generic;
using DaedalusCompiler.Dat;
using DaedalusCompiler.Compilation;
using System.Diagnostics;
using System.IO;
using DaedalusCompiler.Compilation.SemanticAnalysis;

namespace DaedalusCompiler
{
    class Program
    {
        private const string version = "0.7.0";
        private const string compiler_name = "daedalus-compiler";

        static void ShowHelp()
        {
            Console.WriteLine("Daedalus Compiler Version {0}", version);
            Console.WriteLine(
                "usage: {0} file_path [<args>]", compiler_name
            );
            Console.WriteLine(
                "Args description:\n" +
                "--load-dat          loads Gothic DAT file and analyzes it, in that case file_path should be DAT file\n" +
                "--get-assembly      compile code to readable assembly\n" +
                "--gen-ou            generate output units files (ou.cls and ou.bin)\n" +
                "--strict            use more strict syntax version\n" +
                "--suppress          suppress warnings globally\n" +
                "--version           displays version of compiler\n" +
                "--verbose"
            );
        }
        
        static void HandleOptionsParser(string[] args)
        {
            var loadHelp = false;
            var loadDat = false;
            var compileToAssembly = false;
            var generateOutputUnits = false;
            var verbose = false;
            var strict = false;
            var getVersion = false;
            bool suppressModeOn = false;
            bool detectUnused = false;
            string filePath = String.Empty;
            HashSet<string> suppressCodes = new HashSet<string>();

            var optionSet = new NDesk.Options.OptionSet () {
                { "h|?|help",   v => loadHelp = true },
                { "load-dat", v => loadDat = true },
                { "get-assembly", v => compileToAssembly = true },
                { "gen-ou", v => generateOutputUnits = true },
                { "verbose", v => verbose = true },
                { "strict", v => strict = true },
                { "detect-unused", v => detectUnused = true },
                { "suppress", v => suppressModeOn = true },
                { "version|v", v => getVersion = true  },
                { "<>", v =>
                    {
                        if (suppressModeOn)
                        {
                            suppressCodes.Add(v);
                        }
                        else
                        {
                            filePath = v;
                        }
                    }
                },
            };

            if (!detectUnused)
            {
                suppressCodes.Add(UnusedSymbolWarning.WCode);
            }
            
            try {
                optionSet.Parse (args);
            }
            catch (NDesk.Options.OptionException e) {
                Console.WriteLine (e.Message);
                return;
            }

            if (getVersion)
            {
                Console.WriteLine($"v{version}");
                return;
            }

            if ( loadHelp || filePath == String.Empty )
            {
                ShowHelp();
            }
            else
            {
                if (loadDat)
                {
                    AnalyzeDATFile(filePath);
                }
                else
                {
                    CompileDaedalus(filePath, compileToAssembly, verbose, generateOutputUnits, strict, suppressCodes);
                }
            }
        }

        static void AnalyzeDATFile(string path)
        {
            var dat = new DatFile();
            dat.Load(path);

            //TODO: Move save to compilation process
            var fileName = Path.GetFileName(path);
            fileName = Path.ChangeExtension(fileName, "DAT");
            dat.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName));
        }

        static void CompileDaedalus(string path, bool compileToAssembly, bool verbose, bool generateOutputUnits, bool strictSyntax, HashSet<string> suppressCodes)
        {
            var compiler = new Compiler("output", verbose, strictSyntax, suppressCodes);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            bool compiledSuccessfully = compiler.CompileFromSrc(path, compileToAssembly, verbose, generateOutputUnits);
            if (compiledSuccessfully)
            {
                Console.WriteLine($"Compilation completed successfully. Total time: {stopwatch.Elapsed}");
            }
            else
            {
                Console.WriteLine($"Compilation FAILED. Total time: {stopwatch.Elapsed}");
                Environment.Exit(1);
            }
        }

        static void Main(string[] args)
        {
            HandleOptionsParser(args);
        }
    }
}
