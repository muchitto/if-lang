namespace Compiler.Syntax.Nodes;

public class ObjectFieldDeclarationNode(string name, DeclarationNode declarationNode) : BaseNode
{
    public string Name { get; } = name;
    public DeclarationNode Declaration { get; } = declarationNode;
}