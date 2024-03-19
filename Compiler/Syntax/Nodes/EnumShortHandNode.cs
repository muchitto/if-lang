using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class EnumShortHandNode(NodeContext nodeContext, IdentifierNode name) : BaseNode(nodeContext)
{
    public IdentifierNode Name { get; } = name;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitEnumShortHandNode(this);
    }
}