using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ContinueStatementNode(NodeContext nodeContext) : BaseNode(nodeContext)
{
    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitContinueStatementNode(this);
    }
}