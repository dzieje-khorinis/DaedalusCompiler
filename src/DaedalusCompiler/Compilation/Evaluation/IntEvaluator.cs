using System;
using System.Collections.Generic;
using System.Text;

namespace DaedalusCompiler.Compilation.Evaluation
{
    public class IntEvaluator : EvaluatorBase<int>
    {
        protected override int BinaryNegate(int param)
        {
            return ~param;
        }

        protected override int BitAnd(int leftParam, int rightParam)
        {
            return leftParam & rightParam;
        }

        protected override int BitOr(int leftParam, int rightParam)
        {
            return leftParam | rightParam;
        }

        protected override int Divide(int leftParam, int rightParam)
        {
            return leftParam / rightParam;
        }

        protected override int Equal(int leftParam, int rightParam)
        {
            return leftParam == rightParam ? 1 : 0;
        }

        protected override int FromStringValue(string param)
        {
            return int.Parse(param);
        }

        protected override int Greater(int leftParam, int rightParam)
        {
            return leftParam > rightParam ? 1 : 0;
        }

        protected override int GreaterOrEqual(int leftParam, int rightParam)
        {
            return leftParam >= rightParam ? 1 : 0;
        }

        protected override bool IsReferenceEvaluationAllowed()
        {
            return true;
        }

        protected override int Less(int leftParam, int rightParam)
        {
            return leftParam < rightParam ? 1 : 0;
        }

        protected override int LessOrEqual(int leftParam, int rightParam)
        {
            return leftParam <= rightParam ? 1 : 0;
        }

        protected override int LogicAnd(int leftParam, int rightParam)
        {
            return ((leftParam == 0 ? false : true) && (rightParam == 0 ? false : true)) ? 1 : 0;
        }

        protected override int LogicOr(int leftParam, int rightParam)
        {
            return ((leftParam == 0 ? false : true) || (rightParam == 0 ? false : true)) ? 1 : 0;
        }

        protected override int Minus(int param)
        {
            return -param;
        }

        protected override int Modulo(int leftParam, int rightParam)
        {
            return leftParam % rightParam;
        }

        protected override int Multiplicate(int leftParam, int rightParam)
        {
            return leftParam * rightParam;
        }

        protected override int Negate(int param)
        {
            return param == 0 ? 1 : 0;
        }

        protected override int NotEqual(int leftParam, int rightParam)
        {
            return leftParam != rightParam ? 1 : 0;
        }

        protected override int Plus(int param)
        {
            return +param;
        }

        protected override int ShiftLeft(int leftParam, int rightParam)
        {
            return leftParam << rightParam;
        }

        protected override int ShiftRight(int leftParam, int rightParam)
        {
            return leftParam >> rightParam;
        }

        protected override int Subtract(int leftParam, int rightParam)
        {
            return leftParam - rightParam;
        }

        protected override int Sum(int leftParam, int rightParam)
        {
            return leftParam + rightParam;
        }
    }
}
