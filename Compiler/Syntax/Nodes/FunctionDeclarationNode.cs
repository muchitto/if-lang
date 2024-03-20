using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class FunctionDeclarationNode(
    NodeContext nodeContext,
    DeclarationNameNode name,
    List<FunctionDeclarationParameterNode> parameterNodes,
    TypeInfoNode? returnTypeInfo,
    BodyBlockNode body,
    List<AnnotationNode> annotationNodes
) : DeclarationNode(nodeContext, name, annotationNodes)
{
    public List<FunctionDeclarationParameterNode> ParameterNodes { get; } = parameterNodes;
    public TypeInfoNode? ReturnTypeInfo { get; } = returnTypeInfo;
    public BodyBlockNode Body { get; } = body;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitFunctionDeclarationNode(this);
    }
}