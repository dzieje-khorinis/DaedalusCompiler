using System;
using DaedalusCompiler.Compilation.SemanticAnalysis;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    public static class AssemblyBuilderHelpers
    {
        public static ParamLessInstruction GetAssignInstructionForSymbolType(SymbolType type)
        {
            ParamLessInstruction instruction = new ParamLessInstruction();
            switch (type)
            {
                case (SymbolType.Int):
                {
                    instruction = new Assign();
                    break;
                }
                case (SymbolType.String):
                {
                    instruction = new AssignString(); //TODO when to use AssignStringRef?
                    break;
                }
                case (SymbolType.Func):
                {
                    instruction = new AssignFunc();
                    break;
                }
                case (SymbolType.Float):
                {
                    instruction = new AssignFloat();
                    break;
                }
                case (SymbolType.Instance): // TODO check if it happens
                case (SymbolType.Class):
                {
                    instruction = new AssignInstance();
                    break;
                }
            }

            return instruction;
        }

        public static ParamLessInstruction GetInstructionForOperator(
            string operatorVal,
            bool twoArg = true,
            SymbolType leftSideType = SymbolType.Void)
        {
            ParamLessInstruction instruction = new ParamLessInstruction();

            switch (operatorVal)
            {
                case "=":
                    instruction = GetAssignInstructionForSymbolType(leftSideType);
                    break;
                case "+=":
                    instruction = new AssignAdd();
                    break;
                case "-=":
                    instruction = new AssignSubtract();
                    break;
                case "*=":
                    instruction = new AssignMultiply();
                    break;
                case "/=":
                    instruction = new AssignDivide();
                    break;


                case "+":
                    if (twoArg)
                    {
                        instruction = new Add();
                    }
                    else
                    {
                        instruction = new Plus();
                    }

                    break;

                case "-":
                    if (twoArg)
                    {
                        instruction = new Subtract();
                    }
                    else
                    {
                        instruction = new Minus();
                    }

                    break;


                case "<<":
                    instruction = new ShiftLeft();
                    break;
                case ">>":
                    instruction = new ShiftRight();
                    break;


                case ">":
                    instruction = new Greater();
                    break;
                case ">=":
                    instruction = new GreaterOrEqual();
                    break;
                case "<":
                    instruction = new Less();
                    break;
                case "<=":
                    instruction = new LessOrEqual();
                    break;


                case "==":
                    instruction = new Equal();
                    break;
                case "!=":
                    instruction = new NotEqual();
                    break;


                case "!":
                    instruction = new Not();
                    break;
                case "~":
                    instruction = new Negate();
                    break;


                case "*":
                    instruction = new Multiply();
                    break;
                case "/":
                    instruction = new Divide();
                    break;
                case "%":
                    instruction = new Modulo();
                    break;


                case "&":
                    instruction = new BitAnd();
                    break;
                case "|":
                    instruction = new BitOr();
                    break;
                case "&&":
                    instruction = new LogAnd();
                    break;
                case "||":
                    instruction = new LogOr();
                    break;
            }

            if (instruction == null)
            {
                throw new Exception($"'{operatorVal}' does't have insctruction");
            }

            return instruction;
        }
    }
}