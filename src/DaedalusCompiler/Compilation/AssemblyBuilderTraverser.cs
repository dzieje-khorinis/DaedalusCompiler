using System.Collections.Generic;

namespace DaedalusCompiler.Compilation
{
    public class AssemblyBuilderTraverser
    {
        private string _buildAcc;
        private int _labelIdx;

        public AssemblyBuilderTraverser()
        {
            _buildAcc = "";
            _labelIdx = 0;
        }

        private string GetLabel()
        {
            return $"label_{_labelIdx++}";
        }

        public string GetAssembler(List<BaseExecBlockContext> execBlocks)
        {
            foreach (var function in execBlocks)
            {
                _buildAcc += $"# func \"{function.GetSymbol().Name}\" start \n";

                //function.body
                foreach (var element in function.Body)
                {
                    VisitElement(element);
                }

                _buildAcc += $"# func \"{function.GetSymbol().Name}\" end \n";
            }

            return _buildAcc;
        }

        private void VisitElement(AssemblyElement element)
        {
            if (element is ParamLessInstruction)
            {
                var typeName = element.GetType().Name;

                _buildAcc += $"{typeName}\n";
            }
            else if (element is Call callElement)
            {
                _buildAcc += $"call {callElement.Symbol.Name}\n";
            }
            else if (element is PushVar pushVarElement)
            {
                _buildAcc += $"PushVar {pushVarElement.Symbol.Name}\n";
            }
            else if (element is PushArrayVar pushArrVarElement)
            {
                _buildAcc += $"PushArrVar {pushArrVarElement.Symbol.Name}[{pushArrVarElement.Index}]\n";
            }
            else if (element is SetInstance setInstanceElement)
            {
                _buildAcc += $"SetInstance {setInstanceElement.Symbol.Name}\n";
            }
            else if (element is IfBlockStatementContext context)
            {
                var ifBlock = context.IfBlock;
                var elseIfBlocks = context.ElseIfBlocks;
                var elseInstructions = context.ElseBlock.Body;
                var ifStatementEndLabel = GetLabel();
                var nextJumpLabel = ifStatementEndLabel;

                //TODO make sure that this code works good, not tested :(

                foreach (var item in ifBlock.Condition)
                {
                    VisitElement(item);
                }

                if (elseIfBlocks.Count > 0 || elseInstructions.Count > 0)
                {
                    nextJumpLabel = GetLabel();
                }

                VisitElement(new JumpIfToLabel(nextJumpLabel));

                foreach (var item in ifBlock.Body)
                {
                    VisitElement(item);
                }

                VisitElement(new JumpIfToLabel(ifStatementEndLabel));

                if (elseIfBlocks.Count > 0)
                {
                    foreach (var block in elseIfBlocks)
                    {
                        VisitElement(new AssemblyLabel(nextJumpLabel));

                        nextJumpLabel = GetLabel();

                        foreach (var item in block.Condition)
                        {
                            VisitElement(item);
                        }

                        foreach (var item in block.Body)
                        {
                            VisitElement(item);
                        }

                        VisitElement(new JumpIfToLabel(ifStatementEndLabel));
                    }
                }

                if (elseInstructions.Count > 0)
                {
                    VisitElement(new AssemblyLabel(nextJumpLabel));

                    foreach (var item in elseInstructions)
                    {
                        VisitElement(item);
                    }
                }
                else
                {
                    ifStatementEndLabel = nextJumpLabel;
                }

                VisitElement(new AssemblyLabel(ifStatementEndLabel));
            }
            else if (element is AssemblyLabel label)
            {
                _buildAcc += $"{label.Label}:\n";
            }
            else if (element is JumpIfToLabel labelIfJump)
            {
                _buildAcc += $"JumpIfToLabel {labelIfJump.Label}\n";
            }
            else if (element is JumpToLabel labelJump)
            {
                _buildAcc += $"JumpToLabel {labelJump.Label}\n";
            }
            else if (element is PushInt pushint)
            {
                _buildAcc += $"PushInt {pushint.Value}\n";
            }
        }
    }
}