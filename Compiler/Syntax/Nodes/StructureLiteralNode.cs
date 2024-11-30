using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class StructureLiteralNode(NodeContext nodeContext, List<StructureLiteralFieldNode> fields)
    : LiteralNode(nodeContext)
{
    public List<StructureLiteralFieldNode> Fields { get; set; } = fields;


    public bool HasField(string name)
    {
        return Fields.Any(f => f.Name.Name == name);
    }

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitStructureLiteralNode(this);
    }
}