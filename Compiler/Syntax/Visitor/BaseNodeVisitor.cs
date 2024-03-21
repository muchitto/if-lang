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
            BodyBlockNode bodyBlockNode => VisitBodyBlockNode(bodyBlockNode),
            FunctionCallArgumentNode functionCallArgumentNode =>
                VisitFunctionCallArgumentNode(functionCallArgumentNode),
            MemberAccessNode memberAccessNode => VisitMemberAccessNode(memberAccessNode),
            IdentifierNode identifierNode => VisitIdentifierNode(identifierNode),
            FunctionCallNode functionCallNode => VisitFunctionCallNode(functionCallNode),
            ExpressionNode expressionNode => VisitExpressionNode(expressionNode),
            FunctionDeclarationParameterNode functionDeclarationParameterNode => VisitFunctionDeclarationParameterNode(
                functionDeclarationParameterNode),
            IfStatementNode ifStatementNode => VisitIfStatementNode(ifStatementNode),
            AnnotationNode annotationNode => VisitAnnotationNode(annotationNode),
            WhileStatementNode whileStatementNode => VisitWhileNode(whileStatementNode),
            ForStatementNode forStatementNode => VisitForStatementNode(forStatementNode),
            ReturnStatementNode returnStatementNode => VisitReturnStatementNode(returnStatementNode),
            BreakStatementNode breakStatementNode => VisitBreakStatementNode(breakStatementNode),
            ContinueStatementNode continueStatementNode => VisitContinueStatementNode(continueStatementNode),
            DeclarationNode declarationNode => VisitDeclarationNode(declarationNode),
            AssignmentNode assignmentNode => VisitAssignmentNode(assignmentNode),
            LiteralNode literalNode => VisitLiteralNode(literalNode),
            EnumShortHandNode enumShortHandNode => VisitEnumShortHandNode(enumShortHandNode),
            UnaryExpressionNode unaryExpressionNode => VisitUnaryExpressionNode(unaryExpressionNode),
            ArrayAccessNode arrayAccessNode => VisitArrayAccessNode(arrayAccessNode),

            _ => throw new NotImplementedException()
        };
    }

    public virtual DeclarationNode VisitDeclarationNode(DeclarationNode declarationNode)
    {
        return declarationNode switch
        {
            VariableDeclarationNode variableDeclaration => VisitVariableDeclarationNode(variableDeclaration),
            FunctionDeclarationNode functionDeclarationNode => VisitFunctionDeclarationNode(functionDeclarationNode),
            ObjectDeclarationNode objectDeclarationNode => VisitObjectDeclarationNode(objectDeclarationNode),
            EnumDeclarationNode enumDeclarationNode => VisitEnumDeclarationNode(enumDeclarationNode),
            ObjectVariableOverride objectVariableOverride => VisitObjectVariableOverride(objectVariableOverride),
            ExternNode externNode => VisitExternNode(externNode),
            _ => throw new VisitorError("unhandled declaration node")
        };
    }

    public virtual EnumShortHandNode VisitEnumShortHandNode(EnumShortHandNode enumShortHandNode)
    {
        enumShortHandNode.Name.Accept(this);

        foreach (var parameter in enumShortHandNode.Parameters)
        {
            parameter.Accept(this);
        }

        return enumShortHandNode;
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
            NullLiteralNode nullLiteralNode => nullLiteralNode,
            _ => throw new VisitorError("unhandled literal node")
        };
    }

    public virtual TypeInfoNode VisitTypeInfoNode(TypeInfoNode typeInfoNode)
    {
        return typeInfoNode switch
        {
            TypeInfoNameNode typeInfoNameNode => VisitTypeNameNode(typeInfoNameNode),
            TypeInfoArrayNode typeInfoArrayNode => VisitTypeInfoArrayNode(typeInfoArrayNode),
            TypeInfoStructureNode typeInfoStructureNode => VisitTypeInfoStructureNode(typeInfoStructureNode),
            TypeInfoEnumNode typeInfoEnumNode => VisitTypeInfoEnumNode(typeInfoEnumNode),
            TypeInfoEnumFieldNode typeInfoEnumFieldNode => VisitTypeInfoEnumFieldNode(typeInfoEnumFieldNode),
            TypeInfoEnumFieldParamNode typeInfoEnumFieldParamNode => VisitTypeInfoEnumFieldParamNode(
                typeInfoEnumFieldParamNode),
            OptionalTypeInfoNode optionalTypeInfoNode => VisitOptionalTypeInfoNode(optionalTypeInfoNode),
            _ => throw new VisitorError("unhandled type info node")
        };
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


    public virtual VariableDeclarationNode VisitVariableDeclarationNode(VariableDeclarationNode variableDeclarationNode)
    {
        variableDeclarationNode.Name.Accept(this);
        variableDeclarationNode.Value?.Accept(this);
        variableDeclarationNode.TypeInfo?.Accept(this);

        foreach (var annotation in variableDeclarationNode.Annotations)
        {
            annotation.Accept(this);
        }

        return variableDeclarationNode;
    }

    public virtual IdentifierNode VisitIdentifierNode(IdentifierNode identifierNode)
    {
        return identifierNode;
    }

    public virtual FunctionDeclarationNode VisitFunctionDeclarationNode(FunctionDeclarationNode functionDeclarationNode)
    {
        functionDeclarationNode.Name.Accept(this);

        foreach (var annotation in functionDeclarationNode.Annotations)
        {
            annotation.Accept(this);
        }

        foreach (var functionDeclarationParameterNode in functionDeclarationNode.ParameterNodes)
        {
            functionDeclarationParameterNode.TypeInfoNode.Accept(this);
        }

        functionDeclarationNode.Body.Accept(this);

        functionDeclarationNode.ReturnTypeInfo?.Accept(this);

        return functionDeclarationNode;
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
        objectDeclarationNode.BaseName?.Accept(this);

        foreach (var annotation in objectDeclarationNode.Annotations)
        {
            annotation.Accept(this);
        }

        foreach (var field in objectDeclarationNode.Fields)
        {
            field.Accept(this);
        }


        return objectDeclarationNode;
    }

    public virtual FunctionDeclarationParameterNode VisitFunctionDeclarationParameterNode(
        FunctionDeclarationParameterNode functionDeclarationParameterNode)
    {
        functionDeclarationParameterNode.Name.Accept(this);
        functionDeclarationParameterNode.TypeInfoNode.Accept(this);

        return functionDeclarationParameterNode;
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
        foreach (var annotation in enumDeclarationNode.Annotations)
        {
            annotation.Accept(this);
        }

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

    public virtual ObjectVariableOverride VisitObjectVariableOverride(ObjectVariableOverride objectVariableOverride)
    {
        objectVariableOverride.Name.Accept(this);
        objectVariableOverride.Value.Accept(this);

        return objectVariableOverride;
    }

    public virtual UnaryExpressionNode VisitUnaryExpressionNode(UnaryExpressionNode unaryExpressionNode)
    {
        unaryExpressionNode.Value.Accept(this);

        return unaryExpressionNode;
    }

    public virtual ExternNode VisitExternNode(ExternNode externNode)
    {
        return externNode switch
        {
            ExternFunctionNode externFunctionNode => VisitExternFunctionNode(externFunctionNode),
            ExternVariableNode externVariableNode => VisitExternVariableNode(externVariableNode),
            _ => throw new VisitorError("unhandled extern node")
        };
    }

    public virtual ExternFunctionNode VisitExternFunctionNode(ExternFunctionNode externFunctionNode)
    {
        externFunctionNode.Name.Accept(this);
        foreach (var argument in externFunctionNode.ParameterNodes)
        {
            argument.Accept(this);
        }

        externFunctionNode.ReturnType?.Accept(this);

        return externFunctionNode;
    }

    public virtual ExternVariableNode VisitExternVariableNode(ExternVariableNode externVariableNode)
    {
        externVariableNode.Name.Accept(this);
        externVariableNode.TypeInfoNode.Accept(this);

        return externVariableNode;
    }

    public virtual EnumDeclarationItemNode VisitEnumDeclarationItemNode(EnumDeclarationItemNode enumDeclarationItemNode)
    {
        enumDeclarationItemNode.Name.Accept(this);
        foreach (var parameter in enumDeclarationItemNode.ParameterNodes)
        {
            parameter.Accept(this);
        }

        return enumDeclarationItemNode;
    }

    public virtual EnumDeclarationItemParameterNode VisitEnumDeclarationItemParameterNode(
        EnumDeclarationItemParameterNode enumDeclarationItemParameterNode)
    {
        enumDeclarationItemParameterNode.Name.Accept(this);
        enumDeclarationItemParameterNode.TypeInfoNode.Accept(this);

        return enumDeclarationItemParameterNode;
    }

    public virtual TypeInfoEnumNode VisitTypeInfoEnumNode(TypeInfoEnumNode typeInfoEnumNode)
    {
        foreach (var field in typeInfoEnumNode.Fields)
        {
            field.Accept(this);
        }

        return typeInfoEnumNode;
    }

    public virtual TypeInfoEnumFieldParamNode VisitTypeInfoEnumFieldParamNode(
        TypeInfoEnumFieldParamNode typeInfoEnumFieldParamNode)
    {
        typeInfoEnumFieldParamNode.Name.Accept(this);
        typeInfoEnumFieldParamNode.TypeInfoNode.Accept(this);

        return typeInfoEnumFieldParamNode;
    }

    public virtual TypeInfoEnumFieldNode VisitTypeInfoEnumFieldNode(TypeInfoEnumFieldNode typeInfoEnumFieldNode)
    {
        typeInfoEnumFieldNode.Name.Accept(this);

        foreach (var parameter in typeInfoEnumFieldNode.Parameters)
        {
            parameter.Accept(this);
        }

        return typeInfoEnumFieldNode;
    }

    public virtual DeclarationNameNode VisitDeclarationNameNode(DeclarationNameNode declarationNameNode)
    {
        return declarationNameNode;
    }

    public virtual ArrayAccessNode VisitArrayAccessNode(ArrayAccessNode arrayAccessNode)
    {
        arrayAccessNode.Array.Accept(this);
        arrayAccessNode.Accessor.Accept(this);

        return arrayAccessNode;
    }

    public virtual OptionalTypeInfoNode VisitOptionalTypeInfoNode(OptionalTypeInfoNode optionalTypeInfoNode)
    {
        optionalTypeInfoNode.TypeInfo.Accept(this);

        return optionalTypeInfoNode;
    }
}