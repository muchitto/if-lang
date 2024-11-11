using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class EnumDeclarationItemNode(
    NodeContext nodeContext,
    DeclarationNamedNode named,
    List<EnumDeclarationItemParameterNode> parameterNodes) : BaseNode(nodeContext)
{
    public DeclarationNamedNode Named { get; } = named;
    public List<EnumDeclarationItemParameterNode> ParameterNodes { get; } = parameterNodes;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitEnumDeclarationItemNode(this);
    }
}