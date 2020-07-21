using System.Linq;
using System.IO;
using System;
using System.Collections.Generic;
using DaedalusCompiler.Compilation;
using System.Diagnostics;
using Common.SemanticAnalysis;


namespace DaedalusCompiler
{
    static class Program
    {
        private const string Version = "0.9.1";
        private const string AppName = "Daedalus Compiler";
        private const string AppSlug = "daedalus-compiler";

        static void ShowHelp()
        {
            Console.WriteLine($"{AppName} {Version}");
            Console.WriteLine($"usage: {AppSlug} SRC_FILE_PATH [<optional args>]");
            Console.WriteLine(
                "Optional args description:\n" +
                "-r|--runtime FILE_PATH         daedalus externals path (default: g2nk builtins dependant on .src file name)\n" +
                "-o|--output-dat FILE_PATH      .DAT file path(default: \"output\" dir in working directory)\n\n" +
                
                "-g|--gen-ou                    generate extra output units files (ou.cls and ou.bin)\n" +
                "-u|--output-ou DIR_PATH        .ou files directory path (used only if --gen-ou flag is provided)\n\n" +
                
                "-x|--strict                    use more strict syntax version (warnings become errors)\n" +
                "-c|--case-sensitive-code       symbol usage must match definition case-sensitive\n" +
                "-s|--suppress WCODE:[WCODE...] colon separated warning codes, to suppress warnings globally\n\n" +

                "-d|--detect-unused             unused symbols generate warnings\n" +
                "-z|--zen-paths PATH:[PATH...]  ASCII Zens paths, auto enables --detect-unused flag, wildcard * supported in file name\n\n" +
                
                "--version                      displays version of compiler\n" +
                "-v|--verbose\n\n\n" +

                "Usage:\n\n" +

                "In examples below, \"gdc\" command ([g]othic [d]aedalus [c]ompiler) is alias to run this compiler.\n" +
                "How can I create this alias? For example, on Linux/MacOS:\n\n" +
                "If you want to run code directly from cloned repository: \n" + 
                "   $ alias gdc='dotnet run --project /path/to/DaedalusCompiler.csproj --' \n\n" +
                "If you want to run last release: \n" + 
                "   $ alias gdc='dotnet /path/to/DaedalusCompiler/DaedalusCompiler.dll' \n\n" +
                "If you want to run last release (docker): \n" + 
                "   $ alias gdc='docker run -v \"$(pwd)\":/usr/workspace dziejekhorinis/daedalus-compiler' \n\n\n" +

                "Examples:\n\n" +

                "generate Gothic.dat file from Gothic.src file in output directory:\n" +
                "   $ gdc /path/to/Gothic.src\n\n" +

                "generate result.dat file from Gothic.src file in custom directory, using custom runtime:\n" +
                "   $ gdc /path/to/Gothic.src --runtime /path/to/runtime.d --output-dat /path/to/result.dat\n\n" +

                "generate ou.csl, ou.bin and Gothic.dat in output directory, ignore warnings W1 and W2:\n" +
                "   $ gdc /path/to/Gothic.src --gen-ou --suppress W1:W2\n\n" +

                "generate Gothic.dat in 'Scripts/_compiled', ou.csl and ou.bin in 'Scripts/Content/Cutscene':\n" +
                "   $ gdc /path/to/Gothic.src --output-dat \"Scripts/_compiled/Gothic.dat\" --gen-ou --output-ou \"Scripts/Content/Cutscene\"\n\n" +

                "generate Gothic.dat, enable unused symbol detection, provide zen paths to make that detection more accurate':\n" +
                "   $ gdc /path/to/Gothic.src --zen-paths=\"/path/to/zens/*.zen\" \n"
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

            var optionSet = new NDesk.Options.OptionSet()
            {
                {"h|?|help", v => loadHelp = true},

                {"r|runtime=", v => runtimePath = v},
                {"o|output-dat=", v => outputPathDat = v},

                {"g|gen-ou", v => generateOutputUnits = true},
                {"u|output-ou=", v => outputPathOuDir = v},

                {"x|strict", v => strict = true},
                {"i|case-sensitive-code", v => caseSensitiveCode = true},
                {"s|suppress=", v => suppressCodes = v.Split(':').ToHashSet()},

                {"d|detect-unused", v => detectUnused = true},
                {"z|zen-paths=", v => zenPaths = v.Split(':').ToList()},

                {"version", v => getVersion = true},
                {"v|verbose", v => verbose = true},
                {
                    "<>", v =>
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

            try
            {
                optionSet.Parse(args);
            }
            catch (NDesk.Options.OptionException e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            if (zenPaths.Count > 0)
            {
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

            if (loadHelp || srcFilePath == String.Empty)
            {
                ShowHelp();
            }
            else
            {
                CompileDaedalus(zenPaths, srcFilePath, runtimePath, outputPathDat, outputPathOuDir, verbose,
                    generateOutputUnits, strict, suppressCodes);
            }
        }

        static void CompileDaedalus(List<string> zenPaths, string srcFilePath, string runtimePath, string outputPathDat,
            string outputPathOuDir, bool verbose, bool generateOutputUnits, bool strictSyntax,
            HashSet<string> suppressCodes)
        {
            bool compiledSuccessfully = false;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                CreateDirectory(outputPathOuDir);
                CreateDirectory(outputPathDat, isFilePath: true);
                CompilationOptions compilationOptions = new CompilationOptions
                {
                    SrcFilePath = srcFilePath,
                    RuntimePath = runtimePath,
                    OutputPathDat = outputPathDat,
                    GenerateOutputUnits = generateOutputUnits,
                    OutputPathOuDir = outputPathOuDir,
                    ZenPaths = zenPaths,
                    StrictSyntax = strictSyntax,
                    GloballySuppressedCodes = suppressCodes,
                    Verbose = verbose
                };
                Compiler compiler = new Compiler(compilationOptions);
                compiledSuccessfully = compiler.Compile();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

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

        static void CreateDirectory(string directoryPath, bool isFilePath = false)
        {
            if (directoryPath == String.Empty)
            {
                return;
            }

            if (isFilePath)
            {
                directoryPath = Path.GetDirectoryName(directoryPath);
            }

            try
            {
                Directory.CreateDirectory(directoryPath);
            }
            catch (Exception ex)
            {
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
