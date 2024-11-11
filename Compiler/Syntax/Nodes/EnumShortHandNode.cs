using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class EnumShortHandNode(NodeContext nodeContext, DeclarationNamedNode named, List<BaseNode> parameters)
    : BaseNode(nodeContext)
{
    public DeclarationNamedNode Named { get; } = named;

    public List<BaseNode> Parameters { get; } = parameters;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitEnumShortHandNode(this);
    }
}