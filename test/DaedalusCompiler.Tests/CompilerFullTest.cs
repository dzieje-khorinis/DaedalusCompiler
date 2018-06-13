using System.Linq;
using DaedalusCompiler.Compilation;
using DaedalusCompiler.Dat;
using Xunit;

namespace DaedalusCompiler.Tests
{
    public class CompilerFullTest
    {
        private string _code;
        private AssemblyBuilder _assemblyBuilder;

        public CompilerFullTest()
        {
            _assemblyBuilder = new AssemblyBuilder();
        }

        private DatFile compileCodeAndGetDATFileOfThat()
        {
            Utils.WalkSourceCode(_code, _assemblyBuilder);
            var datBuilder = new DatBuilder(_assemblyBuilder);
            var datFile = datBuilder.GetDatFile();
            var datToReturn = new DatFile();
            
            datToReturn.Load(datFile.getBinary());

            return datToReturn;
        }
        
        [Fact]
        public void TestIfSymbolsAreInDatFile()
        {
            _code = @"
                var int x;
                var int y;
                var int z;

                func void testFunc() {
                };
            ";

            var datFile = compileCodeAndGetDATFileOfThat();
            
            Assert.Equal(datFile.Symbols.Count(), 4);
        }
    }
}