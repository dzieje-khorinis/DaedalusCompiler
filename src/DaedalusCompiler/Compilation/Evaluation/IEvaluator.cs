using System;
using System.Collections.Generic;
using System.Text;

namespace DaedalusCompiler.Compilation.Evaluation
{
    public interface IEvaluator
    {
        object EvaluateConst(DaedalusParser.ExpressionContext expression, AssemblyBuilder assemblyBuilder);
    }
}
