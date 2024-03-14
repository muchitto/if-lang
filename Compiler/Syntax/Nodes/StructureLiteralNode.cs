using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class StructureLiteralNode(List<StructureLiteralFieldNode> fields) : LiteralNode
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