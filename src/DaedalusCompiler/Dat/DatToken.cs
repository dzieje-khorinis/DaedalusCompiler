using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DaedalusCompiler.Dat
{
    public enum DatTokenType
    {
        Add = 0,
        Subract = 1,
        Multiply = 2,
        Divide = 3,
        Mod = 4,
        BinOr = 5,
        BinAnd = 6,
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

    [DebuggerDisplay("{TokenType} {Address} {Symbol} {Value} {Index}")]
    public class DatToken
    {
        public static DatToken LoadToken(BinaryFileStream stream)
        {
            var tokenByte = stream.ReadByte();

            if (Enum.IsDefined(typeof(DatTokenType), tokenByte) == false)
                throw new Exception($"Unable to parse DatToken with id = {tokenByte}");

            var token = new DatToken
            {
                TokenType = (DatTokenType)tokenByte
            };

            switch (token.TokenType)
            {
                case DatTokenType.Call:
                    token.Address = stream.ReadInt();
                    break;

                case DatTokenType.CallExternal:
                    token.Symbol = stream.ReadInt();
                    break;

                case DatTokenType.PushInt:
                    token.Value = stream.ReadInt();
                    break;

                case DatTokenType.PushVar:
                    token.Symbol = stream.ReadInt();
                    break;

                case DatTokenType.PushInstance:
                    token.Symbol = stream.ReadInt();
                    break;

                case DatTokenType.Jump:
                    token.Address = stream.ReadInt();
                    break;

                case DatTokenType.JumpIf:
                    token.Address = stream.ReadInt();
                    break;

                case DatTokenType.SetInstance:
                    token.Symbol = stream.ReadInt();
                    break;

                case DatTokenType.PushArrayVar:
                    token.Symbol = stream.ReadInt();
                    token.Index = stream.ReadByte();
                    break;
            }

            return token;
        }

        public DatTokenType TokenType { get; private set; }

        public int Size { get { return 1 + (Address.HasValue ? 4 : 0) + (Symbol.HasValue ? 4 : 0) + (Value.HasValue ? 4 : 0) + (Index.HasValue ? 1 : 0); } }

        public int? Address { get; private set; }

        public int? Symbol { get; private set; }

        public int? Value { get; private set; }

        public int? Index { get; private set; }
    }
}
