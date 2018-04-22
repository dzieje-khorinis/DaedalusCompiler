using System;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    public class SymbolBuilder
    {
        public static DatSymbol BuildVariable(string name, DatSymbolType type)
        {
            var symbol = new DatSymbol { Name = name};

            return symbol;
        }

        public static DatSymbol BuildArrOfVariables(string name, DatSymbolType type, int size)
        {
            var symbol = new DatSymbol { Name = name};

            return symbol;
        }

        public static DatSymbol BuildConst(string name, DatSymbolType type, object value)
        {
            var symbol = new DatSymbol { Name = name};

            return symbol;
        }

        public static DatSymbol BuildArrOfConst(string name, DatSymbolType type, object[] values)
        {
            var symbol = new DatSymbol { Name = name };

            return symbol;
        }

        public static DatSymbol BuildFunc(string name, DatSymbolType returnType)
        {
            var symbol = new DatSymbol { Name = name};

            return symbol;
        }

        public static DatSymbol BuildClass(string name)
        {
            var symbol = new DatSymbol { Name = name };

            return symbol;
        }

        public static DatSymbol BuildPrototype(string name)
        {
            var symbol = new DatSymbol { Name = name };

            return symbol;
        }

        public static DatSymbol BuildInstance(string name)
        {
            var symbol = new DatSymbol { Name = name };

            return symbol;
        }
    }
}