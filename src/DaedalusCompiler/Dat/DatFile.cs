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
            using (var stream = new BinaryFileStream(path, FileMode.Open))
            {
                Version = stream.ReadChar();
                Symbols = LoadSymbols(stream);
                Tokens = LoadTokens(stream);
            }
        }

        private IEnumerable<DatSymbol> LoadSymbols(BinaryFileStream stream)
        {
            var symbolsCount = stream.ReadInt();
            var symbolsOrder = new int[symbolsCount];
            for (int i = 0; i < symbolsCount; i++)
            {
                symbolsOrder[i] = stream.ReadInt();
            }

            var symbols = new DatSymbol[symbolsCount];
            for (int i = 0; i < symbolsCount; i++)
            {
                symbols[i] = DatSymbol.Load(stream);
            }

            return symbols;
        }

        private IEnumerable<DatToken> LoadTokens(BinaryFileStream stream)
        {
            int stackLength = stream.ReadInt();

            List<DatToken> result = new List<DatToken>();
            while (stackLength > 0)
            {
                var token = DatToken.LoadToken(stream);
                result.Add(token);
                stackLength -= token.Size;
            }
            return result;
        }
    }
}
