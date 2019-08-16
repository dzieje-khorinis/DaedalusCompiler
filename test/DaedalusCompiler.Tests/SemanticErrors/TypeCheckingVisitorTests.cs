using Xunit;

namespace DaedalusCompiler.Tests.SemanticErrors
{
    public class TypeCheckingVisitorTests : BaseSemanticErrorsTests
    {
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
    }
}