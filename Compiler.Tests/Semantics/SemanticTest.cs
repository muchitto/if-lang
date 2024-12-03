using Compiler.ErrorHandling;

namespace Compiler.Tests.Semantics;

public class SemanticTest : CompilationTest
{
    [Fact]
    public void TypeCrossReferencesInVariables()
    {
        var source = @"
            class A {
                var b: B
            }

            class B {
                var a: A
            }
        ";

        RunSemanticTest(source);
    }

    [Fact]
    public void TypeCrossReferencesInFunctionReturns()
    {
        var source = @"
            class A {
                func b() -> B {
                    return B()
                }
            }

            class B {
                func a() -> A {
                    return A()
                }
            }
        ";

        RunSemanticTest(source);
    }

    [Fact]
    public void TypeCrossReferencesInFunctionParameters()
    {
        var source = @"
            class A {
                func b(b: B) {
                }
            }

            class B {
                func a(a: A) {
                }
            }
        ";


        RunSemanticTest(source);
    }

    [Fact]
    public void TypeCrossReferencesAndAccess()
    {
        var source = @"
            class A {
                func testA(b : B) {
                    b.i = 0
                }
            }

            class B {
                var i = 0
                func testB(a : A) {
                    a.testA(this)
                }
            }
        ";

        RunSemanticTest(source);
    }

    [Fact]
    public void UsingBaseTypes()
    {
        var source = @"
            var a : int = 0
            var b : int32 = 0
            var c : int64 = 0
            var d : uint16 = 0
            var e : uint32 = 0
            var f : uint64 = 0
            var g : float = 0
            var h : float32 = 0.0
            var i : float64 = 0
            var j : bool = false
        ";

        RunSemanticTest(source);
    }


    [Fact]
    public void UsingBaseTypesAsArrayTypes()
    {
        var source = @"
            var a : int[] = []
            var b : int32[] = []
            var c : int64[] = [] 
            var d : uint16[] = []
            var e : uint32[] = []
            var f : uint64[] = []
            var g : float[] = []
            var h : float32[] = []
            var i : float64[] = []
            var j : bool[] = []
        ";

        RunSemanticTest(source);
    }

    [Fact]
    public void UsingBaseTypeAsArrayTypeWithValueInferred()
    {
        var source = @"
            var a = [10]
        ";

        RunSemanticTest(source);
    }

    [Fact]
    public void UsingAnotherVariableToInferTheTypeOfAnother()
    {
        var source = @"
            var a = 1
            var b = a
        ";

        RunSemanticTest(source);
    }

    [Fact]
    public void AccessingArrayAndInferringTheType()
    {
        var source = @"
            var a = [10]
            var p = a[0]
        ";

        RunSemanticTest(source);
    }

    [Fact]
    public void FunctionCallWithAVariable()
    {
        var source = @"
            var a = 10
            
            func test(h : int) -> int {
                return h * h
            }

            var b = test(a)

            test(b)
        ";

        RunSemanticTest(source);
    }

    [Fact]
    public void ShouldNotMixGeneratedMainFunctionAndAMainFunction()
    {
        var source = @"
            func test() {
                var hah = 10
            }

            test()

            func main() {
                test()
            }
        ";

        try
        {
            RunSemanticTest(source);
        }
        catch (Exception e)
        {
            Assert.True(true);

            return;
        }

        Assert.False(true);
    }

    [Fact]
    public void CreateAnInlineEnumAndUseIt()
    {
        var source = @"
            var enumVar : On | Off = .On
            enumVar = .Off 
            enumVar = .On
        ";

        RunSemanticTest(source);
    }

    [Fact]
    public void CreateAnEnumDeclarationAndUseIt()
    {
        var source = @"
            enum Switch {
                On,
                Off
            }

            var enumVar : Switch = .On
            enumVar = .Off 
            enumVar = .On
            enumVar = Switch.Off
        ";

        RunSemanticTest(source);
    }

    [Fact]
    public void UsingEnumAndCheckingItInAnIf()
    {
        var source = @"
            enum Switch {
                On,
                Off
            }

            func test() {
            }

            var enumVar : Switch = .On
            var enumVar2 : Yes | No = .No
            
            if enumVar == .On {
                test()   
            }

            if enumVar2 == .Yes {
                test()
            }
        ";

        RunSemanticTest(source);
    }


    [Fact]
    public void UsingWrongEnumInlineAndDeclaration()
    {
        var source = @"
            enum Switch {
                On,
                Off
            }

            var enumVar : Switch = .O
            var enumVar2 : Yes | No = .No
            var enumVar3 : Switch = .On

            enumVar = .Off
            enumVar2 = .Noo
            enumVar3 = .Off
        ";

        try
        {
            RunSemanticTest(source);
        }
        catch (CompileError.SemanticError error)
        {
            return;
        }

        Assert.Fail("should have failed");
    }

    [Fact]
    public void UsingEnumInlineWithArguments()
    {
        var source = @"
            var enumVar : Thing(param : string) | No = .Thing(""Test"")
            
            enumVar = .Thing(""no"")
            enumVar = .No
        ";

        RunSemanticTest(source);
    }

    [Fact]
    public void UsingEnumDeclarationWithArguments()
    {
        var source = @"
            enum Switch {
                Thing(param : string),
                No
            }

            var enumVar : Switch = .Thing(""Test"")
            enumVar = .Thing(""no"")
            enumVar = .No 
        ";
    }

    [Fact]
    public void UsingEnumInlineWithWrongTypeOfArguments()
    {
        var source = @"
            var enumVar : Thing(param : string) | No = .No
            enumVar = .Thing(10)
        ";

        try
        {
            RunSemanticTest(source);
        }
        catch (CompileError.SemanticError)
        {
            return;
        }

        Assert.Fail();
    }


    [Fact]
    public void UsingEnumDeclarationWithWrongTypeOfArguments()
    {
        var source = @"
            enum Switch {
                Thing(param : string),
                No
            }

            var enumVar : Thing(param : string) | No = .No
            enumVar = .Thing(10)
        ";

        try
        {
            RunSemanticTest(source);
        }
        catch (CompileError.SemanticError)
        {
            return;
        }

        Assert.Fail();
    }
}