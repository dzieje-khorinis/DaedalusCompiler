using System;
using System.Collections.Generic;
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

            _assemblyBuilder.ErrorFileContext.FilePath = "test.d";
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

            _assemblyBuilder.ErrorFileContext.FileContentLines = _code.Split(Environment.NewLine);
            _assemblyBuilder.ErrorFileContext.SuppressedWarningCodes = Compiler.GetWarningCodesToSuppress(
                _assemblyBuilder.ErrorFileContext.FileContentLines[0]
            );
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
            logger.LogLine("");

            List<CompilationMessage> errors = new List<CompilationMessage>();
            foreach (CompilationMessage error in _assemblyBuilder.Errors)
            {
                if (error is CompilationError)
                {
                    errors.Add(error);
                }
                else if (error is CompilationWarning compilationWarning)
                {
                    if (compilationWarning.IsSuppressed == false)
                    {
                        errors.Add(compilationWarning);
                    }
                }
            }

            if (errors.Any())
            {
                errors.Sort((x, y) => x.CompareTo(y));

                string lastErrorBlockName = null;
                foreach (CompilationMessage error in errors)
                {
                    if (lastErrorBlockName != error.ExecBlockName)
                    {
                        lastErrorBlockName = error.ExecBlockName;
                        if (error.ExecBlockName == null)
                        {
                            logger.LogLine($"{error.FileName}: In global scope:");
                        }
                        else
                        {
                            logger.LogLine($"{error.FileName}: In {error.ExecBlockType} ‘{error.ExecBlockName}’:");
                        }
                        
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
        
        
        [Fact]
        public void TestInconsistentConstArraySizeTooManyElements()
        {
            _code = @"
                const int INVENTORY_SIZE = 2;
                
                const string INVENTORY[INVENTORY_SIZE] =
                {
                    ""SWORD"",
                    ""BOW"",
                    ""AXE""	
                };
                
                func void testFunc() {
                    const int LUCKY_NUMBERS[2] = {2, 4, 8};
                };
                
                const string NAMES[2] = {""DIEGO"", ""ROBERTO"", ""SANCHEZ""};
            ";

            _expectedCompilationOutput = @"
                test.d:3:23: error: array ‘INVENTORY’ has inconsistent size (declared size: 2, elements count: 3)
                const string INVENTORY[INVENTORY_SIZE] =
                                       ^
                test.d: In function ‘testFunc’:
                test.d:11:28: error: array ‘LUCKY_NUMBERS’ has inconsistent size (declared size: 2, elements count: 3)
                    const int LUCKY_NUMBERS[2] = {2, 4, 8};
                                            ^
                test.d: In global scope:
                test.d:14:19: error: array ‘NAMES’ has inconsistent size (declared size: 2, elements count: 3)
                const string NAMES[2] = {""DIEGO"", ""ROBERTO"", ""SANCHEZ""};
                                   ^
            ";

            AssertCompilationOutputMatch();
        }
        
        
        [Fact]
        public void TestInconsistentConstArraySizeNotEnoughElements()
        {
            _code = @"
                const int INVENTORY_SIZE = 4;
                
                const string INVENTORY[INVENTORY_SIZE] =
                {
                    ""SWORD"",
                    ""BOW"",
                    ""AXE""	
                };
                
                func void testFunc() {
                    const int LUCKY_NUMBERS[4] = {2, 4, 8};
                };
                
                const string NAMES[4] = {""DIEGO"", ""ROBERTO"", ""SANCHEZ""};
            ";

            _expectedCompilationOutput = @"
                test.d:3:23: error: array ‘INVENTORY’ has inconsistent size (declared size: 4, elements count: 3)
                const string INVENTORY[INVENTORY_SIZE] =
                                       ^
                test.d: In function ‘testFunc’:
                test.d:11:28: error: array ‘LUCKY_NUMBERS’ has inconsistent size (declared size: 4, elements count: 3)
                    const int LUCKY_NUMBERS[4] = {2, 4, 8};
                                            ^
                test.d: In global scope:
                test.d:14:19: error: array ‘NAMES’ has inconsistent size (declared size: 4, elements count: 3)
                const string NAMES[4] = {""DIEGO"", ""ROBERTO"", ""SANCHEZ""};
                                   ^
            ";

            AssertCompilationOutputMatch();
        }
        
        [Fact]
        public void TestInvalidConstArraySizeElements()
        {
            _code = @"
                const string INVENTORY[INVENTORY_SIZE] =
                {
                    ""SWORD"",
                    ""BOW"",
                    ""AXE""	
                };
                
                const int INVENTORY_SIZE = 3;
                
                const int NUMBERS_SIZE = 0;
                
                func void testFunc() {
                    const int LUCKY_NUMBERS[NUMBERS_SIZE] = {2, 4, 8};
                };
                
                const string NAMES[0] = {""DIEGO"", ""ROBERTO"", ""SANCHEZ""};
            ";

            _expectedCompilationOutput = @"
                test.d:1:23: error: ‘INVENTORY_SIZE’ undeclared
                const string INVENTORY[INVENTORY_SIZE] =
                                       ^
                test.d: In function ‘testFunc’:
                test.d:13:28: error: array ‘LUCKY_NUMBERS’ has invalid size ‘0’
                    const int LUCKY_NUMBERS[NUMBERS_SIZE] = {2, 4, 8};
                                            ^
                test.d: In global scope:
                test.d:16:19: error: array ‘NAMES’ has invalid size ‘0’
                const string NAMES[0] = {""DIEGO"", ""ROBERTO"", ""SANCHEZ""};
                                   ^
            ";

            AssertCompilationOutputMatch();
        }

        [Fact]
        public void TestIntValueOutOfMinAndMaxRange()
        {
            _code = @"
                const int MAX_INT = 2147483647;
                const int MIN_INT = -2147483648;
                const int TOO_BIG_INT_POSITIVE = 2147483648;
                const int TOO_BIG_INT_NEGATIVE = -2147483649;
                
                func void testFunc() {
                    var int max_int;
                    max_int = 2147483647;
                    var int min_int;
                    min_int = -2147483647;  // not -2147483648, because minus and number value are in separate assembly instructions
                    var int too_big_int_positive;
                    too_big_int_positive = 2147483648;
                    var int too_big_int_negative;
                    too_big_int_negative = -2147483648;
                };
            ";

            _expectedCompilationOutput = @"
                test.d:3:33: error: integer literal is too large to be represented in an integer type
                const int TOO_BIG_INT_POSITIVE = 2147483648;
                                                 ^
                test.d:4:33: error: integer literal is too large to be represented in an integer type
                const int TOO_BIG_INT_NEGATIVE = -2147483649;
                                                 ^
                test.d: In function ‘testFunc’:
                test.d:12:27: error: integer literal is too large to be represented in an integer type
                    too_big_int_positive = 2147483648;
                                           ^
                test.d:14:27: error: integer literal is too large to be represented in an integer type
                    too_big_int_negative = -2147483648;
                                           ^
            ";

            AssertCompilationOutputMatch();
        }
        
        [Fact]
        public void TestSingleExpressionHack()
        {
            _code = @"
                func void testFunc() {
                    var int x;
                    x = 5;
                    x;
                    return;
                };
            ";

            _expectedCompilationOutput = @"
                test.d: In function ‘testFunc’:
                test.d:4:4: warning W1: usage of single-expression statement hack
                    x;
                    ^
            ";

            AssertCompilationOutputMatch();
        }
        
        [Fact]
        public void TestSingleExpressionHackStrictMode()
        {
            _assemblyBuilder.StrictSyntax = true;

            _code = @"
                func void testFunc() {
                    var int x;
                    x = 5;
                    x;
                    return;
                };
            ";

            _expectedCompilationOutput = @"
                test.d: In function ‘testFunc’:
                test.d:4:4: error W1: usage of single-expression statement hack
                    x;
                    ^
            ";

            AssertCompilationOutputMatch();
        }
        
        [Fact]
        public void TestSingleExpressionHackWarningLineSuppress()
        {
            _code = @"
                func void testFunc() {
                    var int x;
                    x = 5;
                    x; //suppress: W1
                    return;
                };
            ";

            _expectedCompilationOutput = @"";

            AssertCompilationOutputMatch();
        }

        [Fact]
        public void TestSingleExpressionHackWarningFileSuppress()
        {
            _code = @"
                //suppress: W1
                
                func void testFunc() {
                    var int x;
                    x = 5;
                    x;
                    return;
                };
            ";

            _expectedCompilationOutput = @"";

            AssertCompilationOutputMatch();
        }
        
        
        [Fact]
        public void TestArgumentsCountDoesNotMatch()
        {
            _code = @"
                func int testFunc(var int a) {};
                
                func int secondFunc() {
                    testFunc();
                    testFunc(1);
                    testFunc(2, 3);
                };
            ";

            _expectedCompilationOutput = @"
                test.d: In function ‘secondFunc’:
                test.d:4:4: error: too few arguments to function call, expected 1, have 0
                    testFunc();
                    ^
                test.d:1:9: note: 'testFunc' declared here
                func int testFunc(var int a) {};
                         ^
                test.d:6:4: error: too many arguments to function call, expected 1, have 2
                    testFunc(2, 3);
                    ^
                test.d:1:9: note: 'testFunc' declared here
                func int testFunc(var int a) {};
                         ^
            ";

            AssertCompilationOutputMatch();
        }
    }
}