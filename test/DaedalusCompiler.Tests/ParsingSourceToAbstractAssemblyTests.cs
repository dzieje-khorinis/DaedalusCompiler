using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using DaedalusCompiler.Compilation;
using DaedalusCompiler.Dat;
using Xunit;

namespace DaedalusCompiler.Tests
{
    public class ParsingSourceToAbstractAssemblyTests
    {
        private static readonly Dictionary<string, DatSymbolType> StringToDatSymbolTypeMap =
            new Dictionary<string, DatSymbolType>
            {
                {"void", DatSymbolType.Void},
                {"float", DatSymbolType.Float},
                {"int", DatSymbolType.Int},
                {"string", DatSymbolType.String},
                {"class", DatSymbolType.Class},
                {"func", DatSymbolType.Func},
                {"prototype", DatSymbolType.Prototype},
                {"instance", DatSymbolType.Instance},
            };

        private DatSymbol Var(string variable)
        {
            string[] typeAndName = variable.Split(' ');
            return SymbolBuilder.BuildVariable(typeAndName[1], StringToDatSymbolTypeMap[typeAndName[0]]);
        }

        private DatSymbol Func(string funcname)
        {
            return SymbolBuilder.BuildFunc(funcname, DatSymbolType.Func);
        }

        private List<AssemblyElement> ParseExpressions(string declarations, string expressions)
        {
            string data = $@"
                {declarations}
                func void testFunc()
                {{
                    {expressions}
                }};
            ";
            AntlrInputStream inputStream = new AntlrInputStream(data);
            DaedalusLexer lexer = new DaedalusLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            DaedalusParser parser = new DaedalusParser(commonTokenStream);


            AssemblyBuilder assemblyBuilder = new AssemblyBuilder();
            ParseTreeWalker.Default.Walk(new DaedalusParserListener(assemblyBuilder, 0), parser.daedalusFile());
            return assemblyBuilder.functions.Find(func => func.symbol.Name == "testFunc").body;
        }

        private void CompareInstructionLists(
            List<AssemblyElement> instructions,
            List<AssemblyElement> expectedInstructions)
        {
            for (var index = 0; index < expectedInstructions.Count; index++)
            {
                var instruction = instructions[index];
                var expectedInstruction = expectedInstructions[index];
                CompareInstructions(instruction, expectedInstruction);
            }
        }

        private void CompareInstructions(AssemblyElement instruction, AssemblyElement expectedInstruction)
        {
            Assert.Equal(instruction.GetType(), expectedInstruction.GetType());
            switch (instruction)
            {
                case PushInt pushIntInstruction:
                {
                    Assert.Equal(
                        pushIntInstruction.value,
                        ((PushInt) expectedInstruction).value
                    );
                    break;
                }
                case Call _:
                case CallExternal _:
                case PushVar _:
                {
                    Assert.Equal(
                        ((SymbolInstruction) instruction).symbol.Type,
                        ((SymbolInstruction) expectedInstruction).symbol.Type
                    );
                    Assert.Equal(
                        ((SymbolInstruction) instruction).symbol.Name,
                        ((SymbolInstruction) expectedInstruction).symbol.Name
                    );
                    break;
                }
                case PushArrVar pushArrVarInstruction:
                    Assert.Equal(
                        pushArrVarInstruction.symbol.Type,
                        ((SymbolInstruction) expectedInstruction).symbol.Type
                    );
                    Assert.Equal(
                        pushArrVarInstruction.symbol.Name,
                        ((SymbolInstruction) expectedInstruction).symbol.Name
                    );
                    Assert.Equal(
                        pushArrVarInstruction.index,
                        ((PushArrVar) expectedInstruction).index
                    );
                    break;
            }
        }

        [Fact]
        public void CorrectlyCreateFunction()
        {
            var builder = new AssemblyBuilder();
            var symbol = SymbolBuilder.BuildFunc("test", DatSymbolType.Int);

            builder.execBlockStart(symbol, ExecutebleBlockType.Function);
            builder.execBlockEnd();
            Assert.Equal(1, builder.functions.Count);
        }

        [Fact]
        public void SimpleExpressionAssignPlusMultiply()
        {
            string declarations = "var int x;";
            string expressions = "x = 12 + 9 * 20;";
            List<AssemblyElement> instructions = ParseExpressions(declarations, expressions);

            List<AssemblyElement> expectedInstructions = new List<AssemblyElement>
            {
                new PushInt(20),
                new PushInt(9),
                new Multiply(),
                new PushInt(12),
                new Add(),
                new PushVar(Var("int x")),
                new Assign(),
            };

            CompareInstructionLists(instructions, expectedInstructions);
        }

        [Fact]
        public void ComplexExpression()
        {
            string declarations = @"
                func int otherFunc(var int a, var int b)
                {
                    return 0;
                };
                var int x;
            ";
            string expressions = "x = 12 + 9 * ( 2 + otherFunc(1 + 7 * 3, 4 + 5) );";
            List<AssemblyElement> instructions = ParseExpressions(declarations, expressions);

            List<AssemblyElement> expectedInstructions = new List<AssemblyElement>
            {
                new PushInt(3),
                new PushInt(7),
                new Multiply(),
                new PushInt(1),
                new Add(),
                new PushInt(5),
                new PushInt(4),
                new Add(),
                new Call(Func("otherFunc")),
                new PushInt(2),
                new Add(),
                new PushInt(9),
                new Multiply(),
                new PushInt(12),
                new Add(),
                new PushVar(Var("int x")),
                new Assign(),
            };

            CompareInstructionLists(instructions, expectedInstructions);
        }
    }
}