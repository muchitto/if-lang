using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class StructureLiteralNode(NodeContext nodeContext, List<StructureLiteralFieldNode> fields)
    : LiteralNode(nodeContext)
{
    public List<StructureLiteralFieldNode> Fields { get; } = fields;


    public bool HasField(string name)
    {
        return Fields.Any(f => f.Name.Name == name);
    }

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitStructureLiteralNode(this);
    }
}