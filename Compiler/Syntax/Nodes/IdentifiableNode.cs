using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public abstract class IdentifiableNode(NodeContext nodeContext) : BaseNode(nodeContext)
{
    public abstract string GetName();

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitIdentifiableNode(this);
    }
}