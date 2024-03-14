using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class FunctionCallNode(IdentifierNode name, List<FunctionCallArgumentNode> arguments) : BaseNode
{
    public List<FunctionCallArgumentNode> Arguments = arguments;
    public IdentifierNode Name = name;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitFunctionCallNode(this);
    }
}