using Compiler.Syntax.Nodes;
using Compiler.Syntax.Visitor;

namespace Compiler.Semantics.SemanticPasses;

public class ReorganizeNodesNodeVisitor : BaseNodeVisitor
{
    public override BodyBlockNode VisitBodyBlockNode(BodyBlockNode bodyBlockNode)
    {
        var sortedStatements = bodyBlockNode.Statements
            .Select((statement, index) => new { Statement = statement, Index = index })
            .OrderBy(item => item.Statement is DeclarationNode and not VariableDeclarationNode ? 0 : 1)
            .ThenBy(item => item.Index) // Maintain original order for everything else
            .Select(item => item.Statement)
            .ToList();

        var newBodyBlockNode = new BodyBlockNode(bodyBlockNode.NodeContext, sortedStatements);

        return base.VisitBodyBlockNode(newBodyBlockNode);
    }

    public override ObjectDeclarationNode VisitObjectDeclarationNode(ObjectDeclarationNode objectDeclarationNode)
    {
        objectDeclarationNode.Fields.Sort((a, b) =>
            b is VariableDeclarationNode ? 1 : -1);

        return base.VisitObjectDeclarationNode(objectDeclarationNode);
    }

    public override ProgramNode VisitProgramNode(ProgramNode programNode)
    {
        programNode.Declarations.Sort((a, b) =>
            b is VariableDeclarationNode ? -1 : 1);

        return base.VisitProgramNode(programNode);
    }
}