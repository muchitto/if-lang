using System.Diagnostics;
using System.Diagnostics.Contracts;
using Compiler.ErrorHandling;
using Compiler.Semantics.ScopeHandling;
using Compiler.Semantics.TypeInformation;
using Compiler.Syntax.Nodes.TypeInfoNodes;
using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public abstract class BaseNode
{
    private TypeRef _typeRef;

    protected BaseNode(NodeContext nodeContext)
    {
        NodeContext = nodeContext;
        TypeRef = new TypeRef(TypeInfo.Unknown);
    }

    public TypeRef TypeRef
    {
        get => _typeRef;
        set => SetTypeRef(value);
    }

    public Scope? Scope { get; set; }

    public NodeContext NodeContext { get; }

    public virtual TypeRef ReturnedTypeRef => TypeRef;

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual BaseNode Accept(INodeVisitor nodeVisitor)
    {
        throw new NotImplementedException();
    }

    protected virtual void SetTypeRef(TypeRef typeRef)
    {
        _typeRef = typeRef;
    }


    [Pure]
    public virtual TypeCastNode CastValue(TypeInfo from, TypeInfo to)
    {
        if (from.TypeName == null || to.TypeName == null)
        {
            throw new CompileError.SemanticError(
                $"cannot cast value from type {from} to {to}",
                NodeContext
            );
        }

        var typeCastNode = new TypeCastNode(
            NodeContext,
            new TypeInfoNameNode(
                NodeContext,
                from.TypeName,
                []
            ),
            new TypeInfoNameNode(
                NodeContext,
                to.TypeName,
                []
            )
        );

        return typeCastNode;
    }
}