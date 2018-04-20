using System;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    public class TokenBuilder
    {
        public static DatSymbol BuildVariable(string name, DatSymbolType type)
        {
            var symbol = new DatSymbol {};

            return symbol;
        }

        public static DatSymbol BuildArrOfVariables(string name, DatSymbolType type, int size)
        {
            var symbol = new DatSymbol {};

            return symbol;
        }

        public static DatSymbol BuildConst(string name, DatSymbolType type, object value)
        {
            var symbol = new DatSymbol {};

            return symbol;
        }

        public static DatSymbol BuildFunc(string name, DatSymbolType returnType)
        {
            var symbol = new DatSymbol {  };

            return symbol;
        }
    }
}