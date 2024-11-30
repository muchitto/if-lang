using Compiler.ErrorHandling;
using Compiler.Parsing;
using Compiler.Semantics;
using Compiler.Semantics.SemanticPasses;

namespace Compiler.Tests;

public abstract class CompilationTest
{
    protected CompilationContext CreateCompilationContext(string testFilename, string source)
    {
        return new CompilationContext(new PositionData(testFilename, source, 0, 0));
    }

    protected Parser CreateParser(string testName, string source)
    {
        var compilationContext = CreateCompilationContext(testName, source);

        return new Parser(compilationContext);
    }

    protected void RunSemanticTest(string testName, string source)
    {
        var program = Parser.Parse("TypeCrossReferencesInVariables", source);

        SemanticHelperBaseNodeVisitor.RunDefaultPasses(program, new SemanticContext());
    }
}