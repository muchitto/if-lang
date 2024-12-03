using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ObjectFieldAccessNode(NodeContext nodeContext, IdentifierNode baseObjectName, IdentifiableNode member)
    : FieldAccessNode(nodeContext, baseObjectName, member)
{
    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitObjectFieldAccessNode(this);
    }
}