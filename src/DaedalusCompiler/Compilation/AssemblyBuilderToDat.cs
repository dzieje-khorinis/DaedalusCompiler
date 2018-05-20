using System;
using System.Collections.Generic;
using System.Linq;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    public static class AssemblyBuilderToDat
    {
        private static int GetSymbolId(List<DatSymbol> symbols, DatSymbol symbol)
        {
            return symbols.IndexOf(symbol);
        }

        public static DatFile GetDatFile(List<ExecBlock> execBlocks, List<DatSymbol> symbols)
        {
            // TODO: Refactor this code later

            // TODO Now we take last function, make support all !!!
            var assembly = execBlocks.Last().Body;
            var labels = assembly
                .Select((tokenClass, id) => new {id, tokenClass})
                .Where(x => x.tokenClass is AssemblyLabel)
                .Select((x, i) => new {id = x.id - i, label = ((AssemblyLabel) x.tokenClass).Label})
                .ToDictionary(x => x.label, x => x.id);

            var tokens = assembly
                .Where(x => x is AssemblyIfStatement == false)
                .Select(tokenClass =>
                {
                    var tokenName = tokenClass.GetType().Name;
                    var tokenType = Enum.Parse<DatTokenType>(tokenName);
                    int? intParam = null;
                    byte? byteParam = null;

                    if (tokenClass is PushArrVar arrayVar)
                    {
                        intParam = GetSymbolId(symbols, arrayVar.Symbol);
                        byteParam = (byte) arrayVar.Index;
                    }
                    else if (tokenClass is JumpToLabel label)
                    {
                        //TODO: this is token id, should be changed to token stack location later
                        intParam = labels[label.Label];
                    }
                    else if (tokenClass is SymbolInstruction instruction)
                    {
                        intParam = GetSymbolId(symbols, instruction.Symbol);
                    }
                    else if (tokenClass is ValueInstruction valueInstruction)
                    {
                        intParam = (int) valueInstruction.Value;
                    }
                    else if (tokenClass is AddressInstruction addressInstruction)
                    {
                        intParam = addressInstruction.Address;
                    }

                    return new DatToken {TokenType = tokenType, IntParam = intParam, ByteParam = byteParam};
                });

            return new DatFile
            {
                Version = '2',
                Symbols = symbols,
                Tokens = tokens.ToList()
            };
        }
    }
}