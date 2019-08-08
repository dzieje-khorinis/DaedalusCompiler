namespace DaedalusCompiler.Compilation.SemanticAnalysis
{
    public abstract class CompilationMessage
    {
        public string Content;
    }

    public class CompilationWarning : CompilationMessage
    {
    }

    public class CompilationError : CompilationMessage
    {
    }

    
public class ErrorCollectionVisitor : AbstractSyntaxTreeBaseVisitor
    {
        public FileNode CurrentFileNode;
        public ASTNode CurrentBlockNode; //function, instance, prototype, class
        
        public ErrorCollectionVisitor()
        {
            CurrentFileNode = null;
        }

        protected override void Visit(ASTNode node)
        {
            
            base.Visit(node);
        }

        protected override void VisitFile(FileNode node)
        {
            CurrentFileNode = node;
            base.VisitFile(node);
        }

        protected override void VisitFunctionDefinition(FunctionDefinitionNode node)
        {
            CurrentBlockNode = node;
            base.VisitFunctionDefinition(node);
        }

        protected override void VisitClassDefinition(ClassDefinitionNode node)
        {
            CurrentBlockNode = node;
            base.VisitClassDefinition(node);
        }

        protected override void VisitPrototypeDefinition(PrototypeDefinitionNode node)
        {
            CurrentBlockNode = node;
            base.VisitPrototypeDefinition(node);
        }

        protected override void VisitInstanceDefinition(InstanceDefinitionNode node)
        {
            CurrentBlockNode = node;
            base.VisitInstanceDefinition(node);
        }
    }
}