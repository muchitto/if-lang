namespace Compiler.Syntax.Nodes;

public class DeclarationNode(
    NodeContext nodeContext,
    DeclarationNameNode name,
    List<AnnotationNode> annotationNodes)
    : BaseNode(nodeContext)
{
    public List<AnnotationNode> Annotations { get; } = annotationNodes;
    public DeclarationNameNode Name { get; } = name;
}