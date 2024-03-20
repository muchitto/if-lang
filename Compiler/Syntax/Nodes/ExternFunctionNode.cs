using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class ExternFunctionNode(
    NodeContext nodeContext,
    DeclarationNameNode name,
    List<FunctionDeclarationParameterNode> parameterNodes,
    TypeInfoNode? returnType) : ExternNode(nodeContext, name)
{
    public List<FunctionDeclarationParameterNode> ParameterNodes { get; } = parameterNodes;
    public TypeInfoNode? ReturnType { get; } = returnType;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitExternFunctionNode(this);
    }
}