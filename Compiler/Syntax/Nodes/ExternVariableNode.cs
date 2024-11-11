using Compiler.Syntax.Nodes.TypeInfoNodes;
using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ExternVariableNode(NodeContext nodeContext, DeclarationNamedNode named, TypeInfoNode typeInfoNode)
    : ExternNode(nodeContext, named)
{
    public TypeInfoNode TypeInfoNode { get; } = typeInfoNode;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitExternVariableNode(this);
    }
}