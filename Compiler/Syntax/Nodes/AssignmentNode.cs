using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class AssignmentNode(NodeContext nodeContext, BaseNode name, BaseNode value, Operator op) : BaseNode(nodeContext)
{
    public BaseNode Name { get; } = name;
    public BaseNode Value { get; } = value;

    public Operator Op { get; } = op;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitAssignmentNode(this);
    }
}