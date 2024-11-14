using Compiler.ErrorHandling;
using Compiler.Semantics.TypeInformation.Types;
using Compiler.Syntax.Nodes;
using Compiler.Syntax.Visitor;

namespace Compiler.Semantics.SemanticPasses;

public class NullCheckerNodeVisitor(SemanticHandler semanticHandler) : BaseNodeVisitor(semanticHandler)
{
    public override AssignmentNode VisitAssignmentNode(AssignmentNode assignmentNode)
    {
        base.VisitAssignmentNode(assignmentNode);

        if (assignmentNode.Value is not NullLiteralNode)
        {
            return assignmentNode;
        }

        if (assignmentNode.TypeRef.TypeInfo is not GenericTypeInfo genericTypeInfo)
        {
            throw new CompileError.SemanticError(
                $"Cannot assign null to non-nullable type {assignmentNode.TypeRef.TypeInfo}",
                assignmentNode.NodeContext.PositionData
            );
        }

        if (genericTypeInfo.Name != "Optional" || genericTypeInfo.GenericParams.Count != 1 ||
            genericTypeInfo.GenericParams[0].TypeInfo is not ObjectTypeInfo)
        {
            throw new CompileError.SemanticError(
                $"Cannot assign null to non-nullable type {assignmentNode.TypeRef.TypeInfo}",
                assignmentNode.NodeContext.PositionData
            );
        }

        return assignmentNode;
    }
}