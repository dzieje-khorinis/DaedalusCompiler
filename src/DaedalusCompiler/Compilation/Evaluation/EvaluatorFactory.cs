using DaedalusCompiler.Dat;
using System;
using System.Collections.Generic;
using System.Text;

namespace DaedalusCompiler.Compilation.Evaluation
{
    public static class EvaluatorFactory
    {
        private static Dictionary<DatSymbolType, IEvaluator> evaluators = new Dictionary<DatSymbolType, IEvaluator>()
        {
            { DatSymbolType.Int, new IntEvaluator() },
            { DatSymbolType.String, new StringEvaluator() },
            { DatSymbolType.Float, new FloatEvaluator() },
        };

        public static IEvaluator GetEvaluator(DatSymbolType symbolType)
        {
            if (evaluators.ContainsKey(symbolType) == false)
                throw new Exception($"Unable to evaluate values of type {symbolType}");

            return evaluators[symbolType];
        }
    }
}
