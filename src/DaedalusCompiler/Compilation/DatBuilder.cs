using System;
using System.Collections.Generic;
using System.Linq;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    public class DatBuilder
    {
        private int _currentAddress;
        private readonly List<DatSymbol> _symbols;
        private readonly List<BaseExecBlock> _execBlocks;

        public DatBuilder(AssemblyBuilder assemblyBuilder)
        {
            _currentAddress = 0;
            _symbols = assemblyBuilder.Symbols;
            _execBlocks = assemblyBuilder.ExecBlocks;
        }

        private Dictionary<string, int> GetLabelToAddressDict(BaseExecBlock execBlock)
        {
            Dictionary<string, int> labelToAddress = new Dictionary<string, int>();

            foreach (var instruction in execBlock.Body)
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

        private List<DatToken> GetTokens(BaseExecBlock execBlock)
        {
            List<DatToken> tokens = new List<DatToken>();
            Dictionary<string, int> labelToAddress = GetLabelToAddressDict(execBlock);

            foreach (var instruction in execBlock.Body)
            {
                if (instruction is AssemblyLabel)
                {
                    continue;
                }


                int? intParam = null;
                byte? byteParam = null;
                switch (instruction)
                {
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
                    case AddressInstruction addressInstruction: // TODO check if there is any possibility for this
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
            foreach (var execBlock in _execBlocks)
            {
                if (execBlock.GetSymbol().Flags.HasFlag(DatSymbolFlag.External))
                {
                    continue;
                }
                if (execBlock is SharedExecBlock sharedBlock)
                {
                    foreach (var symbol in sharedBlock.Symbols)
                    {
                        symbol.FirstTokenAddress = _currentAddress;
                    }
                }
                else
                {
                    execBlock.GetSymbol().FirstTokenAddress = _currentAddress;
                }
                tokens.AddRange(GetTokens(execBlock));
            }

            return new DatFile
            {
                Version = '2',
                Symbols = _symbols,
                Tokens = tokens,
            };
        }
    }
}