using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DaedalusCompiler.Dat
{
    /// <summary>
    /// Implements binary writing to DAT file stream
    /// </summary>
    public class DatBinaryWriter
    {
        private readonly Stream output;

        public DatBinaryWriter(Stream output)
        {
            this.output = output;
        }

        public void Write(byte[] value)
        {
            output.Write(value, 0, value.Length);
        }

        public void Write(byte value)
        {
            output.WriteByte(value);
        }

        public void Write(char value)
        {
            output.WriteByte((byte)value);
        }

        public void Write(int value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void Write(uint value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void Write(float value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void Write(string value)
        {
            var bytes = Encoding.ASCII.GetBytes(value).Concat(new byte[] { 0x0A }).ToArray();

            // handling for special char FF
            if (bytes[0] == 0x3F)
                bytes[0] = 0xFF;

            Write(bytes);
        }
    }
}
