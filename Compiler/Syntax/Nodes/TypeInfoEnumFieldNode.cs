using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class TypeInfoEnumFieldNode(
    NodeContext nodeContext,
    DeclarationNameNode name,
    List<TypeInfoEnumFieldParamNode> parameters)
    : TypeInfoNode(nodeContext)
{
    public DeclarationNameNode Name { get; } = name;
    public List<TypeInfoEnumFieldParamNode> Parameters { get; } = parameters;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitTypeInfoEnumFieldNode(this);
    }
}