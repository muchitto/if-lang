using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ForStatementNode(NodeContext nodeContext, BaseNode iteratable, IdentifierNode value, BodyBlockNode body)
    : BaseNode(nodeContext)
{
    public BaseNode Iteratable { get; } = iteratable;
    public IdentifierNode Value { get; } = value;
    public BodyBlockNode Body { get; } = body;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitForStatementNode(this);
    }
}