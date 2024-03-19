using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class MemberAccessNode(NodeContext nodeContext, IdentifierNode baseObject, BaseNode member)
    : IdentifierNode(nodeContext, baseObject.Name)
{
    public IdentifierNode BaseObject { get; set; } = baseObject;
    public BaseNode Member { get; set; } = member;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitMemberAccessNode(this);
    }
}