using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class EnumShortHandNode(NodeContext nodeContext, DeclarationNameNode name, List<BaseNode> parameters)
    : BaseNode(nodeContext)
{
    public DeclarationNameNode Name { get; } = name;

    public List<BaseNode> Parameters { get; } = parameters;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitEnumShortHandNode(this);
    }
}