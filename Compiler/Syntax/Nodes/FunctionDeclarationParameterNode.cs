using Compiler.Semantics.TypeInformation;
using Compiler.Syntax.Nodes.TypeInfoNodes;
using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class FunctionDeclarationParameterNode(
    NodeContext nodeContext,
    DeclarationNamedNode named,
    TypeInfoNode typeInfoNode
)
    : BaseNode(nodeContext)
{
    public DeclarationNamedNode Named { get; set; } = named;
    public TypeInfoNode TypeInfoNode { get; set; } = typeInfoNode;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitFunctionDeclarationParameterNode(this);
    }

    public override string ToString()
    {
        return Named + " : " + TypeInfoNode;
    }

    protected override void SetTypeRef(TypeRef typeRef)
    {
        base.SetTypeRef(typeRef);

        Named.TypeRef = typeRef;
    }
}