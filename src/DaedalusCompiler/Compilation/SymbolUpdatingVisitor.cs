using System;
using DaedalusCompiler.Compilation.SemanticAnalysis;

namespace DaedalusCompiler.Compilation
{
    public class SymbolUpdatingVisitor : AbstractSyntaxTreeBaseVisitor
    {
        private object GetValue(NodeValue value)
        {
            switch (value)
            {
                case IntValue intValue:
                    return intValue.Value;
                case FloatValue floatValue:
                    return floatValue.Value;
                case StringValue stringValue:
                    return stringValue.Value;
                case FunctionValue functionValue:
                    return functionValue.Value;
            }
            throw new Exception();
        }


        protected override void VisitParameterDeclaration(ParameterDeclarationNode node)
        {
            base.VisitParameterDeclaration(node);
        }

        protected override void VisitConstDefinition(ConstDefinitionNode node)
        {
            object value = GetValue(node.RightSideValue);

            if (value is Symbol symbol) // TODO is this needed?
            {
                value = symbol.Index;
            }
            
            switch (node.Symbol.BuiltinType)
            {
                case SymbolType.Int:
                    value = Convert.ToInt32(value);
                    break;
                case SymbolType.Float:
                    value = Convert.ToSingle(value);
                    break;
            }
            node.Symbol.Content = new[] {value};
        }

        protected override void VisitConstArrayDefinition(ConstArrayDefinitionNode node)
        {
            node.Symbol.Content = new object[node.ElementValues.Count];

            for (int i = 0; i < node.ElementValues.Count; ++i)
            {
                object value = GetValue(node.ElementValues[i]);
                
                switch (node.Symbol.BuiltinType)
                {
                    case SymbolType.Int:
                        value = Convert.ToInt32(value);
                        break;
                    case SymbolType.Float:
                        value = Convert.ToSingle(value);
                        break;
                }
                
                node.Symbol.Content[i] = value;
            }
        }
    }
}