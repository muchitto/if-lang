using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class FunctionCallArgumentNode(NodeContext nodeContext, BaseNode value) : BaseNode(nodeContext)
{
    public BaseNode Value = value;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitFunctionCallArgumentNode(this);
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}