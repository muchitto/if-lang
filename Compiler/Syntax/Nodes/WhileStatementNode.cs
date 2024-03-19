using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class WhileStatementNode(NodeContext nodeContext, BaseNode expression, BodyBlockNode body)
    : BaseNode(nodeContext)
{
    public BaseNode Expression { get; } = expression;
    public BodyBlockNode Body { get; } = body;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitWhileNode(this);
    }
}