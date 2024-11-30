using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes.TypeInfoNodes;

public class TypeInfoEnumFieldNode(
    NodeContext nodeContext,
    DeclarationNamedNode named,
    List<TypeInfoEnumFieldParamNode> parameters
)
    : TypeInfoNode(nodeContext)
{
    public DeclarationNamedNode Named { get; set; } = named;
    public List<TypeInfoEnumFieldParamNode> Parameters { get; set; } = parameters;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitTypeInfoEnumFieldNode(this);
    }
}