using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ObjectDeclarationNode(
    NodeContext nodeContext,
    bool isImmediatelyInstanced,
    DeclarationNamedNode named,
    ReferenceNamedNode? baseName,
    List<DeclarationNode> fields,
    List<AnnotationNode> annotationNodes
)
    : DeclarationNode(nodeContext, named, annotationNodes)
{
    public ReferenceNamedNode? BaseName { get; set; } = baseName;
    public List<DeclarationNode> Fields { get; set; } = fields;

    public bool IsImmediatelyInstanced { get; } = isImmediatelyInstanced;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitObjectDeclarationNode(this);
    }

    public override string ToString()
    {
        return $"class {Named}" + (BaseName != null ? $": {BaseName}" : "");
    }
}