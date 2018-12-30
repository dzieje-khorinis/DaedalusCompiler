using System;
using System.IO;
using System.Linq;
using DaedalusCompiler.Compilation;
using Xunit;


namespace DaedalusCompiler.Tests
{
    public class SemanticErrorsTests
    {
        private readonly AssemblyBuilder _assemblyBuilder;
        private string _code;
        private string _externalCode;
        private string _expectedCompilationOutput;

        public SemanticErrorsTests()
        {
            _assemblyBuilder = new AssemblyBuilder();
            _externalCode = String.Empty;
            IfBlockStatementContext.NextLabelIndex = 0;

            _assemblyBuilder.ErrorContext.FilePath = "test.d";
        }

        private void ParseData()
        {
            string[] codeLines = _code.Trim().Split(Environment.NewLine);
            for (int i = 1; i < codeLines.Length; ++i)
            {
                codeLines[i] = codeLines[i].Substring(16);
            }
            _code = string.Join(Environment.NewLine, codeLines);

            string[] compilationOutputLines = _expectedCompilationOutput.Trim().Split(Environment.NewLine);
            for (int i = 1; i < compilationOutputLines.Length; ++i)
            {
                compilationOutputLines[i] = compilationOutputLines[i].Substring(16);
            }
            _expectedCompilationOutput = string.Join(Environment.NewLine, compilationOutputLines);

            _assemblyBuilder.ErrorContext.FileContentLines = _code.Split(Environment.NewLine);
            if (_externalCode != string.Empty)
            {
                _assemblyBuilder.IsCurrentlyParsingExternals = true;
                Utils.WalkSourceCode(_externalCode, _assemblyBuilder);
                _assemblyBuilder.IsCurrentlyParsingExternals = false;
            }

            Utils.WalkSourceCode(_code, _assemblyBuilder);
            _assemblyBuilder.Finish();
        }

        private void AssertCompilationOutputMatch()
        {
            ParseData();

            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            Console.SetOut(writer);

            if (_assemblyBuilder.Errors.Any())
            {
                _assemblyBuilder.Errors.Sort((x, y) => x.CompareTo(y));

                string lastErrorBlockName = "";
                foreach (CompilationMessage error in _assemblyBuilder.Errors)
                {
                    if (lastErrorBlockName != error.ExecBlockName)
                    {
                        lastErrorBlockName = error.ExecBlockName;
                        Console.WriteLine($"{error.FileName}: In {error.ExecBlockType} ‘{error.ExecBlockName}’:");
                    }

                    error.Print();
                }
            }

            writer.Flush();
            stream.Position = 0;
            StreamReader reader = new StreamReader(stream);
            Assert.Equal(_expectedCompilationOutput, reader.ReadToEnd().Trim());
        }

        [Fact]
        public void TestUndeclaredIdentifier()
        {
            _code = @"
                func void testFunc() {
                    x = 5;
                };
            ";

            _expectedCompilationOutput = @"
                test.d: In function ‘testFunc’:
                test.d:2:4: error: ‘x’ undeclared
                    x = 5;
                    ^
                ";

            AssertCompilationOutputMatch();
        }
    }
}