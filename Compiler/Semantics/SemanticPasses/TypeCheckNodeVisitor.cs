using Compiler.ErrorHandling;
using Compiler.Semantics.ScopeHandling;
using Compiler.Semantics.TypeInformation.Types;
using Compiler.Syntax.Nodes;

namespace Compiler.Semantics.SemanticPasses;

public class TypeCheckNodeVisitor(SemanticHandler semanticHandler)
    : SemanticPassBaseNodeVisitor(semanticHandler, new TypeResolutionScopeHandler(semanticHandler))
{
    public override ProgramNode VisitProgramNode(ProgramNode programNode)
    {
        return base.VisitProgramNode(programNode);
    }

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

    public override FunctionDeclarationNode VisitFunctionDeclarationNode(
        FunctionDeclarationNode functionDeclarationNode
    )
    {
        return base.VisitFunctionDeclarationNode(functionDeclarationNode);
    }

    public override ExpressionNode VisitExpressionNode(ExpressionNode expressionNode)
    {
        return base.VisitExpressionNode(expressionNode);
    }

    public override EnumShortHandNode VisitEnumShortHandNode(EnumShortHandNode enumShortHandNode)
    {
        var usedEnumName = enumShortHandNode.Named.Name;

        if (enumShortHandNode.TypeRef.TypeInfo is not BaseEnumTypeInfo enumTypeInfo)
        {
            throw new CompileError.SemanticError(
                "the enum usage does not have type information",
                enumShortHandNode
            );
        }

        var field = enumTypeInfo.Fields.ToList().Find(f => f.Name == usedEnumName);

        if (field == null)
        {
            throw new CompileError.SemanticError(
                $"no field {usedEnumName} in enum",
                enumShortHandNode
            );
        }

        if (field.TypeRef.TypeInfo is not EnumItemTypeInfo enumItemTypeInfo)
        {
            throw new CompileError.SemanticError(
                "wrong typeinfo",
                enumShortHandNode
            );
        }

        if (enumShortHandNode.Parameters.Count != enumItemTypeInfo.Parameters.Count)
        {
            throw new CompileError.SemanticError(
                "wrong amount of arguments given",
                enumShortHandNode
            );
        }

        for (var p = 0; p < enumShortHandNode.Parameters.Count; p++)
        {
            var typeField = enumShortHandNode.Parameters[p];
            var enumField = enumItemTypeInfo.Parameters[p];

            if (!enumField.TypeRef.Compare(typeField.TypeRef))
            {
                throw new CompileError.SemanticError(
                    $"the field {enumField.Name} has a different type",
                    enumShortHandNode
                );
            }
        }

        return base.VisitEnumShortHandNode(enumShortHandNode);
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
            && !variableDeclarationNode.Value.Compare(variableDeclarationNode))
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

        if (variableDeclarationNode.TypeInfoNode is not null)
        {
            if (!variableDeclarationNode.TypeInfoNode.Compare(variableDeclarationNode.Value)
                || !variableDeclarationNode.Compare(variableDeclarationNode.Value))
            {
                throw new CompileError.SemanticError(
                    "the variable type and value does not match",
                    variableDeclarationNode
                );
            }
        }

        if (!variableDeclarationNode.Compare(variableDeclarationNode.Value))
        {
            throw new CompileError.SemanticError(
                "variable value type does not match the type of the variable",
                variableDeclarationNode
            );
        }

        return base.VisitVariableDeclarationNode(variableDeclarationNode);
    }
}