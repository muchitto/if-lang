using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class EnumDeclarationNode(IdentifierNode name, List<IdentifierNode> items) : DeclarationNode(name)
{
    public List<IdentifierNode> Items { get; } = items;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitEnumDeclarationNode(this);
    }
}