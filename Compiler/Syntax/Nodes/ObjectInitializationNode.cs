using Compiler.Semantics.TypeInformation;
using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ObjectInitializationNode(
    NodeContext nodeContext,
    IdentifierNode identifierNode,
    List<FunctionCallArgumentNode> parameters
)
    : FunctionCallNode(nodeContext, identifierNode, parameters)
{
    public override TypeRef ReturnedTypeRef => TypeRef;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitObjectInitializationNode(this);
    }
}