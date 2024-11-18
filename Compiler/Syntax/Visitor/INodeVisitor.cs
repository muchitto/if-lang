using Compiler.Syntax.Nodes;
using Compiler.Syntax.Nodes.TypeInfoNodes;

namespace Compiler.Syntax.Visitor;

public interface INodeVisitor
{
    public ProgramNode VisitProgramNode(ProgramNode programNode);
    public TypeInfoNameNode VisitTypeInfoNameNode(TypeInfoNameNode typeInfoNameNode);
    public BodyBlockNode VisitBodyBlockNode(BodyBlockNode bodyBlockNode);
    public NumberLiteralNode VisitNumberLiteralNode(NumberLiteralNode numberLiteralNode);
    public FunctionCallArgumentNode VisitFunctionCallArgumentNode(FunctionCallArgumentNode functionCallArgumentNode);
    public VariableDeclarationNode VisitVariableDeclarationNode(VariableDeclarationNode variableDeclarationNode);
    public IdentifierNode VisitIdentifierNode(IdentifierNode identifierNode);

    public FunctionDeclarationNode VisitFunctionDeclarationNode(FunctionDeclarationNode functionDeclarationNode);


    public FunctionCallNode VisitFunctionCallNode(FunctionCallNode functionCallNode);
    public ExpressionNode VisitExpressionNode(ExpressionNode expressionNode);
    public ObjectDeclarationNode VisitObjectDeclarationNode(ObjectDeclarationNode objectDeclarationNode);

    public FunctionDeclarationParameterNode VisitFunctionDeclarationParameterNode(
        FunctionDeclarationParameterNode functionDeclarationParameterNode
    );

    public BooleanLiteralNode VisitBooleanLiteralNode(BooleanLiteralNode booleanLiteralNode);
    public IfStatementNode VisitIfStatementNode(IfStatementNode ifStatementNode);
    public StringLiteralNode VisitStringLiteralNode(StringLiteralNode stringLiteralNode);
    public AnnotationNode VisitAnnotationNode(AnnotationNode annotationNode);

    public WhileStatementNode VisitWhileNode(WhileStatementNode whileStatementNode);

    public ForStatementNode VisitForStatementNode(ForStatementNode forStatementNode);

    public ReturnStatementNode VisitReturnStatementNode(ReturnStatementNode returnStatementNode);

    public BreakStatementNode VisitBreakStatementNode(BreakStatementNode breakStatementNode);

    public ContinueStatementNode VisitContinueStatementNode(ContinueStatementNode continueStatementNode);

    public DeclarationNode VisitDeclarationNode(DeclarationNode declarationNode);

    public BaseNode VisitBaseNode(BaseNode baseNode);

    public TypeInfoNode VisitTypeInfoNode(TypeInfoNode typeInfoNode);

    public AssignmentNode VisitAssignmentNode(AssignmentNode assignmentNode);

    public MemberAccessNode VisitMemberAccessNode(MemberAccessNode memberAccessNode);

    public TypeInfoArrayNode VisitTypeInfoArrayNode(TypeInfoArrayNode typeInfoArrayNode);

    public TypeInfoStructureNode VisitTypeInfoStructureNode(TypeInfoStructureNode typeInfoStructureNode);

    public StructureLiteralNode VisitStructureLiteralNode(StructureLiteralNode structureLiteralNode);

    public ArrayLiteralNode VisitArrayLiteralNode(ArrayLiteralNode arrayLiteralNode);

    public EnumDeclarationNode VisitEnumDeclarationNode(EnumDeclarationNode enumDeclarationNode);

    public LiteralNode VisitLiteralNode(LiteralNode literalNode);

    public StructureLiteralFieldNode
        VisitStructureLiteralFieldNode(StructureLiteralFieldNode structureLiteralFieldNode);

    public EnumShortHandNode VisitEnumShortHandNode(EnumShortHandNode enumShortHandNode);

    public ObjectVariableOverride VisitObjectVariableOverride(ObjectVariableOverride objectVariableOverride);

    public UnaryExpressionNode VisitUnaryExpressionNode(UnaryExpressionNode unaryExpressionNode);

    public ExternNode VisitExternNode(ExternNode externNode);

    public ExternFunctionNode VisitExternFunctionNode(ExternFunctionNode externFunctionNode);

    public ExternVariableNode VisitExternVariableNode(ExternVariableNode externVariableNode);

    public EnumDeclarationItemParameterNode VisitEnumDeclarationItemParameterNode(
        EnumDeclarationItemParameterNode enumDeclarationItemParameterNode
    );

    public EnumDeclarationItemNode VisitEnumDeclarationItemNode(EnumDeclarationItemNode enumDeclarationItemNode);

    public TypeInfoAnonymousEnumNode
        VisitTypeInfoAnonymousEnumNode(TypeInfoAnonymousEnumNode typeInfoAnonymousEnumNode);

    public TypeInfoEnumFieldParamNode VisitTypeInfoEnumFieldParamNode(
        TypeInfoEnumFieldParamNode typeInfoEnumFieldParamNode
    );

    public TypeInfoEnumFieldNode VisitTypeInfoEnumFieldNode(TypeInfoEnumFieldNode typeInfoEnumFieldNode);

    public DeclarationNamedNode VisitDeclarationNameNode(DeclarationNamedNode declarationNamedNode);

    public ArrayAccessNode VisitArrayAccessNode(ArrayAccessNode arrayAccessNode);

    public OptionalTypeInfoNode VisitOptionalTypeInfoNode(OptionalTypeInfoNode optionalTypeInfoNode);

    public ReferenceNamedNode VisitReferenceNameNode(ReferenceNamedNode referenceNamedNode);

    public NamedNode VisitNamedNode(NamedNode namedNode);

    public NullLiteralNode VisitNullLiteralNode(NullLiteralNode nullLiteralNode);

    public IdentifiableNode VisitIdentifiableNode(IdentifiableNode identifiableNode);

    public TypeCastNode VisitTypeCastNode(TypeCastNode typeCastNode);
}