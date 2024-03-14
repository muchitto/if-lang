using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ObjectDeclarationNode(
    bool isImmediatelyInstanced,
    TypeInfoNameNode baseName,
    IdentifierNode name,
    List<DeclarationNode> fields)
    : DeclarationNode(name)
{
    public TypeInfoNameNode BaseName { get; } = baseName;
    public List<DeclarationNode> Fields { get; } = fields;

    public bool IsImmediatelyInstanced { get; } = isImmediatelyInstanced;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitObjectDeclarationNode(this);
    }
}