using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class TypeInfoNameNode(string name) : TypeInfoNode
{
    public string Name { get; } = name;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitTypeNameNode(this);
    }
}