using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class StructureLiteralFieldNode(NodeContext nodeContext, IdentifierNode name, BaseNode field)
    : LiteralNode(nodeContext)
{
    public IdentifierNode Name { get; set; } = name;
    public BaseNode Field { get; set; } = field;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitStructureLiteralFieldNode(this);
    }
}