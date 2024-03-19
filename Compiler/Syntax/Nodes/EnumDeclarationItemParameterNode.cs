using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class EnumDeclarationItemParameterNode(
    NodeContext nodeContext,
    IdentifierNode name,
    TypeInfoNode typeInfoNode) : BaseNode(nodeContext)
{
    public IdentifierNode Name { get; } = name;
    public TypeInfoNode TypeInfoNode { get; } = typeInfoNode;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitEnumDeclarationItemParameterNode(this);
    }
}