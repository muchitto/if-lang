using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class VariableDeclarationNode(
    NodeContext nodeContext,
    DeclarationNameNode name,
    TypeInfoNode? typeInfo,
    BaseNode? value,
    List<AnnotationNode> annotationNodes)
    : DeclarationNode(nodeContext, name, annotationNodes)
{
    public TypeInfoNode? TypeInfo { get; } = typeInfo;
    public BaseNode? Value { get; } = value;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitVariableDeclarationNode(this);
    }
}