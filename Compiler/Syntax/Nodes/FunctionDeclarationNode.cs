using Compiler.Semantics.TypeInformation;
using Compiler.Syntax.Nodes.TypeInfoNodes;
using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class FunctionDeclarationNode(
    NodeContext nodeContext,
    DeclarationNamedNode named,
    List<FunctionDeclarationParameterNode> parameterNodes,
    TypeInfoNode? returnTypeInfo,
    BodyBlockNode body,
    List<AnnotationNode> annotationNodes
)
    : DeclarationNode(nodeContext, named, annotationNodes)
{
    public List<FunctionDeclarationParameterNode> ParameterNodes { get; } = parameterNodes;
    public TypeInfoNode? ReturnTypeInfo { get; } = returnTypeInfo;
    public BodyBlockNode Body { get; } = body;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitFunctionDeclarationNode(this);
    }

    public override string ToString()
    {
        return $"func {Named} (" + string.Join(", ", ParameterNodes) + ")" + ReturnTypeInfo;
    }

}