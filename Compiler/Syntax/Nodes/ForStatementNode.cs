using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ForStatementNode(NodeContext nodeContext, BaseNode iteratable, IdentifierNode value, BodyBlockNode body)
    : BaseNode(nodeContext)
{
    public BaseNode Iteratable { get; set; } = iteratable;
    public IdentifierNode Value { get; set; } = value;
    public BodyBlockNode Body { get; set; } = body;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitForStatementNode(this);
    }
}