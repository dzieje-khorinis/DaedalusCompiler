using System;
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

        [Fact]
        public void TestInvalidArgumentTypeToUnaryExpression()
        {
            Code = @"
                class NPC {
                    var int x;
                    var float y;
                }
                prototype ORC(NPC) {};
                instance HERO(NPC) {};

                func int retInt() { return 1; }
                func float retFloat() { return 2.5; }
                func string retString() { return ""str""; }

                func void testFunc() {
                    var int a;
                    var float b;
                    var string c;
                    var NONEXISTANTCLASS d;
                    var NPC enemy;
                    
                    a = -1;
                    a = +2;
                    a = !3;
                    a = ~4;
                    a = -d;
                    a = +d;
                    a = !d;
                    a = ~d;
                    a = -e;
                    a = +e;
                    a = !e;
                    a = ~e;
                    a = -retInt();
                    a = +retInt();
                    a = !retInt();
                    a = ~retInt();
                    a = -retInt;
                    a = +retInt;
                    a = !retInt;
                    a = ~retInt;
                    a = -HERO.x;
                    a = +HERO.x;
                    a = !HERO.x;
                    a = ~HERO.x;
                    a = -enemy.x;
                    a = +enemy.x;
                    a = !enemy.x;
                    a = ~enemy.x;
                    a = -a;
                    a = +a;
                    a = !a;
                    a = ~a;

                    b = -1.5;
                    b = +2.5;
                    b = !3.5;
                    b = ~4.5;
                    b = -retFloat();
                    b = +retFloat();
                    b = !retFloat();
                    b = ~retFloat();
                    b = -retFloat;
                    b = +retFloat;
                    b = !retFloat;
                    b = ~retFloat;
                    b = -HERO.y;
                    b = +HERO.y;
                    b = !HERO.y;
                    b = ~HERO.y;
                    b = -enemy.y;
                    b = +enemy.y;
                    b = !enemy.y;
                    b = ~enemy.y;
                    b = -b;
                    b = +b;
                    b = !b;
                    b = ~b;

                    c = -""minus"";
                    c = +""plus"";
                    c = !""not"";
                    c = ~""negate"";
                    c = -retString();
                    c = +retString();
                    c = !retString();
                    c = ~retString();
                    c = -retString;
                    c = +retString;
                    c = !retString;
                    c = ~retString;
                    c = -c;
                    c = +c;
                    c = !c;
                    c = ~c;

                    enemy = HERO;
                    enemy = -HERO;
                    enemy = +HERO;
                    enemy = !HERO;
                    enemy = ~HERO;
                    enemy = -ORC;
                    enemy = +ORC;
                    enemy = !ORC;
                    enemy = ~ORC;
                    enemy = -NPC;
                    enemy = +NPC;
                    enemy = !NPC;
                    enemy = ~NPC;
                    enemy = -enemy;
                    enemy = +enemy;
                    enemy = !enemy;
                    enemy = ~enemy;
                };
            ";

            ExpectedCompilationOutput = @"
                test.d: In function 'testFunc':
                test.d:16:8: error: unknown type name 'NONEXISTANTCLASS'
                    var NONEXISTANTCLASS d;
                        ^
                test.d:27:9: error: 'e' undeclared
                    a = -e;
                         ^
                test.d:28:9: error: 'e' undeclared
                    a = +e;
                         ^
                test.d:29:9: error: 'e' undeclared
                    a = !e;
                         ^
                test.d:30:9: error: 'e' undeclared
                    a = ~e;
                         ^
                test.d:35:8: error: invalid argument type 'Func' to unary expression
                    a = -retInt;
                        ^
                test.d:36:8: error: invalid argument type 'Func' to unary expression
                    a = +retInt;
                        ^
                test.d:37:8: error: invalid argument type 'Func' to unary expression
                    a = !retInt;
                        ^
                test.d:38:8: error: invalid argument type 'Func' to unary expression
                    a = ~retInt;
                        ^
                test.d:55:8: error: invalid argument type 'Float' to unary expression
                    b = ~4.5;
                        ^
                test.d:59:8: error: invalid argument type 'Float' to unary expression
                    b = ~retFloat();
                        ^
                test.d:60:8: error: invalid argument type 'Func' to unary expression
                    b = -retFloat;
                        ^
                test.d:61:8: error: invalid argument type 'Func' to unary expression
                    b = +retFloat;
                        ^
                test.d:62:8: error: invalid argument type 'Func' to unary expression
                    b = !retFloat;
                        ^
                test.d:63:8: error: invalid argument type 'Func' to unary expression
                    b = ~retFloat;
                        ^
                test.d:67:8: error: invalid argument type 'Float' to unary expression
                    b = ~HERO.y;
                        ^
                test.d:71:8: error: invalid argument type 'Float' to unary expression
                    b = ~enemy.y;
                        ^
                test.d:75:8: error: invalid argument type 'Float' to unary expression
                    b = ~b;
                        ^
                test.d:77:8: error: invalid argument type 'String' to unary expression
                    c = -""minus"";
                        ^
                test.d:78:8: error: invalid argument type 'String' to unary expression
                    c = +""plus"";
                        ^
                test.d:79:8: error: invalid argument type 'String' to unary expression
                    c = !""not"";
                        ^
                test.d:80:8: error: invalid argument type 'String' to unary expression
                    c = ~""negate"";
                        ^
                test.d:81:8: error: invalid argument type 'String' to unary expression
                    c = -retString();
                        ^
                test.d:82:8: error: invalid argument type 'String' to unary expression
                    c = +retString();
                        ^
                test.d:83:8: error: invalid argument type 'String' to unary expression
                    c = !retString();
                        ^
                test.d:84:8: error: invalid argument type 'String' to unary expression
                    c = ~retString();
                        ^
                test.d:85:8: error: invalid argument type 'Func' to unary expression
                    c = -retString;
                        ^
                test.d:86:8: error: invalid argument type 'Func' to unary expression
                    c = +retString;
                        ^
                test.d:87:8: error: invalid argument type 'Func' to unary expression
                    c = !retString;
                        ^
                test.d:88:8: error: invalid argument type 'Func' to unary expression
                    c = ~retString;
                        ^
                test.d:89:8: error: invalid argument type 'String' to unary expression
                    c = -c;
                        ^
                test.d:90:8: error: invalid argument type 'String' to unary expression
                    c = +c;
                        ^
                test.d:91:8: error: invalid argument type 'String' to unary expression
                    c = !c;
                        ^
                test.d:92:8: error: invalid argument type 'String' to unary expression
                    c = ~c;
                        ^
                test.d:95:12: error: invalid argument type 'Instance' to unary expression
                    enemy = -HERO;
                            ^
                test.d:96:12: error: invalid argument type 'Instance' to unary expression
                    enemy = +HERO;
                            ^
                test.d:97:12: error: invalid argument type 'Instance' to unary expression
                    enemy = !HERO;
                            ^
                test.d:98:12: error: invalid argument type 'Instance' to unary expression
                    enemy = ~HERO;
                            ^
                test.d:99:12: error: invalid argument type 'Prototype' to unary expression
                    enemy = -ORC;
                            ^
                test.d:100:12: error: invalid argument type 'Prototype' to unary expression
                    enemy = +ORC;
                            ^
                test.d:101:12: error: invalid argument type 'Prototype' to unary expression
                    enemy = !ORC;
                            ^
                test.d:102:12: error: invalid argument type 'Prototype' to unary expression
                    enemy = ~ORC;
                            ^
                test.d:103:12: error: invalid argument type 'Class' to unary expression
                    enemy = -NPC;
                            ^
                test.d:104:12: error: invalid argument type 'Class' to unary expression
                    enemy = +NPC;
                            ^
                test.d:105:12: error: invalid argument type 'Class' to unary expression
                    enemy = !NPC;
                            ^
                test.d:106:12: error: invalid argument type 'Class' to unary expression
                    enemy = ~NPC;
                            ^
                test.d:107:12: error: invalid argument type 'Instance' to unary expression
                    enemy = -enemy;
                            ^
                test.d:108:12: error: invalid argument type 'Instance' to unary expression
                    enemy = +enemy;
                            ^
                test.d:109:12: error: invalid argument type 'Instance' to unary expression
                    enemy = !enemy;
                            ^
                test.d:110:12: error: invalid argument type 'Instance' to unary expression
                    enemy = ~enemy;
                            ^
            ";

            AssertCompilationOutputMatch();
        }
    }
}