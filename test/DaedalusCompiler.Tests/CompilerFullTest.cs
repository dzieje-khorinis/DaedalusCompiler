/*using System;
using System.Linq;
using System.Text;
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
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _assemblyBuilder = new AssemblyBuilder();
        }

        private DatFile CompileCodeAndGetDatFileOfThat()
        {
            _assemblyBuilder.ErrorFileContext.FileContentLines = _code.Split(Environment.NewLine);
            
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

            var datFile = CompileCodeAndGetDatFileOfThat();
            Assert.Equal(4, datFile.Symbols.Count());
        }
    }
}*/