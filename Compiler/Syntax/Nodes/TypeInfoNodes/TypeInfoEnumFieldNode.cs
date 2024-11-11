using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes.TypeInfoNodes;

public class TypeInfoEnumFieldNode(
    NodeContext nodeContext,
    DeclarationNamedNode named,
    List<TypeInfoEnumFieldParamNode> parameters
)
    : TypeInfoNode(nodeContext)
{
    public DeclarationNamedNode Named { get; } = named;
    public List<TypeInfoEnumFieldParamNode> Parameters { get; } = parameters;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitTypeInfoEnumFieldNode(this);
    }
}