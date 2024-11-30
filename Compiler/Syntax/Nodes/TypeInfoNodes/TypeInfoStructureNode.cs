using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes.TypeInfoNodes;

public class TypeInfoStructureNode(
    NodeContext nodeContext,
    List<TypeInfoStructureField> fields
)
    : TypeInfoNode(nodeContext)
{
    public List<TypeInfoStructureField> Fields { get; set; } = fields;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitTypeInfoStructureNode(this);
    }
}