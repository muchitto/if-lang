using Compiler.Syntax.Nodes;
using Compiler.Syntax.Visitor;

namespace Compiler.Semantics.SemanticPasses;

public class TypePromotionNodeVisitor(SemanticContext semanticContext) : BaseNodeVisitor(semanticContext)
{
    public override VariableDeclarationNode VisitVariableDeclarationNode(
        VariableDeclarationNode variableDeclarationNode
    )
    {
        return base.VisitVariableDeclarationNode(variableDeclarationNode);
    }

    public override AssignmentNode VisitAssignmentNode(AssignmentNode assignmentNode)
    {
        return base.VisitAssignmentNode(assignmentNode);
    }
}