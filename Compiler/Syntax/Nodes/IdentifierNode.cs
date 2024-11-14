using Compiler.Syntax.Nodes.TypeInfoNodes;
using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class IdentifierNode(NodeContext nodeContext, string name) : IdentifiableNode(nodeContext)
{
    public string Name { get; set; } = name;

    public override string GetName()
    {
        return Name;
    }

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitIdentifierNode(this);
    }

    public TypeInfoNameNode ToTypeInfoNameNode()
    {
        return new TypeInfoNameNode(NodeContext, Name, []);
    }

    public DeclarationNamedNode ToDeclarationNameNode()
    {
        return new DeclarationNamedNode(NodeContext, Name, []);
    }

    public override string ToString()
    {
        return Name;
    }
}