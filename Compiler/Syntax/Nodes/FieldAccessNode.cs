using System.Diagnostics;
using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class FieldAccessNode(NodeContext nodeContext, IdentifierNode baseObjectName, IdentifiableNode member)
    : IdentifierNode(nodeContext, baseObjectName.Name)
{
    public IdentifierNode BaseObjectName { get; set; } = baseObjectName;
    public IdentifiableNode Member { get; set; } = member;

    [StackTraceHidden]
    [DebuggerHidden]
    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitFieldAccessNode(this);
    }

    public override string ToString()
    {
        return BaseObjectName + "." + Member;
    }

    public ObjectFieldAccessNode ToObjectFieldAccessNode()
    {
        return new ObjectFieldAccessNode(
            NodeContext,
            BaseObjectName,
            Member
        );
    }

    public TypeFieldAccessNode ToTypeFieldAccessNode()
    {
        return new TypeFieldAccessNode(
            NodeContext,
            BaseObjectName.ToTypeInfoNameNode(),
            Member
        );
    }
}