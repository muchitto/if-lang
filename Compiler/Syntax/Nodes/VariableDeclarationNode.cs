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
    public TypeInfoNode? TypeInfoNode { get; } = typeInfoNode;
    public BaseNode? Value { get; private set; } = value;

    public override void Accept(INodeVisitor nodeVisitor)
    {
        nodeVisitor.VisitVariableDeclarationNode(this);
    }

    public TypeCastNode CastValue(TypeInfo from, TypeInfo to)
    {
        if (Value == null)
        {
            throw new CompileError.SemanticError(
                "there is no value to cast",
                NodeContext
            );
        }

        if (from.TypeName == null || to.TypeName == null)
        {
            throw new CompileError.SemanticError(
                $"cannot cast value from type {from} to {to}",
                NodeContext
            );
        }

        var typeCastNode = new TypeCastNode(
            Value.NodeContext,
            new TypeInfoNameNode(
                NodeContext,
                from.TypeName,
                []
            ),
            new TypeInfoNameNode(
                NodeContext,
                to.TypeName,
                []
            )
        );
        ;

        Value = typeCastNode;

        return typeCastNode;
    }
}