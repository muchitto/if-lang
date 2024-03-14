using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class WithExpressionNode : BaseNode
{
    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitWithExpressionNode(this);
    }
}