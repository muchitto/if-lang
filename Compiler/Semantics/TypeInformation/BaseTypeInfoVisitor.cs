using Compiler.Semantics.TypeInformation.Types;

namespace Compiler.Semantics.TypeInformation;

public class BaseTypeInfoVisitor : ITypeInfoVisitor
{
    public virtual ObjectTypeInfo VisitObjectTypeInfo(ObjectTypeInfo objectTypeInfo)
    {
        foreach (var (key, value) in objectTypeInfo.Fields)
        {
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

    public StructureTypeInfo VisitStructureTypeInfo(StructureTypeInfo structureTypeInfo)
    {
        foreach (var (key, value) in structureTypeInfo.Fields)
        {
            value.TypeInfo.Accept(this);
        }

        return structureTypeInfo;
    }

    public GenericTypeInfo VisitGenericTypeInfo(GenericTypeInfo genericTypeInfo)
    {
        foreach (var genericParam in genericTypeInfo.GenericParams)
        {
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
        typeInfo.Accept(this);
        return typeInfo;
    }

    public FunctionTypeInfo VisitFunctionTypeInfo(FunctionTypeInfo functionTypeInfo)
    {
        functionTypeInfo.ReturnType.TypeInfo.Accept(this);
        foreach (var param in functionTypeInfo.Parameters)
        {
            param.Value.TypeInfo.Accept(this);
        }

        return functionTypeInfo;
    }
}