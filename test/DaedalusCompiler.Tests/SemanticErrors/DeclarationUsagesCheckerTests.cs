using Xunit;

namespace DaedalusCompiler.Tests.SemanticErrors
{
    public class DeclarationUsagesCheckerTests : BaseSemanticErrorsTests
    {
        [Fact]
        public void TestUnusedSymbol()
        {
            Zen = @"
                ZenGin Archive
                ver 1
                zCArchiverGeneric
                ASCII
                saveGame 0
                date 20.4.2020 21:37:00
                user kisioj
                END
                objects 4
                END

                [% oCWorld:zCWorld 64513 0]
                    [VobTree % 0 0]
                        childs0=int:1
                        [% zCVob 52224 1]
                            onStateFunc=string:func1
                            conditionFunc=string:func2
                            scriptFunc=string:func3
                            focusName=string:const2
                        []
                        childs1=int:1
                        [% zCVob 52224 2]
                            focusName=string:const3
                        []
                    []
                    [EndMarker % 0 0]
                    []
                []
            ";
            
            Code = @"
                class Test { var int a; }
                class C_NPC { var int data [200]; }

                prototype Human1(C_NPC) {}
                prototype Human2(C_NPC) {}

                instance Person1(C_NPC) {}
                instance Person2(Human2) {}

                var int a;
                var int b;
                var int c;

                func void G_PickLock (var int a, var int b) {}

                func void func1_S1() {}
                func void func2_S1() {}

                func void func1(var int a, var int b) {
                    a = 1;
                };
                func void func2() {
                    b = 2;
                };
                func void func3() {};
                func void func4() {};

                const int const1 = 0;
                const int const2 = 0;
                const int const3 = 0;
            ";

            ExpectedCompilationOutput = @"
                test.d: In prototype 'Human1':
                test.d:4:10: warning W3: unused symbol
                prototype Human1(C_NPC) {}
                          ^
                test.d: In global scope:
                test.d:10:8: warning W3: unused symbol
                var int a;
                        ^
                test.d:12:8: warning W3: unused symbol
                var int c;
                        ^
                test.d: In function 'func2_S1':
                test.d:17:10: warning W3: unused symbol
                func void func2_S1() {}
                          ^
                test.d: In function 'func1':
                test.d:19:10: warning W3: unused symbol
                func void func1(var int a, var int b) {
                          ^
                test.d:19:35: warning W3: unused symbol
                func void func1(var int a, var int b) {
                                                   ^
                test.d: In function 'func4':
                test.d:26:10: warning W3: unused symbol
                func void func4() {};
                          ^
                test.d: In global scope:
                test.d:28:10: warning W3: unused symbol
                const int const1 = 0;
                          ^
                8 warnings generated.
            ";

            AssertCompilationOutputMatch(detectUnused:true);
        }
        
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
                //! suppress: W3 W5

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