using System.Diagnostics;
using Compiler.Semantics.ScopeHandling;
using Compiler.Semantics.TypeInformation;
using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public abstract class BaseNode(NodeContext nodeContext)
{
    public TypeRef TypeRef { get; set; } = new(TypeInfo.Unknown);

    public Scope? Scope { get; set; }

    public NodeContext NodeContext { get; } = nodeContext;

    public virtual TypeRef ReturnedTypeRef => TypeRef;

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual void Accept(INodeVisitor nodeVisitor)
    {
        throw new NotImplementedException();
    }
}