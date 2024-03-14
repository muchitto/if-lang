using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class FunctionDeclarationArgumentNode(IdentifierNode name, TypeInfoNode typeInfoNode) : BaseNode
{
    public IdentifierNode Name { get; } = name;
    public TypeInfoNode TypeInfoNode { get; } = typeInfoNode;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitFunctionDeclarationParameterNode(this);
    }
}