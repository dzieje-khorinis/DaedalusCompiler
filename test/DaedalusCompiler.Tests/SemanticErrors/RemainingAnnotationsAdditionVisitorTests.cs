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
    }
}