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
            var logger = new StringBufforErrorLogger();

            if (_assemblyBuilder.Errors.Any())
            {
                _assemblyBuilder.Errors.Sort((x, y) => x.CompareTo(y));

                string lastErrorBlockName = "";
                foreach (CompilationMessage error in _assemblyBuilder.Errors)
                {
                    if (lastErrorBlockName != error.ExecBlockName)
                    {
                        lastErrorBlockName = error.ExecBlockName;
                        logger.Log($"{error.FileName}: In {error.ExecBlockType} ‘{error.ExecBlockName}’:");
                    }

                    error.Print(logger);
                }
            }
            Assert.Equal(_expectedCompilationOutput, logger.GetBuffor().Trim());
        }

        [Fact]
        public void TestUndeclaredIdentifierInsideFunctionDef()
        {
            _code = @"
                func void testFunc() {
                    x = 5;
                    x = a + b(c, d);
                };
            ";

            _expectedCompilationOutput = @"
                test.d: In function ‘testFunc’:
                test.d:2:4: error: ‘x’ undeclared
                    x = 5;
                    ^
                test.d:3:4: error: ‘x’ undeclared
                    x = a + b(c, d);
                    ^
                test.d:3:8: error: ‘a’ undeclared
                    x = a + b(c, d);
                        ^
                test.d:3:12: error: ‘b’ undeclared
                    x = a + b(c, d);
                            ^
                test.d:3:14: error: ‘c’ undeclared
                    x = a + b(c, d);
                              ^
                test.d:3:17: error: ‘d’ undeclared
                    x = a + b(c, d);
                                 ^
                ";

            AssertCompilationOutputMatch();
        }

        [Fact]
        public void TestUndeclaredIdentifierInheritance()
        {
            _code = @"
                class C_NPC { var int data [200]; };
                instance self(C_NPC_MISSPELLED) {};
                prototype OrcElite(Orc);
                instance UrukHai(OrcElite);
            ";

            _expectedCompilationOutput = @"
                test.d: In instance ‘self’:
                test.d:2:14: error: ‘C_NPC_MISSPELLED’ undeclared
                instance self(C_NPC_MISSPELLED) {};
                              ^
                test.d: In prototype ‘OrcElite’:
                test.d:3:19: error: ‘Orc’ undeclared
                prototype OrcElite(Orc);
                                   ^
                ";

            AssertCompilationOutputMatch();
        }

        [Fact]
        public void TestAttributeNotFound()
        {
            _code = @"
                class C_NPC {
                    var int y;
                };
                
                instance self(C_NPC) {};
                
                func void testFunc() {
                    self.x = 5;
                    self.y = 10;
                    other.y = 15;
                };
            ";

            _expectedCompilationOutput = @"
                test.d: In function ‘testFunc’:
                test.d:8:4: error: ‘object self’ has no member named ‘x’
                    self.x = 5;
                         ^
                test.d:10:4: error: ‘other’ undeclared
                    other.y = 15;
                    ^
                ";

            AssertCompilationOutputMatch();
        }
    }
}