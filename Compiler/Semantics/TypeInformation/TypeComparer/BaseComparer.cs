using Compiler.ErrorHandling;
using Compiler.Semantics.TypeInformation.Types;

namespace Compiler.Semantics.TypeInformation.TypeComparer;

public abstract class BaseComparer : ITypeComparer
{
    public List<CompileError> Errors { get; } = [];

    public virtual bool Compare(TypeInfo typeInfo, TypeInfo other)
    {
        return typeInfo.Compare(this, other);
    }

    public virtual bool CompareVoidTypeInfo(VoidTypeInfo typeInfo, TypeInfo other)
    {
        return other is VoidTypeInfo;
    }

    public virtual bool CompareUnknownTypeInfo(UnknownTypeInfo typeInfo, TypeInfo other)
    {
        return other is UnknownTypeInfo;
    }

    public virtual bool CompareFunctionTypeInfo(FunctionTypeInfo typeInfo, TypeInfo other)
    {
        if (other is not FunctionTypeInfo functionTypeInfo ||
            typeInfo.ReturnType.TypeInfo.Compare(this, functionTypeInfo.ReturnType.TypeInfo) ||
            typeInfo.Parameters.Count != functionTypeInfo.Parameters.Count)
        {
            return false;
        }

        for (var i = 0; i < typeInfo.Parameters.Count; i++)
        {
            var ourParameter = typeInfo.Parameters[i].TypeRef.TypeInfo;
            var theirParameter = functionTypeInfo.Parameters[i].TypeRef.TypeInfo;
            if (ourParameter.Compare(this, theirParameter))
            {
                return false;
            }
        }

        return false;
    }

    public bool CompareFunctionParameterTypeInfo(FunctionParameterTypeInfo typeInfo, TypeInfo other)
    {
        if (other is not FunctionParameterTypeInfo functionParameterTypeInfo)
        {
            return false;
        }

        return !typeInfo.TypeRef.Compare(functionParameterTypeInfo.TypeRef);
    }

    public virtual bool CompareStringTypeInfo(StringTypeInfo typeInfo, TypeInfo other)
    {
        return other is StringTypeInfo;
    }

    public virtual bool CompareNumberTypeInfo(NumberTypeInfo typeInfo, TypeInfo other)
    {
        return other is NumberTypeInfo otherNumberTypeInfo
               && typeInfo.NumberType == otherNumberTypeInfo.NumberType
               && otherNumberTypeInfo.NumberType == typeInfo.NumberType;
    }

    public virtual bool CompareBooleanTypeInfo(BooleanTypeInfo typeInfo, TypeInfo other)
    {
        return other is BooleanTypeInfo;
    }

    public virtual bool CompareObjectTypeInfo(ObjectTypeInfo typeInfo, TypeInfo other)
    {
        return other is ObjectTypeInfo objectTypeInfo && typeInfo.Name == objectTypeInfo.Name;
    }

    public virtual bool CompareGenericTypeInfo(GenericTypeInfo typeInfo, TypeInfo other)
    {
        if (other is not GenericTypeInfo otherGenericTypeInfo)
        {
            return false;
        }

        if (typeInfo.Name != otherGenericTypeInfo.Name ||
            typeInfo.GenericParams.Count != otherGenericTypeInfo.GenericParams.Count)
        {
            return false;
        }

        for (var c = 0; c < typeInfo.GenericParams.Count; c++)
        {
            var ourParam = typeInfo.GenericParams[c];
            var theirParam = otherGenericTypeInfo.GenericParams[c];

            if (!ourParam.TypeInfo.Compare(this, theirParam.TypeInfo))
            {
                return false;
            }
        }

        return true;
    }

    public virtual bool CompareDeferredTypeInfo(DeferredTypeInfo typeInfo, TypeInfo other)
    {
        return other is DeferredTypeInfo;
    }

    public virtual bool CompareStructureTypeInfo(StructureTypeInfo typeInfo, TypeInfo other)
    {
        if (other is not StructureTypeInfo structureTypeInfo)
        {
            return false;
        }

        // In structures, the order of fields is not important, we only care about that the fields found in
        // this is one is also found in the other, it doesn't matter if the other has more fields
        foreach (var typeInfoField in typeInfo.Fields)
        {
            var field = structureTypeInfo.GetField(typeInfoField.Name);

            if (field == null || !Compare(field.TypeRef.TypeInfo, field.TypeRef.TypeInfo))
            {
                return false;
            }
        }

        return true;
    }

    public bool CompareEnumTypeInfo(EnumTypeInfo typeInfo, TypeInfo other)
    {
        if (other is not EnumTypeInfo enumTypeInfo || typeInfo.Name != enumTypeInfo.Name)
        {
            return false;
        }

        if (typeInfo.Fields.Count != enumTypeInfo.Fields.Count)
        {
            return false;
        }

        for (var i = 0; i < typeInfo.Fields.Count; i++)
        {
            var ourItem = typeInfo.Fields[i].TypeRef;
            var theirItem = enumTypeInfo.Fields[i].TypeRef;

            if (!Compare(ourItem.TypeInfo, theirItem.TypeInfo))
            {
                return false;
            }
        }

        return true;
    }

    public bool CompareEnumItemTypeInfo(EnumItemTypeInfo typeInfo, TypeInfo other)
    {
        if (other is not EnumItemTypeInfo enumItemTypeInfo || typeInfo.Name != enumItemTypeInfo.Name)
        {
            return false;
        }

        for (var i = 0; i < typeInfo.Parameters.Count; i++)
        {
            var ourParam = typeInfo.Parameters[i].TypeRef;
            var theirParam = enumItemTypeInfo.Parameters[i].TypeRef;

            if (!Compare(ourParam.TypeInfo, theirParam.TypeInfo))
            {
                return false;
            }
        }

        return true;
    }

    public bool CompareEnumItemParameterTypeInfo(EnumItemParameterTypeInfo typeInfo, TypeInfo other)
    {
        throw new NotImplementedException();
    }

    public bool CompareInlineEnumTypeInfo(InlineEnumTypeInfo typeInfo, TypeInfo other)
    {
        // TODO: Should we allow comparing inline enums with normal enums?
        if (other is not InlineEnumTypeInfo inlineEnumTypeInfo)
        {
            return false;
        }

        for (var i = 0; i < typeInfo.Fields.Count; i++)
        {
            var ourItem = typeInfo.Fields[i].TypeRef;
            var theirItem = inlineEnumTypeInfo.Fields[i].TypeRef;

            if (Compare(ourItem.TypeInfo, theirItem.TypeInfo))
            {
                return true;
            }
        }

        return true;
    }

    public bool CompareBaseEnumTypeInfo(BaseEnumTypeInfo typeInfo, TypeInfo other)
    {
        return typeInfo.Compare(this, other);
    }

    public bool CompareAbstractStructuralTypeInfo(AbstractStructuralTypeInfo typeInfo, TypeInfo other)
    {
        if (other is not AbstractStructuralTypeInfo abstractStructuralTypeInfo)
        {
            return false;
        }

        if (typeInfo.Fields.Count != abstractStructuralTypeInfo.Fields.Count)
        {
            return false;
        }

        for (var f = 0; f < typeInfo.Fields.Count; f++)
        {
            var field = typeInfo.Fields[f];
            var otherField = abstractStructuralTypeInfo.Fields[f];

            if (!field.TypeRef.TypeInfo.Compare(this, otherField.TypeRef.TypeInfo))
            {
                return false;
            }
        }

        return true;
    }

    public bool CompareAbstractStructuralFieldTypeInfo(AbstractStructuralFieldTypeInfo typeInfo, TypeInfo other)
    {
        if (other is not AbstractStructuralFieldTypeInfo abstractStructuralFieldTypeInfo)
        {
            return false;
        }

        return !abstractStructuralFieldTypeInfo.TypeRef.Compare(other);
    }

    public bool CompareFoundationalTypeInfo(FoundationalTypeInfo foundationalTypeInfo, TypeInfo other)
    {
        return foundationalTypeInfo.Compare(this, other);
    }

    public bool CompareScopedTypeInfo(ScopedTypeInfo scopedTypeInfo, TypeInfo other)
    {
        return scopedTypeInfo.Compare(this, other);
    }
}