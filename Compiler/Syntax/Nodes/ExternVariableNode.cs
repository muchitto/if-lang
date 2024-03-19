using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ExternVariableNode(NodeContext nodeContext, IdentifierNode name, TypeInfoNode typeInfoNode)
    : ExternNode(nodeContext, name)
{
    public TypeInfoNode TypeInfoNode { get; } = typeInfoNode;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitExternVariableNode(this);
    }
}