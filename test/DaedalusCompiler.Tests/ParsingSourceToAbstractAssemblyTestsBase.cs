using System;
using System.Collections.Generic;
using System.Linq;
using Commmon.SemanticAnalysis;
using Common;
using Xunit;
using AssemblyElement = Common.AssemblyElement;
using BlockSymbol = Common.BlockSymbol;
using PushArrayVar = Common.PushArrayVar;
using Symbol = Common.Symbol;
using SymbolInstruction = Common.SymbolInstruction;
using ValueInstruction = Common.ValueInstruction;

namespace DaedalusCompiler.Tests
{
    public class ParsingSourceToAbstractAssemblyTestsBase
    {
        protected string Code;
        protected List<AssemblyElement> Instructions;
        protected List<AssemblyElement> ExpectedInstructions;
        protected List<Symbol> ExpectedSymbols;
        private bool _parsed;
        private TestsHelper _testsHelper;

        protected ParsingSourceToAbstractAssemblyTestsBase()
        {
            Instructions = new List<AssemblyElement>();
            ExpectedInstructions = new List<AssemblyElement>();
            ExpectedSymbols = new List<Symbol>();
            _parsed = false;
            _testsHelper = null;
        }

        protected int RefIndex(string symbolName)
        {
            Symbol symbol = _testsHelper.SymbolTable[symbolName.ToUpper()];
            return symbol.Index;
        }
        
        protected Symbol Ref(string symbolName)
        {
            if (symbolName.StartsWith((char) 255))
            {
                return _testsHelper.SymbolTable[symbolName];
            }
            return _testsHelper.SymbolTable[symbolName.ToUpper()];
        }

        protected void ParseData()
        {
            _parsed = true;
            StringBufforErrorLogger logger = new StringBufforErrorLogger();
            _testsHelper = new TestsHelper(logger, false, false);
            _testsHelper.RunCode(Code);
        }
        
        protected List<AssemblyElement> GetExecBlockInstructions(string execBlockName)
        {
            if (!_parsed)
            {
                ParseData();
            }
            BlockSymbol blockSymbol = (BlockSymbol) _testsHelper.SymbolTable[execBlockName.ToUpper()];
            return blockSymbol.Instructions;
        }

        protected void AssertRefContentEqual(string symbolName, object expectedValue)
        {
            if (expectedValue == null) throw new ArgumentNullException(nameof(expectedValue));
            Assert.Equal(expectedValue, Ref(symbolName).Content[0]);
        }

        protected void AssertInstructionsMatch()
        {
            for (var index = 0; index < ExpectedInstructions.Count; index++)
            {
                var instruction = Instructions[index];
                var expectedInstruction = ExpectedInstructions[index];
                CompareInstructions(expectedInstruction, instruction);
            }
        }

        protected void AssertSymbolsMatch()
        {
            List<Symbol> symbols = _testsHelper.SymbolTable.Values.ToList();
            
            symbols.Sort((x, y) => x.Index.CompareTo(y.SubIndex));
            symbols.Sort((x, y) => x.Index.CompareTo(y.Index));
            

            
            Assert.Equal(ExpectedSymbols.Count, symbols.Count);
            for (var index = 0; index < ExpectedSymbols.Count; index++)
            {
                var symbol = symbols[index];
                var expectedSymbol = ExpectedSymbols[index];
                //TODO add Assert.Equal(index, symbol.Index); because of subindexing?
                Assert.Equal(expectedSymbol.Index, symbol.Index);
                Assert.Equal(expectedSymbol, symbol);
            }
        }


        private void CompareInstructions(AssemblyElement expectedInstruction, AssemblyElement instruction)
        {
            if (expectedInstruction == null) throw new ArgumentNullException(nameof(expectedInstruction));
            Assert.Equal(expectedInstruction.GetType(), instruction.GetType());
            switch (instruction)
            {
                case PushArrayVar pushArrVarInstruction:
                {
                    Assert.Equal(
                        ((SymbolInstruction) expectedInstruction).Symbol.Index,
                        pushArrVarInstruction.Symbol.Index
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

                case SymbolInstruction symbolInstruction:
                    if (((SymbolInstruction) expectedInstruction).Symbol.Index != symbolInstruction.Symbol.Index)
                    {
                        Console.Write("ds");
                    }
                    Assert.Equal(
                        ((SymbolInstruction) expectedInstruction).Symbol.Index,
                        symbolInstruction.Symbol.Index
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