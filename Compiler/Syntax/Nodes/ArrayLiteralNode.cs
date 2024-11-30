using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ArrayLiteralNode(NodeContext nodeContext, List<BaseNode> elements)
    : LiteralNode(nodeContext), IEquatable<BaseNode>
{
    public List<BaseNode> Elements { get; set; } = elements;

    public bool Equals(BaseNode? other)
    {
        return other is ArrayLiteralNode arrayLiteralNode &&
               Elements.SequenceEqual(arrayLiteralNode.Elements);
    }

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitArrayLiteralNode(this);
    }
}