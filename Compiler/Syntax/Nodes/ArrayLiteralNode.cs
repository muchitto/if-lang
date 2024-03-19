using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ArrayLiteralNode(NodeContext nodeContext, List<BaseNode> elements) : LiteralNode(nodeContext)
{
    public List<BaseNode> Elements { get; } = elements;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitArrayLiteralNode(this);
    }
}