using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace DaedalusCompiler.Dat
{
    public enum DatTokenType : byte
    {
        Add = 0,
        Subtract = 1,
        Multiply = 2,
        Divide = 3,
        Modulo = 4,
        BitOr = 5,
        BitAnd = 6,
        Less = 7,
        Greater = 8,
        Assign = 9,
        LogOr = 11,
        LogAnd = 12,
        ShiftLeft = 13,
        ShiftRight = 14,
        LessOrEqual = 15,
        Equal = 16,
        NotEqual = 17,
        GreaterOrEqual = 18,
        AssignAdd = 19,
        AssignSubtract = 20,
        AssignMultiply = 21,
        AssignDivide = 22,
        Plus = 30,
        Minus = 31,
        Not = 32,
        Negate = 33,
        Ret = 60,
        Call = 61,
        CallExternal = 62,
        PushInt = 64,
        PushVar = 65,
        PushInstance = 67,
        AssignString = 70,
        AssignStringRef = 71,
        AssignFunc = 72,
        AssignFloat = 73,
        AssignInstance = 74,
        Jump = 75,
        JumpIf = 76,
        SetInstance = 80,
        PushArrayVar = 245,
    }

    [DebuggerDisplay("{TokenType} I:{IntParam} B:{ByteParam}")]
    public class DatToken
    {
        public static DatToken Load(DatBinaryReader reader)
        {
            var tokenByte = reader.ReadByte();

            if (Enum.IsDefined(typeof(DatTokenType), tokenByte) == false)
                throw new Exception($"Unable to parse DatToken with id = {tokenByte}");

            var token = new DatToken
            {
                TokenType = (DatTokenType)tokenByte
            };

            switch (token.TokenType)
            {
                case DatTokenType.Call:
                case DatTokenType.CallExternal:
                case DatTokenType.PushInt:
                case DatTokenType.PushVar:
                case DatTokenType.PushInstance:
                case DatTokenType.Jump:
                case DatTokenType.JumpIf:
                case DatTokenType.SetInstance:
                    token.IntParam = reader.ReadInt32();
                    break;

                case DatTokenType.PushArrayVar:
                    token.IntParam = reader.ReadInt32();
                    token.ByteParam = reader.ReadByte();
                    break;
            }

            return token;
        }

        public void Save(DatBinaryWriter writer)
        {
            writer.Write((byte)TokenType);

            if (IntParam.HasValue)
                writer.Write(IntParam.Value);

            if (ByteParam.HasValue)
                writer.Write(ByteParam.Value);
        }

        public DatTokenType TokenType { get; set; }

        public int Size { get { return 1 + (IntParam.HasValue ? 4 : 0) + (ByteParam.HasValue ? 1 : 0); } }

        public int? IntParam { get; set; }

        public byte? ByteParam { get; set; }
    }
}
