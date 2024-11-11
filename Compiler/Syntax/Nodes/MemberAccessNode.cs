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
    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitMemberAccessNode(this);
    }
}