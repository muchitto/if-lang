using Compiler.Semantics.TypeInformation.Types;

namespace Compiler.Semantics.TypeInformation.TypeVisitor;

public class DeferredTypesVisitor : BaseTypeInfoVisitor
{
    public bool HasDeferredTypes { get; private set; }

    public override DeferredTypeInfo VisitDeferredTypeInfo(DeferredTypeInfo deferredTypeInfo)
    {
        HasDeferredTypes = true;

        return base.VisitDeferredTypeInfo(deferredTypeInfo);
    }
}