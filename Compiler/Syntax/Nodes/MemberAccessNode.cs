using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class MemberAccessNode(IdentifierNode baseObject, IdentifierNode member) : IdentifierNode(baseObject.Name)
{
    public IdentifierNode BaseObject { get; set; } = baseObject;
    public IdentifierNode Member { get; set; } = member;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitMemberAccessNode(this);
    }
}