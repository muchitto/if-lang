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
    public List<FunctionDeclarationParameterNode> ParameterNodes { get; set; } = parameterNodes;
    public TypeInfoNode? ReturnTypeInfo { get; set; } = returnTypeInfo;
    public BodyBlockNode Body { get; set; } = body;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitFunctionDeclarationNode(this);
    }

    public override string ToString()
    {
        return $"func {Named} (" + string.Join(", ", ParameterNodes) + ")" + ReturnTypeInfo;
    }
}