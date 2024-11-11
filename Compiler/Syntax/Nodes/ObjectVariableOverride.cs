using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ObjectVariableOverride(NodeContext nodeContext, DeclarationNamedNode named, BaseNode value)
    : DeclarationNode(nodeContext, named, [])
{
    public BaseNode Value { get; } = value;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitObjectVariableOverride(this);
    }
}