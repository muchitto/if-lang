using Compiler.Syntax.Nodes;

namespace Compiler.Syntax.Visitor;

public class VisitorError : Exception
{
    public VisitorError(string message) : base(message)
    {
    }
}

public abstract class BaseNodeVisitor : INodeVisitor
{
    public virtual BaseNode VisitBaseNode(BaseNode baseNode)
    {
        return baseNode switch
        {
            TypeInfoNameNode typeInfoNameNode => VisitTypeNameNode(typeInfoNameNode),
            ProgramNode programNode => VisitProgramNode(programNode),
            WithExpressionNode withExpressionNode => VisitWithExpressionNode(withExpressionNode),
            BodyBlockNode bodyBlockNode => VisitBodyBlockNode(bodyBlockNode),
            FunctionCallArgumentNode functionCallArgumentNode =>
                VisitFunctionCallArgumentNode(functionCallArgumentNode),
            FunctionExpressionNode functionExpressionNode => VisitFunctionExpressionNode(functionExpressionNode),
            MemberAccessNode memberAccessNode => VisitMemberAccessNode(memberAccessNode),
            IdentifierNode identifierNode => VisitIdentifierNode(identifierNode),
            PropertySetExpressionNode propertySetExpressionNode => VisitPropertySetExpressionNode(
                propertySetExpressionNode),
            FunctionCallNode functionCallNode => VisitFunctionCallNode(functionCallNode),
            ImportStatementNode importStatementNode => VisitImportStatementNode(importStatementNode),
            ExpressionNode expressionNode => VisitExpressionNode(expressionNode),
            FunctionDeclarationArgumentNode functionDeclarationParameterNode => VisitFunctionDeclarationParameterNode(
                functionDeclarationParameterNode),
            IfStatementNode ifStatementNode => VisitIfStatementNode(ifStatementNode),
            FlagExpressionNode flagExpressionNode => VisitFlagExpressionNode(flagExpressionNode),
            AnnotationNode annotationNode => VisitAnnotationNode(annotationNode),
            WhileStatementNode whileStatementNode => VisitWhileNode(whileStatementNode),
            ForStatementNode forStatementNode => VisitForStatementNode(forStatementNode),
            ReturnStatementNode returnStatementNode => VisitReturnStatementNode(returnStatementNode),
            BreakStatementNode breakStatementNode => VisitBreakStatementNode(breakStatementNode),
            ContinueStatementNode continueStatementNode => VisitContinueStatementNode(continueStatementNode),
            DeclarationNode declarationNode => VisitDeclarationNode(declarationNode),
            AssignmentNode assignmentNode => VisitAssignmentNode(assignmentNode),
            LiteralNode literalNode => VisitLiteralNode(literalNode),
            _ => throw new VisitorError("unhandled base node")
        };
    }

    public virtual DeclarationNode VisitDeclarationNode(DeclarationNode declarationNode)
    {
        return declarationNode switch
        {
            VariableDeclarationNode variableDeclaration => VisitVariableDeclarationNode(variableDeclaration),
            FunctionDeclarationNode functionDeclarationNode => VisitFunctionDeclarationNode(functionDeclarationNode),
            ObjectDeclarationNode objectDeclarationNode => VisitObjectDeclarationNode(objectDeclarationNode),
            _ => throw new VisitorError("unhandled declaration node")
        };
    }

    public virtual LiteralNode VisitLiteralNode(LiteralNode literalNode)
    {
        return literalNode switch
        {
            NumberLiteralNode numberLiteralNode => VisitNumberNode(numberLiteralNode),
            BooleanLiteralNode booleanLiteralNode => VisitBooleanLiteralNode(booleanLiteralNode),
            StringLiteralNode stringLiteralNode => VisitStringLiteralNode(stringLiteralNode),
            StructureLiteralNode structureLiteralNode => VisitStructureLiteralNode(structureLiteralNode),
            StructureLiteralFieldNode structureLiteralFieldNode => VisitStructureLiteralFieldNode(
                structureLiteralFieldNode),
            ArrayLiteralNode arrayLiteralNode => VisitArrayLiteralNode(arrayLiteralNode),
            _ => throw new VisitorError("unhandled literal node")
        };
    }

    public virtual TypeInfoNode VisitTypeInfoNode(TypeInfoNode typeInfoNode)
    {
        switch (typeInfoNode)
        {
            case TypeInfoNameNode typeInfoNameNode:
                return VisitTypeNameNode(typeInfoNameNode);
            case TypeInfoArrayNode typeInfoArrayNode:
                return VisitTypeInfoArrayNode(typeInfoArrayNode);
            case TypeInfoStructureNode typeInfoStructureNode:
                return VisitTypeInfoStructureNode(typeInfoStructureNode);
        }

        throw new VisitorError("unhandled type info node");
    }

    public virtual TypeInfoNameNode VisitTypeNameNode(TypeInfoNameNode typeInfoNameNode)
    {
        return typeInfoNameNode;
    }

    public virtual TypeInfoArrayNode VisitTypeInfoArrayNode(TypeInfoArrayNode typeInfoArrayNode)
    {
        typeInfoArrayNode.BaseType.Accept(this);

        return typeInfoArrayNode;
    }

    public virtual TypeInfoStructureNode VisitTypeInfoStructureNode(TypeInfoStructureNode typeInfoStructureNode)
    {
        foreach (var field in typeInfoStructureNode.Fields)
        {
            field.Value.Accept(this);
        }

        return typeInfoStructureNode;
    }

    public virtual ArrayLiteralNode VisitArrayLiteralNode(ArrayLiteralNode arrayLiteralNode)
    {
        foreach (var value in arrayLiteralNode.Elements)
        {
            value.Accept(this);
        }

        return arrayLiteralNode;
    }

    public virtual StructureLiteralNode VisitStructureLiteralNode(StructureLiteralNode structureLiteralNode)
    {
        foreach (var field in structureLiteralNode.Fields)
        {
            field.Accept(this);
        }

        return structureLiteralNode;
    }

    public virtual ProgramNode VisitProgramNode(ProgramNode programNode)
    {
        foreach (var declaration in programNode.Declarations)
        {
            VisitDeclarationNode(declaration);
        }

        return programNode;
    }

    public virtual WithExpressionNode VisitWithExpressionNode(WithExpressionNode withExpressionNode)
    {
        return withExpressionNode;
    }

    public virtual BodyBlockNode VisitBodyBlockNode(BodyBlockNode bodyBlockNode)
    {
        foreach (var statement in bodyBlockNode.Statements)
        {
            statement.Accept(this);
        }

        return bodyBlockNode;
    }

    public virtual NumberLiteralNode VisitNumberNode(NumberLiteralNode numberLiteralNode)
    {
        return numberLiteralNode;
    }

    public virtual FunctionCallArgumentNode VisitFunctionCallArgumentNode(
        FunctionCallArgumentNode functionCallArgumentNode)
    {
        functionCallArgumentNode.Value.Accept(this);

        return functionCallArgumentNode;
    }

    public virtual FunctionExpressionNode VisitFunctionExpressionNode(FunctionExpressionNode functionExpressionNode)
    {
        return functionExpressionNode;
    }

    public virtual VariableDeclarationNode VisitVariableDeclarationNode(VariableDeclarationNode variableDeclarationNode)
    {
        variableDeclarationNode.Name.Accept(this);
        variableDeclarationNode.Value?.Accept(this);
        variableDeclarationNode.TypeName?.Accept(this);

        return variableDeclarationNode;
    }

    public virtual IdentifierNode VisitIdentifierNode(IdentifierNode identifierNode)
    {
        return identifierNode;
    }

    public virtual FunctionDeclarationNode VisitFunctionDeclarationNode(FunctionDeclarationNode functionDeclarationNode)
    {
        functionDeclarationNode.Name.Accept(this);

        foreach (var defParameterNode in functionDeclarationNode.ParameterNodes)
        {
            defParameterNode.TypeInfoNode.Accept(this);
        }

        functionDeclarationNode.Body.Accept(this);

        functionDeclarationNode.ReturnTypeInfo?.Accept(this);

        return functionDeclarationNode;
    }

    public virtual PropertySetExpressionNode VisitPropertySetExpressionNode(
        PropertySetExpressionNode propertySetExpressionNode)
    {
        return propertySetExpressionNode;
    }

    public virtual FunctionCallNode VisitFunctionCallNode(FunctionCallNode functionCallNode)
    {
        functionCallNode.Name.Accept(this);

        foreach (var argument in functionCallNode.Arguments)
        {
            argument.Accept(this);
        }

        return functionCallNode;
    }

    public virtual ImportStatementNode VisitImportStatementNode(ImportStatementNode importStatementNode)
    {
        return importStatementNode;
    }

    public virtual ExpressionNode VisitExpressionNode(ExpressionNode expressionNode)
    {
        expressionNode.Left.Accept(this);
        expressionNode.Right.Accept(this);

        return expressionNode;
    }

    public virtual ObjectDeclarationNode VisitObjectDeclarationNode(
        ObjectDeclarationNode objectDeclarationNode)
    {
        objectDeclarationNode.Name.Accept(this);
        objectDeclarationNode.BaseName.Accept(this);

        foreach (var field in objectDeclarationNode.Fields)
        {
            field.Accept(this);
        }


        return objectDeclarationNode;
    }

    public virtual FunctionDeclarationArgumentNode VisitFunctionDeclarationParameterNode(
        FunctionDeclarationArgumentNode functionDeclarationArgumentNode)
    {
        functionDeclarationArgumentNode.Name.Accept(this);
        functionDeclarationArgumentNode.TypeInfoNode.Accept(this);

        return functionDeclarationArgumentNode;
    }

    public virtual BooleanLiteralNode VisitBooleanLiteralNode(BooleanLiteralNode booleanLiteralNode)
    {
        return booleanLiteralNode;
    }

    public virtual IfStatementNode VisitIfStatementNode(IfStatementNode ifStatementNode)
    {
        ifStatementNode.Expression?.Accept(this);
        ifStatementNode.Body.Accept(this);
        ifStatementNode.NextIf?.Accept(this);

        return ifStatementNode;
    }

    public virtual FlagExpressionNode VisitFlagExpressionNode(FlagExpressionNode flagExpressionNode)
    {
        return flagExpressionNode;
    }

    public virtual StringLiteralNode VisitStringLiteralNode(StringLiteralNode stringLiteralNode)
    {
        return stringLiteralNode;
    }

    public virtual AnnotationNode VisitAnnotationNode(AnnotationNode annotationNode)
    {
        return annotationNode;
    }

    public virtual WhileStatementNode VisitWhileNode(WhileStatementNode whileStatementNode)
    {
        whileStatementNode.Expression.Accept(this);

        whileStatementNode.Body.Accept(this);

        return whileStatementNode;
    }

    public virtual ForStatementNode VisitForStatementNode(ForStatementNode forStatementNode)
    {
        forStatementNode.Iteratable.Accept(this);
        forStatementNode.Value.Accept(this);
        forStatementNode.Body.Accept(this);

        return forStatementNode;
    }

    public virtual ReturnStatementNode VisitReturnStatementNode(ReturnStatementNode returnStatementNode)
    {
        returnStatementNode.Value?.Accept(this);

        return returnStatementNode;
    }

    public virtual BreakStatementNode VisitBreakStatementNode(BreakStatementNode breakStatementNode)
    {
        return breakStatementNode;
    }

    public virtual ContinueStatementNode VisitContinueStatementNode(ContinueStatementNode continueStatementNode)
    {
        return continueStatementNode;
    }

    public virtual AssignmentNode VisitAssignmentNode(AssignmentNode assignmentNode)
    {
        assignmentNode.Name.Accept(this);
        assignmentNode.Value.Accept(this);

        return assignmentNode;
    }

    public virtual MemberAccessNode VisitMemberAccessNode(MemberAccessNode memberAccessNode)
    {
        memberAccessNode.BaseObject.Accept(this);
        memberAccessNode.Member.Accept(this);

        return memberAccessNode;
    }

    public virtual EnumDeclarationNode VisitEnumDeclarationNode(EnumDeclarationNode enumDeclarationNode)
    {
        foreach (var item in enumDeclarationNode.Items)
        {
            item.Accept(this);
        }

        return enumDeclarationNode;
    }

    public virtual StructureLiteralFieldNode VisitStructureLiteralFieldNode(
        StructureLiteralFieldNode structureLiteralFieldNode)
    {
        structureLiteralFieldNode.Name.Accept(this);
        structureLiteralFieldNode.Field.Accept(this);

        return structureLiteralFieldNode;
    }
}