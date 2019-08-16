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
    }
}