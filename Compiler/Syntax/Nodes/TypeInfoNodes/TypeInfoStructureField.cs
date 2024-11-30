namespace Compiler.Syntax.Nodes.TypeInfoNodes;

public class TypeInfoStructureField(NodeContext nodeContext, string name, TypeInfoNode typeInfoNode)
    : TypeInfoNode(nodeContext)
{
    public string Name { get; set; } = name;
    public TypeInfoNode TypeInfoNode { get; set; } = typeInfoNode;
}