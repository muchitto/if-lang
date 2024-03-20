using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public abstract class ExternNode(NodeContext nodeContext, DeclarationNameNode name)
    : DeclarationNode(nodeContext, name, [])
{
    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitExternNode(this);
    }
}