using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DaedalusCompiler.DatFile
{
    public class DatFileDef
    {
        private DatFileStream stream;

        private DatSymbol[] symbols;
        private List<DatSymbol> symbolsC;
        private List<DatSymbol> symbolsRegular;

        public DatFileDef(string filePath)
        {
            stream = new DatFileStream(File.ReadAllBytes(filePath));

            ReadVersion();
            ReadSymbols();
        }

        private char ReadVersion()
        {
            return stream.ReadChar();
        }

        private void ReadSymbols()
        {
            var sortedSymbolTableLen = stream.ReadInt();
            var sortedSymbolTable = new int[sortedSymbolTableLen];
            for (int i = 0; i < sortedSymbolTableLen; i++)
                sortedSymbolTable[i] = stream.ReadInt();


            symbols = new DatSymbol[sortedSymbolTableLen];
            symbolsC = new List<DatSymbol>();
            symbolsRegular = new List<DatSymbol>();
            for (int i = 0; i < sortedSymbolTableLen; i++)
            {
                symbols[i] = new DatSymbol(this, stream);
                symbolsC.Add(symbols[i]);

                if ((!symbols[i].isLocal) && (symbols[i].name[0] != 'ÿ'))
                    symbolsRegular.Add(symbols[i]);
            }
        }
    }
}
