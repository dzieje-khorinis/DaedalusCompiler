namespace Common.SemanticAnalysis
{
    /// <summary>
    /// Some Daedalus syntax shouldn't be allowed.
    /// We could make grammar not to allow it, then if somebody used that syntax, he would get error message generated by ANTLR4.
    ///
    /// Since ANTLR4-generated error messages aren't that great, we decided to let grammar allow some wrong syntax,
    /// then look for that wrong syntax in this Visitor and make error messages prettier than ANTLR4's default ones.
    /// </summary>
    public class RemainingSyntaxErrorsDetectionVisitor : AbstractSyntaxTreeBaseVisitor
    {
        protected override void VisitVarDeclaration(VarDeclarationNode node)
        {
            if (node.RightSideNode != null)
            {
                ASTNode ancestorNode = node.GetFirstSignificantAncestorNode();
                switch (ancestorNode)
                {
                    case PrototypeDefinitionNode _:
                    case InstanceDefinitionNode _:
                    case FunctionDefinitionNode _:
                        break;
                    
                    default:
                        node.Annotations.Add(new VarAssignmentNotAllowedHereError(node.Location));
                        break;

                }
            }
            //base.VisitVarDeclaration(node);
        }

        protected override void VisitVarArrayDeclaration(VarArrayDeclarationNode node)
        {
            if (node.ElementNodes != null)
            {
                ASTNode ancestorNode = node.GetFirstSignificantAncestorNode();
                switch (ancestorNode)
                {
                    case PrototypeDefinitionNode _:
                    case InstanceDefinitionNode _:
                    case FunctionDefinitionNode _:
                        break;
                    
                    default:
                        node.Annotations.Add(new VarAssignmentNotAllowedHereError(node.Location));
                        break;

                }
            }
            //base.VisitVarArrayDeclaration(node);
        }
    }
}