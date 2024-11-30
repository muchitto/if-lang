using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class AssignmentNode(NodeContext nodeContext, BaseNode name, BaseNode value, Operator op) : BaseNode(nodeContext)
{
    public BaseNode Name { get; set; } = name;
    public BaseNode Value { get; set; } = value;

    public Operator Op { get; } = op;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitAssignmentNode(this);
    }
}