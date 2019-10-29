using Xunit;

namespace DaedalusCompiler.Tests.SemanticErrors
{
    public class DeclarationUsagesCheckerTests : BaseSemanticErrorsTests
    {
        [Fact]
        public void TestNamesNotMatchingCaseWise()
        {
            Code = @"
                class NPC {
                    var int str;
                }
                var NPC HERO1;
                var npc HERO2;
                var NPC2 HERO3;
            ";

            ExpectedCompilationOutput = @"
                test.d:5:4: warning W2: name 'npc' doesn't match declared name 'NPC' case wise
                var npc HERO2;
                    ^
                test.d:1:6: note: 'NPC' declared here
                class NPC {
                      ^
                test.d:6:4: error: unknown type name 'NPC2'
                var NPC2 HERO3;
                    ^
                1 error, 1 warning generated.
                ";

            AssertCompilationOutputMatch();

            
            Code = @"
                class NPC {
                    var int str;
                }
                instance HERO1(NPC);
                instance HERO2(npc);
                instance HERO3(NPC2);
            ";

            ExpectedCompilationOutput = @"
                test.d: In instance 'HERO2':
                test.d:5:15: warning W2: name 'npc' doesn't match declared name 'NPC' case wise
                instance HERO2(npc);
                               ^
                test.d:1:6: note: 'NPC' declared here
                class NPC {
                      ^
                test.d: In instance 'HERO3':
                test.d:6:15: error: 'NPC2' undeclared
                instance HERO3(NPC2);
                               ^
                1 error, 1 warning generated.
                ";

            AssertCompilationOutputMatch();


            Code = @"
                //! suppress: W5

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
                test.d:10:14: warning W2: name 'X' doesn't match declared name 'x' case wise
                const int y = X + 1;
                              ^
                test.d:9:10: note: 'x' declared here
                const int x = 2;
                          ^
                test.d: In function 'testFunc':
                test.d:15:4: warning W2: name 'tesT' doesn't match declared name 'test' case wise
                    tesT();
                    ^
                test.d:12:10: note: 'test' declared here
                func void test() {};
                          ^
                test.d:18:4: warning W2: name 'A' doesn't match declared name 'a' case wise
                    A = B + X;
                    ^
                test.d:16:12: note: 'a' declared here
                    var int a;
                            ^
                test.d:18:8: warning W2: name 'B' doesn't match declared name 'b' case wise
                    A = B + X;
                        ^
                test.d:17:12: note: 'b' declared here
                    var int b;
                            ^
                test.d:18:12: warning W2: name 'X' doesn't match declared name 'x' case wise
                    A = B + X;
                            ^
                test.d:9:10: note: 'x' declared here
                const int x = 2;
                          ^
                test.d:19:8: warning W2: name 'hero' doesn't match declared name 'HERO' case wise
                    b = hero.str + HERO.STR;
                        ^
                test.d:6:9: note: 'HERO' declared here
                instance HERO(NPC);
                         ^
                test.d:19:24: warning W2: name 'STR' doesn't match declared name 'str' case wise
                    b = hero.str + HERO.STR;
                                        ^
                test.d:4:12: note: 'str' declared here
                    var int str;
                            ^
                test.d:20:14: warning W2: name 'STR' doesn't match declared name 'str' case wise
                    b = enemy.STR + ENEMY.str;
                              ^
                test.d:4:12: note: 'str' declared here
                    var int str;
                            ^
                test.d:20:20: warning W2: name 'ENEMY' doesn't match declared name 'enemy' case wise
                    b = enemy.STR + ENEMY.str;
                                    ^
                test.d:7:8: note: 'enemy' declared here
                var NPC enemy;
                        ^
                9 warnings generated.
                ";

            AssertCompilationOutputMatch();
        }
    }
}