using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class BodyBlockNode(NodeContext nodeContext, List<BaseNode> statements) : BaseNode(nodeContext)
{
    public List<BaseNode> Statements { get; set; } = statements;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitBodyBlockNode(this);
    }
}