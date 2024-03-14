using Compiler.TypeInformation.Types;

namespace Compiler.TypeInformation;

public interface ITypeInfoVisitor
{
    public ObjectTypeInfo VisitObjectTypeInfo(ObjectTypeInfo objectTypeInfo);
    public UnknownTypeInfo VisitUnknownTypeInfo(UnknownTypeInfo unknownTypeInfo);
    public VoidTypeInfo VisitVoidTypeInfo(VoidTypeInfo voidTypeInfo);
    public StructureTypeInfo VisitStructureTypeInfo(StructureTypeInfo structureTypeInfo);

    public GenericTypeInfo VisitGenericTypeInfo(GenericTypeInfo genericTypeInfo);
    public StringTypeInfo VisitStringTypeInfo(StringTypeInfo stringTypeInfo);
    public NumberTypeInfo VisitNumberTypeInfo(NumberTypeInfo numberTypeInfo);
    public BooleanTypeInfo VisitBooleanTypeInfo(BooleanTypeInfo booleanTypeInfo);
    public DeferredTypeInfo VisitDeferredTypeInfo(DeferredTypeInfo deferredTypeInfo);
    public TypeInfo VisitTypeInfo(TypeInfo typeInfo);
}