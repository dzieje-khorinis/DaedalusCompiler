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
    }
}

/*

// InfiniteConstReferenceLoopError :)
// ArraySizeEqualsZeroError
// TooBigArraySizeError
// UnsupportedTypeError
// InconsistentSizeError
// IndexOutOfRangeError
// TooBigArrayIndex
// ConstIntegerExpectedError
// NotConstReferenceError
// ArithmeticOperationOverflowError
// InvalidUnaryOperationError
// InvalidBinaryOperationError
// IntegerLiteralTooLargeError
// CannotInitializeConstWithValueOfDifferentType
// CannotInitializeArrayElementWithValueOfDifferentType

*/