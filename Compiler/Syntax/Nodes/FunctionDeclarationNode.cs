using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class FunctionDeclarationNode(
    IdentifierNode name,
    List<FunctionDeclarationArgumentNode> parameterNodes,
    TypeInfoNode? returnTypeInfo,
    BodyBlockNode body) : DeclarationNode(name)
{
    public List<FunctionDeclarationArgumentNode> ParameterNodes { get; } = parameterNodes;
    public TypeInfoNode? ReturnTypeInfo { get; } = returnTypeInfo;
    public BodyBlockNode Body { get; } = body;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitFunctionDeclarationNode(this);
    }
}