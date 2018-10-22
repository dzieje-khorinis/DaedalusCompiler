using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

    public class PushArrayVar : SymbolInstruction
    {
        public readonly int Index;

        public PushArrayVar(DatSymbol symbol, int index) : base(symbol)
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

    public class CallExternal : SymbolInstruction
    {
        public CallExternal(DatSymbol symbol) : base(symbol)
        {
        }
    }

    public class LazyComplexReferenceNodeInstructions : AssemblyInstruction
    {
        private readonly DaedalusParser.ComplexReferenceNodeContext[] _complexReferenceNodes;
        private readonly AssemblyBuilder _assemblyBuilder;
        private readonly DaedalusParserListener _parserListener;
        
        private readonly ExecBlock _activeExecBlock;
        
        private readonly bool _isInsideArgList;
        private readonly bool _isInsideAssignment;
        private readonly bool _isInsideIfCondition;
        private readonly bool _isInsideReturnStatement;
        private readonly DatSymbolType _assignmentType;
        private readonly FuncCallContext _funcCallCtx;
        
        public LazyComplexReferenceNodeInstructions(
            AssemblyBuilder assemblyBuilder,
            DaedalusParser.ComplexReferenceNodeContext[] complexReferenceNodes)
        {
            _complexReferenceNodes = complexReferenceNodes;
            _assemblyBuilder = assemblyBuilder;
            
            _activeExecBlock = assemblyBuilder.ActiveExecBlock;
           
            _isInsideArgList = assemblyBuilder.IsInsideArgList;
            _isInsideAssignment = assemblyBuilder.IsInsideAssignment;
            _isInsideIfCondition = assemblyBuilder.IsInsideIfCondition;
            _isInsideReturnStatement = assemblyBuilder.IsInsideReturnStatement;
            _assignmentType = assemblyBuilder.AssignmentType;
            _funcCallCtx = FuncCallContext.Clone(assemblyBuilder.FuncCallCtx);
        }
        
        public List<AssemblyInstruction> Evaluate()
        {
            _assemblyBuilder.ActiveExecBlock = _activeExecBlock;
            
            _assemblyBuilder.IsInsideArgList = _isInsideArgList;
            _assemblyBuilder.IsInsideAssignment = _isInsideAssignment;
            _assemblyBuilder.IsInsideIfCondition = _isInsideIfCondition;
            _assemblyBuilder.IsInsideReturnStatement = _isInsideReturnStatement;
            _assemblyBuilder.AssignmentType = _assignmentType;
            _assemblyBuilder.FuncCallCtx = _funcCallCtx;
            
            return _assemblyBuilder.GetComplexReferenceNodeInstructions(_complexReferenceNodes);
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

    public class FuncCallContext
    {
        public List<DatSymbolType> ParametersTypes;
        public int ArgIndex;
        public FuncCallContext Parent;

        public FuncCallContext(FuncCallContext parent=null)
        {
            ParametersTypes = new List<DatSymbolType>();
            ArgIndex = -1;
            Parent = parent;
        }

        public static FuncCallContext Clone(FuncCallContext ctx)
        {
            if (ctx == null)
            {
                return new FuncCallContext
                {
                    ParametersTypes = new List<DatSymbolType>(),
                    ArgIndex = -1,
                    Parent = null
                };
            }
            else
            {
                return new FuncCallContext
                {
                    ParametersTypes = ctx.ParametersTypes,
                    ArgIndex = ctx.ArgIndex,
                    Parent = ctx.Parent
                };
            }
                
        }
    }

    public class AssemblyBuilder
    {
        public readonly List<ExecBlock> ExecBlocks;
        public List<DatSymbol> Symbols;
        public Dictionary<string, DatSymbol> SymbolsDict;
        private readonly List<DatSymbol> _stringLiteralSymbols;
        public ExecBlock ActiveExecBlock;
        private AssemblyBuildContext _currentBuildCtx;
        private List<SymbolInstruction> _assignmentLeftSide;
        private FuncArgsBodyContext _funcArgsBodyCtx;
        public FuncCallContext FuncCallCtx;
        private int _labelIndexGenerator;
        private int _nextStringSymbolNumber;
        public bool IsInsideEvalableStatement;
        public bool IsCurrentlyParsingExternals;

        public bool IsInsideArgList;
        public bool IsInsideAssignment;
        public bool IsInsideIfCondition;
        public bool IsInsideReturnStatement;
        public DatSymbolType AssignmentType;
        private int _nextSymbolIndex;
        
        public AssemblyBuilder()
        {
            ExecBlocks = new List<ExecBlock>();
            Symbols = new List<DatSymbol>();
            SymbolsDict = new Dictionary<string, DatSymbol>();
            _stringLiteralSymbols = new List<DatSymbol>();
            _currentBuildCtx = GetEmptyBuildContext();
            ActiveExecBlock = null;
            _assignmentLeftSide = new List<SymbolInstruction>();
            _funcArgsBodyCtx = new FuncArgsBodyContext(null);
            FuncCallCtx = null;
            _labelIndexGenerator = 0;
            _nextStringSymbolNumber = 10000;
            IsInsideEvalableStatement = false;
            IsCurrentlyParsingExternals = false;
            
            IsInsideArgList = false;
            IsInsideAssignment = false;
            IsInsideReturnStatement = false;
            AssignmentType = DatSymbolType.Void;
            _nextSymbolIndex = 0;
        }
        
        public DatSymbolType GetParameterType()
        {
            return FuncCallCtx.ParametersTypes[FuncCallCtx.ArgIndex];
        }

        public string NewStringSymbolName()
        {
            return $"{(char) 255}{_nextStringSymbolNumber++}";
        }

        public DatSymbol GetCurrentSymbol()
        {
            return ActiveExecBlock.Symbol;
        }
        
        public bool IsArgListKeyword(string symbolName)
        {
            return symbolName == "nofunc" || symbolName == "null";
        }

        public List<AssemblyInstruction> GetKeywordInstructions(string symbolName)
        {
            if (symbolName == "nofunc")
            {
                return new List<AssemblyInstruction> { new PushInt(-1) };
            }

            if (symbolName == "null")
            {
                DatSymbol symbol = ResolveSymbol($"{(char)255}instance_help");
                return new List<AssemblyInstruction> { new PushInstance(symbol) };
            }
            
            return new List<AssemblyInstruction>();
        }
        
        private DatSymbol GetComplexReferenceNodeSymbol(DaedalusParser.ComplexReferenceNodeContext context)
        {
            string symbolNameLower = context.referenceNode().GetText().ToLower();
            ExecBlock activeBlock = ActiveExecBlock;

            if (
                activeBlock != null
                && (activeBlock.Symbol.Type == DatSymbolType.Instance || activeBlock.Symbol.Type == DatSymbolType.Prototype)
                && (symbolNameLower == "slf" || symbolNameLower == "self")
            )
            {
                return activeBlock.Symbol;
            }
            
            return ResolveSymbol(context.referenceNode().GetText());
        }
        
        public int GetArrayIndex(DaedalusParser.ComplexReferenceNodeContext context)
        {
            var simpleValueContext = context.simpleValue();
            
            
            int arrIndex = 0;
            if (simpleValueContext != null)
            {
                if (!int.TryParse(simpleValueContext.GetText(), out arrIndex))
                {
                    var constSymbol = ResolveSymbol(simpleValueContext.GetText());
                    if (!constSymbol.Flags.HasFlag(DatSymbolFlag.Const) || constSymbol.Type != DatSymbolType.Int)
                    {
                        throw new Exception($"Expected integer constant: {simpleValueContext.GetText()}");
                    }

                    arrIndex = (int) constSymbol.Content[0];
                }
            }

            return arrIndex;
        }
        
        public AssemblyInstruction PushSymbol(DatSymbol symbol, DatSymbolType? asType=null)
        {
            if (asType == DatSymbolType.Func || (asType == DatSymbolType.Int && symbol.Type != DatSymbolType.Int))
            {
                return new PushInt(symbol.Index);
            }

            if (symbol.Type == DatSymbolType.Instance || asType == DatSymbolType.Instance)  /* DatSymbolType.Class isn't possible */
            {
                return new PushInstance(symbol);
            }
            return new PushVar(symbol);
        }
        
        public AssemblyInstruction GetProperPushInstruction(DatSymbol symbol, int arrIndex)
        {
            ExecBlock activeBlock = ActiveExecBlock;
            
            if (arrIndex > 0)
            {
                return new PushArrayVar(symbol, arrIndex);
            }
            
            if (IsInsideArgList)
            {
                return PushSymbol(symbol, GetParameterType());
            }
            
            if (IsInsideReturnStatement && activeBlock != null)
            {
                return PushSymbol(symbol, activeBlock.Symbol.ReturnType);
            }
            
            if (IsInsideAssignment)
            {
                return PushSymbol(symbol, AssignmentType);
            }
            
            if (IsInsideIfCondition)
            {
                return PushSymbol(symbol, DatSymbolType.Int);
            }

            return PushSymbol(symbol);
        }

        public bool IsDottedReference(DaedalusParser.ComplexReferenceNodeContext[] nodes)
        {
            if (nodes.Length > 2)
            {
                throw new Exception("Too many nodes in reference.");
            }
            return nodes.Length == 2;
        }
        
        public List<AssemblyInstruction> GetComplexReferenceNodeInstructions(
            DaedalusParser.ComplexReferenceNodeContext[] complexReferenceNodes)
        {
            var symbolPart = complexReferenceNodes[0];
            string symbolName = symbolPart.referenceNode().GetText().ToLower();


            if (IsInsideArgList && IsArgListKeyword(symbolName))
            {
                return GetKeywordInstructions(symbolName);
            }

            DatSymbol symbol = GetComplexReferenceNodeSymbol(complexReferenceNodes[0]);
            List<AssemblyInstruction> instructions = new List<AssemblyInstruction>();
            
            
            if (IsDottedReference(complexReferenceNodes))
            {
                var attributePart = complexReferenceNodes[1];
                string attributeName = attributePart.referenceNode().GetText();
                DatSymbol attribute = ResolveAttribute(symbol, attributeName);
                
                bool isInsideExecBlock = ActiveExecBlock != null;
                bool isSymbolSelf = symbol == ActiveExecBlock?.Symbol; // self.attribute, slf.attribute cases
                bool isSymbolPassedToInstanceParameter = IsInsideArgList && GetParameterType() == DatSymbolType.Instance;
                bool isSymbolPassedToFuncParameter = IsInsideArgList && GetParameterType() == DatSymbolType.Func;
                bool isInsideFuncAssignment = IsInsideAssignment && AssignmentType == DatSymbolType.Func;
                
                if (isInsideExecBlock
                    && !isSymbolSelf
                    && !isInsideFuncAssignment
                    && !(isSymbolPassedToInstanceParameter || isSymbolPassedToFuncParameter)
                )
                {
                    instructions.Add(new SetInstance(symbol));
                }
                
                int arrIndex = GetArrayIndex(attributePart);
                instructions.Add(GetProperPushInstruction(attribute, arrIndex));
                return instructions;
            }
            else
            {
                int arrIndex = GetArrayIndex(symbolPart);
                instructions.Add(GetProperPushInstruction(symbol, arrIndex));
                return instructions;
            }

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
            return ActiveExecBlock != null;
        }

        public void AddInstruction(AssemblyInstruction instruction)
        {
            _currentBuildCtx.Body.Add(instruction);
        }

        public void AddInstructions(IEnumerable<AssemblyElement> instructions)
        {
            _currentBuildCtx.Body.AddRange(instructions);
        }

        public void ExecBlockStart(DatSymbol symbol, ExecutebleBlockType blockType)
        {
            switch (blockType)
            {
                case ExecutebleBlockType.Function:
                    var function = new FunctionBlock {Symbol = symbol};
                    ActiveExecBlock = function;
                    break;
                case ExecutebleBlockType.InstanceConstructor:
                    var instanceConstructor = new InstanceConstructorBlock {Symbol = symbol};
                    ActiveExecBlock = instanceConstructor;
                    break;
                case ExecutebleBlockType.PrototypeConstructor:
                    var prototypeConstructor = new PrototypeContructorBlock {Symbol = symbol};
                    ActiveExecBlock = prototypeConstructor;
                    break;
            }

            ExecBlocks.Add(ActiveExecBlock);
            _currentBuildCtx = GetEmptyBuildContext();
        }

        public void ExecBlockEnd()
        {
            ActiveExecBlock.Body = _currentBuildCtx.Body;
            ActiveExecBlock = null;

            _currentBuildCtx = _currentBuildCtx.Parent;
        }

        public void AssigmentStart(SymbolInstruction[] instructions)
        {
            _assignmentLeftSide.AddRange(instructions);
        }

        public void AssigmentEnd(string assignmentOperator)
        {
            //TODO check if there are any possibilities of assignmentLeftSide longer than 2 instructions?
            var operationType = _assignmentLeftSide.Last().Symbol.Type; 
            var assignmentInstruction =
                AssemblyBuilderHelpers.GetInstructionForOperator(assignmentOperator, true, operationType);

            if (!IsInsideEvalableStatement)
            {
                AddInstructions(_assignmentLeftSide.ToArray());   
            }
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
            FuncCallCtx.ArgIndex++;
        }

        public void FuncCallArgEnd()
        {
            _funcArgsBodyCtx.Body.AddRange(_currentBuildCtx.Body);
            _currentBuildCtx = _currentBuildCtx.Parent;
        }

        public void FuncCallStart(DaedalusParser.FuncCallValueContext context)
        {
            _funcArgsBodyCtx = new FuncArgsBodyContext(_funcArgsBodyCtx);
            FuncCallCtx = new FuncCallContext(FuncCallCtx);
            
            /*
            if (IsInsideArgList)
            {
                ArgIndexStack.Push(ArgIndex);
                ParametersTypesStack.Push(ParametersTypes);
            }

            ArgIndex = -1;
            ParametersTypes = new List<DatSymbolType>();
            */
            
            IsInsideArgList = true;
            
            
            string funcName = context.funcCall().nameNode().GetText();
            DatSymbol symbol = GetSymbolByName(funcName);

            for (int i = 1; i <= symbol.ParametersCount; ++i)
            {
                DatSymbol parameter = Symbols[symbol.Index + i];
                FuncCallCtx.ParametersTypes.Add(parameter.Type);
            }
        }

        public void FuncCallEnd(AssemblyElement instruction)
        {
            _currentBuildCtx = _currentBuildCtx.Parent;
            _currentBuildCtx.Body.AddRange(_funcArgsBodyCtx.Body);
            _currentBuildCtx.Body.Add(instruction);

            _funcArgsBodyCtx = _funcArgsBodyCtx.Parent;
            FuncCallCtx = FuncCallCtx.Parent;
            if (FuncCallCtx == null)
            {
                IsInsideArgList = false;
            }
            
            /*
            if (ArgIndexStack.Count > 0)
            {
                ArgIndex = ArgIndexStack.Pop();
                ParametersTypes = ParametersTypesStack.Pop();
            }
            else
            {
                IsInsideArgList = false;
            }
            */
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
            if (IsCurrentlyParsingExternals)
            {
                if (symbol.Type == DatSymbolType.Func)
                {
                    symbol.Flags |= DatSymbolFlag.External;
                }

                if (symbol.Name == "instance_help")
                {
                    symbol.Name = $"{(char) 255}instance_help";
                }
            }
            
            
            SymbolsDict[symbol.Name.ToUpper()] = symbol;
            if (symbol.Name.StartsWith($"{(char) 255}") && symbol.Type == DatSymbolType.String &&
                symbol.Flags == DatSymbolFlag.Const)
            {
                _stringLiteralSymbols.Add(symbol);
            }
            else
            {
                Symbols.Add(symbol);
                symbol.Index = _nextSymbolIndex;
                _nextSymbolIndex++;
            }
        }

        public DatSymbol ResolveAttribute(DatSymbol symbol, string attributeName)
        {
            string attributePath = $"{symbol.Name}.{attributeName}";

            DatSymbol attributeSymbol = null;
            
            while (symbol != null)
            {
                attributeSymbol = SymbolsDict.GetValueOrDefault(attributePath.ToUpper(), null);
                
                if (attributeSymbol == null)
                {
                    if (symbol.ParentIndex == DatSymbol.NULL_INDEX)
                    {
                        break;
                    }

                    symbol = Symbols[symbol.ParentIndex];
                    attributePath = $"{symbol.Name}.{attributeName}";
                    
                    if (symbol.Type == DatSymbolType.Prototype && symbol.ParentIndex != DatSymbol.NULL_INDEX)
                    {
                        symbol = Symbols[symbol.ParentIndex];
                        attributePath = $"{symbol.Name}.{attributeName}";
                    }
                    
                }
                else
                {
                    break;
                }
            }
            
            if (attributeSymbol == null)
            {
                throw new Exception($"attributeSymbol {symbol.Name}.{attributeName} is not added");
            }

            return attributeSymbol;
            
        }
        
        public DatSymbol ResolveSymbol(string symbolName)
        {
            DatSymbol symbol;

            if (ActiveExecBlock != null && !symbolName.Contains("."))
            {
                DatSymbol currentExecBlockSymbol = ActiveExecBlock.Symbol;

                while (currentExecBlockSymbol != null)
                {
                    var targetSymbolName = $"{currentExecBlockSymbol.Name}.{symbolName}";
                    symbol = SymbolsDict.GetValueOrDefault(targetSymbolName.ToUpper(), null);
                    
                    
                    if (symbol == null)
                    {
                        if (currentExecBlockSymbol.ParentIndex == DatSymbol.NULL_INDEX)
                        {
                            break;
                        }

                        currentExecBlockSymbol = Symbols[currentExecBlockSymbol.ParentIndex];
                    }
                    else
                    {
                        return symbol;
                    }
                }
            }

            symbol = SymbolsDict.GetValueOrDefault(symbolName.ToUpper(), null);
            
            if (symbol == null)
            {
                throw new Exception("Symbol " + symbolName + " is not added");
            }

            return symbol;
        }

        public DatSymbol GetSymbolByName(string symbolName)
        {
            return SymbolsDict.GetValueOrDefault(symbolName.ToUpper(), null);
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
            DatBuilder datBuilder = new DatBuilder(this);
            DatFile datFile = datBuilder.GetDatFile();
            datFile.Save("./test.dat");
        }
        
        public void Finish()
        {
            foreach (DatSymbol symbol in _stringLiteralSymbols)
            {
                symbol.Index = _nextSymbolIndex;
                _nextSymbolIndex++;
            }
            
            Symbols = Symbols.Concat(_stringLiteralSymbols).ToList();
            
            int counter = 0;
            int maxCounter = ExecBlocks.Count;
            foreach (ExecBlock execBlock in ExecBlocks)
            {
                Console.WriteLine($"{++counter}/{maxCounter} lazy references resolved");
                for (int i = 0; i < execBlock.Body.Count; ++i)
                {
                    AssemblyElement element = execBlock.Body[i];
                    if (element is LazyComplexReferenceNodeInstructions)
                    {
                        List<AssemblyInstruction> instructions = ((LazyComplexReferenceNodeInstructions) element).Evaluate();
                        execBlock.Body.RemoveAt(i);
                        execBlock.Body.InsertRange(i, instructions);
                    }
                }
            }
        }
    }
}