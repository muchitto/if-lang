using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class VariableDeclarationNode(IdentifierNode name, TypeInfoNode? typeInfo, BaseNode? value)
    : DeclarationNode(name)
{
    public TypeInfoNode? TypeName { get; } = typeInfo;
    public BaseNode? Value { get; } = value;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitVariableDeclarationNode(this);
    }
}