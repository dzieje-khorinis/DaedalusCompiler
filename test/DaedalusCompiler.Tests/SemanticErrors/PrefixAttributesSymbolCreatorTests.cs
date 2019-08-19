using Xunit;

namespace DaedalusCompiler.Tests.SemanticErrors
{
    public class PrefixAttributesSymbolCreatorTests : BaseSemanticErrorsTests
    {
        [Fact]
        public void TestClassTypeAttributes()
        {
            Code = @"
                class Pet {
                    var int x;
                    var int y;
                    var Blah uruk;
                };
                
                class Human {
                    var int b;
                    var Pet dog;
                };
                
                class Orc {
                    var int c;
                    var Pet dog;
                };
                
                instance Person1(Human);
                var Human Person2;
                
                instance Dog1(Pet);
                var Pet Dog2;
                
                func void myFunc() {
                    var int x;
                    x = Person1.dog.x;
                    x = Person2.dog.y;
                    
                    x = Dog1.uruk.b;
                    x = Dog2.uruk.b;
                };
            ";

            ExpectedCompilationOutput = @"
                test.d: In class 'Pet':
                test.d:4:8: error: unknown type name 'Blah'
                    var Blah uruk;
                        ^
                test.d: In function 'myFunc':
                test.d:28:18: error: cannot access attribute 'b' because 'Dog1.uruk' is not an instance of a class
                    x = Dog1.uruk.b;
                                  ^
                test.d:29:18: error: cannot access attribute 'b' because 'Dog2.uruk' is not an instance of a class
                    x = Dog2.uruk.b;
                                  ^
                ";
            AssertCompilationOutputMatch();
        }
        
        [Fact]
        public void TestClassTypeAttributesReferenceLoop()
        {
            Code = @"
                class Pet {
                    var int x;
                    var int y;
                    var Human person;
                };
                
                class Human {
                    var int b;
                    var Pet dog;
                };
                
                class Orc {
                    var int c;
                    var Pet dog;
                };
                
                instance Person1(Human);
                var Human Person2;
                
                instance Dog1(Pet);
                var Pet Dog2;
                
                func void myFunc() {
                    var int x;
                    x = Person1.dog.x;
                    x = Person2.dog.y;
                    
                    x = Dog1.person.b;
                    x = Dog2.person.c;
                };
            ";

            ExpectedCompilationOutput = @"
                test.d: In class 'Pet':
                test.d:4:8: error: circular attribute reference dependency detected
                    var Human person;
                        ^
                test.d: In function 'myFunc':
                test.d:29:20: error: object 'Dog2.person' of type 'Human' has no member named 'c'
                    x = Dog2.person.c;
                                    ^
                ";
            AssertCompilationOutputMatch();
        }
    }
}