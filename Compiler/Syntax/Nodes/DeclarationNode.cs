namespace Compiler.Syntax.Nodes;

public abstract class DeclarationNode(
    NodeContext nodeContext,
    DeclarationNamedNode named,
    List<AnnotationNode> annotationNodes)
    : BaseNode(nodeContext), IEquatable<BaseNode>
{
    public List<AnnotationNode> Annotations { get; } = annotationNodes;
    public DeclarationNamedNode Named { get; } = named;

    public virtual bool Equals(BaseNode? other)
    {
        return other is DeclarationNode declarationNode &&
               Named.Equals(declarationNode.Named) &&
               Annotations.SequenceEqual(declarationNode.Annotations);
    }
}