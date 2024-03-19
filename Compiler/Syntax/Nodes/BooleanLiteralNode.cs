using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class BooleanLiteralNode(NodeContext nodeContext, bool value) : LiteralNode(nodeContext)
{
    public bool Value { get; } = value;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitBooleanLiteralNode(this);
    }
}