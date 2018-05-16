using System;
using System.Collections.Generic;
using System.Linq;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    public static class AssemblyBuilderToDat
    {
        private static int getSymbolId(List<DatSymbol> symbols, DatSymbol symbol)
        {
            return symbols.IndexOf(symbol);
        }

        public static DatFile getDatFile(List<ExecBlock> execBlocks, List<DatSymbol> symbols)
        {
            // TODO: Refactor this code later

            // TODO Now we take last function, make support all !!!
            var assembly = execBlocks.Last().body;
            var labels = assembly
                .Select((tokenClass, id) => new { id, tokenClass })
                .Where(x => x.tokenClass is AssemblyLabel)
                .Select((x, i) => new { id = x.id - i, ((AssemblyLabel)x.tokenClass).label })
                .ToDictionary(x => x.label, x => x.id);

            var tokens = assembly
                .Where(x => x is AssemblyIfStatement == false)
                .Select(tokenClass =>
                {
                    var tokenName = tokenClass.GetType().Name;
                    var tokenType = Enum.Parse<DatTokenType>(tokenName);
                    int? intParam = null;
                    byte? byteParam = null;

                    if (tokenClass is PushArrVar)
                    {
                        var arrayVar = (PushArrVar)tokenClass;
                        intParam = getSymbolId(symbols, arrayVar.symbol);
                        byteParam = (byte)arrayVar.index;
                    }
                    else if (tokenClass is LabelJumpInstruction)
                    {
                        //TODO: this is token id, should be changed to token stack location later
                        intParam = labels[((LabelJumpInstruction)tokenClass).label];
                    }
                    else if (tokenClass is SymbolInstruction)
                    {
                        intParam = getSymbolId(symbols, ((SymbolInstruction)tokenClass).symbol);
                    }
                    else if (tokenClass is ValueInstruction)
                    {
                        intParam = (int)((ValueInstruction)tokenClass).value;
                    }
                    else if (tokenClass is AddressInstruction)
                    {
                        intParam = ((AddressInstruction)tokenClass).address;
                    }

                    return new DatToken { TokenType = tokenType, IntParam = intParam, ByteParam = byteParam };
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