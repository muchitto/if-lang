using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class FunctionCallArgumentNode(NodeContext nodeContext, BaseNode value) : BaseNode(nodeContext)
{
    public BaseNode Value = value;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitFunctionCallArgumentNode(this);
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}