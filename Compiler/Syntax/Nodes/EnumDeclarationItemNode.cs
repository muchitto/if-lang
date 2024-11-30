using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class EnumDeclarationItemNode(
    NodeContext nodeContext,
    DeclarationNamedNode named,
    List<EnumDeclarationItemParameterNode> parameterNodes
) : BaseNode(nodeContext)
{
    public DeclarationNamedNode Named { get; set; } = named;
    public List<EnumDeclarationItemParameterNode> ParameterNodes { get; set; } = parameterNodes;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitEnumDeclarationItemNode(this);
    }
}