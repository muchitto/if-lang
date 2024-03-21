using Compiler.ErrorHandling;
using Compiler.Parsing;
using Compiler.Semantics;
using Compiler.Semantics.SemanticPasses;
using Compiler.Syntax.Visitor;

namespace Compiler;

public static class Compiler
{
    public static void Compile(string filename)
    {
        var sourceCode = File.ReadAllText(filename);

        var position = new PositionData(filename, sourceCode, 0, 0);
        var compilationContext = new CompilationContext(position);
        var parser = new Parser(compilationContext);

        var semanticContext = new SemanticContext();

        var semanticHandler = new SemanticHandler(semanticContext);

        var passes = new List<BaseNodeVisitor>
        {
            new ReorganizeNodesNodeVisitor(),
            new CollectDeclarationsNodeVisitor(semanticContext, semanticHandler),
            new TypeResolutionAndCheckNodeVisitor(semanticContext, semanticHandler),
            new UnknownCheckerVisitor(),
            new NullCheckerNodeVisitor()
        };

        try
        {
            var program = parser.Parse();

            foreach (var pass in passes)
            {
                pass.VisitProgramNode(program);
            }
        }
        catch (CompileError.PositionalError e)
        {
            Console.WriteLine(e.FormatError());
        }
    }
}