using Compiler.Parsing;
using Compiler.Semantics;
using Compiler.Semantics.SemanticPasses;

namespace Compiler;

public static class Compiler
{
    public static void Compile(string filename)
    {
        var sourceCode = File.ReadAllText(filename);
        var program = Parser.Parse(filename, sourceCode);

        SemanticHelperBaseNodeVisitor.RunDefaultPasses(new SemanticContext(), program);
    }
}