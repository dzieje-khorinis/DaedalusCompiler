using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DaedalusCompiler.Dat
{
    public class DatFile_OLD
    {
        private BinaryFileStream stream;

        public DatSymbol_OLD[] symbols;
        private List<DatSymbol_OLD> symbolsC;
        private List<DatSymbol_OLD> symbolsRegular;
        private byte[] stack;

        public List<int> functionOffsets;
        public List<DatSymbol_OLD> functionSymbols;

        public DatFile_OLD(string filePath)
        {
            stream = new BinaryFileStream(filePath, FileMode.Open);

            ReadVersion();
            ReadSymbols();
            ReadStack();

            // dat file loaded
            stream = null;

            InitFuncOffsets();
            
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

            symbols = new DatSymbol_OLD[sortedSymbolTableLen];
            symbolsC = new List<DatSymbol_OLD>();
            symbolsRegular = new List<DatSymbol_OLD>();
            for (int i = 0; i < sortedSymbolTableLen; i++)
            {
                symbols[i] = new DatSymbol_OLD(this, stream);
                symbolsC.Add(symbols[i]);

                if ((!symbols[i].isLocal) && (symbols[i].name[0] != 'ÿ'))
                    symbolsRegular.Add(symbols[i]);
            }
        }

        private void ReadStack()
        {
            int stackLength = stream.ReadInt();
            stack = stream.ReadBytes(stackLength);
        }

        private void InitFuncOffsets()
        {
            if (functionOffsets == null)
            {
                functionOffsets = new List<int>();
                functionSymbols = new List<DatSymbol_OLD>();
                foreach (DatSymbol_OLD s in symbols)
                {
                    if (s.HasFunction())
                    {
                        functionOffsets.Add((int)s.content[0]);
                        functionSymbols.Add(s);
                    }
                }
                functionOffsets.Add(stack.Length);
                functionOffsets.OrderBy(x => x);

                functionSymbols.OrderBy(x => (int)x.content[0]);
            }
        }
    }
}
