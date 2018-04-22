using System;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    public class SymbolBuilder
    {
        public static DatSymbol BuildVariable(string name, DatSymbolType type, DatSymbolLocation location = null)
        {
            var symbol = new DatSymbol
            {
                Name = name,
                Type = type,
                ArrayLength = 1,
                Content = type == DatSymbolType.String ? new object[] { string.Empty } : new object[] { 0 },
                Flags = 0,
                Location = location,
                ReturnType = null,
                ClassSize = null,
                ClassVarOffset = null,
                Parent = -1,
            };

            return symbol;
        }

        public static DatSymbol BuildArrOfVariables(string name, DatSymbolType type, uint size, DatSymbolLocation location = null)
        {
            var symbol = new DatSymbol
            {
                Name = name,
                Type = type,
                ArrayLength = size,
                Content = type == DatSymbolType.String ? new object[] { string.Empty } : new object[] { 0 },
                Flags = 0,
                Location = location,
                ReturnType = null,
                ClassSize = null,
                ClassVarOffset = null,
                Parent = -1,
            };

            return symbol;
        }

        public static DatSymbol BuildConst(string name, DatSymbolType type, object value, DatSymbolLocation location = null)
        {
            var symbol = new DatSymbol
            {
                Name = name,
                Type = type,
                ArrayLength = 1,
                Content = new object[] { value },
                Flags = DatSymbolFlag.Const,
                Location = location,
                ReturnType = null,
                ClassSize = null,
                ClassVarOffset = null,
                Parent = -1,
            };

            return symbol;
        }

        public static DatSymbol BuildArrOfConst(string name, DatSymbolType type, object[] values, DatSymbolLocation location = null)
        {
            var symbol = new DatSymbol
            {
                Name = name,
                Type = type,
                ArrayLength = (uint)values.Length,
                Content = values,
                Flags = DatSymbolFlag.Const,
                Location = location,
                ReturnType = null,
                ClassSize = null,
                ClassVarOffset = null,
                Parent = -1,
            };

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