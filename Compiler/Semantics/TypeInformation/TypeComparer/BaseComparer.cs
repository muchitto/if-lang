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

    public virtual bool CompareStringTypeInfo(StringTypeInfo typeInfo, TypeInfo other)
    {
        return other is StringTypeInfo;
    }

    public virtual bool CompareNumberTypeInfo(NumberTypeInfo typeInfo, TypeInfo other)
    {
        return other is NumberTypeInfo numberTypeInfo && typeInfo.NumberType == numberTypeInfo.NumberType;
    }

    public virtual bool CompareBooleanTypeInfo(BooleanTypeInfo typeInfo, TypeInfo other)
    {
        return other is BooleanTypeInfo;
    }

    public virtual bool CompareObjectTypeInfo(ObjectTypeInfo typeInfo, TypeInfo other)
    {
        return other is ObjectTypeInfo objectTypeInfo && typeInfo.Name == objectTypeInfo.Name;
    }

    public virtual bool CompareGenericType(GenericTypeInfo typeInfo, TypeInfo other)
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

        return typeInfo.Compare(this, other);
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
        foreach (var (key, value) in typeInfo.Fields)
        {
            if (!structureTypeInfo.Fields.TryGetValue(key, out var otherValue) ||
                !Compare(value.TypeInfo, otherValue.TypeInfo))
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
            var ourItem = typeInfo.Fields.ElementAt(i).Value;
            var theirItem = enumTypeInfo.Fields.ElementAt(i).Value;

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
            var ourParam = typeInfo.Parameters.ElementAt(i);
            var theirParam = enumItemTypeInfo.Parameters.ElementAt(i);

            if (!Compare(ourParam.Value.TypeInfo, theirParam.Value.TypeInfo))
            {
                return false;
            }
        }

        return true;
    }

    public bool CompareInlineEnumTypeInfo(InlineEnumTypeInfo typeInfo, TypeInfo other)
    {
        // TODO: Should we allow comparing inline enums with normal enums?
        if (other is not InlineEnumTypeInfo inlineEnumTypeInfo)
        {
            return false;
        }

        for (var i = 0; i < typeInfo.Items.Count; i++)
        {
            var ourItem = typeInfo.Items.ElementAt(i);
            var theirItem = inlineEnumTypeInfo.Items.ElementAt(i);

            if (Compare(ourItem.Value.TypeInfo, theirItem.Value.TypeInfo))
            {
                return true;
            }
        }

        return true;
    }

    public bool CompareAnonymousEnumTypeInfo(AnonymousEnumTypeInfo typeInfo, TypeInfo other)
    {
        if (other is not AnonymousEnumTypeInfo otherAnonymousTypeInfo)
        {
            return false;
        }

        foreach (var (name, value) in typeInfo.Fields)
        {
            if (otherAnonymousTypeInfo.Fields.TryGetValue(name, out var otherField)
                && value.TypeInfo.Compare(this, otherField.TypeInfo))
            {
                continue;
            }

            return false;
        }

        return true;
    }
}