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

        private static DatSymbol Var(string variable)
        {
            string[] typeAndName = variable.Split(' ');
            return SymbolBuilder.BuildVariable(typeAndName[1], StringToDatSymbolTypeMap[typeAndName[0]]);
        }

        private static DatSymbol Func(string funcname)
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
        public void TestFunctionCreation()
        {
            var builder = new AssemblyBuilder();
            var symbol = SymbolBuilder.BuildFunc("test", DatSymbolType.Int);

            builder.execBlockStart(symbol, ExecutebleBlockType.Function);
            builder.execBlockEnd();
            Assert.Equal(1, builder.functions.Count);
        }

        [Fact]
        public void TestIntAddOperator()
        {
            string declarations = "var int x;";
            string expressions = @"
                x = 2 + 3 + 4 + 5;
                x = 2 - 3 - 4 - 5;
            ";
            List<AssemblyElement> instructions = ParseExpressions(declarations, expressions);

            List<AssemblyElement> expectedInstructions = new List<AssemblyElement>
            {
                // x = 2 + 3 + 4 + 5;
                new PushInt(5),
                new PushInt(4),
                new PushInt(3),
                new PushInt(2),
                new Add(),
                new Add(),
                new Add(),
                new PushVar(Var("int x")),
                new Assign(),

                // x = 2 - 3 - 4 - 5;
                new PushInt(5),
                new PushInt(4),
                new PushInt(3),
                new PushInt(2),
                new Subtract(),
                new Subtract(),
                new Subtract(),
                new PushVar(Var("int x")),
                new Assign(),
            };

            CompareInstructionLists(instructions, expectedInstructions);
        }

        [Fact]
        public void TestIntMultOperator()
        {
            string declarations = "var int x;";
            string expressions = @"
                x = 2 * 3 * 4 * 5;
                x = 2 / 3 / 4 / 5;
                x = 2 % 3 % 4 % 5;
            ";
            List<AssemblyElement> instructions = ParseExpressions(declarations, expressions);

            List<AssemblyElement> expectedInstructions = new List<AssemblyElement>
            {
                // x = 2 * 3 * 4 * 5;
                new PushInt(5),
                new PushInt(4),
                new PushInt(3),
                new PushInt(2),
                new Multiply(),
                new Multiply(),
                new Multiply(),
                new PushVar(Var("int x")),
                new Assign(),

                // x = 2 / 3 / 4 / 5;
                new PushInt(5),
                new PushInt(4),
                new PushInt(3),
                new PushInt(2),
                new Divide(),
                new Divide(),
                new Divide(),
                new PushVar(Var("int x")),
                new Assign(),

                // x = 2 / 3 / 4 / 5;
                new PushInt(5),
                new PushInt(4),
                new PushInt(3),
                new PushInt(2),
                new Modulo(),
                new Modulo(),
                new Modulo(),
                new PushVar(Var("int x")),
                new Assign(),
            };

            CompareInstructionLists(instructions, expectedInstructions);
        }

        [Fact]
        public void TestIntAssignmentOperator()
        {
            string declarations = "var int x;";
            string expressions = @"
                x = 1
                x += 2;
                x -= 3;
                x *= 4;
                x /= 5;
            ";
            List<AssemblyElement> instructions = ParseExpressions(declarations, expressions);

            List<AssemblyElement> expectedInstructions = new List<AssemblyElement>
            {
                // x = 1;
                new PushInt(1),
                new PushVar(Var("int x")),
                new Assign(),

                // x += 2;
                new PushInt(2),
                new PushVar(Var("int x")),
                new AssignAdd(),

                // x -= 3;
                new PushInt(3),
                new PushVar(Var("int x")),
                new AssignSubtract(),

                // x *= 4;
                new PushInt(4),
                new PushVar(Var("int x")),
                new AssignMultiply(),

                // x /= 5;
                new PushInt(5),
                new PushVar(Var("int x")),
                new AssignDivide(),
            };

            CompareInstructionLists(instructions, expectedInstructions);
        }

        [Fact]
        public void TestIntAddAndMultOperatorPrecedence()
        {
            string declarations = @"
                var int a;
                var int b;
                var int c;
                var int d;
            ";
            string expressions = @"
                a = 1 + 2 * 3;
                a += 1 + 2 / 3;

                b = 1 - 2 * 3;
                b -= 1 - 2 / 3;
                            
                c = 4 / (5 + 6) * 7;
                c *= 4 * (5 + 6) / 7;

                d = 4 / (5 - 6) * 7;
                d /= 4 * (5 - 6) / 7;
            ";
            List<AssemblyElement> instructions = ParseExpressions(declarations, expressions);

            List<AssemblyElement> expectedInstructions = new List<AssemblyElement>
            {
                // a = 1 + 2 * 3;
                new PushInt(3),
                new PushInt(2),
                new Multiply(),
                new PushInt(1),
                new Add(),
                new PushVar(Var("int a")),
                new Assign(),

                // a += 1 + 2 / 3;
                new PushInt(3),
                new PushInt(2),
                new Divide(),
                new PushInt(1),
                new Add(),
                new PushVar(Var("int a")),
                new AssignAdd(),


                // b = 1 - 2 * 3;
                new PushInt(3),
                new PushInt(2),
                new Multiply(),
                new PushInt(1),
                new Subtract(),
                new PushVar(Var("int b")),
                new Assign(),

                // b -= 1 - 2 / 3;
                new PushInt(3),
                new PushInt(2),
                new Divide(),
                new PushInt(1),
                new Subtract(),
                new PushVar(Var("int b")),
                new AssignSubtract(),

                // c = 4 / (5 + 6) * 7;
                new PushInt(7),
                new PushInt(6),
                new PushInt(5),
                new Add(),
                new PushInt(4),
                new Divide(),
                new Multiply(),
                new PushVar(Var("int c")),
                new Assign(),

                // c *= 4 * (5 + 6) / 7;
                new PushInt(7),
                new PushInt(6),
                new PushInt(5),
                new Add(),
                new PushInt(4),
                new Multiply(),
                new Divide(),
                new PushVar(Var("int c")),
                new AssignMultiply(),

                // d = 4 / (5 - 6) * 7;
                new PushInt(7),
                new PushInt(6),
                new PushInt(5),
                new Subtract(),
                new PushInt(4),
                new Divide(),
                new Multiply(),
                new PushVar(Var("int d")),
                new Assign(),

                // d /= 4 * (5 - 6) / 7;
                new PushInt(7),
                new PushInt(6),
                new PushInt(5),
                new Subtract(),
                new PushInt(4),
                new Multiply(),
                new Divide(),
                new PushVar(Var("int d")),
                new AssignDivide(),
            };

            CompareInstructionLists(instructions, expectedInstructions);
        }

        [Fact]
        public void TestIntEqOperator()
        {
            string declarations = @"
                var int a;
                var int b;
                var int c;
                var int d;
            ";
            string expressions = @"
                a = 1 == 2 != 3;
                a = 1 != 2 == 3;
                a = b == c != d;
            ";
            List<AssemblyElement> instructions = ParseExpressions(declarations, expressions);

            List<AssemblyElement> expectedInstructions = new List<AssemblyElement>
            {
                // a = 1 == 2 != 3;
                new PushInt(3),
                new PushInt(2),
                new PushInt(1),
                new Equal(),
                new NotEqual(),
                new PushVar(Var("int a")),
                new Assign(),

                // a = 1 != 2 == 3;
                new PushInt(3),
                new PushInt(2),
                new PushInt(1),
                new NotEqual(),
                new Equal(),
                new PushVar(Var("int a")),
                new Assign(),

                // a = b == c != d;
                new PushVar(Var("int d")),
                new PushVar(Var("int c")),
                new PushVar(Var("int b")),
                new Equal(),
                new NotEqual(),
                new PushVar(Var("int a")),
                new Assign(),
            };

            CompareInstructionLists(instructions, expectedInstructions);
        }

        [Fact]
        public void TestIntOneArgOperator()
        {
            string declarations = @"
                var int a;
                var int b;
                var int c;
                var int d;
            ";
            string expressions = @"
                a = -1;
                b = !2;
                c = ~3;
                d = +4;
            ";
            List<AssemblyElement> instructions = ParseExpressions(declarations, expressions);

            List<AssemblyElement> expectedInstructions = new List<AssemblyElement>
            {
                // a = -1;
                new PushInt(1),
                new Minus(),
                new PushVar(Var("int a")),
                new Assign(),

                // b = !2;
                new PushInt(2),
                new Not(),
                new PushVar(Var("int b")),
                new Assign(),

                // c = ~3;
                new PushInt(3),
                new Negate(),
                new PushVar(Var("int c")),
                new Assign(),

                // d = +4;
                new PushInt(4),
                new Plus(),
                new PushVar(Var("int d")),
                new Assign(),
            };

            CompareInstructionLists(instructions, expectedInstructions);
        }

        [Fact]
        public void TestIntBitOperator()
        {
            string declarations = @"
                var int a;
                var int b;
                var int c;
                var int d;
            ";
            string expressions = @"
                a = 1 & 2;
                a = 1 | 2;
                a = 1 && 2;
                a = 1 || 2;
                
                a = 1 & b;
                a = 1 | b;
                a = 1 && b;
                a = 1 || b;
                
                a = b & 2;
                a = b | 2;
                a = b && 2;
                a = b || 2;
                
                a = c & d;
                a = c | d;
                a = c && d;
                a = c || d;
            ";
            List<AssemblyElement> instructions = ParseExpressions(declarations, expressions);

            List<AssemblyElement> expectedInstructions = new List<AssemblyElement>
            {
                // a = 1 & 2;
                new PushInt(2),
                new PushInt(1),
                new BitAnd(),
                new PushVar(Var("int a")),
                new Assign(),

                // a = 1 | 2;
                new PushInt(2),
                new PushInt(1),
                new BitOr(),
                new PushVar(Var("int a")),
                new Assign(),

                // a = 1 && 2;
                new PushInt(2),
                new PushInt(1),
                new LogAnd(),
                new PushVar(Var("int a")),
                new Assign(),

                // a = 1 || 2;
                new PushInt(2),
                new PushInt(1),
                new LogOr(),
                new PushVar(Var("int a")),
                new Assign(),

                // a = 1 & b;
                new PushVar(Var("int b")),
                new PushInt(1),
                new BitAnd(),
                new PushVar(Var("int a")),
                new Assign(),

                // a = 1 | b;
                new PushVar(Var("int b")),
                new PushInt(1),
                new BitOr(),
                new PushVar(Var("int a")),
                new Assign(),

                // a = 1 && b;
                new PushVar(Var("int b")),
                new PushInt(1),
                new LogAnd(),
                new PushVar(Var("int a")),
                new Assign(),

                // a = 1 || b;
                new PushVar(Var("int b")),
                new PushInt(1),
                new LogOr(),
                new PushVar(Var("int a")),
                new Assign(),

                // a = b & 2;
                new PushInt(2),
                new PushVar(Var("int b")),
                new BitAnd(),
                new PushVar(Var("int a")),
                new Assign(),

                // a = b | 2;
                new PushInt(2),
                new PushVar(Var("int b")),
                new BitOr(),
                new PushVar(Var("int a")),
                new Assign(),

                // a = b && 2;
                new PushInt(2),
                new PushVar(Var("int b")),
                new LogAnd(),
                new PushVar(Var("int a")),
                new Assign(),

                // a = b || 2;
                new PushInt(2),
                new PushVar(Var("int b")),
                new LogOr(),
                new PushVar(Var("int a")),
                new Assign(),

                // a = c & d;
                new PushVar(Var("int d")),
                new PushVar(Var("int c")),
                new BitAnd(),
                new PushVar(Var("int a")),
                new Assign(),

                // a = c | d;
                new PushVar(Var("int d")),
                new PushVar(Var("int c")),
                new BitOr(),
                new PushVar(Var("int a")),
                new Assign(),

                // a = c && d;
                new PushVar(Var("int d")),
                new PushVar(Var("int c")),
                new LogAnd(),
                new PushVar(Var("int a")),
                new Assign(),

                // a = c || d;
                new PushVar(Var("int d")),
                new PushVar(Var("int c")),
                new LogOr(),
                new PushVar(Var("int a")),
                new Assign(),
            };

            CompareInstructionLists(instructions, expectedInstructions);
        }

        [Fact]
        public void TestIntBitMoveOperator()
        {
            string declarations = @"
                var int a;
                var int b;
                var int c;
                var int d;
            ";
            string expressions = @"
                a = 1 << 2;
                a = 1 >> 2;
                a = 1 << b;
                a = b >> 2;
                a = c << d;
                a = c >> d;
            ";
            List<AssemblyElement> instructions = ParseExpressions(declarations, expressions);

            List<AssemblyElement> expectedInstructions = new List<AssemblyElement>
            {
                // a = 1 << 2;
                new PushInt(2),
                new PushInt(1),
                new ShiftLeft(),
                new PushVar(Var("int a")),
                new Assign(),

                // a = 1 >> 2;
                new PushInt(2),
                new PushInt(1),
                new ShiftRight(),
                new PushVar(Var("int a")),
                new Assign(),

                // a = 1 << b;
                new PushVar(Var("int b")),
                new PushInt(1),
                new ShiftLeft(),
                new PushVar(Var("int a")),
                new Assign(),

                // a = b >> 2;
                new PushInt(2),
                new PushVar(Var("int b")),
                new ShiftRight(),
                new PushVar(Var("int a")),
                new Assign(),

                // a = c << d;
                new PushVar(Var("int d")),
                new PushVar(Var("int c")),
                new ShiftLeft(),
                new PushVar(Var("int a")),
                new Assign(),

                // a = c >> d;
                new PushVar(Var("int d")),
                new PushVar(Var("int c")),
                new ShiftRight(),
                new PushVar(Var("int a")),
                new Assign(),
            };

            CompareInstructionLists(instructions, expectedInstructions);
        }

        [Fact]
        public void TestIntCompOperator()
        {
            string declarations = @"
                var int a;
                var int b;
                var int c;
                var int d;
            ";
            string expressions = @"
                a = 1 < 2;
                b = 1 <= 2;
                c = 1 > 2;
                d = 1 >= 2;
            ";
            List<AssemblyElement> instructions = ParseExpressions(declarations, expressions);

            List<AssemblyElement> expectedInstructions = new List<AssemblyElement>
            {
                // a = 1 < 2;
                new PushInt(2),
                new PushInt(1),
                new Less(),
                new PushVar(Var("int a")),
                new Assign(),

                // b = 1 <= 2;
                new PushInt(2),
                new PushInt(1),
                new LessOrEqual(),
                new PushVar(Var("int b")),
                new Assign(),

                // c = 1 > 2;
                new PushInt(2),
                new PushInt(1),
                new Greater(),
                new PushVar(Var("int c")),
                new Assign(),

                // d = 1 >= 2;
                new PushInt(2),
                new PushInt(1),
                new GreaterOrEqual(),
                new PushVar(Var("int d")),
                new Assign(),
            };

            CompareInstructionLists(instructions, expectedInstructions);
        }

        [Fact]
        public void TestIntComplexExpression()
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