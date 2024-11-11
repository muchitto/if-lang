using Compiler.Semantics.TypeInformation.Types;

namespace Compiler.Semantics.TypeInformation;

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

    public FunctionTypeInfo VisitFunctionTypeInfo(FunctionTypeInfo functionTypeInfo);

    public EnumTypeInfo VisitEnumTypeInfo(EnumTypeInfo enumTypeInfo);

    public InlineEnumTypeInfo VisitInlineEnumTypeInfo(InlineEnumTypeInfo inlineEnumTypeInfo);

    public EnumItemTypeInfo VisitEnumItemTypeInfo(EnumItemTypeInfo enumItemTypeInfo);

    public AbstractStructuralTypeInfo VisitAbstractStructuralTypeInfo(
        AbstractStructuralTypeInfo abstractStructuralTypeInfo);

    public AnonymousEnumTypeInfo VisitAnonymousEnumTypeInfo(AnonymousEnumTypeInfo anonymousEnumTypeInfo);

    public FoundationalTypeInfo VisitFoundationalTypeInfo(FoundationalTypeInfo foundationalTypeInfo);

    public ScopedTypeInfo VisitScopedTypeInfo(ScopedTypeInfo scopedTypeInfo);
}