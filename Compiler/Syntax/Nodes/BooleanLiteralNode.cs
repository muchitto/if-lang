using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class BooleanLiteralNode(bool value) : LiteralNode
{
    public bool Value { get; } = value;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitBooleanLiteralNode(this);
    }
}