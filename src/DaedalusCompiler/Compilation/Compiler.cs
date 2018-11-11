using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Antlr4.Runtime.Tree;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    public class Compiler
    {
        private AssemblyBuilder _assemblyBuilder;
        
        public string GetBuiltinsPath()
        {
            string programStartPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            return Path.Combine(Path.GetDirectoryName(programStartPath), "DaedalusBuiltins");
        }
        
        public void CompileFromSrc(
            string srcFilePath, 
            bool compileToAssembly,
            bool verbose = true,
            bool saveDatToFile = true
            )
        {
            try
            {              
                _assemblyBuilder = new AssemblyBuilder(verbose);
                string[] paths = SrcFileHelper.LoadScriptsFilePaths(srcFilePath).ToArray();
                string srcFileName = Path.GetFileNameWithoutExtension(srcFilePath).ToLower();
                
                string runtimePath = Path.Combine(GetBuiltinsPath(), srcFileName + ".d");
                if (File.Exists(runtimePath))
                {
                    _assemblyBuilder.IsCurrentlyParsingExternals = true;
                    if (verbose) Console.WriteLine($"[0/{paths.Length}]Compiling runtime: {runtimePath}");
                    var parser = GetParser(runtimePath);
                    ParseTreeWalker.Default.Walk(new DaedalusParserListener(_assemblyBuilder, 0), parser.daedalusFile());
                    _assemblyBuilder.IsCurrentlyParsingExternals = false;
                }
                
                
                for (int i = 0; i < paths.Length; i++)
                {
                    if (verbose) Console.WriteLine($"[{i + 1}/{paths.Length}]Compiling: {paths[i]}");

                    // create parser for specific file
                    var parser = GetParser(paths[i]);

                    ParseTreeWalker.Default.Walk(new DaedalusParserListener(_assemblyBuilder, i), parser.daedalusFile());
                }
    
                _assemblyBuilder.Finish();
                if (compileToAssembly)
                {
                    Console.WriteLine(_assemblyBuilder.GetAssembler());
                }
                else if (saveDatToFile)
                {
                    _assemblyBuilder.SaveToDat(srcFileName + ".dat");
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("SRC compilation failed");
                Console.WriteLine($"{exc}");
            }
        }

        public List<DatSymbol> GetSymbols()
        {
            return _assemblyBuilder.GetSymbols();
        }

        public List<BaseExecBlockContext> GetExecBlocks()
        {
            return _assemblyBuilder.GetExecBlocks();
        }

        private DaedalusParser GetParser(string scriptFilePath)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            AntlrFileStream inputStream = new AntlrFileStream(scriptFilePath, Encoding.GetEncoding(1250));
            DaedalusLexer lexer = new DaedalusLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            return new DaedalusParser(commonTokenStream);
        }
    }
}