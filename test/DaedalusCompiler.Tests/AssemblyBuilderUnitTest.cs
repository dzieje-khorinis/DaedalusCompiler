using DaedalusCompiler.Compilation;
using DaedalusCompiler.Dat;
using Xunit;

namespace DaedalusCompiler.Tests
{
    public class AssemblyBuilderTest
    {        
        [Fact]
        public void CorrectlyCreateFunction()
        {
            var builder = new AssemblyBuilder();
            var symbol = SymbolBuilder.BuildVariable("test", DatSymbolType.Int);
            
            builder.execBlockStart(symbol, ExecutebleBlockType.Function);
            builder.execBlockEnd();
            Assert.Equal( 1, builder.functions.Count );
        }
    }
}