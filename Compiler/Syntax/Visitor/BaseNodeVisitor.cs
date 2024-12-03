using System.Diagnostics;
using System.Diagnostics.Contracts;
using Compiler.Semantics;
using Compiler.Semantics.ScopeHandling;
using Compiler.Syntax.Nodes;
using Compiler.Syntax.Nodes.TypeInfoNodes;

namespace Compiler.Syntax.Visitor;

public class VisitorError : Exception
{
    public VisitorError(string message) : base(message)
    {
    }
}

public abstract class BaseNodeVisitor : INodeVisitor
{
    protected BaseNodeVisitor(SemanticHandler semanticHandler, BaseScopeHandler scopeHandler)
    {
        SemanticHandler = semanticHandler;
        ScopeHandler = scopeHandler;
    }

    protected BaseNodeVisitor(SemanticHandler semanticHandler) : this(semanticHandler,
        new DoNothingScopeHandler(semanticHandler))
    {
    }

    protected SemanticContext SemanticContext => SemanticHandler.SemanticContext;
    protected SemanticHandler SemanticHandler { get; init; }

    protected BaseScopeHandler ScopeHandler { get; init; }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual BaseNode VisitBaseNode(BaseNode baseNode)
    {
        return baseNode.Accept(this);
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual ProgramNode VisitProgramNode(ProgramNode programNode)
    {
        using (EnterScope(ScopeType.Program, programNode))
        {
            programNode.Declarations = VisitNodes(programNode.Declarations);
        }

        return programNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual TypeInfoNameNode VisitTypeInfoNameNode(TypeInfoNameNode typeInfoNameNode)
    {
        return typeInfoNameNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual BodyBlockNode VisitBodyBlockNode(BodyBlockNode bodyBlockNode)
    {
        using (EnterScope(ScopeType.BodyBlock, bodyBlockNode))
        {
            bodyBlockNode.Statements = VisitNodes(bodyBlockNode.Statements);
        }

        return bodyBlockNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual NumberLiteralNode VisitNumberLiteralNode(NumberLiteralNode numberLiteralNode)
    {
        return numberLiteralNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual FunctionCallArgumentNode VisitFunctionCallArgumentNode(
        FunctionCallArgumentNode functionCallArgumentNode
    )
    {
        functionCallArgumentNode.Value = VisitNode(functionCallArgumentNode.Value);

        return functionCallArgumentNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual VariableDeclarationNode VisitVariableDeclarationNode(VariableDeclarationNode variableDeclarationNode)
    {
        variableDeclarationNode.Named = VisitNode(variableDeclarationNode.Named);
        variableDeclarationNode.Value = VisitNode(variableDeclarationNode.Value);
        variableDeclarationNode.TypeInfoNode = VisitNode(variableDeclarationNode.TypeInfoNode);
        variableDeclarationNode.Annotations = VisitNodes(variableDeclarationNode.Annotations);
        return variableDeclarationNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual IdentifierNode VisitIdentifierNode(IdentifierNode identifierNode)
    {
        return identifierNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual FunctionDeclarationNode VisitFunctionDeclarationNode(FunctionDeclarationNode functionDeclarationNode)
    {
        using (EnterScope(ScopeType.Function, functionDeclarationNode))
        {
            functionDeclarationNode.Named = VisitNode(functionDeclarationNode.Named);
            functionDeclarationNode.Annotations = VisitNodes(functionDeclarationNode.Annotations);
            functionDeclarationNode.ParameterNodes = VisitNodes(functionDeclarationNode.ParameterNodes);
            functionDeclarationNode.Body = VisitNode(functionDeclarationNode.Body);
        }

        functionDeclarationNode.ReturnTypeInfo = VisitNode(functionDeclarationNode.ReturnTypeInfo);
        return functionDeclarationNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual FunctionCallNode VisitFunctionCallNode(FunctionCallNode functionCallNode)
    {
        functionCallNode.Parameters = VisitNodes(functionCallNode.Parameters);
        return functionCallNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual ExpressionNode VisitExpressionNode(ExpressionNode expressionNode)
    {
        expressionNode.Left = VisitNode(expressionNode.Left);
        expressionNode.Right = VisitNode(expressionNode.Right);
        return expressionNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual ObjectDeclarationNode VisitObjectDeclarationNode(ObjectDeclarationNode objectDeclarationNode)
    {
        using (EnterScope(ScopeType.Object, objectDeclarationNode))
        {
            objectDeclarationNode.Named = VisitNode(objectDeclarationNode.Named);
            objectDeclarationNode.BaseName = VisitNode(objectDeclarationNode.BaseName);
            objectDeclarationNode.Annotations = VisitNodes(objectDeclarationNode.Annotations);
            objectDeclarationNode.Fields = VisitNodes(objectDeclarationNode.Fields);
        }

        return objectDeclarationNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual FunctionDeclarationParameterNode VisitFunctionDeclarationParameterNode(
        FunctionDeclarationParameterNode functionDeclarationParameterNode
    )
    {
        functionDeclarationParameterNode.Named = VisitNode(functionDeclarationParameterNode.Named);
        functionDeclarationParameterNode.TypeInfoNode = VisitNode(functionDeclarationParameterNode.TypeInfoNode);
        return functionDeclarationParameterNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual BooleanLiteralNode VisitBooleanLiteralNode(BooleanLiteralNode booleanLiteralNode)
    {
        return booleanLiteralNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual IfStatementNode VisitIfStatementNode(IfStatementNode ifStatementNode)
    {
        ifStatementNode.Expression = VisitNode(ifStatementNode.Expression);
        ifStatementNode.Body = VisitNode(ifStatementNode.Body);
        ifStatementNode.NextIf = VisitNode(ifStatementNode.NextIf);
        return ifStatementNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual StringLiteralNode VisitStringLiteralNode(StringLiteralNode stringLiteralNode)
    {
        return stringLiteralNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual AnnotationNode VisitAnnotationNode(AnnotationNode annotationNode)
    {
        return annotationNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual TypeCastNode VisitTypeCastNode(TypeCastNode typeCastNode)
    {
        typeCastNode.FromTypeInfoName = VisitNode(typeCastNode.FromTypeInfoName);
        typeCastNode.ToTypeInfoName = VisitNode(typeCastNode.ToTypeInfoName);

        return typeCastNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public ObjectInitializationNode VisitObjectInitializationNode(ObjectInitializationNode objectInitializationNode)
    {
        objectInitializationNode.Parameters = VisitNodes(objectInitializationNode.Parameters);
        return objectInitializationNode;
    }


    [StackTraceHidden]
    [DebuggerHidden]
    public virtual WhileStatementNode VisitWhileNode(WhileStatementNode whileStatementNode)
    {
        whileStatementNode.Expression = VisitNode(whileStatementNode.Expression);
        whileStatementNode.Body = VisitNode(whileStatementNode.Body);
        return whileStatementNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual ForStatementNode VisitForStatementNode(ForStatementNode forStatementNode)
    {
        forStatementNode.Iteratable = VisitNode(forStatementNode.Iteratable);
        forStatementNode.Value = VisitNode(forStatementNode.Value);
        forStatementNode.Body = VisitNode(forStatementNode.Body);
        return forStatementNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual ReturnStatementNode VisitReturnStatementNode(ReturnStatementNode returnStatementNode)
    {
        returnStatementNode.Value = VisitNode(returnStatementNode.Value);
        return returnStatementNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual BreakStatementNode VisitBreakStatementNode(BreakStatementNode breakStatementNode)
    {
        return breakStatementNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual ContinueStatementNode VisitContinueStatementNode(ContinueStatementNode continueStatementNode)
    {
        return continueStatementNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual DeclarationNode VisitDeclarationNode(DeclarationNode declarationNode)
    {
        return VisitNode(declarationNode);
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual TypeInfoNode VisitTypeInfoNode(TypeInfoNode typeInfoNode)
    {
        return VisitNode(typeInfoNode);
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual AssignmentNode VisitAssignmentNode(AssignmentNode assignmentNode)
    {
        assignmentNode.Name = VisitNode(assignmentNode.Name);
        assignmentNode.Value = VisitNode(assignmentNode.Value);

        return assignmentNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual FieldAccessNode VisitFieldAccessNode(FieldAccessNode fieldAccessNode)
    {
        fieldAccessNode.BaseObjectName = VisitNode(fieldAccessNode.BaseObjectName);
        fieldAccessNode.Member = VisitNode(fieldAccessNode.Member);
        return fieldAccessNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual TypeInfoArrayNode VisitTypeInfoArrayNode(TypeInfoArrayNode typeInfoArrayNode)
    {
        typeInfoArrayNode.BaseType = VisitNode(typeInfoArrayNode.BaseType);

        return typeInfoArrayNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual TypeInfoStructureNode VisitTypeInfoStructureNode(TypeInfoStructureNode typeInfoStructureNode)
    {
        using (EnterScope(ScopeType.Object, typeInfoStructureNode))
        {
            typeInfoStructureNode.Fields = VisitNodes(typeInfoStructureNode.Fields);
        }

        return typeInfoStructureNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual StructureLiteralNode VisitStructureLiteralNode(StructureLiteralNode structureLiteralNode)
    {
        using (EnterScope(ScopeType.Object, structureLiteralNode))
        {
            structureLiteralNode.Fields = VisitNodes(structureLiteralNode.Fields);
        }

        return structureLiteralNode;
    }


    [StackTraceHidden]
    [DebuggerHidden]
    public virtual ArrayLiteralNode VisitArrayLiteralNode(ArrayLiteralNode arrayLiteralNode)
    {
        arrayLiteralNode.Elements = VisitNodes(arrayLiteralNode.Elements);
        return arrayLiteralNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual EnumDeclarationNode VisitEnumDeclarationNode(EnumDeclarationNode enumDeclarationNode)
    {
        enumDeclarationNode.Annotations = VisitNodes(enumDeclarationNode.Annotations);
        enumDeclarationNode.Items = VisitNodes(enumDeclarationNode.Items);
        return enumDeclarationNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual LiteralNode VisitLiteralNode(LiteralNode literalNode)
    {
        return literalNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual StructureLiteralFieldNode VisitStructureLiteralFieldNode(
        StructureLiteralFieldNode structureLiteralFieldNode
    )
    {
        structureLiteralFieldNode.Name = VisitNode(structureLiteralFieldNode.Name);
        structureLiteralFieldNode.Field = VisitNode(structureLiteralFieldNode.Field);
        return structureLiteralFieldNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual EnumShortHandNode VisitEnumShortHandNode(EnumShortHandNode enumShortHandNode)
    {
        enumShortHandNode.Named = VisitNode(enumShortHandNode.Named);
        enumShortHandNode.Parameters = VisitNodes(enumShortHandNode.Parameters);
        return enumShortHandNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual ObjectVariableOverride VisitObjectVariableOverride(ObjectVariableOverride objectVariableOverride)
    {
        objectVariableOverride.Named = VisitNode(objectVariableOverride.Named);
        objectVariableOverride.Value = VisitNode(objectVariableOverride.Value);
        return objectVariableOverride;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual UnaryExpressionNode VisitUnaryExpressionNode(UnaryExpressionNode unaryExpressionNode)
    {
        unaryExpressionNode.Value = VisitNode(unaryExpressionNode.Value);
        return unaryExpressionNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual ExternNode VisitExternNode(ExternNode externNode)
    {
        return externNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual ExternFunctionNode VisitExternFunctionNode(ExternFunctionNode externFunctionNode)
    {
        externFunctionNode.Named = VisitNode(externFunctionNode.Named);
        externFunctionNode.ParameterNodes = VisitNodes(externFunctionNode.ParameterNodes);
        externFunctionNode.ReturnType = VisitNode(externFunctionNode.ReturnType);
        return externFunctionNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual ExternVariableNode VisitExternVariableNode(ExternVariableNode externVariableNode)
    {
        externVariableNode.Named = VisitNode(externVariableNode.Named);
        externVariableNode.TypeInfoNode = VisitNode(externVariableNode.TypeInfoNode);
        return externVariableNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual EnumDeclarationItemParameterNode VisitEnumDeclarationItemParameterNode(
        EnumDeclarationItemParameterNode enumDeclarationItemParameterNode
    )
    {
        enumDeclarationItemParameterNode.Named = VisitNode(enumDeclarationItemParameterNode.Named);
        enumDeclarationItemParameterNode.TypeInfoNode = VisitNode(enumDeclarationItemParameterNode.TypeInfoNode);
        return enumDeclarationItemParameterNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual EnumDeclarationItemNode VisitEnumDeclarationItemNode(EnumDeclarationItemNode enumDeclarationItemNode)
    {
        enumDeclarationItemNode.Named = VisitNode(enumDeclarationItemNode.Named);
        enumDeclarationItemNode.ParameterNodes = VisitNodes(enumDeclarationItemNode.ParameterNodes);
        return enumDeclarationItemNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual TypeInfoInlineEnumNode VisitTypeInfoInlineEnumNode(
        TypeInfoInlineEnumNode typeInfoInlineEnumNode
    )
    {
        typeInfoInlineEnumNode.Fields = VisitNodes(typeInfoInlineEnumNode.Fields);
        return typeInfoInlineEnumNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual TypeInfoEnumFieldParamNode VisitTypeInfoEnumFieldParamNode(
        TypeInfoEnumFieldParamNode typeInfoEnumFieldParamNode
    )
    {
        typeInfoEnumFieldParamNode.Named = VisitNode(typeInfoEnumFieldParamNode.Named);
        typeInfoEnumFieldParamNode.TypeInfoNode = VisitNode(typeInfoEnumFieldParamNode.TypeInfoNode);
        return typeInfoEnumFieldParamNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual TypeInfoEnumFieldNode VisitTypeInfoEnumFieldNode(TypeInfoEnumFieldNode typeInfoEnumFieldNode)
    {
        typeInfoEnumFieldNode.Named = VisitNode(typeInfoEnumFieldNode.Named);
        typeInfoEnumFieldNode.Parameters = VisitNodes(typeInfoEnumFieldNode.Parameters);

        return typeInfoEnumFieldNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual DeclarationNamedNode VisitDeclarationNameNode(DeclarationNamedNode declarationNamedNode)
    {
        return declarationNamedNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual ArrayAccessNode VisitArrayAccessNode(ArrayAccessNode arrayAccessNode)
    {
        arrayAccessNode.Array = VisitNode(arrayAccessNode.Array);
        arrayAccessNode.Accessor = VisitNode(arrayAccessNode.Accessor);

        return arrayAccessNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual OptionalTypeInfoNode VisitOptionalTypeInfoNode(OptionalTypeInfoNode optionalTypeInfoNode)
    {
        optionalTypeInfoNode.TypeInfo = VisitNode(optionalTypeInfoNode.TypeInfo);

        return optionalTypeInfoNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual ReferenceNamedNode VisitReferenceNameNode(ReferenceNamedNode referenceNamedNode)
    {
        referenceNamedNode.GenericParameters = VisitNodes(referenceNamedNode.GenericParameters);

        return referenceNamedNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual NamedNode VisitNamedNode(NamedNode namedNode)
    {
        namedNode.GenericParameters = VisitNodes(namedNode.GenericParameters);

        return namedNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual NullLiteralNode VisitNullLiteralNode(NullLiteralNode nullLiteralNode)
    {
        return nullLiteralNode;
    }

    public virtual ObjectFieldAccessNode VisitObjectFieldAccessNode(ObjectFieldAccessNode objectFieldAccessNode)
    {
        objectFieldAccessNode.BaseObjectName = VisitNode(objectFieldAccessNode.BaseObjectName);
        objectFieldAccessNode.Member = VisitNode(objectFieldAccessNode.Member);

        return objectFieldAccessNode;
    }

    public virtual TypeFieldAccessNode VisitTypeFieldAccessNode(TypeFieldAccessNode typeFieldAccessNode)
    {
        typeFieldAccessNode.BaseTypeNode = VisitNode(typeFieldAccessNode.BaseTypeNode);
        typeFieldAccessNode.Member = VisitNode(typeFieldAccessNode.Member);

        return typeFieldAccessNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual IdentifiableNode VisitIdentifiableNode(IdentifiableNode identifiableNode)
    {
        return identifiableNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    [Pure]
    protected T? VisitNode<T>(T? node) where T : BaseNode
    {
        return (T)node?.Accept(this);
    }

    [StackTraceHidden]
    [DebuggerHidden]
    [Pure]
    protected List<T> VisitNodes<T>(IEnumerable<T> nodes) where T : BaseNode
    {
        return nodes.Select(node => (T)node.Accept(this)).ToList();
    }

    protected ActionItemDisposable<Scope> EnterScope(ScopeType scopeType, BaseNode baseNode)
    {
        return EnterScope(scopeType, baseNode, out _);
    }

    protected ActionItemDisposable<Scope> EnterScope(ScopeType scopeType, BaseNode baseNode, out Scope scope)
    {
        scope = ScopeHandler.EnterScope(scopeType, baseNode);

        return new ActionItemDisposable<Scope>(scope, _ => ScopeHandler.ExitScope());
    }

    protected ActionItemDisposable<Scope> MustRecallScope(BaseNode baseNode)
    {
        return MustRecallScope(baseNode, out _);
    }

    protected ActionItemDisposable<Scope> MustRecallScope(BaseNode baseNode, out Scope scope)
    {
        scope = ScopeHandler.MustRecallScope(baseNode);

        return new ActionItemDisposable<Scope>(scope, _ => ScopeHandler.ExitScope());
    }

    protected ActionItemDisposable<Scope> MustRecallScope(Scope scope)
    {
        ScopeHandler.MustRecallScope(scope);

        return new ActionItemDisposable<Scope>(scope, _ => ScopeHandler.ExitScope());
    }
}