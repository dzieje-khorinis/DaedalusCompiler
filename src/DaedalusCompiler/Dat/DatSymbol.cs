using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace DaedalusCompiler.Dat
{
    public enum DatSymbolType
    {
        Void = 0,
        Float = 1,
        Int = 2,
        String = 3,
        Class = 4,
        Func = 5,
        Prototype = 6,
        Instance = 7,
    }

    [Flags]
    public enum DatSymbolFlag
    {
        Const = 1,
        Return = 2,
        Classvar = 4,
        External = 8,
        Merged = 16,
    }

    [DebuggerDisplay("{Type} {ReturnType} {Name} '{Flags}'")]
    public class DatSymbol
    {
        /// <summary>
        /// Symbol name like C_MISSION.RUNNING, C_ITEM, MAX_WISPSKILL
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Length for array variables or constants. Set to 1 for non array variables or constants
        /// </summary>
        public uint ArrayLength { get; set; }

        /// <summary>
        /// Symbol type ex. 'class' or 'func'
        /// </summary>
        public DatSymbolType Type { get; set; }

        /// <summary>
        /// Symbol flags ex. 'const' or 'return'
        /// </summary>
        public DatSymbolFlag Flags { get; set; }

        /// <summary>
        /// Return type which is set only for 'func' symbols with 'return' flag set
        /// </summary>
        public DatSymbolType? ReturnType { get; set; }

        /// <summary>
        /// Addres of parent 'class' symbol set only for 'classvar' symbol.
        /// </summary>
        public int? ClassVarOffset { get; set; }

        /// <summary>
        /// Fields count which is set only for 'class' symbol
        /// </summary>
        public int? ClassSize { get; set; }

        /// <summary>
        /// Specifies where symbol is located in source scripts
        /// </summary>
        public DatSymbolLocation Location { get; set; }

        /// <summary>
        /// Content of const variable or array
        /// </summary>
        public object[] Content { get; set; }

        /// <summary>
        /// Reference to parent symbol for nested symbols like class variables
        /// </summary>
        public int Parent { get; set; }

        /// <summary>
        /// Saves DatSymbol to binary stream using DAT format 
        /// </summary>
        public void Save(DatBinaryWriter writer)
        {
            // Save name
            writer.Write(Convert.ToUInt32(Name != null));
            if (Name != null)
            {
                writer.Write(Name);
            }

            // Save ReturnType / ClassSize / ClassVarOffset
            if (Type == DatSymbolType.Func && Flags.HasFlag(DatSymbolFlag.Return))
            {
                writer.Write((int)ReturnType.Value);
            }
            else if (Type == DatSymbolType.Class)
            {
                writer.Write(ClassSize.Value);
            }
            else if (Flags.HasFlag(DatSymbolFlag.Classvar))
            {
                writer.Write(ClassVarOffset.Value);
            }
            else
            {
                writer.Write(0);
            }

            // Save ArrayLength & Type & Flags
            var bitField = 0u;
            bitField |= ArrayLength;
            bitField |= ((uint)Type << 12);
            bitField |= ((uint)Flags << 16);
            bitField |= 0x400000;
            writer.Write(bitField);

            // Save localization data
            writer.Write(Location.FileNumber);
            writer.Write(Location.Line);
            writer.Write(Location.LinesCount);
            writer.Write(Location.Position);
            writer.Write(Location.PositionsCount);

            //Save content
            foreach(var obj in Content ?? Enumerable.Empty<object>())
            {
                switch (Type)
                {
                    case DatSymbolType.String:
                        writer.Write((string)obj);
                        break;
                    case DatSymbolType.Float:
                        writer.Write((float)obj);
                        break;
                    default:
                        writer.Write((int)obj);
                        break;
                }
            }

            // Save parent
            writer.Write(Parent);
        }

        /// <summary>
        /// Loads DatSymbol from binary DAT formatted stream
        /// </summary>
        public static DatSymbol Load(DatBinaryReader reader)
        {
            var symbol = new DatSymbol();

            // Read Name
            var hasName = Convert.ToBoolean(reader.ReadUInt32());
            if (hasName)
            {
                symbol.Name = reader.ReadString();
            }

            // Read ReturnType / ClassSize / ClassVarOffset / ArrayLength / Type / Flags
            var valueField = reader.ReadInt32();
            var bitField = reader.ReadUInt32();

            symbol.ArrayLength = bitField & 0xFFF;
            symbol.Type = (DatSymbolType)((bitField & 0xF000) >> 12);
            symbol.Flags = (DatSymbolFlag)((bitField & 0x3F0000) >> 16);

            if (symbol.Type == DatSymbolType.Func && symbol.Flags.HasFlag(DatSymbolFlag.Return))
            {
                symbol.ReturnType = (DatSymbolType)valueField;
            }

            if (symbol.Type == DatSymbolType.Class)
            {
                symbol.ClassSize = valueField;
            }

            if (symbol.Flags.HasFlag(DatSymbolFlag.Classvar))
            {
                symbol.ClassVarOffset = valueField;
            }

            // Read Location
            symbol.Location = new DatSymbolLocation
            {
                FileNumber = reader.ReadInt32(),
                Line = reader.ReadInt32(),
                LinesCount = reader.ReadInt32(),
                Position = reader.ReadInt32(),
                PositionsCount = reader.ReadInt32(),
            };


            // Read Content (if exists)
            symbol.Content = GetContentIfExists(reader, symbol);

            // Read Parent
            symbol.Parent = reader.ReadInt32();

            return symbol;
        }

        private static object[] GetContentIfExists(DatBinaryReader reader, DatSymbol symbol)
        {
            // TODO : Verify and refactor this method.

            object[] result = null;

            if (symbol.Flags.HasFlag(DatSymbolFlag.Classvar) == false)
            {
                if (symbol.Type == DatSymbolType.Func || symbol.Type == DatSymbolType.Class || symbol.Type == DatSymbolType.Prototype)
                {
                    result = new object[1];
                }
                else
                {
                    result = new object[symbol.ArrayLength];
                }

                if ((result.Length == 0) && (symbol.Type == DatSymbolType.Instance))
                {
                    result = new object[1];
                }

                for (int i = 0; i < result.Length; i++)
                {
                    switch (symbol.Type)
                    {
                        case DatSymbolType.String:
                            result[i] = reader.ReadString();
                            break;
                        case DatSymbolType.Float:
                            result[i] = reader.ReadSingle();
                            break;
                        default:
                            result[i] = reader.ReadInt32();
                            break;
                    }
                }
            }

            return result;
        }
    }
}
