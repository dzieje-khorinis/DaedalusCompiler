using System.Collections.Generic;
using System.Collections.Immutable;
using DaedalusCompiler.Compilation;
using DaedalusCompiler.Dat;
using Xunit;

namespace DaedalusCompiler.Tests
{
    public class AssemblyBuilderTest
    {        
        [Fact]
        public void CorrectlyCreateFunction()
        {
            var builder = new AssemblyBuilder();
            var symbol = SymbolBuilder.BuildFunc("test", DatSymbolType.Int);
            
            builder.execBlockStart(symbol, ExecutebleBlockType.Function);
            builder.execBlockEnd();
            Assert.Equal( 1, builder.functions.Count );
        }
        
        [Fact]
        public void CorrectlyPassBasicExpression()
        {
            var builder = new AssemblyBuilder();
            var testFunc = new Helpers.TestFunction(builder);
            var xSym = SymbolBuilder.BuildVariable("x", DatSymbolType.Int);
            var push9 = new PushInt(9);
            var push12 = new PushInt(12);
            var push20 = new PushInt(20);
            var mult = new Multiply();
            var add = new Add();
            var correctOrder = new AssemblyInstruction[]  { push20, push9, mult, push12, add };

            testFunc.start();

            // x = 12 + 9 * 20
            builder.assigmentStart(new PushVar(xSym));
            builder.addInstructions(push12, add, push9, mult, push20);
            builder.assigmentEnd();

            testFunc.end();

            for (var l = 0; l < correctOrder.Length; l++)
            {
                Assert.Equal(correctOrder[l], builder.functions[0].body[l]);
            }
        }

        [Fact]
        public void CorrectlyPassComplexExpression()
        {
            var builder = new AssemblyBuilder();
            var testFunc = new Helpers.TestFunction(builder);
            var xSym = SymbolBuilder.BuildVariable("x", DatSymbolType.Int);
            var otherFunc = SymbolBuilder.BuildFunc("otherFunc", DatSymbolType.Func);
            var push12 = new PushInt(12);
            var addFirst = new Add();
            var push9 = new PushInt(9);
            var multFirst = new Multiply();
            var push2 = new PushInt(2);
            var addSecond = new Add();
            var push1 = new PushInt(1);
            var addThird = new Add();
            var push7 = new PushInt(7);
            var multSecond = new Multiply();
            var push3 = new PushInt(3);
            var push4 = new PushInt(4);
            var addFourth = new Add();
            var push5 = new PushInt(5);
            var callFunc = new Call(otherFunc);
            var correctOrder = new AssemblyInstruction[]  { push3, push7, multSecond, push1, addThird, push5, push4, addFourth, callFunc, push2, addSecond, push9, multFirst, push12, addFirst };

            testFunc.start();
           
            // x = 12 + 9 * ( 2 + otherFunc(1 + 7 * 3, 4 + 5) )
            builder.assigmentStart(new PushVar(xSym));

            builder.expressionBlockStart();

            builder.addInstructions(push12, addFirst, push9, multFirst);
            builder.expressionBracketStart();
            builder.addInstructions(push2, addSecond);
            // func otherFunc first arg
            builder.expressionBlockStart();
            builder.addInstructions(push1, addThird, push7, multSecond, push3);
            builder.expressionBlockEnd();
            // func otherFunc second arg
            builder.expressionBlockStart();
            builder.addInstructions(push4, addFourth, push5);
            builder.expressionBlockEnd();
            
            builder.addInstruction(callFunc);
            
            builder.expressionBlockEnd();
            testFunc.end();

            for (var l = 0; l < correctOrder.Length; l++)
            {
                Assert.Equal(correctOrder[l], builder.functions[0].body[l]);
            }
        }
    }
}