using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes.TypeInfoNodes;

public class TypeInfoEnumFieldParamNode(
    NodeContext nodeContext,
    DeclarationNamedNode named,
    TypeInfoNode typeInfoNode
)
    : TypeInfoNode(nodeContext)
{
    public DeclarationNamedNode Named { get; } = named;
    public TypeInfoNode TypeInfoNode { get; } = typeInfoNode;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitTypeInfoEnumFieldParamNode(this);
    }
}