using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class EnumDeclarationNode(
    NodeContext nodeContext,
    DeclarationNamedNode named,
    List<EnumDeclarationItemNode> items,
    List<AnnotationNode> annotationNodes
) : DeclarationNode(nodeContext, named, annotationNodes)
{
    public List<EnumDeclarationItemNode> Items { get; set; } = items;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitEnumDeclarationNode(this);
    }
}