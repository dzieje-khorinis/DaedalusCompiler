using System.Collections.Generic;

namespace Common.SemanticAnalysis
{
    public interface ICommonDaedalusParseTreeVisitor
    {
        void Reset(int sourceFileNumber);
        List<ReferenceNode> ReferenceNodes { get; }
    }
}