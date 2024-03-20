using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ObjectVariableOverride(NodeContext nodeContext, DeclarationNameNode name, BaseNode value)
    : DeclarationNode(nodeContext, name, [])
{
    public BaseNode Value { get; } = value;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitObjectVariableOverride(this);
    }
}