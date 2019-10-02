using Xunit;

namespace DaedalusCompiler.Tests.SemanticErrors
{
    public class UninitializedSymbolUsageDetectionVisitorTests : BaseSemanticErrorsTests
    {
        [Fact]
        public void TestUsageOfNonInitializedNestedAttributes()
        {
            Code = @"
                prototype A(NPC) {
                    CAT.x += 1;
                    CAT.x = 2;
                    CAT.x -= 3;

                    dog.x += 4;
                    dog.x = 5;
                    dog.x -= 6;

                    dog = CAT;
                    dog.x *= 7;
                };

                class PET {
                    var int x;
                };

                class NPC {
                    var PET dog;
                };

                instance CAT(PET) {};
            ";

            ExpectedCompilationOutput = @"
                test.d: In prototype 'A':
                test.d:6:4: warning W5: usage of non-initialized attribute 'dog'
                    dog.x += 4;
                    ^
                test.d:7:4: warning W5: usage of non-initialized attribute 'dog'
                    dog.x = 5;
                    ^
                test.d:8:4: warning W5: usage of non-initialized attribute 'dog'
                    dog.x -= 6;
                    ^
            ";

            AssertCompilationOutputMatch();
        }
        
        
        
        [Fact]
        public void TestUsageOfNonInitializedAttributes()
        {
            Code = @"
                class NPC {
                    var int a;
                    var int b;
                    var int c;
                };

                prototype X(NPC) {
                    a = 1;

                    a += 1;
                    b *= 2;
                    c /= 3;
                };

                instance Z(Y) {
                    c = 3;

                    a += 1;
                    b *= 2;
                    c /= 3;
                };

                prototype Y(X) {
                    b = 2;

                    a += 1;
                    b *= 2;
                    c /= 3;
                };
            ";

            ExpectedCompilationOutput = @"
                test.d: In prototype 'X':
                test.d:11:4: warning W5: usage of non-initialized attribute 'b'
                    b *= 2;
                    ^
                test.d:12:4: warning W5: usage of non-initialized attribute 'c'
                    c /= 3;
                    ^
                test.d: In prototype 'Y':
                test.d:28:4: warning W5: usage of non-initialized attribute 'c'
                    c /= 3;
                    ^
            ";

            AssertCompilationOutputMatch();
        }
        
        
        [Fact]
        public void TestUsageOfNonInitializedArrayAttributes()
        {
            Code = @"
                const int ZERO = 0;
                const int ONE = 1;
                const int TWO = 2;

                class NPC {
                    var int tab[3];
                };

                prototype X(NPC) {
                    tab[0] = 1;

                    tab[ZERO] += 1;
                    tab[ONE] *= 2;
                    tab[TWO] /= 3;
                };

                instance Z(Y) {
                    tab[TWO] = 3;

                    tab[0] += 1;
                    tab[1] *= 2;
                    tab[2] /= 3;
                };

                prototype Y(X) {
                    tab[ONE] = 2;

                    tab[0] += 1;
                    tab[1] *= 2;
                    tab[2] /= 3;
                };
            ";

            ExpectedCompilationOutput = @"
                test.d: In prototype 'X':
                test.d:13:4: warning W5: usage of non-initialized element (index '1') of array attribute 'tab'
                    tab[ONE] *= 2;
                    ^
                test.d:14:4: warning W5: usage of non-initialized element (index '2') of array attribute 'tab'
                    tab[TWO] /= 3;
                    ^
                test.d: In prototype 'Y':
                test.d:30:4: warning W5: usage of non-initialized element (index '2') of array attribute 'tab'
                    tab[2] /= 3;
                    ^
            ";

            AssertCompilationOutputMatch();
        }
        
        
        
        
        [Fact]
        public void TestUsageOfNonInitializedLocalVariables()
        {
            Code = @"
                class NPC {
                    var int a;
                    var int b;
                    var int c;
                };

                prototype ORC(NPC) {
                    var int a;
                    var int b;
                    b = a + b + c;
                    a = 1;
                    b += 2;
                    c -= 3;
                    c = a + b + c;
                };

                instance HOSHPAK(ORC) {
                    var int a;
                    var int b;
                    b = a + b + c;
                    a = 1;
                    b += 2;
                    c -= 3;
                    c = a + b + c;
                };

                func void testFunc(var int d) {
                    var int a;
                    var int b;
                    b = a + b + c + d;
                    a = 1;
                    b += 2;
                    c -= 3;
                    c = a + b + c + d;
                };
            ";

            ExpectedCompilationOutput = @"
                test.d: In prototype 'ORC':
                test.d:10:8: warning W5: usage of non-initialized variable 'a'
                    b = a + b + c;
                        ^
                test.d:10:12: warning W5: usage of non-initialized variable 'b'
                    b = a + b + c;
                            ^
                test.d:10:16: warning W5: usage of non-initialized attribute 'c'
                    b = a + b + c;
                                ^
                test.d:13:4: warning W5: usage of non-initialized attribute 'c'
                    c -= 3;
                    ^
                test.d:14:16: warning W5: usage of non-initialized attribute 'c'
                    c = a + b + c;
                                ^
                test.d: In instance 'HOSHPAK':
                test.d:20:8: warning W5: usage of non-initialized variable 'a'
                    b = a + b + c;
                        ^
                test.d:20:12: warning W5: usage of non-initialized variable 'b'
                    b = a + b + c;
                            ^
                test.d: In function 'testFunc':
                test.d:30:8: warning W5: usage of non-initialized variable 'a'
                    b = a + b + c + d;
                        ^
                test.d:30:12: warning W5: usage of non-initialized variable 'b'
                    b = a + b + c + d;
                            ^
                test.d:30:16: error: 'c' undeclared
                    b = a + b + c + d;
                                ^
                test.d:33:4: error: 'c' undeclared
                    c -= 3;
                    ^
                test.d:34:4: error: 'c' undeclared
                    c = a + b + c + d;
                    ^
                test.d:34:16: error: 'c' undeclared
                    c = a + b + c + d;
                                ^
            ";

            AssertCompilationOutputMatch();
        }
        
        
        
        [Fact]
        public void TestUsageOfNonInitializedLocalArrayVariables()
        {
            Code = @"
                class NPC {
                    var int attr[3];
                };

                prototype ORC(NPC) {
                    var int tab[2];
                    tab[1] = tab[0] + tab[1] + attr[2];
                    tab[0] = 1;
                    tab[1] += 2;
                    attr[2] -= 3;
                    attr[2] = tab[0] + tab[1] + attr[2];
                };

                instance HOSHPAK(ORC) {
                    var int tab[2];
                    tab[1] = tab[0] + tab[1] + attr[2];
                    tab[0] = 1;
                    tab[1] += 2;
                    attr[2] -= 3;
                    attr[2] = tab[0] + tab[1] + attr[2];
                };

                func void testFunc(var int d[2]) {
                    var int tab[2];
                    tab[1] = tab[0] + tab[1] + attr[2] + d[1];
                    tab[0] = 1;
                    tab[1] += 2;
                    attr[2] -= 3;
                    attr[2] = tab[0] + tab[1] + attr[2] + d[0];
                };
            ";

            ExpectedCompilationOutput = @"
                test.d: In prototype 'ORC':
                test.d:7:13: warning W5: usage of non-initialized element (index '0') of array variable 'tab'
                    tab[1] = tab[0] + tab[1] + attr[2];
                             ^
                test.d:7:22: warning W5: usage of non-initialized element (index '1') of array variable 'tab'
                    tab[1] = tab[0] + tab[1] + attr[2];
                                      ^
                test.d:7:31: warning W5: usage of non-initialized element (index '2') of array attribute 'attr'
                    tab[1] = tab[0] + tab[1] + attr[2];
                                               ^
                test.d:10:4: warning W5: usage of non-initialized element (index '2') of array attribute 'attr'
                    attr[2] -= 3;
                    ^
                test.d:11:32: warning W5: usage of non-initialized element (index '2') of array attribute 'attr'
                    attr[2] = tab[0] + tab[1] + attr[2];
                                                ^
                test.d: In instance 'HOSHPAK':
                test.d:16:13: warning W5: usage of non-initialized element (index '0') of array variable 'tab'
                    tab[1] = tab[0] + tab[1] + attr[2];
                             ^
                test.d:16:22: warning W5: usage of non-initialized element (index '1') of array variable 'tab'
                    tab[1] = tab[0] + tab[1] + attr[2];
                                      ^
                test.d: In function 'testFunc':
                test.d:25:13: warning W5: usage of non-initialized element (index '0') of array variable 'tab'
                    tab[1] = tab[0] + tab[1] + attr[2] + d[1];
                             ^
                test.d:25:22: warning W5: usage of non-initialized element (index '1') of array variable 'tab'
                    tab[1] = tab[0] + tab[1] + attr[2] + d[1];
                                      ^
                test.d:25:31: error: 'attr' undeclared
                    tab[1] = tab[0] + tab[1] + attr[2] + d[1];
                                               ^
                test.d:28:4: error: 'attr' undeclared
                    attr[2] -= 3;
                    ^
                test.d:29:4: error: 'attr' undeclared
                    attr[2] = tab[0] + tab[1] + attr[2] + d[0];
                    ^
                test.d:29:32: error: 'attr' undeclared
                    attr[2] = tab[0] + tab[1] + attr[2] + d[0];
                                                ^
            ";

            AssertCompilationOutputMatch();
        }

    }
}