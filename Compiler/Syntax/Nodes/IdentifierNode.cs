using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class IdentifierNode(string name) : BaseNode
{
    public string Name = name;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitIdentifierNode(this);
    }
}