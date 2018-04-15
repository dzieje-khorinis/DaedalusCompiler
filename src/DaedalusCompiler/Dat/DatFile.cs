using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DaedalusCompiler.Dat
{
    /// <summary>
    /// Represents DAT file structure
    /// </summary>
    public class DatFile
    {
        public char Version { get; set; }

        public IEnumerable<DatSymbol> Symbols { get; set; }

        public byte[] Stack { get; set; }

        public void Load(string path)
        {
            using (var stream = new BinaryFileStream(path, FileMode.Open))
            {
                Version = stream.ReadChar();
                Symbols = LoadSymbols(stream);
                Stack = LoadStack(stream);
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
                symbols[i] = new DatSymbol(stream);
            }

            return symbols;
        }

        private byte[] LoadStack(BinaryFileStream stream)
        {
            int stackLength = stream.ReadInt();
            return stream.ReadBytes(stackLength);
        }
    }
}
