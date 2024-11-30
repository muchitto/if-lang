using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ObjectVariableOverride(NodeContext nodeContext, DeclarationNamedNode named, BaseNode value)
    : DeclarationNode(nodeContext, named, [])
{
    public BaseNode Value { get; set; } = value;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitObjectVariableOverride(this);
    }
}