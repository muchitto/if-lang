using Compiler.Syntax.Nodes;
using Compiler.Syntax.Visitor;

namespace Compiler.Semantics.SemanticPasses;

public class ReorganizeNodesNodeVisitor(SemanticHandler semanticHandler) : BaseNodeVisitor(semanticHandler)
{
    public override BodyBlockNode VisitBodyBlockNode(BodyBlockNode bodyBlockNode)
    {
        var sortedStatements = bodyBlockNode.Statements
            .Select((statement, index) => new { Statement = statement, Index = index })
            .OrderByDescending(item => item.Statement is DeclarationNode and not VariableDeclarationNode)
            .ThenBy(item => item.Index) // Maintain original order for everything else
            .Select(item => item.Statement)
            .ToList();

        var newBodyBlockNode = new BodyBlockNode(bodyBlockNode.NodeContext, sortedStatements);

        return base.VisitBodyBlockNode(newBodyBlockNode);
    }

    public override ObjectDeclarationNode VisitObjectDeclarationNode(ObjectDeclarationNode objectDeclarationNode)
    {
        objectDeclarationNode.Fields = objectDeclarationNode.Fields
            .OrderByDescending(a => a is VariableDeclarationNode)
            .ToList();

        return base.VisitObjectDeclarationNode(objectDeclarationNode);
    }

    public override ProgramNode VisitProgramNode(ProgramNode programNode)
    {
        programNode.Declarations = programNode.Declarations
            .OrderByDescending(a => a is VariableDeclarationNode)
            .ToList();

        return base.VisitProgramNode(programNode);
    }
}