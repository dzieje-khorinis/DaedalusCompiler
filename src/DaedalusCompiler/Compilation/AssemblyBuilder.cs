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

    public class AssemblyElement
    {
    }

    public class AssemblyInstruction : AssemblyElement
    {
    }

    public class AssemblyLabel : AssemblyElement
    {
        public readonly string Label;

        public AssemblyLabel(string label)
        {
            Label = label;
        }
    }

    public class IfBlock
    {
        public List<AssemblyElement> Body;
        public List<AssemblyElement> Condition;

        public IfBlock()
        {
            Body = new List<AssemblyElement>();
            Condition = new List<AssemblyElement>();
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
        public IfBlock IfBlock;
        public List<AssemblyElement> ElseBody;
        public readonly List<IfBlock> ElseIfBlock;
        public IfBlockType CurrentBlockType;
        public List<AssemblyElement> ConditionInstructionStack;


        public AssemblyIfStatement()
        {
            IfBlock = new IfBlock();
            ElseBody = new List<AssemblyElement>();
            ElseIfBlock = new List<IfBlock>();
        }
    }

    public class AssemblyOperatorStatement : AssemblyElement
    {
        private List<AssemblyElement> _leftBody;
        private List<AssemblyElement> _rightBody;

        public AssemblyOperatorStatement()
        {
            _leftBody = new List<AssemblyElement>();
            _rightBody = new List<AssemblyElement>();
        }

        public List<AssemblyElement> GetLeft()
        {
            return _leftBody;
        }

        public List<AssemblyElement> GetRight()
        {
            return _rightBody;
        }

        public void SetLeft(List<AssemblyElement> lInstructions)
        {
            _leftBody = lInstructions;
        }

        public void SetRight(List<AssemblyElement> rInstructions)
        {
            _rightBody = rInstructions;
        }
    }

    public class ExecBlock : AssemblyElement
    {
        public List<AssemblyElement> Body;
        public DatSymbol Symbol;
    }

    public class FunctionBlock : ExecBlock
    {
    }

    public class InstanceConstructorBlock : ExecBlock
    {
    }

    public class PrototypeContructorBlock : ExecBlock
    {
    }

    public class SymbolInstruction : AssemblyInstruction
    {
        public readonly DatSymbol Symbol;

        protected SymbolInstruction(DatSymbol symbol)
        {
            Symbol = symbol;
        }
    }

    public class ValueInstruction : AssemblyInstruction
    {
        public readonly object Value;

        protected ValueInstruction(object value)
        {
            Value = value;
        }
    }

    public abstract class AddressInstruction : AssemblyInstruction
    {
        public readonly int Address;

        protected AddressInstruction(int address)
        {
            Address = address;
        }
    }

    public class JumpToLabel : AssemblyInstruction
    {
        public readonly string Label;

        public JumpToLabel(string label)
        {
            Label = label;
        }
    }

    public class ParamLessInstruction : AssemblyInstruction
    {
    }

    public class PushInt : ValueInstruction
    {
        public PushInt(int value) : base(value)
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
        public readonly int Index;

        public PushArrVar(DatSymbol symbol, int index) : base(symbol)
        {
            Index = index;
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
    // public class AssignStringRef : ParamLessInstruction {}
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
        public Call(DatSymbol symbol) : base(symbol)
        {
        }
    }

    public abstract class CallExternal : SymbolInstruction
    {
        protected CallExternal(DatSymbol symbol) : base(symbol)
        {
        }
    }

    public class AssemblyBuildContext
    {
        public AssemblyOperatorStatement CurrentOperatorStatement;
        public AssemblyIfStatement CurrentConditionStatement;
        public List<AssemblyElement> Body;
        public AssemblyBuildContext Parent;
        public bool IsOperatorContext;
    }

    public class FuncArgsBodyContext
    {
        public readonly List<AssemblyElement> Body;
        public readonly FuncArgsBodyContext Parent;

        public FuncArgsBodyContext(FuncArgsBodyContext parent)
        {
            Body = new List<AssemblyElement>();
            Parent = parent;
        }
    }

    public class AssemblyBuilder
    {
        public readonly List<ExecBlock> ExecBlocks;
        public readonly List<DatSymbol> Symbols;
        private readonly List<DatSymbol> _stringLiteralSymbols;
        private ExecBlock _active;
        private AssemblyBuildContext _currentBuildCtx;
        private List<SymbolInstruction> _assignmentLeftSide;
        private FuncArgsBodyContext _funcArgsBodyCtx;
        private int _labelIndexGenerator;
        private int _nextStringSymbolNumber;
        private bool _isInsideConstDefContext;
        private bool _ignoreAddedInstructions;

        public AssemblyBuilder()
        {
            ExecBlocks = new List<ExecBlock>();
            Symbols = new List<DatSymbol>();
            _stringLiteralSymbols = new List<DatSymbol>();
            _currentBuildCtx = GetEmptyBuildContext();
            _active = null;
            _assignmentLeftSide = new List<SymbolInstruction>();
            _funcArgsBodyCtx = new FuncArgsBodyContext(null);
            _labelIndexGenerator = 0;
            _nextStringSymbolNumber = 10000;
            _isInsideConstDefContext = false;
            _ignoreAddedInstructions = false;
        }

        public bool IgnoreAddedInstructions
        {
            get => _ignoreAddedInstructions;
            set => _ignoreAddedInstructions = value;
        }

        public string NewStringSymbolName()
        {
            return $"{(char) 255}{_nextStringSymbolNumber++}";
        }

        public List<DatSymbol> GetAllSymbols()
        {
            return Symbols.Concat(_stringLiteralSymbols).ToList();
        }

        private AssemblyBuildContext GetEmptyBuildContext(bool isOperatorContext = false)
        {
            return new AssemblyBuildContext
            {
                Body = new List<AssemblyElement>(),
                Parent = _currentBuildCtx,
                CurrentConditionStatement = new AssemblyIfStatement(),
                CurrentOperatorStatement = new AssemblyOperatorStatement(),
                IsOperatorContext = isOperatorContext
            };
        }

        public bool IsContextInsideExecBlock()
        {
            return _active != null;
        }

        public void AddInstruction(AssemblyInstruction instruction)
        {
            if (!_ignoreAddedInstructions && !_isInsideConstDefContext)
            {
                _currentBuildCtx.Body.Add(instruction);   
            }
        }

        public void AddInstructions(IEnumerable<AssemblyElement> instructions)
        {
            if (!_ignoreAddedInstructions && !_isInsideConstDefContext)
            {
                _currentBuildCtx.Body.AddRange(instructions);
            }
        }

        public void ExecBlockStart(DatSymbol symbol, ExecutebleBlockType blockType)
        {
            switch (blockType)
            {
                case ExecutebleBlockType.Function:
                    var function = new FunctionBlock {Symbol = symbol};
                    _active = function;
                    break;
                case ExecutebleBlockType.InstanceConstructor:
                    var instanceConstructor = new InstanceConstructorBlock {Symbol = symbol};
                    _active = instanceConstructor;
                    break;
                case ExecutebleBlockType.PrototypeConstructor:
                    var prototypeConstructor = new PrototypeContructorBlock {Symbol = symbol};
                    _active = prototypeConstructor;
                    break;
            }

            ExecBlocks.Add(_active);
            _currentBuildCtx = GetEmptyBuildContext();
        }

        public void ExecBlockEnd()
        {
            _active.Body = _currentBuildCtx.Body;
            _active = null;

            _currentBuildCtx = _currentBuildCtx.Parent;
        }

        public void AssigmentStart(SymbolInstruction[] instructions)
        {
            _assignmentLeftSide.AddRange(instructions);
        }

        public void AssigmentEnd(string assignmentOperator)
        {
            var operationType =
                _assignmentLeftSide.Last().Symbol
                    .Type; //TODO check if there are any possibilities of assignmentLeftSide longer than 2 instructions?
            var assignmentInstruction =
                AssemblyBuilderHelpers.GetInstructionForOperator(assignmentOperator, true, operationType);

            AddInstructions(_assignmentLeftSide.ToArray());
            _assignmentLeftSide = new List<SymbolInstruction>();
            AddInstruction(assignmentInstruction);
        }

        public void ExpressionLeftSideStart()
        {
            _currentBuildCtx = GetEmptyBuildContext(true);
        }

        public void ExpressionRightSideStart()
        {
            _currentBuildCtx.CurrentOperatorStatement.SetLeft(_currentBuildCtx.Body);
            _currentBuildCtx.Body = new List<AssemblyElement>();
        }

        public void FuncCallArgStart()
        {
            _currentBuildCtx = GetEmptyBuildContext();
        }

        public void FuncCallArgEnd()
        {
            _funcArgsBodyCtx.Body.AddRange(_currentBuildCtx.Body);
            _currentBuildCtx = _currentBuildCtx.Parent;
        }

        public void FuncCallStart()
        {
            _funcArgsBodyCtx = new FuncArgsBodyContext(_funcArgsBodyCtx);
        }

        public void FuncCallEnd(AssemblyElement instruction)
        {
            _currentBuildCtx = _currentBuildCtx.Parent;
            _currentBuildCtx.Body.AddRange(_funcArgsBodyCtx.Body);
            _currentBuildCtx.Body.Add(instruction);

            _funcArgsBodyCtx = _funcArgsBodyCtx.Parent;
        }

        public void ExpressionEnd(AssemblyInstruction operatorInstruction)
        {
            //TODO add desc why
            var currentOperatorStatement = _currentBuildCtx.CurrentOperatorStatement;
            var parentBuildContext = _currentBuildCtx.Parent;
            var currentBody = _currentBuildCtx.Body;
            var currentLeftBody = currentOperatorStatement.GetLeft();
            var currentRightBody = currentOperatorStatement.GetRight();
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


            if (!parentBuildContext.IsOperatorContext)
            {
                parentBuildContext.Body.AddRange(instructions);
            }
            else
            {
                var parentRight = parentBuildContext.CurrentOperatorStatement.GetRight();
                var parentLeft = parentBuildContext.CurrentOperatorStatement.GetLeft();
                var parentRightHasItems = parentRight.Count > 0;
                var parentLeftHasItems = parentLeft.Count > 0;

                if (parentRightHasItems && parentLeftHasItems)
                {
                    parentBuildContext.CurrentOperatorStatement.SetLeft(parentLeft.Concat(instructions).ToList());
                }
                else if (parentRightHasItems)
                {
                    //TODO add desc why
                    parentBuildContext.CurrentOperatorStatement.SetRight(instructions);
                    parentBuildContext.CurrentOperatorStatement.SetLeft(parentRight);
                }
                else
                {
                    //TODO add desc why
                    parentBuildContext.CurrentOperatorStatement.SetRight(instructions);
                }
            }

            _currentBuildCtx = parentBuildContext;
        }


        public void ConditionalStart()
        {
        }

        public void ConditionalEnd()
        {
            _currentBuildCtx.Body.AddRange(ResolveIfStatement(_currentBuildCtx.CurrentConditionStatement));

            _currentBuildCtx.CurrentConditionStatement = new AssemblyIfStatement();
        }

        public void ConditionalBlockConditionStart(IfBlockType blockType)
        {
            _currentBuildCtx.CurrentConditionStatement.CurrentBlockType = blockType;

            _currentBuildCtx = GetEmptyBuildContext();
        }

        public void ConditionalBlockConditionEnd()
        {
            var body = _currentBuildCtx.Body;
            // we need firstly get out from condition context
            _currentBuildCtx = _currentBuildCtx.Parent;
            _currentBuildCtx.CurrentConditionStatement.ConditionInstructionStack = body;

            // we need create context for statement block
            _currentBuildCtx = GetEmptyBuildContext();
        }

        public void ConditionalBlockBodyEnd()
        {
            var body = _currentBuildCtx.Body;
            _currentBuildCtx = _currentBuildCtx.Parent;

            var blocktype = _currentBuildCtx.CurrentConditionStatement.CurrentBlockType;

            if (blocktype == IfBlockType.If || blocktype == IfBlockType.ElseIf)
            {
                var ifBlock = new IfBlock
                {
                    Body = body,
                    Condition = _currentBuildCtx.CurrentConditionStatement.ConditionInstructionStack
                };

                if (blocktype == IfBlockType.If)
                {
                    _currentBuildCtx.CurrentConditionStatement.IfBlock = ifBlock;
                }
                else
                {
                    _currentBuildCtx.CurrentConditionStatement.ElseIfBlock.Add(ifBlock);
                }
            }
            else
            {
                _currentBuildCtx.CurrentConditionStatement.ElseBody = body;
            }

            //currentAssemblyBuildContext
        }

        public void AddSymbol(DatSymbol symbol)
        {
            if (symbol.Name.StartsWith($"{(char) 255}") && symbol.Type == DatSymbolType.String &&
                symbol.Flags == DatSymbolFlag.Const)
            {
                _stringLiteralSymbols.Add(symbol);
            }
            else
            {
                Symbols.Add(symbol);
            }
        }

        public DatSymbol ResolveSymbol(string symbolName)
        {
            DatSymbol symbol;

            if (_active != null && !symbolName.Contains("."))
            {
                DatSymbol currentExecBlockSymbol = _active.Symbol;

                while (currentExecBlockSymbol != null)
                {
                    var targetSymbolName = $"{currentExecBlockSymbol.Name}.{symbolName}";

                    symbol = GetAllSymbols().Find(x => x.Name.ToUpper() == targetSymbolName.ToUpper());

                    if (symbol == null)
                    {
                        if (currentExecBlockSymbol.Parent == -1)
                        {
                            break;
                        }

                        currentExecBlockSymbol = Symbols[currentExecBlockSymbol.Parent];
                    }
                    else
                    {
                        return symbol;
                    }
                }
            }

            symbol = GetAllSymbols().Find(x => x.Name.ToUpper() == symbolName.ToUpper());

            if (symbol == null)
            {
                throw new Exception("Symbol " + symbolName + " is not added");
            }

            return symbol;
        }

        public DatSymbol GetSymbolByName(string symbolName)
        {
            return GetAllSymbols().FirstOrDefault(x => x.Name.ToUpper() == symbolName.ToUpper());
        }

        public int GetSymbolId(DatSymbol symbol)
        {
            return GetAllSymbols().IndexOf(symbol);
        }

        private string GetNextLabel()
        {
            var labelVal = _labelIndexGenerator;

            _labelIndexGenerator++;

            return $"label_{labelVal}";
        }

        private List<AssemblyElement> ResolveIfStatement(AssemblyIfStatement ifStatement)
        {
            var instructions = new List<AssemblyElement>();
            var ifBlocks = new List<IfBlock>();
            var haveElse = ifStatement.ElseBody.Count > 0;
            var statementEndLabel = GetNextLabel();
            var elseStartLabel = "";

            ifBlocks.Add(ifStatement.IfBlock);
            ifBlocks.AddRange(ifStatement.ElseIfBlock);

            foreach (var ifBlock in ifBlocks)
            {
                var isLastOne = ifBlock == ifBlocks.Last();

                instructions.AddRange(ifBlock.Condition);

                if (!isLastOne)
                {
                    var nextJumpLabel = GetNextLabel();

                    instructions.Add(new JumpIfToLabel(nextJumpLabel));
                    instructions.AddRange(ifBlock.Body);
                    instructions.Add(new JumpToLabel(statementEndLabel));
                    instructions.Add(new AssemblyLabel(nextJumpLabel));
                }
                else
                {
                    if (haveElse)
                    {
                        elseStartLabel = GetNextLabel();
                        instructions.Add(new JumpIfToLabel(elseStartLabel));
                        instructions.AddRange(ifBlock.Body);
                        instructions.Add(new JumpToLabel(statementEndLabel));
                    }
                    else
                    {
                        instructions.Add(new JumpIfToLabel(statementEndLabel));
                        instructions.AddRange(ifBlock.Body);
                    }
                }
            }

            if (haveElse)
            {
                instructions.Add(new AssemblyLabel(elseStartLabel));
                instructions.AddRange(ifStatement.ElseBody);
            }

            instructions.Add(new AssemblyLabel(statementEndLabel));

            return instructions;
        }

        public string GetAssembler()
        {
            return new AssemblyBuilderTraverser().GetAssembler(ExecBlocks);
        }

        public void SaveToDat()
        {
            // todo finish
            var datFile = AssemblyBuilderToDat.GetDatFile(ExecBlocks, GetAllSymbols());

            datFile.Save("./test.dat");
        }
    }
}