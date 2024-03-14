using Compiler.ErrorHandling;
using Compiler.Syntax.Nodes;
using Compiler.Syntax.Visitor;

namespace Compiler.SemanticPasses;

public class UnknownCheckerVisitor : BaseNodeVisitor
{
    public override TypeInfoNameNode VisitTypeNameNode(TypeInfoNameNode typeInfoNameNode)
    {
        if (typeInfoNameNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError("type name is incomplete");
        }

        return base.VisitTypeNameNode(typeInfoNameNode);
    }

    public override TypeInfoArrayNode VisitTypeInfoArrayNode(TypeInfoArrayNode typeInfoArrayNode)
    {
        if (typeInfoArrayNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError("array type is incomplete");
        }

        return base.VisitTypeInfoArrayNode(typeInfoArrayNode);
    }

    public override TypeInfoStructureNode VisitTypeInfoStructureNode(TypeInfoStructureNode typeInfoStructureNode)
    {
        if (typeInfoStructureNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError("structure type is incomplete");
        }

        return base.VisitTypeInfoStructureNode(typeInfoStructureNode);
    }

    public override ArrayLiteralNode VisitArrayLiteralNode(ArrayLiteralNode arrayLiteralNode)
    {
        if (arrayLiteralNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError("array type is incomplete");
        }

        return base.VisitArrayLiteralNode(arrayLiteralNode);
    }

    public override EnumDeclarationNode VisitEnumDeclarationNode(EnumDeclarationNode enumDeclarationNode)
    {
        if (enumDeclarationNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError("enum type is incomplete");
        }

        return base.VisitEnumDeclarationNode(enumDeclarationNode);
    }

    public override StructureLiteralFieldNode VisitStructureLiteralFieldNode(
        StructureLiteralFieldNode structureLiteralFieldNode)
    {
        if (structureLiteralFieldNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError("structure field type is incomplete");
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
            throw new CompileError.SemanticError("structure type is incomplete");
        }

        return base.VisitStructureLiteralNode(structureLiteralNode);
    }

    public override ProgramNode VisitProgramNode(ProgramNode programNode)
    {
        return base.VisitProgramNode(programNode);
    }

    public override WithExpressionNode VisitWithExpressionNode(WithExpressionNode withExpressionNode)
    {
        return base.VisitWithExpressionNode(withExpressionNode);
    }

    public override BodyBlockNode VisitBodyBlockNode(BodyBlockNode bodyBlockNode)
    {
        return base.VisitBodyBlockNode(bodyBlockNode);
    }

    public override NumberLiteralNode VisitNumberNode(NumberLiteralNode numberLiteralNode)
    {
        if (numberLiteralNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError("number type is incomplete");
        }

        return base.VisitNumberNode(numberLiteralNode);
    }

    public override FunctionCallArgumentNode VisitFunctionCallArgumentNode(
        FunctionCallArgumentNode functionCallArgumentNode)
    {
        if (functionCallArgumentNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError("function call argument type is incomplete");
        }

        return base.VisitFunctionCallArgumentNode(functionCallArgumentNode);
    }

    public override FunctionExpressionNode VisitFunctionExpressionNode(FunctionExpressionNode functionExpressionNode)
    {
        return base.VisitFunctionExpressionNode(functionExpressionNode);
    }

    public override VariableDeclarationNode VisitVariableDeclarationNode(
        VariableDeclarationNode variableDeclarationNode)
    {
        if (variableDeclarationNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError("variable type is incomplete");
        }

        return base.VisitVariableDeclarationNode(variableDeclarationNode);
    }

    public override IdentifierNode VisitIdentifierNode(IdentifierNode identifierNode)
    {
        if (identifierNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError($"identifier {identifierNode.Name} type is incomplete");
        }

        return base.VisitIdentifierNode(identifierNode);
    }

    public override AssignmentNode VisitAssignmentNode(AssignmentNode assignmentNode)
    {
        if (assignmentNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError("assignment type is incomplete");
        }

        return base.VisitAssignmentNode(assignmentNode);
    }

    public override FunctionDeclarationNode VisitFunctionDeclarationNode(
        FunctionDeclarationNode functionDeclarationNode)
    {
        if (functionDeclarationNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError("function type is incomplete");
        }

        return base.VisitFunctionDeclarationNode(functionDeclarationNode);
    }

    public override PropertySetExpressionNode VisitPropertySetExpressionNode(
        PropertySetExpressionNode propertySetExpressionNode)
    {
        return base.VisitPropertySetExpressionNode(propertySetExpressionNode);
    }

    public override FunctionCallNode VisitFunctionCallNode(FunctionCallNode functionCallNode)
    {
        if (functionCallNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError("function call type is incomplete");
        }

        return base.VisitFunctionCallNode(functionCallNode);
    }

    public override ImportStatementNode VisitImportStatementNode(ImportStatementNode importStatementNode)
    {
        return base.VisitImportStatementNode(importStatementNode);
    }

    public override ExpressionNode VisitExpressionNode(ExpressionNode expressionNode)
    {
        if (expressionNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError("expression type is incomplete");
        }

        return base.VisitExpressionNode(expressionNode);
    }

    public override MemberAccessNode VisitMemberAccessNode(MemberAccessNode memberAccessNode)
    {
        if (memberAccessNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError("member access type is incomplete");
        }

        return base.VisitMemberAccessNode(memberAccessNode);
    }

    public override ObjectDeclarationNode VisitObjectDeclarationNode(ObjectDeclarationNode objectDeclarationNode)
    {
        if (objectDeclarationNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError("object type is incomplete");
        }

        return base.VisitObjectDeclarationNode(objectDeclarationNode);
    }

    public override FunctionDeclarationArgumentNode VisitFunctionDeclarationParameterNode(
        FunctionDeclarationArgumentNode functionDeclarationArgumentNode)
    {
        if (functionDeclarationArgumentNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError("function parameter type is incomplete");
        }

        return base.VisitFunctionDeclarationParameterNode(functionDeclarationArgumentNode);
    }

    public override BooleanLiteralNode VisitBooleanLiteralNode(BooleanLiteralNode booleanLiteralNode)
    {
        if (booleanLiteralNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError("boolean type is incomplete");
        }

        return base.VisitBooleanLiteralNode(booleanLiteralNode);
    }

    public override IfStatementNode VisitIfStatementNode(IfStatementNode ifStatementNode)
    {
        return base.VisitIfStatementNode(ifStatementNode);
    }

    public override FlagExpressionNode VisitFlagExpressionNode(FlagExpressionNode flagExpressionNode)
    {
        return base.VisitFlagExpressionNode(flagExpressionNode);
    }

    public override StringLiteralNode VisitStringLiteralNode(StringLiteralNode stringLiteralNode)
    {
        if (stringLiteralNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError("string type is incomplete");
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

    public override ReturnStatementNode VisitReturnStatementNode(ReturnStatementNode returnStatementNode)
    {
        if (returnStatementNode.TypeRef.TypeInfo.IsIncomplete)
        {
            throw new CompileError.SemanticError("return type is incomplete");
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
            throw new CompileError.SemanticError("type is incomplete");
        }

        return base.VisitTypeInfoNode(typeInfoNode);
    }
}