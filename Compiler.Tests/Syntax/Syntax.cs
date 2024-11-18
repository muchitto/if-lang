using Compiler.Syntax.Nodes;
using Compiler.Syntax.Nodes.TypeInfoNodes;

namespace Compiler.Tests.Syntax;

public class Syntax : CompilationTest
{
    [Fact]
    public void ShouldParseVariableDeclarationIntoSyntaxTree()
    {
        var source = @"
            var a: Int = 10
        ";

        var parser = CreateParser("ShouldParseVariableDeclarationIntoSyntaxTree", source);

        try
        {
            var program = parser.Parse();

            Assert.Single(program.Declarations);

            if (program.Declarations[0] is VariableDeclarationNode variableDeclaration)
            {
                Assert.Equal("a", variableDeclaration.Named.Name);
                Assert.True(variableDeclaration.TypeInfoNode is TypeInfoNameNode { Name: "Int" });
                Assert.True(variableDeclaration.Value is NumberLiteralNode { Number: "10" });
            }
            else
            {
                Assert.Fail("Expected VariableDeclarationNode");
            }
        }
        catch (Exception e)
        {
            Assert.Fail(e.Message);
        }
    }
}