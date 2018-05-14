using System;

namespace DaedalusCompiler.Compilation
{
    public class AssemblyBuilderHelpers
    {
        public static ParamLessInstruction GetInstructionForOperator(string operatorVal, bool twoArg = false)
        {
            ParamLessInstruction instruction = new ParamLessInstruction();

            switch (operatorVal)
            {
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

            }

            if (instruction == null)
            {
                throw new Exception($"'{operatorVal}' does't have insctruction");
            }

            return instruction;
        }
    }
}