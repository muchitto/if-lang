using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class PropertySetExpressionNode : BaseNode
{
    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitPropertySetExpressionNode(this);
    }
}