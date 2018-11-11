using System;
using System.Collections.Generic;
using DaedalusCompiler.Compilation;
using DaedalusCompiler.Dat;

namespace DaedalusLib
{
    public class DaedalusCodeLoader
    {
        private Compiler _compiler;
        
        public DaedalusCodeLoader(string sourcePath)
        {
            _compiler = new Compiler();

            _compiler.CompileFromSrc(sourcePath, false, false, false);
        }
        
        public List<DatSymbol> GetSymbols()
        {
            return _compiler.GetSymbols();
        }

        // TODO implement
        public List<DatSymbol> GetSymbolsExtendedByInstance(string instanceName)
        {
            return new List<DatSymbol>();
        }

        public List<BaseExecBlockContext> GetExecBlocks()
        {
            return _compiler.GetExecBlocks();
        }
    }
}