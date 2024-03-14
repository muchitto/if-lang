using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class BodyBlockNode(List<BaseNode> statements) : BaseNode
{
    public List<BaseNode> Statements { get; } = statements;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitBodyBlockNode(this);
    }
}