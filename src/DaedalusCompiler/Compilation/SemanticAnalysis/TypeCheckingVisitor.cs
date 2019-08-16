namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    public class TypeCheckingVisitor : AbstractSyntaxTreeBaseVisitor
    {
        protected override void VisitFunctionCall(FunctionCallNode node)
        {
            if (node.FunctionReferenceNode.Symbol != null)
            {
                FunctionDefinitionNode functionDefinitionNode = (FunctionDefinitionNode) node.FunctionReferenceNode.Symbol.Node;
                string identifier = functionDefinitionNode.NameNode.Value;
                int parametersCount = functionDefinitionNode.ParameterNodes.Count;
                int argumentsCount = node.ArgumentNodes.Count;
            
                if (parametersCount != argumentsCount)
                {
                    node.Annotations.Add(new ArgumentsCountDoesNotMatchError(identifier, parametersCount, argumentsCount, functionDefinitionNode.NameNode.Location));
                }
            }

            base.VisitFunctionCall(node);
        }
    }
}