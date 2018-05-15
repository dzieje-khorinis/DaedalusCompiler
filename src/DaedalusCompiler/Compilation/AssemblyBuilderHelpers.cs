using System;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    public class AssemblyBuilderHelpers
    {
        public static ParamLessInstruction GetInstructionForOperator(
            string operatorVal,
            bool twoArg = true,
            DatSymbolType rightHandSideType = DatSymbolType.Void)
        {
            ParamLessInstruction instruction = new ParamLessInstruction();

            switch (operatorVal)
            {
                case "=":
                    switch (rightHandSideType)
                    {
                        case (DatSymbolType.Int):
                        {
                            instruction = new Assign();
                            break;
                        }
                        case (DatSymbolType.String):
                        {
                            instruction = new AssignString(); //TODO when to use AssignStringRef?
                            break;
                        }
                        case (DatSymbolType.Func):
                        {
                            instruction = new AssignFunc();
                            break;
                        }
                        case (DatSymbolType.Float):
                        {
                            instruction = new AssignFloat();
                            break;
                        }
                        case (DatSymbolType.Instance):
                        {
                            instruction = new AssignInstance();
                            break;
                        }
                    }

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
                        ;
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