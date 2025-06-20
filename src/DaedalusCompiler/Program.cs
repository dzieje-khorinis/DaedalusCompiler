using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Common.SemanticAnalysis;
using DaedalusCompiler.Compilation;
using DaedalusCompiler.Resources;
using NDesk.Options;

namespace DaedalusCompiler
{
    static class Program
    {
        private const string Version = "0.9.2";
        private const string AppName = "Daedalus Compiler";
        private const string AppSlug = "daedalus-compiler";

        private static void ShowHelpAndExit()
        {
            Console.WriteLine($"{AppName} {Version}");
            Console.WriteLine($"usage: {AppSlug} SRC_FILE_PATH [<optional args>]");
            Console.WriteLine(ResourcesHelper.Get(ResourceType.HelpTxt));
            Environment.Exit(0);
        }

        static void HandleOptionsParser(string[] args)
        {
            CompilationOptions options = new CompilationOptions();


            var optionSet = new OptionSet()
            {
                { "h|?|help", _ => ShowHelpAndExit() },

                { "ie|input-encoding=", v => options.SrcEncoding = v },

                { "o|output-dat=", v => options.OutputPathDat = v },
                { "oe|output-encoding=", v => options.DstEncoding = v },

                { "g|gen-ou", _ => options.GenerateOutputUnits = true },
                { "u|output-ou=", v => options.OutputPathOuDir = v },

                {
                    "i|case-sensitive-code",
                    _ => options.GloballySuppressedCodes.Remove(NamesNotMatchingCaseWiseWarning.WCode)
                },
                {
                    "s|suppress=", v => v.Split(':').ToList().ForEach(code => options.GloballySuppressedCodes.Add(code))
                },

                { "d|detect-unused", _ => options.GloballySuppressedCodes.Remove(UnusedSymbolWarning.WCode) },
                {
                    "z|zen-paths=", v =>
                    {
                        options.ZenPaths = v.Split(':').ToList();
                        options.GloballySuppressedCodes.Remove(UnusedSymbolWarning.WCode);
                    }
                },

                { "skip-types", _ => options.SkipBuiltinTypes = true },
                { "x|strict", _ => options.StrictSyntax = true },
                {
                    "version", _ =>
                    {
                        Console.WriteLine($"v{Version}");
                        Environment.Exit(1);
                    }
                },
                { "v|verbose", _ => options.Verbose = true },
                {
                    "<>", v =>
                    {
                        if (options.SrcFilePath == string.Empty)
                        {
                            options.SrcFilePath = v;
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
            catch (OptionException e)
            {
                ExitFailed(e.Message);
            }


            if (string.IsNullOrEmpty(options.SrcFilePath))
            {
                ShowHelpAndExit();
            }


            if (options.OutputPathDat == string.Empty)
            {
                var srcFileName = Path.GetFileNameWithoutExtension(options.OutputPathDat).ToLower();
                options.OutputPathDat = Path.Combine("output", srcFileName + ".dat");
            }

            CompileDaedalus(options);
        }

        private static void CompileDaedalus(CompilationOptions compilationOptions)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                CreateDirectory(compilationOptions.OutputPathOuDir);
                CreateDirectory(compilationOptions.OutputPathDat, isFilePath: true);
                var compiler = new Compiler(compilationOptions);
                PrintCompilationResult(compiler.Compile(), stopwatch.Elapsed);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                PrintCompilationResult(false, stopwatch.Elapsed);
            }
        }

        static void PrintCompilationResult(bool compilationResult, TimeSpan timeElapsed)
        {
            if (compilationResult)
            {
                Console.WriteLine($"Compilation completed successfully. Total time: {timeElapsed}");
                Environment.Exit(0);
            }
            else
            {
                ExitFailed($"Compilation FAILED. Total time: {timeElapsed}");
            }
        }

        static void ExitFailed(string message)
        {
            Console.WriteLine(message);
            Environment.Exit(1);
        }

        static void CreateDirectory(string directoryPath, bool isFilePath = false)
        {
            if (directoryPath == string.Empty)
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
                ExitFailed($"ERROR: {ex.Message}");
            }
        }

        static void Main(string[] args)
        {
            HandleOptionsParser(args);
        }
    }
}