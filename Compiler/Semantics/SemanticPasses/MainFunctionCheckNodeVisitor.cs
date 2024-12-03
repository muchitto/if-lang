using Compiler.ErrorHandling;
using Compiler.Semantics.ScopeHandling;
using Compiler.Syntax.Nodes;

namespace Compiler.Semantics.SemanticPasses;

public class MainFunctionCheckNodeVisitor(SemanticHandler semanticHandler)
    : SemanticPassBaseNodeVisitor(semanticHandler, new DoNothingScopeHandler(semanticHandler))
{
    public override ProgramNode VisitProgramNode(ProgramNode programNode)
    {
        if (!programNode.Declarations.Any(d => d is FunctionDeclarationNode { Named.Name: "main" }))
        {
            programNode.Declarations.Add(new FunctionDeclarationNode(
                programNode.NodeContext,
                new DeclarationNamedNode(programNode.NodeContext, "main", []),
                [],
                null,
                new BodyBlockNode(
                    programNode.NodeContext,
                    programNode.GeneratedMainFunctionStatements
                ),
                []
            ));

            return programNode;
        }

        if (programNode.GeneratedMainFunctionStatements.Count != 0)
        {
            throw new CompileError.SemanticError(
                "you have a main function but also having statements outside of main function, which means you are using a generated main function, which is not allowed",
                programNode
            );
        }

        return programNode;
    }
}