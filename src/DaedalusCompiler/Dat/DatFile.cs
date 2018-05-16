using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DaedalusCompiler.Dat
{
    /// <summary>
    /// Represents DAT file structure
    /// </summary>
    public class DatFile
    {
        public char Version { get; set; }

        public IEnumerable<DatSymbol> Symbols { get; set; }

        public IEnumerable<DatToken> Tokens { get; set; }

        public void Load(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var reader = new DatBinaryReader(stream);

                Version = reader.ReadChar();
                Symbols = LoadSymbols(reader);
                Tokens = LoadTokens(reader);
            }
        }

        public void Save(string path)
        {
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                var writer = new DatBinaryWriter(stream);

                writer.Write(Version);
                SaveSymbols(writer, Symbols);
                SaveTokens(writer, Tokens);
            }
        }

        private IEnumerable<DatSymbol> LoadSymbols(DatBinaryReader reader)
        {
            var symbolsCount = reader.ReadInt32();
            var symbolsOrder = new int[symbolsCount];
            for (int i = 0; i < symbolsCount; i++)
            {
                symbolsOrder[i] = reader.ReadInt32();
            }

            var symbols = new DatSymbol[symbolsCount];
            for (int i = 0; i < symbolsCount; i++)
            {
                symbols[i] = DatSymbol.Load(reader);
            }

            return symbols;
        }

        private IEnumerable<DatToken> LoadTokens(DatBinaryReader reader)
        {
            int stackLength = reader.ReadInt32();

            List<DatToken> result = new List<DatToken>();
            while (stackLength > 0)
            {
                var token = DatToken.Load(reader);
                result.Add(token);
                stackLength -= token.Size;
            }
            return result;
        }

        private void SaveSymbols(DatBinaryWriter writer, IEnumerable<DatSymbol> symbols)
        {
            writer.Write(symbols.Count());

            var symbolsOrder = symbols
                .Select((symbol, id) => new { Id = id, SymbolName = symbol.Name })
                .OrderBy(s => s.SymbolName, StringComparer.Ordinal)
                .Select(s => s.Id)
                .ToList();
            symbolsOrder.ForEach(id => writer.Write(id));

            foreach (var symbol in symbols)
            {
                symbol.Save(writer);
            }
        }

        private void SaveTokens(DatBinaryWriter writer, IEnumerable<DatToken> tokens)
        {
            var stackLength = Tokens.Select(x => x.Size).Sum();
            writer.Write(stackLength);

            foreach(var token in Tokens)
            {
                token.Save(writer);
            }
        }
    }
}
