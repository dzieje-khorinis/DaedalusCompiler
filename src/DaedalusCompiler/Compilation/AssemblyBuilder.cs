using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    public class AssemblyInstruction
    {
    }

    public class LabelInstruction : AssemblyInstruction
    {
        public string label;
    }

    public class SymbolInstruction : AssemblyInstruction
    {
        public DatSymbol symbol;
    }

    public class ValueInstruction : AssemblyInstruction
    {
        public object value;
    }

    public class AddressInstruction : AssemblyInstruction
    {
        public int address;
    }

    public class LabelJumpInstruction : AssemblyInstruction
    {
        public string label;
    }

    public class ParamLessInstruction : AssemblyInstruction{}

    public class PushInt : ValueInstruction {}
    
    public class PushVar : SymbolInstruction {}

    public class Less : ParamLessInstruction {}
    public class Greater : ParamLessInstruction {}
    public class Assign : ParamLessInstruction {}
    public class AssignString : ParamLessInstruction {}
    public class Ret : ParamLessInstruction {}
    public class Add : ParamLessInstruction {}
    public class Multiply : ParamLessInstruction {}
    public class Divide : ParamLessInstruction {}
    public class Subract : ParamLessInstruction {}

    public class JumpIf : LabelJumpInstruction {}
    public class Call : LabelJumpInstruction {}
    
    public class CallExternal: SymbolInstruction {}

    public class AssemblyFunction
    {
        public List<AssemblyInstruction> body;
        public DatSymbol symbol;
    }

    public class AssemblyBuilder
    {
        private List<AssemblyFunction> functions;
        private List<DatSymbol> symbols;
        private AssemblyFunction active;
        private List<AssemblyInstruction> instructionStack;

        public AssemblyBuilder()
        {
            functions = new List<AssemblyFunction>();
            symbols = new List<DatSymbol>();
            instructionStack = new List<AssemblyInstruction>();
            active = null;
        }

        public void registerFunction(DatSymbol symbol)
        {
            functions.Add(new AssemblyFunction() { symbol = symbol});
            setActiveFunction(symbol);
        }

        public void addInstruction(AssemblyInstruction instruction)
        {
            instructionStack.Add(instruction);
        }

        public void functionEnd()
        {
            active.body = instructionStack;
            active = null;

            instructionStack = new ArrayList<AssemblyInstruction>();
        }

//        public void addFunctionBody(string name, List<AssemblyInstruction> body)
//        {
//            var funcToUpdate = functions.Find(x => x.name == name);
//
//            if (funcToUpdate == null)
//            {
//                throw new Exception("Function with name " + name + " is not added to assembly builder");
//            }
//
//            funcToUpdate.body = body;
//        }

        public void setActiveFunction(DatSymbol symbol)
        {
            active = functions.Find(x => x.symbol == symbol);
        }

        public void addSymbol(DatSymbol symbol)
        {
            symbols.Add(symbol);
        }
        
        public void addSymbols(List<DatSymbol> symbols)
        {
            symbols.AddRange(symbols);
        }

        public DatSymbol resolveSymbol(string symbolName)
        {
            var funcName = active.symbol.Name;
            var symbolLocalScope = symbols.Find(x => x.Name == funcName + "." + symbolName);

            if (symbolLocalScope == null)
            {
                var symbol = symbols.Find(x => x.Name == symbolName);

                if (symbol == null)
                {
                    throw new Exception("Symbol " + symbolName + " is not added");
                }

                return symbol;
            }
            else
            {
                return symbolLocalScope;
            }
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