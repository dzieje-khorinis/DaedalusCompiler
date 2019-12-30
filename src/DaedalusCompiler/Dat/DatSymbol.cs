using System;
using System.Diagnostics;
using System.Linq;
using DaedalusCompiler.Compilation;
using DaedalusCompiler.Compilation.SemanticAnalysis;

namespace DaedalusCompiler.Dat
{
    
    [DebuggerDisplay("{IsFunction ? \"func \"}{BuiltinType} {Name} '{Flags}'")]
    public class DatSymbol
    {
        public bool IsFunction;
        public bool IsAddressable;
        
        public int Index;

        public string Name;

        public uint OffClsRet;  // Offset (ClassVar) | Size (Class) | ReturnType (Func)

        public uint Count; // AttributesCount (Class) | ParametersCount (Function) | 0 (External function) | ArraySize (Var/Const) | 1 (Default)
        
        public SymbolType BuiltinType;
        
        public SymbolFlag Flags;

        public uint FileIndex;
        public uint Line;
        public uint LinesCount;
        public uint Column;
        public uint CharsCount;
        
        public object[] Content; // Offset (Class) | FirstTokenAddress (Callable) | Content (Const)

        public int ParentIndex;


        /// <summary>
        /// Creates DatSymbol from Symbol
        /// </summary>
        public DatSymbol(Symbol symbol, int address)
        {
            IsFunction = symbol is FunctionSymbol;
            IsAddressable = symbol is BlockSymbol;
            
            Index = symbol.Index;
            Name = symbol.Path;
            
            switch (symbol)
            {
                case FunctionSymbol functionSymbol:
                    OffClsRet = (uint)functionSymbol.BuiltinType;
                    break;
                case AttributeSymbol attributeSymbol:
                    OffClsRet = (uint) attributeSymbol.Offset;
                    break;
                case ClassSymbol classSymbol:
                    OffClsRet = (uint) classSymbol.Size;
                    break;
                default:
                    OffClsRet = 0;
                    break;
            }
            
            
            switch (symbol)
            {
                case FunctionSymbol functionSymbol:
                    Count = (uint)functionSymbol.ParametersCount;
                    break;
                
                case ClassSymbol classSymbol:
                    Count = (uint)classSymbol.AttributesCount;
                    break;
                
                case IArraySymbol arraySymbol:
                    Count = (uint)arraySymbol.Size;
                    break;
                
                case ExternalParameterSymbol _:
                    Count = 0;
                    break;
                
                default:
                    Count = 1;
                    break;
            }

            BuiltinType = IsFunction ? SymbolType.Func : symbol.BuiltinType;
            Flags = symbol.Flags;
            
            FileIndex = (uint) symbol.Node.Location.FileIndex;
            Line = (uint) symbol.Node.Location.Line;
            LinesCount = (uint) symbol.Node.Location.LinesCount;
            Column = (uint) symbol.Node.Location.Column;
            CharsCount = (uint) symbol.Node.Location.CharsCount;

            switch (symbol)
            {
                case ClassSymbol classSymbol:
                    Content = new object[] {classSymbol.Offset};
                    break;
                
                case BlockSymbol _:
                    Content = new object[] {address};
                    break;

                default:
                    Content = symbol.Content;
                    break;
                
            }

            ParentIndex = -1;
            switch (symbol)
            {
                case SubclassSymbol subclassSymbol:
                    ParentIndex = subclassSymbol.InheritanceParentSymbol.Index;
                    break;
                case AttributeSymbol attributeSymbol:
                    ParentIndex = attributeSymbol.ParentBlockSymbol.Index;
                    break;
                case VarSymbol varSymbol:
                    if (varSymbol.ComplexType is ClassSymbol classSymbol)
                    {
                        ParentIndex = classSymbol.Index;
                    }
                    break;
            }
        }

        /// <summary>
        /// Creates DatSymbol from binary (DAT formatted) stream
        /// </summary>
        public DatSymbol(DatBinaryReader reader)
        {
            var hasName = Convert.ToBoolean(reader.ReadUInt32());
            if (hasName)  // TODO is it even possible for symbol to not have name?
            {
                Name = reader.ReadString();
            }

            OffClsRet = reader.ReadUInt32();
            
            uint bitField = reader.ReadUInt32();
            Count = bitField & 0xFFF;
            BuiltinType = (SymbolType)((bitField & 0xF000) >> 12);
            Flags = (SymbolFlag)((bitField & 0x3F0000) >> 16);

            FileIndex = reader.ReadUInt32();
            Line = reader.ReadUInt32();
            LinesCount = reader.ReadUInt32();
            Column = reader.ReadUInt32();
            CharsCount = reader.ReadUInt32();

            Content = null;
            switch (BuiltinType)
            {
                case SymbolType.Class:
                case SymbolType.Func:
                case SymbolType.Instance:
                case SymbolType.Prototype:
                    Content = new object[]{reader.ReadInt32()};
                    break;
                
                default:
                    if (Flags.HasFlag(SymbolFlag.ClassVar))
                    {
                        break;
                    }
                    Content = new object[Count];
                    for (int i = 0; i < Count; ++i)
                    {
                        switch (BuiltinType)
                        {
                            case SymbolType.String:
                                Content[i] = reader.ReadString();
                                break;
                            case SymbolType.Float:
                                Content[i] = reader.ReadSingle();
                                break;
                            default:
                                Content[i] = reader.ReadInt32();
                                break;
                        }
                    }
                    break;
            }

            ParentIndex = reader.ReadInt32();
        }
        

        /// <summary>
        /// Saves DatSymbol to binary stream (in DAT format) 
        /// </summary>
        public void Save(DatBinaryWriter writer)
        {
            writer.Write(Convert.ToUInt32(Name != null)); // TODO is it even possible for symbol to not have name?
            if (Name != null)
            {
                writer.Write(Name);
            }

            writer.Write(OffClsRet);
            
            uint bitField = Count;
            bitField |= (uint)BuiltinType << 12;
            bitField |= (uint)Flags << 16;
            bitField |= 0x400000;
            writer.Write(bitField);

            writer.Write(FileIndex);
            writer.Write(Line);
            writer.Write(LinesCount);
            writer.Write(Column);
            writer.Write(CharsCount);
            
            foreach(var obj in Content ?? Enumerable.Empty<object>())
            {
                switch (BuiltinType)
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
            
            writer.Write(ParentIndex);
        }
    }
}
