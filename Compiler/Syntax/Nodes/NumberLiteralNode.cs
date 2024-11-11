using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class NumberLiteralNode(NodeContext nodeContext, string number) : LiteralNode(nodeContext)
{
    public string Number { get; } = number;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitNumberLiteralNode(this);
    }

    public bool IsFloat()
    {
        return Number.Contains('.');
    }
}