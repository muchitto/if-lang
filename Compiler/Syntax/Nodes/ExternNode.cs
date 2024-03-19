using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public abstract class ExternNode(NodeContext nodeContext, IdentifierNode name) : DeclarationNode(nodeContext, name, [])
{
    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitExternNode(this);
    }
}