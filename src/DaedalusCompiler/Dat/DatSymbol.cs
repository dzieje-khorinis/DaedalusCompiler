using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DaedalusCompiler.Dat
{
    public enum DatSymbolType
    {
        Void = 0x0000,
        Float = 0x1000,
        Int = 0x2000,
        String = 0x3000,
        Class = 0x4000,
        Func = 0x5000,
        Prototype = 0x6000,
        Instance = 0x7000
    }

    [Flags]
    public enum DatSymbolFlag
    {
        Const = 0x10000,
        Return = 0x20000,
        Classvar = 0x40000,
        External = 0x80000,
        Merged = 0x100000,
    }

    [DebuggerDisplay("{Type} {name} '{Flags}'")]
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
        /// Symbol type and flags encoded in bits 
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
        /// Content of const variable or array
        /// </summary>
        private object[] content;

        /// <summary>
        /// ??? Some kind of reference to parent symbol for nested symbols like class variables
        /// </summary>
        private int parent;

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
            content = GetContentIfExists(stream);
            parent = stream.ReadInt();
        }

        public DatSymbolType Type
        {
            get { return (DatSymbolType)(bitField & 0xF000); }
        }

        public DatSymbolFlag Flags
        {
            get { return (DatSymbolFlag)(bitField & 0x3F0000); }
        }

        public bool IsLocal
        {
            get { return name.Contains("."); }
        }

        private object[] GetContentIfExists(BinaryFileStream stream)
        {
            object[] result = null;

            if (Flags.HasFlag(DatSymbolFlag.Classvar) == false)
            {
                if (Type == DatSymbolType.Func || Type == DatSymbolType.Class || Type == DatSymbolType.Prototype)
                {
                    result = new object[1];
                }
                else
                {
                    result = new object[GetContentElementsCount()];
                }

                if ((result.Length == 0) && (Type == DatSymbolType.Instance))
                {
                    result = new object[1];
                }

                for (int i = 0; i < result.Length; i++)
                {
                    switch (Type)
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

        private int GetContentElementsCount()
        {
            return bitField & 0xFFF;
        }
    }
}
