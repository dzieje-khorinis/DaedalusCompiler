using System;
using System.Collections.Generic;
using System.Linq;
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

    public class AssemblyOperatorStatement: AssemblyElement
    {
        private List<AssemblyElement> leftBody;
        private List<AssemblyElement> rightBody;

        public AssemblyOperatorStatement()
        {
            leftBody = new List<AssemblyElement>();
            rightBody = new List<AssemblyElement>();
        }

        public List<AssemblyElement> getLeft()
        {
            return leftBody;
        }

        public List<AssemblyElement> getRight()
        {
            return rightBody;
        }

        public void setLeft(List<AssemblyElement> lInstructions)
        {
            leftBody = lInstructions;
        }

        public void setRight(List<AssemblyElement> rInstructions)
        {
            rightBody = rInstructions;
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

    public class JumpToLabel : AssemblyInstruction
    {
        public string label;

        public JumpToLabel(string label)
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
    
    public class PushInstance : SymbolInstruction
    {
        public PushInstance(DatSymbol symbol) : base(symbol)
        {

        }
    }
    
    public class SetInstance : SymbolInstruction
    {
        public SetInstance(DatSymbol symbol) : base(symbol)
        {
            
        }
    }

    public class Equal : ParamLessInstruction {}
    public class NotEqual : ParamLessInstruction {}
    public class Less : ParamLessInstruction {}
    public class LessOrEqual : ParamLessInstruction {}
    public class Greater : ParamLessInstruction {}
    public class GreaterOrEqual : ParamLessInstruction {}
    public class Assign : ParamLessInstruction {}
    public class AssignAdd : ParamLessInstruction {}
    public class AssignSubtract : ParamLessInstruction {}
    public class AssignMultiply : ParamLessInstruction {}
    public class AssignDivide : ParamLessInstruction {}
    public class AssignString : ParamLessInstruction {}
    public class AssignStringRef : ParamLessInstruction {}
    public class AssignFunc : ParamLessInstruction {}
    public class AssignFloat : ParamLessInstruction {}
    public class AssignInstance : ParamLessInstruction {}
    public class Ret : ParamLessInstruction {}
    public class Add : ParamLessInstruction {}
    public class Multiply : ParamLessInstruction {}
    public class Divide : ParamLessInstruction {}
    public class Subtract : ParamLessInstruction {}
    public class Modulo : ParamLessInstruction {}
    public class Not: ParamLessInstruction {}
    public class Minus: ParamLessInstruction {}
    public class Plus: ParamLessInstruction {}
    public class Negate: ParamLessInstruction {}
    public class ShiftLeft: ParamLessInstruction {}
    public class ShiftRight: ParamLessInstruction {}
    public class BitAnd: ParamLessInstruction {}
    public class BitOr: ParamLessInstruction {}
    public class LogAnd: ParamLessInstruction {}
    public class LogOr: ParamLessInstruction {}

    public class JumpIfToLabel : JumpToLabel
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
        public AssemblyOperatorStatement currentOperatorStatement;
        public AssemblyIfStatement currentConditionStatement;
        public List<AssemblyElement> body;
        public AssemblyBuildContext parent;
        public bool isOperatorContext;
    }

    public class FuncArgsBodyContext
    {
        public List<AssemblyElement> body;
        public FuncArgsBodyContext parent;

        public FuncArgsBodyContext(FuncArgsBodyContext parent)
        {
            body = new List<AssemblyElement>();
            this.parent = parent;
        }
    }

    public class AssemblyBuilder
    {
        public List<ExecBlock> execBlocks;
        public List<DatSymbol> symbols;
        private List<DatSymbol> stringLiteralSymbols;
        private ExecBlock active;
        private AssemblyBuildContext currentBuildCtx; // current assembly build context
        private List<SymbolInstruction> assignmentLeftSide;
        private FuncArgsBodyContext funcArgsBodyCtx;
        private int labelIndexGenerator;
        private int nextStringSymbolNumber;

        public AssemblyBuilder()
        {
            execBlocks = new List<ExecBlock>();
            symbols = new List<DatSymbol>();
            stringLiteralSymbols = new List<DatSymbol>();
            currentBuildCtx = getEmptyBuildContext();
            active = null;
            assignmentLeftSide = new List<SymbolInstruction>();
            funcArgsBodyCtx = new FuncArgsBodyContext(null);
            labelIndexGenerator = 0;
            nextStringSymbolNumber = 10000;
        }

        public string newStringSymbolName()
        {
            return $"{(char)255}{nextStringSymbolNumber++}";
        }

        public List<DatSymbol> getAllSymbols()
        {
            return symbols.Concat(stringLiteralSymbols).ToList();
        }

        public AssemblyBuildContext getEmptyBuildContext(bool isOperatorContext = false)
        {
            return new AssemblyBuildContext()
            {
                body = new List<AssemblyElement>(),
                parent = currentBuildCtx,
                currentConditionStatement = new AssemblyIfStatement(),
                currentOperatorStatement = new AssemblyOperatorStatement(),
                isOperatorContext = isOperatorContext
            };
        }

        public bool isContextInsideExecBlock()
        {
            return active != null;
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
            switch (blockType)
            {
                case ExecutebleBlockType.Function:
                    var function = new FunctionBlock(){ symbol = symbol };
                    active = function;
                    break;
                case ExecutebleBlockType.InstanceConstructor:
                    var instanceConstructor = new InstanceConstructorBlock(){ symbol = symbol };
                    active = instanceConstructor;
                    break;
                case ExecutebleBlockType.PrototypeConstructor:
                    var prototypeConstructor = new PrototypeContructorBlock(){ symbol = symbol };
                    active = prototypeConstructor;
                    break;
            }
            execBlocks.Add(active);
            currentBuildCtx = getEmptyBuildContext();
        }

        public void execBlockEnd()
        {
            active.body = currentBuildCtx.body;
            active = null;

            currentBuildCtx = currentBuildCtx.parent;
        }

        public void assigmentStart(SymbolInstruction[] instructions)
        {
            assignmentLeftSide.AddRange(instructions);
        }

        public void assigmentEnd(string assignmentOperator)
        {
            var operationType = assignmentLeftSide.Last().symbol.Type;   //TODO check if there are any possibilities of assignmentLeftSide longer than 2 instructions?
            var assignmentInstruction = AssemblyBuilderHelpers.GetInstructionForOperator(assignmentOperator, true, operationType);
          
            addInstructions((SymbolInstruction[])assignmentLeftSide.ToArray());
            assignmentLeftSide = new List<SymbolInstruction>();
            addInstruction(assignmentInstruction);
        }

        public void expressionLeftSideStart()
        {
            currentBuildCtx = getEmptyBuildContext(true);
        }

        public void expressionRightSideStart()
        {
            currentBuildCtx.currentOperatorStatement.setLeft(currentBuildCtx.body);
            currentBuildCtx.body = new List<AssemblyElement>();
        }

        public void funcCallArgStart()
        {
            currentBuildCtx = getEmptyBuildContext();
        }

        public void funcCallArgEnd()
        {
            funcArgsBodyCtx.body.AddRange(currentBuildCtx.body);
            currentBuildCtx = currentBuildCtx.parent;
        }

        public void funcCallStart()
        {
            funcArgsBodyCtx = new FuncArgsBodyContext(funcArgsBodyCtx);
        }

        public void funcCallEnd(AssemblyElement instruction)
        {
            currentBuildCtx = currentBuildCtx.parent;
            currentBuildCtx.body.AddRange(funcArgsBodyCtx.body);
            currentBuildCtx.body.Add(instruction);

            funcArgsBodyCtx = funcArgsBodyCtx.parent;
        }

        public void expressionEnd(AssemblyInstruction operatorInstruction)
        {
            //TODO add desc why
            var currentOperatorStatement = currentBuildCtx.currentOperatorStatement;
            var parentBuildContext = currentBuildCtx.parent;
            var currentBody = currentBuildCtx.body;
            var currentLeftBody = currentOperatorStatement.getLeft();
            var currentRightBody = currentOperatorStatement.getRight();
            var newLeftBody = currentLeftBody;
            var newRightBody = currentRightBody;

            if (currentRightBody.Count > 0)
            {
                if (currentLeftBody.Count > 0)
                {
                    if (currentBody.Count > 0)
                    {
                        //TODO make sure if that case happen
                        newRightBody = currentRightBody.Concat(currentBody).ToList();
                    }
                }
                else
                {
                    newLeftBody = currentRightBody;
                    newRightBody = currentBody;
                }
            }
            else
            {
                newRightBody = currentBody;
            }

            var instructions = newRightBody.Concat(newLeftBody).Append(operatorInstruction).ToList();


            if ( !parentBuildContext.isOperatorContext )
            {
                parentBuildContext.body.AddRange( instructions );
            }
            else
            {
                var parentRight = parentBuildContext.currentOperatorStatement.getRight();
                var parentLeft = parentBuildContext.currentOperatorStatement.getLeft();
                var parentRightHasItems = parentRight.Count > 0;
                var parentLeftHasItems = parentLeft.Count > 0;

                if (parentRightHasItems && parentLeftHasItems)
                {
                    parentBuildContext.currentOperatorStatement.setLeft(parentLeft.Concat(instructions).ToList());
                }
                else if (parentRightHasItems)
                {
                    //TODO add desc why
                    parentBuildContext.currentOperatorStatement.setRight(instructions);
                    parentBuildContext.currentOperatorStatement.setLeft( parentRight );
                }
                else
                {
                    //TODO add desc why
                    parentBuildContext.currentOperatorStatement.setRight( instructions );
                }
            }

            currentBuildCtx = parentBuildContext;
        }

        public void expressionBracketStart()
        {
            // to remove, need to refactor testst
            //todo implement
        }

        public void expressionBracketEnd()
        {
            // to remove, need to refactor testst
            //todo implement
        }

        public void expressionBlockStart()
        {
            // to remove, need to refactor testst
            //todo implement
        }

        public void expressionBlockEnd()
        {
            // to remove, need to refactor testst
            //todo implement
        }


        public void conditionalStart()
        {

        }

        public void conditionalEnd()
        {
            currentBuildCtx.body.AddRange(resolveIfStatement(currentBuildCtx.currentConditionStatement));

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
            if (symbol.Name.StartsWith($"{(char) 255}") && symbol.Type == DatSymbolType.String &&
                symbol.Flags == DatSymbolFlag.Const)
            {
                stringLiteralSymbols.Add(symbol);
            }
            else
            {
                symbols.Add(symbol);
            }
            
        }

        public void addSymbols(List<DatSymbol> symbols)
        {
            symbols.AddRange(symbols);
        }

        public DatSymbol resolveSymbol(string symbolName)
        {
            string targetSymbolName;
            DatSymbol symbolLocalScope = null;
            DatSymbol symbol = null;
            
            if (active != null && !symbolName.Contains("."))
            {
                DatSymbol currentExecBlockSymbol = active.symbol;
                
                while (currentExecBlockSymbol != null)
                {
                    targetSymbolName = $"{currentExecBlockSymbol.Name}.{symbolName}";
                    
                    symbol = getAllSymbols().Find(x => x.Name.ToUpper() == targetSymbolName.ToUpper());
                    
                    if (symbol == null)
                    {
                        if (currentExecBlockSymbol.Parent == -1)
                        {
                            currentExecBlockSymbol = null;
                            break;
                        }
                        currentExecBlockSymbol = symbols[currentExecBlockSymbol.Parent];
                    }
                    else
                    {
                        return symbol;
                    }
                }
            }

            symbol = getAllSymbols().Find(x => x.Name.ToUpper() == symbolName.ToUpper());

            if (symbol == null)
            {
                throw new Exception("Symbol " + symbolName + " is not added");
            }

            return symbol;

        }

        public DatSymbol getSymbolByName(string symbolName)
        {
            return getAllSymbols().FirstOrDefault(x => x.Name.ToUpper() == symbolName.ToUpper());
        }

        public int getSymbolId(DatSymbol symbol)
        {
            return getAllSymbols().IndexOf(symbol);
        }

        public string getNextLabel()
        {
            var labelVal = labelIndexGenerator;

            labelIndexGenerator++;
            
            return $"label_{labelVal}";
        }

        public List<AssemblyElement> resolveIfStatement(AssemblyIfStatement ifStatement)
        {
            var instructions = new List<AssemblyElement>();
            var ifBlocks = new List<IfBlock>();
            var haveElse = ifStatement.elseBody.Count > 0;
            var statementEndLabel = getNextLabel();
            var elseStartLabel = "";
            
            ifBlocks.Add(ifStatement.ifBlock);
            ifBlocks.AddRange(ifStatement.elseIfBlock);

            foreach (var ifBlock in ifBlocks)
            {
                var isLastOne = ifBlock == ifBlocks.Last();

                instructions.AddRange(ifBlock.condition);

                if (!isLastOne)
                {
                    var nextJumpLabel = getNextLabel();

                    instructions.Add(new JumpIfToLabel(nextJumpLabel));
                    instructions.AddRange(ifBlock.body);
                    instructions.Add(new JumpToLabel(statementEndLabel));
                    instructions.Add(new AssemblyLabel(nextJumpLabel));
                }
                else
                {
                    if (haveElse)
                    {
                        elseStartLabel = getNextLabel();
                        instructions.Add(new JumpIfToLabel(elseStartLabel));
                        instructions.AddRange(ifBlock.body);
                        instructions.Add(new JumpToLabel(statementEndLabel));
                    }
                    else
                    {
                        instructions.Add(new JumpIfToLabel(statementEndLabel));
                        instructions.AddRange(ifBlock.body);
                    }
                }
            }

            if (haveElse)
            {
                instructions.Add(new AssemblyLabel(elseStartLabel));
                instructions.AddRange(ifStatement.elseBody);
            }
            
            instructions.Add(new AssemblyLabel(statementEndLabel));

            return instructions;
        }

        public string getAssembler()
        {
            return new AssemblyBuilderTraverser().getAssembler(execBlocks, getAllSymbols());
        }

        public void saveToDat()
        {
            // todo finish
            var datFile = AssemblyBuilderToDat.getDatFile(execBlocks, getAllSymbols());
            
            datFile.Save("./test.dat");
        }
    }
}