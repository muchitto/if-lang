using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ExpressionNode(NodeContext nodeContext, BaseNode left, BaseNode right, Operator op)
    : BaseNode(nodeContext)
{
    public BaseNode Left = left;
    public Operator Operator = op;
    public BaseNode Right = right;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitExpressionNode(this);
    }
}