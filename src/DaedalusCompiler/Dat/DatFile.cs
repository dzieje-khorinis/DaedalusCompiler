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

        public IEnumerable<Symbol> Symbols { get; set; }

        public IEnumerable<DatToken> Tokens { get; set; }


        public DatFile() {}

        /*
        public DatFile(string filePath)
        {
            using (FileStream stream = File.Open(filePath, FileMode.Open))
            {
                Load(stream);
            }
        }
        */

        /*
        private void Load(Stream stream)
        {
            var reader = new DatBinaryReader(stream);

            Version = reader.ReadChar();
            Symbols = LoadSymbols(reader);
            Tokens = LoadTokens(reader);
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
        */

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
            SaveSymbols(writer, Symbols);
            SaveTokens(writer, Tokens);
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

        /*
        private IEnumerable<Symbol> LoadSymbols(DatBinaryReader reader)
        {
            var symbolsCount = reader.ReadInt32();
            var symbolsOrder = new int[symbolsCount];
            for (int i = 0; i < symbolsCount; i++)
            {
                symbolsOrder[i] = reader.ReadInt32();
            }

            var symbols = new Symbol[symbolsCount];
            for (int i = 0; i < symbolsCount; i++)
            {
                symbols[i] = Symbol.Load(reader);
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
        */

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

        private void SaveSymbols(DatBinaryWriter writer, IEnumerable<Symbol> symbols)
        {
            writer.Write(symbols.Count());

            var symbolsOrder = symbols
                .Select((symbol, id) => new { Id = id, SymbolName = symbol.Name })
                .OrderBy(s => s.SymbolName, StringComparer.OrdinalIgnoreCase)
                .Select(s => s.Id)
                .ToList();
            symbolsOrder.ForEach(id => writer.Write(id));

            foreach (var symbol in symbols)
            {
                SaveSymbol(writer, symbol);
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
