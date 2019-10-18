using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Antlr4.Runtime.Misc;
using DaedalusCompiler.Compilation;
using DaedalusCompiler.Compilation.SemanticAnalysis;
using DaedalusCompiler.Dat;
using Xunit;

namespace DaedalusCompiler.Tests
{
    public class ParsingSourceToAbstractAssemblyTestsBase : TestsHelper
    {
        protected string _code;
        protected string _externalCode;
        protected List<AssemblyElement> _instructions;
        protected List<AssemblyElement> _expectedInstructions;
        protected List<Symbol> _expectedSymbols;
        protected bool _parsed;

        public ParsingSourceToAbstractAssemblyTestsBase()
        {
            _instructions = new List<AssemblyElement>();
            _expectedInstructions = new List<AssemblyElement>();
            _expectedSymbols = new List<Symbol>();
            _parsed = false;
            _externalCode = String.Empty;
        }

        protected int RefIndex(string symbolName)
        {
            Symbol symbol = SymbolTable[symbolName.ToUpper()];
            return symbol.Index;
        }
        
        protected Symbol Ref(string symbolName)
        {
            return SymbolTable[symbolName.ToUpper()];
        }

        protected List<AssemblyElement> GetExecBlockInstructions(string execBlockName)
        {
            if (!_parsed)
            {
                _parsed = true;
                RunCode(_code, _externalCode);
            }
            BlockSymbol blockSymbol = (BlockSymbol) SymbolTable[execBlockName.ToUpper()];
            return blockSymbol.Instructions;
        }

        protected void AssertRefContentEqual(string symbolName, object expectedValue)
        {
            if (expectedValue == null) throw new ArgumentNullException(nameof(expectedValue));
            //ConstDefinitionNode constDefinitionNode = (ConstDefinitionNode) Ref(symbolName).Node;
            
            
            
            Assert.Equal(expectedValue, constDefinitionNode.RightSideValue);//TODO who did this ever work?
        }

        protected void AssertInstructionsMatch()
        {
            for (var index = 0; index < _expectedInstructions.Count; index++)
            {
                var instruction = _instructions[index];
                var expectedInstruction = _expectedInstructions[index];
                CompareInstructions(expectedInstruction, instruction);
            }
        }

        protected void AssertSymbolsMatch()
        {
            Assert.Equal(_expectedSymbols.Count, _assemblyBuilder.Symbols.Count);
            for (var index = 0; index < _expectedSymbols.Count; index++)
            {
                var symbol = _assemblyBuilder.Symbols[index];
                var expectedSymbol = _expectedSymbols[index];
                Assert.Equal(index, symbol.Index);
                Assert.Equal(expectedSymbol, symbol);
            }
        }


        protected void CompareInstructions(AssemblyElement expectedInstruction, AssemblyElement instruction)
        {
            if (expectedInstruction == null) throw new ArgumentNullException(nameof(expectedInstruction));
            Assert.Equal(expectedInstruction.GetType(), instruction.GetType());
            switch (instruction)
            {
                case PushArrayVar pushArrVarInstruction:
                {
                    Assert.Equal(
                        ((SymbolInstruction) expectedInstruction).Symbol.Index,
                        ((SymbolInstruction) instruction).Symbol.Index
                    );
                    Assert.Equal(
                        ((PushArrayVar) expectedInstruction).Index,
                        pushArrVarInstruction.Index
                    );
                    break;
                }
                
                case ValueInstruction valueInstruction:
                {
                    Assert.Equal(
                        ((PushInt) expectedInstruction).Value,
                        valueInstruction.Value
                    );
                    break;
                }

                case SymbolInstruction _:
                    Assert.Equal(
                        ((SymbolInstruction) expectedInstruction).Symbol.Index,
                        ((SymbolInstruction) instruction).Symbol.Index
                    );
                    break;
                
                case AssemblyLabel assemblyLabelInstruction:
                {
                    Assert.Equal(
                        ((AssemblyLabel) expectedInstruction).Label,
                        assemblyLabelInstruction.Label
                    );
                    break;
                }

                case JumpIfToLabel _:
                case JumpToLabel _:
                {
                    var jumpInstruction = (JumpToLabel) instruction;
                    Assert.Equal(
                        ((JumpToLabel) expectedInstruction).Label,
                        jumpInstruction.Label);
                    break;
                }
            }
        }
    }
}