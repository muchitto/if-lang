using System.Diagnostics;
using Compiler.Semantics.TypeInformation.Types;

namespace Compiler.Semantics.TypeInformation.TypeVisitor;

public class BaseTypeInfoVisitor : ITypeInfoVisitor
{
    private readonly HashSet<TypeRef> _visitedTypes = [];

    public virtual ObjectTypeInfo VisitObjectTypeInfo(ObjectTypeInfo objectTypeInfo)
    {
        VisitTypeRefs(objectTypeInfo.Fields);
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
        VisitTypeRefs(structureTypeInfo.Fields);
        return structureTypeInfo;
    }

    public virtual GenericTypeInfo VisitGenericTypeInfo(GenericTypeInfo genericTypeInfo)
    {
        VisitTypeRefs(genericTypeInfo.GenericParams);
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
        VisitTypeRef(functionTypeInfo.ReturnType);
        VisitTypeRefs(functionTypeInfo.Parameters);
        return functionTypeInfo;
    }

    public virtual EnumTypeInfo VisitEnumTypeInfo(EnumTypeInfo enumTypeInfo)
    {
        return enumTypeInfo;
    }

    public virtual InlineEnumTypeInfo VisitInlineEnumTypeInfo(InlineEnumTypeInfo inlineEnumTypeInfo)
    {
        VisitTypeRefs(inlineEnumTypeInfo.Fields);
        return inlineEnumTypeInfo;
    }

    public virtual EnumItemTypeInfo VisitEnumItemTypeInfo(EnumItemTypeInfo enumItemTypeInfo)
    {
        VisitTypeRefs(enumItemTypeInfo.Parameters);
        return enumItemTypeInfo;
    }

    public virtual AbstractStructuralTypeInfo VisitAbstractStructuralTypeInfo(
        AbstractStructuralTypeInfo abstractStructuralTypeInfo
    )
    {
        return abstractStructuralTypeInfo switch
        {
            EnumTypeInfo enumTypeInfo => VisitEnumTypeInfo(enumTypeInfo),
            ObjectTypeInfo objectTypeInfo => VisitObjectTypeInfo(objectTypeInfo),
            StructureTypeInfo structureTypeInfo => VisitStructureTypeInfo(structureTypeInfo),
            _ => abstractStructuralTypeInfo
        };
    }

    public AbstractStructuralFieldTypeInfo VisitAbstractStructuralFieldTypeInfo(
        AbstractStructuralFieldTypeInfo abstractStructuralFieldTypeInfo
    )
    {
        throw new NotImplementedException();
    }

    public virtual FoundationalTypeInfo VisitFoundationalTypeInfo(FoundationalTypeInfo foundationalTypeInfo)
    {
        return foundationalTypeInfo;
    }

    public virtual ScopedTypeInfo VisitScopedTypeInfo(ScopedTypeInfo scopedTypeInfo)
    {
        return scopedTypeInfo;
    }

    [DebuggerHidden]
    private bool IsVisitedAndMark(TypeRef typeRef)
    {
        // Returns true if typeRef was already visited
        return !_visitedTypes.Add(typeRef);
    }

    [DebuggerHidden]
    [StackTraceHidden]
    private void VisitTypeRef(TypeRef typeRef)
    {
        if (IsVisitedAndMark(typeRef))
        {
            return;
        }

        typeRef.TypeInfo.Accept(this);
    }

    [DebuggerHidden]
    [StackTraceHidden]
    private void VisitTypeRefs(IEnumerable<TypeRef> typeRefs)
    {
        foreach (var typeRef in typeRefs)
        {
            VisitTypeRef(typeRef);
        }
    }

    [DebuggerHidden]
    [StackTraceHidden]
    private void VisitTypeRefs(IEnumerable<NameTypeRefData> typeRefDatas)
    {
        foreach (var nameTypeRefData in typeRefDatas)
        {
            VisitTypeRef(nameTypeRefData.TypeRef);
        }
    }
}