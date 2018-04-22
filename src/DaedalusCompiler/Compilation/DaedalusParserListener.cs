using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime.Misc;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    public class DaedalusParserListener: DaedalusBaseListener
    {
        private AssemblyBuilder assemblyBuilder;

        public DaedalusParserListener(AssemblyBuilder assemblyBuilder)
        {
            this.assemblyBuilder = assemblyBuilder;
        }

        public override void ExitConstDef([NotNull] DaedalusParser.ConstDefContext context)
        {
            var name = context.nameNode()?.GetText();
            var typeName = context.typeReference()?.GetText();
            var type = DatSymbolTypeFromString(typeName);
            var value = context.constAssignment()?.expression()?.value()?.GetText(); // TODO: expression evaluation && convert value to specific type
            
            var symbol = SymbolBuilder.BuildConst(name, type.Value, value); // TODO : Validate params
            assemblyBuilder.addSymbol(symbol);
        }

        public override void ExitVarDecl([NotNull] DaedalusParser.VarDeclContext context)
        {
            var typeName = context.typeReference()?.GetText();
            var type = DatSymbolTypeFromString(typeName);

            foreach(var nameContext in context.nameNode())
            {
                var name = nameContext.GetText();

                var symbol = SymbolBuilder.BuildVariable(name, type.Value); // TODO : Validate params
                assemblyBuilder.addSymbol(symbol);
            }
        }

        public override void ExitFunctionDef([NotNull] DaedalusParser.FunctionDefContext context)
        {
            var name = context.nameNode()?.GetText();
            var typeName = context.typeReference()?.GetText();
            var type = DatSymbolTypeFromString(typeName);

            var symbol = SymbolBuilder.BuildFunc(name, type.Value); // TODO : Validate params
            assemblyBuilder.addSymbol(symbol);
        }

        private DatSymbolType? DatSymbolTypeFromString(string typeReference)
        {
            if (String.IsNullOrWhiteSpace(typeReference))
                return null;

            // FirstCharToUpper 
            typeReference = typeReference.First().ToString().ToUpper() + typeReference.Substring(1).ToLower();

            DatSymbolType type;
            if (Enum.TryParse(typeReference, out type))
                return type;
            else
                return null;
        }
    }
}
