using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ContinueStatementNode(NodeContext nodeContext) : BaseNode(nodeContext)
{
    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitContinueStatementNode(this);
    }
}