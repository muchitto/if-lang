using Compiler.Semantics.TypeInformation;

namespace Compiler.Syntax.Nodes;

public abstract class DeclarationNode(
    NodeContext nodeContext,
    DeclarationNamedNode named,
    List<AnnotationNode> annotationNodes
)
    : BaseNode(nodeContext), IEquatable<BaseNode>
{
    public List<AnnotationNode> Annotations { get; set; } = annotationNodes;
    public DeclarationNamedNode Named { get; set; } = named;

    public virtual bool Equals(BaseNode? other)
    {
        return other is DeclarationNode declarationNode &&
               Named.Equals(declarationNode.Named) &&
               Annotations.SequenceEqual(declarationNode.Annotations);
    }

    protected override void SetTypeRef(TypeRef typeRef)
    {
        base.SetTypeRef(typeRef);

        Named.TypeRef = typeRef;
    }
}