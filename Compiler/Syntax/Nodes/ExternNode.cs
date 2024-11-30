using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public abstract class ExternNode(NodeContext nodeContext, DeclarationNamedNode named)
    : DeclarationNode(nodeContext, named, []), IEquatable<BaseNode>
{
    public override bool Equals(BaseNode? other)
    {
        if (!base.Equals(other))
        {
            return false;
        }

        return other is ExternNode;
    }

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitExternNode(this);
    }
}