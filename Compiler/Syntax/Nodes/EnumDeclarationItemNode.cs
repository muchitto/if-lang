using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class EnumDeclarationItemNode(
    NodeContext nodeContext,
    DeclarationNameNode name,
    List<EnumDeclarationItemParameterNode> parameterNodes) : BaseNode(nodeContext)
{
    public DeclarationNameNode Name { get; } = name;
    public List<EnumDeclarationItemParameterNode> ParameterNodes { get; } = parameterNodes;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitEnumDeclarationItemNode(this);
    }
}