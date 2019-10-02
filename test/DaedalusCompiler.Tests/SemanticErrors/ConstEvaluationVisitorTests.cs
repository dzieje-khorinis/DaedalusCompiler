using Xunit;

namespace DaedalusCompiler.Tests.SemanticErrors
{
    public class ConstEvaluationVisitorTests : BaseSemanticErrorsTests
    {
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
        public void TestArraySizeEqualsZero()
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
                
                func void testFunc(var int a[NUMBERS_SIZE]) {
                    var int b[NUMBERS_SIZE];
                    const int LUCKY_NUMBERS[NUMBERS_SIZE] = {2, 4, 8};
                };
                
                const string NAMES[0] = {""DIEGO"", ""ROBERTO"", ""SANCHEZ""};
            ";

            ExpectedCompilationOutput = @"
                test.d: In function 'testFunc':
                test.d:12:29: error: size of array 'a' cannot equal zero
                func void testFunc(var int a[NUMBERS_SIZE]) {
                                             ^
                test.d:13:14: error: size of array 'b' cannot equal zero
                    var int b[NUMBERS_SIZE];
                              ^
                test.d:14:28: error: size of array 'LUCKY_NUMBERS' cannot equal zero
                    const int LUCKY_NUMBERS[NUMBERS_SIZE] = {2, 4, 8};
                                            ^
                test.d: In global scope:
                test.d:17:19: error: size of array 'NAMES' cannot equal zero
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
                    x = c[3]; //! suppress: W5
                    x = c[4]; //! suppress: W5
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
                    x = c[4]; //! suppress: W5
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
                
                class NPC
                {
                    var int attribute[8];
                };
                
                instance self(NPC) {};
                
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
                    x = c[THREE]; //! suppress: W5
                    x = c[FOUR]; //! suppress: W5
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
                    x = c[FOUR]; //! suppress: W5
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
        public void TestInfiniteConstReferenceLoop()
        {
            Code = @"
                const int a = b;
                const int b = a;
                
                const int c[2] = {2, d};
                const int d = c[1];
                
                const int e[f] = {1, 2};
                const int f = e[1];
                
                var int g[2];
                const int h[g[3]] = {2, g[1]};
                
                var int i;
                const int j[i] = {i, 3};
                
                const int k[l[1]] = {1, 2};
                const int l[k[1]] = {3, 4};
            ";

            ExpectedCompilationOutput = @"
                test.d:1:14: error: circular const reference dependency detected
                const int a = b;
                              ^
                test.d:5:14: error: circular const reference dependency detected
                const int d = c[1];
                              ^
                test.d:8:14: error: circular const reference dependency detected
                const int f = e[1];
                              ^
                test.d:11:12: error: const reference required
                const int h[g[3]] = {2, g[1]};
                            ^
                test.d:11:24: error: const reference required
                const int h[g[3]] = {2, g[1]};
                                        ^
                test.d:14:12: error: const reference required
                const int j[i] = {i, 3};
                            ^
                test.d:14:18: error: const reference required
                const int j[i] = {i, 3};
                                  ^
                test.d:16:12: error: circular const reference dependency detected
                const int k[l[1]] = {1, 2};
                            ^
            ";

            AssertCompilationOutputMatch();
        }
        
        [Fact]
        public void TestArraySizeNotConstInteger()
        {
            Code = @"
                const string name = ""TEST"";
                const float height = 2.55;

                const int a[name] = {1, 2};
                var int b[name];

                func void test(var int c[name], var int d[height]) {
                    const int e[height] = {3, 4, 5};
                    var int f[height];
                }
            ";

            ExpectedCompilationOutput = @"
                test.d:4:12: error: array size must be of const integer type
                const int a[name] = {1, 2};
                            ^
                test.d:5:10: error: array size must be of const integer type
                var int b[name];
                          ^
                test.d: In function 'test':
                test.d:7:25: error: array size must be of const integer type
                func void test(var int c[name], var int d[height]) {
                                         ^
                test.d:7:42: error: array size must be of const integer type
                func void test(var int c[name], var int d[height]) {
                                                          ^
                test.d:8:16: error: array size must be of const integer type
                    const int e[height] = {3, 4, 5};
                                ^
                test.d:9:14: error: array size must be of const integer type
                    var int f[height];
                              ^
            ";

            AssertCompilationOutputMatch();
        }

        [Fact]
        public void TestArrayIndexNotConstInteger()
        {
            Code = @"
                const int INDEX1 = 0;
                const float INDEX2 = 1.1;
                const string INDEX3 = ""two"";

                const int a[2] = {1, 2};
                var int b[2];

                func void test(var int c[2]) {
                    const int d[2] = {3, 4};
                    var int e[2];

                    var int x;
                    x = a[INDEX1];
                    x = a[INDEX2];
                    x = a[INDEX3];

                    x = b[INDEX1];
                    x = b[INDEX2];
                    x = b[INDEX3];

                    x = c[INDEX1];
                    x = c[INDEX2];
                    x = c[INDEX3];

                    x = d[INDEX1];
                    x = d[INDEX2];
                    x = d[INDEX3];

                    x = e[INDEX1];  //! suppress: W5
                    x = e[INDEX2];
                    x = e[INDEX3];
                }
            ";

            ExpectedCompilationOutput = @"
                test.d: In function 'test':
                test.d:14:10: error: array index must be of const integer type
                    x = a[INDEX2];
                          ^
                test.d:15:10: error: array index must be of const integer type
                    x = a[INDEX3];
                          ^
                test.d:18:10: error: array index must be of const integer type
                    x = b[INDEX2];
                          ^
                test.d:19:10: error: array index must be of const integer type
                    x = b[INDEX3];
                          ^
                test.d:22:10: error: array index must be of const integer type
                    x = c[INDEX2];
                          ^
                test.d:23:10: error: array index must be of const integer type
                    x = c[INDEX3];
                          ^
                test.d:26:10: error: array index must be of const integer type
                    x = d[INDEX2];
                          ^
                test.d:27:10: error: array index must be of const integer type
                    x = d[INDEX3];
                          ^
                test.d:30:10: error: array index must be of const integer type
                    x = e[INDEX2];
                          ^
                test.d:31:10: error: array index must be of const integer type
                    x = e[INDEX3];
                          ^
            ";

            AssertCompilationOutputMatch();
        }
        
        [Fact]
        public void TestNotConstReference()
        {
            Code = @"
                const int a = 1;
                var int b;
                const int c = a + b;

                func void testFunc() {
                    const int d = 2;
                    var int e;
                    const int f = e + d;
                }
            ";

            ExpectedCompilationOutput = @"
                test.d:3:18: error: const reference required
                const int c = a + b;
                                  ^
                test.d: In function 'testFunc':
                test.d:8:18: error: const reference required
                    const int f = e + d;
                                  ^
            ";

            AssertCompilationOutputMatch();
        }
        
        [Fact]
        public void TestArithmeticOperationOverflow()
        {
            Code = @"
                const int a = 2147483648;
                const int b = --2147483648;
                const int c = 2147483647 + 1;
                const int d = 2147483647 * 2147483647;
            ";

            ExpectedCompilationOutput = @"
                test.d:1:14: error: integer literal is too large to be represented in an integer type (min: -2147483648, max: 2147483647)
                const int a = 2147483648;
                              ^
                test.d:2:14: error: arithmetic operation resulted in an overflow
                const int b = --2147483648;
                              ^
                test.d:3:25: error: arithmetic operation resulted in an overflow
                const int c = 2147483647 + 1;
                                         ^
                test.d:4:25: error: arithmetic operation resulted in an overflow
                const int d = 2147483647 * 2147483647;
                                         ^
            ";

            AssertCompilationOutputMatch();
        }
        
        [Fact]
        public void TestDivideByZero()
        {
            Code = @"
                const int a = 5 / 0;
                const int b = 5 % 0;
                const int c = 5.0 / 0;
                const int d = 5.0 / 0.0;
            ";

            ExpectedCompilationOutput = @"
                test.d:1:16: error: cannot divide by zero
                const int a = 5 / 0;
                                ^ ~
                test.d:2:16: error: cannot divide by zero
                const int b = 5 % 0;
                                ^ ~
                test.d:3:18: error: cannot divide by zero
                const int c = 5.0 / 0;
                                  ^ ~
                test.d:4:18: error: cannot divide by zero
                const int d = 5.0 / 0.0;
                                  ^ ~~~
            ";

            AssertCompilationOutputMatch();
        }
        
        [Fact]
        public void TestInvalidUnaryOperation()
        {
            Code = @"
                const int a = -""name"";
                const int b = ~2.5;
            ";

            ExpectedCompilationOutput = @"
                test.d:1:14: error: invalid unary operation
                const int a = -""name"";
                              ^
                test.d:2:14: error: invalid unary operation
                const int b = ~2.5;
                              ^
            ";

            AssertCompilationOutputMatch();
        }
        
        [Fact]
        public void TestInvalidBinaryOperation()
        {
            Code = @"
                const string a = 2 * ""name"";
                const string b = 2.5 * ""name"";
                const string c = ""name"" * 2.5;
                const string d = ""name"" + ""na"";
                const string e = ""name"" - ""na"";
            ";

            ExpectedCompilationOutput = @"
                test.d:2:21: error: invalid binary operation
                const string b = 2.5 * ""name"";
                                 ~~~ ^ ~~~~~~
                test.d:3:24: error: invalid binary operation
                const string c = ""name"" * 2.5;
                                 ~~~~~~ ^ ~~~
                test.d:5:24: error: invalid binary operation
                const string e = ""name"" - ""na"";
                                 ~~~~~~ ^ ~~~~
                ";

            AssertCompilationOutputMatch();
        }
        
        [Fact]
        public void TestIfCannotInitializeConstWithValueOfDifferentType()
        {
            Code = @"
                const int a = 2.5;
                const float b = ""two"";
                const string c = 2;
            ";

            ExpectedCompilationOutput = @"
                test.d:1:10: error: cannot initialize a constant of type 'int' with an rvalue of type 'float'
                const int a = 2.5;
                          ^   ~~~
                test.d:2:12: error: cannot initialize a constant of type 'float' with an rvalue of type 'string'
                const float b = ""two"";
                            ^   ~~~~~
                test.d:3:13: error: cannot initialize a constant of type 'string' with an rvalue of type 'int'
                const string c = 2;
                             ^   ~
            ";

            AssertCompilationOutputMatch();
        }
        
        [Fact]
        public void TestIfCannotInitializeArrayElementWithValueOfDifferentType()
        {
            Code = @"
                const int a[3] = {0, ""zero"", 0.5};
                const float b[3] = {1, ""one"", 1.5};
                const string c[3] = {2, ""two"", 2.5};
            ";

            ExpectedCompilationOutput = @"
                test.d:1:21: error: cannot initialize an array element of type 'int' with an rvalue of type 'string'
                const int a[3] = {0, ""zero"", 0.5};
                                     ^~~~~~
                test.d:1:29: error: cannot initialize an array element of type 'int' with an rvalue of type 'float'
                const int a[3] = {0, ""zero"", 0.5};
                                             ^~~
                test.d:2:6: error: unsupported array type
                const float b[3] = {1, ""one"", 1.5};
                      ^
                test.d:3:21: error: cannot initialize an array element of type 'string' with an rvalue of type 'int'
                const string c[3] = {2, ""two"", 2.5};
                                     ^
                test.d:3:31: error: cannot initialize an array element of type 'string' with an rvalue of type 'float'
                const string c[3] = {2, ""two"", 2.5};
                                               ^~~
            ";

            AssertCompilationOutputMatch();
        }
        
        public void TEMPLATE()
        {
            Code = @"

            ";

            ExpectedCompilationOutput = @"

            ";

            AssertCompilationOutputMatch();
        }

        
    }
}