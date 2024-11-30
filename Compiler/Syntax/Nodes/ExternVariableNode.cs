using Compiler.Syntax.Nodes.TypeInfoNodes;
using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ExternVariableNode(NodeContext nodeContext, DeclarationNamedNode named, TypeInfoNode typeInfoNode)
    : ExternNode(nodeContext, named)
{
    public TypeInfoNode TypeInfoNode { get; set; } = typeInfoNode;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitExternVariableNode(this);
    }
}