using Compiler.Syntax.Nodes.TypeInfoNodes;
using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class VariableDeclarationNode(
    NodeContext nodeContext,
    DeclarationNamedNode named,
    TypeInfoNode? typeInfo,
    BaseNode? value,
    List<AnnotationNode> annotationNodes
)
    : DeclarationNode(nodeContext, named, annotationNodes)
{
    public TypeInfoNode? TypeInfo { get; } = typeInfo;
    public BaseNode? Value { get; } = value;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitVariableDeclarationNode(this);
    }
}