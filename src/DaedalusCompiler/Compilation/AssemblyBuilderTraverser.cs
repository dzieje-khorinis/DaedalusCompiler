using System;
using System.Collections.Generic;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    public class AssemblyBuilderTraverser
    {
        private string buildAcc;
        private int labelIdx;
        
        public AssemblyBuilderTraverser()
        {
            buildAcc = "";
            labelIdx = 0;
        }

        public string getLabel()
        {
            return $"label_{labelIdx++}";
        }
        
        public string traverse(bool getAssembly, List<AssemblyFunction> functions, List<DatSymbol> symbols)
        {
            foreach (var symbol in symbols)
            {
                //TODO
            }
            
            foreach (var function in functions)
            {
                buildAcc += $"# func \"{function.symbol.Name}\" start \n";

                //function.body
                foreach (var element in function.body)
                {
                    visitElement(element);
                }

                buildAcc += $"# func \"{function.symbol.Name}\" end \n";
            }

            return buildAcc;
        }

        public void visitElement(AssemblyElement element)
        {
            if (element is ParamLessInstruction)
            {
                var typeName = element.GetType().Name;

                buildAcc += $"{typeName}\n";
            }
            else if (element is AssemblyIfStatement ifElement)
            {
                var ifBlock = ifElement.ifBlock;
                var elseIfBlocks = ifElement.elseIfBlock;
                var elseInstructions = ifElement.elseBody;
                var ifStatementEndLabel = getLabel();
                var nextJumpLabel = ifStatementEndLabel;
                
                //TODO make sure that this code works good, not tested :(

                foreach (var item in ifBlock.condition)
                {
                    visitElement(item);
                }

                if (elseIfBlocks.Count > 0 || elseInstructions.Count > 0)
                {
                    nextJumpLabel = getLabel();
                }

                visitElement(new JumpIfToLabel(nextJumpLabel));

                foreach (var item in ifBlock.body)
                {
                    visitElement(item);
                }

                visitElement(new JumpIfToLabel(ifStatementEndLabel));

                if (elseIfBlocks.Count > 0)
                {
                    foreach (var block in elseIfBlocks)
                    {
                        visitElement(new AssemblyLabel(nextJumpLabel));
                        
                        nextJumpLabel = getLabel();
                        
                        foreach (var item in block.condition)
                        {
                            visitElement(item);
                        }

                        foreach (var item in block.body)
                        {
                            visitElement(item);
                        }
                        
                        visitElement(new JumpIfToLabel(ifStatementEndLabel));
                    }   
                }

                if (elseInstructions.Count > 0)
                {
                    visitElement(new AssemblyLabel(nextJumpLabel));
                    
                    foreach (var item in elseInstructions)
                    {
                        visitElement(item);
                    }
                }
                else
                {
                    ifStatementEndLabel = nextJumpLabel;
                }
                
                visitElement(new AssemblyLabel(ifStatementEndLabel));
            }
            else if (element is AssemblyLabel label)
            {
                buildAcc += $"{label.label}:\n";
            }
            else if (element is LabelJumpInstruction labelJump)
            {
                buildAcc += $"JumpIfToLabel {labelJump.label}\n";
            }
            else if (element is PushInt pushint)
            {
                buildAcc += $"PushInt {pushint.value}\n";
            }
        }
    }
}