using System;
using System.Linq;
using DaedalusCompiler.Compilation;

namespace DaedalusCompiler.Dat
{
    
    //[DebuggerDisplay("{IsFunction} {BuiltinType} {Name} '{Flags}'")]
    public class DatSymbol
    {
        public readonly bool IsFunction;
        public bool IsAddressable;
        
        public int Index;

        public readonly string Name;

        public readonly uint OffClsRet;  // Offset (ClassVar) | Size (Class) | ReturnType (Func)

        public readonly uint Count; // AttributesCount (Class) | ParametersCount (Function) | 0 (External function) | ArraySize (Var/Const) | 1 (Default)
        
        public readonly SymbolType BuiltinType;
        
        public readonly SymbolFlag Flags;

        public readonly uint FileIndex;
        public readonly uint Line;
        public readonly uint LinesCount;
        public readonly uint Column;
        public readonly uint CharsCount;
        
        public readonly object[] Content; // Offset (Class) | FirstTokenAddress (Callable) | Content (Const)

        public readonly int ParentIndex;
        
        /// <summary>
        /// Creates DatSymbol from Symbol
        /// </summary>
        public DatSymbol(Symbol symbol)
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
                
                case PrototypeSymbol _:
                    Count = 0;
                    break;
                
                case InstanceSymbol _:
                    Count = 0;
                    break;
                
                default:
                    Count = 1;
                    break;
            }

            BuiltinType = IsFunction ? SymbolType.Func : symbol.BuiltinType;
            if (symbol is ConstSymbol && symbol.BuiltinType == SymbolType.Func)
            {
                BuiltinType = SymbolType.Int;
            }
            
            Flags = symbol.Flags;

            if (!symbol.Path.StartsWith(".")) // .HELPER_INSTANCE symbol (used for nested attributes) is automatically created
            {
                FileIndex = (uint) symbol.Node.Location.FileIndex;
                Line = (uint) symbol.Node.Location.Line;
                LinesCount = (uint) symbol.Node.Location.LinesCount;
                Column = (uint) symbol.Node.Location.Column;
                CharsCount = (uint) symbol.Node.Location.CharsCount;
            }

            switch (symbol)
            {
                case ClassSymbol classSymbol:
                    Content = new object[] {classSymbol.Offset};
                    break;
                
                case BlockSymbol blockSymbol:
                    Content = new object[] {blockSymbol.FirstTokenAddress};
                    break;

                case AttributeSymbol _:
                    Content = new object[]{};
                    break;
                
                case StringConstSymbol stringConstSymbol:
                    Content = stringConstSymbol.Content;
                    break;
                
                case ConstSymbol constSymbol:
                    Content = constSymbol.Content;
                    break;
                
                case VarSymbol _:
                    Content = new object[Count];
                    for (int i = 0; i < Count; ++i)
                    {
                        switch (BuiltinType)
                        {
                            case SymbolType.String:
                                Content[i] = "";
                                break;
                            case SymbolType.Float:
                                Content[i] = 0.0f;
                                break;
                            default:
                                Content[i] = 0;
                                break;
                        }
                    }
                    break;

                case ExternalParameterSymbol externalParameterSymbol:
                    if (externalParameterSymbol.BuiltinType == SymbolType.Instance || externalParameterSymbol.BuiltinType == SymbolType.Func)
                    {
                        Content = new object[] {0};
                    }
                    else
                    {
                        Content = new object[]{};
                    }
                    break;

                case ParameterSymbol _:
                    switch (BuiltinType)
                    {
                        case SymbolType.String:
                            Content = new object[] {""};
                            break;
                        case SymbolType.Float:
                            Content = new object[] {0.0f};
                            break;
                        default:
                            Content = new object[] {0};
                            break;
                    }
                    break;
                
                default:
                    Content = new object[]{};
                    break;
                
            }

            ParentIndex = -1;
            switch (symbol)
            {
                case SubclassSymbol subclassSymbol:
                    if (subclassSymbol.Path.StartsWith(".")) // .HELPER_INSTANCE symbol (used for nested attributes) is automatically created
                    {
                        ParentIndex = -1;
                    }
                    else
                    {
                        ParentIndex = subclassSymbol.InheritanceParentSymbol.Index;
                    }
                    
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
                case ParameterSymbol parameterSymbol:
                    if (parameterSymbol.ComplexType is ClassSymbol classSymbol2)
                    {
                        ParentIndex = classSymbol2.Index;
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

            Content = new object[]{};
            
            if (!Flags.HasFlag(SymbolFlag.ClassVar))
            {
                switch (BuiltinType)
                {
                    case SymbolType.Class:
                    case SymbolType.Func:
                    case SymbolType.Instance:
                    case SymbolType.Prototype:
                        Content = new object[]{reader.ReadInt32()};
                        break;
                
                    default:
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
            }
            
            ParentIndex = reader.ReadInt32();
        }

        public DatSymbol()
        {
            Index = 0;
            Name = $"{(char) 255}INSTANCE_HELP";
            
            IsAddressable = false;
            IsFunction = false;

            OffClsRet = 0;
            Count = 1;
            BuiltinType = SymbolType.Instance;
            Flags = 0;
            Content = new object[]{0};

            FileIndex = 0;
            Line = 0;
            LinesCount = 0;
            Column = 0;
            CharsCount = 0;

            ParentIndex = -1;
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
