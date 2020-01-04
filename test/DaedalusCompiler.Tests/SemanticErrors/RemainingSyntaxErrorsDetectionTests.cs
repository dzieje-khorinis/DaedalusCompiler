using Xunit;

namespace DaedalusCompiler.Tests.SemanticErrors
{
    public class RemainingSyntaxErrorsDetectionTests : BaseSemanticErrorsTests
    {
        [Fact]
        public void TestVarDeclAssignment()
        {
            Code = @"
                var int a1 = 6;
                var int c1[2] = {9};
                var int d1[2] = {8, 9};
                var int e1[2] = {7, 8, 9};
                
                class NPC {
                    var int a2 = 6;
                    var int c2[2] = {9};
                    var int d2[2] = {8, 9};
                    var int e2[2] = {7, 8, 9};
                }
                
                prototype Proto(NPC) {
                    var int a3 = 6;
                    var int c3[2] = {9};
                    var int d3[2] = {8, 9};
                    var int e3[2] = {7, 8, 9};
                }
                
                instance Inst(NPC) {
                    var int a4 = 6;
                    var int c4[2] = {9};
                    var int d4[2] = {8, 9};
                    var int e4[2] = {7, 8, 9};
                }
                
                func void testFunc() {
                    var int a5 = 6;
                    var int c5[2] = {9};
                    var int d5[2] = {8, 9};
                    var int e5[2] = {7, 8, 9};
                }
            ";

            ExpectedCompilationOutput = @"
                test.d:1:8: error: variable assignment allowed only in functions, prototypes and instances
                var int a1 = 6;
                        ^
                test.d:2:8: error: variable assignment allowed only in functions, prototypes and instances
                var int c1[2] = {9};
                        ^
                test.d:3:8: error: variable assignment allowed only in functions, prototypes and instances
                var int d1[2] = {8, 9};
                        ^
                test.d:4:8: error: variable assignment allowed only in functions, prototypes and instances
                var int e1[2] = {7, 8, 9};
                        ^
                test.d: In class 'NPC':
                test.d:7:12: error: variable assignment allowed only in functions, prototypes and instances
                    var int a2 = 6;
                            ^
                test.d:8:12: error: variable assignment allowed only in functions, prototypes and instances
                    var int c2[2] = {9};
                            ^
                test.d:9:12: error: variable assignment allowed only in functions, prototypes and instances
                    var int d2[2] = {8, 9};
                            ^
                test.d:10:12: error: variable assignment allowed only in functions, prototypes and instances
                    var int e2[2] = {7, 8, 9};
                            ^
                test.d: In prototype 'Proto':
                test.d:17:15: error: array 'e3' has inconsistent size (declared size: 2, elements count: 3)
                    var int e3[2] = {7, 8, 9};
                               ^
                test.d: In instance 'Inst':
                test.d:24:15: error: array 'e4' has inconsistent size (declared size: 2, elements count: 3)
                    var int e4[2] = {7, 8, 9};
                               ^
                test.d: In function 'testFunc':
                test.d:31:15: error: array 'e5' has inconsistent size (declared size: 2, elements count: 3)
                    var int e5[2] = {7, 8, 9};
                               ^
                11 errors generated.
            ";

            AssertCompilationOutputMatch();
        }

    }
}