using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class UnaryExpressionNode(NodeContext nodeContext, Operator op, BaseNode value) : BaseNode(nodeContext)
{
    public Operator Operator { get; } = op;
    public BaseNode Value { get; set; } = value;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitUnaryExpressionNode(this);
    }
}