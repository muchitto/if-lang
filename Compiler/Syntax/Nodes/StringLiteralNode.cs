using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class StringLiteralNode(string value) : LiteralNode
{
    public string Value { get; } = value;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitStringLiteralNode(this);
    }
}