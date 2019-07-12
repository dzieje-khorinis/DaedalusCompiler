using System;
using System.Collections.Generic;
using System.Linq;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    
    
    public abstract class AssemblyBuilderContext : AssemblyElement
    {
        
        public readonly AssemblyBuilderContext Parent;

        protected AssemblyBuilderContext(AssemblyBuilderContext parent)
        {
            Parent = parent;
        }

        public virtual void SetEndInstruction(AssemblyElement element)
        {
            throw new NotImplementedException();
        }
        
        public abstract List<AssemblyElement> GetInstructions();

        public virtual void AddInstruction(AssemblyElement element)
        {
            throw new NotImplementedException();
        }
        
        public virtual void AddInstructions(List<AssemblyElement> elements)
        {
            throw new NotImplementedException();
        }

        public virtual void FetchInstructions(AssemblyBuilderContext context)
        {
            AddInstructions(context.GetInstructions());
        }
    }


    public enum ExecBlockType
    {
        Function,
        Instance,
        Prototype,
        Shared,
    }

    public abstract class BaseExecBlockContext : BlockContext
    {
        protected BaseExecBlockContext(AssemblyBuilderContext parent) : base(parent)
        {
        }

        public abstract DatSymbol GetSymbol();
    }
    
    public class ExecBlockContext : BaseExecBlockContext
    {
        public DatSymbol Symbol;
        
        public ExecBlockContext(DatSymbol symbol) : base(parent:null)
        {
            Symbol = symbol;
        }

        public override DatSymbol GetSymbol()
        {
            return Symbol;
        }
    }

    public class FunctionBlockContext : ExecBlockContext
    {
        public FunctionBlockContext(DatSymbol symbol) : base(symbol)
        {
        }
    }
    
    public class PrototypeBlockContext : ExecBlockContext
    {
        public PrototypeBlockContext(DatSymbol symbol) : base(symbol)
        {
        }
    }
    
    public class InstanceBlockContext : ExecBlockContext
    {
        public InstanceBlockContext(DatSymbol symbol) : base(symbol)
        {
        }
    }

    public class SharedExecBlockContext : BaseExecBlockContext
    {
        public List<DatSymbol> Symbols;
        
        public SharedExecBlockContext(List<DatSymbol> symbols) : base(parent:null)
        {
            Symbols = symbols;
        }

        public override DatSymbol GetSymbol()
        {
            return Symbols.First();
        }
    }
    
    

    
    
    
    public class BlockContext : AssemblyBuilderContext
    {
        public readonly List<AssemblyElement> Body;

        public BlockContext(AssemblyBuilderContext parent) : base(parent)
        {
            Body = new List<AssemblyElement>();
        }
        
        public override List<AssemblyElement> GetInstructions()
        {
            return Body;
        }
        
        public override void AddInstruction(AssemblyElement element)
        {
            Body.Add(element);
        }
        
        public override void AddInstructions(List<AssemblyElement> elements)
        {
            Body.AddRange(elements);
        }
    }

    public class WhileStatementContext : ConditionalBlockContext
    {

        private readonly LabelManager _labelManager;
        
        public WhileStatementContext(AssemblyBuilderContext parent, LabelManager labelManager) : base(parent)
        {
            _labelManager = labelManager;
        }
        
        private string GetNextLabel()
        {
            return _labelManager.GetWhileLabel();
        }
        
        public override List<AssemblyElement> GetInstructions()
        {
            string startLabel = GetNextLabel();
            string endLabel = GetNextLabel();
                
            List<AssemblyElement> instructions = new List<AssemblyElement>();
            
            instructions.Add(new AssemblyLabel(startLabel));
            
            instructions.AddRange(Condition);
            instructions.Add(new JumpIfToLabel(endLabel));


            foreach (AssemblyElement instruction in Body)
            {
                if (instruction is JumpToLoopStart)
                {
                    instructions.Add(new JumpToLabel(startLabel));
                }
                else if (instruction is JumpToLoopEnd)
                {
                    instructions.Add(new JumpToLabel(endLabel));
                }
                else
                {
                    instructions.Add(instruction);
                }
            }
            //instructions.AddRange(Body);
            
            
            instructions.Add(new JumpToLabel(startLabel));
            
            instructions.Add(new AssemblyLabel(endLabel));

            return instructions;
        }
    }
    
    public class IfBlockStatementContext : AssemblyBuilderContext
    {
        public IfBlockContext IfBlock;
        public readonly List<ElseIfBlockContext> ElseIfBlocks;
        public ElseBlockContext ElseBlock;

        private readonly LabelManager _labelManager;
        

        public IfBlockStatementContext(AssemblyBuilderContext parent, LabelManager labelManager) : base(parent)
        {
            IfBlock = null;
            ElseIfBlocks = new List<ElseIfBlockContext>();
            ElseBlock = null;
            _labelManager = labelManager;
        }

        public override void FetchInstructions(AssemblyBuilderContext context)
        {
            switch (context)
            {
                case IfBlockContext ifBlockContext:
                    IfBlock = ifBlockContext;
                    break;
                case ElseIfBlockContext elseIfBlockContext:
                    ElseIfBlocks.Add(elseIfBlockContext);
                    break;
                case ElseBlockContext elseBlockContext:
                    ElseBlock = elseBlockContext;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private string GetNextLabel()
        {
            return _labelManager.GetIfLabel();
        }
        
        public override List<AssemblyElement> GetInstructions()
        {
            string statementEndLabel = GetNextLabel();
            string elseStartLabel = "";
            
            List<AssemblyElement> instructions = new List<AssemblyElement>();
            
            
            List<ConditionalBlockContext> conditionalBlocks = new List<ConditionalBlockContext>();
            
            conditionalBlocks.Add(IfBlock);
            conditionalBlocks.AddRange(ElseIfBlocks);
            
            foreach (var conditionalBlock in conditionalBlocks)
            {
                instructions.AddRange(conditionalBlock.Condition);
                
                bool isLastIteration = (conditionalBlock == conditionalBlocks.Last());
                if (isLastIteration)
                {
                    if (ElseBlock != null)
                    {
                        elseStartLabel = GetNextLabel();
                        instructions.Add(new JumpIfToLabel(elseStartLabel));
                        instructions.AddRange(conditionalBlock.Body);
                        instructions.Add(new JumpToLabel(statementEndLabel));
                    }
                    else
                    {
                        instructions.Add(new JumpIfToLabel(statementEndLabel));
                        instructions.AddRange(conditionalBlock.Body);
                    }
                }
                else
                {
                    string nextJumpLabel = GetNextLabel();
                    instructions.Add(new JumpIfToLabel(nextJumpLabel));
                    instructions.AddRange(conditionalBlock.Body);
                    instructions.Add(new JumpToLabel(statementEndLabel));
                    instructions.Add(new AssemblyLabel(nextJumpLabel));
                }
            }

            if (ElseBlock != null)
            {
                instructions.Add(new AssemblyLabel(elseStartLabel));
                instructions.AddRange(ElseBlock.Body);
            }

            instructions.Add(new AssemblyLabel(statementEndLabel));

            return instructions;
        }
    }
    
    
    public class ConditionalBlockContext : BlockContext
    {
        public readonly List<AssemblyElement> Condition;
        
        protected ConditionalBlockContext(AssemblyBuilderContext parent) : base(parent)
        {
            Condition = new List<AssemblyElement>();
        }
        
        public override void AddInstruction(AssemblyElement element)
        {
            AddInstructions(new List<AssemblyElement>{element});
        }
        
        public override void AddInstructions(List<AssemblyElement> elements)
        {
            if (Condition.Count == 0)
            {
                Condition.AddRange(elements);
            }
            else
            {
                Body.AddRange(elements);
            }
            
        }
    }

    public class IfBlockContext : ConditionalBlockContext
    {
        public IfBlockContext(AssemblyBuilderContext parent) : base(parent)
        {
        }
    }
    
    public class ElseIfBlockContext : ConditionalBlockContext
    {
        public ElseIfBlockContext(AssemblyBuilderContext parent) : base(parent)
        {
        }
    }
    
    public class ElseBlockContext : BlockContext
    {
        public ElseBlockContext(AssemblyBuilderContext parent) : base(parent)
        {

        }
    }

    
    
    public class StackBasedContext : AssemblyBuilderContext
    {
        private readonly Stack<List<AssemblyElement>> _instructionsStack;


        protected StackBasedContext(AssemblyBuilderContext parent) : base(parent)
        {
            _instructionsStack = new Stack<List<AssemblyElement>>();
        }
        
        public override List<AssemblyElement> GetInstructions()
        {
            List<AssemblyElement> totalInstructions = new List<AssemblyElement>();

            foreach (var instructions in _instructionsStack)
            {
                totalInstructions.AddRange(instructions);
            }
            
            return totalInstructions;
        }
        
        public override void AddInstruction(AssemblyElement element)
        {
            AddInstructions(new List<AssemblyElement>() {element});
        }

        public override void AddInstructions(List<AssemblyElement> elements)
        {
            _instructionsStack.Push(elements);
        }
    }


    public class ArgumentContext : StackBasedContext
    {
        protected ArgumentContext(AssemblyBuilderContext parent) : base(parent)
        {
        }
    }

    public class ExpressionContext : StackBasedContext
    {
        
        private AssemblyElement _operatorInstruction;
        
        
        public ExpressionContext(AssemblyBuilderContext parent) : base(parent)
        {
            _operatorInstruction = null;
        }
        
        public override void SetEndInstruction(AssemblyElement element)
        {
            _operatorInstruction = element;
        }
        
        public override List<AssemblyElement> GetInstructions()
        {
            List<AssemblyElement> instructions = base.GetInstructions();  // TODO check which one this calls
            if (_operatorInstruction != null)
            {
                instructions.Add(_operatorInstruction);
            }
            return instructions;
        }
    }
    
    
    public class FuncCallContext : AssemblyBuilderContext
    {
        private readonly List<AssemblyElement> _instructions;
        private AssemblyElement _callInstruction;

        private readonly DatSymbol _symbol;
        private readonly List<DatSymbolType> _parametersTypes;
        public int ArgIndex;
        public readonly FuncCallContext OuterCall;
        
        public FuncCallContext(AssemblyBuilderContext parent, FuncCallContext outerCall, List<DatSymbolType> parametersTypes, DatSymbol symbol) : base(parent)
        {
            _symbol = symbol;
            _parametersTypes = parametersTypes;
            ArgIndex = -1;
            OuterCall = outerCall;
            _instructions = new List<AssemblyElement>();
        }
        
        public FuncCallContext(FuncCallContext ctx) : base(ctx.Parent)
        {
            _symbol = ctx._symbol;
            _parametersTypes = ctx._parametersTypes;
            ArgIndex = ctx.ArgIndex;
            OuterCall = ctx.OuterCall;
            _instructions = ctx._instructions;
        }
        

        public override void SetEndInstruction(AssemblyElement element)
        {
            _callInstruction = element;
        }

        public override List<AssemblyElement> GetInstructions()
        {
            List<AssemblyElement> instructions = _instructions;
            instructions.Add(_callInstruction);
            return instructions;
        }
        
        public override void AddInstruction(AssemblyElement element)
        {
            AddInstructions(new List<AssemblyElement>() {element});
        }

        public override void AddInstructions(List<AssemblyElement> elements)
        {
            _instructions.AddRange(elements);
        }
        
        public DatSymbolType GetParameterType()
        {
            if (_symbol == DatSymbolReference.UndeclaredSymbol || (ArgIndex >= _parametersTypes.Count))
            {
                return DatSymbolType.Undefined;
            }
            return _parametersTypes[ArgIndex];
        }
    }
}