using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class TypeInfoEnumFieldParamNode(NodeContext nodeContext, DeclarationNameNode name, TypeInfoNode typeInfoNode)
    : TypeInfoNode(nodeContext)
{
    public DeclarationNameNode Name { get; } = name;
    public TypeInfoNode TypeInfoNode { get; } = typeInfoNode;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitTypeInfoEnumFieldParamNode(this);
    }
}