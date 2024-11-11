using Compiler.Syntax.Nodes.TypeInfoNodes;
using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ExternFunctionNode(
    NodeContext nodeContext,
    DeclarationNamedNode named,
    List<FunctionDeclarationParameterNode> parameterNodes,
    TypeInfoNode? returnType,
    List<TypeInfoNode> genericParameters
) : ExternNode(nodeContext, named), IEquatable<BaseNode>
{
    public List<TypeInfoNode> GenericParameters { get; } = genericParameters;
    public List<FunctionDeclarationParameterNode> ParameterNodes { get; } = parameterNodes;
    public TypeInfoNode? ReturnType { get; } = returnType;

    public bool Equals(BaseNode? other)
    {
        if (other is not ExternFunctionNode externFunctionNode)
        {
            return false;
        }

        if (!Named.Equals(externFunctionNode.Named))
        {
            return false;
        }

        if (!ParameterNodes.SequenceEqual(externFunctionNode.ParameterNodes))
        {
            return false;
        }

        if (ReturnType == null && externFunctionNode.ReturnType == null)
        {
            return true;
        }

        if (ReturnType == null || externFunctionNode.ReturnType == null)
        {
            return false;
        }

        return ReturnType.Equals(externFunctionNode.ReturnType);
    }

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitExternFunctionNode(this);
    }
}