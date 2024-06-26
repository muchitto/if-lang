using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class EnumDeclarationItemParameterNode(
    NodeContext nodeContext,
    DeclarationNameNode name,
    TypeInfoNode typeInfoNode) : BaseNode(nodeContext)
{
    public DeclarationNameNode Name { get; } = name;
    public TypeInfoNode TypeInfoNode { get; } = typeInfoNode;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitEnumDeclarationItemParameterNode(this);
    }
}