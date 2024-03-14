using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ReturnStatementNode(BaseNode? value) : BaseNode
{
    public BaseNode? Value { get; } = value;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitReturnStatementNode(this);
    }
}