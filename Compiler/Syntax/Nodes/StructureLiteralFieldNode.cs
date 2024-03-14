using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class StructureLiteralFieldNode(IdentifierNode name, BaseNode field) : LiteralNode
{
    public IdentifierNode Name { get; } = name;
    public BaseNode Field { get; } = field;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitStructureLiteralFieldNode(this);
    }
}