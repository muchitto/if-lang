namespace Compiler.Syntax.Nodes;

public class ObjectFieldDeclarationNode(NodeContext nodeContext, string name, DeclarationNode declarationNode)
    : BaseNode(nodeContext)
{
    public string Name { get; } = name;
    public DeclarationNode Declaration { get; } = declarationNode;
}