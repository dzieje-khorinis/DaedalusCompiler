using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    public class AnnotationsAdditionVisitor : AbstractSyntaxTreeBaseVisitor
    {
        protected override void VisitConstDefinition(ConstDefinitionNode node)
        {
            
        }

        protected override void VisitBinaryExpression(BinaryExpressionNode binaryExpressionNode)
        {
            DatSymbolType leftType = binaryExpressionNode.LeftSideNode.BuiltinType;
            DatSymbolType rightType = binaryExpressionNode.RightSideNode.BuiltinType;
            
            if ((leftType == DatSymbolType.Int || leftType == DatSymbolType.Instance)
                && (rightType == DatSymbolType.Int || rightType == DatSymbolType.Instance))
            {
                
                /*
                 // TODO: original compiler didn't allow to have lhs or rhs to be functionCall of type Instance,
                 
                if ((leftType == DatSymbolType.Instance && binaryExpressionNode.LeftSideNode is FunctionCallNode)
                    || (rightType == DatSymbolType.Instance && binaryExpressionNode.RightSideNode is FunctionCallNode))
                {
                    binaryExpressionNode.BuiltinType = DatSymbolType.Undefined;
                }
                */
                binaryExpressionNode.BuiltinType = DatSymbolType.Int;
            }
            else
            {
                binaryExpressionNode.BuiltinType = DatSymbolType.Undefined;
            }
        }

        protected override void VisitUnaryExpression(UnaryExpressionNode node)
        {
            //if (node.)
            
            
            /*
             *
             switch (lValueType)
            {
                case DatSymbolType.Int:
                case DatSymbolType.Instance:
                    if (rValue.Type == DatSymbolType.Int)
                    {
                        return Compability.Full;
                    }
                    
                    if (rValue.Type == DatSymbolType.Instance && rValue.Category == DataCategory.Variable)
                    {
                        // TODO, check if this is worth supporting
                        return Compability.Full;
                    }
                    
                    break;
            }
            
            return Compability.None;
             */
        }
    }
}