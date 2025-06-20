using System;
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
        private readonly Stream _output;
        private readonly Encoding _encoding;

        public DatBinaryWriter(Stream output, Encoding encoding)
        {
            _output = output;
            _encoding = encoding;
        }

        public void Write(byte[] value)
        {
            _output.Write(value, 0, value.Length);
        }

        public void Write(byte value)
        {
            _output.WriteByte(value);
        }

        public void Write(char value)
        {
            _output.WriteByte((byte)value);
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
            var bytes = _encoding.GetBytes(value).Concat(new byte[] { 0x0A }).ToArray();

            // handling for special char FF
            if (bytes[0] == 0x79)
                bytes[0] = 0xFF;

            Write(bytes);
        }
    }
}
