using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class StringLiteralNode(NodeContext nodeContext, string value) : LiteralNode(nodeContext)
{
    public string Value { get; } = value;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitStringLiteralNode(this);
    }
}