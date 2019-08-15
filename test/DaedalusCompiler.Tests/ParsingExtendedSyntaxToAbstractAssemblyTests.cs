/*
using System.Collections.Generic;
using DaedalusCompiler.Compilation;
using DaedalusCompiler.Dat;
using Xunit;


namespace DaedalusCompiler.Tests
{
    public class ParsingExtendedSyntaxToAbstractAssemblyTests : ParsingSourceToAbstractAssemblyTestsBase
    {
       
        [Fact]
        public void TestWhileLoop()
        {
            _code = @"
                func void firstFunc() {
                    var int x;
                    x = 0;
                    while(x < 5) {
                        x += 1;
                    }
                };
           ";
            
            _instructions = GetExecBlockInstructions("firstFunc");
            _expectedInstructions = new List<AssemblyElement>
            {
                // x = 0;
                new PushInt(0),
                new PushVar(Ref("firstFunc.x")),
                new Assign(),
                
                // while(x < 5) {
                new AssemblyLabel("label_while_0"),
                new PushInt(5),
                new PushVar(Ref("firstFunc.x")),
                new Less(),
                new JumpIfToLabel("label_while_1"),
                
                //     x += 1;
                new PushInt(1),
                new PushVar(Ref("firstFunc.x")),
                new AssignAdd(),
                
                // }
                new JumpToLabel("label_while_0"),
                new AssemblyLabel("label_while_1"),

                new Ret(),
            };
            AssertInstructionsMatch();

            _expectedSymbols = new List<DatSymbol>
            {
                Ref("firstFunc"),
                Ref("firstFunc.x"),
            };
            AssertSymbolsMatch();
            
        }
        
        
        [Fact]
        public void TestWhileLoopBreakContinue()
        {
            _code = @"                
                func void secondFunc() {
                    var int x;
                    x = 0;
                    while(x < 5) {
                        x += 1;
                        if (x == 3) {
                            break;
                        } else if (x == 4) {
                            continue;
                        }
                    }
                };
           ";

            _instructions = GetExecBlockInstructions("secondFunc");
            _expectedInstructions = new List<AssemblyElement>
            {
                // x = 0;
                new PushInt(0),
                new PushVar(Ref("secondFunc.x")),
                new Assign(),
                
                // while(x < 5) {
                new AssemblyLabel("label_while_0"),
                new PushInt(5),
                new PushVar(Ref("secondFunc.x")),
                new Less(),
                new JumpIfToLabel("label_while_1"),
                
                //     x += 1;
                new PushInt(1),
                new PushVar(Ref("secondFunc.x")),
                new AssignAdd(),
                
                //     if (x == 3) {
                new PushInt(3),
                new PushVar(Ref("secondFunc.x")),
                new Equal(),
                new JumpIfToLabel("label_1"),
                
                //         break;
                new JumpToLabel("label_while_1"),
                
                //     } else if (x == 4) {
                new JumpToLabel("label_0"),
                new AssemblyLabel("label_1"),
                new PushInt(4),
                new PushVar(Ref("secondFunc.x")),
                new Equal(),
                new JumpIfToLabel("label_0"),
                
                //         continue;
                new JumpToLabel("label_while_0"),
                
                //     }
                new AssemblyLabel("label_0"),
                
                // }
                new JumpToLabel("label_while_0"),
                new AssemblyLabel("label_while_1"),


                new Ret(),
            };
            AssertInstructionsMatch();
            
            
            _expectedSymbols = new List<DatSymbol>
            {
                Ref("secondFunc"),
                Ref("secondFunc.x"),
            };
            AssertSymbolsMatch();
            
        }
    }
}
*/
