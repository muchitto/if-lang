using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes.TypeInfoNodes;

public class TypeInfoEnumFieldParamNode(
    NodeContext nodeContext,
    DeclarationNamedNode named,
    TypeInfoNode typeInfoNode
)
    : TypeInfoNode(nodeContext)
{
    public DeclarationNamedNode Named { get; set; } = named;
    public TypeInfoNode TypeInfoNode { get; set; } = typeInfoNode;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitTypeInfoEnumFieldParamNode(this);
    }
}