using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class DeclarationNameNode(NodeContext nodeContext, string name) : BaseNode(nodeContext)
{
    public string Name { get; } = name;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitDeclarationNameNode(this);
    }
}