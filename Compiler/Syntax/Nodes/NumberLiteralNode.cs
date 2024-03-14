using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class NumberLiteralNode(string number) : LiteralNode
{
    public string Number { get; } = number;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitNumberNode(this);
    }
}