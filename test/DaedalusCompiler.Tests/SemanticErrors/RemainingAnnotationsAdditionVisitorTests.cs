using Xunit;

namespace DaedalusCompiler.Tests.SemanticErrors
{
    public class RemainingAnnotationsAdditionVisitorTests : BaseSemanticErrorsTests
    {
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

        [Fact]
        public void TestIntegerLiteralTooLarge()
        {
            Code = @"
                func void testFunc() {
                    var int x;
                    x = 2147483648;
                    x = 214748364821474836482147483648214748364821474836482147483648;
                };
            ";
            
            ExpectedCompilationOutput = @"
                test.d: In function 'testFunc':
                test.d:3:8: error: integer literal is too large to be represented in an integer type (min: -2147483648, max: 2147483647)
                    x = 2147483648;
                        ^
                test.d:4:8: error: integer literal is too large to be represented in an integer type (min: -2147483648, max: 2147483647)
                    x = 214748364821474836482147483648214748364821474836482147483648;
                        ^
            ";

            AssertCompilationOutputMatch();
        }

        [Fact]
        public void TestWrongClassSize()
        {
            Code = @"
                class C_NPC { };
                class C_INFO { };
                class C_ITEMREACT { };
            ";
            
            ExpectedCompilationOutput = @"
                test.d:1:6: error: size of class 'C_NPC' must be 800 bytes (currently it's 0 bytes)
                class C_NPC { };
                      ^
                test.d:2:6: error: size of class 'C_INFO' must be 48 bytes (currently it's 0 bytes)
                class C_INFO { };
                      ^
                test.d:3:6: error: size of class 'C_ITEMREACT' must be 28 bytes (currently it's 0 bytes)
                class C_ITEMREACT { };
                      ^
            ";

            AssertCompilationOutputMatch();
        }
    }
}