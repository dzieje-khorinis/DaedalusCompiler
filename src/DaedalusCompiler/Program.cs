using System;
using System.Collections.Generic;
using DaedalusCompiler.Compilation;
using System.Diagnostics;
using DaedalusCompiler.Compilation.SemanticAnalysis;

namespace DaedalusCompiler
{
    class Program
    {
        private const string Version = "0.7.0";
        private const string AppName = "Daedalus Compiler Version";
        private const string AppSlug = "daedalus-compiler";

        static void ShowHelp()
        {
            Console.WriteLine($"{AppName} {Version}");
            Console.WriteLine($"usage: {AppSlug} file_path [<args>]");
            Console.WriteLine(
                "Args description:\n" +
                "--load-dat          loads Gothic DAT file and analyzes it, in that case file_path should be DAT file\n" +
                "--get-assembly      compile code to readable assembly\n" +
                "--gen-ou            generate output units files (ou.cls and ou.bin)\n" +
                "--strict            use more strict syntax version\n" +
                "--suppress          suppress warnings globally\n" +
                "--version           displays version of compiler\n" +
                "-r|--runtime <path> (optional) custom externals file\n" +
                "-o|--output <path>  (optional) output .DAT file\n" +
                "--verbose"
            );
        }
        
        static void HandleOptionsParser(string[] args)
        {
            var loadHelp = false;
            var generateOutputUnits = false;
            var verbose = false;
            var strict = false;
            var getVersion = false;
            bool suppressModeOn = false;
            bool detectUnused = false;
            bool caseSensitiveCode = false;
            string filePath = String.Empty;
            string runtimePath = String.Empty;
            string outputPath = String.Empty;
            HashSet<string> suppressCodes = new HashSet<string>();

            var optionSet = new NDesk.Options.OptionSet () {
                { "h|?|help",   v => loadHelp = true },
                { "gen-ou", v => generateOutputUnits = true },
                { "verbose", v => verbose = true },
                { "strict", v => strict = true },
                { "detect-unused", v => detectUnused = true },
                { "case-sensitive-code", v => caseSensitiveCode = true },
                { "suppress", v => suppressModeOn = true },
                { "version|v", v => getVersion = true  },
                { "r|runtime=", v => runtimePath = v},
                { "o|output=", v => outputPath = v},
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
            
            try {
                optionSet.Parse (args);
            }
            catch (NDesk.Options.OptionException e) {
                Console.WriteLine (e.Message);
                return;
            }
            
            if (!caseSensitiveCode)
            {
                suppressCodes.Add(NamesNotMatchingCaseWiseWarning.WCode);
            }

            
            if (!detectUnused)
            {
                suppressCodes.Add(UnusedSymbolWarning.WCode);
            }

            if (getVersion)
            {
                Console.WriteLine($"v{Version}");
                return;
            }

            if ( loadHelp || filePath == String.Empty )
            {
                ShowHelp();
            }
            else
            {
                CompileDaedalus(filePath, runtimePath, outputPath, verbose, generateOutputUnits, strict, suppressCodes);
            }
        }
        
        static void CompileDaedalus(string path, string runtimePath, string outputPath, bool verbose, bool generateOutputUnits, bool strictSyntax, HashSet<string> suppressCodes)
        {
            var compiler = new Compiler("output", verbose, strictSyntax, suppressCodes);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            bool compiledSuccessfully = compiler.CompileFromSrc(path, runtimePath, outputPath, verbose, generateOutputUnits);
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
