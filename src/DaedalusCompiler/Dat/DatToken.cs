using System;
using System.Collections.Generic;
using System.Text;

namespace DaedalusCompiler.Dat
{
    public enum DatTokenType
    {
        Add = 0,
        Sub = 1,
        Mul = 2,
        Div = 3,
        Mod = 4,
        Or = 5,
        And = 6,
        Lwr = 7,
        Hgh = 8,
        Is = 9,
        Lor = 11, 
        Land = 12,
        Shl = 13,
        Shr = 14,
        Leq = 15,
        Eq = 16,
        Neq = 17,
        Heq = 18,
        Iadd = 19,
        Isub = 20,
        Imul = 21,
        Idiv = 22,
        Uplus = 30,
        Uminus = 31,
        Not = 32,
        Neg = 33,
        BracketOn = 40,
        BracketOff = 41,
        Semikolon = 42,
        Komma = 43,
        Tail = 44,
        None = 45,
        Float = 51,
        Var = 52,
        Operator = 53,
        Retn = 60,
        Call = 61,
        Callx = 62,
        Popi = 63,
        Pushi = 64,
        Pushv = 65,
        Pushs = 66,
        Pushin = 67,
        Pushid = 68,
        Popv = 68,
        Astr = 70,
        Astrp = 71,
        Afnc = 72,
        Aflt = 73,
        Ains = 74,
        Jmp = 75,
        Jmpf = 76,
        Sinst = 80,
        Skip = 90,
        Label = 91,
        Func = 92,
        FuncEnd = 93,
        Class = 94,
        ClassEnd = 95,
        Instance = 96,
        InstanceEnd = 96,
        NewString = 98,
        FlagArray = 180,
        Pusha = 245,
    }

    public class DatToken
    {
        private DatTokenType tokenType;

        public DatToken(BinaryFileStream stream)
        {
            tokenType = (DatTokenType)stream.ReadByte();
            Size = 1;

            int param = stream.ReadInt();
            Size += 4;

            // TODO : complete token deserialization
        }

        public int Size { get; private set; }
    }

}
