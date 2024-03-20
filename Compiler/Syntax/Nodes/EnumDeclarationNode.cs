using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class EnumDeclarationNode(
    NodeContext nodeContext,
    DeclarationNameNode name,
    List<EnumDeclarationItemNode> items,
    List<AnnotationNode> annotationNodes)
    : DeclarationNode(nodeContext, name, annotationNodes)
{
    public List<EnumDeclarationItemNode> Items { get; } = items;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitEnumDeclarationNode(this);
    }
}