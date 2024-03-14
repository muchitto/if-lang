using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class WhileStatementNode(BaseNode expression, BodyBlockNode body) : BaseNode
{
    public BaseNode Expression { get; } = expression;
    public BodyBlockNode Body { get; } = body;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitWhileNode(this);
    }
}