using Xunit;

namespace DaedalusCompiler.Tests.SemanticErrors
{
    public class SymbolTableCreationTests : BaseSemanticErrorsTests
    {
        [Fact]
        public void TestGlobalRedefinition()
        {
            Code = @"
                class __class {};
                func void __class() {};
                func void __class() {};
            ";

            ExpectedCompilationOutput = @"
                test.d:2:10: error: redefinition of '__class'
                func void __class() {};
                          ^
                test.d:1:6: note: previous definition is here
                class __class {};
                      ^
                test.d:3:10: error: redefinition of '__class'
                func void __class() {};
                          ^
                test.d:1:6: note: previous definition is here
                class __class {};
                      ^
                ";
            AssertCompilationOutputMatch();
            

            Code = @"
                func void __func() {};
                class __func {};
                class __func {};
            ";

            ExpectedCompilationOutput = @"
                test.d:2:6: error: redefinition of '__func'
                class __func {};
                      ^
                test.d:1:10: note: previous definition is here
                func void __func() {};
                          ^
                test.d:3:6: error: redefinition of '__func'
                class __func {};
                      ^
                test.d:1:10: note: previous definition is here
                func void __func() {};
                          ^
                ";

            AssertCompilationOutputMatch();
            
            
            Code = @"
                class C_NPC { var int data [200]; };
                instance __instanceDecl(C_NPC);
                prototype __instanceDecl(C_NPC) {};
                prototype __instanceDecl(C_NPC) {};
            ";

            ExpectedCompilationOutput = @"
                test.d:3:10: error: redefinition of '__instanceDecl'
                prototype __instanceDecl(C_NPC) {};
                          ^
                test.d:2:9: note: previous definition is here
                instance __instanceDecl(C_NPC);
                         ^
                test.d:4:10: error: redefinition of '__instanceDecl'
                prototype __instanceDecl(C_NPC) {};
                          ^
                test.d:2:9: note: previous definition is here
                instance __instanceDecl(C_NPC);
                         ^
                ";

            AssertCompilationOutputMatch();
            
            
            Code = @"
                class C_NPC { var int data [200]; };
                prototype __prototype(C_NPC) {};
                instance __prototype(C_NPC);
                instance __prototype(C_NPC);
            ";

            ExpectedCompilationOutput = @"
                test.d:3:9: error: redefinition of '__prototype'
                instance __prototype(C_NPC);
                         ^
                test.d:2:10: note: previous definition is here
                prototype __prototype(C_NPC) {};
                          ^
                test.d:4:9: error: redefinition of '__prototype'
                instance __prototype(C_NPC);
                         ^
                test.d:2:10: note: previous definition is here
                prototype __prototype(C_NPC) {};
                          ^
                ";

            AssertCompilationOutputMatch();
            
            
            Code = @"
                class C_NPC { var int data [200]; };
                instance instanceDef(C_NPC) {};
                const int instanceDef = 0;
                const int instanceDef = 0;
            ";

            ExpectedCompilationOutput = @"
                test.d:3:10: error: redefinition of 'instanceDef'
                const int instanceDef = 0;
                          ^
                test.d:2:9: note: previous definition is here
                instance instanceDef(C_NPC) {};
                         ^
                test.d:4:10: error: redefinition of 'instanceDef'
                const int instanceDef = 0;
                          ^
                test.d:2:9: note: previous definition is here
                instance instanceDef(C_NPC) {};
                         ^
                ";

            AssertCompilationOutputMatch();
            
            
            Code = @"
                class C_NPC { var int data [200]; };
                const int constInt = 0;
                instance constInt(C_NPC) {};
                instance constInt(C_NPC) {};
            ";

            ExpectedCompilationOutput = @"
                test.d:3:9: error: redefinition of 'constInt'
                instance constInt(C_NPC) {};
                         ^
                test.d:2:10: note: previous definition is here
                const int constInt = 0;
                          ^
                test.d:4:9: error: redefinition of 'constInt'
                instance constInt(C_NPC) {};
                         ^
                test.d:2:10: note: previous definition is here
                const int constInt = 0;
                          ^
                ";

            AssertCompilationOutputMatch();
            
            
            Code = @"
                const int constIntArr[2] = {0, 1};
                var float constIntArr;
                var float constIntArr;
            ";

            ExpectedCompilationOutput = @"
                test.d:2:10: error: redefinition of 'constIntArr'
                var float constIntArr;
                          ^
                test.d:1:10: note: previous definition is here
                const int constIntArr[2] = {0, 1};
                          ^
                test.d:3:10: error: redefinition of 'constIntArr'
                var float constIntArr;
                          ^
                test.d:1:10: note: previous definition is here
                const int constIntArr[2] = {0, 1};
                          ^
                ";

            AssertCompilationOutputMatch();
            
            
            Code = @"
                var float varFloat;
                const int varFloat[2] = {0, 1};
                const int varFloat[2] = {0, 1};
            ";

            ExpectedCompilationOutput = @"
                test.d:2:10: error: redefinition of 'varFloat'
                const int varFloat[2] = {0, 1};
                          ^
                test.d:1:10: note: previous definition is here
                var float varFloat;
                          ^
                test.d:3:10: error: redefinition of 'varFloat'
                const int varFloat[2] = {0, 1};
                          ^
                test.d:1:10: note: previous definition is here
                var float varFloat;
                          ^
                ";

            AssertCompilationOutputMatch();
        }

        [Fact]
        public void TestLocalRedefinition()
        {
            Code = @"
                class C_NPC { var int data [200]; };
                var int a;
                var float b;
                var string c;
                var C_NPC d;
                var func e;
                
                func void firstFunc() {
                    var int a;
                    var float b;
                    var string c;
                    var C_NPC d;
                    var func e;
                    
                    var int a;
                    var float b;
                    var string c;
                    var C_NPC d;
                    var func e;
                }
                
                func void secondFunc() {
                    var int a;
                    var float b;
                    var string c;
                    var C_NPC d;
                    var func e;
                }
            ";

            ExpectedCompilationOutput = @"
                test.d: In function 'firstFunc':
                test.d:15:12: error: redefinition of 'a'
                    var int a;
                            ^
                test.d:9:12: note: previous definition is here
                    var int a;
                            ^
                test.d:16:14: error: redefinition of 'b'
                    var float b;
                              ^
                test.d:10:14: note: previous definition is here
                    var float b;
                              ^
                test.d:17:15: error: redefinition of 'c'
                    var string c;
                               ^
                test.d:11:15: note: previous definition is here
                    var string c;
                               ^
                test.d:18:14: error: redefinition of 'd'
                    var C_NPC d;
                              ^
                test.d:12:14: note: previous definition is here
                    var C_NPC d;
                              ^
                test.d:19:13: error: redefinition of 'e'
                    var func e;
                             ^
                test.d:13:13: note: previous definition is here
                    var func e;
                             ^
                ";
            AssertCompilationOutputMatch();
        }
        
        
        
        [Fact]
        public void TestWhileKeywordAsIdentifier()
        {
            Code = @"
                const int break = 1;
                const int continue = 2;
                
                func void while() {};
                
                func void myFunc(var func x) {};
                
                func void testFunc() {
                    while();
                    myFunc(while);
                };
            ";

            ExpectedCompilationOutput = @"
                test.d:1:10: error: 'break' is keyword and shouldn't be used as an identifier
                const int break = 1;
                          ^
                test.d:2:10: error: 'continue' is keyword and shouldn't be used as an identifier
                const int continue = 2;
                          ^
                test.d:4:10: error: 'while' is keyword and shouldn't be used as an identifier
                func void while() {};
                          ^
            ";

            AssertCompilationOutputMatch();
        }

        [Fact]
        public void TestIterationStatementNotInLoop()
        {
            Code = @"
                const int break = 1;
                
                func void testFunc() {
                    break;
                    continue;
                };
            ";

            ExpectedCompilationOutput = @"
                test.d:1:10: error: 'break' is keyword and shouldn't be used as an identifier
                const int break = 1;
                          ^
                test.d: In function 'testFunc':
                test.d:4:4: error: 'break' statement not allowed outside of loop statement
                    break;
                    ^
                test.d:5:4: error: 'continue' statement not allowed outside of loop statement
                    continue;
                    ^
            ";

            AssertCompilationOutputMatch();
        }
    }
}