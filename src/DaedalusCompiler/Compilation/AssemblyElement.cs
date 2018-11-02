using System.Collections.Generic;
using System.Linq;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    public class AssemblyElement
    {
    }

    public class AssemblyInstruction : AssemblyElement
    {
    }

    public class AssemblyLabel : AssemblyElement
    {
        public readonly string Label;

        public AssemblyLabel(string label)
        {
            Label = label;
        }
    }

    public class SymbolInstruction : AssemblyInstruction
    {
        public readonly DatSymbol Symbol;

        protected SymbolInstruction(DatSymbol symbol)
        {
            Symbol = symbol;
        }
    }

    public class ValueInstruction : AssemblyInstruction
    {
        public readonly object Value;

        protected ValueInstruction(object value)
        {
            Value = value;
        }
    }

    public abstract class AddressInstruction : AssemblyInstruction
    {
        public readonly int Address;

        protected AddressInstruction(int address)
        {
            Address = address;
        }
    }

    public class JumpToLabel : AssemblyInstruction
    {
        public readonly string Label;

        public JumpToLabel(string label)
        {
            Label = label;
        }
    }

    public class ParamLessInstruction : AssemblyInstruction
    {
    }

    public class PushInt : ValueInstruction
    {
        public PushInt(int value) : base(value)
        {
        }
    }

    public class PushVar : SymbolInstruction
    {
        public PushVar(DatSymbol symbol) : base(symbol)
        {
        }
    }

    public class PushArrayVar : SymbolInstruction
    {
        public readonly int Index;

        public PushArrayVar(DatSymbol symbol, int index) : base(symbol)
        {
            Index = index;
        }
    }

    public class PushInstance : SymbolInstruction
    {
        public PushInstance(DatSymbol symbol) : base(symbol)
        {
        }
    }

    public class SetInstance : SymbolInstruction
    {
        public SetInstance(DatSymbol symbol) : base(symbol)
        {
        }
    }

    public class Equal : ParamLessInstruction {}
    public class NotEqual : ParamLessInstruction {}
    public class Less : ParamLessInstruction {}
    public class LessOrEqual : ParamLessInstruction {}
    public class Greater : ParamLessInstruction {}
    public class GreaterOrEqual : ParamLessInstruction {}
    public class Assign : ParamLessInstruction {}
    public class AssignAdd : ParamLessInstruction {}
    public class AssignSubtract : ParamLessInstruction {}
    public class AssignMultiply : ParamLessInstruction {}
    public class AssignDivide : ParamLessInstruction {}
    public class AssignString : ParamLessInstruction {}
    // public class AssignStringRef : ParamLessInstruction {}
    public class AssignFunc : ParamLessInstruction {}
    public class AssignFloat : ParamLessInstruction {}
    public class AssignInstance : ParamLessInstruction {}
    public class Ret : ParamLessInstruction {}
    public class Add : ParamLessInstruction {}
    public class Multiply : ParamLessInstruction {}
    public class Divide : ParamLessInstruction {}
    public class Subtract : ParamLessInstruction {}
    public class Modulo : ParamLessInstruction {}
    public class Not: ParamLessInstruction {}
    public class Minus: ParamLessInstruction {}
    public class Plus: ParamLessInstruction {}
    public class Negate: ParamLessInstruction {}
    public class ShiftLeft: ParamLessInstruction {}
    public class ShiftRight: ParamLessInstruction {}
    public class BitAnd: ParamLessInstruction {}
    public class BitOr: ParamLessInstruction {}
    public class LogAnd: ParamLessInstruction {}
    public class LogOr: ParamLessInstruction {}
    
    public class JumpIfToLabel : JumpToLabel
    {
        public JumpIfToLabel(string name) : base(name)
        {
        }
    }
    //public class Call : LabelJumpInstruction {}

    public class Call : SymbolInstruction
    {
        public Call(DatSymbol symbol) : base(symbol)
        {
        }
    }

    public class CallExternal : SymbolInstruction
    {
        public CallExternal(DatSymbol symbol) : base(symbol)
        {
        }
    }

    public class LazyReferenceAtomInstructions : AssemblyInstruction
    {
        public readonly DaedalusParser.ReferenceAtomContext[] ReferenceAtoms;
        public readonly AssemblyBuilderSnapshot AssemblyBuilderSnapshot;
        
        public LazyReferenceAtomInstructions(
            AssemblyBuilderSnapshot assemblyBuilderSnapshot,
            DaedalusParser.ReferenceAtomContext[] referenceAtoms)
        {
            AssemblyBuilderSnapshot = assemblyBuilderSnapshot;
            ReferenceAtoms = referenceAtoms;
        }
    }
    
}