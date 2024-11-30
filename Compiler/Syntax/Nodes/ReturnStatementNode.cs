using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ReturnStatementNode(NodeContext nodeContext, BaseNode? value) : BaseNode(nodeContext)
{
    public BaseNode? Value { get; set; } = value;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitReturnStatementNode(this);
    }
}