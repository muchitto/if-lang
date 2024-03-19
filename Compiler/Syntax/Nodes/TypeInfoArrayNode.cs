using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class TypeInfoArrayNode(NodeContext nodeContext, TypeInfoNode baseType) : TypeInfoNode(nodeContext)
{
    public TypeInfoNode BaseType { get; } = baseType;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitTypeInfoArrayNode(this);
    }
}