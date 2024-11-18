using System.Diagnostics;
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
        baseNode.Accept(this);
        return baseNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual ProgramNode VisitProgramNode(ProgramNode programNode)
    {
        using (EnterScope(ScopeType.Program, programNode))
        {
            VisitNodes(programNode.Declarations);
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
            VisitNodes(bodyBlockNode.Statements);
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
        VisitNode(functionCallArgumentNode.Value);
        return functionCallArgumentNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual VariableDeclarationNode VisitVariableDeclarationNode(VariableDeclarationNode variableDeclarationNode)
    {
        VisitNode(variableDeclarationNode.Named);
        VisitNode(variableDeclarationNode.Value);
        VisitNode(variableDeclarationNode.TypeInfoNode);
        VisitNodes(variableDeclarationNode.Annotations);
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
            VisitNode(functionDeclarationNode.Named);
            VisitNodes(functionDeclarationNode.Annotations);
            VisitNodes(functionDeclarationNode.ParameterNodes);
            VisitNode(functionDeclarationNode.Body);
        }

        VisitNode(functionDeclarationNode.ReturnTypeInfo);
        return functionDeclarationNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual FunctionCallNode VisitFunctionCallNode(FunctionCallNode functionCallNode)
    {
        VisitNodes(functionCallNode.Parameters);
        return functionCallNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual ExpressionNode VisitExpressionNode(ExpressionNode expressionNode)
    {
        VisitNode(expressionNode.Left);
        VisitNode(expressionNode.Right);
        return expressionNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual ObjectDeclarationNode VisitObjectDeclarationNode(ObjectDeclarationNode objectDeclarationNode)
    {
        using (EnterScope(ScopeType.Object, objectDeclarationNode))
        {
            VisitNode(objectDeclarationNode.Named);
            VisitNode(objectDeclarationNode.BaseName);
            VisitNodes(objectDeclarationNode.Annotations);
            VisitNodes(objectDeclarationNode.Fields);
        }

        return objectDeclarationNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual FunctionDeclarationParameterNode VisitFunctionDeclarationParameterNode(
        FunctionDeclarationParameterNode functionDeclarationParameterNode
    )
    {
        VisitNode(functionDeclarationParameterNode.Named);
        VisitNode(functionDeclarationParameterNode.TypeInfoNode);
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
        VisitNode(ifStatementNode.Expression);
        VisitNode(ifStatementNode.Body);
        VisitNode(ifStatementNode.NextIf);
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
        typeCastNode.FromTypeInfoName.Accept(this);
        typeCastNode.ToTypeInfoName.Accept(this);

        return typeCastNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual WhileStatementNode VisitWhileNode(WhileStatementNode whileStatementNode)
    {
        VisitNode(whileStatementNode.Expression);
        VisitNode(whileStatementNode.Body);
        return whileStatementNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual ForStatementNode VisitForStatementNode(ForStatementNode forStatementNode)
    {
        VisitNode(forStatementNode.Iteratable);
        VisitNode(forStatementNode.Value);
        VisitNode(forStatementNode.Body);
        return forStatementNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual ReturnStatementNode VisitReturnStatementNode(ReturnStatementNode returnStatementNode)
    {
        VisitNode(returnStatementNode.Value);
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
        VisitNode(declarationNode);
        return declarationNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual TypeInfoNode VisitTypeInfoNode(TypeInfoNode typeInfoNode)
    {
        VisitNode(typeInfoNode);
        return typeInfoNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual AssignmentNode VisitAssignmentNode(AssignmentNode assignmentNode)
    {
        VisitNode(assignmentNode.Name);
        VisitNode(assignmentNode.Value);
        return assignmentNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual MemberAccessNode VisitMemberAccessNode(MemberAccessNode memberAccessNode)
    {
        VisitNode(memberAccessNode.BaseObject);
        VisitNode(memberAccessNode.Member);
        return memberAccessNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual TypeInfoArrayNode VisitTypeInfoArrayNode(TypeInfoArrayNode typeInfoArrayNode)
    {
        VisitNode(typeInfoArrayNode.BaseType);
        return typeInfoArrayNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual TypeInfoStructureNode VisitTypeInfoStructureNode(TypeInfoStructureNode typeInfoStructureNode)
    {
        using (EnterScope(ScopeType.Object, typeInfoStructureNode))
        {
            VisitNodes(typeInfoStructureNode.Fields.Values);
        }

        return typeInfoStructureNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual StructureLiteralNode VisitStructureLiteralNode(StructureLiteralNode structureLiteralNode)
    {
        using (EnterScope(ScopeType.Object, structureLiteralNode))
        {
            VisitNodes(structureLiteralNode.Fields);
        }

        return structureLiteralNode;
    }


    [StackTraceHidden]
    [DebuggerHidden]
    public virtual ArrayLiteralNode VisitArrayLiteralNode(ArrayLiteralNode arrayLiteralNode)
    {
        VisitNodes(arrayLiteralNode.Elements);
        return arrayLiteralNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual EnumDeclarationNode VisitEnumDeclarationNode(EnumDeclarationNode enumDeclarationNode)
    {
        VisitNodes(enumDeclarationNode.Annotations);
        VisitNodes(enumDeclarationNode.Items);
        return enumDeclarationNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual LiteralNode VisitLiteralNode(LiteralNode literalNode)
    {
        VisitNode(literalNode);
        return literalNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual StructureLiteralFieldNode VisitStructureLiteralFieldNode(
        StructureLiteralFieldNode structureLiteralFieldNode
    )
    {
        VisitNode(structureLiteralFieldNode.Name);
        VisitNode(structureLiteralFieldNode.Field);
        return structureLiteralFieldNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual EnumShortHandNode VisitEnumShortHandNode(EnumShortHandNode enumShortHandNode)
    {
        VisitNode(enumShortHandNode.Named);
        VisitNodes(enumShortHandNode.Parameters);
        return enumShortHandNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual ObjectVariableOverride VisitObjectVariableOverride(ObjectVariableOverride objectVariableOverride)
    {
        VisitNode(objectVariableOverride.Named);
        VisitNode(objectVariableOverride.Value);
        return objectVariableOverride;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual UnaryExpressionNode VisitUnaryExpressionNode(UnaryExpressionNode unaryExpressionNode)
    {
        VisitNode(unaryExpressionNode.Value);
        return unaryExpressionNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual ExternNode VisitExternNode(ExternNode externNode)
    {
        VisitNode(externNode);
        return externNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual ExternFunctionNode VisitExternFunctionNode(ExternFunctionNode externFunctionNode)
    {
        VisitNode(externFunctionNode.Named);
        VisitNodes(externFunctionNode.ParameterNodes);
        VisitNode(externFunctionNode.ReturnType);
        return externFunctionNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual ExternVariableNode VisitExternVariableNode(ExternVariableNode externVariableNode)
    {
        VisitNode(externVariableNode.Named);
        VisitNode(externVariableNode.TypeInfoNode);
        return externVariableNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual EnumDeclarationItemParameterNode VisitEnumDeclarationItemParameterNode(
        EnumDeclarationItemParameterNode enumDeclarationItemParameterNode
    )
    {
        VisitNode(enumDeclarationItemParameterNode.Named);
        VisitNode(enumDeclarationItemParameterNode.TypeInfoNode);
        return enumDeclarationItemParameterNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual EnumDeclarationItemNode VisitEnumDeclarationItemNode(EnumDeclarationItemNode enumDeclarationItemNode)
    {
        VisitNode(enumDeclarationItemNode.Named);
        VisitNodes(enumDeclarationItemNode.ParameterNodes);
        return enumDeclarationItemNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual TypeInfoAnonymousEnumNode VisitTypeInfoAnonymousEnumNode(
        TypeInfoAnonymousEnumNode typeInfoAnonymousEnumNode
    )
    {
        VisitNodes(typeInfoAnonymousEnumNode.Fields);
        return typeInfoAnonymousEnumNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual TypeInfoEnumFieldParamNode VisitTypeInfoEnumFieldParamNode(
        TypeInfoEnumFieldParamNode typeInfoEnumFieldParamNode
    )
    {
        VisitNode(typeInfoEnumFieldParamNode.Named);
        VisitNode(typeInfoEnumFieldParamNode.TypeInfoNode);
        return typeInfoEnumFieldParamNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual TypeInfoEnumFieldNode VisitTypeInfoEnumFieldNode(TypeInfoEnumFieldNode typeInfoEnumFieldNode)
    {
        VisitNode(typeInfoEnumFieldNode.Named);
        VisitNodes(typeInfoEnumFieldNode.Parameters);
        return typeInfoEnumFieldNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual DeclarationNamedNode VisitDeclarationNameNode(DeclarationNamedNode declarationNamedNode)
    {
        VisitNamedNode(declarationNamedNode);
        return declarationNamedNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual ArrayAccessNode VisitArrayAccessNode(ArrayAccessNode arrayAccessNode)
    {
        VisitNode(arrayAccessNode.Array);
        VisitNode(arrayAccessNode.Accessor);
        return arrayAccessNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual OptionalTypeInfoNode VisitOptionalTypeInfoNode(OptionalTypeInfoNode optionalTypeInfoNode)
    {
        VisitNode(optionalTypeInfoNode.TypeInfo);
        return optionalTypeInfoNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual ReferenceNamedNode VisitReferenceNameNode(ReferenceNamedNode referenceNamedNode)
    {
        VisitNamedNode(referenceNamedNode);
        return referenceNamedNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual NamedNode VisitNamedNode(NamedNode namedNode)
    {
        VisitNodes(namedNode.GenericParameters);
        return namedNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual NullLiteralNode VisitNullLiteralNode(NullLiteralNode nullLiteralNode)
    {
        return nullLiteralNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    public virtual IdentifiableNode VisitIdentifiableNode(IdentifiableNode identifiableNode)
    {
        return identifiableNode;
    }

    [StackTraceHidden]
    [DebuggerHidden]
    protected void VisitNode(BaseNode? node)
    {
        node?.Accept(this);
    }

    [StackTraceHidden]
    [DebuggerHidden]
    protected void VisitNodes(IEnumerable<BaseNode> nodes)
    {
        foreach (var node in nodes)
        {
            node.Accept(this);
        }
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