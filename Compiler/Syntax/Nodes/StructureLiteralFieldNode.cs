using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class StructureLiteralFieldNode(NodeContext nodeContext, IdentifierNode name, BaseNode field)
    : LiteralNode(nodeContext)
{
    public IdentifierNode Name { get; } = name;
    public BaseNode Field { get; } = field;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitStructureLiteralFieldNode(this);
    }
}