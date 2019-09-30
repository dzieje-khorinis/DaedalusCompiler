using Xunit;

namespace DaedalusCompiler.Tests.SemanticErrors
{
    public class ReferenceResolvingVisitorTests : BaseSemanticErrorsTests
    {
        [Fact]
        public void TestAccessToAttributeOfArrayElement()
        {
            Code = @"
                func void myFunc() {
                    const int x[2] = {1, 2};
                    const int y = x[0].attr;
                    const int z = y[1].attr;
                    const int q = xxx[2].attr;
                    
                    var int a[2];
                    var int b;
                    b = a[0].attr;
                    b = b[1].attr;
                    b = xxx[2].attr;
                }
            ";

            ExpectedCompilationOutput = @"
                test.d: In function 'myFunc':
                test.d:3:23: error: access to attribute of array element not supported
                    const int y = x[0].attr;
                                       ^
                test.d:4:18: error: cannot access array element because 'y' is not an array
                    const int z = y[1].attr;
                                  ^
                test.d:5:18: error: 'xxx' undeclared
                    const int q = xxx[2].attr;
                                  ^
                test.d:9:13: error: access to attribute of array element not supported
                    b = a[0].attr;
                             ^
                test.d:10:8: error: cannot access array element because 'b' is not an array
                    b = b[1].attr;
                        ^
                test.d:11:8: error: 'xxx' undeclared
                    b = xxx[2].attr;
                        ^
                ";

            AssertCompilationOutputMatch();
        }
        
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
                var C_NPC victim;
                
                func void testFunc() {
                    self.x = 5;
                    self.y = 10;
                    other.y = 15;
                    victim.x = 20;
                    victim.y = 25;
                };
            ";

            ExpectedCompilationOutput = @"
                test.d: In function 'testFunc':
                test.d:9:9: error: object 'self' of type 'C_NPC' has no member named 'x'
                    self.x = 5;
                         ^
                test.d:11:4: error: 'other' undeclared
                    other.y = 15;
                    ^
                test.d:12:11: error: object 'victim' of type 'C_NPC' has no member named 'x'
                    victim.x = 20;
                           ^
                ";

            AssertCompilationOutputMatch();
        }
        
        
        [Fact]
        public void TestAttributeOfNonInstance()
        {
            Code = @"
                prototype Proto(X) {};
                
                func void testFunc() {
                    var int x;
                    var int y;
                    x = y.attr;
                    y.attr = x;
                    
                    x = Proto.a;
                    x = Proto;
                    
                    x = NonExistant.a;
                };
            ";

            ExpectedCompilationOutput = @"
                test.d: In prototype 'Proto':
                test.d:1:16: error: 'X' undeclared
                prototype Proto(X) {};
                                ^
                test.d: In function 'testFunc':
                test.d:6:10: error: cannot access attribute 'attr' because 'y' is not an instance of a class
                    x = y.attr;
                          ^
                test.d:7:6: error: cannot access attribute 'attr' because 'y' is not an instance of a class
                    y.attr = x;
                      ^
                test.d:9:14: error: cannot access attribute 'a' because 'Proto' is not an instance of a class
                    x = Proto.a;
                              ^
                test.d:12:8: error: 'NonExistant' undeclared
                    x = NonExistant.a;
                        ^
                ";

            AssertCompilationOutputMatch();
        }


        [Fact]
        public void TestNamesNotMatchingCaseWise()
        {
            Code = @"
                class NPC {
                    var int str;
                }
                instance HERO(NPC);
                var NPC enemy;

                const int x = 2;
                const int y = X + 1;

                func void test() {};

                func void testFunc() {
                    tesT();
                    var int a;
                    var int b;
                    A = B + X;
                    b = hero.str + HERO.STR;
                    b = enemy.STR + ENEMY.str;
                };
            ";

            ExpectedCompilationOutput = @"
                test.d:8:14: warning W2: name 'X' doesn't match declared name 'x' case wise
                const int y = X + 1;
                              ^
                test.d:7:10: note: 'x' declared here
                const int x = 2;
                          ^
                test.d: In function 'testFunc':
                test.d:13:4: warning W2: name 'tesT' doesn't match declared name 'test' case wise
                    tesT();
                    ^
                test.d:10:10: note: 'test' declared here
                func void test() {};
                          ^
                test.d:16:4: warning W2: name 'A' doesn't match declared name 'a' case wise
                    A = B + X;
                    ^
                test.d:14:12: note: 'a' declared here
                    var int a;
                            ^
                test.d:16:8: warning W2: name 'B' doesn't match declared name 'b' case wise
                    A = B + X;
                        ^
                test.d:15:12: note: 'b' declared here
                    var int b;
                            ^
                test.d:16:12: warning W2: name 'X' doesn't match declared name 'x' case wise
                    A = B + X;
                            ^
                test.d:7:10: note: 'x' declared here
                const int x = 2;
                          ^
                test.d:17:8: warning W2: name 'hero' doesn't match declared name 'HERO' case wise
                    b = hero.str + HERO.STR;
                        ^
                test.d:4:9: note: 'HERO' declared here
                instance HERO(NPC);
                         ^
                test.d:17:24: warning W2: name 'STR' doesn't match declared name 'str' case wise
                    b = hero.str + HERO.STR;
                                        ^
                test.d:2:12: note: 'str' declared here
                    var int str;
                            ^
                test.d:18:14: warning W2: name 'STR' doesn't match declared name 'str' case wise
                    b = enemy.STR + ENEMY.str;
                              ^
                test.d:2:12: note: 'str' declared here
                    var int str;
                            ^
                test.d:18:20: warning W2: name 'ENEMY' doesn't match declared name 'enemy' case wise
                    b = enemy.STR + ENEMY.str;
                                    ^
                test.d:5:8: note: 'enemy' declared here
                var NPC enemy;
                        ^
                ";

            AssertCompilationOutputMatch();
        }
    }
}