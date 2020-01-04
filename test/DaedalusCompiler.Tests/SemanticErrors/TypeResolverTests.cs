using System;
using Xunit;

namespace DaedalusCompiler.Tests.SemanticErrors
{
    public class TypeResolverTests : BaseSemanticErrorsTests
    {
        [Fact]
        public void TestUnknownTypeName()
        {
            Code = @"
                const b a = 1;
                const a b[2] = {1, 2};
                var d c;
                var c d[3];
                
                func x myFunc() {
                    const c a = 1;
                    const d b[4] = {2, 3, 4, 5};
                    var a c;
                    var b d[5];
                };
            ";

            ExpectedCompilationOutput = @"
                test.d:1:6: error: unknown type name 'b'
                const b a = 1;
                      ^
                test.d:2:6: error: unknown type name 'a'
                const a b[2] = {1, 2};
                      ^
                test.d:3:4: error: unknown type name 'd'
                var d c;
                    ^
                test.d:4:4: error: unknown type name 'c'
                var c d[3];
                    ^
                test.d:6:5: error: unknown type name 'x'
                func x myFunc() {
                     ^
                test.d: In function 'myFunc':
                test.d:7:10: error: unknown type name 'c'
                    const c a = 1;
                          ^
                test.d:8:10: error: unknown type name 'd'
                    const d b[4] = {2, 3, 4, 5};
                          ^
                test.d:9:8: error: unknown type name 'a'
                    var a c;
                        ^
                test.d:10:8: error: unknown type name 'b'
                    var b d[5];
                        ^
                9 errors generated.
            ";

            AssertCompilationOutputMatch();
        }

        [Fact]
        public void TestUnsupportedFunctionType()
        {
            Code = @"
                func Uninitialized func1() {}
                func Void func2() {}
                func Float func3() {}
                func Int func4() {}
                func String func5() {}
                func Class func6() {}
                func Func func7() {}
                func Prototype func8() {}
                func Instance func9() {}
            ";

            ExpectedCompilationOutput = @"
                test.d:1:5: error: unknown type name 'Uninitialized'
                func Uninitialized func1() {}
                     ^
                test.d:6:5: error: unsupported function return type
                func Class func6() {}
                     ^
                test.d:7:5: error: unsupported function return type
                func Func func7() {}
                     ^
                test.d:8:5: error: unsupported function return type
                func Prototype func8() {}
                     ^
                4 errors generated.
            ";

            AssertCompilationOutputMatch();
        }
        
        
        [Fact]
        public void TestUnsupportedArrayType()
        {
            Code = @"
                const Uninitialized a1[2] = {1, 200};
                const Void a2[2] = {1, 200};
                const Float a3[2] = {1, 200};
                const Int a4[2] = {1, 200};
                const String a5[2] = {1, 200};
                const Class a6[2] = {1, 200};
                const Func a7[2] = {1, 200};
                const Prototype a8[2] = {1, 200};
                const Instance a9[2] = {1, 200};
                
                var Uninitialized b1[2];
                var Void b2[2];
                var Float b3[2];
                var Int b4[2];
                var String b5[2];
                var Class b6[2];
                var Func b7[2];
                var Prototype b8[2];
                var Instance b9[2];
                
                func void myFunc() {
                    const Uninitialized c1[2] = {1, 200};
                    const Void c2[2] = {1, 200};
                    const Float c3[2] = {1, 200};
                    const Int c4[2] = {1, 200};
                    const String c5[2] = {1, 200};
                    const Class c6[2] = {1, 200};
                    const Func c7[2] = {1, 200};
                    const Prototype c8[2] = {1, 200};
                    const Instance c9[2] = {1, 200};
                    
                    var Uninitialized d1[2];
                    var Void d2[2];
                    var Float d3[2];
                    var Int d4[2];
                    var String d5[2];
                    var Class d6[2];
                    var Func d7[2];
                    var Prototype d8[2];
                    var Instance d9[2];
                }
            ";

            ExpectedCompilationOutput = @"
                test.d:1:6: error: unknown type name 'Uninitialized'
                const Uninitialized a1[2] = {1, 200};
                      ^
                test.d:2:6: error: unsupported array type
                const Void a2[2] = {1, 200};
                      ^
                test.d:5:22: error: cannot initialize an array element of type 'string' with an rvalue of type 'int'
                const String a5[2] = {1, 200};
                                      ^
                test.d:5:25: error: cannot initialize an array element of type 'string' with an rvalue of type 'int'
                const String a5[2] = {1, 200};
                                         ^~~
                test.d:6:6: error: unsupported array type
                const Class a6[2] = {1, 200};
                      ^
                test.d:7:20: error: cannot initialize an array element of type 'func' with an rvalue of type 'int'
                const Func a7[2] = {1, 200};
                                    ^
                test.d:7:23: error: cannot initialize an array element of type 'func' with an rvalue of type 'int'
                const Func a7[2] = {1, 200};
                                       ^~~
                test.d:8:6: error: unsupported array type
                const Prototype a8[2] = {1, 200};
                      ^
                test.d:9:6: error: unsupported array type
                const Instance a9[2] = {1, 200};
                      ^
                test.d:11:4: error: unknown type name 'Uninitialized'
                var Uninitialized b1[2];
                    ^
                test.d:12:4: error: unsupported array type
                var Void b2[2];
                    ^
                test.d:16:4: error: unsupported array type
                var Class b6[2];
                    ^
                test.d:18:4: error: unsupported array type
                var Prototype b8[2];
                    ^
                test.d:19:4: error: unsupported array type
                var Instance b9[2];
                    ^
                test.d: In function 'myFunc':
                test.d:22:10: error: unknown type name 'Uninitialized'
                    const Uninitialized c1[2] = {1, 200};
                          ^
                test.d:23:10: error: unsupported array type
                    const Void c2[2] = {1, 200};
                          ^
                test.d:26:26: error: cannot initialize an array element of type 'string' with an rvalue of type 'int'
                    const String c5[2] = {1, 200};
                                          ^
                test.d:26:29: error: cannot initialize an array element of type 'string' with an rvalue of type 'int'
                    const String c5[2] = {1, 200};
                                             ^~~
                test.d:27:10: error: unsupported array type
                    const Class c6[2] = {1, 200};
                          ^
                test.d:28:24: error: cannot initialize an array element of type 'func' with an rvalue of type 'int'
                    const Func c7[2] = {1, 200};
                                        ^
                test.d:28:27: error: cannot initialize an array element of type 'func' with an rvalue of type 'int'
                    const Func c7[2] = {1, 200};
                                           ^~~
                test.d:29:10: error: unsupported array type
                    const Prototype c8[2] = {1, 200};
                          ^
                test.d:30:10: error: unsupported array type
                    const Instance c9[2] = {1, 200};
                          ^
                test.d:32:8: error: unknown type name 'Uninitialized'
                    var Uninitialized d1[2];
                        ^
                test.d:33:8: error: unsupported array type
                    var Void d2[2];
                        ^
                test.d:37:8: error: unsupported array type
                    var Class d6[2];
                        ^
                test.d:39:8: error: unsupported array type
                    var Prototype d8[2];
                        ^
                test.d:40:8: error: unsupported array type
                    var Instance d9[2];
                        ^
                28 errors generated.
            ";

            AssertCompilationOutputMatch();
        }
        
        
        [Fact]
        public void TestUnsupportedVariableType()
        {
            Code = @"
                const Uninitialized a1 = 1;
                const Void a2 = 2;
                const Float a3 = 3;
                const Int a4 = 4;
                const String a5 = 5;
                const Class a6 = 6;
                const Func a7 = 7;
                const Prototype a8 = 8;
                const Instance a9 = 9;
                
                var Uninitialized b1;
                var Void b2;
                var Float b3;
                var Int b4;
                var String b5;
                var Class b6;
                var Func b7;
                var Prototype b8;
                var Instance b9;
                
                func void myFunc() {
                    const Uninitialized c1 = 1;
                    const Void c2 = 2;
                    const Float c3 = 3;
                    const Int c4 = 4;
                    const String c5 = 5;
                    const Class c6 = 6;
                    const Func c7 = 7;
                    const Prototype c8 = 8;
                    const Instance c9 = 9;
                    
                    var Uninitialized d1;
                    var Void d2;
                    var Float d3;
                    var Int d4;
                    var String d5;
                    var Class d6;
                    var Func d7;
                    var Prototype d8;
                    var Instance d9;
                }
            ";

            ExpectedCompilationOutput = @"
                test.d:1:6: error: unknown type name 'Uninitialized'
                const Uninitialized a1 = 1;
                      ^
                test.d:2:6: error: unsupported type
                const Void a2 = 2;
                      ^
                test.d:5:13: error: cannot initialize a constant of type 'string' with an rvalue of type 'int'
                const String a5 = 5;
                             ^    ~
                test.d:6:6: error: unsupported type
                const Class a6 = 6;
                      ^
                test.d:7:11: error: cannot initialize a constant of type 'func' with an rvalue of type 'int'
                const Func a7 = 7;
                           ^    ~
                test.d:8:6: error: unsupported type
                const Prototype a8 = 8;
                      ^
                test.d:9:15: error: cannot initialize a constant of type 'instance' with an rvalue of type 'int'
                const Instance a9 = 9;
                               ^    ~
                test.d:11:4: error: unknown type name 'Uninitialized'
                var Uninitialized b1;
                    ^
                test.d:12:4: error: unsupported type
                var Void b2;
                    ^
                test.d:16:4: error: unsupported type
                var Class b6;
                    ^
                test.d:18:4: error: unsupported type
                var Prototype b8;
                    ^
                test.d: In function 'myFunc':
                test.d:22:10: error: unknown type name 'Uninitialized'
                    const Uninitialized c1 = 1;
                          ^
                test.d:23:10: error: unsupported type
                    const Void c2 = 2;
                          ^
                test.d:26:17: error: cannot initialize a constant of type 'string' with an rvalue of type 'int'
                    const String c5 = 5;
                                 ^    ~
                test.d:27:10: error: unsupported type
                    const Class c6 = 6;
                          ^
                test.d:28:15: error: cannot initialize a constant of type 'func' with an rvalue of type 'int'
                    const Func c7 = 7;
                               ^    ~
                test.d:29:10: error: unsupported type
                    const Prototype c8 = 8;
                          ^
                test.d:30:19: error: cannot initialize a constant of type 'instance' with an rvalue of type 'int'
                    const Instance c9 = 9;
                                   ^    ~
                test.d:32:8: error: unknown type name 'Uninitialized'
                    var Uninitialized d1;
                        ^
                test.d:33:8: error: unsupported type
                    var Void d2;
                        ^
                test.d:37:8: error: unsupported type
                    var Class d6;
                        ^
                test.d:39:8: error: unsupported type
                    var Prototype d8;
                        ^
                22 errors generated.
            ";

            AssertCompilationOutputMatch();
        }
    }
}