using Xunit;

namespace DaedalusCompiler.Tests.SemanticErrors
{
    public class SyntaxErrorsTests : BaseSemanticErrorsTests
    {
        [Fact]
        public void TestSimpleSyntaxError()
        {
            Code = @"
                super
                const int INVENTORY_SIZE = 2; partia
            ";

            ExpectedCompilationOutput = @"
                test.d
                test.d:1:0: extraneous input 'super' expecting {<EOF>, Const, Var, Func, Class, Prototype, Instance, 'extern'}
                super
                ^
                test.d:2:30: extraneous input 'partia' expecting {<EOF>, Const, Var, Func, Class, Prototype, Instance, 'extern'}
                const int INVENTORY_SIZE = 2; partia
                                              ^
                2 syntax errors generated.
            ";

            AssertCompilationOutputMatch();
        }
        
        [Fact]
        public void TestUnexpectedChatacterSyntaxError()
        {
            Code = @"
                :
            ";

            ExpectedCompilationOutput = @"
                test.d
                test.d:1:0: extraneous input ':' expecting {<EOF>, Const, Var, Func, Class, Prototype, Instance, 'extern'}
                :
                ^
                1 syntax error generated.
            ";

            AssertCompilationOutputMatch();
        }
        
    }
}