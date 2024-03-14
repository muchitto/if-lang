using Compiler.Syntax.Nodes;

namespace Compiler.Syntax.Visitor;

public interface INodeVisitor
{
    public ProgramNode VisitProgramNode(ProgramNode programNode);
    public TypeInfoNameNode VisitTypeNameNode(TypeInfoNameNode typeInfoNameNode);
    public WithExpressionNode VisitWithExpressionNode(WithExpressionNode withExpressionNode);
    public BodyBlockNode VisitBodyBlockNode(BodyBlockNode bodyBlockNode);
    public NumberLiteralNode VisitNumberNode(NumberLiteralNode numberLiteralNode);
    public FunctionCallArgumentNode VisitFunctionCallArgumentNode(FunctionCallArgumentNode functionCallArgumentNode);
    public FunctionExpressionNode VisitFunctionExpressionNode(FunctionExpressionNode functionExpressionNode);
    public VariableDeclarationNode VisitVariableDeclarationNode(VariableDeclarationNode variableDeclarationNode);
    public IdentifierNode VisitIdentifierNode(IdentifierNode identifierNode);

    public FunctionDeclarationNode VisitFunctionDeclarationNode(FunctionDeclarationNode functionDeclarationNode);

    public PropertySetExpressionNode
        VisitPropertySetExpressionNode(PropertySetExpressionNode propertySetExpressionNode);

    public FunctionCallNode VisitFunctionCallNode(FunctionCallNode functionCallNode);
    public ImportStatementNode VisitImportStatementNode(ImportStatementNode importStatementNode);
    public ExpressionNode VisitExpressionNode(ExpressionNode expressionNode);
    public ObjectDeclarationNode VisitObjectDeclarationNode(ObjectDeclarationNode objectDeclarationNode);

    public FunctionDeclarationArgumentNode VisitFunctionDeclarationParameterNode(
        FunctionDeclarationArgumentNode functionDeclarationArgumentNode);

    public BooleanLiteralNode VisitBooleanLiteralNode(BooleanLiteralNode booleanLiteralNode);
    public IfStatementNode VisitIfStatementNode(IfStatementNode ifStatementNode);
    public FlagExpressionNode VisitFlagExpressionNode(FlagExpressionNode flagExpressionNode);
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
}