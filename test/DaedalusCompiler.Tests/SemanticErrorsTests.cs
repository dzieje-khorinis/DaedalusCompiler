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
        private AssemblyBuilder _assemblyBuilder;
        private string _code;
        private string _externalCode;
        private string _expectedCompilationOutput;

        public SemanticErrorsTests()
        {
            ResetTestsConfiguration();
        }

        private void ResetTestsConfiguration()
        {
            _assemblyBuilder = new AssemblyBuilder();
            _assemblyBuilder.ErrorFileContext.FilePath = "test.d";
            _externalCode = String.Empty;
            IfBlockStatementContext.NextLabelIndex = 0;
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

        [Fact]
        public void TestConstAssignment()
        {
            _code = @"
                const int a = 1;
                const int b = 1.5; //error
                const int c = 2 + 2;
                const int d = 2.5 + 3; //error
                const int e = 4 + 4.5; //error
                const int f = 5.5 + 5.5; //error
                
                const float g = 1;
                const float h = 1.5;
                const float i = 2 + 2; //error
                const float j = 2.5 + 3; //error
                const float k = 4 + 4.5; //error
                const float l = 5.5 + 5.5; //error
                
                const string m = ""ha"";
                const string n = ""ha"" + ""ha""; //error
            ";

            _expectedCompilationOutput = @"
                test.d:2:14: error: unable to evaluate const value
                const int b = 1.5; //error
                              ^
                test.d:4:14: error: unable to evaluate const value
                const int d = 2.5 + 3; //error
                              ^
                test.d:5:14: error: unable to evaluate const value
                const int e = 4 + 4.5; //error
                              ^
                test.d:6:14: error: unable to evaluate const value
                const int f = 5.5 + 5.5; //error
                              ^
                test.d:10:16: error: unable to evaluate const value
                const float i = 2 + 2; //error
                                ^
                test.d:11:16: error: unable to evaluate const value
                const float j = 2.5 + 3; //error
                                ^
                test.d:12:16: error: unable to evaluate const value
                const float k = 4 + 4.5; //error
                                ^
                test.d:13:16: error: unable to evaluate const value
                const float l = 5.5 + 5.5; //error
                                ^
                test.d:16:17: error: unable to evaluate const value
                const string n = ""ha"" + ""ha""; //error
                                 ^
                ";

            AssertCompilationOutputMatch();
        }

        [Fact]
        public void TestGlobalRedefinition()
        {
            _code = @"
                class __class {};
                func void __class() {};
                func void __class() {};
            ";

            _expectedCompilationOutput = @"
                test.d:2:10: error: redefinition of '__class'
                func void __class() {};
                          ^
                test.d:1:6: note: previous definition is here
                class __class {};
                      ^
                test.d:3:10: error: redefinition of '__class'
                func void __class() {};
                          ^
                test.d:1:6: note: previous definition is here
                class __class {};
                      ^
                ";
            AssertCompilationOutputMatch();
            ResetTestsConfiguration();

            _code = @"
                func void __func() {};
                class __func {};
                class __func {};
            ";

            _expectedCompilationOutput = @"
                test.d:2:6: error: redefinition of '__func'
                class __func {};
                      ^
                test.d:1:10: note: previous definition is here
                func void __func() {};
                          ^
                test.d:3:6: error: redefinition of '__func'
                class __func {};
                      ^
                test.d:1:10: note: previous definition is here
                func void __func() {};
                          ^
                ";

            AssertCompilationOutputMatch();
            ResetTestsConfiguration();
            
            _code = @"
                class C_NPC { var int data [200]; };
                instance __instanceDecl(C_NPC);
                prototype __instanceDecl(C_NPC) {};
                prototype __instanceDecl(C_NPC) {};
            ";

            _expectedCompilationOutput = @"
                test.d:3:10: error: redefinition of '__instanceDecl'
                prototype __instanceDecl(C_NPC) {};
                          ^
                test.d:2:9: note: previous definition is here
                instance __instanceDecl(C_NPC);
                         ^
                test.d:4:10: error: redefinition of '__instanceDecl'
                prototype __instanceDecl(C_NPC) {};
                          ^
                test.d:2:9: note: previous definition is here
                instance __instanceDecl(C_NPC);
                         ^
                ";

            AssertCompilationOutputMatch();
            ResetTestsConfiguration();
            
            _code = @"
                class C_NPC { var int data [200]; };
                prototype __prototype(C_NPC) {};
                instance __prototype(C_NPC);
                instance __prototype(C_NPC);
            ";

            _expectedCompilationOutput = @"
                test.d:3:9: error: redefinition of '__prototype'
                instance __prototype(C_NPC);
                         ^
                test.d:2:10: note: previous definition is here
                prototype __prototype(C_NPC) {};
                          ^
                test.d:4:9: error: redefinition of '__prototype'
                instance __prototype(C_NPC);
                         ^
                test.d:2:10: note: previous definition is here
                prototype __prototype(C_NPC) {};
                          ^
                ";

            AssertCompilationOutputMatch();
            ResetTestsConfiguration();
            
            _code = @"
                class C_NPC { var int data [200]; };
                instance instanceDef(C_NPC) {};
                const int instanceDef = 0;
                const int instanceDef = 0;
            ";

            _expectedCompilationOutput = @"
                test.d:3:10: error: redefinition of 'instanceDef'
                const int instanceDef = 0;
                          ^
                test.d:2:9: note: previous definition is here
                instance instanceDef(C_NPC) {};
                         ^
                test.d:4:10: error: redefinition of 'instanceDef'
                const int instanceDef = 0;
                          ^
                test.d:2:9: note: previous definition is here
                instance instanceDef(C_NPC) {};
                         ^
                ";

            AssertCompilationOutputMatch();
            ResetTestsConfiguration();
            
            _code = @"
                class C_NPC { var int data [200]; };
                const int constInt = 0;
                instance constInt(C_NPC) {};
                instance constInt(C_NPC) {};
            ";

            _expectedCompilationOutput = @"
                test.d:3:9: error: redefinition of 'constInt'
                instance constInt(C_NPC) {};
                         ^
                test.d:2:10: note: previous definition is here
                const int constInt = 0;
                          ^
                test.d:4:9: error: redefinition of 'constInt'
                instance constInt(C_NPC) {};
                         ^
                test.d:2:10: note: previous definition is here
                const int constInt = 0;
                          ^
                ";

            AssertCompilationOutputMatch();
            ResetTestsConfiguration();
            
            _code = @"
                const int constIntArr[2] = {0, 1};
                var float constIntArr;
                var float constIntArr;
            ";

            _expectedCompilationOutput = @"
                test.d:2:10: error: redefinition of 'constIntArr'
                var float constIntArr;
                          ^
                test.d:1:10: note: previous definition is here
                const int constIntArr[2] = {0, 1};
                          ^
                test.d:3:10: error: redefinition of 'constIntArr'
                var float constIntArr;
                          ^
                test.d:1:10: note: previous definition is here
                const int constIntArr[2] = {0, 1};
                          ^
                ";

            AssertCompilationOutputMatch();
            ResetTestsConfiguration();
            
            _code = @"
                var float varFloat;
                const int varFloat[2] = {0, 1};
                const int varFloat[2] = {0, 1};
            ";

            _expectedCompilationOutput = @"
                test.d:2:10: error: redefinition of 'varFloat'
                const int varFloat[2] = {0, 1};
                          ^
                test.d:1:10: note: previous definition is here
                var float varFloat;
                          ^
                test.d:3:10: error: redefinition of 'varFloat'
                const int varFloat[2] = {0, 1};
                          ^
                test.d:1:10: note: previous definition is here
                var float varFloat;
                          ^
                ";

            AssertCompilationOutputMatch();
        }

        [Fact]
        public void TestLocalRedefinition()
        {
            _code = @"
                class C_NPC { var int data [200]; };
                var int a;
                var float b;
                var string c;
                var C_NPC d;
                var func e;
                
                func void firstFunc() {
                    var int a;
                    var float b;
                    var string c;
                    var C_NPC d;
                    var func e;
                    
                    var int a;
                    var float b;
                    var string c;
                    var C_NPC d;
                    var func e;
                }
                
                func void secondFunc() {
                    var int a;
                    var float b;
                    var string c;
                    var C_NPC d;
                    var func e;
                }
            ";

            _expectedCompilationOutput = @"
                test.d: In function ‘firstFunc’:
                test.d:15:12: error: redefinition of 'a'
                    var int a;
                            ^
                test.d:9:12: note: previous definition is here
                    var int a;
                            ^
                test.d:16:14: error: redefinition of 'b'
                    var float b;
                              ^
                test.d:10:14: note: previous definition is here
                    var float b;
                              ^
                test.d:17:15: error: redefinition of 'c'
                    var string c;
                               ^
                test.d:11:15: note: previous definition is here
                    var string c;
                               ^
                test.d:18:14: error: redefinition of 'd'
                    var C_NPC d;
                              ^
                test.d:12:14: note: previous definition is here
                    var C_NPC d;
                              ^
                test.d:19:13: error: redefinition of 'e'
                    var func e;
                             ^
                test.d:13:13: note: previous definition is here
                    var func e;
                             ^
                ";
            AssertCompilationOutputMatch();
        }
        
        
        [Fact]
        public void TestNotValidClassOrPrototype()
        {
            _code = @"
                class myClass {};
                func void myFunc() {};
                
                instance WRONG1(myFunc);
                instance WRONG2(myFunc) {};
                prototype WRONG3(myFunc) {};
                
                instance CORRECT1(myClass);
                instance CORRECT2(myClass) {};
                prototype CORRECT3(myClass) {};
                
                instance CORRECT4(WRONG3);
                instance CORRECT5(WRONG3) {};
                prototype CORRECT6(WRONG3) {};
            ";

            _expectedCompilationOutput = @"
                test.d:4:16: error: not a valid class or prototype
                instance WRONG1(myFunc);
                                ^
                test.d: In instance ‘WRONG2’:
                test.d:5:16: error: not a valid class or prototype
                instance WRONG2(myFunc) {};
                                ^
                test.d: In prototype ‘WRONG3’:
                test.d:6:17: error: not a valid class or prototype
                prototype WRONG3(myFunc) {};
                                 ^
                ";
            AssertCompilationOutputMatch();
        }
        
        [Fact]
        public void TestTooBigArraySize()
        {
            _code = @"
                var int a[4095];
                var int b[4096];
                
                func void myFunc(var int c[4095], var int d[4096]) {
                    var int e[4095];
                    var int f[4096];
                };
            ";

            _expectedCompilationOutput = @"
                test.d:2:10: error: too big array size (max: 4095)
                var int b[4096];
                          ^
                test.d: In function ‘myFunc’:
                test.d:4:44: error: too big array size (max: 4095)
                func void myFunc(var int c[4095], var int d[4096]) {
                                                            ^
                test.d:6:14: error: too big array size (max: 4095)
                    var int f[4096];
                              ^
            ";

            AssertCompilationOutputMatch();
        }
        
        [Fact]
        public void TestArrayIndexOutOfRange()
        {
            _code = @"
                const int a[2] = {2 , 3};
                
                func void myFunc(var int b[3]) {
                    var int c[4];
                    var int x;
                    x = a[1];
                    x = a[2];
                    x = b[2];
                    x = b[3];
                    x = c[3];
                    x = c[4];
                };
            ";

            _expectedCompilationOutput = @"
                test.d: In function ‘myFunc’:
                test.d:7:10: error: array index out of range (max index for this array is 1)
                    x = a[2];
                          ^
                test.d:9:10: error: array index out of range (max index for this array is 2)
                    x = b[3];
                          ^
                test.d:11:10: error: array index out of range (max index for this array is 3)
                    x = c[4];
                          ^
            ";

            AssertCompilationOutputMatch();
        }
        
        [Fact]
        public void TestTooBigArrayIndex()
        {
            _code = @"
                const int a[2] = {2 , 3};
                var int b[4095];
                
                func void myFunc() {
                    var int x;
                    x = a[255];
                    x = a[256];
                    x = b[255];
                    x = b[256];
                };
            ";

            _expectedCompilationOutput = @"
                test.d: In function ‘myFunc’:
                test.d:6:10: error: array index out of range (max index for this array is 1)
                    x = a[255];
                          ^
                test.d:7:10: error: array index out of range (max index for this array is 1)
                    x = a[256];
                          ^
                test.d:9:10: error: too big array index (max: 255)
                    x = b[256];
                          ^
            ";

            AssertCompilationOutputMatch();
        }
    }
}