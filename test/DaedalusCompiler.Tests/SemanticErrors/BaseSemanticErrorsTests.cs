using System;
using DaedalusCompiler.Compilation.SemanticAnalysis;
using Xunit;


namespace DaedalusCompiler.Tests.SemanticErrors
{
    public class BaseSemanticErrorsTests
    {
        protected string Code;
        protected string ExpectedCompilationOutput;
        
        private void ParseData()
        {
            string[] codeLines = Code.Trim().Split(Environment.NewLine);
            for (int i = 1; i < codeLines.Length; ++i)
            {
                codeLines[i] = codeLines[i].Substring(16);
            }
            Code = string.Join(Environment.NewLine, codeLines);

            string[] compilationOutputLines = ExpectedCompilationOutput.Trim().Split(Environment.NewLine);
            for (int i = 1; i < compilationOutputLines.Length; ++i)
            {
                compilationOutputLines[i] = compilationOutputLines[i].Substring(16);
            }
            ExpectedCompilationOutput = string.Join(Environment.NewLine, compilationOutputLines);
        }
        
        protected void AssertCompilationOutputMatch(bool strictSyntax=false)
        {
            ParseData();
            StringBufforErrorLogger logger = new StringBufforErrorLogger();
            TestsHelper testsHelper = new TestsHelper(logger, strictSyntax);
            testsHelper.RunCode(Code);
            Assert.Equal(ExpectedCompilationOutput, logger.GetBuffor().Trim());
        }

        [Fact]
        public void TestUndeclaredIdentifierInsideFunctionDef()
        {
            Code = @"
                func void testFunc() {
                    x = 5;
                    x = a + b(c, d);
                };
            ";

            ExpectedCompilationOutput = @"
                test.d: In function 'testFunc':
                test.d:2:4: error: 'x' undeclared
                    x = 5;
                    ^
                test.d:3:4: error: 'x' undeclared
                    x = a + b(c, d);
                    ^
                test.d:3:8: error: 'a' undeclared
                    x = a + b(c, d);
                        ^
                test.d:3:12: error: 'b' undeclared
                    x = a + b(c, d);
                            ^
                test.d:3:14: error: 'c' undeclared
                    x = a + b(c, d);
                              ^
                test.d:3:17: error: 'd' undeclared
                    x = a + b(c, d);
                                 ^
                ";

            AssertCompilationOutputMatch();
        }

        [Fact]
        public void TestUndeclaredIdentifierInheritance()
        {
            Code = @"
                class C_NPC { var int data [200]; };
                instance self(C_NPC_MISSPELLED) {};
                prototype OrcElite(Orc) {};
                instance UrukHai(OrcElite);
            ";

            ExpectedCompilationOutput = @"
                test.d: In instance 'self':
                test.d:2:14: error: 'C_NPC_MISSPELLED' undeclared
                instance self(C_NPC_MISSPELLED) {};
                              ^
                test.d: In prototype 'OrcElite':
                test.d:3:19: error: 'Orc' undeclared
                prototype OrcElite(Orc) {};
                                   ^
                ";

            AssertCompilationOutputMatch();
        }
        
        [Fact]
        public void TestUndeclaredIdentifierConstAssignment()
        {
            Code = @"
                const int a = b;
            ";

            ExpectedCompilationOutput = @"
                test.d:1:14: error: 'b' undeclared
                const int a = b;
                              ^
                ";

            AssertCompilationOutputMatch();
        }

        [Fact]
        public void TestAttributeNotFound()
        {
            Code = @"
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

            ExpectedCompilationOutput = @"
                test.d: In function 'testFunc':
                test.d:8:9: error: object 'self' of type 'C_NPC' has no member named 'x'
                    self.x = 5;
                         ^
                test.d:10:4: error: 'other' undeclared
                    other.y = 15;
                    ^
                ";

            AssertCompilationOutputMatch();
        }
        
        
        [Fact]
        public void TestInconsistentConstArraySizeTooManyElements()
        {
            Code = @"
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

            ExpectedCompilationOutput = @"
                test.d:3:23: error: array 'INVENTORY' has inconsistent size (declared size: 2, elements count: 3)
                const string INVENTORY[INVENTORY_SIZE] =
                                       ^
                test.d: In function 'testFunc':
                test.d:11:28: error: array 'LUCKY_NUMBERS' has inconsistent size (declared size: 2, elements count: 3)
                    const int LUCKY_NUMBERS[2] = {2, 4, 8};
                                            ^
                test.d: In global scope:
                test.d:14:19: error: array 'NAMES' has inconsistent size (declared size: 2, elements count: 3)
                const string NAMES[2] = {""DIEGO"", ""ROBERTO"", ""SANCHEZ""};
                                   ^
            ";

            AssertCompilationOutputMatch();
        }
        
        
        [Fact]
        public void TestInconsistentConstArraySizeNotEnoughElements()
        {
            Code = @"
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

            ExpectedCompilationOutput = @"
                test.d:3:23: error: array 'INVENTORY' has inconsistent size (declared size: 4, elements count: 3)
                const string INVENTORY[INVENTORY_SIZE] =
                                       ^
                test.d: In function 'testFunc':
                test.d:11:28: error: array 'LUCKY_NUMBERS' has inconsistent size (declared size: 4, elements count: 3)
                    const int LUCKY_NUMBERS[4] = {2, 4, 8};
                                            ^
                test.d: In global scope:
                test.d:14:19: error: array 'NAMES' has inconsistent size (declared size: 4, elements count: 3)
                const string NAMES[4] = {""DIEGO"", ""ROBERTO"", ""SANCHEZ""};
                                   ^
            ";

            AssertCompilationOutputMatch();
        }
        
        [Fact]
        public void TestInvalidConstArraySizeElements()
        {
            Code = @"
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

            ExpectedCompilationOutput = @"
                test.d: In function 'testFunc':
                test.d:13:28: error: size of array 'LUCKY_NUMBERS' cannot equal zero
                    const int LUCKY_NUMBERS[NUMBERS_SIZE] = {2, 4, 8};
                                            ^
                test.d: In global scope:
                test.d:16:19: error: size of array 'NAMES' cannot equal zero
                const string NAMES[0] = {""DIEGO"", ""ROBERTO"", ""SANCHEZ""};
                                   ^
            ";

            AssertCompilationOutputMatch();
        }

        [Fact]
        public void TestIntValueOutOfMinAndMaxRange()
        {
            Code = @"
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

            ExpectedCompilationOutput = @"
                test.d:3:33: error: integer literal is too large to be represented in an integer type (min: -2147483648, max: 2147483647)
                const int TOO_BIG_INT_POSITIVE = 2147483648;
                                                 ^
                test.d:4:33: error: integer literal is too large to be represented in an integer type (min: -2147483648, max: 2147483647)
                const int TOO_BIG_INT_NEGATIVE = -2147483649;
                                                 ^
                test.d: In function 'testFunc':
                test.d:12:27: error: integer literal is too large to be represented in an integer type (min: -2147483648, max: 2147483647)
                    too_big_int_positive = 2147483648;
                                           ^
                test.d:14:28: error: integer literal is too large to be represented in an integer type (min: -2147483648, max: 2147483647)
                    too_big_int_negative = -2147483648;
                                            ^
            ";

            AssertCompilationOutputMatch();
        }
        
        [Fact]
        public void TestSingleExpressionHack()
        {
            Code = @"
                func void testFunc() {
                    var int x;
                    x = 5;
                    x;
                    return;
                };
            ";

            ExpectedCompilationOutput = @"
                test.d: In function 'testFunc':
                test.d:4:4: warning W1: usage of single-expression statement hack
                    x;
                    ^
            ";

            AssertCompilationOutputMatch();
        }
        
        [Fact]
        public void TestSingleExpressionHackStrictMode()
        {
            Code = @"
                func void testFunc() {
                    var int x;
                    x = 5;
                    x;
                    return;
                };
            ";

            ExpectedCompilationOutput = @"
                test.d: In function 'testFunc':
                test.d:4:4: error W1: usage of single-expression statement hack
                    x;
                    ^
            ";

            AssertCompilationOutputMatch(true);
        }
        
        [Fact]
        public void TestSingleExpressionHackWarningLineSuppress()
        {
            Code = @"
                func void testFunc() {
                    var int x;
                    x = 5;
                    x; //! suppress: W1
                    return;
                };
            ";

            ExpectedCompilationOutput = @"";

            AssertCompilationOutputMatch();
        }

        [Fact]
        public void TestSingleExpressionHackWarningFileSuppress()
        {
            Code = @"
                //! suppress: W1
                
                func void testFunc() {
                    var int x;
                    x = 5;
                    x;
                    return;
                };
            ";

            ExpectedCompilationOutput = @"";

            AssertCompilationOutputMatch();
        }
        
        
        [Fact]
        public void TestArgumentsCountDoesNotMatch()
        {
            Code = @"
                func int testFunc(var int a) {};
                
                func int secondFunc() {
                    testFunc();
                    testFunc(1);
                    testFunc(2, 3);
                };
            ";

            ExpectedCompilationOutput = @"
                test.d: In function 'secondFunc':
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
            Code = @"
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

            ExpectedCompilationOutput = @"
                test.d:2:10: error: cannot initialize a constant of type 'int' with an rvalue of type 'float'
                const int b = 1.5; //error
                          ^   ~~~
                test.d:4:10: error: cannot initialize a constant of type 'int' with an rvalue of type 'float'
                const int d = 2.5 + 3; //error
                          ^   ~~~~~~~
                test.d:5:10: error: cannot initialize a constant of type 'int' with an rvalue of type 'float'
                const int e = 4 + 4.5; //error
                          ^   ~~~~~~~
                test.d:6:10: error: cannot initialize a constant of type 'int' with an rvalue of type 'float'
                const int f = 5.5 + 5.5; //error
                          ^   ~~~~~~~~~
                ";

            AssertCompilationOutputMatch();
        }

        
        
        
        [Fact]
        public void TestNotValidClassOrPrototype()
        {
            Code = @"
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

            ExpectedCompilationOutput = @"
                test.d: In instance 'WRONG1':
                test.d:4:16: error: not a valid class or prototype
                instance WRONG1(myFunc);
                                ^
                test.d: In instance 'WRONG2':
                test.d:5:16: error: not a valid class or prototype
                instance WRONG2(myFunc) {};
                                ^
                test.d: In prototype 'WRONG3':
                test.d:6:17: error: not a valid class or prototype
                prototype WRONG3(myFunc) {};
                                 ^
                ";
            AssertCompilationOutputMatch();
        }
        
        [Fact]
        public void TestTooBigArraySize()
        {
            Code = @"
                var int a[4095];
                var int b[4096];
                
                func void myFunc(var int c[4095], var int d[4096]) {
                    var int e[4095];
                    var int f[4096];
                };
            ";

            ExpectedCompilationOutput = @"
                test.d:2:10: error: too big array size (max: 4095)
                var int b[4096];
                          ^
                test.d: In function 'myFunc':
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
        public void TestTooBigArraySizeConstInt()
        {
            Code = @"
                const int OK_SIZE = 4095;
                const int WRONG_SIZE = 4096;
                
                var int a[OK_SIZE];
                var int b[WRONG_SIZE];
                
                func void myFunc(var int c[OK_SIZE], var int d[WRONG_SIZE]) {
                    var int e[OK_SIZE];
                    var int f[WRONG_SIZE];
                };
            ";

            ExpectedCompilationOutput = @"
                test.d:5:10: error: too big array size (max: 4095)
                var int b[WRONG_SIZE];
                          ^
                test.d: In function 'myFunc':
                test.d:7:47: error: too big array size (max: 4095)
                func void myFunc(var int c[OK_SIZE], var int d[WRONG_SIZE]) {
                                                               ^
                test.d:9:14: error: too big array size (max: 4095)
                    var int f[WRONG_SIZE];
                              ^
            ";

            AssertCompilationOutputMatch();
        }
        
        [Fact]
        public void TestArrayIndexOutOfRange()
        {
            Code = @"
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

            ExpectedCompilationOutput = @"
                test.d: In function 'myFunc':
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
        public void TestAttributeArrayIndexOutOfRange()
        {
            Code = @"
                const int GOOD_INDEX = 7;
                const int BAD_INDEX = 8;
                
                class C_NPC
                {
                    var int attribute[8];
                };
                
                instance self(C_NPC) {};
                
                func void myFunc()
                {
                    self.attribute[GOOD_INDEX] = 100;
                    self.attribute[BAD_INDEX] = 200;
                };
            ";

            ExpectedCompilationOutput = @"
                test.d: In function 'myFunc':
                test.d:14:19: error: array index out of range (max index for this array is 7)
                    self.attribute[BAD_INDEX] = 200;
                                   ^
            ";

            AssertCompilationOutputMatch();
        }
        
        [Fact]
        public void TestArrayIndexOutOfRangeConstInt()
        {
            Code = @"
                const int ONE = 1;
                const int TWO = 2;
                const int THREE = 3;
                const int FOUR = 4;
                
                const int a[2] = {2 , 3};
                
                func void myFunc(var int b[3]) {
                    var int c[4];
                    var int x;
                    x = a[ONE];
                    x = a[TWO];
                    x = b[TWO];
                    x = b[THREE];
                    x = c[THREE];
                    x = c[FOUR];
                };
            ";

            ExpectedCompilationOutput = @"
                test.d: In function 'myFunc':
                test.d:12:10: error: array index out of range (max index for this array is 1)
                    x = a[TWO];
                          ^
                test.d:14:10: error: array index out of range (max index for this array is 2)
                    x = b[THREE];
                          ^
                test.d:16:10: error: array index out of range (max index for this array is 3)
                    x = c[FOUR];
                          ^
            ";

            AssertCompilationOutputMatch();
        }
        
        [Fact]
        public void TestTooBigArrayIndex()
        {
            Code = @"
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

            ExpectedCompilationOutput = @"
                test.d: In function 'myFunc':
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
        
        
        [Fact]
        public void TestTooBigArrayIndexConstIndex()
        {
            Code = @"
                const int INDEX1 = 255;
                const int INDEX2 = 256;
                
                const int a[2] = {2 , 3};
                var int b[4095];
                
                func void myFunc() {
                    var int x;
                    x = a[INDEX1];
                    x = a[INDEX2];
                    x = b[INDEX1];
                    x = b[INDEX2];
                };
            ";

            ExpectedCompilationOutput = @"
                test.d: In function 'myFunc':
                test.d:9:10: error: array index out of range (max index for this array is 1)
                    x = a[INDEX1];
                          ^
                test.d:10:10: error: array index out of range (max index for this array is 1)
                    x = a[INDEX2];
                          ^
                test.d:12:10: error: too big array index (max: 255)
                    x = b[INDEX2];
                          ^
            ";

            AssertCompilationOutputMatch();
        }
        
        
        [Fact]
        public void TestIterationStatementNotInLoop()
        {
            Code = @"
                const int break = 1;
                
                func void testFunc() {
                    break;
                    continue;
                };
            ";

            ExpectedCompilationOutput = @"
                test.d:1:10: error: 'break' is keyword and shouldn't be used as an identifier
                const int break = 1;
                          ^
                test.d: In function 'testFunc':
                test.d:4:4: error: 'break' statement not allowed outside of loop statement
                    break;
                    ^
                test.d:5:4: error: 'continue' statement not allowed outside of loop statement
                    continue;
                    ^
            ";

            AssertCompilationOutputMatch();
        }
    }
}