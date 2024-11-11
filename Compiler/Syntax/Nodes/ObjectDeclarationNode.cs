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
    public ReferenceNamedNode? BaseName { get; } = baseName;
    public List<DeclarationNode> Fields { get; } = fields;

    public bool IsImmediatelyInstanced { get; } = isImmediatelyInstanced;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitObjectDeclarationNode(this);
    }
}