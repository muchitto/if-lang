using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class BodyBlockNode(NodeContext nodeContext, List<BaseNode> statements) : BaseNode(nodeContext)
{
    public List<BaseNode> Statements { get; } = statements;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitBodyBlockNode(this);
    }
}