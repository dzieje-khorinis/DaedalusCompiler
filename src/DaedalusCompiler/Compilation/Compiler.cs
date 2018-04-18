using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DaedalusCompiler.Compilation
{
    public class Compiler
    {
        public string SrcFilePath { get; }

        public List<string> SourceFilePaths { get; }

        public Compiler(string srcFilePath)
        {
            SrcFilePath = srcFilePath;
            SourceFilePaths = new List<string>();
        }

        public void Compile()
        {
            try
            {
                SourceFilePaths.AddRange(SrcFileHelper.LoadScriptsFilePaths(SrcFilePath));
            }
            catch (Exception exc)
            {
                Console.WriteLine("Compilation failed");
                Console.WriteLine($"{exc.ToString()}");
            }
        }
    }
}
