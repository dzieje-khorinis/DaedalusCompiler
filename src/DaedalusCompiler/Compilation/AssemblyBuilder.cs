using System;
using System.Collections.Generic;
using System.Linq;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    public class AssemblyInstruction
    {
        public DatTokenType opcode;
    }

    public class SymbolInstruction : AssemblyInstruction
    {
        public DatSymbol symbol;
    }

    public class ValueInstruction : AssemblyInstruction
    {
        public object value;
    }

    public class AddressInstruction : AssemblyFunction
    {
        public int address;
    }

    public class ParamLessInstruction : AssemblyInstruction{}

    public class PushInt : ValueInstruction {}
    
    public class PushVar : SymbolInstruction {}
    
    public class Less : ParamLessInstruction {}
    public class Greater : ParamLessInstruction {}
    public class Assign : ParamLessInstruction {}
    public class AssignString : ParamLessInstruction {}
    public class Ret : ParamLessInstruction {}
    
    public class JumpIf : AddressInstruction {}

    public class AssemblyFunction
    {
        public List<DatSymbol> parameters;
        public List<AssemblyInstruction> body;
    }
    
    public class AssemblyBuilder
    {
        private Dictionary<string, AssemblyFunction> functions;
        private Dictionary<string, DatSymbol> symbols;
        
        public AssemblyBuilder()
        {
            functions = new Dictionary<string, AssemblyFunction>();
            symbols = new Dictionary<string, DatSymbol>();
        }

        public void RegisterFunction(string name, AssemblyFunction func)
        {
            functions.Add(name, func);
        }

//        public void getParamSymbol()
//        {
//            
//        }

        public void RegisterSymbol(string name, DatSymbol symbol)
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