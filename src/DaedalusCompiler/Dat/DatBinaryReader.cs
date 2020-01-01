using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
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
            /*
            List<byte> bytes = new List<byte>();


            bool isPrefixed = false;
            byte lastByte = ReadByte();
            
            if (lastByte == 255)
            {
                isPrefixed = true;
            }
            else
            {
                bytes.Add(lastByte);
            }


            while (true)
            {
                lastByte = ReadByte();
                if (lastByte == '\n')
                {
                    break;
                }
                bytes.Add(lastByte);
            }

            string result = Encoding.GetEncoding(1250).GetString(bytes.ToArray());
            if (isPrefixed)
            {
                result = $"{(char) 255}{result}";
            }
            
            return result;
            */

            
            List<byte> bytes = new List<byte>();
            var nextChar = ReadByte();
            bool isPrefixed = nextChar == 255;
   
            while (nextChar != '\n')
            {
                bytes.Add(nextChar);
                nextChar = ReadByte();
            }
            
            string result = Encoding.GetEncoding(1250).GetString(bytes.ToArray());
            if (isPrefixed)
            {
                result = (char) 255 + result.Substring(1);
            }
            
            return result;
        }
    }
}
