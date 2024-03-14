using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class FunctionExpressionNode : BaseNode
{
    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitFunctionExpressionNode(this);
    }
}