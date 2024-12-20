using Compiler.Syntax.Nodes.TypeInfoNodes;
using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class NamedNode(NodeContext nodeContext, string name, List<TypeInfoNode> genericParameters)
    : BaseNode(nodeContext)
{
    public string Name { get; } = name;

    public List<TypeInfoNode> GenericParameters { get; set; } = genericParameters;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitNamedNode(this);
    }

    public TypeInfoNameNode ToTypeInfoNameNode()
    {
        return new TypeInfoNameNode(NodeContext, Name, GenericParameters);
    }

    public ReferenceNamedNode ToReferenceNameNode()
    {
        return new ReferenceNamedNode(NodeContext, Name, GenericParameters);
    }

    public DeclarationNamedNode ToDeclarationNameNode()
    {
        return new DeclarationNamedNode(NodeContext, Name, GenericParameters);
    }

    public override string ToString()
    {
        return Name;
    }
}