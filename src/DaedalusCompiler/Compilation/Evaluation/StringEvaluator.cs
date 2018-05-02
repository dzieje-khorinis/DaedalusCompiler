using System;
using System.Collections.Generic;
using System.Text;

namespace DaedalusCompiler.Compilation.Evaluation
{
    public class StringEvaluator : EvaluatorBase<string>
    {
        private const string message = "String constant must be assigned directly with string lteral. Expressions are not alowed.";

        protected override string BinaryNegate(string param)
        {
            throw new Exception(message);
        }

        protected override string BitAnd(string leftParam, string rightParam)
        {
            throw new Exception(message);
        }

        protected override string BitOr(string leftParam, string rightParam)
        {
            throw new Exception(message);
        }

        protected override string Divide(string leftParam, string rightParam)
        {
            throw new Exception(message);
        }

        protected override string Equal(string leftParam, string rightParam)
        {
            throw new Exception(message);
        }

        protected override string FromStringValue(string param)
        {
            return param;
        }

        protected override string Greater(string leftParam, string rightParam)
        {
            throw new Exception(message);
        }

        protected override string GreaterOrEqual(string leftParam, string rightParam)
        {
            throw new Exception(message);
        }

        protected override bool IsReferenceEvaluationAllowed()
        {
            return false;
        }

        protected override string Less(string leftParam, string rightParam)
        {
            throw new Exception(message);
        }

        protected override string LessOrEqual(string leftParam, string rightParam)
        {
            throw new Exception(message);
        }

        protected override string LogicAnd(string leftParam, string rightParam)
        {
            throw new Exception(message);
        }

        protected override string LogicOr(string leftParam, string rightParam)
        {
            throw new Exception(message);
        }

        protected override string Minus(string param)
        {
            throw new Exception(message);
        }

        protected override string Modulo(string leftParam, string rightParam)
        {
            throw new Exception(message);
        }

        protected override string Multiplicate(string leftParam, string rightParam)
        {
            throw new Exception(message);
        }

        protected override string Negate(string param)
        {
            throw new Exception(message);
        }

        protected override string NotEqual(string leftParam, string rightParam)
        {
            throw new Exception(message);
        }

        protected override string Plus(string param)
        {
            throw new Exception(message);
        }

        protected override string ShiftLeft(string leftParam, string rightParam)
        {
            throw new Exception(message);
        }

        protected override string ShiftRight(string leftParam, string rightParam)
        {
            throw new Exception(message);
        }

        protected override string Subtract(string leftParam, string rightParam)
        {
            throw new Exception(message);
        }

        protected override string Sum(string leftParam, string rightParam)
        {
            throw new Exception(message);
        }
    }
}
