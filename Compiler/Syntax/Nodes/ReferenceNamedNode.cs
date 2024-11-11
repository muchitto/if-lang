using Compiler.Syntax.Nodes.TypeInfoNodes;
using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ReferenceNamedNode(NodeContext nodeContext, string name, List<TypeInfoNode> genericParameters)
    : NamedNode(nodeContext, name, genericParameters)
{
    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitReferenceNameNode(this);
    }
}