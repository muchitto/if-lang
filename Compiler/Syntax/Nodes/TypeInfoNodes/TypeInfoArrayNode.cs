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

    public TypeInfoNode BaseType { get; } = baseType;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitTypeInfoArrayNode(this);
    }
}