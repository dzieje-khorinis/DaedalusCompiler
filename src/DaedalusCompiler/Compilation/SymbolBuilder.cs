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
                BuiltinType = DatSymbolType.Class,
                ClassOffset = classOffset,
            };
        }
        
        public static DatSymbol BuildClassConst(string varName, DatSymbolType builtinType, DatSymbol classSymbol)
        {
            return new DatSymbol
            {
                Name = $"{classSymbol.Name}.{varName}",
                ArrayLength = 1,
                BuiltinType = builtinType,
                Flags = DatSymbolFlag.Classvar | DatSymbolFlag.Const,
                Parent = classSymbol,
            };
        }
        
        public static DatSymbol BuildGlobalConst(string name, DatSymbolType builtinType)
        {
            return new DatSymbol
            {
                Name = name,
                ArrayLength = 1,
                BuiltinType = builtinType,
                Flags = DatSymbolFlag.Const,
            };
        }
        
        public static DatSymbol BuildLocalConst(string name, DatSymbolType builtinType, DatSymbol functionSymbol)
        {
            return new DatSymbol
            {
                Name = $"{functionSymbol.Name}.{name}",
                ArrayLength = 1,
                BuiltinType = builtinType,
                Flags = DatSymbolFlag.Const,
            };
        }
        
        public static DatSymbol BuildClassVar(string varName, DatSymbolType builtinType, DatSymbol classSymbol)
        {
            return new DatSymbol
            {
                Name = $"{classSymbol.Name}.{varName}",
                ArrayLength = 1,
                BuiltinType = builtinType,
                Flags = DatSymbolFlag.Classvar,
                Parent = classSymbol,
            };
        }
        
        public static DatSymbol BuildGlobalVar(string name, DatSymbolType builtinType)
        {
            return new DatSymbol
            {
                Name = name,
                ArrayLength = 1,
                BuiltinType = builtinType,
                Content = builtinType == DatSymbolType.String ? new object[] {string.Empty} : new object[] {0},
            };
        }
        
        public static DatSymbol BuildLocalVar(string name, DatSymbolType builtinType, DatSymbol functionSymbol)
        {
            return new DatSymbol
            {
                Name = $"{functionSymbol.Name}.{name}",
                ArrayLength = 1,
                BuiltinType = builtinType,
                Content = builtinType == DatSymbolType.String ? new object[] {string.Empty} : new object[] {0},
            };
        }
        
        
        
        
        
        public static DatSymbol BuildParameter(string name, DatSymbolType builtinType, DatSymbol functionSymbol)
        {
            return new DatSymbol
            {
                Name = $"{functionSymbol.Name}.{name}",
                ArrayLength = 1,
                BuiltinType = builtinType,
                ParametersCount = (uint) ((builtinType == DatSymbolType.Func) ? 1 : 0),
            };
        }
        
        public static DatSymbol BuildExternalParameter(string name, DatSymbolType builtinType, DatSymbol functionSymbol)
        {
            return new DatSymbol
            {
                Name = $"{functionSymbol.Name}.{name}",
                BuiltinType = builtinType,
            };
        }
        
        
        
        
        
        /*
        public static DatSymbol BuildArrOfVariables(string name, DatSymbolType type, uint size,
            NodeLocation location = null)
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
        */
        
        public static DatSymbol BuildStringConst(string name, object value)
        {
            return new DatSymbol
            {
                Name = name,
                BuiltinType = DatSymbolType.String,
                ArrayLength = 1,
                Content = new[] {value},
                Flags = DatSymbolFlag.Const,
            };
        }
        
        /*
        public static DatSymbol BuildArrOfConst(string name, DatSymbolType type, object[] values,
            NodeLocation location = null)
        {
            var symbol = new DatSymbol
            {
                Name = name,
                BuiltinType = type,
                ArrayLength = (uint) values.Length,
                Content = values,
                Flags = DatSymbolFlag.Const,
                Location = location,
                ParentIndex = -1,
            };

            return symbol;
        }
        */

        public static DatSymbol BuildFunction(string name, int parametersCount, DatSymbolType returnType)
        {
            if (returnType == DatSymbolType.Class)
            {
                returnType = DatSymbolType.Instance;
            }
            
            DatSymbolFlag flags = DatSymbolFlag.Const;
            if (returnType != DatSymbolType.Void)
            {
                flags |= DatSymbolFlag.Return;
            }
            
            return new DatSymbol
            {
                Name = name,
                BuiltinType = DatSymbolType.Func,
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
                BuiltinType = DatSymbolType.Prototype,
                FirstTokenAddress = -1,
            };
        }

        public static DatSymbol BuildInstance(string name)
        {
            return new DatSymbol
            {
                Name = name,
                BuiltinType = DatSymbolType.Instance,
                FirstTokenAddress = -1,
            };
        }
    }
}