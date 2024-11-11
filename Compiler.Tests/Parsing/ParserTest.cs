namespace Compiler.Tests.Parsing;

public class ParserTest : CompilationTest
{
    [Fact]
    public void ShouldParseGlobalFunction()
    {
        var source = @"
         func add(a: Int, b: Int) -> Int {
            return a + b
           }
        ";

        var parser = CreateParser("ShouldParseGlobalFunction", source);

        try
        {
            var program = parser.Parse();
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public void ShouldParseGlobalVariable()
    {
        var source = @"
            var a: Int = 10
            var a = 10
            var a: Int
        ";

        var parser = CreateParser("ShouldParseGlobalVariable", source);

        try
        {
            var program = parser.Parse();

            Assert.Equal(3, program.Declarations.Count);
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public void ShouldParseGlobalObject()
    {
        var source = @"
            class A {
            }
        ";

        var parser = CreateParser("ShouldParseGlobalObject", source);

        try
        {
            parser.Parse();
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public void ShouldParseGlobalObjectWithFunction()
    {
        var source = @"
            class A {
                func a() {
                }
            }
        ";

        var parser = CreateParser("ShouldParseGlobalObjectWithFunction", source);

        try
        {
            parser.Parse();
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public void ShouldParseGlobalObjectWithVariable()
    {
        var source = @"
            class A {
                var a: Int = 10
            }
        ";

        var parser = CreateParser("ShouldParseGlobalObjectWithVariable", source);

        try
        {
            parser.Parse();
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }

    [Fact]
    public void ShouldParseGlobalObjectWithVariableAndFunction()
    {
        var source = @"
            class A {
                var a: Int = 10

                func b() {
                }
            }
        ";

        var parser = CreateParser("ShouldParseGlobalObjectWithVariableAndFunction", source);

        try
        {
            parser.Parse();
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }
}