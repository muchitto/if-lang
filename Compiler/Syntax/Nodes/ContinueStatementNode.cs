using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ContinueStatementNode : BaseNode
{
    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitContinueStatementNode(this);
    }
}