using System.Collections.Generic;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Compilation
{
    public class AssemblyBuilderUsage
    {
        public AssemblyBuilderUsage()
        {
            var assemblyBuilder = new AssemblyBuilder();
            var functionName = "someFunc";
            var functionSecondName = "secondFunc";
            
            assemblyBuilder.addSymbol(SymbolBuilder.BuildFunc("Print", DatSymbolType.Void));

            assemblyBuilder.addSymbol(SymbolBuilder.BuildVariable("someVar1", DatSymbolType.Int));
            assemblyBuilder.addSymbol(SymbolBuilder.BuildConst("someConst1", DatSymbolType.Int, 34));

            assemblyBuilder.addSymbol(SymbolBuilder.BuildVariable(functionName + ".param1", DatSymbolType.Int));
            assemblyBuilder.addSymbol(SymbolBuilder.BuildVariable(functionName + ".param2", DatSymbolType.Int));
            assemblyBuilder.addSymbol(SymbolBuilder.BuildVariable(functionName + ".inner", DatSymbolType.Int));

            //assemblyBuilder.registerFunction(functionName);

            List<AssemblyInstruction> functionBody = new List<AssemblyInstruction>(new AssemblyInstruction[]
            {
                new PushVar() { symbol = assemblyBuilder.resolveSymbol("param1")},
                new Assign(),
                new PushVar() { symbol = assemblyBuilder.resolveSymbol("param2")},
                new Assign(),
                new PushVar() { symbol = assemblyBuilder.resolveSymbol("param1")},
                new PushVar() { symbol = assemblyBuilder.resolveSymbol("param2")},
                new Add(),
                new PushVar() { symbol = assemblyBuilder.resolveSymbol("inner")},
                /* ... */
                new JumpIf() { label = "jump1"},
                new PushVar() { symbol = assemblyBuilder.resolveSymbol("inner")},
                //new PushInt() { value = 10},
                new Assign(),
                //new LabelInstruction() { label = "jump1"},
                //new PushInt() { value = 110},
                new CallExternal() { symbol = assemblyBuilder.resolveSymbol("Print") }, 
                new Call() { label = functionSecondName}
                /* e.t.c */
            });
            
            //assemblyBuilder.registerFunction(functionSecondName);
            //assemblyBuilder.addFunctionBody(functionName, functionBody);
        }
    }
}