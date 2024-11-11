using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class NullLiteralNode(NodeContext nodeContext) : LiteralNode(nodeContext)
{
    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitNullLiteralNode(this);
    }
}