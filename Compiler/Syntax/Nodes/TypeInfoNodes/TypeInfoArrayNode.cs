using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes.TypeInfoNodes;

public class TypeInfoArrayNode(
    NodeContext nodeContext,
    TypeInfoNode baseType,
    List<TypeInfoNode> genericParameters
)
    : TypeInfoNode(nodeContext)
{
    public List<TypeInfoNode> GenericParameters { get; } = genericParameters;

    public TypeInfoNode BaseType { get; set; } = baseType;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitTypeInfoArrayNode(this);
    }
}