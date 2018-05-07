using DaedalusCompiler.Compilation;
using DaedalusCompiler.Dat;

namespace DaedalusCompiler.Tests
{
    namespace Helpers
    {
        class TestFunction
        {

            private DatSymbol funcSymbol;
            private AssemblyBuilder ab;

            public TestFunction(AssemblyBuilder ab, string funcName = "test_func")
            {
                funcSymbol = SymbolBuilder.BuildFunc(funcName, DatSymbolType.Void);
                this.ab = ab;
            }

            public void start()
            {
                ab.execBlockStart(funcSymbol, ExecutebleBlockType.Function);
            }

            public void end()
            {
                ab.execBlockEnd();
            }
        }
    }
}