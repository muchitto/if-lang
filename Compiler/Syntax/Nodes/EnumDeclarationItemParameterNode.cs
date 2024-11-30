using Compiler.Syntax.Nodes.TypeInfoNodes;
using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class EnumDeclarationItemParameterNode(
    NodeContext nodeContext,
    DeclarationNamedNode named,
    TypeInfoNode typeInfoNode
) : BaseNode(nodeContext)
{
    public DeclarationNamedNode Named { get; set; } = named;
    public TypeInfoNode TypeInfoNode { get; set; } = typeInfoNode;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitEnumDeclarationItemParameterNode(this);
    }
}