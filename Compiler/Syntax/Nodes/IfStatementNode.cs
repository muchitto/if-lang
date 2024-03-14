using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class IfStatementNode(BaseNode? expression, BodyBlockNode body, IfStatementNode? nextIf) : BaseNode
{
    public BaseNode? Expression { get; } = expression;
    public BodyBlockNode Body { get; } = body;
    public IfStatementNode? NextIf { get; } = nextIf;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitIfStatementNode(this);
    }
}