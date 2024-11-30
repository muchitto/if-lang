using System.Diagnostics;
using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes.TypeInfoNodes;

public class TypeInfoNameNode(NodeContext nodeContext, string name, List<TypeInfoNode> genericParameters)
    : TypeInfoNode(nodeContext)
{
    public string Name { get; } = name;

    public List<TypeInfoNode> GenericParameters { get; } = genericParameters;

    [DebuggerHidden]
    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitTypeInfoNameNode(this);
    }

    public TypeInfoNameNode WithGenericParameters(List<TypeInfoNode> genericParameters)
    {
        return new TypeInfoNameNode(NodeContext, Name, genericParameters);
    }

    public override string ToString()
    {
        return Name;
    }
}