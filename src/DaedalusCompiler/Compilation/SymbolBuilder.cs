/*
using Antlr4.Runtime.Misc;
using DaedalusCompiler.Compilation.SemanticAnalysis;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    public static class SymbolBuilder
    {
        public static DatSymbol BuildClass(string name, int attributesCount)
        {
            int classOffset = 0;
            string nameUpper = name.ToUpper();
            if (nameUpper == "C_NPC" || nameUpper == "C_ITEM")
            {
                classOffset = 288;
            }
            
            return new DatSymbol
            {
                Name = name,
                ArrayLength = (uint) attributesCount,
                BuiltinType = SymbolType.Class,
                ClassOffset = classOffset,
            };
        }
        
        public static DatSymbol BuildClassConst(string varName, SymbolType builtinType, DatSymbol classSymbol)
        {
            return new DatSymbol
            {
                Name = $"{classSymbol.Name}.{varName}",
                ArrayLength = 1,
                BuiltinType = builtinType,
                Flags = SymbolFlag.Classvar | SymbolFlag.Const,
                Parent = classSymbol,
            };
        }
        
        public static DatSymbol BuildGlobalConst(string name, SymbolType builtinType)
        {
            return new DatSymbol
            {
                Name = name,
                ArrayLength = 1,
                BuiltinType = builtinType,
                Flags = SymbolFlag.Const,
            };
        }
        
        public static DatSymbol BuildLocalConst(string name, SymbolType builtinType, DatSymbol functionSymbol)
        {
            return new DatSymbol
            {
                Name = $"{functionSymbol.Name}.{name}",
                ArrayLength = 1,
                BuiltinType = builtinType,
                Flags = SymbolFlag.Const,
            };
        }
        
        public static DatSymbol BuildClassVar(string varName, SymbolType builtinType, DatSymbol classSymbol)
        {
            return new DatSymbol
            {
                Name = $"{classSymbol.Name}.{varName}",
                ArrayLength = 1,
                BuiltinType = builtinType,
                Flags = SymbolFlag.Classvar,
                Parent = classSymbol,
            };
        }
        
        public static DatSymbol BuildGlobalVar(string name, SymbolType builtinType)
        {
            return new DatSymbol
            {
                Name = name,
                ArrayLength = 1,
                BuiltinType = builtinType,
                Content = builtinType == SymbolType.String ? new object[] {string.Empty} : new object[] {0},
            };
        }
        
        public static DatSymbol BuildLocalVar(string name, SymbolType builtinType, DatSymbol functionSymbol)
        {
            return new DatSymbol
            {
                Name = $"{functionSymbol.Name}.{name}",
                ArrayLength = 1,
                BuiltinType = builtinType,
                Content = builtinType == SymbolType.String ? new object[] {string.Empty} : new object[] {0},
            };
        }
        
        
        
        
        
        public static DatSymbol BuildParameter(string name, SymbolType builtinType, DatSymbol functionSymbol)
        {
            return new DatSymbol
            {
                Name = $"{functionSymbol.Name}.{name}",
                ArrayLength = 1,
                BuiltinType = builtinType,
                ParametersCount = (uint) ((builtinType == SymbolType.Func) ? 1 : 0),
            };
        }
        
        public static DatSymbol BuildExternalParameter(string name, SymbolType builtinType, DatSymbol functionSymbol)
        {
            return new DatSymbol
            {
                Name = $"{functionSymbol.Name}.{name}",
                BuiltinType = builtinType,
            };
        }
        
        
        

        public static DatSymbol BuildArrOfVariables(string name, SymbolType type, uint size,
            NodeLocation location = null)
        {
            object[] content = new object[size];
            for (int i = 0; i < content.Length; ++i)
            {
                switch (type)
                {
                    case SymbolType.Int:
                        content[i] = 0;
                        break;
                    
                    case SymbolType.Float:
                        content[i] = 0.0;
                        break;
                    
                    case SymbolType.String:
                        content[i] = string.Empty;
                        break;
                }              
            }
  
            
            var symbol = new DatSymbol
            {
                Name = name,
                BuiltinType = type,
                ArrayLength = size,
                Content = content,
                Flags = 0,
                Location = location,
                ParentIndex = -1,
            };

            return symbol;
        }
        
        
        public static DatSymbol BuildStringConst(string name, object value)
        {
            return new DatSymbol
            {
                Name = name,
                BuiltinType = SymbolType.String,
                ArrayLength = 1,
                Content = new[] {value},
                Flags = SymbolFlag.Const,
            };
        }
        
        
        public static DatSymbol BuildArrOfConst(string name, SymbolType type, object[] values,
            NodeLocation location = null)
        {
            var symbol = new DatSymbol
            {
                Name = name,
                BuiltinType = type,
                ArrayLength = (uint) values.Length,
                Content = values,
                Flags = SymbolFlag.Const,
                Location = location,
                ParentIndex = -1,
            };

            return symbol;
        }
        

        public static DatSymbol BuildFunction(string name, int parametersCount, SymbolType returnType)
        {
            if (returnType == SymbolType.Class)
            {
                returnType = SymbolType.Instance;
            }
            
            SymbolFlag flags = SymbolFlag.Const;
            if (returnType != SymbolType.Void)
            {
                flags |= SymbolFlag.Return;
            }
            
            return new DatSymbol
            {
                Name = name,
                BuiltinType = SymbolType.Func,
                Flags = flags,
                FirstTokenAddress = -1,
                ReturnType = returnType,
                ParametersCount = (uint) parametersCount,
            };
        }

        public static DatSymbol BuildPrototype(string name)
        {
            return new DatSymbol
            {
                Name = name,
                BuiltinType = SymbolType.Prototype,
                FirstTokenAddress = -1,
            };
        }

        public static DatSymbol BuildInstance(string name)
        {
            return new DatSymbol
            {
                Name = name,
                BuiltinType = SymbolType.Instance,
                FirstTokenAddress = -1,
            };
        }
    }
}
*/