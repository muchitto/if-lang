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


    public override string GetName()
    {
        return Name.Name;
    }

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitFunctionCallNode(this);
    }

    public override string ToString()
    {
        return Name + "(" + string.Join(", ", Parameters) + ")";
    }
}