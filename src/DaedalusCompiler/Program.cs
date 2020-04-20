﻿using System.Linq;
using System.IO;
using System;
using System.Collections.Generic;
using DaedalusCompiler.Compilation;
using System.Diagnostics;
using Common.SemanticAnalysis;


namespace DaedalusCompiler
{
    class Program
    {
        private const string Version = "0.8.0";
        private const string AppName = "Daedalus Compiler Version";
        private const string AppSlug = "daedalus-compiler";

        static void ShowHelp()
        {
            Console.WriteLine($"{AppName} {Version}");
            Console.WriteLine($"usage: {AppSlug} file_path [<optional args>]");
            Console.WriteLine(
                "Optional args description:\n" +
                "-r|--runtime FILE_PATH         daedalus externals path (default: g2nk builtins dependant on .src file name)\n" +
                "-o|--output-dat FILE_PATH      .DAT file path(default: \"output\" dir in working directory)\n\n" +
                
                "-g|--gen-ou                    generate extra output units files (ou.cls and ou.bin)\n" +
                "-u|--output-ou DIR_PATH        .ou files directory path (used only if --gen-ou flag is provided)\n\n" +
                
                "-x|--strict                    use more strict syntax version (warnings become errors)\n" +
                "-i|--case-sensitive-code       symbol usage must match definition case-sensitive\n" +
                "-s|--suppress WCODE:[WCODE...] colon separated warning codes, to suppress warnings globally\n\n" +

                "-d|--detect-unused             unused symbols generate warnings\n" +
                "-z|--zen-paths PATH:[PATH...]  ASCII Zens paths, auto enables --detect-unused flag, wildcard * supported in file name\n\n" +
                
                "--version                      displays version of compiler\n" +
                "-v|--verbose\n\n\n" +

                "Example usage:\n\n" +
                
                "generate Gothic.dat file from Gothic.src file in output directory:\n" +
                "   $ dotnet run --project DaedalusCompiler.csproj /path/to/Gothic.src\n\n" +

                "generate Result.dat file from Gothic.src file in custom directory, using custom runtime:\n" +
                "   $ dotnet run --project DaedalusCompiler.csproj /path/to/Gothic.src -- \n" +
                "     --runtime /path/to/runtime.d --output-dat /path/to/result.dat\n\n" +

                "generate ou.csl, ou.bin and Gothic.dat in output directory, ignore warnings W1 and W2:\n" +
                "   $ dotnet run --project DaedalusCompiler.csproj /path/to/Gothic.src --\n" +
                "     --gen-ou --suppress W1:W2\n\n" +

                "generate Gothic.dat in 'Scripts/_compiled', ou.csl and ou.bin in 'Scripts/Content/Cutscene':\n" +
                "   $ dotnet run --project DaedalusCompiler.csproj /path/to/Gothic.src --\n" +
                "     --output-dat \"Scripts/_compiled/Gothic.dat\" --gen-ou --output-ou \"Scripts/Content/Cutscene\"\n"
            );
        }
        
        static void HandleOptionsParser(string[] args)
        {
            var loadHelp = false;
            var generateOutputUnits = false;
            var verbose = false;
            var strict = false;
            var getVersion = false;
            bool detectUnused = false;
            bool caseSensitiveCode = false;
            string srcFilePath = String.Empty;
            string runtimePath = String.Empty;
            string outputPathDat = String.Empty;
            string outputPathOuDir = "output";
            List<string> zenPaths = new List<string>();

            HashSet<string> suppressCodes = new HashSet<string>();

            var optionSet = new NDesk.Options.OptionSet () {
                { "h|?|help",   v => loadHelp = true },

                { "r|runtime=", v => runtimePath = v},
                { "o|output-dat=", v => outputPathDat = v},
                
                { "g|gen-ou", v => generateOutputUnits = true },
                { "u|output-ou=", v => outputPathOuDir = v},
               
                { "x|strict", v => strict = true },
                { "i|case-sensitive-code", v => caseSensitiveCode = true },
                { "s|suppress=", v => suppressCodes = v.Split(':').ToHashSet() },

                { "d|detect-unused", v => detectUnused = true },
                { "z|zen-paths=", v => zenPaths = v.Split(':').ToList() },

                { "version", v => getVersion = true  },
                { "v|verbose", v => verbose = true },
                { "<>", v =>
                    {
                        if (srcFilePath == String.Empty)
                        {
                            srcFilePath = v;
                        }
                        else
                        {
                            Console.WriteLine($"Invalid positional argument: '{v}'");
                            Environment.Exit(1);
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

            if (zenPaths.Count > 0) {
                detectUnused = true;
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

            if (outputPathDat == String.Empty)
            {
                string srcFileName = Path.GetFileNameWithoutExtension(srcFilePath).ToLower();
                outputPathDat = Path.Combine("output", srcFileName + ".dat");
            }

            if ( loadHelp || srcFilePath == String.Empty )
            {
                ShowHelp();
            }
            else
            {
                CompileDaedalus(zenPaths, srcFilePath, runtimePath, outputPathDat, outputPathOuDir, verbose, generateOutputUnits, strict, suppressCodes);
            }
        }
        
        static void CompileDaedalus(List<string> zenPaths, string srcFilePath, string runtimePath, string outputPathDat, string outputPathOuDir, bool verbose, bool generateOutputUnits, bool strictSyntax, HashSet<string> suppressCodes)
        {
            CreateDirectory(outputPathOuDir);
            CreateDirectory(outputPathDat, isFilePath: true);

            var compiler = new Compiler(outputPathOuDir, verbose, strictSyntax, suppressCodes);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            bool compiledSuccessfully = compiler.CompileFromSrc(zenPaths, srcFilePath, runtimePath, outputPathDat, verbose, generateOutputUnits);
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

        static void CreateDirectory(string directoryPath, bool isFilePath=false) {
            if (directoryPath == String.Empty) {
                return;
            }
            if (isFilePath) {
                directoryPath = Path.GetDirectoryName(directoryPath);
            }
            try {
                Directory.CreateDirectory(directoryPath);
            } catch (Exception ex) {
                Console.WriteLine($"ERROR: {ex.Message}");
                Environment.Exit(1);
            }
        }

        static void Main(string[] args)
        {
            HandleOptionsParser(args);
        }
    }
}
