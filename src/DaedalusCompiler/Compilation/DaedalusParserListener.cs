using System;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    public class DaedalusParserListener: DaedalusBaseListener
    {
        private readonly AssemblyBuilder assemblyBuilder;
        private readonly int sourceFileNumber;

        public DaedalusParserListener(AssemblyBuilder assemblyBuilder, int sourceFileNumber)
        {
            this.assemblyBuilder = assemblyBuilder;
            this.sourceFileNumber = sourceFileNumber;
        }

        public override void ExitConstDef([NotNull] DaedalusParser.ConstDefContext context)
        {
            var typeName = context.typeReference().GetText();
            var type = DatSymbolTypeFromString(typeName);

            // TODO: Fix this solution to preserve declaration order (ex. const int arr[2] = { 1, 2 }, val = 1, arr2[a] = { 1, 2 }, val2 = 2;)
            foreach (var constValueContext in context.constValueDef())
            {
                var name = constValueContext.nameNode().GetText();
                var location = GetLocation(context);
                var value = constValueContext.constValueAssignment().expression().value().GetText();

                var symbol = SymbolBuilder.BuildConst(name, type.Value, value, location); // TODO : Validate params
                assemblyBuilder.addSymbol(symbol);
            }

            foreach (var constArrayContext in context.constArrayDef())
            {
                var name = constArrayContext.nameNode().GetText();
                var location = GetLocation(context);
                var sizeString = constArrayContext.simpleValue().GetText(); // TODO : Allow set array size by reference to constant
                var size = int.Parse(sizeString); // TODO : Validate array size an its assignment content size
                var content = constArrayContext.constArrayAssignment().expression() // TODO: expression evaluation && convert values to specific type
                    .Select(expr => expr.value().GetText())
                    .ToArray();              

                var symbol = SymbolBuilder.BuildArrOfConst(name, type.Value, content, location); // TODO : Validate params
                assemblyBuilder.addSymbol(symbol);
            }
        }

        public override void ExitVarDecl([NotNull] DaedalusParser.VarDeclContext context)
        {
            var typeName = context.typeReference().GetText();
            var type = DatSymbolTypeFromString(typeName);

            // TODO: Fix this solution to preserve declaration order (ex. var int arr[5], val, arr2[a], val2;)
            foreach (var varValueContext in context.varValueDecl())
            {
                var name = varValueContext.nameNode().GetText();
                var location = GetLocation(context);

                var symbol = SymbolBuilder.BuildVariable(name, type.Value, location); // TODO : Validate params
                assemblyBuilder.addSymbol(symbol);
            }

            foreach(var varArrayContext in context.varArrayDecl())
            {
                var name = varArrayContext.nameNode().GetText();
                var location = GetLocation(context);
                var sizeString = varArrayContext.simpleValue().GetText(); // TODO : Allow set array size by reference to constant
                var size = uint.Parse(sizeString);

                var symbol = SymbolBuilder.BuildArrOfVariables(name, type.Value, size); // TODO : Validate params
                assemblyBuilder.addSymbol(symbol);
            }
        }

        public override void EnterFunctionDef([NotNull] DaedalusParser.FunctionDefContext context)
        {
            var name = context.nameNode().GetText();
            var typeName = context.typeReference().GetText();
            var type = DatSymbolTypeFromString(typeName);

            var symbol = SymbolBuilder.BuildFunc(name, type.Value); // TODO : Validate params
            assemblyBuilder.addSymbol(symbol);
            assemblyBuilder.registerFunction(symbol);
        }
        
        public override void ExitFunctionDef([NotNull] DaedalusParser.FunctionDefContext context)
        {
            // we invoke functionEnd, thanks that ab will assign all instructions
            // to currently exited function
            assemblyBuilder.functionEnd();
        }

        public override void ExitReturnStatement([NotNull] DaedalusParser.ReturnStatementContext context)
        {
            assemblyBuilder.addInstruction(new Ret());
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

        private DatSymbolLocation GetLocation(ParserRuleContext context)
        {
            return new DatSymbolLocation
            {
                FileNumber = sourceFileNumber,
                Line = context.Start.Line,
                LinesCount = context.Stop.Line - context.Start.Line + 1,
                Position = context.Start.StartIndex,
                PositionsCount = context.Stop.StopIndex - context.Start.StartIndex + 1
            };
        }
    }
}
