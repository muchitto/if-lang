using Compiler.ErrorHandling;
using Compiler.Semantics.TypeInformation.Types;

namespace Compiler.Semantics.TypeInformation;

public interface ITypeComparer : ITypeComparer<CompileError>
{
}

public interface ITypeComparer<TError>
{
    List<TError> Errors { get; }

    bool Compare(TypeInfo typeInfo, TypeInfo other);

    bool CompareVoidTypeInfo(VoidTypeInfo typeInfo, TypeInfo other);

    bool CompareUnknownTypeInfo(UnknownTypeInfo typeInfo, TypeInfo other);

    bool CompareFunctionTypeInfo(FunctionTypeInfo typeInfo, TypeInfo other);

    bool CompareFunctionParameterTypeInfo(FunctionParameterTypeInfo typeInfo, TypeInfo other);

    bool CompareStringTypeInfo(StringTypeInfo typeInfo, TypeInfo other);

    bool CompareNumberTypeInfo(NumberTypeInfo typeInfo, TypeInfo other);

    bool CompareBooleanTypeInfo(BooleanTypeInfo typeInfo, TypeInfo other);

    bool CompareObjectTypeInfo(ObjectTypeInfo typeInfo, TypeInfo other);

    bool CompareGenericTypeInfo(GenericTypeInfo typeInfo, TypeInfo other);

    bool CompareDeferredTypeInfo(DeferredTypeInfo typeInfo, TypeInfo other);

    bool CompareStructureTypeInfo(StructureTypeInfo typeInfo, TypeInfo other);

    bool CompareEnumTypeInfo(EnumTypeInfo typeInfo, TypeInfo other);

    bool CompareEnumItemTypeInfo(EnumItemTypeInfo typeInfo, TypeInfo other);

    bool CompareEnumItemParameterTypeInfo(EnumItemParameterTypeInfo typeInfo, TypeInfo other);

    bool CompareInlineEnumTypeInfo(InlineEnumTypeInfo typeInfo, TypeInfo other);

    bool CompareBaseEnumTypeInfo(BaseEnumTypeInfo typeInfo, TypeInfo other);

    bool CompareAbstractStructuralTypeInfo(AbstractStructuralTypeInfo typeInfo, TypeInfo other);

    bool CompareAbstractStructuralFieldTypeInfo(AbstractStructuralFieldTypeInfo typeInfo, TypeInfo other);

    bool CompareFoundationalTypeInfo(FoundationalTypeInfo foundationalTypeInfo, TypeInfo other);

    bool CompareScopedTypeInfo(ScopedTypeInfo scopedTypeInfo, TypeInfo other);
}