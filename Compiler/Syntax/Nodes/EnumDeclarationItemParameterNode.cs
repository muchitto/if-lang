using Compiler.Syntax.Nodes.TypeInfoNodes;
using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class EnumDeclarationItemParameterNode(
    NodeContext nodeContext,
    DeclarationNamedNode named,
    TypeInfoNode typeInfoNode
) : BaseNode(nodeContext)
{
    public DeclarationNamedNode Named { get; } = named;
    public TypeInfoNode TypeInfoNode { get; } = typeInfoNode;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitEnumDeclarationItemParameterNode(this);
    }
}