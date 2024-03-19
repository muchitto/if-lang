using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class IfStatementNode : BaseNode
{
    public IfStatementNode(NodeContext nodeContext, BaseNode? expression, BodyBlockNode body, IfStatementNode? nextIf)
        : base(nodeContext)
    {
        Expression = expression;
        Body = body;
        NextIf = nextIf;
    }

    public BaseNode? Expression { get; }
    public BodyBlockNode Body { get; }
    public IfStatementNode? NextIf { get; }

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitIfStatementNode(this);
    }
}