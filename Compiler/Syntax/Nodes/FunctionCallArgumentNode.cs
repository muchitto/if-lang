using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class FunctionCallArgumentNode(BaseNode value) : BaseNode
{
    public BaseNode Value = value;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitFunctionCallArgumentNode(this);
    }
}