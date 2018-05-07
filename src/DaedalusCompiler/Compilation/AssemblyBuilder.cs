using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    public enum ExecutebleBlockType
    {
        Function,
        InstanceConstructor,
        PrototypeConstructor
    }
    
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

    public class ExecBlock : AssemblyElement
    {
        public List<AssemblyElement> body;
        public DatSymbol symbol;
    }

    public class FunctionBlock : ExecBlock {}

    public class InstanceConstructorBlock : ExecBlock {}

    public class PrototypeContructorBlock : ExecBlock {}

    public class SymbolInstruction : AssemblyInstruction
    {
        public DatSymbol symbol;

        public SymbolInstruction(DatSymbol symbol)
        {
            this.symbol = symbol;
        }
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

    public class PushVar : SymbolInstruction
    {
        public PushVar(DatSymbol symbol) : base(symbol)
        {
            
        }
    }
    
    public class PushArrVar : SymbolInstruction
    {
        public int index;

        public PushArrVar(DatSymbol symbol, int index) : base(symbol)
        {
            this.index = index;
        }
    }

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

    public class Call : SymbolInstruction
    {
        public Call(DatSymbol symbol) : base(symbol) {}
    }

    public class CallExternal : SymbolInstruction
    {
        public CallExternal(DatSymbol symbol) : base(symbol)
        {
            
        }
    }

    public class AssemblyBuildContext
    {
        public AssemblyIfStatement currentConditionStatement;
        public List<AssemblyElement> body;
        public AssemblyBuildContext parent;
    }

    public class AssemblyBuilder
    {
        public List<FunctionBlock> functions;
        public List<PrototypeContructorBlock> prototypeContructors;
        public List<InstanceConstructorBlock> instanceConstructors;
        public List<DatSymbol> symbols;
        private ExecBlock active;
        private AssemblyBuildContext currentBuildCtx; // current assembly build context
        private List<AssemblyElement> assembly;
        private DatSymbol refSymbol; // we use that for prototype and instance definintions
        private SymbolInstruction assigmentLInstruction;

        public AssemblyBuilder()
        {
            functions = new List<FunctionBlock>();
            prototypeContructors = new List<PrototypeContructorBlock>();
            instanceConstructors = new List<InstanceConstructorBlock>();
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
        
        
        public void addInstruction(AssemblyInstruction instruction)
        {
            currentBuildCtx.body.Add(instruction);
        }
        
        public void addInstructions(params AssemblyInstruction[] instructions)
        {
            currentBuildCtx.body.AddRange(instructions);
        }

        public void execBlockStart(DatSymbol symbol, ExecutebleBlockType blockType)
        {
            var newExecBlockInstructionStack = getEmptyBuildContext();
            switch (blockType)
            {
                case ExecutebleBlockType.Function:
                    var function = new FunctionBlock(){ symbol = symbol};
                    
                    functions.Add(function);

                    active = function;
                    break;
                case ExecutebleBlockType.InstanceConstructor:
                    var instanceConstructor = new InstanceConstructorBlock(){ symbol = symbol};
                    
                    instanceConstructors.Add(instanceConstructor);
                    
                    active = instanceConstructor;
                    break;
                case ExecutebleBlockType.PrototypeConstructor:
                    var prototypeConstructor = new PrototypeContructorBlock(){ symbol = symbol};
                    
                    prototypeContructors.Add(prototypeConstructor);

                    active = prototypeConstructor;
                    break;
            }

            currentBuildCtx = newExecBlockInstructionStack;
        }

        public void execBlockEnd()
        {
            active.body = currentBuildCtx.body;
            active = null;

            currentBuildCtx = currentBuildCtx.parent;
        }

        public void assigmentStart(SymbolInstruction instruction)
        {
            assigmentLInstruction = instruction;
        }

        public void assigmentEnd()
        {
            var operationType = assigmentLInstruction.symbol.Type;

            addInstruction(assigmentLInstruction);
            switch (operationType)
            {
                case DatSymbolType.Int:
                    addInstruction(new Assign());
                    break;
                case DatSymbolType.String:
                    addInstruction(new AssignString());
                    break;
                //todo implement rest
            }
        }

        public void expressionBracketStart()
        {
            //todo implement
        }

        public void expressionBracketEnd()
        {
            //todo implement
        }

        public void expressionBlockStart()
        {
            //todo implement
        }

        public void expressionBlockEnd()
        {
            //todo implement
        }

        public void setRefSymbol(DatSymbol refSymbol)
        {
            this.refSymbol = refSymbol;
        }


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
            string targetSymbolName;

            if (active is PrototypeContructorBlock || active is InstanceConstructorBlock)
            {
                targetSymbolName = $"{refSymbol.Name}.{symbolName}";
            }
            else
            {
                targetSymbolName = $"{active.symbol.Name}.{symbolName}";
            }

            var symbolLocalScope = symbols.Find(x => x.Name == targetSymbolName);

            if (symbolLocalScope == null)
            {
                // in that case we look for symbol in global scope
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

        public DatSymbol getSymbolByName(string symbolName)
        {
            return symbols.FirstOrDefault(x => x.Name == symbolName);
        }

        public int getSymbolId(DatSymbol symbol)
        {
            return symbols.IndexOf(symbol);
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