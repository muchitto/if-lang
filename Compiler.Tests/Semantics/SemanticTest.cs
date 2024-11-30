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

        RunSemanticTest("TypeCrossReferencesInVariables", source);
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

        RunSemanticTest("TypeCrossReferencesInFunctionReturns", source);
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

        RunSemanticTest("TypeCrossReferencesInFunctionParameters", source);
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

        RunSemanticTest("TypeCrossReferencesAndAccess", source);
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

        RunSemanticTest("UsingBaseTypes", source);
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

        RunSemanticTest("UsingBaseTypesAsArrayTypes", source);
    }

    [Fact]
    public void UsingBaseTypeAsArrayTypeWithValueInferred()
    {
        var source = @"
            var a = [10]
        ";

        RunSemanticTest("UsingBaseTypeAsArrayTypeWithValueInferred", source);
    }

    [Fact]
    public void UsingAnotherVariableToInferTheTypeOfAnother()
    {
        var source = @"
            var a = 1
            var b = a
        ";

        RunSemanticTest("UsingAnotherVariableToInferTheTypeOfAnother", source);
    }

    [Fact]
    public void AccessingArrayAndInferringTheType()
    {
        var source = @"
            var a = [10]
            var p = a[0]
        ";

        RunSemanticTest("AccessingArrayAndInferringTheType", source);
    }
}