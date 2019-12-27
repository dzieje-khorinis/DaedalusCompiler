using System;
using System.Collections.Generic;
using System.Linq;
using DaedalusCompiler.Compilation;
using DaedalusCompiler.Compilation.SemanticAnalysis;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Dat
{
    public class DatBuilder
    {
        private int _currentAddress;
        private string _datPath;
        private readonly Dictionary<string, Symbol> _symbolTable;

        public DatBuilder(Dictionary<string, Symbol> symbolTable, string datPath)
        {
            _currentAddress = 0;
            _symbolTable = symbolTable;
            _datPath = datPath;
        }

        private Dictionary<string, int> GetLabelToAddressDict(BlockSymbol blockSymbol)
        {
            Dictionary<string, int> labelToAddress = new Dictionary<string, int>();

            foreach (var instruction in blockSymbol.Instructions)
            {
                switch (instruction)
                {
                    case PushArrayVar _:
                    {
                        _currentAddress += 6;
                        break;
                    }

                    case JumpToLabel _:
                    case SymbolInstruction _:
                    case ValueInstruction _:
                    {
                        _currentAddress += 5;
                        break;
                    }

                    case ParamLessInstruction _:
                    {
                        _currentAddress += 1;
                        break;
                    }


                    case AssemblyLabel assemblyLabel:
                    {
                        labelToAddress[assemblyLabel.Label] = _currentAddress;
                        break;
                    }
                }
            }

            return labelToAddress;
        }

        private List<DatToken> GetTokens(BlockSymbol blockSymbol)
        {
            List<DatToken> tokens = new List<DatToken>();
            Dictionary<string, int> labelToAddress = GetLabelToAddressDict(blockSymbol);

            char prefix = (char) 255;
            string null_instance_name = $"{prefix}instance_help".ToUpper();
            
            foreach (AssemblyElement it in blockSymbol.Instructions)
            {
                AssemblyElement instruction = it;
                
                if (instruction is AssemblyLabel)
                {
                    continue;
                }

                if (instruction is PushNullInstance)
                {
                    instruction = new PushInstance(_symbolTable[null_instance_name]);
                }

                int? intParam = null;
                byte? byteParam = null;
                switch (instruction)
                {
                    case Call call:
                    {
                        intParam = ((BlockSymbol)call.Symbol).FirstTokenAddress;
                        break;
                    }
                    case PushArrayVar pushArrVar:
                    {
                        intParam = pushArrVar.Symbol.Index;
                        byteParam = (byte) pushArrVar.Index;
                        break;
                    }
                    case JumpToLabel jumpToLabel:
                    {
                        intParam = labelToAddress[jumpToLabel.Label];
                        break;
                    }
                    case SymbolInstruction symbolInstruction:
                    {
                        intParam = symbolInstruction.Symbol.Index;
                        break;
                    }
                    case ValueInstruction valueInstruction:
                    {
                        intParam = (int) valueInstruction.Value;
                        break;
                    }
                    case AddressInstruction addressInstruction:
                    {
                        intParam = addressInstruction.Address;
                        break;
                    }
                }


                string tokenName = instruction.GetType().Name;
                if (instruction is JumpToLabel)
                {
                    tokenName = tokenName.Substring(0, tokenName.Length - "ToLabel".Length);
                }
                
                DatTokenType datTokenType = Enum.Parse<DatTokenType>(tokenName);
                tokens.Add(new DatToken {TokenType = datTokenType, IntParam = intParam, ByteParam = byteParam});
            }

            return tokens;
        }

        public DatFile GetDatFile()
        {
            List<DatToken> tokens = new List<DatToken>();
            
            List<Symbol> symbols = _symbolTable.Values.ToList();
            symbols.Sort((x, y) => x.Index.CompareTo(y.SubIndex));
            symbols.Sort((x, y) => x.Index.CompareTo(y.Index));
            
            foreach (Symbol symbol in symbols)
            {
                if (symbol is FunctionSymbol functionSymbol && functionSymbol.IsExternal)
                {
                    continue;
                }
                
                if (symbol is BlockSymbol blockSymbol)
                {
                    blockSymbol.FirstTokenAddress = _currentAddress;
                    tokens.AddRange(GetTokens(blockSymbol));
                }
                
                
                /*
                if (execBlock.GetSymbol().Flags.HasFlag(DatSymbolFlag.External))
                {
                    continue;
                }
                if (execBlock is SharedExecBlockContext sharedBlock)
                {
                    foreach (var symbol in sharedBlock.Symbols)
                    {
                        if (symbol.FirstTokenAddress == -1)
                        {
                            symbol.FirstTokenAddress = _currentAddress;
                        }
                    }
                }
                else
                {
                    DatSymbol symbol = execBlock.GetSymbol();
                    if (symbol.FirstTokenAddress == -1)
                    {
                        symbol.FirstTokenAddress = _currentAddress;
                    }
                }
                tokens.AddRange(GetTokens(execBlock));
                */
            }

            return new DatFile
            {
                Version = '2',
                Symbols = symbols,
                Tokens = tokens,
            };
        }
    }
}
