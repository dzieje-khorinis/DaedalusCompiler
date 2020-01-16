using System.Collections.Generic;
using Common;
using Xunit;


namespace DaedalusCompiler.Tests
{

    
    
    public class ParsingSourceToAbstractAssemblyTests : ParsingSourceToAbstractAssemblyTestsBase
    {
        [Fact]
        public void TestIntAddOperator()
        {
            Code = @"
                var int x;

                func void testFunc() {
                    x = 2 + 3 + 4 + 5;
                    x = 2 - 3 - 4 - 5;
                };
            ";

            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
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

            ExpectedSymbols = new List<Symbol>
            {
                Ref("x"),
                Ref("testFunc"),
            };
            AssertSymbolsMatch();
        }

        [Fact]
        public void TestIntMultOperator()
        {
            Code = @"
                var int x;

                func void testFunc() {
                    x = 2 * 3 * 4 * 5;
                    x = 2 / 3 / 4 / 5;
                    x = 2 % 3 % 4 % 5;
                };
            ";

            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
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

            ExpectedSymbols = new List<Symbol>
            {
                Ref("x"),
                Ref("testFunc"),
            };
            AssertSymbolsMatch();
        }

        [Fact]
        public void TestIntAssignmentOperator()
        {
            Code = @"
                var int x;

                func void testFunc() {
                    x = 1;
                    x += 2;
                    x -= 3;
                    x *= 4;
                    x /= 5;
                };
            ";

            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
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

            ExpectedSymbols = new List<Symbol>
            {
                Ref("x"),
                Ref("testFunc"),
            };
            AssertSymbolsMatch();
        }

        [Fact]
        public void TestInstanceAssignment()
        {
            Code = @"
                extern func instance HLP_GetNpc(var int par0);
                
                class C_NPC { var int data [200]; };
                var C_NPC person;
                
                func void  testFunc()
                {
                    var int test;
                    person = HLP_GetNpc (0);
                    test = person == person;
                    test = test == person;
                    test = person + person;
                    test = test + person;
                };
            ";

            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // person = HLP_GetNpc (0);
                new PushInt(0),
                new CallExternal(Ref("HLP_GetNpc")),
                new PushInstance(Ref("person")),
                new AssignInstance(),
                
                // test = person == person;
                new PushInt(RefIndex("person")),
                new PushInt(RefIndex("person")),
                new Equal(),
                new PushVar(Ref("testFunc.test")),
                new Assign(),
                
                // test = test == person;
                new PushInt(RefIndex("person")),
                new PushVar(Ref("testFunc.test")),
                new Equal(),
                new PushVar(Ref("testFunc.test")),
                new Assign(),
                
                // test = person + person;
                new PushInt(RefIndex("person")),
                new PushInt(RefIndex("person")),
                new Add(),
                new PushVar(Ref("testFunc.test")),
                new Assign(),
                
                // test = test + person;
                new PushInt(RefIndex("person")),
                new PushVar(Ref("testFunc.test")),
                new Add(),
                new PushVar(Ref("testFunc.test")),
                new Assign(),


                new Ret(),
            };
            AssertInstructionsMatch();

            ExpectedSymbols = new List<Symbol>
            {
                Ref("HLP_GetNpc"),
                Ref("HLP_GetNpc.par0"),
                Ref("C_NPC"),
                Ref("C_NPC.data"),
                Ref("person"),
                Ref("testFunc"),
                Ref("testFunc.test"),
            };
            AssertSymbolsMatch();
        }
        
        [Fact]
        public void TestFullAssignment()
        {
            Code = @"
                class C_NPC { var int data [200]; };

                // var void varvoid;
                var float varfloat;
                var int varint;
                var string varstring;
                // var class varclass;
                var C_NPC varclass;
                var func varfunc;
                // var prototype varprototype;
                prototype varprototype(C_NPC) {};
                // var instance varinstance;
                instance varinstance(C_NPC) {};
                instance varinstance2(varprototype) {};
                
                
                func void retvoid() {};
                func float retfloat() {};
                func int retint() {};
                func string retstring() {};
                // func class retclass() {};
                func C_NPC retC_NPC() {};
                // func func retfunc() {};
                // func prototype retprototype() {};
                // func varprototype retvarprototype() {};
                // func instance retinstance() {};
                // func varinstance retvarinstance() {};
                // func varinstance2 retvarinstance2() {};
                
                
                func void testFunc() {
                    var float locfloat;
                    var int locint;
                    var string locstring;
                    var C_NPC locclass;
                    var func locfunc;
                    
                    // locfloat = varfloat;
                    // locfloat = varint;
                    // locfloat = varstring;
                    // locfloat = varclass;
                    // locfloat = varfunc;
                    // locfloat = varprototype;
                    // locfloat = varinstance;
                    // locfloat = varinstance2;
                    // locfloat = retvoid;
                    // locfloat = retfloat;
                    // locfloat = retint;
                    // locfloat = retstring;
                    // locfloat = retC_NPC;
                    // locfloat = retvoid();
                    locfloat = retfloat();
                    // locfloat = retint();
                    // locfloat = retstring();
                    // locfloat = retC_NPC();
                    
                    // locint = varfloat;
                    locint = varint;
                    // locint = varstring;
                    locint = varclass;
                    // locint = varfunc;
                    locint = varprototype;
                    locint = varinstance;
                    locint = varinstance2;
                    // locint = retvoid;
                    // locint = retfloat;
                    // locint = retint;
                    // locint = retstring;
                    // locint = retC_NPC;
                    // locint = retvoid();
                    // locint = retfloat();
                    locint = retint();
                    // locint = retstring();
                    // locint = retC_NPC();
                    
                    // locstring = varfloat;
                    // locstring = varint;
                    locstring = varstring;
                    // locstring = varclass;
                    // locstring = varfunc;
                    // locstring = varprototype;
                    // locstring = varinstance;
                    // locstring = varinstance2;
                    // locstring = retvoid;
                    // locstring = retfloat;
                    // locstring = retint;
                    // locstring = retstring;
                    // locstring = retC_NPC;
                    // locstring = retvoid();
                    // locstring = retfloat();
                    // locstring = retint();
                    locstring = retstring();
                    // locstring = retC_NPC();
                    
                    // locclass = varfloat;
                    // locclass = varint;
                    // locclass = varstring;
                    // locclass = varclass;
                    // locclass = varfunc;
                    // locclass = varprototype;
                    // locclass = varinstance;
                    // locclass = varinstance2;
                    // locclass = retvoid;
                    // locclass = retfloat;
                    // locclass = retint;
                    // locclass = retstring;
                    // locclass = retC_NPC;
                    // locclass = retvoid();
                    // locclass = retfloat();
                    // locclass = retint();
                    // locclass = retstring();
                    locclass = retC_NPC();
                    
                    // locfunc = varfloat;
                    // locfunc = varint;
                    // locfunc = varstring;
                    locfunc = varclass;
                    locfunc = varfunc;
                    locfunc = varprototype;
                    locfunc = varinstance;
                    locfunc = varinstance2;
                    locfunc = retvoid;
                    locfunc = retfloat;
                    locfunc = retint;
                    locfunc = retstring;
                    locfunc = retC_NPC;
                    // locfunc = retvoid();
                    // locfunc = retfloat();
                    // locfunc = retint();
                    // locfunc = retstring();
                    // locfunc = retC_NPC();
                };
            ";

            Instructions = GetExecBlockInstructions("testfunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // locfloat = retfloat();
                new Call(Ref("retfloat")),
                new PushVar(Ref("testfunc.locfloat")),
                new AssignFloat(),
                
                // locint = varint;
                new PushVar(Ref("varint")),
                new PushVar(Ref("testfunc.locint")),
                new Assign(),
                
                // locint = varclass;
                new PushInt(RefIndex("varclass")),
                new PushVar(Ref("testfunc.locint")),
                new Assign(),
                
                // locint = varprototype;
                new PushInt(RefIndex("varprototype")),
                new PushVar(Ref("testfunc.locint")),
                new Assign(),
                
                // locint = varinstance;
                new PushInt(RefIndex("varinstance")),
                new PushVar(Ref("testfunc.locint")),
                new Assign(),
                
                // locint = varinstance2;
                new PushInt(RefIndex("varinstance2")),
                new PushVar(Ref("testfunc.locint")),
                new Assign(),
                
                // locint = retint();
                new Call(Ref("retint")),
                new PushVar(Ref("testfunc.locint")),
                new Assign(),

                // locstring = varstring;
                new PushVar(Ref("varstring")),
                new PushVar(Ref("testfunc.locstring")),
                new AssignString(),
                
                // locstring = retstring();
                new Call(Ref("retstring")),
                new PushVar(Ref("testfunc.locstring")),
                new AssignString(),
                
                // locclass = retC_NPC();
                new Call(Ref("retC_NPC")),
                new PushInstance(Ref("testfunc.locclass")),
                new AssignInstance(),

                // locfunc = varclass;
                new PushInt(RefIndex("varclass")),
                new PushVar(Ref("testfunc.locfunc")),
                new AssignFunc(),
                
                // locfunc = varfunc;
                new PushInt(RefIndex("varfunc")),
                new PushVar(Ref("testfunc.locfunc")),
                new AssignFunc(),
                
                // locfunc = varprototype;
                new PushInt(RefIndex("varprototype")),
                new PushVar(Ref("testfunc.locfunc")),
                new AssignFunc(),
                
                // locfunc = varinstance;
                new PushInt(RefIndex("varinstance")),
                new PushVar(Ref("testfunc.locfunc")),
                new AssignFunc(),
                
                // locfunc = varinstance2;
                new PushInt(RefIndex("varinstance2")),
                new PushVar(Ref("testfunc.locfunc")),
                new AssignFunc(),
                
                // locfunc = retvoid;
                new PushInt(RefIndex("retvoid")),
                new PushVar(Ref("testfunc.locfunc")),
                new AssignFunc(),
                
                // locfunc = retfloat;
                new PushInt(RefIndex("retfloat")),
                new PushVar(Ref("testfunc.locfunc")),
                new AssignFunc(),
                
                // locfunc = retint;
                new PushInt(RefIndex("retint")),
                new PushVar(Ref("testfunc.locfunc")),
                new AssignFunc(),
                
                // locfunc = retstring;
                new PushInt(RefIndex("retstring")),
                new PushVar(Ref("testfunc.locfunc")),
                new AssignFunc(),
                
                // locfunc = retC_NPC;
                new PushInt(RefIndex("retC_NPC")),
                new PushVar(Ref("testfunc.locfunc")),
                new AssignFunc(),

                new Ret(),
            };
            AssertInstructionsMatch();

            ExpectedSymbols = new List<Symbol>
            {
                Ref("C_NPC"),
                Ref("C_NPC.data"),
                Ref("varfloat"),
                Ref("varint"),
                Ref("varstring"),
                Ref("varclass"),
                Ref("varfunc"),
                Ref("varprototype"),
                Ref("varinstance"),
                Ref("varinstance2"),
                Ref("retvoid"),
                Ref("retfloat"),
                Ref("retint"),
                Ref("retstring"),
                Ref("retC_NPC"),
                Ref("testfunc"),
                Ref("testfunc.locfloat"),
                Ref("testfunc.locint"),
                Ref("testfunc.locstring"),
                Ref("testfunc.locclass"),
                Ref("testfunc.locfunc"),
            };
            AssertSymbolsMatch();
        }
        
        [Fact]
        public void TestFullAssignmentLiterals()
        {
            Code = @"
            func void testFunc() {
                var float varfloat;
                var int varint;
            
                varfloat = 0;
                varfloat = 2.5;
                varfloat = 5;
                varfloat = -5;
                varfloat = -2.5;
                varfloat = +100;
                
                varint = 0;
                varint = 5;
                varint = -5;
                varint = +100;
                
                varint = 0 - -1;
                varint = 0 - +1;
                varint = 0 - 1;
                
                varint = 5 - -1;
                varint = 5 - +1;
                varint = 5 - 1;
                
                varint = -5 - -1;
                varint = -5 - +1;
                varint = -5 - 1;
                
                varint = +100 - -1;
                varint = +100 - +1;
                varint = +100 - 1;
            };

            ";

            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {    
                
                // varfloat = 0;
                new PushInt(0),
                new PushVar(Ref("testFunc.varfloat")),
                new AssignFloat(),
                
                // varfloat = 2.5;
                new PushInt(1075838976),
                new PushVar(Ref("testFunc.varfloat")),
                new AssignFloat(),
                
                // varfloat = 5;
                new PushInt(1084227584),
                new PushVar(Ref("testFunc.varfloat")),
                new AssignFloat(),

                // varfloat = -5;
                new PushInt(-1063256064),
                new PushVar(Ref("testFunc.varfloat")),
                new AssignFloat(),

                // varfloat = -2.5;
                new PushInt(-1071644672),
                new PushVar(Ref("testFunc.varfloat")),
                new AssignFloat(),

                // varfloat = +100;
                new PushInt(1120403456),
                new PushVar(Ref("testFunc.varfloat")),
                new AssignFloat(),

                // varint = 0;
                new PushInt(0),
                new PushVar(Ref("testFunc.varint")),
                new Assign(),

                // varint = 5;
                new PushInt(5),
                new PushVar(Ref("testFunc.varint")),
                new Assign(),
                
                // varint = -5;
                new PushInt(5),
                new Minus(),
                new PushVar(Ref("testFunc.varint")),
                new Assign(),

                // varint = +100;
                new PushInt(100),
                new Plus(),
                new PushVar(Ref("testFunc.varint")),
                new Assign(),
                
                // varint = 0 - -1;
                new PushInt(1),
                new Minus(),
                new PushInt(0),
                new Subtract(),
                new PushVar(Ref("testFunc.varint")),
                new Assign(),
                
                // varint = 0 - +1;
                new PushInt(1),
                new Plus(),
                new PushInt(0),
                new Subtract(),
                new PushVar(Ref("testFunc.varint")),
                new Assign(),
                
                // varint = 0 - 1;
                new PushInt(1),
                new PushInt(0),
                new Subtract(),
                new PushVar(Ref("testFunc.varint")),
                new Assign(),
                
                // varint = 5 - -1;
                new PushInt(1),
                new Minus(),
                new PushInt(5),
                new Subtract(),
                new PushVar(Ref("testFunc.varint")),
                new Assign(),
                
                // varint = 5 - +1;
                new PushInt(1),
                new Plus(),
                new PushInt(5),
                new Subtract(),
                new PushVar(Ref("testFunc.varint")),
                new Assign(),
                
                // varint = 5 - 1;
                new PushInt(1),
                new PushInt(5),
                new Subtract(),
                new PushVar(Ref("testFunc.varint")),
                new Assign(),
                
                // varint = -5 - -1;
                new PushInt(1),
                new Minus(),
                new PushInt(5),
                new Minus(),
                new Subtract(),
                new PushVar(Ref("testFunc.varint")),
                new Assign(),
                
                // varint = -5 - +1;
                new PushInt(1),
                new Plus(),
                new PushInt(5),
                new Minus(),
                new Subtract(),
                new PushVar(Ref("testFunc.varint")),
                new Assign(),
                
                // varint = -5 - 1;
                new PushInt(1),
                new PushInt(5),
                new Minus(),
                new Subtract(),
                new PushVar(Ref("testFunc.varint")),
                new Assign(),
                
                // varint = +100 - -1;
                new PushInt(1),
                new Minus(),
                new PushInt(100),
                new Plus(),
                new Subtract(),
                new PushVar(Ref("testFunc.varint")),
                new Assign(),
                
                // varint = +100 - +1;
                new PushInt(1),
                new Plus(),
                new PushInt(100),
                new Plus(),
                new Subtract(),
                new PushVar(Ref("testFunc.varint")),
                new Assign(),
                
                // varint = +100 - 1;
                new PushInt(1),
                new PushInt(100),
                new Plus(),
                new Subtract(),
                new PushVar(Ref("testFunc.varint")),
                new Assign(),
                
                new Ret(),
            };
            AssertInstructionsMatch();

            ExpectedSymbols = new List<Symbol>
            {
                Ref("testFunc"),
                Ref("testFunc.varfloat"),
                Ref("testFunc.varint"),
            };
            AssertSymbolsMatch();
        }
        
        [Fact]
        public void TestFullAttributeAssignment()
        {
            Code = @"
                class C_NPC {
                    var float varfloat;
                    var int varint;
                    var string varstring;
                    var C_NPC varclass;    // this has size 0 in original compiler
                    var func varfunc;
                    var int data[191]; // because C_NPC class is supposed to have 800 bytes
                };
                
                var float varfloat;
                var int varint;
                var string varstring;
                var C_NPC varclass;
                var func varfunc;
                prototype varprototype(C_NPC) {};
                instance varinstance(C_NPC) {};
                instance varinstance2(varprototype) {};
                
                func void retvoid() {};
                func float retfloat() {};
                func int retint() {};
                func string retstring() {};
                func C_NPC retC_NPC() {};
                
                
                func void testFunc() {
                    varinstance.varfloat = retfloat();
                    
                    varinstance.varint = varinstance2.varint;
                    varinstance.varint = varinstance2.varclass;
                    varinstance.varint = varprototype;
                    varinstance.varint = varinstance;
                    varinstance.varint = varinstance2;
                    varinstance.varint = retint();
                
                    varinstance.varstring = varinstance2.varstring;
                    varinstance.varstring = retstring();
                
                    // varinstance.varclass = retC_NPC();
                
                    varinstance.varfunc = varinstance2.varclass;
                    varinstance.varfunc = varinstance2.varfunc;
                    varinstance.varfunc = varprototype;
                    varinstance.varfunc = varinstance;
                    varinstance.varfunc = varinstance2;
                    varinstance.varfunc = retvoid;
                    varinstance.varfunc = retfloat;
                    varinstance.varfunc = retint;
                    varinstance.varfunc = retstring;
                    varinstance.varfunc = retC_NPC;
                };
            ";

            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // varinstance.varfloat = retfloat();
                new Call(Ref("retfloat")),
                new SetInstance(Ref("varinstance")),
                new PushVar(Ref("C_NPC.varfloat")),
                new AssignFloat(),
                
                // varinstance.varint = varinstance2.varint;
                new SetInstance(Ref("varinstance2")),
                new PushVar(Ref("C_NPC.varint")),
                new SetInstance(Ref("varinstance")),
                new PushVar(Ref("C_NPC.varint")),
                new Assign(),
                
                // varinstance.varint = varinstance2.varclass;
                new SetInstance(Ref("varinstance2")),
                new PushInt(RefIndex("C_NPC.varclass")),
                new SetInstance(Ref("varinstance")),
                new PushVar(Ref("C_NPC.varint")),
                new Assign(),
                
                // varinstance.varint = varprototype;
                new PushInt(RefIndex("varprototype")),
                new SetInstance(Ref("varinstance")),
                new PushVar(Ref("C_NPC.varint")),
                new Assign(),
                
                // varinstance.varint = varinstance;
                new PushInt(RefIndex("varinstance")),
                new SetInstance(Ref("varinstance")),
                new PushVar(Ref("C_NPC.varint")),
                new Assign(),
                
                // varinstance.varint = varinstance2;
                new PushInt(RefIndex("varinstance2")),
                new SetInstance(Ref("varinstance")),
                new PushVar(Ref("C_NPC.varint")),
                new Assign(),
                
                // varinstance.varint = retint();
                new Call(Ref("retint")),
                new SetInstance(Ref("varinstance")),
                new PushVar(Ref("C_NPC.varint")),
                new Assign(),
            
                // varinstance.varstring = varinstance2.varstring;
                new SetInstance(Ref("varinstance2")),
                new PushVar(Ref("C_NPC.varstring")),
                new SetInstance(Ref("varinstance")),
                new PushVar(Ref("C_NPC.varstring")),
                new AssignString(),
                
                // varinstance.varstring = retstring();
                new Call(Ref("retstring")),
                new SetInstance(Ref("varinstance")),
                new PushVar(Ref("C_NPC.varstring")),
                new AssignString(),
            
                // varinstance.varclass = retC_NPC();
                // new Call(Ref("retC_NPC")),
                // new PushInstance(Ref("C_NPC.varclass")), //  should be SetInstance && PushVar instead, probably Gothic's compiler bug
                // new AssignInstance(),
                
                // varinstance.varfunc = varinstance2.varclass;
                new SetInstance(Ref("varinstance2")),  //TODO original compiler doesn't add SetInstance here, but it would be good if we had it
                new PushInt(RefIndex("C_NPC.varclass")), // SetInstance before?
                new SetInstance(Ref("varinstance")),
                new PushVar(Ref("C_NPC.varfunc")),
                new AssignFunc(),
                
                // varinstance.varfunc = varinstance2.varfunc;
                new SetInstance(Ref("varinstance2")),  //TODO original compiler doesn't add SetInstance here, but it would be good if we had it
                new PushInt(RefIndex("C_NPC.varfunc")),
                new SetInstance(Ref("varinstance")),
                new PushVar(Ref("C_NPC.varfunc")),
                new AssignFunc(),
                
                // varinstance.varfunc = varprototype;
                new PushInt(RefIndex("varprototype")),
                new SetInstance(Ref("varinstance")),
                new PushVar(Ref("C_NPC.varfunc")),
                new AssignFunc(),
                
                // varinstance.varfunc = varinstance;
                new PushInt(RefIndex("varinstance")),
                new SetInstance(Ref("varinstance")),
                new PushVar(Ref("C_NPC.varfunc")),
                new AssignFunc(),
                
                // varinstance.varfunc = varinstance2;
                new PushInt(RefIndex("varinstance2")),
                new SetInstance(Ref("varinstance")),
                new PushVar(Ref("C_NPC.varfunc")),
                new AssignFunc(),
                
                // varinstance.varfunc = retvoid;
                new PushInt(RefIndex("retvoid")),
                new SetInstance(Ref("varinstance")),
                new PushVar(Ref("C_NPC.varfunc")),
                new AssignFunc(),
                
                // varinstance.varfunc = retfloat;
                new PushInt(RefIndex("retfloat")),
                new SetInstance(Ref("varinstance")),
                new PushVar(Ref("C_NPC.varfunc")),
                new AssignFunc(),
                
                // varinstance.varfunc = retint;
                new PushInt(RefIndex("retint")),
                new SetInstance(Ref("varinstance")),
                new PushVar(Ref("C_NPC.varfunc")),
                new AssignFunc(),
                
                // varinstance.varfunc = retstring;
                new PushInt(RefIndex("retstring")),
                new SetInstance(Ref("varinstance")),
                new PushVar(Ref("C_NPC.varfunc")),
                new AssignFunc(),
                
                // varinstance.varfunc = retC_NPC;
                new PushInt(RefIndex("retC_NPC")),
                new SetInstance(Ref("varinstance")),
                new PushVar(Ref("C_NPC.varfunc")),
                new AssignFunc(),
                

                new Ret(),
            };
            AssertInstructionsMatch();

            ExpectedSymbols = new List<Symbol>
            {
                Ref("C_NPC"),
                Ref("C_NPC.varfloat"),
                Ref("C_NPC.varint"),
                Ref("C_NPC.varstring"),
                Ref("C_NPC.varclass"),
                Ref("C_NPC.varfunc"),
                Ref("C_NPC.data"),
                Ref("varfloat"),
                Ref("varint"),
                Ref("varstring"),
                Ref("varclass"),
                Ref("varfunc"),
                Ref("varprototype"),
                Ref("varinstance"),
                Ref("varinstance2"),
                Ref("retvoid"),
                Ref("retfloat"),
                Ref("retint"),
                Ref("retstring"),
                Ref("retC_NPC"),
                Ref("testFunc"),
            };
            AssertSymbolsMatch();
        }
        
        
        [Fact]
        public void TestIntAddAndMultOperatorPrecedence()
        {
            Code = @"
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

            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
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

            ExpectedSymbols = new List<Symbol>
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
            Code = @"
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

            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
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

            ExpectedSymbols = new List<Symbol>
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
            Code = @"
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

            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
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

            ExpectedSymbols = new List<Symbol>
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
            Code = @"
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

            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
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

            ExpectedSymbols = new List<Symbol>
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
            Code = @"
                var int a;
                var int b;

                func void testFunc() {
                    a = 0 || 1 && 2 | 3 & 4;
                    b = 5 & 6 | 7 && 8 || 9;
                };
            ";

            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
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

            ExpectedSymbols = new List<Symbol>
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
            Code = @"
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

            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
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

            ExpectedSymbols = new List<Symbol>
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
            Code = @"
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

            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
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

            ExpectedSymbols = new List<Symbol>
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
            Code = @"
                func int otherFunc(var int a, var int b)
                {
                    return 0;
                };

                var int x;

                func void testFunc() {
                    x = 12 + 9 * ( 2 + otherFunc(1 + 7 * 3, 4 + 5) );
                };
            ";

            Instructions = GetExecBlockInstructions("otherFunc");
            ExpectedInstructions = new List<AssemblyElement>
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

            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
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

            ExpectedSymbols = new List<Symbol>
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
            Code = @"
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

            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
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
                new PushArrayVar(Ref("tab"), 1),
                new Assign(),

                // tab[2] = x;
                new PushVar(Ref("x")),
                new PushArrayVar(Ref("tab"), 2),
                new Assign(),

                // x = tab[0] + tab[1] * tab[2];
                new PushArrayVar(Ref("tab"), 2),
                new PushArrayVar(Ref("tab"), 1),
                new Multiply(),
                new PushVar(Ref("tab")),
                new Add(),
                new PushVar(Ref("x")),
                new Assign(),

                new Ret(),
            };
            AssertInstructionsMatch();

            ExpectedSymbols = new List<Symbol>
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
            Code = @"
                const int TAB_SIZE = 0 + 1 + 2;
                const int INDEX_ZERO = 0 - 0 + 0 * 0;
                const int INDEX_ONE = 0 + 1 - 0;
                const int INDEX_TWO = 1 + 1;
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

            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
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
                new PushArrayVar(Ref("tab"), 1),
                new Assign(),

                //  tab[INDEX_TWO] = x;
                new PushVar(Ref("x")),
                new PushArrayVar(Ref("tab"), 2),
                new Assign(),

                // x = tab[INDEX_ZERO] + tab[INDEX_ONE] * tab[INDEX_TWO];
                new PushArrayVar(Ref("tab"), 2),
                new PushArrayVar(Ref("tab"), 1),
                new Multiply(),
                new PushVar(Ref("tab")),
                new Add(),
                new PushVar(Ref("x")),
                new Assign(),

                new Ret(),
            };
            AssertInstructionsMatch();

            ExpectedSymbols = new List<Symbol>
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
            Code = @"
                const int TAB_SIZE = 3;
                var int x;
                var int tab[TAB_SIZE];

                func void testFunc() {
                    const int INDEX_ZERO = 0;
                    const int INDEX_ONE = 1;
                    const int INDEX_TWO = 1 + 1;
                    x = 1;
                    tab[INDEX_ZERO] = 2;
                    tab[INDEX_ONE] = 3;
                    tab[INDEX_TWO] = x;
                    x = tab[INDEX_ZERO] + tab[INDEX_ONE] * tab[INDEX_TWO];
                };
            ";

            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
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
                new PushArrayVar(Ref("tab"), 1),
                new Assign(),

                //  tab[INDEX_TWO] = x;
                new PushVar(Ref("x")),
                new PushArrayVar(Ref("tab"), 2),
                new Assign(),

                // x = tab[INDEX_ZERO] + tab[INDEX_ONE] * tab[INDEX_TWO];
                new PushArrayVar(Ref("tab"), 2),
                new PushArrayVar(Ref("tab"), 1),
                new Multiply(),
                new PushVar(Ref("tab")),
                new Add(),
                new PushVar(Ref("x")),
                new Assign(),

                new Ret(),
            };
            AssertInstructionsMatch();

            ExpectedSymbols = new List<Symbol>
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
            Code = @"
                const int ATTRS_COUNT = 5;
                
                func void testFunc(var int x, var int tab[3], var float attrs[ATTRS_COUNT]) {
                    x = 1;
                    tab[0] = 2;
                    tab[1] = 3;
                    tab[2] = x;
                    x = tab[0] + tab[1] * tab[2];
                };
            ";

            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
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
                new PushArrayVar(Ref("testFunc.tab"), 1),
                new Assign(),

                // tab[2] = x;
                new PushVar(Ref("testFunc.x")),
                new PushArrayVar(Ref("testFunc.tab"), 2),
                new Assign(),

                // x = tab[0] + tab[1] * tab[2];
                new PushArrayVar(Ref("testFunc.tab"), 2),
                new PushArrayVar(Ref("testFunc.tab"), 1),
                new Multiply(),
                new PushVar(Ref("testFunc.tab")),
                new Add(),
                new PushVar(Ref("testFunc.x")),
                new Assign(),

                new Ret(),
            };
            AssertInstructionsMatch();

            ExpectedSymbols = new List<Symbol>
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
            Code = @"
                var int x;

                func void testFunc() {
                    x = +1 * -2 / !3 % ~4 + 5 - 6 << 7 >> 8 < 9 > 10 <= 11 >= 12 & 13 | 14 && 15 || 16;
                    x = 16 || 15 && 14 | 13 & 12 >= 11 <= 10 > 9 < 8 >> 7 << 6 - 5 + ~4 % !3 / -2 * +1;
                };
            ";

            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
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

            ExpectedSymbols = new List<Symbol>
            {
                Ref("x"),
                Ref("testFunc"),
            };
            AssertSymbolsMatch();
        }

        [Fact]
        public void TestFuncClassParameterAndAttributesInSimpleExpressions()
        {
            Code = @"
                class person {
                    var int age;
                };
                
                var int a;

                func void testFunc(var person d) {
                    d.age = 5;
                    a = d.age;
                };
            ";

            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
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

            ExpectedSymbols = new List<Symbol>
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
            Code = @"
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

            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
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

            ExpectedSymbols = new List<Symbol>
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
            Code = @"
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

            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
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

            ExpectedSymbols = new List<Symbol>
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
        public void TestInlineVarAndConstDeclarations()
        {
            Code = @"
                const int a = 1, b = 2, c = 3;
                var int d, e, f;
                
                func void testFunc(var int g) {
                    const int h = 4;
                    var int k;
                    d = 7;
                    e = 8;
                    f = 9;
                    k = 10;
                    g = 11;
                };
            ";

            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // parameters
                new PushVar(Ref("testFunc.g")),
                new Assign(),

                // d = 7;
                new PushInt(7),
                new PushVar(Ref("d")),
                new Assign(),

                // e = 8;
                new PushInt(8),
                new PushVar(Ref("e")),
                new Assign(),

                // f = 9;
                new PushInt(9),
                new PushVar(Ref("f")),
                new Assign(),

                // k = 10;
                new PushInt(10),
                new PushVar(Ref("testFunc.k")),
                new Assign(),

                // g = 11;
                new PushInt(11),
                new PushVar(Ref("testFunc.g")),
                new Assign(),

                new Ret(),
            };
            AssertInstructionsMatch();

            ExpectedSymbols = new List<Symbol>
            {
                Ref("a"),
                Ref("b"),
                Ref("c"),
                Ref("d"),
                Ref("e"),
                Ref("f"),
                Ref("testFunc"),
                Ref("testFunc.g"),
                Ref("testFunc.h"),
                Ref("testFunc.k"),
            };
            AssertSymbolsMatch();
        }

        [Fact]
        public void TestClassPrototypeInstanceInheritanceAndVarAndConstDeclarationsInsidePrototypeAndInstance()
        {
            Code = @"
                class Person {
                    var int age;
                };
                
                prototype Orc(Person) {
                    age = 10;
                    var int weight;
                    weight = 20;
                    const int HEIGHT = 100;
                };
                
                instance OrcShaman(Orc) {
                    age = 10;
                    var int mana;
                    mana = 30;
                    const int HP = 200;
                };
                
                var int a;
                
                func void testFunc()
                {
                    var person d;
                    d.age = 5;
                    a = d.age;
                };
            ";

            Instructions = GetExecBlockInstructions("Orc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // age = 10;
                new PushInt(10),
                new PushVar(Ref("Person.age")),
                new Assign(),

                // weight = 20;
                new PushInt(20),
                new PushVar(Ref("Orc.weight")),
                new Assign(),

                new Ret(),
            };
            AssertInstructionsMatch();

            Instructions = GetExecBlockInstructions("OrcShaman");
            ExpectedInstructions = new List<AssemblyElement>
            {
                new Call(Ref("Orc")), // only when parent is prototype

                // age = 10;
                new PushInt(10),
                new PushVar(Ref("Person.age")),
                new Assign(),

                // mana = 30;
                new PushInt(30),
                new PushVar(Ref("OrcShaman.mana")),
                new Assign(),

                new Ret(),
            };
            AssertInstructionsMatch();

            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // d.age = 5;
                new PushInt(5),
                new SetInstance(Ref("testFunc.d")),
                new PushVar(Ref("Person.age")),
                new Assign(),

                // a = d.age;
                new SetInstance(Ref("testFunc.d")),
                new PushVar(Ref("Person.age")),
                new PushVar(Ref("a")),
                new Assign(),

                new Ret(),
            };
            AssertInstructionsMatch();

            ExpectedSymbols = new List<Symbol>
            {
                Ref("Person"),
                Ref("Person.age"),
                Ref("Orc"),
                Ref("Orc.weight"),
                Ref("Orc.height"),
                Ref("OrcShaman"),
                Ref("OrcShaman.mana"),
                Ref("OrcShaman.hp"),
                Ref("a"),
                Ref("testFunc"),
                Ref("testFunc.d"),
            };
            AssertSymbolsMatch();
        }

        [Fact]
        public void TestClassInstanceInheritanceAndVarAndConstDeclarationsInsidePrototypeAndInstance()
        {
            Code = @"
                class Person {
                    var int age;
                };

                instance OrcShaman(Person) {
                    age = 10;
                    var int mana;
                    mana = 30;
                    const int HP = 200;
                };
                
                var int a;
                
                func void testFunc()
                {
                    var person d;
                    d.age = 5;
                    a = d.age;
                };
            ";

            Instructions = GetExecBlockInstructions("OrcShaman");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // age = 10;
                new PushInt(10),
                new PushVar(Ref("Person.age")),
                new Assign(),

                // mana = 30;
                new PushInt(30),
                new PushVar(Ref("OrcShaman.mana")),
                new Assign(),

                new Ret(),
            };
            AssertInstructionsMatch();

            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // d.age = 5;
                new PushInt(5),
                new SetInstance(Ref("testFunc.d")),
                new PushVar(Ref("Person.age")),
                new Assign(),

                // a = d.age;
                new SetInstance(Ref("testFunc.d")),
                new PushVar(Ref("Person.age")),
                new PushVar(Ref("a")),
                new Assign(),

                new Ret(),
            };
            AssertInstructionsMatch();

            ExpectedSymbols = new List<Symbol>
            {
                Ref("Person"),
                Ref("Person.age"),
                Ref("OrcShaman"),
                Ref("OrcShaman.mana"),
                Ref("OrcShaman.hp"),
                Ref("a"),
                Ref("testFunc"),
                Ref("testFunc.d"),
            };
            AssertSymbolsMatch();
        }

        [Fact]
        public void TestFloatExpressions()
        {
            Code = @"
                var float x;

                func float otherFunc() {};                
                
                const float a = -10;
                const float b = +20;
                const float c = 10.;
                const float d = .5;
                const float e = 12.5e+001;
                const float f = -12.e-002;

                func void testFunc(var float y) {
                    const float q = -12.5;
                    y = 5.5;
                    y = 5.;
                    y = .5;
                    y = .5e+001;
                    y = 5.e-001;

                    x = -1.5;
                    x = -1;
                    x = 0;
                    x = 1;
                    x = 1.5;
                    x = +2.5;
                    x = 3.5;
                };
            ";

            Instructions = GetExecBlockInstructions("otherFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                new Ret(),
            };
            AssertInstructionsMatch();

            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // parameters
                new PushVar(Ref("testFunc.y")),
                new AssignFloat(),

                // y = 5.5;
                new PushInt(1085276160),
                new PushVar(Ref("testFunc.y")),
                new AssignFloat(),
                
                // y = 5.;
                new PushInt(1084227584),
                new PushVar(Ref("testFunc.y")),
                new AssignFloat(),
                
                // y = .5;
                new PushInt(1056964608),
                new PushVar(Ref("testFunc.y")),
                new AssignFloat(),
                
                // y = .5e+001;
                new PushInt(1084227584),
                new PushVar(Ref("testFunc.y")),
                new AssignFloat(),
                
                // y = 5.e-001;
                new PushInt(1056964608),
                new PushVar(Ref("testFunc.y")),
                new AssignFloat(),

                // x = -1.5;
                new PushInt(-1077936128),
                new PushVar(Ref("x")),
                new AssignFloat(),

                // x = -1;
                new PushInt(-1082130432),
                new PushVar(Ref("x")),
                new AssignFloat(),

                // x = 0;
                new PushInt(0),
                new PushVar(Ref("x")),
                new AssignFloat(),

                // x = 1;
                new PushInt(1065353216),
                new PushVar(Ref("x")),
                new AssignFloat(),

                // x = 1.5;
                new PushInt(1069547520),
                new PushVar(Ref("x")),
                new AssignFloat(),

                // x = 2.5;
                new PushInt(1075838976),
                new PushVar(Ref("x")),
                new AssignFloat(),

                // x = 3.5;
                new PushInt(1080033280),
                new PushVar(Ref("x")),
                new AssignFloat(),

                new Ret(),
            };
            AssertInstructionsMatch();

            ExpectedSymbols = new List<Symbol>
            {
                Ref("x"),
                Ref("otherFunc"),
                Ref("a"),
                Ref("b"),
                Ref("c"),
                Ref("d"),
                Ref("e"),
                Ref("f"),
                Ref("testFunc"),
                Ref("testFunc.y"),
                Ref("testFunc.q"),
            };
            AssertSymbolsMatch();

            AssertRefContentEqual("a", -10.0f);
            AssertRefContentEqual("b", 20.0f);
            AssertRefContentEqual("c", 10.0f);
            AssertRefContentEqual("d", 0.5f);
            AssertRefContentEqual("e", 125.0f);
            AssertRefContentEqual("f", -0.12f);
            AssertRefContentEqual("testFunc.q", -12.5f);
        }

        [Fact]
        public void TestStringExpressions()
        {
            Code = @"
                const string hyzio = ""Hyzio"";
                const string dyzio = ""Dyzio"";
                const string zyzio = ""Zyzio"";
    
                var string lech;
    
                func string someFunc() {
                    return lech;
                };
    
                func string otherFunc(var string john) {
                    return ""Dyzio"";
                };
    
                func string anotherFunc() {
                    return zyzio;
                };
    
                func void testFunc(var string czech) {
                    var string rus;
                    lech = ""Lech"";
        
                    czech = ""Czech"";
                    czech = ""Dyzio"";
        
                    rus = ""Rus"";
        
                    const string hyzio_clone = ""Hyzio"";
                    hyzio_clone = hyzio;
                    hyzio_clone = ""Hyzio"";
                    hyzio_clone = ""Hyzio"";
        
                    var string lech_clone;
                    lech_clone = ""Lech"";
                    lech_clone = otherFunc(""John"");
                };
            ";

            char prefix = (char) 255;

            Instructions = GetExecBlockInstructions("someFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // return lech;
                new PushVar(Ref("lech")),
                new Ret(),

                new Ret(),
            };
            AssertInstructionsMatch();

            Instructions = GetExecBlockInstructions("otherFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // parameters
                new PushVar(Ref("otherFunc.john")),
                new AssignString(),

                // return "Dyzio";
                new PushVar(Ref($"{prefix}10000")),
                new Ret(),

                new Ret(),
            };
            AssertInstructionsMatch();

            Instructions = GetExecBlockInstructions("anotherFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // return zyzio;
                new PushVar(Ref("zyzio")),
                new Ret(),

                new Ret(),
            };
            AssertInstructionsMatch();

            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // parameters
                new PushVar(Ref("testFunc.czech")),
                new AssignString(),

                // lech = "Lech";
                new PushVar(Ref($"{prefix}10001")),
                new PushVar(Ref("lech")),
                new AssignString(),

                // czech = "Czech";
                new PushVar(Ref($"{prefix}10002")),
                new PushVar(Ref("testFunc.czech")),
                new AssignString(),

                // czech = "Dyzio";
                new PushVar(Ref($"{prefix}10003")),
                new PushVar(Ref("testFunc.czech")),
                new AssignString(),

                // rus = "Rus";
                new PushVar(Ref($"{prefix}10004")),
                new PushVar(Ref("testFunc.rus")),
                new AssignString(),

                // hyzio_clone = hyzio;
                new PushVar(Ref("hyzio")),
                new PushVar(Ref("testFunc.hyzio_clone")),
                new AssignString(),

                // hyzio_clone = "Hyzio";
                new PushVar(Ref($"{prefix}10005")),
                new PushVar(Ref("testFunc.hyzio_clone")),
                new AssignString(),

                // hyzio_clone = "Hyzio";
                new PushVar(Ref($"{prefix}10006")),
                new PushVar(Ref("testFunc.hyzio_clone")),
                new AssignString(),

                // lech_clone = "Lech";
                new PushVar(Ref($"{prefix}10007")),
                new PushVar(Ref("testFunc.lech_clone")),
                new AssignString(),

                // lech_clone = otherFunc("John");
                new PushVar(Ref($"{prefix}10008")),
                new Call(Ref("otherFunc")),
                new PushVar(Ref("testFunc.lech_clone")),
                new AssignString(),

                new Ret(),
            };
            AssertInstructionsMatch();

            ExpectedSymbols = new List<Symbol>
            {
                Ref("hyzio"),
                Ref("dyzio"),
                Ref("zyzio"),
                Ref("lech"),
                Ref("someFunc"),
                Ref("otherFunc"),
                Ref("otherFunc.john"),
                Ref("anotherFunc"),
                Ref("testFunc"),
                Ref("testFunc.czech"),
                Ref("testFunc.rus"),
                Ref("testFunc.hyzio_clone"),
                Ref("testFunc.lech_clone"),
                Ref($"{prefix}10000"),
                Ref($"{prefix}10001"),
                Ref($"{prefix}10002"),
                Ref($"{prefix}10003"),
                Ref($"{prefix}10004"),
                Ref($"{prefix}10005"),
                Ref($"{prefix}10006"),
                Ref($"{prefix}10007"),
                Ref($"{prefix}10008"),
            };
            AssertSymbolsMatch();

            AssertRefContentEqual("hyzio", "Hyzio");
            AssertRefContentEqual("dyzio", "Dyzio");
            AssertRefContentEqual("zyzio", "Zyzio");
            AssertRefContentEqual($"{prefix}10000", "Dyzio");
            AssertRefContentEqual($"{prefix}10001", "Lech");
            AssertRefContentEqual($"{prefix}10002", "Czech");
            AssertRefContentEqual($"{prefix}10003", "Dyzio");
            AssertRefContentEqual($"{prefix}10004", "Rus");
            AssertRefContentEqual($"{prefix}10005", "Hyzio");
            AssertRefContentEqual($"{prefix}10006", "Hyzio");
            AssertRefContentEqual($"{prefix}10007", "Lech");
            AssertRefContentEqual($"{prefix}10008", "John");
        }

        [Fact]
        public void TestIfInstruction()
        {
            Code = @"
                var int a;

                func void testFunc()
                {
                    if ( 1 < 2 ) {
                        a = 5;
                        a = 2 * 2;
                    };
                };
            ";
            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // if ( 1 < 2 )
                new PushInt(2),
                new PushInt(1),
                new Less(),
                new JumpIfToLabel("#0001_endif"),
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
                new AssemblyLabel("#0001_endif"),
                new Ret()
            };
            AssertInstructionsMatch();

            ExpectedSymbols = new List<Symbol>
            {
                Ref("a"),
                Ref("testFunc"),
            };
            AssertSymbolsMatch();
        }

        [Fact]
        public void TestIfInstructionWithoutParenthesis()
        {
            Code = @"
                func void testFunc() {
                    var int x;
                    if x == 1
                    {
                    
                    } else if x {
                    
                    };
                };
            ";
            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // if x == 1
                new PushInt(1),
                new PushVar(Ref("testFunc.x")),
                new Equal(),
                new JumpIfToLabel("#0001_else_if_1"),
                new JumpToLabel("#0001_endif"),
                // endif 
                
                // } else if x {
                new AssemblyLabel("#0001_else_if_1"),
                new PushVar(Ref("testFunc.x")),
                new JumpIfToLabel("#0001_endif"),
                // endelseif
                
                new AssemblyLabel("#0001_endif"),
                new Ret()
            };
            AssertInstructionsMatch();

            ExpectedSymbols = new List<Symbol>
            {
                Ref("testFunc"),
                Ref("testFunc.x"),
            };
            AssertSymbolsMatch();
        }
        
        [Fact]
        public void TestStringLiteralSymbolsOrder()
        {
            Code = @"
                func int firstFunc(var string par){};
                
                func void testFunc() {
                    if (firstFunc(""GOMEZ"") || firstFunc(""KRUK"") || firstFunc(""DIEGO"")) {
                
                    };
                };
            ";
            
            char prefix = (char) 255;
            
            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // if (firstFunc(""GOMEZ"") || firstFunc(""KRUK"") || firstFunc(""DIEGO"")) {
                new PushVar(Ref($"{prefix}10000")),
                new Call(Ref("firstFunc")),
                new PushVar(Ref($"{prefix}10001")),
                new Call(Ref("firstFunc")),
                new PushVar(Ref($"{prefix}10002")),
                new Call(Ref("firstFunc")),
                new LogOr(),
                new LogOr(),
                new JumpIfToLabel("#0001_endif"),
                // endif 
            
                new AssemblyLabel("#0001_endif"),    
                new Ret()
            };
            AssertInstructionsMatch();

            
            ExpectedSymbols = new List<Symbol>
            {
                Ref("firstFunc"),
                Ref("firstFunc.par"),
                Ref("testFunc"),
                Ref($"{prefix}10000"),
                Ref($"{prefix}10001"),
                Ref($"{prefix}10002"),
            };
            AssertSymbolsMatch();

            AssertRefContentEqual($"{prefix}10000", "DIEGO");
            AssertRefContentEqual($"{prefix}10001", "KRUK");
            AssertRefContentEqual($"{prefix}10002", "GOMEZ");
        }
        
        [Fact]
        public void TestNestedIfWithEmptyElse()
        {
            Code = @"
                extern func int NPC_IsDead(var instance par0);

                class C_NPC { var int data [200]; };

                instance self(C_NPC) {};
                instance other(C_NPC) {};
                instance victim(C_NPC) {};

                func void testFunc ()
                {
                    if ( NPC_IsDead(self) == 1 )
                    {
                        if ( NPC_IsDead(other) == 0)
                        {
                            if ( NPC_IsDead(victim) == 1)
                            {
                                return; 
                            }; //#0003_endif
                        }
                        else //#0002_else
                        {
                        
                        };//#0002_endif
                    }; //#0001_endif
                };
            ";
            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // if ( NPC_IsDead(self) == 1 ) {
                new PushInt(1),
                new PushInstance(Ref("self")),
                new CallExternal(Ref("NPC_IsDead")),
                new Equal(),
                new JumpIfToLabel("#0001_endif"),
                
                // if ( NPC_IsDead(other) == 0) {
                new PushInt(0),
                new PushInstance(Ref("other")),
                new CallExternal(Ref("NPC_IsDead")),
                new Equal(),
                new JumpIfToLabel("#0002_else"),
                
                // if ( NPC_IsDead(victim) == 1 ) {
                new PushInt(1),
                new PushInstance(Ref("victim")),
                new CallExternal(Ref("NPC_IsDead")),
                new Equal(),
                new JumpIfToLabel("#0003_endif"),
 
                // return; 
                new Ret(),
                            
                // };
                new AssemblyLabel("#0003_endif"),

                new JumpToLabel("#0002_endif"),
                // } else {
                new AssemblyLabel("#0002_else"),
                    
                    
                // };
                new AssemblyLabel("#0002_endif"),

                // };
                new AssemblyLabel("#0001_endif"),
                new Ret(),
            };
            AssertInstructionsMatch();

            ExpectedSymbols = new List<Symbol>
            {
                Ref("NPC_IsDead"),
                Ref("NPC_IsDead.par0"),
                Ref("C_NPC"),
                Ref("C_NPC.data"),
                Ref("self"),
                Ref("other"),
                Ref("victim"),
                Ref("testFunc"),
            };
            AssertSymbolsMatch();
        }
        
        [Fact]
        public void TestIfInstructionFull()
        {
            Code = @"
                class Pet {
                    var string name;
                };

                class C_NPC {
                    var float varfloat;
                    var int varint;
                    var string varstring;
                    var Pet varclass;
                    var func varfunc;
                    var int data[191];
                };
                
                var float varfloat;
                var int varint;
                var string varstring;
                var C_NPC varclass;
                var func varfunc;
                prototype varprototype(C_NPC) {};
                instance varinstance(C_NPC) {};
                instance varinstance2(varprototype) {};
                instance varinstance3(varprototype);
                func int retint() {};
                
                
                func void testFunc() {
                    if(varint) {};
                    if(varclass) {};
                    if(varprototype) {};
                    if(varinstance) {};
                    if(varinstance2) {};
                    
                    if(retint()) {};
                    
                    if(varinstance.varint) {};
                    if(varinstance.varclass) {};
                };

            ";
            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // if(varint)
                new PushVar(Ref("varint")),
                new JumpIfToLabel("#0001_endif"),
                // endif
                new AssemblyLabel("#0001_endif"),
                
                // if(varclass)
                new PushInt(RefIndex("varclass")),
                new JumpIfToLabel("#0002_endif"),
                // endif
                new AssemblyLabel("#0002_endif"),
                
                // if(varprototype)
                new PushInt(RefIndex("varprototype")),
                new JumpIfToLabel("#0003_endif"),
                // endif
                new AssemblyLabel("#0003_endif"),
                
                // if(varinstance)
                new PushInt(RefIndex("varinstance")),
                new JumpIfToLabel("#0004_endif"),
                // endif
                new AssemblyLabel("#0004_endif"),
                
                // if(varinstance2)
                new PushInt(RefIndex("varinstance2")),
                new JumpIfToLabel("#0005_endif"),
                // endif
                new AssemblyLabel("#0005_endif"),
                        
                // if(retint())
                new Call(Ref("retint")),
                new JumpIfToLabel("#0006_endif"),
                // endif
                new AssemblyLabel("#0006_endif"),
                        
                // if(varinstance.varint)
                new SetInstance(Ref("varinstance")),
                new PushVar(Ref("C_NPC.varint")),
                new JumpIfToLabel("#0007_endif"),
                // endif
                new AssemblyLabel("#0007_endif"),
                
                // if(varinstance.varclass)
                new SetInstance(Ref("varinstance")),
                new PushInt(RefIndex("C_NPC.varclass")),
                new JumpIfToLabel("#0008_endif"),
                // endif
                new AssemblyLabel("#0008_endif"),
                
                new Ret()
            };
            AssertInstructionsMatch();

            Instructions = GetExecBlockInstructions("varinstance");
            ExpectedInstructions = new List<AssemblyElement>
            {
                new Ret()
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("varinstance2");
            ExpectedInstructions = new List<AssemblyElement>
            {
                new Call(Ref("varprototype")),

                new Ret()
            };
            AssertInstructionsMatch();

            Instructions = GetExecBlockInstructions("varinstance3");
            ExpectedInstructions = new List<AssemblyElement>
            {
                new Call(Ref("varprototype")),

                new Ret()
            };
            AssertInstructionsMatch();
            
            ExpectedSymbols = new List<Symbol>
            {
                Ref("Pet"),
                Ref("Pet.name"),
                Ref("C_NPC"),
                Ref("C_NPC.varfloat"),
                Ref("C_NPC.varint"),
                Ref("C_NPC.varstring"),
                Ref("C_NPC.varclass"),
                Ref("C_NPC.varfunc"),
                Ref("C_NPC.data"),
                Ref("varfloat"),
                Ref("varint"),
                Ref("varstring"),
                Ref("varclass"),
                Ref("varfunc"),
                Ref("varprototype"),
                Ref("varinstance"),
                Ref("varinstance2"),
                Ref("varinstance3"),
                Ref("retint"),
                Ref("testFunc"),
                
            };
            AssertSymbolsMatch();
        }
        
        [Fact]
        public void TestIfAndElseInstruction()
        {
            Code = @"
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
            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // if ( 1 < 2 )
                new PushInt(2),
                new PushInt(1),
                new Less(),
                new JumpIfToLabel("#0001_else"),
                // a = 3;
                new PushInt(3),
                new PushVar(Ref("a")),
                new Assign(),
                new JumpToLabel("#0001_endif"),
                // if end
                // else start
                new AssemblyLabel("#0001_else"),
                // a = 4;
                new PushInt(4),
                new PushVar(Ref("a")),
                new Assign(),
                // else and if end
                new AssemblyLabel("#0001_endif"),
                new Ret()
            };
            AssertInstructionsMatch();

            ExpectedSymbols = new List<Symbol>
            {
                Ref("a"),
                Ref("testFunc"),
            };
            AssertSymbolsMatch();
        }

        [Fact]
        public void TestIfElseInstruction()
        {
            Code = @"
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
            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // if ( 1 < 2 )
                new PushInt(2),
                new PushInt(1),
                new Less(),
                new JumpIfToLabel("#0001_else_if_1"),
                // a = 3;
                new PushInt(3),
                new PushVar(Ref("a")),
                new Assign(),
                new JumpToLabel("#0001_endif"),
                // if end
                new AssemblyLabel("#0001_else_if_1"),
                // else if ( 4 < 5 )
                new PushInt(5),
                new PushInt(4),
                new Less(),
                new JumpIfToLabel("#0001_else"),
                // a = 6;
                new PushInt(6),
                new PushVar(Ref("a")),
                new Assign(),
                new JumpToLabel("#0001_endif"),
                // else if end
                // else start
                new AssemblyLabel("#0001_else"),
                //a = 7;
                new PushInt(7),
                new PushVar(Ref("a")),
                new Assign(),
                // else end if end
                new AssemblyLabel("#0001_endif"),
                new Ret()
            };
            AssertInstructionsMatch();

            ExpectedSymbols = new List<Symbol>
            {
                Ref("a"),
                Ref("testFunc"),
            };
            AssertSymbolsMatch();
        }
        
        [Fact]
        public void TestFuncCalls()
        {
            Code = @"
                class person {
                    var int age;
                };
                
                func void firstFunc (var person par) {};
                func void secondFunc (var int par) {};
                func void thirdFunc (var float par) {};
                func void fourthFunc (var string par) {};
                
                var person a;
                var int b;
                var float c;
                var string d;
                
                func void testFunc () {
                    var person e;
                    var int f;
                    var float g;
                    var string h;
                    
                    firstFunc(a);
                    //firstFunc(b);
                    //firstFunc(c);
                    //firstFunc(d);
                    firstFunc(e);
                    //firstFunc(f);
                    //firstFunc(g);
                    //firstFunc(h);
                        
                    secondFunc(a);
                    secondFunc(b);
                    //secondFunc(c);
                    //secondFunc(d);
                    secondFunc(e);
                    secondFunc(f);
                    //secondFunc(g);
                    //secondFunc(h);
                    
                    
                    //thirdFunc(a);
                    //thirdFunc(b);
                    thirdFunc(c);
                    //thirdFunc(d);
                    //thirdFunc(e);
                    //thirdFunc(f);
                    thirdFunc(g);
                    //thirdFunc(h);
                    
                    //fourthFunc(a);
                    //fourthFunc(b);
                    //fourthFunc(c);
                    fourthFunc(d);
                    //fourthFunc(e);
                    //fourthFunc(f);
                    //fourthFunc(g);
                    fourthFunc(h);
                    
                };
            ";
            
            Instructions = GetExecBlockInstructions("firstFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // parameters
                new PushInstance(Ref("firstFunc.par")),
                new AssignInstance(),
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("secondFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // parameters
                new PushVar(Ref("secondFunc.par")),
                new Assign(),
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("thirdFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // parameters
                new PushVar(Ref("thirdFunc.par")),
                new AssignFloat(),
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("fourthFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // parameters
                new PushVar(Ref("fourthFunc.par")),
                new AssignString(),
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // firstFunc(a);
                new PushInstance(Ref("a")),
                new Call(Ref("firstFunc")),
                
                // firstFunc(e);
                new PushInstance(Ref("testFunc.e")),
                new Call(Ref("firstFunc")),
                
                // secondFunc(a);
                new PushInt(RefIndex("a")),
                new Call(Ref("secondFunc")),
                
                // secondFunc(b);
                new PushVar(Ref("b")),
                new Call(Ref("secondFunc")),
                
                // secondFunc(e);
                new PushInt(RefIndex("testFunc.e")),
                new Call(Ref("secondFunc")),
                
                // secondFunc(f);
                new PushVar(Ref("testFunc.f")),
                new Call(Ref("secondFunc")),
                
                // thirdFunc(c);
                new PushVar(Ref("c")),
                new Call(Ref("thirdFunc")),
                
                // thirdFunc(c);
                new PushVar(Ref("testFunc.g")),
                new Call(Ref("thirdFunc")),
                
                // fourthFunc(d);
                new PushVar(Ref("d")),
                new Call(Ref("fourthFunc")),
                
                // fourthFunc(h);
                new PushVar(Ref("testFunc.h")),
                new Call(Ref("fourthFunc")),
                
                new Ret(),
            };
            AssertInstructionsMatch();

            ExpectedSymbols = new List<Symbol>
            {
                Ref("person"),
                Ref("person.age"),
                Ref("firstFunc"),
                Ref("firstFunc.par"),
                Ref("secondFunc"),
                Ref("secondFunc.par"),
                Ref("thirdFunc"),
                Ref("thirdFunc.par"),
                Ref("fourthFunc"),
                Ref("fourthFunc.par"),
                Ref("a"),
                Ref("b"),
                Ref("c"),
                Ref("d"),
                Ref("testFunc"),
                Ref("testFunc.e"),
                Ref("testFunc.f"),
                Ref("testFunc.g"),
                Ref("testFunc.h"),
            };
            AssertSymbolsMatch();
        }
        
        [Fact]
        public void TestFuncCallsWithArguments()
        {
            Code = @"
                extern func int WLD_DetectPlayer(var instance par0);

                class C_NPC { var int data [200]; };

                // var void varvoid;
                var float varfloat;
                var int varint;
                var string varstring;
                // var class varclass;
                var C_NPC varclass;
                var func varfunc;
                // var prototype varprototype;
                prototype varprototype(C_NPC) {};
                // var instance varinstance;
                instance varinstance(C_NPC) {};
                instance varinstance2(varprototype) {};
                
                
                // func void parvoid(var void par) {};
                func void parfloat(var float par) {};
                func void parint(var int par) {};
                func void parstring(var string par) {};
                // func void parclass(var class par) {};
                func void parC_NPC(var C_NPC par) {};
                func void parfunc(var func par) {};
                // func void parprototype(var prototype par) {};
                // func void parprototype2(var varprototype par) {};
                // func void parinstance(var instance par) {};
                // func void parinstance2(var varinstance par) {};
                // func void parinstance3(var varinstance2 par) {};
                
                
                func void testFunc() {
                    parfloat(varfloat);
                    // parfloat(varint);
                    // parfloat(varstring);
                    // parfloat(varclass);
                    // parfloat(varfunc);
                    // parfloat(varprototype);
                    // parfloat(varinstance);
                    // parfloat(varinstance2);
                    // parfloat(parfloat);
                    // parfloat(NULL);
                    // parfloat(NOFUNC);
                    
                    // parint(varfloat);
                    parint(varint);
                    // parint(varstring);
                    parint(varclass);
                    // parint(varfunc);
                    parint(varprototype);
                    parint(varinstance);
                    parint(varinstance2);
                    // parint(parint);
                    // parint(NULL);
                    // parint(NOFUNC);
                    
                    // parstring(varfloat);
                    // parstring(varint);
                    parstring(varstring);
                    // parstring(varclass);
                    // parstring(varfunc);
                    // parstring(varprototype);
                    // parstring(varinstance);
                    // parstring(varinstance2);
                    // parstring(parstring);
                    // parstring(NULL);
                    // parstring(NOFUNC);
                
                    // parC_NPC(varfloat);
                    // parC_NPC(varint);
                    // parC_NPC(varstring);
                    parC_NPC(varclass);
                    // parC_NPC(varfunc);
                    // parC_NPC(varprototype);
                    parC_NPC(varinstance);
                    parC_NPC(varinstance2);
                    // parC_NPC(parC_NPC);
                    parC_NPC(NULL);
                    // parC_NPC(NOFUNC);
                    
                    // parfunc(varfloat);
                    // parfunc(varint);
                    // parfunc(varstring);
                    parfunc(varclass);
                    parfunc(varfunc);
                    parfunc(varprototype);
                    parfunc(varinstance);
                    parfunc(varinstance2);
                    parfunc(parfunc);
                    // parfunc(NULL);
                    parfunc(NOFUNC);
                    
                    
                    // WLD_DetectPlayer(varfloat);
                    // WLD_DetectPlayer(varint);
                    // WLD_DetectPlayer(varstring);
                    WLD_DetectPlayer(varclass);
                    // WLD_DetectPlayer(varfunc);
                    // WLD_DetectPlayer(varprototype);
                    WLD_DetectPlayer(varinstance);
                    WLD_DetectPlayer(varinstance2);
                    // WLD_DetectPlayer(parfunc);
                    // WLD_DetectPlayer(WLD_DetectPlayer);
                    WLD_DetectPlayer(NULL);
                    //WLD_DetectPlayer(NOFUNC);
                };
            ";
            
            Instructions = GetExecBlockInstructions("varprototype");
            ExpectedInstructions = new List<AssemblyElement>
            {
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("varinstance");
            ExpectedInstructions = new List<AssemblyElement>
            {
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("varinstance2");
            ExpectedInstructions = new List<AssemblyElement>
            {
                new Call(Ref("varprototype")),
                
                new Ret(),
            };
            AssertInstructionsMatch();
           
            Instructions = GetExecBlockInstructions("parfloat");
            ExpectedInstructions = new List<AssemblyElement>
            {
                new PushVar(Ref("parfloat.par")),
                new AssignFloat(),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("parint");
            ExpectedInstructions = new List<AssemblyElement>
            {
                new PushVar(Ref("parint.par")),
                new Assign(),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("parstring");
            ExpectedInstructions = new List<AssemblyElement>
            {
                new PushVar(Ref("parstring.par")),
                new AssignString(),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("parC_NPC");
            ExpectedInstructions = new List<AssemblyElement>
            {
                new PushInstance(Ref("parC_NPC.par")),
                new AssignInstance(),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("parfunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                new PushVar(Ref("parfunc.par")),
                new AssignFunc(),
                
                new Ret(),
            };
            AssertInstructionsMatch();

            Instructions = GetExecBlockInstructions("testfunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // parfloat(varfloat);
                new PushVar(Ref("varfloat")),
                new Call(Ref("parfloat")),

                // parint(varint);
                new PushVar(Ref("varint")),
                new Call(Ref("parint")),
                
                // parint(varclass);
                new PushInt(RefIndex("varclass")),
                new Call(Ref("parint")),
                
                // parint(varprototype);
                new PushInt(RefIndex("varprototype")),
                new Call(Ref("parint")),
                
                // parint(varinstance);
                new PushInt(RefIndex("varinstance")),
                new Call(Ref("parint")),
                
                // parint(varinstance2);
                new PushInt(RefIndex("varinstance2")),
                new Call(Ref("parint")),

                // parstring(varstring);
                new PushVar(Ref("varstring")),
                new Call(Ref("parstring")),

                // parC_NPC(varclass);
                new PushInstance(Ref("varclass")),
                new Call(Ref("parC_NPC")),
                
                // parC_NPC(varinstance);
                new PushInstance(Ref("varinstance")),
                new Call(Ref("parC_NPC")),
                
                // parC_NPC(varinstance2);
                new PushInstance(Ref("varinstance2")),
                new Call(Ref("parC_NPC")),
                
                // parC_NPC(NULL);
                new PushNullInstance(),
                new Call(Ref("parC_NPC")),

                // parfunc(varclass);
                new PushInt(RefIndex("varclass")),
                new Call(Ref("parfunc")),
                
                // parfunc(varfunc);
                new PushInt(RefIndex("varfunc")),
                new Call(Ref("parfunc")),
                
                // parfunc(varprototype);
                new PushInt(RefIndex("varprototype")),
                new Call(Ref("parfunc")),
                
                // parfunc(varinstance);
                new PushInt(RefIndex("varinstance")),
                new Call(Ref("parfunc")),
                
                // parfunc(varinstance2);
                new PushInt(RefIndex("varinstance2")),
                new Call(Ref("parfunc")),
                
                // parfunc(parfunc);
                new PushInt(RefIndex("parfunc")),
                new Call(Ref("parfunc")),
                
                // parfunc(NOFUNC);
                new PushInt(-1),
                new Call(Ref("parfunc")),
                
                // WLD_DetectPlayer(varclass);
                new PushInstance(Ref("varclass")),
                new CallExternal(Ref("WLD_DetectPlayer")),
                
                // WLD_DetectPlayer(varinstance);
                new PushInstance(Ref("varinstance")),
                new CallExternal(Ref("WLD_DetectPlayer")),
                
                // WLD_DetectPlayer(varinstance2);
                new PushInstance(Ref("varinstance2")),
                new CallExternal(Ref("WLD_DetectPlayer")),
                
                // WLD_DetectPlayer(NULL);
                new PushNullInstance(),
                new CallExternal(Ref("WLD_DetectPlayer")),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            ExpectedSymbols = new List<Symbol>
            {
                Ref("WLD_DetectPlayer"),
                Ref("WLD_DetectPlayer.par0"),
                Ref("C_NPC"),
                Ref("C_NPC.data"),
                Ref("varfloat"),
                Ref("varint"),
                Ref("varstring"),
                Ref("varclass"),
                Ref("varfunc"),
                Ref("varprototype"),
                Ref("varinstance"),
                Ref("varinstance2"),
                Ref("parfloat"),
                Ref("parfloat.par"),
                Ref("parint"),
                Ref("parint.par"),
                Ref("parstring"),
                Ref("parstring.par"),
                Ref("parC_NPC"),
                Ref("parC_NPC.par"),
                Ref("parfunc"),
                Ref("parfunc.par"),
                Ref("testfunc"),

            };
            AssertSymbolsMatch();
        }
        
        [Fact]
        public void TestFuncCallsWithLiteralArguments()
        {
            Code = @"
                func void parfloat(var float par) {};
                func void parint(var int par) {};
                func void parstring(var string par) {};
                
                
                func void testFunc() {
                    parfloat(0);
                    parfloat(2.5);
                    parfloat(5);
                    parfloat(-5);
                    parfloat(-2.5);
                    parfloat(+100);
                    
                    parint(0);
                    parint(5);
                    parint(-5);
                    parint(+100);
                    
                    parint(0 - -1);
                    parint(0 - +1);
                    parint(0 - 1);
                
                    parint(5 - -3);
                    parint(5 - +3);
                    parint(5 - 3);
                    
                    parint(-5 - -4);
                    parint(-5 - +4);
                    parint(-5 - 4);
                    
                    parint(+100 - -6);
                    parint(+100 - +6);
                    parint(+100 - 6);
                
                    parstring(""100"");
                };
            ";
     

            char prefix = (char)255;
            
            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // parfloat(0);
                new PushInt(0),
                new Call(Ref("parfloat")),
                
                // parfloat(2.5);
                new PushInt(1075838976),
                new Call(Ref("parfloat")),
                
                // parfloat(5);
                new PushInt(1084227584),
                new Call(Ref("parfloat")),
                
                // parfloat(-5);
                new PushInt(-1063256064),
                new Call(Ref("parfloat")),
                
                // parfloat(-2.5);
                new PushInt(-1071644672),
                new Call(Ref("parfloat")),
                
                // parfloat(+100);
                new PushInt(1120403456),
                new Call(Ref("parfloat")),
                    
                // parint(0);
                new PushInt(0),
                new Call(Ref("parint")),
                
                // parint(5);
                new PushInt(5),
                new Call(Ref("parint")),
                
                // parint(-5);
                new PushInt(5),
                new Minus(),
                new Call(Ref("parint")),
                
                // parint(+100);
                new PushInt(100),
                new Plus(),
                new Call(Ref("parint")),
                    
                // parint(0 - -1);
                new PushInt(1),
                new Minus(),
                new PushInt(0),
                new Subtract(),
                new Call(Ref("parint")),
                
                // parint(0 - +1);
                new PushInt(1),
                new Plus(),
                new PushInt(0),
                new Subtract(),
                new Call(Ref("parint")),
                
                // parint(0 - 1);
                new PushInt(1),
                new PushInt(0),
                new Subtract(),
                new Call(Ref("parint")),
                
                // parint(5 - -3);
                new PushInt(3),
                new Minus(),
                new PushInt(5),
                new Subtract(),
                new Call(Ref("parint")),
                
                // parint(5 - +3);
                new PushInt(3),
                new Plus(),
                new PushInt(5),
                new Subtract(),
                new Call(Ref("parint")),
                
                // parint(5 - 3);
                new PushInt(3),
                new PushInt(5),
                new Subtract(),
                new Call(Ref("parint")),
                    
                // parint(-5 - -4);
                new PushInt(4),
                new Minus(),
                new PushInt(5),
                new Minus(),
                new Subtract(),
                new Call(Ref("parint")),
                
                // parint(-5 - +4);
                new PushInt(4),
                new Plus(),
                new PushInt(5),
                new Minus(),
                new Subtract(),
                new Call(Ref("parint")),
                
                // parint(-5 - 4);
                new PushInt(4),
                new PushInt(5),
                new Minus(),
                new Subtract(),
                new Call(Ref("parint")),
                    
                // parint(+100 - -6);
                new PushInt(6),
                new Minus(),
                new PushInt(100),
                new Plus(),
                new Subtract(),
                new Call(Ref("parint")),
                
                // parint(+100 - +6);
                new PushInt(6),
                new Plus(),
                new PushInt(100),
                new Plus(),
                new Subtract(),
                new Call(Ref("parint")),
                
                // parint(+100 - 6);
                new PushInt(6),
                new PushInt(100),
                new Plus(),
                new Subtract(),
                new Call(Ref("parint")),
                
                // parstring(""100"");
                new PushVar(Ref($"{prefix}10000")),
                new Call(Ref("parstring")),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            ExpectedSymbols = new List<Symbol>
            {
                Ref("parfloat"),
                Ref("parfloat.par"),
                Ref("parint"),
                Ref("parint.par"),
                Ref("parstring"),
                Ref("parstring.par"),
                Ref("testFunc"),
                Ref($"{prefix}10000"),
            };
            AssertSymbolsMatch();
        }
        
        [Fact]
        public void TestFuncCallsWithAttributeArguments()
        {
            Code = @"
                extern func int WLD_DetectPlayer(var instance par0);

                class C_NPC {
                    var float varfloat;
                    var int varint;
                    var string varstring;
                    var C_NPC varclass;
                    var func varfunc;

                    var int data[191]; // because C_NPC class is supposed to have 800 bytes
                };
                
                instance varinstance(C_NPC) {};
                
                func void parfloat(var float par) {};
                func void parint(var int par) {};
                func void parstring(var string par) {};
                func void parC_NPC(var C_NPC par) {};
                func void parfunc(var func par) {};
                
                
                
                func void testFunc() {
                    parfloat(varinstance.varfloat);
                
                    parint(varinstance.varint);
                    parint(varinstance.varclass);
                    
                    parstring(varinstance.varstring);
                
                    parC_NPC(varinstance.varclass);
                
                    parfunc(varinstance.varclass);
                    parfunc(varinstance.varfunc);
                
                    WLD_DetectPlayer(varinstance.varclass);
                };
            ";
            
            
            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // parfloat(varinstance.varfloat);
                new SetInstance(Ref("varinstance")),
                new PushVar(Ref("C_NPC.varfloat")),
                new Call(Ref("parfloat")),
                
                // parint(varinstance.varint);
                new SetInstance(Ref("varinstance")),
                new PushVar(Ref("C_NPC.varint")),
                new Call(Ref("parint")),
                
                // parint(varinstance.varclass);
                new SetInstance(Ref("varinstance")),
                new PushInt(RefIndex("C_NPC.varclass")),
                new Call(Ref("parint")),
                    
                // parstring(varinstance.varstring);
                new SetInstance(Ref("varinstance")),
                new PushVar(Ref("C_NPC.varstring")),
                new Call(Ref("parstring")),
                
                // parC_NPC(varinstance.varclass);
                new SetInstance(Ref("varinstance")),  //TODO original compiler doesn't add SetInstance here, but it would be good if we had it
                new PushInstance(Ref("C_NPC.varclass")), // SetInstance before
                new Call(Ref("parC_NPC")),
                
                // parfunc(varinstance.varclass);
                new SetInstance(Ref("varinstance")),  //TODO original compiler doesn't add SetInstance here, but it would be good if we had it
                new PushInt(RefIndex("C_NPC.varclass")), // SetInstance before
                new Call(Ref("parfunc")),
                
                // parfunc(varinstance.varfunc);
                new SetInstance(Ref("varinstance")),  //TODO original compiler doesn't add SetInstance here, but it would be good if we had it
                new PushInt(RefIndex("C_NPC.varfunc")), // SetInstance before
                new Call(Ref("parfunc")),
                
                // WLD_DetectPlayer(varinstance.varclass);
                new SetInstance(Ref("varinstance")),  //TODO original compiler doesn't add SetInstance here, but it would be good if we had it
                new PushInstance(Ref("C_NPC.varclass")), // SetInstance before
                new CallExternal(Ref("WLD_DetectPlayer")),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            ExpectedSymbols = new List<Symbol>
            {
                Ref("WLD_DetectPlayer"),
                Ref("WLD_DetectPlayer.par0"),
                Ref("C_NPC"),
                Ref("C_NPC.varfloat"),
                Ref("C_NPC.varint"),
                Ref("C_NPC.varstring"),
                Ref("C_NPC.varclass"),
                Ref("C_NPC.varfunc"),
                Ref("C_NPC.data"),
                Ref("varinstance"),
                Ref("parfloat"),
                Ref("parfloat.par"),
                Ref("parint"),
                Ref("parint.par"),
                Ref("parstring"),
                Ref("parstring.par"),
                Ref("parC_NPC"),
                Ref("parC_NPC.par"),
                Ref("parfunc"),
                Ref("parfunc.par"),
                Ref("testFunc"),
            };
            AssertSymbolsMatch();
        }
        
        
        [Fact]
        public void TestMultiparameterFuncCall()
        {
            Code = @"
                class person {
                    var int age;
                };
                
                var person a;
                var int b;
                var float c;
                var string d;
                
                
                func void firstFunc (var person par0, var int par1, var float par2, var string par3, var person par4) {};
                
                func void testFunc () {
                    firstFunc(a, a, c, d, a);
                    firstFunc(a, b, c, d, a);
                };
            ";
            
            Instructions = GetExecBlockInstructions("firstFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // parameters
                new PushInstance(Ref("firstFunc.par4")),
                new AssignInstance(),
                
                new PushVar(Ref("firstFunc.par3")),
                new AssignString(),
                
                new PushVar(Ref("firstFunc.par2")),
                new AssignFloat(),
                
                new PushVar(Ref("firstFunc.par1")),
                new Assign(),
                
                new PushInstance(Ref("firstFunc.par0")),
                new AssignInstance(),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // firstFunc(a, a, c, d, a);
                new PushInstance(Ref("a")),
                new PushInt(RefIndex("a")),
                new PushVar(Ref("c")),
                new PushVar(Ref("d")),
                new PushInstance(Ref("a")),
                new Call(Ref("firstFunc")),
                
                // firstFunc(a, b, c, d, a);
                new PushInstance(Ref("a")),
                new PushVar(Ref("b")),
                new PushVar(Ref("c")),
                new PushVar(Ref("d")),
                new PushInstance(Ref("a")),
                new Call(Ref("firstFunc")),
                
                new Ret(),
            };
            AssertInstructionsMatch();

            ExpectedSymbols = new List<Symbol>
            {
                Ref("person"),
                Ref("person.age"),
                Ref("a"),
                Ref("b"),
                Ref("c"),
                Ref("d"),
                Ref("firstFunc"),
                Ref("firstFunc.par0"),
                Ref("firstFunc.par1"),
                Ref("firstFunc.par2"),
                Ref("firstFunc.par3"),
                Ref("firstFunc.par4"),
                Ref("testFunc"),
            };
            AssertSymbolsMatch(); 
        }
        
        [Fact]
        public void TestFullReturn()
        {
            Code = @"
                class C_NPC {
                    var float varfloat;
                    var int varint;
                    var string varstring;
                    var C_NPC varclass;
                    var func varfunc;
                    var int data[191]; // because C_NPC class is supposed to have 800 bytes
                };
                
                var float varfloat;
                var int varint;
                var string varstring;
                var C_NPC varclass;
                var func varfunc;
                prototype varprototype(C_NPC) {};
                instance varinstance(C_NPC) {};
                
                func void retvoid() { return; };
                
                func float retfloat00() { return; };
                // func float retfloat01() { return 0; };
                // func float retfloat02() { return ""zero""; };
                // func float retfloat03() { return 0.5; };
                // func float retfloat04() { return varfloat; };
                // func float retfloat05() { return varint; };
                // func float retfloat06() { return varstring; };
                // func float retfloat07() { return varclass; };
                // func float retfloat08() { return varfunc; };
                // func float retfloat09() { return varprototype; };
                // func float retfloat10() { return varinstance; };
                // func float retfloat11() { return varinstance.varfloat; };
                // func float retfloat12() { return varinstance.varint; };
                // func float retfloat13() { return varinstance.varstring; };
                // func float retfloat14() { return varinstance.varclass; };
                // func float retfloat15() { return varinstance.varfunc; };
                
                // func int retint00() { return; };
                func int retint01() { return 0; };
                // func int retint02() { return ""zero""; };
                // func int retint03() { return 0.5; };
                // func int retint04() { return varfloat; };
                func int retint05() { return varint; };
                // func int retint06() { return varstring; };
                func int retint07() { return varclass; };
                // func int retint08() { return varfunc; };
                func int retint09() { return varprototype; };
                func int retint10() { return varinstance; };
                // func int retint11() { return varinstance.varfloat; };
                func int retint12() { return varinstance.varint; };
                // func int retint13() { return varinstance.varstring; };
                func int retint14() { return varinstance.varclass; };
                // func int retint15() { return varinstance.varfunc; };
                
                // func string retstring00() { return; };
                // func string retstring01() { return 0; };
                func string retstring02() { return ""zero""; };
                // func string retstring03() { return 0.5; };
                // func string retstring04() { return varfloat; };
                // func string retstring05() { return varint; };
                func string retstring06() { return varstring; };
                // func string retstring07() { return varclass; };
                // func string retstring08() { return varfunc; };
                // func string retstring09() { return varprototype; };
                // func string retstring10() { return varinstance; };
                // func string retstring11() { return varinstance.varfloat; };
                // func string retstring12() { return varinstance.varint; };
                func string retstring13() { return varinstance.varstring; };
                // func string retstring14() { return varinstance.varclass; };
                // func string retstring15() { return varinstance.varfunc; };
                
                func C_NPC retC_NPC00() { return; };
                // func C_NPC retC_NPC01() { return 0; };
                // func C_NPC retC_NPC02() { return ""zero""; };
                // func C_NPC retC_NPC03() { return 0.5; };
                // func C_NPC retC_NPC04() { return varfloat; };
                // func C_NPC retC_NPC05() { return varint; };
                // func C_NPC retC_NPC06() { return varstring; };
                // func C_NPC retC_NPC07() { return varclass; };
                // func C_NPC retC_NPC08() { return varfunc; };
                // func C_NPC retC_NPC09() { return varprototype; };
                // func C_NPC retC_NPC10() { return varinstance; };
                // func C_NPC retC_NPC11() { return varinstance.varfloat; };
                // func C_NPC retC_NPC12() { return varinstance.varint; };
                // func C_NPC retC_NPC13() { return varinstance.varstring; };
                // func C_NPC retC_NPC14() { return varinstance.varclass; };
                // func C_NPC retC_NPC15() { return varinstance.varfunc; };
            ";

            char prefix = (char) 255;
            
            Instructions = GetExecBlockInstructions("retvoid");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // return;
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("retfloat00");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // return;
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("retint01");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // return 0;
                new PushInt(0),
                new Ret(),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("retint05");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // return varint;
                new PushVar(Ref("varint")),
                new Ret(),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("retint07");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // return varclass;
                new PushInt(RefIndex("varclass")),
                new Ret(),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("retint09");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // return varprototype;
                new PushInt(RefIndex("varprototype")),
                new Ret(),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("retint10");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // return varinstance;
                new PushInt(RefIndex("varinstance")),
                new Ret(),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("retint12");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // return varinstance.varint;
                new SetInstance(Ref("varinstance")),
                new PushVar(Ref("C_NPC.varint")),
                new Ret(),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("retint14");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // return varinstance.varclass;
                new SetInstance(Ref("varinstance")),
                new PushInt(RefIndex("C_NPC.varclass")),
                new Ret(),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            
            Instructions = GetExecBlockInstructions("retstring02");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // return "zero";
                new PushVar(Ref($"{prefix}10000")),
                new Ret(),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("retstring06");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // return varstring;
                new PushVar(Ref("varstring")),
                new Ret(),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("retstring13");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // return varinstance.varstring;
                new SetInstance(Ref("varinstance")),
                new PushVar(Ref("C_NPC.varstring")),
                new Ret(),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("retC_NPC00");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // return;
                new Ret(),
            };
            AssertInstructionsMatch();
            
            ExpectedSymbols = new List<Symbol>
            {
                Ref("C_NPC"),
                Ref("C_NPC.varfloat"),
                Ref("C_NPC.varint"),
                Ref("C_NPC.varstring"),
                Ref("C_NPC.varclass"),
                Ref("C_NPC.varfunc"),
                Ref("C_NPC.data"),
                Ref("varfloat"),
                Ref("varint"),
                Ref("varstring"),
                Ref("varclass"),
                Ref("varfunc"),
                Ref("varprototype"),
                Ref("varinstance"),
                Ref("retvoid"),
                Ref("retfloat00"),
                Ref("retint01"),
                Ref("retint05"),
                Ref("retint07"),
                Ref("retint09"),
                Ref("retint10"),
                Ref("retint12"),
                Ref("retint14"),
                Ref("retstring02"),
                Ref("retstring06"),
                Ref("retstring13"),
                Ref("retC_NPC00"),
                Ref($"{prefix}10000"),
            };
            AssertSymbolsMatch(); 
        }
        
        [Fact]
        public void TestConstFunc()
        {
            Code = @"
                func int intFunc(var int par) {
                    return 0;
                };
                func void voidFunc() {};
                
                const func constIntFunc = intFunc; 
                const func constVoidFunc = voidFunc;
            ";
            ParseData();
            
            ExpectedSymbols = new List<Symbol>
            {
                Ref("intFunc"),
                Ref("intFunc.par"),
                Ref("voidFunc"),
                Ref("constIntFunc"),
                Ref("constVoidFunc"),
            };
            AssertSymbolsMatch(); 
        }
        
        [Fact]
        public void TestExternalFunc()
        {
            Code = @"
                extern func float IntToFloat(var int par0);

                func float floatFunc() {};

                func void testFunc() {
                    var int wait;
                    var float waitTime;
                    waitTime = 2.5;
                    waitTime = floatFunc();
                    waitTime = IntToFloat(wait);
                };
            ";
            
            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // waitTime = 2.5;
                new PushInt(1075838976),
                new PushVar(Ref("testFunc.waitTime")),
                new AssignFloat(),
                
                // waitTime = floatFunc();
                new Call(Ref("floatFunc")),
                new PushVar(Ref("testFunc.waitTime")),
                new AssignFloat(),
                
                // waitTime = IntToFloat(wait);
                new PushVar(Ref("testFunc.wait")),
                new CallExternal(Ref("IntToFloat")),
                new PushVar(Ref("testFunc.waitTime")),
                new AssignFloat(),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            
            ExpectedSymbols = new List<Symbol>
            {
                Ref("IntToFloat"),
                Ref("IntToFloat.par0"),
                Ref("floatFunc"),
                Ref("testFunc"),
                Ref("testFunc.wait"),
                Ref("testFunc.waitTime"),
            };
            AssertSymbolsMatch(); 
        }
        
        [Fact]
        public void TestLazyReferenceFunctionCall()
        {
            Code = @"
                class person {
                    var int age;
                };
                
                func void firstFunc (var int par) {};
                func void secondFunc (var person par) {};
    
                func void testFunc () {
                    firstFunc(a);
                    firstFunc(b);
                    firstFunc(8);

                    secondFunc(a);
                };
                
                var person a;
                var int b;
            ";
            
            Instructions = GetExecBlockInstructions("firstFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // parameters
                new PushVar(Ref("firstFunc.par")),
                new Assign(),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("secondFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // parameters
                new PushInstance(Ref("secondFunc.par")),
                new AssignInstance(),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // firstFunc(a);
                new PushInt(RefIndex("a")),
                new Call(Ref("firstFunc")),
                
                // firstFunc(b);
                new PushVar(Ref("b")),
                new Call(Ref("firstFunc")),
                
                // firstFunc(8);
                new PushInt(8),
                new Call(Ref("firstFunc")),
                
                // secondFunc(a);
                new PushInstance(Ref("a")),
                new Call(Ref("secondFunc")),
                
                new Ret(),
            };
            AssertInstructionsMatch();

            ExpectedSymbols = new List<Symbol>
            {
                Ref("person"),
                Ref("person.age"),
                Ref("firstFunc"),
                Ref("firstFunc.par"),
                Ref("secondFunc"),
                Ref("secondFunc.par"),
                Ref("testFunc"),
                Ref("a"),
                Ref("b"),
            };
            AssertSymbolsMatch(); 
        }
        
        
        
        [Fact]
        public void TestLazyReferenceExternalFunctionCall()
        {
            Code = @"
                extern func int NPC_HasNews(var instance par0, var int par1, var instance par2, var instance par3);

                class C_NPC { var int data [200]; };

                func int firstFunc(var C_NPC par0, var C_NPC par1, var C_NPC par2) {};

                func int testFunc()
                {
                    firstFunc(person, person, person);
                    NPC_HasNews(person, person, person, person);
                };
                
                instance person(C_NPC);
            ";
            
            Instructions = GetExecBlockInstructions("firstFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // parameters
                new PushInstance(Ref("firstFunc.par2")),
                new AssignInstance(),
                
                new PushInstance(Ref("firstFunc.par1")),
                new AssignInstance(),
                
                new PushInstance(Ref("firstFunc.par0")),
                new AssignInstance(),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            
            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // firstFunc(person, person, person);
                new PushInstance(Ref("person")),
                new PushInstance(Ref("person")),
                new PushInstance(Ref("person")),
                new Call(Ref("firstFunc")),
                
                // NPC_HasNews(person, person, person, person);
                new PushInstance(Ref("person")),
                new PushInt(RefIndex("person")),
                new PushInstance(Ref("person")),
                new PushInstance(Ref("person")),
                new CallExternal(Ref("NPC_HasNews")),

                new Ret(),
            };
            AssertInstructionsMatch();

            ExpectedSymbols = new List<Symbol>
            {
                Ref("NPC_HasNews"),
                Ref("NPC_HasNews.par0"),
                Ref("NPC_HasNews.par1"),
                Ref("NPC_HasNews.par2"),
                Ref("NPC_HasNews.par3"),
                
                Ref("C_NPC"),
                Ref("C_NPC.data"),
                Ref("firstFunc"),
                Ref("firstFunc.par0"),
                Ref("firstFunc.par1"),
                Ref("firstFunc.par2"),
                Ref("testFunc"),
                Ref("person"),
            };
            AssertSymbolsMatch(); 
        }
        
        
        [Fact]
        public void TestLazyReferenceAssignFunctionCall()
        {
            Code = @"
                class person {
                    var int age;
                };
                func int firstFunc (var int par) {
                    return par;
                };
                func int secondFunc (var person par) {
                    return par;
                };
                
                func void testFunc () {
                    var int c;
                
                    c = firstFunc(a);
                    c = firstFunc(b);
                    c = firstFunc(8);
                    
                    c = secondFunc(a);
                };
                
                var person a;
                var int b;
            ";
            
            Instructions = GetExecBlockInstructions("firstFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // parameters
                new PushVar(Ref("firstFunc.par")),
                new Assign(),
                
                // return par;
                new PushVar(Ref("firstFunc.par")),
                new Ret(),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            // TODO not working but probably never happens in Gothic code
            Instructions = GetExecBlockInstructions("secondFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // parameters
                new PushInstance(Ref("secondFunc.par")),
                new AssignInstance(),
                
                // return par;
                new PushInt(RefIndex("secondFunc.par")),
                new Ret(),
                
                new Ret(),
            };
            AssertInstructionsMatch();

            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // c = firstFunc(a);
                new PushInt(RefIndex("a")),
                new Call(Ref("firstFunc")),
                new PushVar(Ref("testFunc.c")),
                new Assign(),
                
                // c = firstFunc(b);
                new PushVar(Ref("b")),
                new Call(Ref("firstFunc")),
                new PushVar(Ref("testFunc.c")),
                new Assign(),
                
                // c = firstFunc(8);
                new PushInt(8),
                new Call(Ref("firstFunc")),
                new PushVar(Ref("testFunc.c")),
                new Assign(),
                
                // c = secondFunc(a);
                new PushInstance(Ref("a")),
                new Call(Ref("secondFunc")),
                new PushVar(Ref("testFunc.c")),
                new Assign(),
                
                new Ret(),
            };
            AssertInstructionsMatch();

            ExpectedSymbols = new List<Symbol>
            {
                Ref("person"),
                Ref("person.age"),
                Ref("firstFunc"),
                Ref("firstFunc.par"),
                Ref("secondFunc"),
                Ref("secondFunc.par"),
                Ref("testFunc"),
                Ref("testFunc.c"),
                Ref("a"),
                Ref("b"),
            };
            AssertSymbolsMatch(); 
        }
        [Fact]
        public void TestLazyReferenceAssignIntString()
        {
            Code = @"
                func void testFunc () {
                    var int e;
                    var string f;
                    
                    e = a;
                    f = b;
                    
                    e = c;
                    f = d;
                };
                
                const int a = 1;
                const string b = ""super"";
                
                var int c;
                var string d;
            ";
            
           
            
            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // e = a;
                new PushVar(Ref("a")),
                new PushVar(Ref("testFunc.e")),
                new Assign(),
                
                // f = b;
                new PushVar(Ref("b")),
                new PushVar(Ref("testFunc.f")),
                new AssignString(),
                
                // e = c;
                new PushVar(Ref("c")),
                new PushVar(Ref("testFunc.e")),
                new Assign(),
                
                // f = d;
                new PushVar(Ref("d")),
                new PushVar(Ref("testFunc.f")),
                new AssignString(),
                
                new Ret(),
            };
            AssertInstructionsMatch();

            ExpectedSymbols = new List<Symbol>
            {
                Ref("testFunc"),
                Ref("testFunc.e"),
                Ref("testFunc.f"),
                Ref("a"),
                Ref("b"),
                Ref("c"),
                Ref("d"),
            };
            AssertSymbolsMatch(); 
        }
        
        [Fact]
        public void TestLazyReferenceInsideIfCondition()
        {
            Code = @"
                func int intFunc(var int par) {
                    return 0;
                };
                
                func void testFunc () {
                    var int c;
                    
                    if (intFunc(d)) {
                        c = 0;
                    }else if (d == a) {
                        c = 1;
                    } else if (d == b) {
                        c = 2;
                    } else if (d == 100) {
                        c = 3;
                    } else {
                        c = d;
                    };
                
                    var int d;
                };
                
                const int a = 1;
                var int b;
            ";
            
            
            Instructions = GetExecBlockInstructions("intFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // parameters
                new PushVar(Ref("intFunc.par")),
                new Assign(),
                
                // return 0;
                new PushInt(0),
                new Ret(),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // if(intFunc(d))
                new PushVar(Ref("testFunc.d")),
                new Call(Ref("intFunc")),
                new JumpIfToLabel("#0001_else_if_1"),

                // c = 0;
                new PushInt(0),
                new PushVar(Ref("testFunc.c")),
                new Assign(),
                new JumpToLabel("#0001_endif"),
                
                // else if(d == a)
                new AssemblyLabel("#0001_else_if_1"),
                new PushVar(Ref("a")),
                new PushVar(Ref("testFunc.d")),
                new Equal(),
                new JumpIfToLabel("#0001_else_if_2"),
                
                // c = 1;
                new PushInt(1),
                new PushVar(Ref("testFunc.c")),
                new Assign(),
                new JumpToLabel("#0001_endif"),
                
                // else if(d == b)
                new AssemblyLabel("#0001_else_if_2"),
                new PushVar(Ref("b")),
                new PushVar(Ref("testFunc.d")),
                new Equal(),
                new JumpIfToLabel("#0001_else_if_3"),
                
                // c = 2;
                new PushInt(2),
                new PushVar(Ref("testFunc.c")),
                new Assign(),
                new JumpToLabel("#0001_endif"),
                
                // else if(d == 100)
                new AssemblyLabel("#0001_else_if_3"),
                new PushInt(100),
                new PushVar(Ref("testFunc.d")),
                new Equal(),
                new JumpIfToLabel("#0001_else"),
                
                // c = 3;
                new PushInt(3),
                new PushVar(Ref("testFunc.c")),
                new Assign(),
                new JumpToLabel("#0001_endif"),
                
                // else
                new AssemblyLabel("#0001_else"),
                
                // c = d;
                new PushVar(Ref("testFunc.d")),
                new PushVar(Ref("testFunc.c")),
                new Assign(),
                
                new AssemblyLabel("#0001_endif"),
                
                new Ret(),
            };
            AssertInstructionsMatch();

            ExpectedSymbols = new List<Symbol>
            {
                Ref("intFunc"),
                Ref("intFunc.par"),
                Ref("testFunc"),
                Ref("testFunc.c"),
                Ref("testFunc.d"),
                Ref("a"),
                Ref("b"),
            };
            AssertSymbolsMatch(); 
        }
        
        
        [Fact]
        public void TestLazyReferenceReturn()
        {
            Code = @"
                class person {
                    var int age;
                };
                
                func int firstFunc() {
                    return a;
                };
                
                func string secondFunc() {
                    return b;
                };
                
                func int thirdFunc() {
                    return c;
                };
                
                var int a;
                var string b;
                instance c(person);
            ";
            
            Instructions = GetExecBlockInstructions("firstFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // return a;
                new PushVar(Ref("a")),
                new Ret(),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("secondFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // return b;
                new PushVar(Ref("b")),
                new Ret(),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            Instructions = GetExecBlockInstructions("thirdFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // return c;
                new PushInt(RefIndex("c")),
                new Ret(),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            
            ExpectedSymbols = new List<Symbol>
            {
                Ref("person"),
                Ref("person.age"),
                Ref("firstFunc"),
                Ref("secondFunc"),
                Ref("thirdFunc"),
                Ref("a"),
                Ref("b"),
                Ref("c"),
            };
            AssertSymbolsMatch(); 
        }
        
        [Fact]
        public void TestLazyReferenceInsideComplexIfCondition()
        {
            Code = @"
                extern func int NPC_HasItems(var instance par0, var int par1);

                class C_NPC { var int data [200]; };

                func int testFunc()
                {
                    var int newWeapon;
                    
                    if (NPC_HasItems(person, sword) >= 1)
                    {
                        return sword;
                    };
                    if ( (oldWeapon == axe) || (oldWeapon == sword) )
                    {
                        newWeapon = 0;
                    };
                    var int oldWeapon;
                };
                instance person(C_NPC);
                instance sword (C_NPC){};
                instance axe (C_NPC){};
            ";
            
            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // if (NPC_HasItems(person, sword) >= 1)
                new PushInt(1),
                new PushInstance(Ref("person")),
                new PushInt(RefIndex("sword")),
                new CallExternal(Ref("NPC_HasItems")),
                new GreaterOrEqual(),
                new JumpIfToLabel("#0001_endif"),
                
                // return sword;
                new PushInt(RefIndex("sword")),
                new Ret(),
                
                // endif
                new AssemblyLabel("#0001_endif"),
                
                // if ( (oldWeapon == axe) || (oldWeapon == sword) )
                new PushInt(RefIndex("sword")),
                new PushVar(Ref("testFunc.oldWeapon")),
                new Equal(),
                new PushInt(RefIndex("axe")),
                new PushVar(Ref("testFunc.oldWeapon")),
                new Equal(),
                new LogOr(),
                new JumpIfToLabel("#0002_endif"),
                
                // newWeapon = 0;
                new PushInt(0),
                new PushVar(Ref("testFunc.newWeapon")),
                new Assign(),
                
                //endif
                new AssemblyLabel("#0002_endif"),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            
            ExpectedSymbols = new List<Symbol>
            {
                Ref("NPC_HasItems"),
                Ref("NPC_HasItems.par0"),
                Ref("NPC_HasItems.par1"),
                
                Ref("C_NPC"),
                Ref("C_NPC.data"),
                Ref("testFunc"),
                Ref("testFunc.newWeapon"),
                Ref("testFunc.oldWeapon"),
                Ref("person"),
                Ref("sword"),
                Ref("axe"),

            };
            AssertSymbolsMatch(); 
        }
        
        
        [Fact]
        public void TestNestedFunctionCallsWithLazyFunctionsAsArguments()
        {
            Code = @"
                extern func void Info_AddChoice(var int par0, var string par1, var func par2);
                extern func int NPC_IsInState(var instance par0, var func par1);

                class C_NPC {
                    var int data [200];
                };

                instance other(C_NPC);


                func string firstFunc(var string text, var int num)
                {
                    return text;
                };
                
                func int secondFunc(var C_NPC oth, var int talent, var int skill)
                {
                    return 0;
                };
                
                func void testFunc()
                {
                    Info_AddChoice(info, firstFunc(""test"", secondFunc(other, 1, 2)), thirdFunc);
                    NPC_IsInState(NULL, NOFUNC);
                };
        
                class C_INFO {
                    var int data[12];
                };
                instance info(C_INFO){};
        
                func void thirdFunc(){};
            ";
            
            char prefix = (char) 255;
            
            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // Info_AddChoice(info, firstFunc(""test"", secondFunc(other, 1, 2)), thirdFunc);
                new PushInt(RefIndex("info")),
                new PushVar(Ref($"{prefix}10000")),
                new PushInstance(Ref("other")),
                new PushInt(1),
                new PushInt(2),
                new Call(Ref("secondFunc")),
                new Call(Ref("firstFunc")),
                new PushInt(RefIndex("thirdFunc")),
                new CallExternal(Ref("Info_AddChoice")),
                
                // NPC_IsInState(NULL, NOFUNC);
                new PushNullInstance(),
                new PushInt(-1), //NOFUNC = -1
                new CallExternal(Ref("NPC_IsInState")), 
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            
            ExpectedSymbols = new List<Symbol>
            {
                Ref("Info_AddChoice"),
                Ref("Info_AddChoice.par0"),
                Ref("Info_AddChoice.par1"),
                Ref("Info_AddChoice.par2"),
                Ref("NPC_IsInState"),
                Ref("NPC_IsInState.par0"),
                Ref("NPC_IsInState.par1"),
                Ref("C_NPC"),
                Ref("C_NPC.data"),
                Ref("other"),
                Ref("firstFunc"),
                Ref("firstFunc.text"),
                Ref("firstFunc.num"),
                Ref("secondFunc"),
                Ref("secondFunc.oth"),
                Ref("secondFunc.talent"),
                Ref("secondFunc.skill"),
                Ref("testFunc"),
                Ref("C_INFO"),
                Ref("C_INFO.data"),
                Ref("info"),
                Ref("thirdFunc"),
                Ref($"{prefix}10000"),

            };
            AssertSymbolsMatch(); 
        }
        
        [Fact]
        public void TestComplexIfCondition()
        {
            Code = @"
               func void testFunc() {
                   if (!(0 == 1 || 2 == 3))
                   && (!(4 == 5 || 6 == 7))
                   && (8 > 9) {
                   
                   };
               };
           ";
   
            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // (8 > 9)
                new PushInt(9),
                new PushInt(8),
                new Greater(),
       
                // !(4 == 5 || 6 == 7)
                new PushInt(7),
                new PushInt(6),
                new Equal(),
                new PushInt(5),
                new PushInt(4),
                new Equal(),
                new LogOr(),
                new Not(),
       
                // !(0 == 1 || 2 == 3)
                new PushInt(3),
                new PushInt(2),
                new Equal(),
                new PushInt(1),
                new PushInt(0),
                new Equal(),
                new LogOr(),
                new Not(),       
                new LogAnd(),
                new LogAnd(),
                new JumpIfToLabel("#0001_endif"),
                // endif
       
                new AssemblyLabel("#0001_endif"),
       
                new Ret()
            };
            AssertInstructionsMatch();
 
            ExpectedSymbols = new List<Symbol>
            {
                Ref("testFunc"),
            };
            AssertSymbolsMatch();
        }

        [Fact]
        public void TestLocalInstanceAttributeAssignment()
        {
            Code = @"
                class Person {
                    var int age; 
                    var int weight;
                    var int height;
                };

                func int secondFunc(var int x) {
                    return x + 2;
                };

                func int testFunc() {
                    var int a;
                    var Person person;
                    person.age = 5;
                    a = person.age;
                    person.age = secondfunc(person.age);
                    return +person.age;
                };
           ";

            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // person.age = 5;
                new PushInt(5),
                new SetInstance(Ref("testFunc.person")),
                new PushVar(Ref("Person.age")),
                new Assign(),

                // a = person.age;
                new SetInstance(Ref("testFunc.person")),
                new PushVar(Ref("Person.age")),
                new PushVar(Ref("testFunc.a")),
                new Assign(),

                // person.age = secondfunc(person.age);
                new SetInstance(Ref("testFunc.person")),
                new PushVar(Ref("Person.age")),
                new Call(Ref("secondFunc")),
                new SetInstance(Ref("testFunc.person")),
                new PushVar(Ref("Person.age")),
                new Assign(),

                // return +person.age;
                new SetInstance(Ref("testFunc.person")),
                new PushVar(Ref("Person.age")),
                new Plus(),
                new Ret(),

                new Ret()
            };
            AssertInstructionsMatch();

            ExpectedSymbols = new List<Symbol>
            {
                Ref("Person"),
                Ref("Person.age"),
                Ref("Person.weight"),
                Ref("Person.height"),

                Ref("secondFunc"),
                Ref("secondFunc.x"),

                Ref("testFunc"),
                Ref("testFunc.a"),
                Ref("testFunc.person")
            };
            AssertSymbolsMatch();
        }

        [Fact]
        public void TestLocalVariableTypeSameAsOtherLocalVariableNameIgnorecase()
        {
            Code = @"
                class Person {
                    var int age; 
                };


                func void testFunc(var int person) {
                    var Person p;
                    p.age = 5;
                };
           ";

            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                new PushVar(Ref("testFunc.person")),
                new Assign(),

                // p.age = 5;
                new PushInt(5),
                new SetInstance(Ref("testFunc.p")),
                new PushVar(Ref("Person.age")),
                new Assign(),

                new Ret()
            };
            AssertInstructionsMatch();

            ExpectedSymbols = new List<Symbol>
            {
                Ref("Person"),
                Ref("Person.age"),

                Ref("testFunc"),
                Ref("testFunc.person"),
                Ref("testFunc.p"),
            };
            AssertSymbolsMatch();
        }

        [Fact]
        public void TestSingleExpressionReturnHack()
        {
            Code = @"
                func void testFunc() {
                var int x;
                    x = 5;
                    x;
                    return;
                };
           ";

            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                // x = 5;
                new PushInt(5),
                new PushVar(Ref("testFunc.x")),
                new Assign(),

                // x;
                new PushVar(Ref("testFunc.x")),

                // return;
                new Ret(),

                new Ret()
            };
            AssertInstructionsMatch();

            ExpectedSymbols = new List<Symbol>
            {
                Ref("testFunc"),
                Ref("testFunc.x"),
            };
            AssertSymbolsMatch();
        }
        
        
        [Fact]
        public void TestInlineInstanceDeclarations()
        {
            Code = @"
                class C_NPC { var int data [200]; };
                instance Mage, Archer, Barbarian(C_NPC);

                prototype Orc(C_NPC) {};
                instance Warrior, Elite, Shaman(Orc);
           ";

            
            Instructions = GetExecBlockInstructions("Mage");
            ExpectedInstructions = new List<AssemblyElement>
            {
                new Ret(),
            };
            AssertInstructionsMatch();
            
            
            Instructions = GetExecBlockInstructions("Archer");
            ExpectedInstructions = new List<AssemblyElement>
            {
                new Ret(),
            };
            AssertInstructionsMatch();
            
            
            Instructions = GetExecBlockInstructions("Barbarian");
            ExpectedInstructions = new List<AssemblyElement>
            {
                new Ret(),
            };
            AssertInstructionsMatch();
            
            
            Instructions = GetExecBlockInstructions("Orc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                new Ret(),
            };
            AssertInstructionsMatch();
            
            
            Instructions = GetExecBlockInstructions("Warrior");
            ExpectedInstructions = new List<AssemblyElement>
            {
                new Call(Ref("Orc")),
            };
            AssertInstructionsMatch();
            
            
            Instructions = GetExecBlockInstructions("Elite");
            ExpectedInstructions = new List<AssemblyElement>
            {
                new Call(Ref("Orc")),
            };
            AssertInstructionsMatch();
            
            
            Instructions = GetExecBlockInstructions("Shaman");
            ExpectedInstructions = new List<AssemblyElement>
            {
                new Call(Ref("Orc")),
                new Ret(),
            };
            AssertInstructionsMatch();

            
            ExpectedSymbols = new List<Symbol>
            {
                Ref("C_NPC"),
                Ref("C_NPC.data"),
                
                Ref("Mage"),
                Ref("Archer"),
                Ref("Barbarian"),
                
                Ref("Orc"),
                
                Ref("Warrior"),
                Ref("Elite"),
                Ref("Shaman")
            };
            AssertSymbolsMatch();
        }
        
        
        [Fact]
        public void TestIntArrayLocalVariableReturn()
        {
            Code = @"
                func int testFunc() {
                    var int a[2048];
                    return a;
                };

                func int secondFunc() {
                    const int b[3] = {1, 2, 3};
                    return b;
                };
           ";

            
            Instructions = GetExecBlockInstructions("testFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                new PushVar(Ref("testFunc.a")),
                new Ret(),
                
                new Ret(),
            };
            AssertInstructionsMatch();
            
            
            Instructions = GetExecBlockInstructions("secondFunc");
            ExpectedInstructions = new List<AssemblyElement>
            {
                new PushVar(Ref("secondFunc.b")),
                new Ret(),
                
                new Ret(),
            };
            AssertInstructionsMatch();


            ExpectedSymbols = new List<Symbol>
            {
                Ref("testFunc"),
                Ref("testFunc.a"),

                Ref("secondFunc"),
                Ref("secondFunc.b")
            };
            AssertSymbolsMatch();
        }
    }
}
