using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DaedalusCompiler.Dat
{
    [DebuggerDisplay("Name = {name}")]
    public class DatSymbol_OLD
    {
        public static readonly int bitfield_ele = 4095;
        public static readonly int bitfield_type = 61440;
        public static readonly int bitfield_flags = 4128768;

        public static int cID = 0;

        public DatFileDef_OLD thedat;
        public int id;
        public int b_hasName;
        public String name;
        public String nameLo;
        public String nameGl;
        public int offset;
        public int bitfield;
        public int filenr;
        public int line;
        public int line_anz;
        public int pos_beg;
        public int pos_anz;
        public int parent;
        public String parentS;
        public Object[] content;
        public bool isLocal;

        public DatSymbol_OLD(DatFileDef_OLD dat, BinaryFileStream s)
        {
            thedat = dat;

            id = (cID++);
            b_hasName = s.ReadInt();
            name = s.ReadString();
            nameLo = name.ToLower();
            offset = s.ReadInt();
            bitfield = s.ReadInt();
            filenr = s.ReadInt();
            line = s.ReadInt();
            line_anz = s.ReadInt();
            pos_beg = s.ReadInt();
            pos_anz = s.ReadInt();

            isLocal = name.Contains(".");

            if ((Flags() & 0x40000) == 0)
            {
                int type = Type();
                if ((type == 0x5000) || (type == 0x4000) || (type == 0x6000))
                {
                    content = new Object[1];
                }
                else
                {
                    content = new Object[Ele()];
                }
                if ((content.Length == 0) && (type == 0x7000))
                {
                    content = new Object[1];
                }
                for (int i = 0; i < content.Length; i++)
                {
                    switch (type)
                    {
                        case 0x3000:
                            content[i] = s.ReadString();
                            break;
                        case 0x1000:
                            content[i] = s.ReadFloat();
                            break;
                        default:
                            content[i] = s.ReadInt();
                            break;
                    }

                }
            }
            parent = s.ReadInt();
        }

        public int Flags()
        {
            return bitfield & 0x3F0000;
        }

        public int Type()
        {
            return bitfield & 0xF000;
        }

        public int Ele()
        {
            return bitfield & 0xFFF;
        }

        public bool HasFunction()
        {
            return ((HasFlags(0x1000)) && (Type() == 0x5000)) || (Type() == 0x7000) || (Type() == 0x6000);
        }

        public bool HasFlags(int v)
        {
            return (Flags() & v) > 0;
        }
    }
}
