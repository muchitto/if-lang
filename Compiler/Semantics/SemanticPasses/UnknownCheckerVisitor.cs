using Compiler.ErrorHandling;
using Compiler.Syntax.Nodes;
using Compiler.Syntax.Nodes.TypeInfoNodes;
using Compiler.Syntax.Visitor;

namespace Compiler.Semantics.SemanticPasses;

public class UnknownCheckerVisitor(SemanticHandler semanticHandler) : BaseNodeVisitor(semanticHandler)
{
    public override TypeInfoNameNode VisitTypeInfoNameNode(TypeInfoNameNode typeInfoNameNode)
    {
        if (typeInfoNameNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError(
                "type name is incomplete",
                typeInfoNameNode.NodeContext.PositionData
            );
        }

        return base.VisitTypeInfoNameNode(typeInfoNameNode);
    }

    public override TypeInfoArrayNode VisitTypeInfoArrayNode(TypeInfoArrayNode typeInfoArrayNode)
    {
        if (typeInfoArrayNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError(
                "array type is incomplete",
                typeInfoArrayNode.NodeContext.PositionData
            );
        }

        return base.VisitTypeInfoArrayNode(typeInfoArrayNode);
    }

    public override TypeInfoStructureNode VisitTypeInfoStructureNode(TypeInfoStructureNode typeInfoStructureNode)
    {
        if (typeInfoStructureNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError(
                "structure type is incomplete",
                typeInfoStructureNode.NodeContext.PositionData
            );
        }

        return base.VisitTypeInfoStructureNode(typeInfoStructureNode);
    }

    public override ArrayLiteralNode VisitArrayLiteralNode(ArrayLiteralNode arrayLiteralNode)
    {
        if (arrayLiteralNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError(
                "array type is incomplete",
                arrayLiteralNode.NodeContext.PositionData
            );
        }

        return base.VisitArrayLiteralNode(arrayLiteralNode);
    }

    public override EnumDeclarationNode VisitEnumDeclarationNode(EnumDeclarationNode enumDeclarationNode)
    {
        if (enumDeclarationNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError(
                "enum type is incomplete",
                enumDeclarationNode.NodeContext.PositionData
            );
        }

        return base.VisitEnumDeclarationNode(enumDeclarationNode);
    }

    public override StructureLiteralFieldNode VisitStructureLiteralFieldNode(
        StructureLiteralFieldNode structureLiteralFieldNode
    )
    {
        if (structureLiteralFieldNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError(
                "structure field type is incomplete",
                structureLiteralFieldNode.NodeContext.PositionData
            );
        }

        return base.VisitStructureLiteralFieldNode(structureLiteralFieldNode);
    }

    public override LiteralNode VisitLiteralNode(LiteralNode literalNode)
    {
        return base.VisitLiteralNode(literalNode);
    }

    public override StructureLiteralNode VisitStructureLiteralNode(StructureLiteralNode structureLiteralNode)
    {
        if (structureLiteralNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError(
                "structure type is incomplete",
                structureLiteralNode.NodeContext.PositionData
            );
        }

        return base.VisitStructureLiteralNode(structureLiteralNode);
    }

    public override ProgramNode VisitProgramNode(ProgramNode programNode)
    {
        return base.VisitProgramNode(programNode);
    }

    public override BodyBlockNode VisitBodyBlockNode(BodyBlockNode bodyBlockNode)
    {
        return base.VisitBodyBlockNode(bodyBlockNode);
    }

    public override NumberLiteralNode VisitNumberLiteralNode(NumberLiteralNode numberLiteralNode)
    {
        if (numberLiteralNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError(
                "number type is incomplete",
                numberLiteralNode.NodeContext.PositionData
            );
        }

        return base.VisitNumberLiteralNode(numberLiteralNode);
    }

    public override FunctionCallArgumentNode VisitFunctionCallArgumentNode(
        FunctionCallArgumentNode functionCallArgumentNode
    )
    {
        if (functionCallArgumentNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError(
                "function call argument type is incomplete",
                functionCallArgumentNode.NodeContext.PositionData
            );
        }

        return base.VisitFunctionCallArgumentNode(functionCallArgumentNode);
    }

    public override VariableDeclarationNode VisitVariableDeclarationNode(
        VariableDeclarationNode variableDeclarationNode
    )
    {
        if (variableDeclarationNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError(
                "variable type is incomplete",
                variableDeclarationNode.NodeContext.PositionData
            );
        }

        return base.VisitVariableDeclarationNode(variableDeclarationNode);
    }

    public override NamedNode VisitNamedNode(NamedNode namedNode)
    {
        if (namedNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError(
                $"named {namedNode.Name} type is incomplete",
                namedNode
            );
        }

        return base.VisitNamedNode(namedNode);
    }

    public override IdentifierNode VisitIdentifierNode(IdentifierNode identifierNode)
    {
        if (identifierNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError(
                $"identifier {identifierNode.Name} type is incomplete",
                identifierNode.NodeContext.PositionData
            );
        }

        return base.VisitIdentifierNode(identifierNode);
    }

    public override AssignmentNode VisitAssignmentNode(AssignmentNode assignmentNode)
    {
        if (assignmentNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError(
                "assignment type is incomplete",
                assignmentNode.NodeContext.PositionData
            );
        }

        return base.VisitAssignmentNode(assignmentNode);
    }

    public override FunctionDeclarationNode VisitFunctionDeclarationNode(
        FunctionDeclarationNode functionDeclarationNode
    )
    {
        if (functionDeclarationNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError(
                "function type is incomplete",
                functionDeclarationNode.NodeContext.PositionData
            );
        }

        return base.VisitFunctionDeclarationNode(functionDeclarationNode);
    }

    public override FunctionCallNode VisitFunctionCallNode(FunctionCallNode functionCallNode)
    {
        if (functionCallNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError(
                "function call type is incomplete",
                functionCallNode.NodeContext.PositionData
            );
        }

        return base.VisitFunctionCallNode(functionCallNode);
    }

    public override ExpressionNode VisitExpressionNode(ExpressionNode expressionNode)
    {
        if (expressionNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError(
                "expression type is incomplete",
                expressionNode.NodeContext.PositionData
            );
        }

        return base.VisitExpressionNode(expressionNode);
    }

    public override MemberAccessNode VisitMemberAccessNode(MemberAccessNode memberAccessNode)
    {
        if (memberAccessNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError(
                "member access type is incomplete",
                memberAccessNode.NodeContext.PositionData
            );
        }

        return base.VisitMemberAccessNode(memberAccessNode);
    }

    public override ObjectDeclarationNode VisitObjectDeclarationNode(ObjectDeclarationNode objectDeclarationNode)
    {
        if (objectDeclarationNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError(
                "object type is incomplete",
                objectDeclarationNode.NodeContext.PositionData
            );
        }

        return base.VisitObjectDeclarationNode(objectDeclarationNode);
    }

    public override FunctionDeclarationParameterNode VisitFunctionDeclarationParameterNode(
        FunctionDeclarationParameterNode functionDeclarationParameterNode
    )
    {
        if (functionDeclarationParameterNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError(
                "function parameter type is incomplete",
                functionDeclarationParameterNode.NodeContext.PositionData
            );
        }

        return base.VisitFunctionDeclarationParameterNode(functionDeclarationParameterNode);
    }

    public override BooleanLiteralNode VisitBooleanLiteralNode(BooleanLiteralNode booleanLiteralNode)
    {
        if (booleanLiteralNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError(
                "boolean type is incomplete",
                booleanLiteralNode.NodeContext.PositionData
            );
        }

        return base.VisitBooleanLiteralNode(booleanLiteralNode);
    }

    public override IfStatementNode VisitIfStatementNode(IfStatementNode ifStatementNode)
    {
        return base.VisitIfStatementNode(ifStatementNode);
    }

    public override StringLiteralNode VisitStringLiteralNode(StringLiteralNode stringLiteralNode)
    {
        if (stringLiteralNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError(
                "string type is incomplete",
                stringLiteralNode.NodeContext.PositionData
            );
        }

        return base.VisitStringLiteralNode(stringLiteralNode);
    }

    public override AnnotationNode VisitAnnotationNode(AnnotationNode annotationNode)
    {
        return base.VisitAnnotationNode(annotationNode);
    }

    public override WhileStatementNode VisitWhileNode(WhileStatementNode whileStatementNode)
    {
        return base.VisitWhileNode(whileStatementNode);
    }

    public override ForStatementNode VisitForStatementNode(ForStatementNode forStatementNode)
    {
        return base.VisitForStatementNode(forStatementNode);
    }

    public override EnumShortHandNode VisitEnumShortHandNode(EnumShortHandNode enumShortHandNode)
    {
        if (enumShortHandNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError(
                "enum type is incomplete",
                enumShortHandNode.NodeContext.PositionData
            );
        }

        return base.VisitEnumShortHandNode(enumShortHandNode);
    }

    public override ReturnStatementNode VisitReturnStatementNode(ReturnStatementNode returnStatementNode)
    {
        if (returnStatementNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError(
                "return type is incomplete",
                returnStatementNode.NodeContext.PositionData
            );
        }

        return base.VisitReturnStatementNode(returnStatementNode);
    }

    public override BreakStatementNode VisitBreakStatementNode(BreakStatementNode breakStatementNode)
    {
        return base.VisitBreakStatementNode(breakStatementNode);
    }

    public override ContinueStatementNode VisitContinueStatementNode(ContinueStatementNode continueStatementNode)
    {
        return base.VisitContinueStatementNode(continueStatementNode);
    }

    public override DeclarationNode VisitDeclarationNode(DeclarationNode declarationNode)
    {
        return base.VisitDeclarationNode(declarationNode);
    }

    public override BaseNode VisitBaseNode(BaseNode baseNode)
    {
        return base.VisitBaseNode(baseNode);
    }

    public override TypeInfoNode VisitTypeInfoNode(TypeInfoNode typeInfoNode)
    {
        if (typeInfoNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError(
                "type is incomplete",
                typeInfoNode.NodeContext.PositionData
            );
        }

        return base.VisitTypeInfoNode(typeInfoNode);
    }

    public override ObjectVariableOverride VisitObjectVariableOverride(ObjectVariableOverride objectVariableOverride)
    {
        if (objectVariableOverride.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError(
                "object variable override type is incomplete",
                objectVariableOverride.NodeContext.PositionData
            );
        }

        return base.VisitObjectVariableOverride(objectVariableOverride);
    }

    public override UnaryExpressionNode VisitUnaryExpressionNode(UnaryExpressionNode unaryExpressionNode)
    {
        if (unaryExpressionNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError(
                "unary expression type is incomplete",
                unaryExpressionNode.NodeContext.PositionData
            );
        }

        return base.VisitUnaryExpressionNode(unaryExpressionNode);
    }

    public override ExternVariableNode VisitExternVariableNode(ExternVariableNode externVariableNode)
    {
        if (externVariableNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError(
                "extern variable type is incomplete",
                externVariableNode.NodeContext.PositionData
            );
        }

        return base.VisitExternVariableNode(externVariableNode);
    }

    public override ExternFunctionNode VisitExternFunctionNode(ExternFunctionNode externFunctionNode)
    {
        if (externFunctionNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError(
                "extern function type is incomplete",
                externFunctionNode.NodeContext.PositionData
            );
        }

        return base.VisitExternFunctionNode(externFunctionNode);
    }

    public override ExternNode VisitExternNode(ExternNode externNode)
    {
        if (externNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError(
                "extern type is incomplete",
                externNode.NodeContext.PositionData
            );
        }

        return base.VisitExternNode(externNode);
    }

    public override ArrayAccessNode VisitArrayAccessNode(ArrayAccessNode arrayAccessNode)
    {
        if (arrayAccessNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError(
                "array access type is incomplete",
                arrayAccessNode.NodeContext.PositionData
            );
        }

        return base.VisitArrayAccessNode(arrayAccessNode);
    }
}