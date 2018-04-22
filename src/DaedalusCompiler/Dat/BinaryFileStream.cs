using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;

namespace DaedalusCompiler.Dat
{
    /// <summary>
    /// Provides methods to load and convert bytes from binary file
    /// </summary>
    public class BinaryFileStream : FileStream
    {
        public BinaryFileStream(string path, FileMode mode) : base(path, mode)
        {
        }

        public byte[] ReadBytes(int count)
        {
            byte[] array = new byte[count];
            this.Read(array, 0, count);
            return array;
        }

        public int ReadInt()
        {
            var bytes = ReadBytes(4);
            return BitConverter.ToInt32(bytes, 0);            
        }

        public uint ReadUInt()
        {
            var bytes = ReadBytes(4);
            return BitConverter.ToUInt32(bytes, 0);
        }

        public float ReadFloat()
        {
            var bytes = ReadBytes(4);
            return BitConverter.Int32BitsToSingle(ReadInt());
        }

        public char ReadChar()
        {
            return (char)ReadByte();
        }

        public String ReadString()
        {
            var builder = new StringBuilder();
            var nextChar = ReadChar();
            while (nextChar != '\n')
            {
                builder.Append(nextChar);
                nextChar = ReadChar();
            }
            return builder.ToString();
        }
    }
}
