using Compiler.Lexing;
using Compiler.Parsing;
using Compiler.ScopeHandler;
using Compiler.SemanticPasses;
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
        var program = parser.Parse();
        var semanticContext = new SemanticContext();

        var semanticHandler = new SemanticHandler(semanticContext);

        var passes = new List<BaseNodeVisitor>
        {
            new ReorganizeNodesNodeVisitor(),
            new CollectDeclarationsNodeVisitor(semanticContext, semanticHandler),
            new TypeResolutionAndCheckNodeVisitor(semanticContext, semanticHandler),
            new UnknownCheckerVisitor()
        };

        foreach (var pass in passes)
        {
            pass.VisitProgramNode(program);
        }
    }
}