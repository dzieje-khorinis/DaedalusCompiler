using System.Linq;
using System.IO;
using System;
using System.Collections.Generic;
using System.Reflection;
using DaedalusCompiler.Compilation;
using System.Diagnostics;
using Common.SemanticAnalysis;
using System.CommandLine;
using System.CommandLine.Invocation;


namespace DaedalusCompiler
{
    static class Program
    {
        private static readonly string Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "unknown";
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
            // Create argument for the source file path
            var srcFileArgument = new Argument<string?>(
                name: "srcFile",
                description: "Path to the .src file to compile"
            );

            // Create options
            var runtimeOption = new Option<string?>(
                aliases: new[] { "-r", "--runtime" },
                description: "Daedalus externals path (default: g2nk builtins dependant on .src file name)"
            );

            var outputDatOption = new Option<string?>(
                aliases: new[] { "-o", "--output-dat" },
                description: ".DAT file path (default: \"output\" dir in working directory)"
            );

            var genOuOption = new Option<bool>(
                aliases: new[] { "-g", "--gen-ou" },
                description: "Generate extra output units files (ou.cls and ou.bin)"
            );

            var outputOuOption = new Option<string>(
                aliases: new[] { "-u", "--output-ou" },
                description: ".ou files directory path (used only if --gen-ou flag is provided)",
                getDefaultValue: () => "output"
            );

            var strictOption = new Option<bool>(
                aliases: new[] { "-x", "--strict" },
                description: "Use more strict syntax version (warnings become errors)"
            );

            var caseSensitiveOption = new Option<bool>(
                aliases: new[] { "-c", "--case-sensitive-code" },
                description: "Symbol usage must match definition case-sensitive"
            );

            var suppressOption = new Option<string?>(
                aliases: new[] { "-s", "--suppress" },
                description: "Colon separated warning codes, to suppress warnings globally"
            );

            var detectUnusedOption = new Option<bool>(
                aliases: new[] { "-d", "--detect-unused" },
                description: "Unused symbols generate warnings"
            );

            var zenPathsOption = new Option<string?>(
                aliases: new[] { "-z", "--zen-paths" },
                description: "ASCII Zens paths, auto enables --detect-unused flag, wildcard * supported in file name"
            );

            var verboseOption = new Option<bool>(
                aliases: new[] { "-v", "--verbose" },
                description: "Enable verbose output"
            );

            // Create root command (System.CommandLine automatically provides --version)
            var rootCommand = new RootCommand($"{AppName} {Version}")
            {
                srcFileArgument,
                runtimeOption,
                outputDatOption,
                genOuOption,
                outputOuOption,
                strictOption,
                caseSensitiveOption,
                suppressOption,
                detectUnusedOption,
                zenPathsOption,
                verboseOption
            };

            // Set the handler using the modern System.CommandLine approach
            rootCommand.SetHandler((InvocationContext context) =>
            {
                var srcFile = context.ParseResult.GetValueForArgument(srcFileArgument);
                var runtime = context.ParseResult.GetValueForOption(runtimeOption);
                var outputDat = context.ParseResult.GetValueForOption(outputDatOption);
                var genOu = context.ParseResult.GetValueForOption(genOuOption);
                var outputOu = context.ParseResult.GetValueForOption(outputOuOption);
                var strict = context.ParseResult.GetValueForOption(strictOption);
                var caseSensitive = context.ParseResult.GetValueForOption(caseSensitiveOption);
                var suppress = context.ParseResult.GetValueForOption(suppressOption);
                var detectUnused = context.ParseResult.GetValueForOption(detectUnusedOption);
                var zenPaths = context.ParseResult.GetValueForOption(zenPathsOption);
                var verbose = context.ParseResult.GetValueForOption(verboseOption);

                if (string.IsNullOrEmpty(srcFile))
                {
                    ShowHelp();
                    return;
                }

                // Process zen paths
                List<string> zenPathsList = new List<string>();
                if (!string.IsNullOrEmpty(zenPaths))
                {
                    zenPathsList = zenPaths.Split(':').ToList();
                    detectUnused = true; // Auto enable detect-unused when zen paths are provided
                }

                // Process suppress codes
                HashSet<string> suppressCodes = new HashSet<string>();
                if (!string.IsNullOrEmpty(suppress))
                {
                    suppressCodes = suppress.Split(':').ToHashSet();
                }

                // Add default suppress codes based on options
                if (!caseSensitive)
                {
                    suppressCodes.Add(NamesNotMatchingCaseWiseWarning.WCode);
                }

                if (!detectUnused)
                {
                    suppressCodes.Add(UnusedSymbolWarning.WCode);
                }

                // Set default output path if not provided
                string finalOutputDat = outputDat ?? "";
                if (string.IsNullOrEmpty(finalOutputDat))
                {
                    string srcFileName = Path.GetFileNameWithoutExtension(srcFile).ToLower();
                    finalOutputDat = Path.Combine("output", srcFileName + ".dat");
                }

                // Compile
                CompileDaedalus(zenPathsList, srcFile, runtime ?? "", finalOutputDat, outputOu, 
                    verbose, genOu, strict, suppressCodes);
            });

            // Parse and invoke
            rootCommand.Invoke(args);
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
