using Compiler.Parsing;
using Compiler.Semantics;
using Compiler.Semantics.SemanticPasses;

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

        try
        {
            var program = Parser.Parse("TypeCrossReferencesInVariables", source);

            SemanticHelperBaseNodeVisitor.RunDefaultPasses(program, new SemanticContext());
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
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

        var program = Parser.Parse("TypeCrossReferencesInFunctionReturns", source);

        SemanticHelperBaseNodeVisitor.RunDefaultPasses(program, new SemanticContext());
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

        var program = Parser.Parse("TypeCrossReferencesInFunctionParameters", source);

        SemanticHelperBaseNodeVisitor.RunDefaultPasses(program, new SemanticContext());
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

        var program = Parser.Parse("TypeCrossReferencesAndAccess", source);

        SemanticHelperBaseNodeVisitor.RunDefaultPasses(program, new SemanticContext());
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
            var h : float32 = 0
            var i : float64 = 0
            var j : bool = false
        ";

        var program = Parser.Parse("UsingBaseTypes", source);

        SemanticHelperBaseNodeVisitor.RunDefaultPasses(program, new SemanticContext());
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
        
        var program = Parser.Parse("UsingBaseTypesAsArrayTypes", source);

        SemanticHelperBaseNodeVisitor.RunDefaultPasses(program, new SemanticContext());
    }
}