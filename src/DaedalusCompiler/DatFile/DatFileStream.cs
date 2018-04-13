using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaedalusCompiler.DatFile
{
    public class DatFileStream
    {
        private byte[] bytes;
        private int offset;

        public DatFileStream(byte[] bytes)
        {
            this.bytes = bytes;
        }
        
        public int ReadByte()
        {
            int b = bytes[(offset++)];
            return b < 0 ? b + 256 : b;
        }

        public byte[] ReadBytes(int len)
        {
            byte[] ret = new byte[len];
            Array.Copy(bytes, offset, ret, 0, len);
            return ret;
        }

        public int ReadInt()
        {
            return ReadByte() + (ReadByte() << 8) + (ReadByte() << 16) + (ReadByte() << 24);
        }

        public float ReadFloat()
        {
            return BitConverter.Int32BitsToSingle(ReadInt());
        }

        public char ReadChar()
        {
            return (char)ReadByte();
        }

        public String ReadString()
        {
            int s = offset;
            while ((char)bytes[(offset++)] != '\n') { }
                        
            return Encoding.ASCII.GetString(bytes.Skip(s).Take(offset - s).ToArray()); 
        }
    }
}
