using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ArrayLiteralNode(List<BaseNode> elements) : LiteralNode
{
    public List<BaseNode> Elements { get; } = elements;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitArrayLiteralNode(this);
    }
}