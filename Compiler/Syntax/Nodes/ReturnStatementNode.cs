using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ReturnStatementNode(NodeContext nodeContext, BaseNode? value) : BaseNode(nodeContext)
{
    public BaseNode? Value { get; } = value;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitReturnStatementNode(this);
    }
}