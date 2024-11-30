using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class BooleanLiteralNode(NodeContext nodeContext, bool value) : LiteralNode(nodeContext)
{
    public bool Value { get; } = value;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitBooleanLiteralNode(this);
    }
}