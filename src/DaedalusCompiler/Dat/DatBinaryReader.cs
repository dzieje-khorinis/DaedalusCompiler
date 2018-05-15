using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;

namespace DaedalusCompiler.Dat
{
    /// <summary>
    /// Implements binary reading from DAT file stream
    /// </summary>
    public class DatBinaryReader
    {
        private readonly Stream input;

        public DatBinaryReader(Stream input)
        {
            this.input = input;
        }

        public byte ReadByte()
        {
            return (byte)input.ReadByte();
        }

        public byte[] ReadBytes(int count)
        {
            byte[] array = new byte[count];
            input.Read(array, 0, count);
            return array;
        }

        public char ReadChar()
        {
            return (char)input.ReadByte();
        }

        public int ReadInt32()
        {
            return BitConverter.ToInt32(ReadBytes(4), 0);
        }

        public uint ReadUInt32()
        {
            return BitConverter.ToUInt32(ReadBytes(4), 0);
        }

        public float ReadSingle()
        {
            return BitConverter.ToSingle(ReadBytes(4), 0);
        }

        public string ReadString()
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
