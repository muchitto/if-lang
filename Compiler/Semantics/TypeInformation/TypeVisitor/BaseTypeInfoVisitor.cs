using Compiler.Semantics.TypeInformation.Types;

namespace Compiler.Semantics.TypeInformation.TypeVisitor;

public class BaseTypeInfoVisitor : ITypeInfoVisitor
{
    private readonly List<TypeRef> _visitedTypes = [];

    public virtual ObjectTypeInfo VisitObjectTypeInfo(ObjectTypeInfo objectTypeInfo)
    {
        foreach (var (key, value) in objectTypeInfo.Fields)
        {
            if (IsVisitedAndMark(value))
            {
                continue;
            }

            value.TypeInfo.Accept(this);
        }

        return objectTypeInfo;
    }

    public virtual UnknownTypeInfo VisitUnknownTypeInfo(UnknownTypeInfo unknownTypeInfo)
    {
        return unknownTypeInfo;
    }

    public virtual VoidTypeInfo VisitVoidTypeInfo(VoidTypeInfo voidTypeInfo)
    {
        return voidTypeInfo;
    }

    public virtual StructureTypeInfo VisitStructureTypeInfo(StructureTypeInfo structureTypeInfo)
    {
        foreach (var (key, value) in structureTypeInfo.Fields)
        {
            if (IsVisitedAndMark(value))
            {
                continue;
            }

            value.TypeInfo.Accept(this);
        }

        return structureTypeInfo;
    }

    public virtual GenericTypeInfo VisitGenericTypeInfo(GenericTypeInfo genericTypeInfo)
    {
        foreach (var genericParam in genericTypeInfo.GenericParams)
        {
            if (IsVisitedAndMark(genericParam))
            {
                continue;
            }

            genericParam.TypeInfo.Accept(this);
        }

        return genericTypeInfo;
    }

    public virtual StringTypeInfo VisitStringTypeInfo(StringTypeInfo stringTypeInfo)
    {
        return stringTypeInfo;
    }

    public virtual NumberTypeInfo VisitNumberTypeInfo(NumberTypeInfo numberTypeInfo)
    {
        return numberTypeInfo;
    }

    public virtual BooleanTypeInfo VisitBooleanTypeInfo(BooleanTypeInfo booleanTypeInfo)
    {
        return booleanTypeInfo;
    }

    public virtual DeferredTypeInfo VisitDeferredTypeInfo(DeferredTypeInfo deferredTypeInfo)
    {
        return deferredTypeInfo;
    }

    public virtual TypeInfo VisitTypeInfo(TypeInfo typeInfo)
    {
        return typeInfo switch
        {
            ObjectTypeInfo objectTypeInfo => VisitObjectTypeInfo(objectTypeInfo),
            UnknownTypeInfo unknownTypeInfo => VisitUnknownTypeInfo(unknownTypeInfo),
            VoidTypeInfo voidTypeInfo => VisitVoidTypeInfo(voidTypeInfo),
            StructureTypeInfo structureTypeInfo => VisitStructureTypeInfo(structureTypeInfo),
            GenericTypeInfo genericTypeInfo => VisitGenericTypeInfo(genericTypeInfo),
            StringTypeInfo stringTypeInfo => VisitStringTypeInfo(stringTypeInfo),
            NumberTypeInfo numberTypeInfo => VisitNumberTypeInfo(numberTypeInfo),
            BooleanTypeInfo booleanTypeInfo => VisitBooleanTypeInfo(booleanTypeInfo),
            DeferredTypeInfo deferredTypeInfo => VisitDeferredTypeInfo(deferredTypeInfo),
            FunctionTypeInfo functionTypeInfo => VisitFunctionTypeInfo(functionTypeInfo),
            EnumTypeInfo enumTypeInfo => VisitEnumTypeInfo(enumTypeInfo),
            InlineEnumTypeInfo inlineEnumTypeInfo => VisitInlineEnumTypeInfo(inlineEnumTypeInfo),
            EnumItemTypeInfo enumItemTypeInfo => VisitEnumItemTypeInfo(enumItemTypeInfo),
            _ => throw new NotImplementedException()
        };
    }

    public virtual FunctionTypeInfo VisitFunctionTypeInfo(FunctionTypeInfo functionTypeInfo)
    {
        functionTypeInfo.ReturnType.TypeInfo.Accept(this);
        foreach (var (key, value) in functionTypeInfo.Parameters)
        {
            if (IsVisitedAndMark(value))
            {
                continue;
            }

            value.TypeInfo.Accept(this);
        }

        return functionTypeInfo;
    }

    public virtual EnumTypeInfo VisitEnumTypeInfo(EnumTypeInfo enumTypeInfo)
    {
        foreach (var value in enumTypeInfo.Items.Values)
        {
            if (IsVisitedAndMark(value))
            {
                continue;
            }

            value.TypeInfo.Accept(this);
        }

        return enumTypeInfo;
    }

    public virtual InlineEnumTypeInfo VisitInlineEnumTypeInfo(InlineEnumTypeInfo inlineEnumTypeInfo)
    {
        foreach (var (key, value) in inlineEnumTypeInfo.Items)
        {
            if (IsVisitedAndMark(value))
            {
                continue;
            }

            value.TypeInfo.Accept(this);
        }

        return inlineEnumTypeInfo;
    }

    public virtual EnumItemTypeInfo VisitEnumItemTypeInfo(EnumItemTypeInfo enumItemTypeInfo)
    {
        foreach (var parameter in enumItemTypeInfo.Parameters.Values)
        {
            if (IsVisitedAndMark(parameter))
            {
                continue;
            }

            parameter.TypeInfo.Accept(this);
        }

        return enumItemTypeInfo;
    }

    private bool IsVisitedAndMark(TypeRef typeRef)
    {
        if (_visitedTypes.Contains(typeRef))
        {
            return true;
        }

        _visitedTypes.Add(typeRef);
        return false;
    }
}