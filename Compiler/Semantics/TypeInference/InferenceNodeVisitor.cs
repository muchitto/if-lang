using Compiler.ErrorHandling;
using Compiler.Semantics.TypeInformation;
using Compiler.Semantics.TypeInformation.TypeComparer;
using Compiler.Semantics.TypeInformation.Types;
using Compiler.Syntax.Nodes;
using Compiler.Syntax.Visitor;

namespace Compiler.Semantics.TypeInference;

public class InferenceNodeVisitor(SemanticHandler semanticHandler) : BaseNodeVisitor(semanticHandler)
{
    public Stack<TypeInfo> TypeStack { get; } = new();
    public List<CompileError> Errors { get; } = [];

    public override VariableDeclarationNode VisitVariableDeclarationNode(
        VariableDeclarationNode variableDeclarationNode
    )
    {
        if (variableDeclarationNode.Value == null)
        {
            return base.VisitVariableDeclarationNode(variableDeclarationNode);
        }

        if (!variableDeclarationNode.TypeRef.Compare(TypeInfo.Unknown)
            && variableDeclarationNode.Value.TypeRef.HasDeferredTypes())
        {
            using (TypeStackScope(variableDeclarationNode.TypeRef.TypeInfo))
            {
                variableDeclarationNode.Value.Accept(this);
            }
        }
        else if (variableDeclarationNode.Value is EnumShortHandNode enumShortHandNode)
        {
            // we need to infer the type of the enum shorthand
            if (variableDeclarationNode.TypeInfoNode == null)
            {
                throw new CompileError.SemanticError(
                    "enum shorthand needs a type",
                    enumShortHandNode.NodeContext.PositionData
                );
            }

            using (TypeStackScope(variableDeclarationNode.TypeRef.TypeInfo))
            {
                enumShortHandNode.Accept(this);
            }
        }

        return variableDeclarationNode;
    }

    public override StructureLiteralNode VisitStructureLiteralNode(StructureLiteralNode structureLiteralNode)
    {
        var currentType = TypeStack.Peek();

        if (currentType is not StructureTypeInfo structureTypeInfo)
        {
            throw new CompileError.SemanticError(
                "types dont match",
                structureLiteralNode.NodeContext.PositionData
            );
        }

        using (TypeStackScope(structureTypeInfo))
        {
            foreach (var field in structureLiteralNode.Fields)
            {
                var typeInfoField = structureTypeInfo.GetField(field.Name.Name);

                if (typeInfoField is null)
                {
                    throw new CompileError.SemanticError(
                        "field not found",
                        field.NodeContext.PositionData
                    );
                }

                field.Accept(this);
            }
        }

        return structureLiteralNode;
    }

    public override StructureLiteralFieldNode VisitStructureLiteralFieldNode(
        StructureLiteralFieldNode structureLiteralFieldNode
    )
    {
        var currentType = TypeStack.Peek();

        if (currentType is not StructureTypeInfo structureTypeInfo)
        {
            throw new CompileError.SemanticError(
                "types dont match",
                structureLiteralFieldNode.NodeContext.PositionData
            );
        }

        var field = structureTypeInfo.GetField(structureLiteralFieldNode.Name.Name);

        if (field is not { TypeRef: var typeRef })
        {
            throw new CompileError.SemanticError(
                "field not found",
                structureLiteralFieldNode.NodeContext.PositionData
            );
        }

        using (TypeStackScope(typeRef.TypeInfo))
        {
            structureLiteralFieldNode.Field.Accept(this);
        }

        return structureLiteralFieldNode;
    }

    public override ArrayLiteralNode VisitArrayLiteralNode(ArrayLiteralNode arrayLiteralNode)
    {
        /*
         * TODO: Maybe we should figure out how to do a general generic type inference
         * function instead of always manually handling it.
         */
        if (arrayLiteralNode.TypeRef.TypeInfo is not GenericTypeInfo genericTypeInfo)
        {
            throw new CompileError.SemanticError(
                "array type should be generic",
                arrayLiteralNode.NodeContext.PositionData
            );
        }

        if (genericTypeInfo.GenericParams.Count != 1)
        {
            throw new CompileError.SemanticError(
                "array type should have only one parameter",
                arrayLiteralNode.NodeContext.PositionData
            );
        }

        if (!genericTypeInfo.GenericParams.First().Compare<BasicComparer>(TypeInfo.Deferred))
        {
            throw new CompileError.SemanticError(
                "generic parameter needs to be a deferred one or why else we are here",
                arrayLiteralNode.NodeContext.PositionData
            );
        }

        var currentType = TypeStack.Peek();

        if (currentType is not GenericTypeInfo peekedGenericTypeInfo)
        {
            throw new CompileError.SemanticError(
                "types don't match",
                arrayLiteralNode.NodeContext.PositionData
            );
        }

        genericTypeInfo.GenericParams.First().TypeInfo = peekedGenericTypeInfo.GenericParams.First().TypeInfo;

        return arrayLiteralNode;
    }

    public override EnumShortHandNode VisitEnumShortHandNode(EnumShortHandNode enumShortHandNode)
    {
        var currentType = TypeStack.Peek();

        if (currentType is not InlineEnumTypeInfo && currentType is not EnumTypeInfo)
        {
            throw new CompileError.SemanticError(
                $"cannot use enum shorthand in {currentType}",
                enumShortHandNode.NodeContext.PositionData
            );
        }

        enumShortHandNode.TypeRef.TypeInfo = currentType;

        return enumShortHandNode;
    }

    public override ObjectVariableOverride VisitObjectVariableOverride(ObjectVariableOverride objectVariableOverride)
    {
        if (!objectVariableOverride.Value.TypeRef.HasDeferredTypes())
        {
            return objectVariableOverride;
        }

        using (TypeStackScope(objectVariableOverride.TypeRef.TypeInfo))
        {
            objectVariableOverride.Value.Accept(this);
        }

        return objectVariableOverride;
    }

    public override ExpressionNode VisitExpressionNode(ExpressionNode expressionNode)
    {
        var leftDeferred = expressionNode.Left.TypeRef.HasDeferredTypes();
        var rightDeferred = expressionNode.Right.TypeRef.HasDeferredTypes();

        if (leftDeferred && rightDeferred)
        {
            throw new CompileError.SemanticError(
                "cannot infer type of both sides of the expression",
                expressionNode.NodeContext.PositionData
            );
        }

        if (leftDeferred == rightDeferred)
        {
            return expressionNode;
        }

        var targetNode = leftDeferred ? expressionNode.Left : expressionNode.Right;
        var sourceNode = leftDeferred ? expressionNode.Right : expressionNode.Left;

        using (TypeStackScope(sourceNode.TypeRef.TypeInfo))
        {
            targetNode.Accept(this);
        }

        targetNode.TypeRef = sourceNode.TypeRef;

        return expressionNode;
    }

    private ActionDisposable TypeStackScope(TypeInfo typeInfo)
    {
        TypeStack.Push(typeInfo);
        return new ActionDisposable(() => TypeStack.Pop());
    }
}