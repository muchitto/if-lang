using Compiler.ErrorHandling;
using Compiler.Semantics.ScopeHandling;
using Compiler.Semantics.TypeInformation.Types;
using Compiler.Syntax.Nodes;

namespace Compiler.Semantics.SemanticPasses;

public class TypeCheckNodeVisitor(SemanticHandler semanticHandler)
    : SemanticPassBaseNodeVisitor(semanticHandler, new TypeResolutionScopeHandler(semanticHandler))
{
    public override AssignmentNode VisitAssignmentNode(AssignmentNode assignmentNode)
    {
        if (!assignmentNode.TypeRef.Compare(assignmentNode.Value))
        {
            throw new CompileError.SemanticError(
                "cannot assign a value of wrong type",
                assignmentNode
            );
        }

        return base.VisitAssignmentNode(assignmentNode);
    }

    public override ObjectDeclarationNode VisitObjectDeclarationNode(ObjectDeclarationNode objectDeclarationNode)
    {
        return base.VisitObjectDeclarationNode(objectDeclarationNode);
    }

    public override FunctionCallNode VisitFunctionCallNode(FunctionCallNode functionCallNode)
    {
        if (functionCallNode.TypeRef.TypeInfo is not FunctionTypeInfo functionTypeInfo)
        {
            throw new CompileError.SemanticError(
                "calling a non function",
                functionCallNode
            );
        }

        // TODO: Come back to this when we are adding default values
        if (functionCallNode.Parameters.Count != functionTypeInfo.Parameters.Count)
        {
            throw new CompileError.SemanticError(
                "calling a function with wrong amount of arguments",
                functionCallNode
            );
        }

        for (var a = 0; a < functionCallNode.Parameters.Count; a++)
        {
            var argument = functionCallNode.Parameters[a];
            var functionTypeArgument = functionTypeInfo.Parameters[a];

            if (!argument.TypeRef.Compare(functionTypeArgument.TypeRef))
            {
                throw new CompileError.SemanticError(
                    $"argument {functionCallNode.Name} type does not match",
                    argument
                );
            }
        }

        return base.VisitFunctionCallNode(functionCallNode);
    }

    public override FunctionCallArgumentNode VisitFunctionCallArgumentNode(
        FunctionCallArgumentNode functionCallArgumentNode
    )
    {
        return base.VisitFunctionCallArgumentNode(functionCallArgumentNode);
    }

    public override FunctionDeclarationParameterNode VisitFunctionDeclarationParameterNode(
        FunctionDeclarationParameterNode functionDeclarationParameterNode
    )
    {
        return base.VisitFunctionDeclarationParameterNode(functionDeclarationParameterNode);
    }

    public override VariableDeclarationNode VisitVariableDeclarationNode(
        VariableDeclarationNode variableDeclarationNode
    )
    {
        if (variableDeclarationNode.Value == null)
        {
            return base.VisitVariableDeclarationNode(variableDeclarationNode);
        }

        if (variableDeclarationNode.Value.TypeRef.TypeInfo is NumberTypeInfo valueNumberTypeInfo
            && valueNumberTypeInfo.CanImplicitlyConvert(valueNumberTypeInfo.NumberType)
            && !variableDeclarationNode.Value.TypeRef.Compare(variableDeclarationNode.TypeRef))
        {
            if (variableDeclarationNode.TypeRef.TypeInfo is not NumberTypeInfo variableNumberTypeInfo)
            {
                throw new CompileError.SemanticError(
                    "the variable type is not a number",
                    variableDeclarationNode
                );
            }

            var typeCastNode = variableDeclarationNode.CastValue(
                valueNumberTypeInfo,
                variableNumberTypeInfo
            );

            new TypeResolutionNodeVisitor(semanticHandler).VisitTypeCastNode(typeCastNode);
        }

        if (
            (variableDeclarationNode.TypeInfoNode != null &&
             !variableDeclarationNode.TypeInfoNode.TypeRef.Compare(variableDeclarationNode.Value))
            || !variableDeclarationNode.TypeRef.Compare(variableDeclarationNode.Value))
        {
            throw new CompileError.SemanticError(
                "the variable type and value does not match",
                variableDeclarationNode
            );
        }

        if (!variableDeclarationNode.TypeRef.Compare(variableDeclarationNode.Value))
        {
            throw new CompileError.SemanticError(
                "variable value type does not match the type of the variable",
                variableDeclarationNode
            );
        }

        return base.VisitVariableDeclarationNode(variableDeclarationNode);
    }
}