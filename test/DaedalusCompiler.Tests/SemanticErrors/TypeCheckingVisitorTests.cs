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

                func void parFloat(var float a) {}
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

                    parFloat(-1.5);
                    parFloat(+2.5);
                    parFloat(!3.5);
                    parFloat(~4.5);
                    parFloat(-retFloat());
                    parFloat(+retFloat());
                    parFloat(!retFloat());
                    parFloat(~retFloat());
                    parFloat(-retFloat);
                    parFloat(+retFloat);
                    parFloat(!retFloat);
                    parFloat(~retFloat);
                    parFloat(-HERO.y);
                    parFloat(+HERO.y);
                    parFloat(!HERO.y);
                    parFloat(~HERO.y);
                    parFloat(-enemy.y);
                    parFloat(+enemy.y);
                    parFloat(!enemy.y);
                    parFloat(~enemy.y);
                    parFloat(-b);
                    parFloat(+b);
                    parFloat(!b);
                    parFloat(~b);

                    return -1.5;
                    return +2.5;
                    return !3.5;
                    return ~4.5;
                    return -retFloat();
                    return +retFloat();
                    return !retFloat();
                    return ~retFloat();
                    return -retFloat;
                    return +retFloat;
                    return !retFloat;
                    return ~retFloat;
                    return -HERO.y;
                    return +HERO.y;
                    return !HERO.y;
                    return ~HERO.y;
                    return -enemy.y;
                    return +enemy.y;
                    return !enemy.y;
                    return ~enemy.y;
                    return -b;
                    return +b;
                    return !b;
                    return ~b;
                };
            ";

            ExpectedCompilationOutput = @"
                test.d: In function 'testFunc':
                test.d:17:8: error: unknown type name 'NONEXISTANTCLASS'
                    var NONEXISTANTCLASS d;
                        ^
                test.d:28:9: error: 'e' undeclared
                    a = -e;
                         ^
                test.d:29:9: error: 'e' undeclared
                    a = +e;
                         ^
                test.d:30:9: error: 'e' undeclared
                    a = !e;
                         ^
                test.d:31:9: error: 'e' undeclared
                    a = ~e;
                         ^
                test.d:36:8: error: invalid argument type 'func' to unary expression
                    a = -retInt;
                        ^
                test.d:37:8: error: invalid argument type 'func' to unary expression
                    a = +retInt;
                        ^
                test.d:38:8: error: invalid argument type 'func' to unary expression
                    a = !retInt;
                        ^
                test.d:39:8: error: invalid argument type 'func' to unary expression
                    a = ~retInt;
                        ^
                test.d:56:8: error: invalid argument type 'float' to unary expression
                    b = ~4.5;
                        ^
                test.d:60:8: error: invalid argument type 'float' to unary expression
                    b = ~retFloat();
                        ^
                test.d:61:8: error: invalid argument type 'func' to unary expression
                    b = -retFloat;
                        ^
                test.d:62:8: error: invalid argument type 'func' to unary expression
                    b = +retFloat;
                        ^
                test.d:63:8: error: invalid argument type 'func' to unary expression
                    b = !retFloat;
                        ^
                test.d:64:8: error: invalid argument type 'func' to unary expression
                    b = ~retFloat;
                        ^
                test.d:68:8: error: invalid argument type 'float' to unary expression
                    b = ~HERO.y;
                        ^
                test.d:72:8: error: invalid argument type 'float' to unary expression
                    b = ~enemy.y;
                        ^
                test.d:76:8: error: invalid argument type 'float' to unary expression
                    b = ~b;
                        ^
                test.d:78:8: error: invalid argument type 'string' to unary expression
                    c = -""minus"";
                        ^
                test.d:79:8: error: invalid argument type 'string' to unary expression
                    c = +""plus"";
                        ^
                test.d:80:8: error: invalid argument type 'string' to unary expression
                    c = !""not"";
                        ^
                test.d:81:8: error: invalid argument type 'string' to unary expression
                    c = ~""negate"";
                        ^
                test.d:82:8: error: invalid argument type 'string' to unary expression
                    c = -retString();
                        ^
                test.d:83:8: error: invalid argument type 'string' to unary expression
                    c = +retString();
                        ^
                test.d:84:8: error: invalid argument type 'string' to unary expression
                    c = !retString();
                        ^
                test.d:85:8: error: invalid argument type 'string' to unary expression
                    c = ~retString();
                        ^
                test.d:86:8: error: invalid argument type 'func' to unary expression
                    c = -retString;
                        ^
                test.d:87:8: error: invalid argument type 'func' to unary expression
                    c = +retString;
                        ^
                test.d:88:8: error: invalid argument type 'func' to unary expression
                    c = !retString;
                        ^
                test.d:89:8: error: invalid argument type 'func' to unary expression
                    c = ~retString;
                        ^
                test.d:90:8: error: invalid argument type 'string' to unary expression
                    c = -c;
                        ^
                test.d:91:8: error: invalid argument type 'string' to unary expression
                    c = +c;
                        ^
                test.d:92:8: error: invalid argument type 'string' to unary expression
                    c = !c;
                        ^
                test.d:93:8: error: invalid argument type 'string' to unary expression
                    c = ~c;
                        ^
                test.d:96:12: error: invalid argument type 'instance' to unary expression
                    enemy = -HERO;
                            ^
                test.d:97:12: error: invalid argument type 'instance' to unary expression
                    enemy = +HERO;
                            ^
                test.d:98:12: error: invalid argument type 'instance' to unary expression
                    enemy = !HERO;
                            ^
                test.d:99:12: error: invalid argument type 'instance' to unary expression
                    enemy = ~HERO;
                            ^
                test.d:100:12: error: invalid argument type 'prototype' to unary expression
                    enemy = -ORC;
                            ^
                test.d:101:12: error: invalid argument type 'prototype' to unary expression
                    enemy = +ORC;
                            ^
                test.d:102:12: error: invalid argument type 'prototype' to unary expression
                    enemy = !ORC;
                            ^
                test.d:103:12: error: invalid argument type 'prototype' to unary expression
                    enemy = ~ORC;
                            ^
                test.d:104:12: error: invalid argument type 'class' to unary expression
                    enemy = -NPC;
                            ^
                test.d:105:12: error: invalid argument type 'class' to unary expression
                    enemy = +NPC;
                            ^
                test.d:106:12: error: invalid argument type 'class' to unary expression
                    enemy = !NPC;
                            ^
                test.d:107:12: error: invalid argument type 'class' to unary expression
                    enemy = ~NPC;
                            ^
                test.d:108:12: error: invalid argument type 'instance' to unary expression
                    enemy = -enemy;
                            ^
                test.d:109:12: error: invalid argument type 'instance' to unary expression
                    enemy = +enemy;
                            ^
                test.d:110:12: error: invalid argument type 'instance' to unary expression
                    enemy = !enemy;
                            ^
                test.d:111:12: error: invalid argument type 'instance' to unary expression
                    enemy = ~enemy;
                            ^
                test.d:116:13: error: invalid argument type 'float' to unary expression
                    parFloat(~4.5);
                             ^
                test.d:120:13: error: invalid argument type 'float' to unary expression
                    parFloat(~retFloat());
                             ^
                test.d:121:13: error: invalid argument type 'func' to unary expression
                    parFloat(-retFloat);
                             ^
                test.d:122:13: error: invalid argument type 'func' to unary expression
                    parFloat(+retFloat);
                             ^
                test.d:123:13: error: invalid argument type 'func' to unary expression
                    parFloat(!retFloat);
                             ^
                test.d:124:13: error: invalid argument type 'func' to unary expression
                    parFloat(~retFloat);
                             ^
                test.d:128:13: error: invalid argument type 'float' to unary expression
                    parFloat(~HERO.y);
                             ^
                test.d:132:13: error: invalid argument type 'float' to unary expression
                    parFloat(~enemy.y);
                             ^
                test.d:136:13: error: invalid argument type 'float' to unary expression
                    parFloat(~b);
                             ^
                test.d:141:11: error: invalid argument type 'float' to unary expression
                    return ~4.5;
                           ^
                test.d:145:11: error: invalid argument type 'float' to unary expression
                    return ~retFloat();
                           ^
                test.d:146:11: error: invalid argument type 'func' to unary expression
                    return -retFloat;
                           ^
                test.d:147:11: error: invalid argument type 'func' to unary expression
                    return +retFloat;
                           ^
                test.d:148:11: error: invalid argument type 'func' to unary expression
                    return !retFloat;
                           ^
                test.d:149:11: error: invalid argument type 'func' to unary expression
                    return ~retFloat;
                           ^
                test.d:153:11: error: invalid argument type 'float' to unary expression
                    return ~HERO.y;
                           ^
                test.d:157:11: error: invalid argument type 'float' to unary expression
                    return ~enemy.y;
                           ^
                test.d:161:11: error: invalid argument type 'float' to unary expression
                    return ~b;
                           ^
            ";

            AssertCompilationOutputMatch();
        }

        [Fact]
        public void TestBinaryOperationsNotAllowedInsideFloatExpression()
        {
            Code = @"
                func void parFloat(var float a) {
                    a = 1 + 2;
                    a = 2.5 + 2.5;
                    a = a + a;
                };

                func float retFloat() {
                    var float a;
                    a = 2.5;
                    return 1 + 2;
                    return 2.5 + 2.5;
                    return a + a;
                };

                func void testFunc() {
                    var float a;
                    a = 2.5;
                    parFloat(1 + 2);
                    parFloat(2.5 + 2.5);
                    parFloat(a + a);
                };
            ";

            ExpectedCompilationOutput = @"
                test.d: In function 'parFloat':
                test.d:2:10: error: binary operations not allowed inside 'float' expression
                    a = 1 + 2;
                          ^
                test.d:3:12: error: binary operations not allowed inside 'float' expression
                    a = 2.5 + 2.5;
                            ^
                test.d:4:10: error: binary operations not allowed inside 'float' expression
                    a = a + a;
                          ^
                test.d: In function 'retFloat':
                test.d:10:13: error: binary operations not allowed inside 'float' expression
                    return 1 + 2;
                             ^
                test.d:11:15: error: binary operations not allowed inside 'float' expression
                    return 2.5 + 2.5;
                               ^
                test.d:12:13: error: binary operations not allowed inside 'float' expression
                    return a + a;
                             ^
                test.d: In function 'testFunc':
                test.d:18:15: error: binary operations not allowed inside 'float' expression
                    parFloat(1 + 2);
                               ^
                test.d:19:17: error: binary operations not allowed inside 'float' expression
                    parFloat(2.5 + 2.5);
                                 ^
                test.d:20:15: error: binary operations not allowed inside 'float' expression
                    parFloat(a + a);
                               ^
            ";

            AssertCompilationOutputMatch();
        }


        [Fact]
        public void TestCompoundAssignmentSupport()
        {
            Code = @"
                func string ConcatStrings(var string par0, var string par1) {};

                class NPC {};
                prototype ORC(NPC) {};
                instance BOB(ORC) {};
                func void firstFunc() {};

                func void testFunc() {
                    var string b;
                    b = ""Jan"";
                    var float a;
                    a = 0;
                    a += 1;

                    NPC += 2;
                    ORC += 3;
                    BOB += 4;
                    firstFunc += 5;
                    b += "" "" + ""Kowalski"";
                };
            ";

            ExpectedCompilationOutput = @"
                test.d: In function 'testFunc':
                test.d:13:6: error: 'float' type doesn't have support for compound assignments
                    a += 1;
                      ^
                test.d:15:8: error: unsupported operation
                    NPC += 2;
                        ^
                test.d:16:8: error: unsupported operation
                    ORC += 3;
                        ^
                test.d:17:8: error: unsupported operation
                    BOB += 4;
                        ^
                test.d:18:14: error: unsupported operation
                    firstFunc += 5;
                              ^
            ";

            AssertCompilationOutputMatch();
        }
        
    }
}