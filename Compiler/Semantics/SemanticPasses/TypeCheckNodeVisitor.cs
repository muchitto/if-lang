using Compiler.Semantics.TypeInformation.Types;
using Compiler.Syntax.Nodes;

namespace Compiler.Semantics.SemanticPasses;

public class TypeCheckNodeVisitor(SemanticContext semanticContext) : SemanticPassBaseNodeVisitor(semanticContext)
{
    public override AssignmentNode VisitAssignmentNode(AssignmentNode assignmentNode)
    {
        return base.VisitAssignmentNode(assignmentNode);
    }

    public override VariableDeclarationNode VisitVariableDeclarationNode(
        VariableDeclarationNode variableDeclarationNode
    )
    {
        if (variableDeclarationNode.Value == null)
        {
            return base.VisitVariableDeclarationNode(variableDeclarationNode);
        }

        if (variableDeclarationNode.Value.TypeRef.TypeInfo is NumberTypeInfo numberTypeInfo
            && numberTypeInfo.CanImplicitlyConvert(numberTypeInfo.NumberType))
        {
        }

        return base.VisitVariableDeclarationNode(variableDeclarationNode);
    }
}