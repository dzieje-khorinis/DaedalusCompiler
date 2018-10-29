using Antlr4.Runtime.Misc;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    public static class SymbolBuilder
    {
        public static DatSymbol BuildVariable(string name, DatSymbolType type, DatSymbolLocation location = null,
            int parentIndex = -1)
        {
            var symbol = new DatSymbol
            {
                Name = name,
                Type = type,
                ArrayLength = 1,
                Content = type == DatSymbolType.String ? new object[] {string.Empty} : new object[] {0},
                Flags = 0,
                Location = location,
                ParentIndex = parentIndex,
            };
            return symbol;
        }
        
        public static DatSymbol BuildParameter(string name, DatSymbolType type, DatSymbolLocation location = null,
            int parentIndex = -1)
        {
            return BuildVariable(name, type, location, parentIndex);
        }
        
        public static DatSymbol BuildExternalParameter(string name, DatSymbolType type, DatSymbolLocation location = null,
            int parentIndex = -1)
        {
            var symbol = BuildParameter(name, type, location, parentIndex);
            symbol.ArrayLength = 0;
            symbol.Content = null;
            return symbol;
        }

        public static DatSymbol BuildArrOfVariables(string name, DatSymbolType type, uint size,
            DatSymbolLocation location = null)
        {
            object[] content = new object[size];
            for (int i = 0; i < content.Length; ++i)
            {
                switch (type)
                {
                    case DatSymbolType.Int:
                        content[i] = 0;
                        break;
                    
                    case DatSymbolType.Float:
                        content[i] = 0.0;
                        break;
                    
                    case DatSymbolType.String:
                        content[i] = string.Empty;  // TODO shouldn't it be string in some kind of Gothic's format?
                        break;
                }              
            }
  
            
            var symbol = new DatSymbol
            {
                Name = name,
                Type = type,
                ArrayLength = size,
                Content = content,
                Flags = 0,
                Location = location,
                ParentIndex = -1,
            };

            return symbol;
        }
        
        public static DatSymbol BuildStringConst(object value, DatSymbolLocation location = null)
        {
            return BuildConst($"{(char) 255}", DatSymbolType.String, value, location);
        }

        public static DatSymbol BuildConst(string name, DatSymbolType type, object value,
            DatSymbolLocation location = null)
        {
            var symbol = new DatSymbol
            {
                Name = name,
                Type = type,
                ArrayLength = 1,
                Content = new[] {value},
                Flags = DatSymbolFlag.Const,
                Location = location,
                ParentIndex = -1,
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
                ParentIndex = -1,
            };

            return symbol;
        }

        public static DatSymbol BuildFunc(string name, uint parametersCount, [NotNull] DatSymbolType returnType)
        {
            DatSymbolFlag Flags = DatSymbolFlag.Const;
            if (returnType != DatSymbolType.Void)
            {
                Flags |= DatSymbolFlag.Return;
            }
            
            var symbol = new DatSymbol
            {
                Name = name,
                Type = DatSymbolType.Func,
                Flags = Flags,
                Content = null,
                FirstTokenAddress = -1,
                ReturnType = returnType,
                ParentIndex = -1,
                ParametersCount = parametersCount,
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
                ClassOffset = 0,
                Content = null,
                Flags = 0,
                Location = location,
                ClassSize = classSize,
                ParentIndex = -1,
            };
            
            string lowerName = name.ToLower();
            if (lowerName == "c_npc" || lowerName == "c_item")
            {
                symbol.ClassOffset = 288;
            }

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
                ClassVarOffset = classVarOffset,
                ParentIndex = classId,
            };

            return symbol;
        }

        public static DatSymbol BuildPrototype(string name, int referenceId, DatSymbolLocation location = null)
        {
            var symbol = new DatSymbol
            {
                Name = name,
                Type = DatSymbolType.Prototype,
                ArrayLength = 0,
                Content = null,
                FirstTokenAddress = -1,
                Flags = 0,
                Location = location,
                ParentIndex = referenceId,
            };

            return symbol;
        }

        public static DatSymbol BuildInstance(string name, int referenceId, DatSymbolLocation location = null)
        {
            var symbol = new DatSymbol
            {
                Name = name,
                Type = DatSymbolType.Instance,
                ArrayLength = 0,
                Content = null,
                FirstTokenAddress = -1,
                Flags = 0,
                Location = location,
                ParentIndex = referenceId,
            };

            return symbol;
        }
    }
}