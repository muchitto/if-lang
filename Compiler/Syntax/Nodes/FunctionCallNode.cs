using Compiler.Semantics.TypeInformation;
using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class FunctionCallNode(NodeContext nodeContext, IdentifierNode name, List<FunctionCallArgumentNode> arguments)
    : IdentifiableNode(nodeContext)
{
    public List<FunctionCallArgumentNode> Arguments = arguments;
    public IdentifierNode Name { get; set; } = name;

    public override string GetName()
    {
        return Name.Name;
    }

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitFunctionCallNode(this);
    }

    public override void SetTypeRef(TypeRef typeRef)
    {
        base.SetTypeRef(typeRef);
    }
}