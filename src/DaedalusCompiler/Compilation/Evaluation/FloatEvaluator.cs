using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DaedalusCompiler.Compilation.Evaluation
{
    public class FloatEvaluator : EvaluatorBase<float>
    {
        private const string message = "Float constant must be assigned directly with float lteral. Expressions are not alowed.";

        protected override float BinaryNegate(float param)
        {
            throw new Exception(message);
        }

        protected override float BitAnd(float leftParam, float rightParam)
        {
            throw new Exception(message);
        }

        protected override float BitOr(float leftParam, float rightParam)
        {
            throw new Exception(message);
        }

        protected override float Divide(float leftParam, float rightParam)
        {
            throw new Exception(message);
        }

        protected override float Equal(float leftParam, float rightParam)
        {
            throw new Exception(message);
        }

        protected override float FromStringValue(string param)
        {
            return float.Parse(param, CultureInfo.InvariantCulture);
        }

        protected override float Greater(float leftParam, float rightParam)
        {
            throw new Exception(message);
        }

        protected override float GreaterOrEqual(float leftParam, float rightParam)
        {
            throw new Exception(message);
        }

        protected override bool IsReferenceEvaluationAllowed()
        {
            return false;
        }

        protected override float Less(float leftParam, float rightParam)
        {
            throw new Exception(message);
        }

        protected override float LessOrEqual(float leftParam, float rightParam)
        {
            throw new Exception(message);
        }

        protected override float LogicAnd(float leftParam, float rightParam)
        {
            throw new Exception(message);
        }

        protected override float LogicOr(float leftParam, float rightParam)
        {
            throw new Exception(message);
        }

        protected override float Minus(float param)
        {
            throw new Exception(message);
        }

        protected override float Modulo(float leftParam, float rightParam)
        {
            throw new Exception(message);
        }

        protected override float Multiplicate(float leftParam, float rightParam)
        {
            throw new Exception(message);
        }

        protected override float Negate(float param)
        {
            throw new Exception(message);
        }

        protected override float NotEqual(float leftParam, float rightParam)
        {
            throw new Exception(message);
        }

        protected override float Plus(float param)
        {
            throw new Exception(message);
        }

        protected override float ShiftLeft(float leftParam, float rightParam)
        {
            throw new Exception(message);
        }

        protected override float ShiftRight(float leftParam, float rightParam)
        {
            throw new Exception(message);
        }

        protected override float Subtract(float leftParam, float rightParam)
        {
            throw new Exception(message);
        }

        protected override float Sum(float leftParam, float rightParam)
        {
            throw new Exception(message);
        }
    }
}
