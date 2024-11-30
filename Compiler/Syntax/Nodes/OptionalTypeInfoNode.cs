using Compiler.Syntax.Nodes.TypeInfoNodes;
using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class OptionalTypeInfoNode(
    NodeContext nodeContext,
    TypeInfoNode typeInfoNode
) : TypeInfoNode(nodeContext)
{
    public TypeInfoNode TypeInfo { get; set; } = typeInfoNode;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitOptionalTypeInfoNode(this);
    }
}