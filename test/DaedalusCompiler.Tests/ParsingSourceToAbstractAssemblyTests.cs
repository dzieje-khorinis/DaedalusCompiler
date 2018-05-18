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
        private readonly AssemblyBuilder assemblyBuilder;
        private string code;
        private List<AssemblyElement> instructions;
        private List<AssemblyElement> expectedInstructions;
        private List<DatSymbol> expectedSymbols;
        private bool parsed;

        public ParsingSourceToAbstractAssemblyTests()
        {
            assemblyBuilder = new AssemblyBuilder();
            instructions = new List<AssemblyElement>();
            expectedInstructions = new List<AssemblyElement>();
            expectedSymbols = new List<DatSymbol>();
            parsed = false;
        }

        private DatSymbol Ref(string symbolName)
        {
            return assemblyBuilder.resolveSymbol(symbolName);
        }

        private List<AssemblyElement> GetExecBlockInstructions(string execBlockName)
        {
            if (!parsed)
            {
                ParseData();
            }

            return assemblyBuilder.execBlocks
                .Find(execBlock => execBlock.symbol.Name.ToUpper() == execBlockName.ToUpper()).body;
        }

        private void ParseData()
        {
            parsed = true;

            AntlrInputStream inputStream = new AntlrInputStream(code);
            DaedalusLexer lexer = new DaedalusLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            DaedalusParser parser = new DaedalusParser(commonTokenStream);

            ParseTreeWalker.Default.Walk(new DaedalusParserListener(assemblyBuilder, 0), parser.daedalusFile());
        }

        private void AssertInstructionsMatch()
        {
            for (var index = 0; index < expectedInstructions.Count; index++)
            {
                var instruction = instructions[index];
                var expectedInstruction = expectedInstructions[index];
                CompareInstructions(expectedInstruction, instruction);
            }
        }

        private void AssertSymbolsMatch()
        {
            Assert.Equal(expectedSymbols.Count, assemblyBuilder.symbols.Count);
            for (var index = 0; index < expectedSymbols.Count; index++)
            {
                var symbol = assemblyBuilder.symbols[index];
                var expectedSymbol = expectedSymbols[index];
                Assert.Equal(expectedSymbol, symbol);
            }
        }


        private void CompareInstructions(AssemblyElement expectedInstruction, AssemblyElement instruction)
        {
            Assert.Equal(expectedInstruction.GetType(), instruction.GetType());
            switch (instruction)
            {
                case PushInt pushIntInstruction:
                {
                    Assert.Equal(
                        ((PushInt) expectedInstruction).value,
                        pushIntInstruction.value
                    );
                    break;
                }
                case Call _:
                case CallExternal _:
                case PushVar _:
                    Assert.Equal(
                        ((SymbolInstruction) expectedInstruction).symbol,
                        ((SymbolInstruction) instruction).symbol
                    );
                    break;
                case PushArrVar pushArrVarInstruction:
                {
                    Assert.Equal(
                        ((SymbolInstruction) expectedInstruction).symbol,
                        ((SymbolInstruction) instruction).symbol
                    );
                    Assert.Equal(
                        ((PushArrVar) expectedInstruction).index,
                        pushArrVarInstruction.index
                    );
                    break;
                }
                case AssemblyLabel assemblyLabelInstruction:
                {
                    Assert.Equal(
                        ((AssemblyLabel) expectedInstruction).label,
                        assemblyLabelInstruction.label
                    );
                    break;
                }

                case JumpIfToLabel _:
                case JumpToLabel _:
                {
                    var jumpInstruction = (JumpToLabel) instruction;
                    Assert.Equal(
                        ((JumpToLabel) expectedInstruction).label,
                        jumpInstruction.label);
                    break;
                }
            }
        }

        [Fact]
        public void TestIntAddOperator()
        {
            code = @"
                var int x;

                func void testFunc() {
                    x = 2 + 3 + 4 + 5;
                    x = 2 - 3 - 4 - 5;
                };
            ";

            instructions = GetExecBlockInstructions("testFunc");
            expectedInstructions = new List<AssemblyElement>
            {
                // x = 2 + 3 + 4 + 5;
                new PushInt(5),
                new PushInt(4),
                new PushInt(3),
                new PushInt(2),
                new Add(),
                new Add(),
                new Add(),
                new PushVar(Ref("x")),
                new Assign(),

                // x = 2 - 3 - 4 - 5;
                new PushInt(5),
                new PushInt(4),
                new PushInt(3),
                new PushInt(2),
                new Subtract(),
                new Subtract(),
                new Subtract(),
                new PushVar(Ref("x")),
                new Assign(),

                new Ret(),
            };
            AssertInstructionsMatch();

            expectedSymbols = new List<DatSymbol>
            {
                Ref("x"),
                Ref("testFunc"),
            };
            AssertSymbolsMatch();
        }

        [Fact]
        public void TestIntMultOperator()
        {
            code = @"
                var int x;

                func void testFunc() {
                    x = 2 * 3 * 4 * 5;
                    x = 2 / 3 / 4 / 5;
                    x = 2 % 3 % 4 % 5;
                };
            ";

            instructions = GetExecBlockInstructions("testFunc");
            expectedInstructions = new List<AssemblyElement>
            {
                // x = 2 * 3 * 4 * 5;
                new PushInt(5),
                new PushInt(4),
                new PushInt(3),
                new PushInt(2),
                new Multiply(),
                new Multiply(),
                new Multiply(),
                new PushVar(Ref("x")),
                new Assign(),

                // x = 2 / 3 / 4 / 5;
                new PushInt(5),
                new PushInt(4),
                new PushInt(3),
                new PushInt(2),
                new Divide(),
                new Divide(),
                new Divide(),
                new PushVar(Ref("x")),
                new Assign(),

                // x = 2 / 3 / 4 / 5;
                new PushInt(5),
                new PushInt(4),
                new PushInt(3),
                new PushInt(2),
                new Modulo(),
                new Modulo(),
                new Modulo(),
                new PushVar(Ref("x")),
                new Assign(),

                new Ret(),
            };
            AssertInstructionsMatch();

            expectedSymbols = new List<DatSymbol>
            {
                Ref("x"),
                Ref("testFunc"),
            };
            AssertSymbolsMatch();
        }

        [Fact]
        public void TestIntAssignmentOperator()
        {
            code = @"
                var int x;

                func void testFunc() {
                    x = 1
                    x += 2;
                    x -= 3;
                    x *= 4;
                    x /= 5;
                };
            ";

            instructions = GetExecBlockInstructions("testFunc");
            expectedInstructions = new List<AssemblyElement>
            {
                // x = 1;
                new PushInt(1),
                new PushVar(Ref("x")),
                new Assign(),

                // x += 2;
                new PushInt(2),
                new PushVar(Ref("x")),
                new AssignAdd(),

                // x -= 3;
                new PushInt(3),
                new PushVar(Ref("x")),
                new AssignSubtract(),

                // x *= 4;
                new PushInt(4),
                new PushVar(Ref("x")),
                new AssignMultiply(),

                // x /= 5;
                new PushInt(5),
                new PushVar(Ref("x")),
                new AssignDivide(),

                new Ret(),
            };
            AssertInstructionsMatch();

            expectedSymbols = new List<DatSymbol>
            {
                Ref("x"),
                Ref("testFunc"),
            };
            AssertSymbolsMatch();
        }

        [Fact]
        public void TestIntAddAndMultOperatorPrecedence()
        {
            code = @"
                var int a;
                var int b;
                var int c;
                var int d;

                func void testFunc() {
                    a = 1 + 2 * 3;
                    a += 1 + 2 / 3;
    
                    b = 1 - 2 * 3;
                    b -= 1 - 2 / 3;
                                
                    c = 4 / (5 + 6) * 7;
                    c *= 4 * (5 + 6) / 7;
    
                    d = 4 / (5 - 6) * 7;
                    d /= 4 * (5 - 6) / 7;
                };
            ";

            instructions = GetExecBlockInstructions("testFunc");
            expectedInstructions = new List<AssemblyElement>
            {
                // a = 1 + 2 * 3;
                new PushInt(3),
                new PushInt(2),
                new Multiply(),
                new PushInt(1),
                new Add(),
                new PushVar(Ref("a")),
                new Assign(),

                // a += 1 + 2 / 3;
                new PushInt(3),
                new PushInt(2),
                new Divide(),
                new PushInt(1),
                new Add(),
                new PushVar(Ref("a")),
                new AssignAdd(),

                // b = 1 - 2 * 3;
                new PushInt(3),
                new PushInt(2),
                new Multiply(),
                new PushInt(1),
                new Subtract(),
                new PushVar(Ref("b")),
                new Assign(),

                // b -= 1 - 2 / 3;
                new PushInt(3),
                new PushInt(2),
                new Divide(),
                new PushInt(1),
                new Subtract(),
                new PushVar(Ref("b")),
                new AssignSubtract(),

                // c = 4 / (5 + 6) * 7;
                new PushInt(7),
                new PushInt(6),
                new PushInt(5),
                new Add(),
                new PushInt(4),
                new Divide(),
                new Multiply(),
                new PushVar(Ref("c")),
                new Assign(),

                // c *= 4 * (5 + 6) / 7;
                new PushInt(7),
                new PushInt(6),
                new PushInt(5),
                new Add(),
                new PushInt(4),
                new Multiply(),
                new Divide(),
                new PushVar(Ref("c")),
                new AssignMultiply(),

                // d = 4 / (5 - 6) * 7;
                new PushInt(7),
                new PushInt(6),
                new PushInt(5),
                new Subtract(),
                new PushInt(4),
                new Divide(),
                new Multiply(),
                new PushVar(Ref("d")),
                new Assign(),

                // d /= 4 * (5 - 6) / 7;
                new PushInt(7),
                new PushInt(6),
                new PushInt(5),
                new Subtract(),
                new PushInt(4),
                new Multiply(),
                new Divide(),
                new PushVar(Ref("d")),
                new AssignDivide(),

                new Ret(),
            };
            AssertInstructionsMatch();

            expectedSymbols = new List<DatSymbol>
            {
                Ref("a"),
                Ref("b"),
                Ref("c"),
                Ref("d"),
                Ref("testFunc"),
            };
            AssertSymbolsMatch();
        }

        [Fact]
        public void TestIntEqOperator()
        {
            code = @"
                var int a;
                var int b;
                var int c;
                var int d;

                func void testFunc() {
                    a = 1 == 2 != 3;
                    a = 1 != 2 == 3;
                    a = b == c != d;
                };
            ";

            instructions = GetExecBlockInstructions("testFunc");
            expectedInstructions = new List<AssemblyElement>
            {
                // a = 1 == 2 != 3;
                new PushInt(3),
                new PushInt(2),
                new PushInt(1),
                new Equal(),
                new NotEqual(),
                new PushVar(Ref("a")),
                new Assign(),

                // a = 1 != 2 == 3;
                new PushInt(3),
                new PushInt(2),
                new PushInt(1),
                new NotEqual(),
                new Equal(),
                new PushVar(Ref("a")),
                new Assign(),

                // a = b == c != d;
                new PushVar(Ref("d")),
                new PushVar(Ref("c")),
                new PushVar(Ref("b")),
                new Equal(),
                new NotEqual(),
                new PushVar(Ref("a")),
                new Assign(),

                new Ret(),
            };
            AssertInstructionsMatch();

            expectedSymbols = new List<DatSymbol>
            {
                Ref("a"),
                Ref("b"),
                Ref("c"),
                Ref("d"),
                Ref("testFunc"),
            };
            AssertSymbolsMatch();
        }

        [Fact]
        public void TestIntOneArgOperator()
        {
            code = @"
                var int a;
                var int b;
                var int c;
                var int d;

                func void testFunc() {
                    a = -1;
                    b = !2;
                    c = ~3;
                    d = +4;
                };
            ";

            instructions = GetExecBlockInstructions("testFunc");
            expectedInstructions = new List<AssemblyElement>
            {
                // a = -1;
                new PushInt(1),
                new Minus(),
                new PushVar(Ref("a")),
                new Assign(),

                // b = !2;
                new PushInt(2),
                new Not(),
                new PushVar(Ref("b")),
                new Assign(),

                // c = ~3;
                new PushInt(3),
                new Negate(),
                new PushVar(Ref("c")),
                new Assign(),

                // d = +4;
                new PushInt(4),
                new Plus(),
                new PushVar(Ref("d")),
                new Assign(),

                new Ret(),
            };
            AssertInstructionsMatch();

            expectedSymbols = new List<DatSymbol>
            {
                Ref("a"),
                Ref("b"),
                Ref("c"),
                Ref("d"),
                Ref("testFunc"),
            };
            AssertSymbolsMatch();
        }


        [Fact]
        public void TestIntLogAndBinOperator()
        {
            code = @"
                var int a;
                var int b;
                var int c;
                var int d;

                func void testFunc() {
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
                };
            ";

            instructions = GetExecBlockInstructions("testFunc");
            expectedInstructions = new List<AssemblyElement>
            {
                // a = 1 & 2;
                new PushInt(2),
                new PushInt(1),
                new BitAnd(),
                new PushVar(Ref("a")),
                new Assign(),

                // a = 1 | 2;
                new PushInt(2),
                new PushInt(1),
                new BitOr(),
                new PushVar(Ref("a")),
                new Assign(),

                // a = 1 && 2;
                new PushInt(2),
                new PushInt(1),
                new LogAnd(),
                new PushVar(Ref("a")),
                new Assign(),

                // a = 1 || 2;
                new PushInt(2),
                new PushInt(1),
                new LogOr(),
                new PushVar(Ref("a")),
                new Assign(),

                // a = 1 & b;
                new PushVar(Ref("b")),
                new PushInt(1),
                new BitAnd(),
                new PushVar(Ref("a")),
                new Assign(),

                // a = 1 | b;
                new PushVar(Ref("b")),
                new PushInt(1),
                new BitOr(),
                new PushVar(Ref("a")),
                new Assign(),

                // a = 1 && b;
                new PushVar(Ref("b")),
                new PushInt(1),
                new LogAnd(),
                new PushVar(Ref("a")),
                new Assign(),

                // a = 1 || b;
                new PushVar(Ref("b")),
                new PushInt(1),
                new LogOr(),
                new PushVar(Ref("a")),
                new Assign(),

                // a = b & 2;
                new PushInt(2),
                new PushVar(Ref("b")),
                new BitAnd(),
                new PushVar(Ref("a")),
                new Assign(),

                // a = b | 2;
                new PushInt(2),
                new PushVar(Ref("b")),
                new BitOr(),
                new PushVar(Ref("a")),
                new Assign(),

                // a = b && 2;
                new PushInt(2),
                new PushVar(Ref("b")),
                new LogAnd(),
                new PushVar(Ref("a")),
                new Assign(),

                // a = b || 2;
                new PushInt(2),
                new PushVar(Ref("b")),
                new LogOr(),
                new PushVar(Ref("a")),
                new Assign(),

                // a = c & d;
                new PushVar(Ref("d")),
                new PushVar(Ref("c")),
                new BitAnd(),
                new PushVar(Ref("a")),
                new Assign(),

                // a = c | d;
                new PushVar(Ref("d")),
                new PushVar(Ref("c")),
                new BitOr(),
                new PushVar(Ref("a")),
                new Assign(),

                // a = c && d;
                new PushVar(Ref("d")),
                new PushVar(Ref("c")),
                new LogAnd(),
                new PushVar(Ref("a")),
                new Assign(),

                // a = c || d;
                new PushVar(Ref("d")),
                new PushVar(Ref("c")),
                new LogOr(),
                new PushVar(Ref("a")),
                new Assign(),

                new Ret(),
            };
            AssertInstructionsMatch();

            expectedSymbols = new List<DatSymbol>
            {
                Ref("a"),
                Ref("b"),
                Ref("c"),
                Ref("d"),
                Ref("testFunc"),
            };
            AssertSymbolsMatch();
        }


        [Fact]
        public void TestIntLogAndBinOperatorPrecedence()
        {
            code = @"
                var int a;
                var int b;

                func void testFunc() {
                    a = 0 || 1 && 2 | 3 & 4;
                    b = 5 & 6 | 7 && 8 || 9;
                };
            ";

            instructions = GetExecBlockInstructions("testFunc");
            expectedInstructions = new List<AssemblyElement>
            {
                // a = 0 || 1 && 2 | 3 & 4;
                new PushInt(4),
                new PushInt(3),
                new BitAnd(),
                new PushInt(2),
                new BitOr(),
                new PushInt(1),
                new LogAnd(),
                new PushInt(0),
                new LogOr(),
                new PushVar(Ref("a")),
                new Assign(),

                // b = 5 & 6 | 7 && 8 || 9;
                new PushInt(9),
                new PushInt(8),
                new PushInt(7),
                new PushInt(6),
                new PushInt(5),
                new BitAnd(),
                new BitOr(),
                new LogAnd(),
                new LogOr(),
                new PushVar(Ref("b")),
                new Assign(),

                new Ret(),
            };
            AssertInstructionsMatch();

            expectedSymbols = new List<DatSymbol>
            {
                Ref("a"),
                Ref("b"),
                Ref("testFunc"),
            };
            AssertSymbolsMatch();
        }

        [Fact]
        public void TestIntBitMoveOperator()
        {
            code = @"
                var int a;
                var int b;
                var int c;
                var int d;

                func void testFunc() {
                    a = 1 << 2;
                    a = 1 >> 2;
                    a = 1 << b;
                    a = b >> 2;
                    a = c << d;
                    a = c >> d;
                };
            ";

            instructions = GetExecBlockInstructions("testFunc");
            expectedInstructions = new List<AssemblyElement>
            {
                // a = 1 << 2;
                new PushInt(2),
                new PushInt(1),
                new ShiftLeft(),
                new PushVar(Ref("a")),
                new Assign(),

                // a = 1 >> 2;
                new PushInt(2),
                new PushInt(1),
                new ShiftRight(),
                new PushVar(Ref("a")),
                new Assign(),

                // a = 1 << b;
                new PushVar(Ref("b")),
                new PushInt(1),
                new ShiftLeft(),
                new PushVar(Ref("a")),
                new Assign(),

                // a = b >> 2;
                new PushInt(2),
                new PushVar(Ref("b")),
                new ShiftRight(),
                new PushVar(Ref("a")),
                new Assign(),

                // a = c << d;
                new PushVar(Ref("d")),
                new PushVar(Ref("c")),
                new ShiftLeft(),
                new PushVar(Ref("a")),
                new Assign(),

                // a = c >> d;
                new PushVar(Ref("d")),
                new PushVar(Ref("c")),
                new ShiftRight(),
                new PushVar(Ref("a")),
                new Assign(),

                new Ret(),
            };
            AssertInstructionsMatch();

            expectedSymbols = new List<DatSymbol>
            {
                Ref("a"),
                Ref("b"),
                Ref("c"),
                Ref("d"),
                Ref("testFunc"),
            };
            AssertSymbolsMatch();
        }

        [Fact]
        public void TestIntCompOperator()
        {
            code = @"
                var int a;
                var int b;
                var int c;
                var int d;

                func void testFunc() {
                    a = 1 < 2;
                    b = 1 <= 2;
                    c = 1 > 2;
                    d = 1 >= 2;
                };
            ";

            instructions = GetExecBlockInstructions("testFunc");
            expectedInstructions = new List<AssemblyElement>
            {
                // a = 1 < 2;
                new PushInt(2),
                new PushInt(1),
                new Less(),
                new PushVar(Ref("a")),
                new Assign(),

                // b = 1 <= 2;
                new PushInt(2),
                new PushInt(1),
                new LessOrEqual(),
                new PushVar(Ref("b")),
                new Assign(),

                // c = 1 > 2;
                new PushInt(2),
                new PushInt(1),
                new Greater(),
                new PushVar(Ref("c")),
                new Assign(),

                // d = 1 >= 2;
                new PushInt(2),
                new PushInt(1),
                new GreaterOrEqual(),
                new PushVar(Ref("d")),
                new Assign(),

                new Ret(),
            };
            AssertInstructionsMatch();

            expectedSymbols = new List<DatSymbol>
            {
                Ref("a"),
                Ref("b"),
                Ref("c"),
                Ref("d"),
                Ref("testFunc"),
            };
            AssertSymbolsMatch();
        }

        [Fact]
        public void TestIntComplexExpression()
        {
            code = @"
                func int otherFunc(var int a, var int b)
                {
                    return 0;
                };

                var int x;

                func void testFunc() {
                    x = 12 + 9 * ( 2 + otherFunc(1 + 7 * 3, 4 + 5) );
                };
            ";

            instructions = GetExecBlockInstructions("otherFunc");
            expectedInstructions = new List<AssemblyElement>
            {
                // parameters
                new PushVar(Ref("otherFunc.b")),
                new Assign(),
                new PushVar(Ref("otherFunc.a")),
                new Assign(),

                // return 0
                new PushInt(0),
                new Ret(),

                new Ret(),
            };
            AssertInstructionsMatch();

            instructions = GetExecBlockInstructions("testFunc");
            expectedInstructions = new List<AssemblyElement>
            {
                new PushInt(3),
                new PushInt(7),
                new Multiply(),
                new PushInt(1),
                new Add(),
                new PushInt(5),
                new PushInt(4),
                new Add(),
                new Call(Ref("otherFunc")),
                new PushInt(2),
                new Add(),
                new PushInt(9),
                new Multiply(),
                new PushInt(12),
                new Add(),
                new PushVar(Ref("x")),
                new Assign(),

                new Ret(),
            };
            AssertInstructionsMatch();

            expectedSymbols = new List<DatSymbol>
            {
                Ref("otherFunc"),
                Ref("otherFunc.a"),
                Ref("otherFunc.b"),
                Ref("x"),
                Ref("testFunc"),
            };
            AssertSymbolsMatch();
        }

        [Fact]
        public void TestIntArrElementExpression()
        {
            code = @"
                var int x;
                var int tab[3];

                func void testFunc() {
                    x = 1;
                    tab[0] = 2;
                    tab[1] = 3;
                    tab[2] = x;
                    x = tab[0] + tab[1] * tab[2];
                };
            ";

            instructions = GetExecBlockInstructions("testFunc");
            expectedInstructions = new List<AssemblyElement>
            {
                // x = 1;
                new PushInt(1),
                new PushVar(Ref("x")),
                new Assign(),

                // tab[0] = 2;
                new PushInt(2),
                new PushVar(Ref("tab")),
                new Assign(),

                // tab[1] = 3;
                new PushInt(3),
                new PushArrVar(Ref("tab"), 1),
                new Assign(),

                // tab[2] = x;
                new PushVar(Ref("x")),
                new PushArrVar(Ref("tab"), 2),
                new Assign(),

                // x = tab[0] + tab[1] * tab[2];
                new PushArrVar(Ref("tab"), 2),
                new PushArrVar(Ref("tab"), 1),
                new Multiply(),
                new PushVar(Ref("tab")),
                new Add(),
                new PushVar(Ref("x")),
                new Assign(),

                new Ret(),
            };
            AssertInstructionsMatch();

            expectedSymbols = new List<DatSymbol>
            {
                Ref("x"),
                Ref("tab"),
                Ref("testFunc"),
            };
            AssertSymbolsMatch();
        }


        [Fact]
        public void TestIntArrElementWithGlobalConstIntIndexExpression()
        {
            code = @"
                const int TAB_SIZE = 3;
                const int INDEX_ZERO = 0;
                const int INDEX_ONE = 1;
                const int INDEX_TWO = 2;
                var int x;
                var int tab[TAB_SIZE];

                func void testFunc() {
                    x = 1;
                    tab[INDEX_ZERO] = 2;
                    tab[INDEX_ONE] = 3;
                    tab[INDEX_TWO] = x;
                    x = tab[INDEX_ZERO] + tab[INDEX_ONE] * tab[INDEX_TWO];
                };
            ";

            instructions = GetExecBlockInstructions("testFunc");
            expectedInstructions = new List<AssemblyElement>
            {
                // x = 1;
                new PushInt(1),
                new PushVar(Ref("x")),
                new Assign(),

                // tab[INDEX_ZERO] = 2;
                new PushInt(2),
                new PushVar(Ref("tab")),
                new Assign(),

                // tab[INDEX_ONE] = 3;
                new PushInt(3),
                new PushArrVar(Ref("tab"), 1),
                new Assign(),

                //  tab[INDEX_TWO] = x;
                new PushVar(Ref("x")),
                new PushArrVar(Ref("tab"), 2),
                new Assign(),

                // x = tab[INDEX_ZERO] + tab[INDEX_ONE] * tab[INDEX_TWO];
                new PushArrVar(Ref("tab"), 2),
                new PushArrVar(Ref("tab"), 1),
                new Multiply(),
                new PushVar(Ref("tab")),
                new Add(),
                new PushVar(Ref("x")),
                new Assign(),

                new Ret(),
            };
            AssertInstructionsMatch();

            expectedSymbols = new List<DatSymbol>
            {
                Ref("TAB_SIZE"),
                Ref("INDEX_ZERO"),
                Ref("INDEX_ONE"),
                Ref("INDEX_TWO"),
                Ref("x"),
                Ref("tab"),
                Ref("testFunc"),
            };
            AssertSymbolsMatch();
        }

        [Fact]
        public void TestIntArrElementWithLocalConstIntIndexExpression()
        {
            code = @"
                const int TAB_SIZE = 3;
                var int x;
                var int tab[TAB_SIZE];

                func void testFunc() {
                    const int INDEX_ZERO = 0;
                    const int INDEX_ONE = 1;
                    const int INDEX_TWO = 2;
                    x = 1;
                    tab[INDEX_ZERO] = 2;
                    tab[INDEX_ONE] = 3;
                    tab[INDEX_TWO] = x;
                    x = tab[INDEX_ZERO] + tab[INDEX_ONE] * tab[INDEX_TWO];
                };
            ";

            instructions = GetExecBlockInstructions("testFunc");
            expectedInstructions = new List<AssemblyElement>
            {
                // x = 1;
                new PushInt(1),
                new PushVar(Ref("x")),
                new Assign(),

                // tab[INDEX_ZERO] = 2;
                new PushInt(2),
                new PushVar(Ref("tab")),
                new Assign(),

                // tab[INDEX_ONE] = 3;
                new PushInt(3),
                new PushArrVar(Ref("tab"), 1),
                new Assign(),

                //  tab[INDEX_TWO] = x;
                new PushVar(Ref("x")),
                new PushArrVar(Ref("tab"), 2),
                new Assign(),

                // x = tab[INDEX_ZERO] + tab[INDEX_ONE] * tab[INDEX_TWO];
                new PushArrVar(Ref("tab"), 2),
                new PushArrVar(Ref("tab"), 1),
                new Multiply(),
                new PushVar(Ref("tab")),
                new Add(),
                new PushVar(Ref("x")),
                new Assign(),

                new Ret(),
            };
            AssertInstructionsMatch();

            expectedSymbols = new List<DatSymbol>
            {
                Ref("TAB_SIZE"),
                Ref("x"),
                Ref("tab"),
                Ref("testFunc"),
                Ref("testFunc.INDEX_ZERO"),
                Ref("testFunc.INDEX_ONE"),
                Ref("testFunc.INDEX_TWO"),
            };
            AssertSymbolsMatch();
        }

        [Fact]
        public void TestIntArrParameter()
        {
            code = @"
                const int ATTRS_COUNT = 5;
                
                func void testFunc(var int x, var int tab[3], var float attrs[ATTRS_COUNT]) {
                    x = 1;
                    tab[0] = 2;
                    tab[1] = 3;
                    tab[2] = x;
                    x = tab[0] + tab[1] * tab[2];
                };
            ";

            instructions = GetExecBlockInstructions("testFunc");
            expectedInstructions = new List<AssemblyElement>
            {
                // parameters
                new PushVar(Ref("testFunc.attrs")),
                new AssignFloat(),
                new PushVar(Ref("testFunc.tab")),
                new Assign(),
                new PushVar(Ref("testFunc.x")),
                new Assign(),

                // x = 1;
                new PushInt(1),
                new PushVar(Ref("testFunc.x")),
                new Assign(),

                // tab[0] = 2;
                new PushInt(2),
                new PushVar(Ref("testFunc.tab")),
                new Assign(),

                // tab[1] = 3;
                new PushInt(3),
                new PushArrVar(Ref("testFunc.tab"), 1),
                new Assign(),

                // tab[2] = x;
                new PushVar(Ref("testFunc.x")),
                new PushArrVar(Ref("testFunc.tab"), 2),
                new Assign(),

                // x = tab[0] + tab[1] * tab[2];
                new PushArrVar(Ref("testFunc.tab"), 2),
                new PushArrVar(Ref("testFunc.tab"), 1),
                new Multiply(),
                new PushVar(Ref("testFunc.tab")),
                new Add(),
                new PushVar(Ref("testFunc.x")),
                new Assign(),

                new Ret(),
            };
            AssertInstructionsMatch();

            expectedSymbols = new List<DatSymbol>
            {
                Ref("attrs_count"),
                Ref("testFunc"),
                Ref("testFunc.x"),
                Ref("testFunc.tab"),
                Ref("testFunc.attrs"),
            };
            AssertSymbolsMatch();
        }


        [Fact]
        public void TestMostOperatorsPrecedence()
        {
            code = @"
                var int x;

                func void testFunc() {
                    x = +1 * -2 / !3 % ~4 + 5 - 6 << 7 >> 8 < 9 > 10 <= 11 >= 12 & 13 | 14 && 15 || 16;
                    x = 16 || 15 && 14 | 13 & 12 >= 11 <= 10 > 9 < 8 >> 7 << 6 - 5 + ~4 % !3 / -2 * +1;
                };
            ";

            instructions = GetExecBlockInstructions("testFunc");
            expectedInstructions = new List<AssemblyElement>
            {
                // x = +1 * -2 / !3 % ~4 + 5 - 6 << 7 >> 8 < 9 > 10 <= 11 >= 12 & 13 | 14 && 15 || 16;
                new PushInt(16),
                new PushInt(15),
                new PushInt(14),
                new PushInt(13),
                new PushInt(12),
                new PushInt(11),
                new PushInt(10),
                new PushInt(9),
                new PushInt(8),
                new PushInt(7),
                new PushInt(6),
                new PushInt(5),
                new PushInt(4),
                new Negate(),
                new PushInt(3),
                new Not(),
                new PushInt(2),
                new Minus(),
                new PushInt(1),
                new Plus(),
                new Multiply(),
                new Divide(),
                new Modulo(),
                new Add(),
                new Subtract(),
                new ShiftLeft(),
                new ShiftRight(),
                new Less(),
                new Greater(),
                new LessOrEqual(),
                new GreaterOrEqual(),
                new BitAnd(),
                new BitOr(),
                new LogAnd(),
                new LogOr(),
                new PushVar(Ref("x")),
                new Assign(),

                // x = 16 || 15 && 14 | 13 & 12 >= 11 <= 10 > 9 < 8 >> 7 << 6 - 5 + ~4 % !3 / -2 * +1;
                new PushInt(1),
                new Plus(),
                new PushInt(2),
                new Minus(),
                new PushInt(3),
                new Not(),
                new PushInt(4),
                new Negate(),
                new Modulo(),
                new Divide(),
                new Multiply(),
                new PushInt(5),
                new PushInt(6),
                new Subtract(),
                new Add(),
                new PushInt(7),
                new PushInt(8),
                new ShiftRight(),
                new ShiftLeft(),
                new PushInt(9),
                new PushInt(10),
                new PushInt(11),
                new PushInt(12),
                new GreaterOrEqual(),
                new LessOrEqual(),
                new Greater(),
                new Less(),
                new PushInt(13),
                new BitAnd(),
                new PushInt(14),
                new BitOr(),
                new PushInt(15),
                new LogAnd(),
                new PushInt(16),
                new LogOr(),
                new PushVar(Ref("x")),
                new Assign(),

                new Ret(),
            };
            AssertInstructionsMatch();

            expectedSymbols = new List<DatSymbol>
            {
                Ref("x"),
                Ref("testFunc"),
            };
            AssertSymbolsMatch();
        }

        [Fact]
        public void TestFuncClassParameterAndAttributesInSimpleExpressions()
        {
            code = @"
                class person {
                    var int age;
                };
                
                var int a;

                func void testFunc(var person d) {
                    d.age = 5;
                    a = d.age;
                };
            ";

            instructions = GetExecBlockInstructions("testFunc");
            expectedInstructions = new List<AssemblyElement>
            {
                // parameters
                new PushInstance(Ref("testFunc.d")),
                new AssignInstance(),

                // d.age = 5;
                new PushInt(5),
                new SetInstance(Ref("testFunc.d")),
                new PushVar(Ref("person.age")),
                new Assign(),

                // a = d.age;
                new SetInstance(Ref("testFunc.d")),
                new PushVar(Ref("person.age")),
                new PushVar(Ref("a")),
                new Assign(),

                new Ret(),
            };
            AssertInstructionsMatch();

            expectedSymbols = new List<DatSymbol>
            {
                Ref("person"),
                Ref("person.age"),
                Ref("a"),
                Ref("testFunc"),
                Ref("testFunc.d"),
            };
            AssertSymbolsMatch();
        }

        [Fact]
        public void TestGlobalVarClassAndAttributesInSimpleExpressions()
        {
            code = @"
                class person {
                    var int age;
                };
                
                var int a;
                var person d;

                func void testFunc() {
                    d.age = 5;
                    a = d.age;
                };
            ";

            instructions = GetExecBlockInstructions("testFunc");
            expectedInstructions = new List<AssemblyElement>
            {
                // d.age = 5;
                new PushInt(5),
                new SetInstance(Ref("d")),
                new PushVar(Ref("person.age")),
                new Assign(),

                // a = d.age;
                new SetInstance(Ref("d")),
                new PushVar(Ref("person.age")),
                new PushVar(Ref("a")),
                new Assign(),

                new Ret(),
            };
            AssertInstructionsMatch();

            expectedSymbols = new List<DatSymbol>
            {
                Ref("person"),
                Ref("person.age"),
                Ref("a"),
                Ref("d"),
                Ref("testFunc"),
            };
            AssertSymbolsMatch();
        }

        [Fact]
        public void TestLocalVarClassAndAttributesInSimpleExpressions()
        {
            code = @"
                class person {
                    var int age;
                };
                
                var int a;

                func void testFunc() {
                    var person d;
                    d.age = 5;
                    a = d.age;
                };
            ";

            instructions = GetExecBlockInstructions("testFunc");
            expectedInstructions = new List<AssemblyElement>
            {
                // d.age = 5;
                new PushInt(5),
                new SetInstance(Ref("testFunc.d")),
                new PushVar(Ref("person.age")),
                new Assign(),

                // a = d.age;
                new SetInstance(Ref("testFunc.d")),
                new PushVar(Ref("person.age")),
                new PushVar(Ref("a")),
                new Assign(),

                new Ret(),
            };
            AssertInstructionsMatch();

            expectedSymbols = new List<DatSymbol>
            {
                Ref("person"),
                Ref("person.age"),
                Ref("a"),
                Ref("testFunc"),
                Ref("testFunc.d"),
            };
            AssertSymbolsMatch();
        }

        [Fact]
        public void TestIfInstruction()
        {
            code = @"
                var int a;

                func void testFunc()
                {
                    if ( 1 < 2 ) {
                        a = 5;
                        a = 2 * 2;
                    };
                };
            ";
            instructions = GetExecBlockInstructions("testFunc");
            expectedInstructions = new List<AssemblyElement>
            {
                // if ( 1 < 2 )
                new PushInt(2),
                new PushInt(1),
                new Less(),
                new JumpIfToLabel("label_0"),
                // a = 5;
                new PushInt(5),
                new PushVar(Ref("a")),
                new Assign(),
                // a = 2 * 2;
                new PushInt(2),
                new PushInt(2),
                new Multiply(),
                new PushVar(Ref("a")),
                new Assign(),
                // if end
                new AssemblyLabel("label_0"),
                new Ret()
            };
            AssertInstructionsMatch();

            expectedSymbols = new List<DatSymbol>
            {
                Ref("a"),
                Ref("testFunc"),
            };
            AssertSymbolsMatch();
        }

        [Fact]
        public void TestIfAndElseInstruction()
        {
            code = @"
                var int a;

                func void testFunc()
                {
                    if ( 1 < 2 ) {
                        a = 3;
                    } else {
                        a = 4;
                    };
                };
            ";
            instructions = GetExecBlockInstructions("testFunc");
            expectedInstructions = new List<AssemblyElement>
            {
                // if ( 1 < 2 )
                new PushInt(2),
                new PushInt(1),
                new Less(),
                new JumpIfToLabel("label_1"),
                // a = 3;
                new PushInt(3),
                new PushVar(Ref("a")),
                new Assign(),
                new JumpToLabel("label_0"),
                // if end
                // else start
                new AssemblyLabel("label_1"),
                // a = 4;
                new PushInt(4),
                new PushVar(Ref("a")),
                new Assign(),
                // else and if end
                new AssemblyLabel("label_0"),
                new Ret()
            };
            AssertInstructionsMatch();

            expectedSymbols = new List<DatSymbol>
            {
                Ref("a"),
                Ref("testFunc"),
            };
            AssertSymbolsMatch();
        }

        [Fact]
        public void TestIfElseInstruction()
        {
            code = @"
                var int a;

                func void testFunc()
                {
                        if ( 1 < 2 ) {
                            a = 3;
                        } else if ( 4 < 5 ) {
                            a = 6;
                        } else {
                            a = 7;
                        };
                };
            ";
            instructions = GetExecBlockInstructions("testFunc");
            expectedInstructions = new List<AssemblyElement>
            {
                // if ( 1 < 2 )
                new PushInt(2),
                new PushInt(1),
                new Less(),
                new JumpIfToLabel("label_1"),
                // a = 3;
                new PushInt(3),
                new PushVar(Ref("a")),
                new Assign(),
                new JumpToLabel("label_0"),
                // if end
                new AssemblyLabel("label_1"),
                // else if ( 4 < 5 )
                new PushInt(5),
                new PushInt(4),
                new Less(),
                new JumpIfToLabel("label_2"),
                // a = 6;
                new PushInt(6),
                new PushVar(Ref("a")),
                new Assign(),
                new JumpToLabel("label_0"),
                // else if end
                // else start
                new AssemblyLabel("label_2"),
                //a = 7;
                new PushInt(7),
                new PushVar(Ref("a")),
                new Assign(),
                // else end if end
                new AssemblyLabel("label_0"),
                new Ret()
            };
            AssertInstructionsMatch();

            expectedSymbols = new List<DatSymbol>
            {
                Ref("a"),
                Ref("testFunc"),
            };
            AssertSymbolsMatch();
        }
    }
}