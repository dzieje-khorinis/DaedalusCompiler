using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Antlr4.Runtime.Misc;
using DaedalusCompiler.Compilation;
using DaedalusCompiler.Compilation.SemanticAnalysis;
using DaedalusCompiler.Dat;
using Xunit;

namespace DaedalusCompiler.Tests
{
    public class ParsingSourceToAbstractAssemblyTestsBase
    {
        protected string _code;
        protected string _externalCode;
        protected List<AssemblyElement> _instructions;
        protected List<AssemblyElement> _expectedInstructions;
        protected List<Symbol> _expectedSymbols;
        protected bool _parsed;
        protected TestsHelper _testsHelper;

        public ParsingSourceToAbstractAssemblyTestsBase()
        {
            _instructions = new List<AssemblyElement>();
            _expectedInstructions = new List<AssemblyElement>();
            _expectedSymbols = new List<Symbol>();
            _parsed = false;
            _externalCode = String.Empty;
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
            _testsHelper.RunCode(_code, _externalCode);
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
            for (var index = 0; index < _expectedInstructions.Count; index++)
            {
                var instruction = _instructions[index];
                var expectedInstruction = _expectedInstructions[index];
                CompareInstructions(expectedInstruction, instruction);
            }
        }

        protected void AssertSymbolsMatch()
        {
            List<Symbol> symbols = _testsHelper.SymbolTable.Values.ToList();
            
            symbols.Sort((x, y) => x.Index.CompareTo(y.SubIndex));
            symbols.Sort((x, y) => x.Index.CompareTo(y.Index));
            

            
            Assert.Equal(_expectedSymbols.Count, symbols.Count);
            for (var index = 0; index < _expectedSymbols.Count; index++)
            {
                var symbol = symbols[index];
                var expectedSymbol = _expectedSymbols[index];
                //Assert.Equal(index, symbol.Index); //because of subindexing
                Assert.Equal(expectedSymbol.Index, symbol.Index);
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
    
    
    
    /*
    public class ParsingSourceToAbstractAssemblyTestsBase
    {
        protected readonly AssemblyBuilder _assemblyBuilder;
        protected string _code;
        protected string _externalCode;
        protected List<AssemblyElement> _instructions;
        protected List<AssemblyElement> _expectedInstructions;
        protected List<DatSymbol> _expectedSymbols;
        protected bool _parsed;

        public ParsingSourceToAbstractAssemblyTestsBase()
        {
            _assemblyBuilder = new AssemblyBuilder();
            _instructions = new List<AssemblyElement>();
            _expectedInstructions = new List<AssemblyElement>();
            _expectedSymbols = new List<DatSymbol>();
            _parsed = false;
            _externalCode = String.Empty;
        }

        protected int RefIndex(string symbolName)
        {
            DatSymbol symbol = _assemblyBuilder.ResolveSymbol(symbolName);
            return symbol.Index;
        }
        
        protected DatSymbol Ref(string symbolName)
        {
            _assemblyBuilder.ActiveExecBlock = null;
            return _assemblyBuilder.ResolveSymbol(symbolName);
        }

        protected List<AssemblyElement> GetExecBlockInstructions(string execBlockName)
        {
            if (!_parsed)
            {
                ParseData();
            }

            List<AssemblyElement> instructions = new List<AssemblyElement>();

            bool alreadyFound = false;
            foreach (BaseExecBlockContext execBlock in _assemblyBuilder.ExecBlocks)
            {
                DatSymbol testedSymbol = execBlock.GetSymbol();
                if (execBlock is SharedExecBlockContext sharedExecBlockContext)
                {
                    foreach (DatSymbol symbol in sharedExecBlockContext.Symbols)
                    {
                        if (symbol.Name.ToUpper() == execBlockName.ToUpper())
                        {
                            testedSymbol = symbol;
                            break;
                        }
                    }
                }
                
                if (testedSymbol.Name.ToUpper() == execBlockName.ToUpper())
                {
                    instructions.AddRange(execBlock.Body);
                    _assemblyBuilder.ActiveExecBlock = execBlock;
                    alreadyFound = true;
                }
                else if (alreadyFound)
                {
                    break;
                }
            }

            if (alreadyFound == false )
            {
                throw new KeyNotFoundException();
            }

            return instructions;
        }

        protected void ParseData()
        {
            _parsed = true;
            
            if (_externalCode != string.Empty)
            {
                _assemblyBuilder.IsCurrentlyParsingExternals = true;
                _assemblyBuilder.ErrorFileContext.FileContentLines = _externalCode.Split(Environment.NewLine);
                Utils.WalkSourceCode(_externalCode, _assemblyBuilder);
                _assemblyBuilder.IsCurrentlyParsingExternals = false;
            }

            _assemblyBuilder.ErrorFileContext.FileContentLines = _code.Split(Environment.NewLine);
            _assemblyBuilder.ErrorFileContext.SuppressedWarningCodes = Compiler.GetWarningCodesToSuppress(
                _assemblyBuilder.ErrorFileContext.FileContentLines[0]
            );
            Utils.WalkSourceCode(_code, _assemblyBuilder);
            _assemblyBuilder.Finish();
        }

        protected void AssertRefContentEqual(string symbolName, object expectedValue)
        {
            if (expectedValue == null) throw new ArgumentNullException(nameof(expectedValue));
            Assert.Equal(expectedValue, Ref(symbolName).Content[0]);
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
    */
}