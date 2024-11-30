using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class NumberLiteralNode(NodeContext nodeContext, string number) : LiteralNode(nodeContext)
{
    public string Number { get; } = number;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitNumberLiteralNode(this);
    }

    public bool IsFloat()
    {
        return Number.Contains('.');
    }
}