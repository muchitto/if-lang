using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class EnumShortHandNode(NodeContext nodeContext, DeclarationNamedNode named, List<BaseNode> parameters)
    : BaseNode(nodeContext)
{
    public DeclarationNamedNode Named { get; set; } = named;

    public List<BaseNode> Parameters { get; set; } = parameters;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitEnumShortHandNode(this);
    }
}