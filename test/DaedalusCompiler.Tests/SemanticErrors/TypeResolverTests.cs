using Xunit;

namespace DaedalusCompiler.Tests.SemanticErrors
{
    public class TypeResolverTests : BaseSemanticErrorsTests
    {
        [Fact]
        public void TestUnknownTypeName()
        {
            Code = @"
                const b a = 1;
                const a b[2] = {1, 2};
                var d c;
                var c d[3];
                
                func x myFunc() {
                    const c a = 1;
                    const d b[4] = {2, 3, 4, 5};
                    var a c;
                    var b d[5];
                };
            ";

            ExpectedCompilationOutput = @"
                test.d:1:6: error: unknown type name 'b'
                const b a = 1;
                      ^
                test.d:2:6: error: unknown type name 'a'
                const a b[2] = {1, 2};
                      ^
                test.d:3:4: error: unknown type name 'd'
                var d c;
                    ^
                test.d:4:4: error: unknown type name 'c'
                var c d[3];
                    ^
                test.d: In function 'myFunc':
                test.d:6:5: error: unknown type name 'x'
                func x myFunc() {
                     ^
                test.d:7:10: error: unknown type name 'c'
                    const c a = 1;
                          ^
                test.d:8:10: error: unknown type name 'd'
                    const d b[4] = {2, 3, 4, 5};
                          ^
                test.d:9:8: error: unknown type name 'a'
                    var a c;
                        ^
                test.d:10:8: error: unknown type name 'b'
                    var b d[5];
                        ^
            ";

            AssertCompilationOutputMatch();
        }
    }
}