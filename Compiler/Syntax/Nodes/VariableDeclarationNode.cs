using Compiler.ErrorHandling;
using Compiler.Semantics.TypeInformation;
using Compiler.Syntax.Nodes.TypeInfoNodes;
using Compiler.Syntax.Visitor;

namespace Compiler.Syntax.Nodes;

public class VariableDeclarationNode(
    NodeContext nodeContext,
    DeclarationNamedNode named,
    TypeInfoNode? typeInfoNode,
    BaseNode? value,
    List<AnnotationNode> annotationNodes
)
    : DeclarationNode(nodeContext, named, annotationNodes)
{
    public TypeInfoNode? TypeInfoNode { get; set; } = typeInfoNode;
    public BaseNode? Value { get; set; } = value;

    public override BaseNode Accept(INodeVisitor nodeVisitor)
    {
        return nodeVisitor.VisitVariableDeclarationNode(this);
    }

    public override TypeCastNode CastValue(TypeInfo from, TypeInfo to)
    {
        if (Value == null)
        {
            throw new CompileError.SemanticError(
                "there is no value to cast",
                NodeContext
            );
        }

        var typeCastNode = base.CastValue(from, to);

        Value = typeCastNode;

        return typeCastNode;
    }
}