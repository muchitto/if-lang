using Compiler.Syntax.Nodes.TypeInfoNodes;
using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class TypeFieldAccessNode(
    NodeContext nodeContext,
    TypeInfoNameNode baseTypeNameNode,
    IdentifiableNode member
)
    : FieldAccessNode(nodeContext, baseTypeNameNode.IdentifierNode(), member)
{
    public TypeInfoNameNode BaseTypeNode { get; set; } = baseTypeNameNode;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitTypeFieldAccessNode(this);
    }
}