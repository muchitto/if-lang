using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class TypeInfoArrayNode(TypeInfoNode baseType) : TypeInfoNode
{
    public TypeInfoNode BaseType { get; } = baseType;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitTypeInfoArrayNode(this);
    }
}