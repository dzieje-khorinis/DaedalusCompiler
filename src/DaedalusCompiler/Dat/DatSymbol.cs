using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        /// ??? Some kind of reference to parent symbol for nested symbols like class variables
        /// </summary>
        public int Parent { get; set; }

        /// <summary>
        /// Loads DatSymbol from binary DAT formatted stream
        /// </summary>
        public static DatSymbol Load(BinaryFileStream stream)
        {
            var symbol = new DatSymbol();

            // Read symbol name
            var hasName = Convert.ToBoolean(stream.ReadUInt());
            if (hasName)
            {
                symbol.Name = stream.ReadString();
            }

            // Read symbol properties
            var offset = stream.ReadInt();
            var bitField = stream.ReadUInt();

            symbol.ArrayLength = bitField & 0xFFF;
            symbol.Type = (DatSymbolType)((bitField & 0xF000) >> 12);
            symbol.Flags = (DatSymbolFlag)((bitField & 0x3F0000) >> 16);

            if (symbol.Type == DatSymbolType.Func && symbol.Flags.HasFlag(DatSymbolFlag.Return))
            {
                symbol.ReturnType = (DatSymbolType)offset;
            }

            if (symbol.Type == DatSymbolType.Class)
            {
                symbol.ClassSize = offset;
            }

            if (symbol.Flags.HasFlag(DatSymbolFlag.Classvar))
            {
                symbol.ClassVarOffset = offset;
            }

            // Read symbol localization data
            symbol.Location = new DatSymbolLocation
            {
                FileNumber = stream.ReadInt(),
                Line = stream.ReadInt(),
                LinesCount = stream.ReadInt(),
                Position = stream.ReadInt(),
                PositionsCount = stream.ReadInt(),
            };


            // Read symbol content if exists
            symbol.Content = GetContentIfExists(stream, symbol);

            // Read symbol parent
            symbol.Parent = stream.ReadInt();

            return symbol;            
        }

        private static object[] GetContentIfExists(BinaryFileStream stream, DatSymbol symbol)
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
                            result[i] = stream.ReadString();
                            break;
                        case DatSymbolType.Float:
                            result[i] = stream.ReadFloat();
                            break;
                        default:
                            result[i] = stream.ReadInt();
                            break;
                    }
                }
            }

            return result;
        }
    }
}
