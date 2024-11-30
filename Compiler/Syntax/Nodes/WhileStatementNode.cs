using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class WhileStatementNode(NodeContext nodeContext, BaseNode expression, BodyBlockNode body)
    : BaseNode(nodeContext)
{
    public BaseNode Expression { get; set; } = expression;
    public BodyBlockNode Body { get; set; } = body;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitWhileNode(this);
    }
}