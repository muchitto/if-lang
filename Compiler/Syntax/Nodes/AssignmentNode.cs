using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class AssignmentNode(BaseNode name, BaseNode value, Operator op) : BaseNode
{
    public BaseNode Name { get; } = name;
    public BaseNode Value { get; } = value;

    public Operator Op { get; } = op;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitAssignmentNode(this);
    }
}