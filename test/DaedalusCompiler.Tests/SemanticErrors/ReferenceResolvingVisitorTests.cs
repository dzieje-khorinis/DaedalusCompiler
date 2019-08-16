using Xunit;

namespace DaedalusCompiler.Tests.SemanticErrors
{
    public class ReferenceResolvingVisitorTests : BaseSemanticErrorsTests
    {
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
    }
}