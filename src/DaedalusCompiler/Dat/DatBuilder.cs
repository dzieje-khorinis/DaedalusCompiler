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
        private readonly IOrderedEnumerable<Symbol> _symbols;

        public DatBuilder(Dictionary<string, Symbol> symbolTable, string datPath)
        {
            _currentAddress = 0;
            _symbols = symbolTable.Values.OrderBy(symbol => symbol.Index).ThenBy(symbol => symbol.SubIndex);
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

            foreach (AssemblyElement it in blockSymbol.Instructions)
            {
                AssemblyElement instruction = it;
                
                if (instruction is AssemblyLabel)
                {
                    continue;
                }

                if (instruction is PushNullInstance)
                {
                    instruction = new PushInstance(_symbols.First());
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
            List<DatToken> datTokens = new List<DatToken>();
            List<DatSymbol> datSymbols = new List<DatSymbol>();

            foreach (Symbol symbol in _symbols)
            {
                datSymbols.Add(new DatSymbol(symbol, _currentAddress));

                if (symbol is FunctionSymbol functionSymbol && functionSymbol.IsExternal)
                {
                    continue;
                }
                
                if (symbol is BlockSymbol blockSymbol)
                {
                    datTokens.AddRange(GetTokens(blockSymbol));
                }
            }

            return new DatFile
            {
                Version = '2',
                DatSymbols = datSymbols,
                DatTokens = datTokens,
            };
        }
    }
}
