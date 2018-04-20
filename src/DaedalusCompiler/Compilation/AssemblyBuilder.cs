using System;
using System.Collections.Generic;
using System.Linq;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    public class AssemblyInstruction
    {
        public DatToken opcode;
        public DatSymbol symbol;
    }

    public class AseemblyFunction
    {
        public List<DatSymbol> parameters;
        public List<AssemblyInstruction> body;
    }
    
    public class AssemblyBuilder
    {
        private Dictionary<string, AseemblyFunction> functions;
        private Dictionary<string, DatSymbol> symbols;
        
        public AssemblyBuilder()
        {
            functions = new Dictionary<string, AseemblyFunction>();
            symbols = new Dictionary<string, DatSymbol>();
        }

        public void RegisterFunction(String name, AseemblyFunction func)
        {
            functions.Add(name, func);
        }

        public void RegisterSymbol(String name, DatSymbol symbol)
        {
            symbols.Add(name, symbol);
        }

        public string getAssembler()
        {
            return "";
        }

        public string getByteCode()
        {
            return "";
        }

        public string getOutput(bool getAssembler = false)
        {
            if (getAssembler)
            {
                return this.getAssembler();
            }
            else
            {
                return getByteCode();
            }
        }
    }
}