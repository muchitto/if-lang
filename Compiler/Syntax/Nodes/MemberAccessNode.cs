using System.Diagnostics;
using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class MemberAccessNode(NodeContext nodeContext, IdentifierNode baseObject, IdentifiableNode member)
    : IdentifierNode(nodeContext, baseObject.Name)
{
    public IdentifierNode BaseObject { get; set; } = baseObject;
    public IdentifiableNode Member { get; set; } = member;

    [StackTraceHidden]
    [DebuggerHidden]
    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitMemberAccessNode(this);
    }

    public override string ToString()
    {
        return BaseObject + "." + Member;
    }
}