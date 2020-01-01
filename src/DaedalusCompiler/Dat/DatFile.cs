using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using DaedalusCompiler.Compilation.SemanticAnalysis;

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
        
        public void Load(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                Load(stream);
            }
        }

        public void Load(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                Load(stream);
            }
        }
        

        public void Save(string path)
        {
            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                var writer = new DatBinaryWriter(stream);

                writeToStreamProgram(writer);
            }
        }

        private void writeToStreamProgram(DatBinaryWriter writer)
        {
            writer.Write(Version);
            SaveDatSymbols(writer, DatSymbols);
            SaveDatTokens(writer, DatTokens);
        }

        public byte[] getBinary()
        {
            using (var stream = new MemoryStream())
            {
                var writer = new DatBinaryWriter(stream);
                
                writeToStreamProgram(writer);

                return stream.ToArray();
            }
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
                if (_nextSymbolIndex == 21)
                {
                    Console.WriteLine("Ds");
                }
                DatSymbol symbol = new DatSymbol(reader) {Index = _nextSymbolIndex};
                symbols.Add(symbol);
                _nextSymbolIndex++;
            }
            return symbols;
        }

        private List<DatToken> LoadDatTokens(DatBinaryReader reader)
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
        

        /*
        private void SaveSymbol(DatBinaryWriter writer, Symbol symbol)
        {
            NodeLocation location = symbol.Node.Location;
            
            writer.Write(Convert.ToUInt32(symbol.Path != null));
            if (symbol.Path != null)
            {
                writer.Write(symbol.Path);
            }

            switch (symbol)
            {
                case FunctionSymbol functionSymbol:
                    writer.Write((int)functionSymbol.BuiltinType);
                    break;
                case AttributeSymbol attributeSymbol:
                    writer.Write(attributeSymbol.Offset);
                    break;
                case ClassSymbol classSymbol:
                    writer.Write(classSymbol.Size);
                    break;
                default:
                    writer.Write(0);
                    break;
            }
            
            uint bitField = 0u;
            switch (symbol)
            {
                case FunctionSymbol functionSymbol:
                    bitField |= (uint)functionSymbol.ParametersCount;
                    break;
                
                case ClassSymbol classSymbol:
                    bitField |= (uint)classSymbol.AttributesCount;
                    break;
                
                case IArraySymbol arraySymbol:
                    bitField |= (uint)arraySymbol.Size;
                    break;
                
                case ExternalParameterSymbol _:
                    break;
                
                default:
                    bitField |= 1;
                    break;
            }

            if (symbol is FunctionSymbol)
            {
                bitField |= ((uint)SymbolType.Func << 12);
            }
            else
            {
                bitField |= ((uint)symbol.BuiltinType << 12);
            }
            bitField |= ((uint)symbol.Flags << 16);
            bitField |= 0x400000;
            writer.Write(bitField);
            
            
            writer.Write(location.FileIndex);
            writer.Write(location.Line);
            writer.Write(location.LinesCount);
            writer.Write(location.Index);
            writer.Write(location.CharsCount);

            switch (symbol)
            {
                case ClassSymbol classSymbol:
                    writer.Write(classSymbol.Offset);
                    break;
                
                case BlockSymbol blockSymbol:
                    writer.Write(blockSymbol.FirstTokenAddress);
                    break;
                
                default:
                    foreach (var obj in symbol.Content ?? Enumerable.Empty<object>())
                    {
                        switch (symbol.BuiltinType)
                        {
                            case SymbolType.String:
                                writer.Write((string)obj);
                                break;
                            case SymbolType.Float:
                                writer.Write(Convert.ToSingle(obj));
                                break;
                            default:
                                writer.Write(Convert.ToInt32(obj));
                                break;
                        }
                    }
                    break;
            }
            
            switch (symbol)
            {
                case InstanceSymbol instanceSymbol:
                    writer.Write(instanceSymbol.BaseClassSymbol.Index);
                    break;
                case VarSymbol varSymbol:
                    if (varSymbol.BuiltinType == SymbolType.Instance && varSymbol.ComplexType != null)
                    {
                        writer.Write(varSymbol.ComplexType.Index);
                    }
                    else
                    {
                        writer.Write(-1);
                    }
                    break;
                default:
                    writer.Write(-1);
                    break;
            }
        }
        */

        private void SaveDatSymbols(DatBinaryWriter writer, List<DatSymbol> symbols)
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

        private void SaveDatTokens(DatBinaryWriter writer, List<DatToken> tokens)
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
