using System.Diagnostics;
using Compiler.Semantics.TypeInformation;
using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public abstract class BaseNode(NodeContext nodeContext)
{
    private TypeRef _typeRef = new(TypeInfo.Unknown);

    public Scope? Scope { get; set; }

    public TypeRef TypeRef
    {
        get => _typeRef;
        set => SetTypeRef(value);
    }

    public NodeContext NodeContext { get; } = nodeContext;

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual void Accept(INodeVisitor nodeVisitor)
    {
        throw new NotImplementedException();
    }

    public virtual void SetTypeRef(TypeRef typeRef)
    {
        _typeRef = typeRef;
    }
}