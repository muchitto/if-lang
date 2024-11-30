using Compiler.Semantics.TypeInformation;
using Compiler.Semantics.TypeInformation.Types;
using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class FunctionCallNode(NodeContext nodeContext, IdentifierNode name, List<FunctionCallArgumentNode> parameters)
    : IdentifiableNode(nodeContext)
{
    public List<FunctionCallArgumentNode> Parameters = parameters;

    public IdentifierNode Name { get; set; } = name;

    public override TypeRef ReturnedTypeRef
    {
        get
        {
            if (TypeRef.TypeInfo is FunctionTypeInfo functionTypeInfo)
            {
                return functionTypeInfo.ReturnType;
            }

            throw new Exception("type is not a function");
        }
    }

    protected override void SetTypeRef(TypeRef typeRef)
    {
        base.SetTypeRef(typeRef);

        Name.TypeRef = typeRef;
    }

    public override string GetName()
    {
        return Name.Name;
    }

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitFunctionCallNode(this);
    }

    public override string ToString()
    {
        return Name + "(" + string.Join(", ", Parameters) + ")";
    }

    public ObjectInitializationNode ConvertToObjectInitializationNode()
    {
        return new ObjectInitializationNode(NodeContext, Name, Parameters);
    }
}