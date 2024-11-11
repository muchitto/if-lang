using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class EnumDeclarationNode(
    NodeContext nodeContext,
    DeclarationNamedNode named,
    List<EnumDeclarationItemNode> items,
    List<AnnotationNode> annotationNodes
) : DeclarationNode(nodeContext, named, annotationNodes)
{
    public List<EnumDeclarationItemNode> Items { get; } = items;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitEnumDeclarationNode(this);
    }
}