using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public abstract class IdentifiableNode(NodeContext nodeContext) : BaseNode(nodeContext)
{
    public abstract string GetName();

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitIdentifiableNode(this);
    }
}