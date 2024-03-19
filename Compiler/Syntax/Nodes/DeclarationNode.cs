using Compiler.Semantics.TypeInformation;

namespace Compiler.Syntax.Nodes;

public class DeclarationNode(
    NodeContext nodeContext,
    IdentifierNode name,
    List<AnnotationNode> annotationNodes)
    : BaseNode(nodeContext)
{
    private TypeRef _typeRef = new(TypeInfo.Unknown);

    public List<AnnotationNode> Annotations { get; } = annotationNodes;
    public IdentifierNode Name { get; } = name;

    public override TypeRef TypeRef
    {
        get => _typeRef;
        set => Name.TypeRef = _typeRef = value;
    }
}