using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class NullLiteralNode(NodeContext nodeContext) : LiteralNode(nodeContext)
{
    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitNullLiteralNode(this);
    }
}