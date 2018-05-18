using System;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    public class SymbolBuilder
    {
        public static DatSymbol BuildVariable(string name, DatSymbolType type, DatSymbolLocation location = null,
            int parent = -1)
        {
            var symbol = new DatSymbol
            {
                Name = name,
                Type = type,
                ArrayLength = 1,
                Content = type == DatSymbolType.String ? new object[] {string.Empty} : new object[] {0},
                Flags = 0,
                Location = location,
                ReturnType = null,
                ClassSize = null,
                ClassVarOffset = null,
                Parent = parent,
            };

            return symbol;
        }

        public static DatSymbol BuildArrOfVariables(string name, DatSymbolType type, uint size,
            DatSymbolLocation location = null)
        {
            var symbol = new DatSymbol
            {
                Name = name,
                Type = type,
                ArrayLength = size,
                Content = type == DatSymbolType.String ? new object[] {string.Empty} : new object[] {0},
                Flags = 0,
                Location = location,
                ReturnType = null,
                ClassSize = null,
                ClassVarOffset = null,
                Parent = -1,
            };

            return symbol;
        }

        public static DatSymbol BuildConst(string name, DatSymbolType type, object value,
            DatSymbolLocation location = null)
        {
            var symbol = new DatSymbol
            {
                Name = name,
                Type = type,
                ArrayLength = 1,
                Content = new object[] {value},
                Flags = DatSymbolFlag.Const,
                Location = location,
                ReturnType = null,
                ClassSize = null,
                ClassVarOffset = null,
                Parent = -1,
            };

            return symbol;
        }

        public static DatSymbol BuildArrOfConst(string name, DatSymbolType type, object[] values,
            DatSymbolLocation location = null)
        {
            var symbol = new DatSymbol
            {
                Name = name,
                Type = type,
                ArrayLength = (uint) values.Length,
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
            var symbol = new DatSymbol
            {
                Name = name,
                Parent = -1,
            };

            return symbol;
        }

        public static DatSymbol BuildClass(string name, uint classLength, int classSize,
            DatSymbolLocation location = null)
        {
            var symbol = new DatSymbol
            {
                Name = name,
                Type = DatSymbolType.Class,
                ArrayLength = classLength,
                Content = null,
                Flags = 0,
                Location = location,
                ReturnType = null,
                ClassSize = classSize,
                ClassVarOffset = null,
                Parent = -1,
            };

            return symbol;
        }

        public static DatSymbol BuildClassVar(string varName, DatSymbolType varType, uint arraySize, string className,
            int classId, int classVarOffset, DatSymbolLocation location = null)
        {
            var symbol = new DatSymbol
            {
                Name = $"{className}.{varName}",
                Type = varType,
                ArrayLength = arraySize,
                Content = null,
                Flags = DatSymbolFlag.Classvar,
                Location = location,
                ReturnType = null,
                ClassSize = null,
                ClassVarOffset = classVarOffset,
                Parent = classId,
            };

            return symbol;
        }

        public static DatSymbol BuildPrototype(string name, int referenceId, int firstTokenAddress,
            DatSymbolLocation location = null)
        {
            var symbol = new DatSymbol
            {
                Name = name,
                Type = DatSymbolType.Prototype,
                ArrayLength = 0,
                Content = new object[] {firstTokenAddress},
                Flags = 0,
                Location = location,
                ReturnType = null,
                ClassSize = null,
                ClassVarOffset = null,
                Parent = referenceId,
            };

            return symbol;
        }

        public static DatSymbol BuildInstance(string name)
        {
            var symbol = new DatSymbol {Name = name};

            return symbol;
        }
    }
}