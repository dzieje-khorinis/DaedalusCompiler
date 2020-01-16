using System.Collections.Generic;

namespace Commmon.SemanticAnalysis
{
    public interface ICommonDaedalusParseTreeVisitor
    {
        void Reset(int sourceFileNumber);
        List<ReferenceNode> ReferenceNodes { get; }
    }
}