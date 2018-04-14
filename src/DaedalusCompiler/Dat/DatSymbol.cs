using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DaedalusCompiler.Dat
{
    public enum DatSymbolType
    {
        Void,
        Float,
        Int,
        String,
        Class,
        Func,
        Prototype,
        Instance,
        Unknown,
    }

    [Flags]
    public enum DatSymbolFlag : int
    {
        Const = 0x10000,
        Return = 0x20000,
        Classvar = 0x40000,
        External = 0x80000,
        Merged = 0x100000,
    }

    [DebuggerDisplay("Name = {name}")]
    public class DatSymbol
    {
        /// <summary>
        /// ??? almost always is set to 1
        /// </summary>
        private int bHasName;

        /// <summary>
        /// Symbol name
        /// </summary>
        private string name;

        /// <summary>
        /// ???
        /// </summary>
        private int offset;

        /// <summary>
        /// Field containis informations about symbol type
        /// </summary>
        private int bitField;

        /// <summary>
        /// Zero based source file number
        /// </summary>
        private int fileNumber;

        /// <summary>
        /// Line number in source file where symbol is located
        /// </summary>
        private int line;

        /// <summary>
        /// Number of lines (count) in source file taken by symbol definition
        /// </summary>
        private int linesCount;

        /// <summary>
        /// Char position from begining of source file where symbol is located
        /// </summary>
        private int positionBegin;

        /// <summary>
        /// Number of characters to next symbol
        /// </summary>
        private int positionsCount;

        /// <summary>
        /// ???
        /// </summary>
        private int parent;

        /// <summary>
        /// ???
        /// </summary>
        private object[] content;

        public DatSymbol(BinaryFileStream stream)
        {
            bHasName = stream.ReadInt();
            name = stream.ReadString();
            offset = stream.ReadInt();
            bitField = stream.ReadInt();
            fileNumber = stream.ReadInt();
            line = stream.ReadInt();
            linesCount = stream.ReadInt();
            positionBegin = stream.ReadInt();
            positionsCount = stream.ReadInt();

            if (Flags.HasFlag(DatSymbolFlag.Classvar) == false)
            {
                if(Type == DatSymbolType.Func || Type == DatSymbolType.Class || Type == DatSymbolType.Prototype)
                {
                    content = new object[1];
                }
                else
                {
                    content = new object[Ele];
                }

                if ((content.Length == 0) && (Type == DatSymbolType.Instance))
                {
                    content = new Object[1];
                }

                for (int i = 0; i < content.Length; i++)
                {
                    switch (Type)
                    {
                        case DatSymbolType.String:
                            content[i] = stream.ReadString();
                            break;
                        case DatSymbolType.Float:
                            content[i] = stream.ReadFloat();
                            break;
                        default:
                            content[i] = stream.ReadInt();
                            break;
                    }
                }
            }

            parent = stream.ReadInt();
        }

        public DatSymbolType Type
        {
            get { return GetTypeFromBitField(); }
        }

        public DatSymbolFlag Flags
        {
            get { return GetFlagsFromBitField(); }
        }

        public bool IsLocal
        {
            get { return name.Contains("."); }
        }

        public int Ele
        {
            get { return bitField & 0xFFF; }
        }

        private DatSymbolType GetTypeFromBitField()
        {
            var type = bitField & 0xF000;

            switch (type)
            {
                case 0x0:
                    return DatSymbolType.Void;
                case 0x1000:
                    return DatSymbolType.Float;
                case 0x2000:
                    return DatSymbolType.Int;
                case 0x3000:
                    return DatSymbolType.String;
                case 0x4000:
                    return DatSymbolType.Class;
                case 0x5000:
                    return DatSymbolType.Func;
                case 0x6000:
                    return DatSymbolType.Prototype;
                case 0x7000:
                    return DatSymbolType.Instance;
                default:
                    return DatSymbolType.Unknown;
            }
        }

        private DatSymbolFlag GetFlagsFromBitField()
        {
            return (DatSymbolFlag)(bitField & 0x3F0000);
        }
    }
}
