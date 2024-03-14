using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class FlagExpressionNode : BaseNode
{
    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitFlagExpressionNode(this);
    }
}