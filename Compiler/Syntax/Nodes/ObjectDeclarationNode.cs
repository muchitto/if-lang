using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ObjectDeclarationNode(
    NodeContext nodeContext,
    bool isImmediatelyInstanced,
    TypeInfoNameNode? baseName,
    DeclarationNameNode name,
    List<DeclarationNode> fields,
    List<AnnotationNode> annotationNodes
)
    : DeclarationNode(nodeContext, name, annotationNodes)
{
    public TypeInfoNameNode? BaseName { get; } = baseName;
    public List<DeclarationNode> Fields { get; } = fields;

    public bool IsImmediatelyInstanced { get; } = isImmediatelyInstanced;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitObjectDeclarationNode(this);
    }
}