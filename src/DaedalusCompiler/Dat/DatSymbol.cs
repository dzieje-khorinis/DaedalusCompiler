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
        Instance = 7
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

    [DebuggerDisplay("{SymbolType} {ReturnType} {name} '{Flags}'")]
    public class DatSymbol
    {
        /// <summary>
        /// ??? almost always is set to 1
        /// </summary>
        public int bHasName;

        /// <summary>
        /// Symbol name
        /// </summary>
        public string name;

        /// <summary>
        /// ??? Contains information about return type
        /// </summary>
        public int offset;

        /// <summary>
        /// Symbol type, flags, content size encoded in bits 
        /// </summary>
        public int bitField;

        /// <summary>
        /// Zero based source file number
        /// </summary>
        public int fileNumber;

        /// <summary>
        /// Line number in source file where symbol is located
        /// </summary>
        public int line;

        /// <summary>
        /// Number of lines (count) in source file taken by symbol definition
        /// </summary>
        public int linesCount;

        /// <summary>
        /// Char position from begining of source file where symbol is located
        /// </summary>
        public int positionBegin;

        /// <summary>
        /// Number of characters to next symbol
        /// </summary>
        public int positionsCount;

        /// <summary>
        /// Content of const variable or array
        /// </summary>
        public object[] content;

        /// <summary>
        /// ??? Some kind of reference to parent symbol for nested symbols like class variables
        /// </summary>
        private int parent;

        public DatSymbol(BinaryFileStream stream)
        {
            bHasName = stream.ReadInt();
            if (bHasName != 0)
            {
                name = stream.ReadString();   
            }
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

        public DatSymbol()
        {
            
        }

        public DatSymbolType SymbolType
        {
            get { return (DatSymbolType)((bitField & 0xF000) >> 12); }
        }

        public DatSymbolType ReturnType
        {
            get { return (DatSymbolType)offset; }
        }

        public DatSymbolFlag Flags
        {
            get { return (DatSymbolFlag)((bitField & 0x3F0000) >> 16); }
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
                if (SymbolType == DatSymbolType.Func || SymbolType == DatSymbolType.Class || SymbolType == DatSymbolType.Prototype)
                {
                    result = new object[1];
                }
                else
                {
                    result = new object[GetContentSize()];
                }

                if ((result.Length == 0) && (SymbolType == DatSymbolType.Instance))
                {
                    result = new object[1];
                }

                for (int i = 0; i < result.Length; i++)
                {
                    switch (SymbolType)
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

        private int GetContentSize()
        {
            return bitField & 0xFFF;
        }
    }
}
