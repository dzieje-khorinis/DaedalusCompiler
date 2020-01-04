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

        public List<DatSymbol> DatSymbols { get; set; }

        public List<DatToken> DatTokens { get; set; }

        private int _nextSymbolIndex;

        public DatFile() {}

        
        public DatFile(string filePath)
        {
            using (FileStream stream = File.Open(filePath, FileMode.Open))
            {
                Load(stream);
            }
        }
        
        private void Load(Stream stream)
        {
            var reader = new DatBinaryReader(stream);

            Version = reader.ReadChar();
            DatSymbols = LoadDatSymbols(reader);
            DatTokens = LoadDatTokens(reader);
        }

        public void Save(string path)
        {
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                var writer = new DatBinaryWriter(stream);

                WriteToStreamProgram(writer);
            }
        }

        private void WriteToStreamProgram(DatBinaryWriter writer)
        {
            writer.Write(Version);
            SaveDatSymbols(writer, DatSymbols);
            SaveDatTokens(writer, DatTokens);
        }

        private List<DatSymbol> LoadDatSymbols(DatBinaryReader reader)
        {
            _nextSymbolIndex = 0;
            var symbolsCount = reader.ReadInt32();
            var symbolsOrder = new int[symbolsCount];
            for (int i = 0; i < symbolsCount; i++)
            {
                symbolsOrder[i] = reader.ReadInt32();
            }

            List<DatSymbol> symbols = new List<DatSymbol>();
            for (int i = 0; i < symbolsCount; i++)
            {
                DatSymbol symbol = new DatSymbol(reader) {Index = _nextSymbolIndex};
                symbols.Add(symbol);
                _nextSymbolIndex++;
            }
            return symbols;
        }

        private static List<DatToken> LoadDatTokens(DatBinaryReader reader)
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

        private static void SaveDatSymbols(DatBinaryWriter writer, List<DatSymbol> symbols)
        {
            writer.Write(symbols.Count);

            List<int> nameOrderedSymbols = symbols
                .Select((symbol, id) => new { Id = id, SymbolName = symbol.Name })
                .OrderBy(s => s.SymbolName, StringComparer.OrdinalIgnoreCase)
                .Select(s => s.Id)
                .ToList();
            nameOrderedSymbols.ForEach(writer.Write);

            foreach (DatSymbol symbol in symbols)
            {
                symbol.Save(writer);
            }
        }

        private static void SaveDatTokens(DatBinaryWriter writer, List<DatToken> tokens)
        {
            int stackLength = tokens.Select(x => x.Size).Sum();
            writer.Write(stackLength);

            foreach(var token in tokens)
            {
                token.Save(writer);
            }
        }
    }
}
