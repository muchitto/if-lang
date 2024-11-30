using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class IfStatementNode(NodeContext nodeContext, BaseNode? expression, BodyBlockNode body, IfStatementNode? nextIf)
    : BaseNode(nodeContext), IEquatable<BaseNode>
{
    public BaseNode? Expression { get; set; } = expression;
    public BodyBlockNode Body { get; set; } = body;
    public IfStatementNode? NextIf { get; set; } = nextIf;

    public bool Equals(BaseNode? other)
    {
        return other is IfStatementNode ifStatementNode &&
               Equals(Expression, ifStatementNode.Expression) &&
               Body.Equals(ifStatementNode.Body) &&
               Equals(NextIf, ifStatementNode.NextIf);
    }

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitIfStatementNode(this);
    }
}