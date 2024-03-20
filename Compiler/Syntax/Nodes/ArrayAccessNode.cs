using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ArrayAccessNode(NodeContext nodeContext, BaseNode array, BaseNode accessor) : BaseNode(nodeContext)
{
    public BaseNode Array { get; } = array;
    public BaseNode Accessor { get; } = accessor;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitArrayAccessNode(this);
    }
}