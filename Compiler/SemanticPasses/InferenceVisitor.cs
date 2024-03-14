using Compiler.ErrorHandling;
using Compiler.Syntax.Nodes;
using Compiler.Syntax.Visitor;
using Compiler.TypeInformation;
using Compiler.TypeInformation.Types;

namespace Compiler.SemanticPasses;

public class InferenceVisitor : BaseNodeVisitor
{
    public Stack<TypeInfo> TypeStack { get; } = new();

    public override VariableDeclarationNode VisitVariableDeclarationNode(
        VariableDeclarationNode variableDeclarationNode)
    {
        if (variableDeclarationNode.Value == null)
        {
            return base.VisitVariableDeclarationNode(variableDeclarationNode);
        }

        if (!variableDeclarationNode.TypeRef.TypeInfo.Compare(TypeInfo.Unknown) &&
            variableDeclarationNode.Value.TypeRef.TypeInfo.HasDeferredTypes())
        {
            TypeStack.Push(variableDeclarationNode.TypeRef.TypeInfo);

            variableDeclarationNode.Value.Accept(this);

            TypeStack.Pop();
        }

        return variableDeclarationNode;
    }

    public override StructureLiteralNode VisitStructureLiteralNode(StructureLiteralNode structureLiteralNode)
    {
        var currentType = TypeStack.Peek();

        if (currentType is not StructureTypeInfo structureTypeInfo)
        {
            throw new CompileError.SemanticError("types dont match");
        }

        TypeStack.Push(structureTypeInfo);

        foreach (var field in structureLiteralNode.Fields)
        {
            if (!structureTypeInfo.Fields.TryGetValue(field.Name.Name, out var typeRef))
            {
                throw new CompileError.SemanticError("field not found");
            }

            field.Accept(this);
        }

        TypeStack.Pop();

        return structureLiteralNode;
    }

    public override StructureLiteralFieldNode VisitStructureLiteralFieldNode(
        StructureLiteralFieldNode structureLiteralFieldNode)
    {
        var currentType = TypeStack.Peek();

        if (currentType is not StructureTypeInfo structureTypeInfo)
        {
            throw new CompileError.SemanticError("types dont match");
        }

        if (!structureTypeInfo.Fields.TryGetValue(structureLiteralFieldNode.Name.Name, out var typeRef))
        {
            throw new CompileError.SemanticError("field not found");
        }

        TypeStack.Push(typeRef.TypeInfo);

        structureLiteralFieldNode.Field.Accept(this);

        TypeStack.Pop();

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
            throw new CompileError.SemanticError("array type should be generic");
        }

        if (genericTypeInfo.GenericParams.Count != 1)
        {
            throw new CompileError.SemanticError("array type should have only one parameter");
        }

        if (!genericTypeInfo.GenericParams.First().TypeInfo.Compare(TypeInfo.Deferred))
        {
            throw new CompileError.SemanticError(
                "generic parameter needs to be a deferred one or why else we are here"
            );
        }

        var currentType = TypeStack.Peek();

        if (currentType is not GenericTypeInfo peekedGenericTypeInfo)
        {
            throw new CompileError.SemanticError(
                "types don't match"
            );
        }


        genericTypeInfo.GenericParams.First().TypeInfo = peekedGenericTypeInfo.GenericParams.First().TypeInfo;

        return arrayLiteralNode;
    }
}