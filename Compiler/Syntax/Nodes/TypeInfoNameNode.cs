using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class TypeInfoNameNode(NodeContext nodeContext, string name) : TypeInfoNode(nodeContext)
{
    public string Name { get; } = name;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitTypeNameNode(this);
    }
}