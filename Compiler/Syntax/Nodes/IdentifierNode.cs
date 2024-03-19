using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class IdentifierNode(NodeContext nodeContext, string name) : BaseNode(nodeContext)
{
    public string Name = name;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitIdentifierNode(this);
    }

    public TypeInfoNameNode ToTypeInfoNameNode()
    {
        return new TypeInfoNameNode(NodeContext, Name);
    }
}