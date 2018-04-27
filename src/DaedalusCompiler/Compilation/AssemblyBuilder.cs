using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    public class AssemblyElement {}
    
    public class AssemblyInstruction: AssemblyElement
    {
    }

    public class AssemblyLabel: AssemblyElement
    {
        public string label;

        public AssemblyLabel(string label)
        {
            this.label = label;
        }
    }

    public class IfBlock
    {
        public List<AssemblyElement> body;
        public List<AssemblyElement> condition;

        public IfBlock()
        {
            body = new List<AssemblyElement>();
            condition = new List<AssemblyElement>();
        }
    }
    
    public enum IfBlockType
    {
        If,
        ElseIf,
        Else
    }

    public class AssemblyIfStatement : AssemblyElement
    {
        
        public IfBlock ifBlock;
        public List<AssemblyElement> elseBody;
        public List<IfBlock> elseIfBlock;
        public IfBlockType currentBlockType;
        public List<AssemblyElement> conditionInstructionStack;


        public AssemblyIfStatement()
        {
            ifBlock = new IfBlock();
            elseBody = new List<AssemblyElement>();
            elseIfBlock = new List<IfBlock>();
        }
    }

    public class AssemblyFunction : AssemblyElement
    {
        public List<AssemblyElement> body;
        public DatSymbol symbol;
    }

    public class SymbolInstruction : AssemblyInstruction
    {
        public DatSymbol symbol;
    }

    public class ValueInstruction : AssemblyInstruction
    {
        public object value;

        public ValueInstruction(object value)
        {
            this.value = value;
        }
    }

    public class AddressInstruction : AssemblyInstruction
    {
        public int address;
    }

    public class LabelJumpInstruction : AssemblyInstruction
    {
        public string label;

        public LabelJumpInstruction(string label)
        {
            this.label = label;
        }
    }

    public class ParamLessInstruction : AssemblyInstruction{}

    public class PushInt : ValueInstruction
    {
        public PushInt(int value): base(value)
        {
            
        }
    }
    
    public class PushVar : SymbolInstruction {}

    public class Less : ParamLessInstruction {}
    public class Greater : ParamLessInstruction {}
    public class Assign : ParamLessInstruction {}
    public class AssignString : ParamLessInstruction {}
    public class Ret : ParamLessInstruction {}
    public class Add : ParamLessInstruction {}
    public class Multiply : ParamLessInstruction {}
    public class Divide : ParamLessInstruction {}
    public class Subract : ParamLessInstruction {}

    public class JumpIfToLabel : LabelJumpInstruction
    {
        public JumpIfToLabel(string name) : base(name)
        {
        }
    }
    //public class Call : LabelJumpInstruction {}
    
    public class CallExternal: SymbolInstruction {}

    public class AssemblyBuildContext
    {
        public AssemblyIfStatement currentConditionStatement;
        public List<AssemblyElement> body;
        public AssemblyBuildContext parent;
    }

    public class AssemblyBuilder
    {
        private List<AssemblyFunction> functions;
        private List<DatSymbol> symbols;
        private AssemblyFunction active;
        private AssemblyBuildContext currentBuildCtx; // current assembly build context
        private List<AssemblyElement> assembly;

        public AssemblyBuilder()
        {
            functions = new List<AssemblyFunction>();
            symbols = new List<DatSymbol>();
            currentBuildCtx = getEmptyBuildContext();
            active = null;
            assembly = new List<AssemblyElement>();
        }

        public AssemblyBuildContext getEmptyBuildContext()
        {
            return new AssemblyBuildContext()
            {
                body = new List<AssemblyElement>(),
                parent = currentBuildCtx,
                currentConditionStatement = new AssemblyIfStatement()
            };
        }
        
        public void setActiveFunction(DatSymbol symbol)
        {
            active = functions.Find(x => x.symbol == symbol);
        }
        
        public void addInstruction(AssemblyInstruction instruction)
        {
            currentBuildCtx.body.Add(instruction);
        }

        public void functionStart(DatSymbol symbol)
        {
            var newFunctionInstructionStack = getEmptyBuildContext();

            functions.Add(new AssemblyFunction() { symbol = symbol});
            setActiveFunction(symbol);

            currentBuildCtx = newFunctionInstructionStack;
        }

        public void functionEnd()
        {
            active.body = currentBuildCtx.body;
            active = null;

            currentBuildCtx = currentBuildCtx.parent;
        }

//        public void addFunctionBody(string name, List<AssemblyInstruction> body)
//        {
//            var funcToUpdate = functions.Find(x => x.name == name);
//
//            if (funcToUpdate == null)
//            {
//                throw new Exception("Function with name " + name + " is not added to assembly builder");
//            }
//
//            funcToUpdate.body = body;
//        }

        public void conditionalStart()
        {
            
        }

        public void conditionalEnd()
        {
            currentBuildCtx.body.Add(currentBuildCtx.currentConditionStatement);

            currentBuildCtx.currentConditionStatement = new AssemblyIfStatement();
        }
        
        public void conditionalBlockConditionStart(IfBlockType blockType)
        {
            currentBuildCtx.currentConditionStatement.currentBlockType = blockType;

            currentBuildCtx = getEmptyBuildContext();
        }

        public void conditionalBlockConditionEnd()
        {
            var body = currentBuildCtx.body;
            // we need firstly get out from condition context
            currentBuildCtx = currentBuildCtx.parent;
            currentBuildCtx.currentConditionStatement.conditionInstructionStack = body;

            // we need create context for statement block
            currentBuildCtx = getEmptyBuildContext();
        }

        public void conditionalBlockBodyEnd()
        {
            var body = currentBuildCtx.body;
            currentBuildCtx = currentBuildCtx.parent;

            var blocktype = currentBuildCtx.currentConditionStatement.currentBlockType;

            if (blocktype == IfBlockType.If || blocktype == IfBlockType.ElseIf)
            {
                var ifBlock = new IfBlock()
                {
                    body = body,
                    condition = currentBuildCtx.currentConditionStatement.conditionInstructionStack
                };

                if (blocktype == IfBlockType.If)
                {
                    currentBuildCtx.currentConditionStatement.ifBlock =ifBlock;
                }
                else
                {
                    currentBuildCtx.currentConditionStatement.elseIfBlock.Add(ifBlock);
                }
            }
            else
            {
                currentBuildCtx.currentConditionStatement.elseBody = body;
            }
            
            //currentAssemblyBuildContext
        }

        public void addSymbol(DatSymbol symbol)
        {
            symbols.Add(symbol);
        }
        
        public void addSymbols(List<DatSymbol> symbols)
        {
            symbols.AddRange(symbols);
        }

        public DatSymbol resolveSymbol(string symbolName)
        {
            var funcName = active.symbol.Name;
            var symbolLocalScope = symbols.Find(x => x.Name == funcName + "." + symbolName);

            if (symbolLocalScope == null)
            {
                var symbol = symbols.Find(x => x.Name == symbolName);

                if (symbol == null)
                {
                    throw new Exception("Symbol " + symbolName + " is not added");
                }

                return symbol;
            }
            else
            {
                return symbolLocalScope;
            }
        }

        public string getAssembler()
        {
            return new AssemblyBuilderTraverser().traverse(true, functions, symbols);
        }

        public string getByteCode()
        {
            return "";
        }

        public string getOutput(bool getAssembler = false)
        {
            if (getAssembler)
            {
                return this.getAssembler();
            }
            else
            {
                return getByteCode();
            }
        }
    }
}