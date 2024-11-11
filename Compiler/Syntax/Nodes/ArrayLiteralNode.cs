using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ArrayLiteralNode(NodeContext nodeContext, List<BaseNode> elements)
    : LiteralNode(nodeContext), IEquatable<BaseNode>
{
    public List<BaseNode> Elements { get; } = elements;

    public bool Equals(BaseNode? other)
    {
        return other is ArrayLiteralNode arrayLiteralNode &&
               Elements.SequenceEqual(arrayLiteralNode.Elements);
    }

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitArrayLiteralNode(this);
    }
}