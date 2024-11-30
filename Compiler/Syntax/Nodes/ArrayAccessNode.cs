using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ArrayAccessNode(NodeContext nodeContext, BaseNode array, BaseNode accessor)
    : BaseNode(nodeContext), IEquatable<BaseNode>
{
    public BaseNode Array { get; set; } = array;
    public BaseNode Accessor { get; set; } = accessor;

    public bool Equals(BaseNode? other)
    {
        return other is ArrayAccessNode arrayAccessNode &&
               Array.Equals(arrayAccessNode.Array) &&
               Accessor.Equals(arrayAccessNode.Accessor);
    }

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitArrayAccessNode(this);
    }
}