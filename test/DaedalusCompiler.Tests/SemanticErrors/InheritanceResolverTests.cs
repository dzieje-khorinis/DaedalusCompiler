using Xunit;

namespace DaedalusCompiler.Tests.SemanticErrors
{
    public class InheritanceResolverTests : BaseSemanticErrorsTests
    {
        [Fact]
        public void TestInfiniteReferenceLoop()
        {
            Code = @"
                instance a(b) {};
                instance b(a) {};
            ";

            ExpectedCompilationOutput = @"
                test.d: In instance 'a':
                test.d:1:11: error: circular inheritance dependency detected
                instance a(b) {};
                           ^
            ";

            AssertCompilationOutputMatch();
        }
        
        
        [Fact]
        public void TestUndeclaredIdentifierInheritance()
        {
            Code = @"
                class C_NPC { var int data [200]; };
                instance self(C_NPC_MISSPELLED) {};
                prototype OrcElite(Orc) {};
                instance UrukHai(OrcElite);
            ";

            ExpectedCompilationOutput = @"
                test.d: In instance 'self':
                test.d:2:14: error: 'C_NPC_MISSPELLED' undeclared
                instance self(C_NPC_MISSPELLED) {};
                              ^
                test.d: In prototype 'OrcElite':
                test.d:3:19: error: 'Orc' undeclared
                prototype OrcElite(Orc) {};
                                   ^
                ";

            AssertCompilationOutputMatch();
        }
        
        [Fact]
        public void TestNotValidClassOrPrototype()
        {
            Code = @"
                class myClass {};
                func void myFunc() {};
                
                instance WRONG1(myFunc);
                instance WRONG2(myFunc) {};
                prototype WRONG3(myFunc) {};
                
                instance CORRECT1(myClass);
                instance CORRECT2(myClass) {};
                prototype CORRECT3(myClass) {};
                
                instance CORRECT4(WRONG3);
                instance CORRECT5(WRONG3) {};
                prototype CORRECT6(WRONG3) {};
            ";

            ExpectedCompilationOutput = @"
                test.d: In instance 'WRONG1':
                test.d:4:16: error: not a valid class or prototype
                instance WRONG1(myFunc);
                                ^
                test.d: In instance 'WRONG2':
                test.d:5:16: error: not a valid class or prototype
                instance WRONG2(myFunc) {};
                                ^
                test.d: In prototype 'WRONG3':
                test.d:6:17: error: not a valid class or prototype
                prototype WRONG3(myFunc) {};
                                 ^
                ";
            AssertCompilationOutputMatch();
        }
    }
}