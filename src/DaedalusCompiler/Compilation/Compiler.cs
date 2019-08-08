using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Antlr4.Runtime.Tree;
using DaedalusCompiler.Compilation.SemanticAnalysis;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    public class Compiler
    {

        //private readonly AssemblyBuilder _assemblyBuilder;
        private readonly OutputUnitsBuilder _ouBuilder;
        private readonly string _outputDirPath;
        

        public Compiler(string outputDirPath="output", bool verbose=true, bool strictSyntax=false)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //_assemblyBuilder = new AssemblyBuilder(verbose, strictSyntax);
            _ouBuilder = new OutputUnitsBuilder(verbose);
            _outputDirPath = outputDirPath;
        }

        public static string[] GetWarningCodesToSuppress(string line)
        {
            string ws = @"(?:[ \t])*";
            string newline = @"(?:\r\n?|\n)";
            RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline;
            string suppressWarningsPattern = $@"//!{ws}suppress{ws}:((?:{ws}[a-zA-Z0-9]+)+){ws}{newline}?$";
            MatchCollection matches = Regex.Matches(line, suppressWarningsPattern, options);
            foreach (Match match in matches)
            {
                return match.Groups[1].Value.Split(" ").Where(s => !s.Equals(String.Empty)).ToArray();
            }
            return new string[]{};
        }
        
        public string GetBuiltinsPath()
        {
            string programStartPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            return Path.Combine(Path.GetDirectoryName(programStartPath), "DaedalusBuiltins");
        }

        public bool CompileFromSrc(
            string srcFilePath, 
            bool compileToAssembly,
            bool verbose = true,
            bool generateOutputUnits = true
        )
        {
            var absoluteSrcFilePath = Path.GetFullPath(srcFilePath);

            try
            {
                /*
                Dictionary<string, NodeValue> d = new Dictionary<string, NodeValue>();
                d["null"] = null;
                d["1"] = new IntValue(1);
                d["1.5"] = new FloatValue(1.5);
                d["2"] = new IntValue(2);
                d["2.5"] = new FloatValue(2.5);
                d["tekst"] = new StringValue("tekst");

                NodeValue x = EvaluationHelper.EvaluateBinaryOperation(BinaryOperator.Mult, d["2"], d["tekst"]);
                
                Console.WriteLine($"x: {x}");

                return true;
                */
                
                string[] paths = SrcFileHelper.LoadScriptsFilePaths(absoluteSrcFilePath).ToArray();
                string srcFileName = Path.GetFileNameWithoutExtension(absoluteSrcFilePath).ToLower();
                
                string runtimePath = Path.Combine(GetBuiltinsPath(), srcFileName + ".d");
                List<IParseTree> parseTrees = new List<IParseTree>();

                int externalFilesCount = 0;
                
                if (File.Exists(runtimePath) && false)
                {
                    externalFilesCount++;
                    
                    if (verbose) Console.WriteLine($"[0/{paths.Length}]Parsing runtime: {runtimePath}");

                    DaedalusParser parser = GetParserForScriptsFile(runtimePath);
                    parseTrees.Add(parser.daedalusFile());

                    /*
                    _assemblyBuilder.IsCurrentlyParsingExternals = true;
                    
                    string fileContent = GetFileContent(runtimePath);
                    DaedalusParser parser = GetParserForText(fileContent);

                    _assemblyBuilder.ErrorFileContext.FileContentLines = fileContent.Split(Environment.NewLine);
                    _assemblyBuilder.ErrorFileContext.FilePath = runtimePath;
                    _assemblyBuilder.ErrorFileContext.FileIndex = -1;
                    
                    ParseTreeWalker.Default.Walk(new DaedalusListener(_assemblyBuilder, 0), parser.daedalusFile());
                    _assemblyBuilder.IsCurrentlyParsingExternals = false;
                    */
                }
                else
                {
                    Console.WriteLine($"Runtime {runtimePath} doesn't exist.");
                }

                int syntaxErrorsCount = 0;
                for (int i = 0; i < paths.Length; i++)
                {
                    if (verbose) Console.WriteLine($"[{i + 1}/{paths.Length}]Parsing: {paths[i]}");

                    DaedalusParser parser = GetParserForScriptsFile(paths[i]);
                    SyntaxErrorListener syntaxErrorListener = new SyntaxErrorListener();
                    parser.AddErrorListener(syntaxErrorListener);
                    parseTrees.Add(parser.daedalusFile());

                    syntaxErrorsCount += syntaxErrorListener.ErrorsCount;



                    /*
                    string fileContent = GetFileContent(paths[i]);
                    DaedalusParser parser = GetParserForText(fileContent);
                    //parser.RemoveErrorListeners(); // TODO uncomment this line once SyntaxErrorListener is fully implemented
                    SyntaxErrorListener syntaxErrorListener = new SyntaxErrorListener();
                    parser.AddErrorListener(syntaxErrorListener);

                    _assemblyBuilder.ErrorFileContext.FileContentLines = fileContent.Split(Environment.NewLine);
                    _assemblyBuilder.ErrorFileContext.FilePath = paths[i];
                    _assemblyBuilder.ErrorFileContext.FileIndex = i;
                    _assemblyBuilder.ErrorFileContext.SuppressedWarningCodes = Compiler.GetWarningCodesToSuppress(
                        _assemblyBuilder.ErrorFileContext.FileContentLines[0]
                    );
                    ParseTreeWalker.Default.Walk(new DaedalusListener(_assemblyBuilder, i), parser.daedalusFile());
                    syntaxErrorsCount += syntaxErrorListener.ErrorsCount;
                    
                    if (generateOutputUnits && syntaxErrorListener.ErrorsCount == 0)
                    {
                        _ouBuilder.ParseText(fileContent);
                    }
                    */
                }
                
                if (syntaxErrorsCount > 0)
                {
                    StdErrorLogger logger = new StdErrorLogger();
                    logger.LogLine($"{syntaxErrorsCount} syntax {(syntaxErrorsCount == 1 ? "error" : "errors")} generated.");
                    return false;
                }

                
                Console.WriteLine("parseTrees created");
                SemanticAnalyzer semanticAnalyzer = new SemanticAnalyzer(parseTrees, externalFilesCount);
                
                semanticAnalyzer.CreateSymbolTable();
                
                semanticAnalyzer.EvaluateReferencesAndTypesAndArraySize();
                
                semanticAnalyzer.DetectErrors();
                
                Console.WriteLine(parseTrees.Count);
                
                
                /*
                
                if (syntaxErrorsCount > 0)
                {
                    StdErrorLogger logger = new StdErrorLogger();
                    logger.LogLine($"{syntaxErrorsCount} syntax {(syntaxErrorsCount == 1 ? "error" : "errors")} generated.");
                    return false;
                }

                if (!compileToAssembly)
                {
                    Directory.CreateDirectory(_outputDirPath);
                }

                if (generateOutputUnits)
                {
                    _ouBuilder.SaveOutputUnits(_outputDirPath);
                }
                
                _assemblyBuilder.Finish();

                List<CompilationMessage> errors = new List<CompilationMessage>();
                foreach (CompilationMessage error in _assemblyBuilder.Errors)
                {
                    if (error is CompilationError)
                    {
                        errors.Add(error);
                    }
                    else if (error is CompilationWarning compilationWarning)
                    {
                        if (compilationWarning.IsSuppressed == false)
                        {
                            errors.Add(compilationWarning);
                        }
                    }
                }


                int errorsCount = errors.Count(x => x is CompilationError);
                int warningsCount = errors.Count(x => x is CompilationWarning);
                if (_assemblyBuilder.StrictSyntax)
                {
                    errorsCount += warningsCount;
                    warningsCount = 0;
                }

                if (errors.Any())
                {
                    errors.Sort((x, y) => x.CompareTo(y));

                    bool stopCompilation = _assemblyBuilder.StrictSyntax;

                    string lastErrorFilePath = "";
                    string lastErrorBlockName = null;
                    StdErrorLogger logger = new StdErrorLogger();

                    foreach (CompilationMessage error in errors)
                    {
                        if (error is CompilationError)
                        {
                            stopCompilation = true;
                        }

                        if (lastErrorFilePath != error.FilePath)
                        {
                            lastErrorFilePath = error.FilePath;
                            Console.WriteLine(error.FilePath);
                        }

                        if (lastErrorBlockName != error.ExecBlockName)
                        {
                            lastErrorBlockName = error.ExecBlockName;
                            if (error.ExecBlockName == null)
                            {
                                Console.WriteLine($"{error.FileName}: In global scope:");
                            }
                            else
                            {
                                Console.WriteLine($"{error.FileName}: In {error.ExecBlockType} ‘{error.ExecBlockName}’:");
                            }
                            
                        }

                        error.Print(logger);
                    }

                    if (stopCompilation)
                    {

                        if (errorsCount > 0 || warningsCount > 0)
                        {
                            if (errorsCount > 0)
                            {
                                logger.Log($"{errorsCount} {(errorsCount == 1 ? "error" : "errors")}");
                            }

                            if (warningsCount > 0)
                            {
                                if (errorsCount > 0)
                                {
                                    logger.Log(", ");
                                }
                                logger.Log($"{warningsCount} {(warningsCount == 1 ? "warning" : "warnings")}");
                            }
                            logger.LogLine(" generated.");
                        }
                        return false;
                    }
                }

                if (compileToAssembly)
                {
                    Console.WriteLine(_assemblyBuilder.GetAssembler());
                }
                else
                {
                    Directory.CreateDirectory(_outputDirPath);
                    string datPath = Path.Combine(_outputDirPath, srcFileName + ".dat");
                    _assemblyBuilder.SaveToDat(datPath);
                }
                
                */

                return true;
            }
            catch (Exception exc)
            {
                Console.WriteLine("SRC compilation failed");
                Console.WriteLine($"{exc}");
                return false;
            }
        }

        /*
        public List<DatSymbol> GetSymbols()
        {
            return _assemblyBuilder.GetSymbols();
        }

        public List<BaseExecBlockContext> GetExecBlocks()
        {
            return _assemblyBuilder.GetExecBlocks();
        }
        */

        public void SetCompilationDateTimeText(string compilationDateTimeText)
        {
            _ouBuilder.SetGenerationDateTimeText(compilationDateTimeText);
        }
        
        public void SetCompilationUserName(string userName)
        {
            _ouBuilder.SetGenerationUserName(userName);
        }

        private string GetFileContent(string filePath)
        {
            return File.ReadAllText(filePath, Encoding.GetEncoding(1250));
        }
        
        private DaedalusParser GetParserForText(string input)
        {
            AntlrInputStream inputStream = new AntlrInputStream(input);
            DaedalusLexer lexer = new DaedalusLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            return new DaedalusParser(commonTokenStream);
        }

        private DaedalusParser GetParserForScriptsFile(string scriptFilePath)
        {
            string fileContent = GetFileContent(scriptFilePath);
            return GetParserForText(fileContent);
        }
    }
}